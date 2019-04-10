using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.LSM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Contract.LSM;
using ServiceCenter.MES.Service.LSM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NHibernate;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using ServiceCenter.Common;

namespace ServiceCenter.MES.Service.LSM
{
    
    public partial class MaterialReceiptService : IMaterialReceiptContract
    {
        /// <summary>
        /// 添加领料单。
        /// </summary>
        /// <param name="obj">领料单数据表头</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult<MaterialReceipt> AddMaterialReceipt(MaterialReceipt obj)
        {
            MethodReturnResult<MaterialReceipt> result = new MethodReturnResult<MaterialReceipt>()
            {
                Code = 0,
                Message = ""
            };

            if(string.IsNullOrEmpty(obj.Key)==false)
            { 
                //判断领料单是否存在。
                if( this.MaterialReceiptDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(@"领料单{0}已存在.", obj.Key);
                    return result;
                }
            }else
            {
                obj.Key = GenerateReceiptNo(null);
                //判断领料单是否存在。
                if (this.MaterialReceiptDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1002;
                    result.Message = String.Format(@"系统自动生成领料单编号{0}失败.", obj.Key);
                    return result;
                }
            }

            //判断工单号是否存在。
            if (!this.WorkOrderDataEngine.IsExists(obj.OrderNumber))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialReceiptService_WorkOrderIsNotExists
                                                , obj.OrderNumber);
                return result;
            }

            #region //开始事物处理
            ISession db = this.WorkOrderDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = db.BeginTransaction();
            try
            {
               
                obj.State = EnumReceiptState.Created;
                result.Data = obj;
                //新增领料单。
                this.MaterialReceiptDataEngine.Insert(obj,db);
                transaction.Commit();
                db.Close();
            }
            catch (Exception err)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += err.Message;
                return result;
            }
            #endregion

            return result;
        }


        public MethodReturnResult<MaterialReceipt> DeleteMaterialReceipt(string materialReceiptNo)
        {
            MethodReturnResult<MaterialReceipt> result = new MethodReturnResult<MaterialReceipt>()
            {
                Code = 0,
                Message = ""
            };

            //判断领料单是否存在。
            MaterialReceipt  materialReceipt = this.MaterialReceiptDataEngine.Get(materialReceiptNo);
            if (materialReceipt==null)
            {
                result.Code = 1001;
                result.Message = String.Format(@"领料单{0}不存在.", materialReceiptNo);
                return result;
            }

            if(materialReceipt.State==EnumReceiptState.Approved)
            {
                result.Code = 1002;
                result.Message = String.Format(@"领料单{0}已审批，不能删除.", materialReceiptNo);
                return result;
            }

            PagingConfig cfg = new PagingConfig()
            {
                Where = string.Format("Key.ReceiptNo='{0}' ", materialReceiptNo),
                IsPaging=false
            };
            IList<MaterialReceiptDetail> lstMaterialReceiptDetail = this.MaterialReceiptDetailDataEngine.Get(cfg);

            #region //开始事物处理
            ISession db = this.WorkOrderDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = db.BeginTransaction();
            try
            {
                foreach (MaterialReceiptDetail materialReceiptDetail in lstMaterialReceiptDetail)
                {
                    this.MaterialReceiptDetailDataEngine.Delete(materialReceiptDetail.Key, db);
                }
                this.MaterialReceiptDataEngine.Delete(materialReceiptNo, db);
                transaction.Commit();
                db.Close();
            }
            catch (Exception err)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += err.Message;
                return result;
            }
            #endregion

            return result;
        }

        protected string GenerateReceiptNo(ISession db)
        {
            if(db==null)
            {
                db = this.LineStoreMaterialDataEngine.SessionFactory.OpenSession();
            }

            string strReceiptNo = "";
            string prefix = string.Format("LMK{0:yyMMdd}", DateTime.Now);
            int itemNo = 0;
           
            PagingConfig cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format("Key LIKE '{0}%'", prefix),
                OrderBy = "Key Desc"
            };
            IList<MaterialReceipt> lstMaterialReceipt= this.MaterialReceiptDataEngine.Get(cfg,db);

            if (lstMaterialReceipt != null && lstMaterialReceipt.Count>0)
            {
                string sItemNo = lstMaterialReceipt[0].Key.Replace(prefix, "");
                int.TryParse(sItemNo, out itemNo);
            }
            strReceiptNo = prefix + (itemNo + 1).ToString("0000");
            return strReceiptNo;
        }

        public MethodReturnResult AddMaterialReceiptDetail(MaterialReceiptDetailParamter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            System.Data.DataRow dRowOfBox = null;

            //判断领料单是否存在。
            MaterialReceipt materialReceipt = this.MaterialReceiptDataEngine.Get(p.ReceiptNo);
            if (materialReceipt == null)
            {
                result.Code = 1001;
                result.Message = String.Format(@"领料单{0}不存在.", p.ReceiptNo);
                return result;
            }

            if (materialReceipt.State == EnumReceiptState.Approved)
            {
                result.Code = 1002;
                result.Message = String.Format(@"领料单{0}已审批.", p.ReceiptNo);
                return result;
            }

            if(p.IsReceiptOfCell)
            {
                //需要从电池MES获取箱信息
                try
                {
                    System.Data.DataSet dsOfBox = null;
                    using (DbConnection con = this._dbCell.CreateConnection())
                    {
                        DbCommand cmd = con.CreateCommand();
                        cmd.CommandText = string.Format(@" select * from V_Package_Info where PACKAGE_NO='{0}'", p.MaterialLotNumber);
                        dsOfBox = this._dbCell.ExecuteDataSet(cmd);
                        if(dsOfBox!=null && dsOfBox.Tables.Count>0)
                        {
                            System.Data.DataTable dt = dsOfBox.Tables[0];
                            if(dt.Rows.Count>0)
                            {
                                dRowOfBox = dt.Rows[0];
                            }
                        }
                    }
                    if(dRowOfBox==null)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("箱号{0}在电池MES中不存在",p.MaterialLotNumber);
                        return result;
                    }

                    MaterialReceiptDetail materialDetail = new MaterialReceiptDetail
                    {
                        LineStoreName = materialReceipt.LineStore,
                        MaterialCode = dRowOfBox["MATERIAL_CODE"].ToString(),
                        MaterialLot = p.MaterialLotNumber,
                        Key = new MaterialReceiptDetailKey() 
                        { 
                            ReceiptNo=p.ReceiptNo,
                            ItemNo=0
                        },
                        SupplierMaterialLot = p.MaterialLotNumber,
                        SupplierCode = "000000",
                        Qty = Double.Parse(dRowOfBox["PACKAGE_QTY"].ToString()),
                        Attr1 = string.Format("{0:00.0}0",dRowOfBox["EFFI_NAME"].ToString()),
                        Attr2 = dRowOfBox["COLOR"].ToString(),
                        Attr3 = "",
                        Attr4 = "",
                        Attr5 = "",
                        Creator = p.Creator,
                        Editor = p.Creator,
                        CreateTime = System.DateTime.Now,
                        EditTime = System.DateTime.Now
                    };

                    //判断物料编码在工单BOM中是否存在。
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format("Key.OrderNumber='{0}' AND MaterialCode='{1}'"
                                              , materialReceipt.OrderNumber
                                              , materialDetail.MaterialCode),
                        PageNo = 0,
                        PageSize = 1
                    };
                    IList<WorkOrderBOM> lstBOM = this.WorkOrderBOMDataEngine.Get(cfg);
                    if (lstBOM.Count == 0)
                    {
                        result.Code = 2003;
                        result.Message = String.Format(StringResource.MaterialReceiptService_MaterialCodeIsNotExists
                                                        , string.Format("{0}-{1}", materialReceipt.OrderNumber, materialDetail.MaterialCode)
                                                        , materialDetail.Key.ItemNo);
                        return result;
                    }
                    //检查箱号
                    cfg = new PagingConfig()
                    {
                        //State='0' AND
                        Where = string.Format(@" 1=1 AND
                                        EXISTS(  FROM MaterialReceiptDetail as p 
                                                        WHERE p.Key.ReceiptNo=self.Key 
                                                        AND p.MaterialLot ='{1}'
                                                )", p.ReceiptNo, materialDetail.MaterialLot),
                        PageNo = 0,
                        PageSize = 1
                    };
                    IList<MaterialReceipt> lstMaterialReceipt= this.MaterialReceiptDataEngine.Get(cfg);
                    if (lstMaterialReceipt != null && lstMaterialReceipt.Count>0)
                    {
                        result.Code = 1003;
                        result.Message = String.Format("箱号已扫入到领料单{0}中,不允许重复扫", lstMaterialReceipt[0].Key);
                        return result;
                    }
                    result = ExecAddMaterialReceiptDetail(materialDetail, null, false);
                }
                catch (Exception ex)
                {
                    result.Code = 1000;
                    result.Message = ex.Message;
                    result.Detail = ex.ToString();
                    return result;
                }

            }
            return result;
        }


        public MethodReturnResult DeleteMaterialReceiptDetail(MaterialReceiptDetailParamter p)
        {

            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0,
                Message = ""
            };

            //判断领料单是否存在。
            MaterialReceipt materialReceipt = this.MaterialReceiptDataEngine.Get(p.ReceiptNo);
            if (materialReceipt == null)
            {
                result.Code = 1001;
                result.Message = String.Format(@"领料单{0}不存在.", p.ReceiptNo);
                return result;
            }

            if (materialReceipt.State == EnumReceiptState.Approved)
            {
                result.Code = 1002;
                result.Message = String.Format(@"领料单{0}已审批，不能删除.", p.ReceiptNo);
                return result;
            }

            PagingConfig cfg = new PagingConfig()
            {
                Where = string.Format("Key.ReceiptNo='{0}' and MaterialLot='{1}' ", p.ReceiptNo,p.MaterialLotNumber),
                IsPaging = false
            };
            IList<MaterialReceiptDetail> lstMaterialReceiptDetail = this.MaterialReceiptDetailDataEngine.Get(cfg);

            #region //开始事物处理
            ISession db = this.WorkOrderDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = db.BeginTransaction();
            try
            {
                foreach (MaterialReceiptDetail materialReceiptDetail in lstMaterialReceiptDetail)
                {
                    this.MaterialReceiptDetailDataEngine.Delete(materialReceiptDetail.Key, db);
                }
                transaction.Commit();
                db.Close();
            }
            catch (Exception err)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += err.Message;
                return result;
            }
            #endregion
            return result;
        }

        protected MethodReturnResult ExecAddMaterialReceiptDetail(MaterialReceiptDetail materialReceiptDetail, ISession db, bool executedWithTransaction)
        {
            MethodReturnResult result = new MethodReturnResult();
            ITransaction transaction = null;

            if (executedWithTransaction == false)
            {
                db = this.SessionFactory.OpenSession();
                transaction = db.BeginTransaction();
            }
            try
            { 
                //检查箱号
                PagingConfig cfg = new PagingConfig()
                {
                    Where = string.Format(@" Key.ReceiptNo='{0}'", materialReceiptDetail.Key.ReceiptNo),
                    PageNo = 0,
                    PageSize = 1,
                    OrderBy = " Key.ItemNo desc "
                };
                int nItemNo = 0;
                IList<MaterialReceiptDetail> lstMaterialReceiptDetail = this.MaterialReceiptDetailDataEngine.Get(cfg);
                if (lstMaterialReceiptDetail != null && lstMaterialReceiptDetail.Count>0)
                {
                    nItemNo = lstMaterialReceiptDetail[0].Key.ItemNo + 1;
                }else
                {
                    nItemNo = 1;
                }
                MaterialReceiptDetailKey mKey = new MaterialReceiptDetailKey
                {
                    ReceiptNo = materialReceiptDetail.Key.ReceiptNo,
                    ItemNo = nItemNo
                };
                materialReceiptDetail.Key = mKey;

                this.MaterialReceiptDetailDataEngine.Insert(materialReceiptDetail,db);

                if (executedWithTransaction == false)
                {
                    transaction.Commit();
                    db.Close();
                }
                else
                {
                    db.Flush();
                }
            }
            catch (Exception err)
            {
                if (executedWithTransaction == false)
                {
                    transaction.Rollback();
                    db.Close();
                }
                result.Code = 1000;
                result.Message +=  err.Message;
                return result;
            }
            return result;
        }


        public MethodReturnResult ApproveMaterialReceipt(MaterialReceiptParamter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0,
                Message = ""
            };

            //判断领料单是否存在。
            MaterialReceipt materialReceipt = this.MaterialReceiptDataEngine.Get(p.ReceiptNo);
            if (materialReceipt == null)
            {
                result.Code = 1001;
                result.Message = String.Format(@"领料单{0}不存在.", p.ReceiptNo);
                return result;
            }

            if (materialReceipt.State == EnumReceiptState.Approved)
            {
                result.Code = 1002;
                result.Message = String.Format(@"领料单{0}已审批，不能再审批.", p.ReceiptNo);
                return result;
            }

            PagingConfig cfg = new PagingConfig()
            {
                Where = string.Format("Key.ReceiptNo='{0}' ", p.ReceiptNo),
                IsPaging = false
            };
            IList<MaterialReceiptDetail> lstMaterialReceiptDetail = this.MaterialReceiptDetailDataEngine.Get(cfg);

            List<LineStoreMaterial> lstLineStoreMaterialForInsert = new List<LineStoreMaterial>();
            List<LineStoreMaterial> lstLineStoreMaterialForUpdate = new List<LineStoreMaterial>();
            List<LineStoreMaterialDetail> lstLineStoreMaterialDetailForInsert = new List<LineStoreMaterialDetail>();
            List<LineStoreMaterialDetail> lstLineStoreMaterialDetailForUpdate = new List<LineStoreMaterialDetail>();

            var lstLineStoreMaterial = from item in lstMaterialReceiptDetail
                               group item  by  new 
                               {
                                   item.LineStoreName,
                                   item.MaterialCode
                               } into g
                               select new 
                               {    
                                   g.Key.LineStoreName,
                                   g.Key.MaterialCode
                                   //totalQty= g.Sum(item=>item.Qty)
                               };
            System.DateTime dNow = System.DateTime.Now;            
            foreach (var lineStoreMaterial in lstLineStoreMaterial)
            {
                #region 更新线边仓表头
                LineStoreMaterialKey lsmKey = new LineStoreMaterialKey()
                {
                    LineStoreName = lineStoreMaterial.LineStoreName,
                    MaterialCode = lineStoreMaterial.MaterialCode
                };
                LineStoreMaterial lsm = this.LineStoreMaterialDataEngine.Get(lsmKey);

                //如果对应线边仓中无物料数据，则新增线边仓物料数据。

                if (lsm == null)
                {
                    lsm = new LineStoreMaterial()
                    {
                        Key = lsmKey,
                        CreateTime = dNow,
                        Creator = p.Creator,
                        Editor = p.Creator,
                        EditTime = dNow
                    };
                    lstLineStoreMaterialForInsert.Add(lsm);
                    //this.LineStoreMaterialDataEngine.Insert(lsm);
                }
                else
                {//更新线边仓物料数据。
                    LineStoreMaterial lsmUpdate = new LineStoreMaterial()
                    {
                        Key = lsmKey,
                        CreateTime = dNow,
                        Creator = p.Creator,
                        Editor = p.Creator,
                        EditTime = dNow
                    };
                    lstLineStoreMaterialForUpdate.Add(lsmUpdate);
                }
                #endregion
            }
          
            foreach (MaterialReceiptDetail item in lstMaterialReceiptDetail)
            {
                #region  //新增线边仓明细数据
                LineStoreMaterialDetailKey lsmdKey = new LineStoreMaterialDetailKey()
                {
                    LineStoreName = item.LineStoreName,
                    OrderNumber =   materialReceipt.OrderNumber,
                    MaterialCode = item.MaterialCode,
                    MaterialLot = item.MaterialLot
                };
                LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(lsmdKey);
                //如果对应线边仓中无物料明细数据，则新增线边仓物料明细数据。
                if (lsmd == null)
                {
                    lsmd = new LineStoreMaterialDetail()
                    {
                        Key = lsmdKey,
                        CurrentQty = item.Qty,
                        ReceiveQty = item.Qty,
                        LoadingQty = 0,
                        UnloadingQty = 0,
                        ReturnQty = 0,
                        ScrapQty = 0,
                        Attr1 = item.Attr1,
                        Attr2 = item.Attr2,
                        Attr3 = item.Attr3,
                        Attr4 = item.Attr4,
                        Attr5 = item.Attr5,
                        SupplierMaterialLot = item.SupplierMaterialLot,
                        SupplierCode = item.SupplierCode,
                        Description = item.Description,
                        CreateTime = dNow,
                        Creator = p.Creator,
                        Editor = p.Creator,
                        EditTime = dNow
                    };
                    //this.LineStoreMaterialDetailDataEngine.Insert(lsmd);
                    lstLineStoreMaterialDetailForInsert.Add(lsmd);
                }
                else
                {
                    LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
                    //更新线边仓物料明细数据。
                    lsmdUpdate.ReceiveQty += item.Qty;
                    lsmdUpdate.CurrentQty += item.Qty;
                    lsmdUpdate.EditTime = dNow;
                    //this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate);
                    lstLineStoreMaterialDetailForUpdate.Add(lsmdUpdate);
                }
                #endregion
            }


            ISession db = this.SessionFactory.OpenSession();
            ITransaction transaction = db.BeginTransaction();
            try
            {
                #region //开始事物处理

                //更新批次LineStoreMaterial信息
                foreach (LineStoreMaterial obj in lstLineStoreMaterialForInsert)
                {
                    this.LineStoreMaterialDataEngine.Insert(obj, db);
                }


                //更新批次LineStoreMaterial信息
                foreach (LineStoreMaterial obj in lstLineStoreMaterialForUpdate)
                {
                    this.LineStoreMaterialDataEngine.Update(obj, db);
                }

                //更新批次LineStoreMaterialDetail信息
                foreach (LineStoreMaterialDetail obj in lstLineStoreMaterialDetailForInsert)
                {
                    this.LineStoreMaterialDetailDataEngine.Insert(obj, db);
                }

                //更新批次LineStoreMaterialDetail信息
                foreach (LineStoreMaterialDetail obj in lstLineStoreMaterialDetailForUpdate)
                {
                    this.LineStoreMaterialDetailDataEngine.Update(obj, db);
                }

                MaterialReceipt materialReceiptForUpdate = (MaterialReceipt)materialReceipt.Clone();
                materialReceiptForUpdate.State = EnumReceiptState.Approved;
                materialReceiptForUpdate.Editor = p.Creator;
                materialReceiptForUpdate.EditTime = dNow;

                this.MaterialReceiptDataEngine.Update(materialReceiptForUpdate, db);

                transaction.Commit();
                db.Close();
                #endregion
            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("ApproveMaterialReceipt>", err);
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message +=  err.Message;
                result.Detail = err.ToString();
            }
            return result;
        }


        public MethodReturnResult<MaterialReceipt> ModifyMaterialReceipt(MaterialReceipt obj)
        {


            MethodReturnResult<MaterialReceipt> result = new MethodReturnResult<MaterialReceipt>()
            {
                Code = 0,
                Message = ""
            };

            //判断领料单是否存在。
            MaterialReceipt materialReceipt = this.MaterialReceiptDataEngine.Get(obj.Key);
            if (materialReceipt == null)
            {
                result.Code = 1001;
                result.Message = String.Format(@"领料单{0}不存在.", obj.Key);
                return result;
            }

            if (materialReceipt.State == EnumReceiptState.Approved)
            {
                result.Code = 1002;
                result.Message = String.Format(@"领料单{0}已审批，不能修改.", obj.Key);
                return result;
            }

            PagingConfig cfg = new PagingConfig()
            {
                Where = string.Format("Key.ReceiptNo='{0}' ", obj.Key),
                IsPaging = false
            };
            IList<MaterialReceiptDetail> lstMaterialReceiptDetail = this.MaterialReceiptDetailDataEngine.Get(cfg);

            //判断领料单中是否有明细。
            if (lstMaterialReceiptDetail.Count > 0)
            {
                result.Code = 1002;
                result.Message = String.Format("领料单{0}中已经有明细存在，禁止修改!"
                                                , obj.Key);
                return result;
            }
            //判断工单号是否存在。
            if (!this.WorkOrderDataEngine.IsExists(obj.OrderNumber))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialReceiptService_WorkOrderIsNotExists
                                                , obj.OrderNumber);
                return result;
            }

            #region //开始事物处理
            ISession db = this.WorkOrderDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = db.BeginTransaction();
            try
            {
                obj.State = EnumReceiptState.Created;
                //Create the ReceiptNo
                //obj.Key = GenerateReceiptNo(db);
                result.Data = obj;
                //修改领料单。
                this.MaterialReceiptDataEngine.Modify(obj, db);
                transaction.Commit();
                db.Close();
            }
            catch (Exception err)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += err.Message;
                return result;
            }
            #endregion

            return result;
        }

    }
}
