using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System.ServiceModel.Activation;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using NHibernate;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.Common.DataAccess.NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.Service.Contract.ZPVM;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.SqlClient;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Contract.ERP;
using ServiceCenter.MES.Service.Class.COMMON;

namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现包装入柜服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PackageInChestService : IPackageInChestContract, IPackageInChestCheck
    {
        #region 定义全局变量
        string localName = System.Configuration.ConfigurationSettings.AppSettings["LocalName"];       
        #endregion

        #region 定义数据库实例

        protected Database _db;
        protected Database query_db;

        public ISessionFactory SessionFactory { get; set; }

        #endregion

        #region 构造函数
        public PackageInChestService(ISessionFactory sf)
        {
            this.SessionFactory = sf;
            this.query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
            this._db = DatabaseFactory.CreateDatabase();
            this.RegisterCheckInstance(this);
        }

        #endregion

        #region 定义数据访问对象

        //柜动作日志数据访问对象
        public IChestLogDataEngine ChestLogDataEngine { get; set; }

        // 产品成柜规则数据访问对象
        public IMaterialChestParameterDataEngine MaterialChestParameterDataEngine { get; set; }

        // 批次IV测试数据访问对象
        public IIVTestDataDataEngine IVTestDataDataEngine { get; set; }

        // 工单等级包装规则数据访问对象
        public IWorkOrderGradeDataEngine WorkOrderGradeDataEngine { get; set; }

        // 批次数据访问对象
        public ILotDataEngine LotDataEngine { get; set; }

        // 包装数据访问对象
        public IPackageDataEngine PackageDataEngine { get; set; }

        // 柜数据访问对象
        public IChestDataEngine ChestDataEngine { get; set; }

        // 包装明细数据访问对象
        public IPackageDetailDataEngine PackageDetailDataEngine { get; set; }

        // 柜明细数据访问对象
        public IChestDetailDataEngine ChestDetailDataEngine { get; set; }

        // 工序属性数据访问对象
        public IRouteOperationAttributeDataEngine RouteOperationAttributeDataEngine { get; set; }

        // 基础数据值访问对象
        public IBaseAttributeValueDataEngine BaseAttributeValueDataEngine { get; set; }

        // OEM数据值访问对象
        public IOemDataEngine OemDataEngine { get; set; }

        //工单分档数据访问对象
        public IWorkOrderPowersetDataEngine WorkOrderPowersetDataEngine { get; set; }

        // 工单数据访问对象
        public IWorkOrderDataEngine WorkOrderDataEngine { get; set; }

        #endregion

        #region 定于数据访问对象的列表

        List<Chest> lstChestDataForUpdate = new List<Chest>();
        List<Chest> lstChestDataForInsert = new List<Chest>();
        List<ChestDetail> lstChestDetailForInsert = new List<ChestDetail>();
        List<ChestDetail> lstChestDetailForUpdate = new List<ChestDetail>();
        List<ChestDetail> lstChestDetailForDelete = new List<ChestDetail>();

        #endregion

        #region 定义事件
        // 操作前检查事件
        public event Func<ChestParameter, MethodReturnResult> CheckEvent;
        #endregion

        #region 定义事件清单列表
        // 自定义操作前检查的清单列表
        private IList<IPackageInChestCheck> CheckList { get; set; }
        #endregion

        #region 定义事件操作实例
        // 注册自定义检查的操作实例
        public void RegisterCheckInstance(IPackageInChestCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<IPackageInChestCheck>();
            }
            this.CheckList.Add(obj);
        }
        #endregion

        #region 定义事件执行方法
        // 操作前检查
        protected virtual MethodReturnResult OnCheck(ChestParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.CheckEvent != null)
            {
                foreach (Func<ChestParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }
            if (this.CheckList != null)
            {
                foreach (IPackageInChestCheck d in this.CheckList)
                {
                    result = d.Check(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }
            return result;
        }
        
        #endregion               

        #region 已注释
        
        //string creatChestType = System.Configuration.ConfigurationSettings.AppSettings["CreatChestType"];

        #region 注释--原柜号明细查询方法
        //public MethodReturnResult<DataSet> GetChestDetail(ref ChestParameter p)
        //{
        //    string strErrorMessage = string.Empty;
        //    MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
        //    try
        //    {
        //        using (DbConnection con = this._db.CreateConnection())
        //        {
        //            DbCommand cmd = con.CreateCommand();
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.CommandText = "sp_WIP_ChestDetail_list";
        //            this._db.AddInParameter(cmd, "@ChestNo", DbType.String, p.ChestNo);
        //            this._db.AddInParameter(cmd, "@PackageNo", DbType.String, p.PackageNo);
        //            this._db.AddInParameter(cmd, "@MaterialCode", DbType.String, p.MaterialCode);
        //            this._db.AddInParameter(cmd, "@ChestDateStart", DbType.String, p.ChestDateStart);
        //            this._db.AddInParameter(cmd, "@ChestDateEnd", DbType.String, p.ChestDateEnd);
        //            this._db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
        //            this._db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);

        //            //返回总记录数
        //            this._db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
        //            cmd.Parameters["@Records"].Direction = ParameterDirection.Output;

        //            //错误信息
        //            cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
        //            cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

        //            //返回参数
        //            SqlParameter parReturn = new SqlParameter("@return_value", SqlDbType.Int);
        //            parReturn.Direction = ParameterDirection.ReturnValue;
        //            cmd.Parameters.Add(parReturn);

        //            cmd.CommandTimeout = 120;

        //            //执行
        //            result.Data = this._db.ExecuteDataSet(cmd);

        //            //返回总记录数
        //            p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);
        //            int i = (int)cmd.Parameters["@return_value"].Value;

        //            if (i == -1)
        //            {
        //                strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
        //                result.Code = 1000;
        //                result.Message = strErrorMessage;
        //                result.Detail = strErrorMessage;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Code = 1000;
        //        result.Message = ex.Message;
        //        result.Detail = ex.ToString();
        //    }
        //    return result;
        //}
        #endregion

        #region 注释--自动生成正式包装号
        //public string Generate(string chestNo)
        //{
        //    string minChestNo = "";
        //    string maxChestNo = "";
        //    string prefixChestNo = "";
        //    int seqNo = 1;
        //    PagingConfig cfg = new PagingConfig();
        //    if (creatChestType == "05")
        //    {
        //        #region 协鑫永能P660柜号生成规则
        //        prefixChestNo = string.Format("0000"); 
        //        if (localName == "G01")
        //        {
        //            seqNo = 840;
        //            //晋中柜号流水码限制
        //            minChestNo = string.Format("{0}0840", prefixChestNo);
        //            maxChestNo = string.Format("{0}0870", prefixChestNo);
        //        }
        //        if (localName == "K01")
        //        {
        //            seqNo = 871;
        //            //文水柜号流水码限制
        //            minChestNo = string.Format("{0}0871", prefixChestNo);
        //            maxChestNo = string.Format("{0}0910", prefixChestNo);
        //        }

        //        cfg = new PagingConfig()
        //        {
        //            PageNo = 0,
        //            PageSize = 1,
        //            Where = string.Format(@"Key >= '{0}' AND Key < '{1}'"
        //                                                , minChestNo
        //                                                , maxChestNo),
        //            OrderBy = "Key DESC"
        //        };

        //        IList<Chest> lstChest = this.ChestDataEngine.Get(cfg);

        //        if (lstChest.Count > 0)
        //        {
        //            string maxSeqNo = lstChest[0].Key.Replace(prefixChestNo, "");
        //            if (int.TryParse(maxSeqNo, out seqNo))
        //            {
        //                seqNo = seqNo + 1;
        //            }
        //        }
        //        return string.Format("{0}{1}", prefixChestNo, seqNo.ToString("0000"));
        //        #endregion
        //    }
        //    if (creatChestType == "64")
        //    {
        //        #region 协鑫晋能M660柜号生成规则
        //        prefixChestNo = string.Format("0000");
        //        if (localName == "G01")
        //        {
        //            seqNo = 910;
        //            //晋中柜号流水码限制
        //            minChestNo = string.Format("{0}0910", prefixChestNo);
        //            maxChestNo = string.Format("{0}0940", prefixChestNo);
        //        }
        //        if (localName == "K01")
        //        {
        //            seqNo = 1;
        //            //文水柜号流水码限制
        //            minChestNo = string.Format("{0}0001", prefixChestNo);
        //            maxChestNo = string.Format("{0}0840", prefixChestNo);
        //        }

        //        cfg = new PagingConfig()
        //        {
        //            PageNo = 0,
        //            PageSize = 1,
        //            Where = string.Format(@"Key >= '{0}' AND Key < '{1}' AND MaterialCode <> '2512020201' "
        //                                                , minChestNo
        //                                                , maxChestNo),
        //            OrderBy = "Key DESC"
        //        };

        //        IList<Chest> lstChest = this.ChestDataEngine.Get(cfg);

        //        if (lstChest.Count > 0)
        //        {
        //            string maxSeqNo = lstChest[0].Key.Replace(prefixChestNo, "");
        //            if (int.TryParse(maxSeqNo, out seqNo))
        //            {
        //                seqNo = seqNo + 1;
        //            }
        //        }
        //        return string.Format("{0}{1}", prefixChestNo, seqNo.ToString("0000"));
        //        #endregion
        //    }
        //    else
        //    {
        //        #region 晋能柜号生成规则
        //        prefixChestNo = string.Format("G{0}", DateTime.Now.ToString("yyMMdd"));
        //        cfg = new PagingConfig()
        //        {
        //            PageNo = 0,
        //            PageSize = 1,
        //            Where = string.Format(@"Key LIKE '{0}%'"
        //                                    , prefixChestNo),
        //            OrderBy = "Key DESC"
        //        };

        //        IList<Chest> lstChest = this.ChestDataEngine.Get(cfg);
        //        if (lstChest.Count > 0)
        //        {
        //            string maxSeqNo = lstChest[0].Key.Replace(prefixChestNo, "");
        //            if (int.TryParse(maxSeqNo, out seqNo))
        //            {
        //                seqNo = seqNo + 1;
        //            }
        //        }
        //        return string.Format("{0}{1}", prefixChestNo, seqNo.ToString("000"));
        //        #endregion
        //    }           
        //}
        #endregion

        #region 注释--原柜号生成方法
        //public string GenerateEx(string packageNo, ISession session)
        //{
        //    Package obj = this.PackageDataEngine.Get(packageNo);

        //    if (obj.ContainerNo == "" || obj.ContainerNo == null)
        //    {
        //        string prefixChestNo = string.Format("G{0}", DateTime.Now.ToString("yyMMdd"));

        //        int seqNo = 1;
        //        PagingConfig cfg = new PagingConfig()
        //        {
        //            PageNo = 0,
        //            PageSize = 1,
        //            Where = string.Format(@"Key LIKE '{0}%'"
        //                                    , prefixChestNo),
        //            OrderBy = "Key DESC"
        //        };
        //        IList<Chest> lstChest = this.ChestDataEngine.Get(cfg, session);
        //        if (lstChest.Count > 0)
        //        {
        //            string maxSeqNo = lstChest[0].Key.Replace(prefixChestNo, "");
        //            if (int.TryParse(maxSeqNo, out seqNo))
        //            {
        //                seqNo = seqNo + 1;
        //            }
        //        }
        //        return string.Format("{0}{1}", prefixChestNo, seqNo.ToString("000"));
        //    }
        //    else
        //    {
        //        return obj.ContainerNo;
        //    }
        //}
        #endregion

        #region 注释--临时柜号转正式
        //MethodReturnResult IPackageInChestContract.ChangeChest(string chestNo)
        //{
        //    MethodReturnResult result = new MethodReturnResult();

        //    try
        //    {
        //        string realChestNo = "";
        //        ISession session = null;
        //        ITransaction transaction = null;              
        //        List<Package> newPackageForUpdate = new List<Package>();
        //        Chest newChestForInsert = null;
        //        string oldChestNoForDelete = null;
        //        List<ChestDetail> newChestDetailForInsert = new List<ChestDetail>();   
        //        List<ChestDetailKey> oldChestDetailKeyForDelete = new List<ChestDetailKey>();  

        //        #region 1 将临时柜号变更为正式柜号
        //        // 获取原柜信息
        //        Chest oldChest = this.ChestDataEngine.Get(chestNo);
        //        if (oldChest != null)
        //        {
        //            // 获取原柜明细
        //            PagingConfig cfg = new PagingConfig()
        //            {
        //                Where = string.Format(@"Key.ChestNo='{0}'", chestNo),
        //                OrderBy = "ItemNo desc"
        //            };

        //            #region 柜号变更
        //            IList<ChestDetail> oldChestDetailList = this.ChestDetailDataEngine.Get(cfg);
        //            if (oldChestDetailList != null && oldChestDetailList.Count > 0)
        //            {
        //                Package newPackage = null;
        //                ChestDetail newChestDetail = null;
        //                ChestDetailKey oldChestDetailKey;
        //                foreach (ChestDetail item in oldChestDetailList)
        //                {
        //                    newPackage = this.PackageDataEngine.Get(item.Key.ObjectNumber);
        //                    if (newPackage != null)
        //                    {
        //                        if (realChestNo == "")
        //                        {
        //                            // 生成正式柜号                   
        //                            realChestNo = Generate(newPackage.Key);
        //                        }
        //                        newPackage.ContainerNo = realChestNo;

        //                        // 更新托号的柜号
        //                        newPackageForUpdate.Add(newPackage);

        //                        // 增加新柜号明细及删除旧柜号明细
        //                        newChestDetail = new ChestDetail()
        //                        {
        //                            Key = new ChestDetailKey()
        //                            {
        //                                ChestNo = realChestNo,                             //柜号
        //                                ObjectNumber = item.Key.ObjectNumber,              //托号
        //                                ObjectType = EnumChestObjectType.PackageNo         //PackageNo 包装号
        //                            },
        //                            ItemNo = Convert.ToInt32(item.ItemNo),                 //入柜项目号（入柜顺序）
        //                            Creator = item.Creator,                                //创建人
        //                            CreateTime = item.CreateTime,                          //创建时间                   
        //                            MaterialCode = item.MaterialCode,                      //物料编码
        //                        };
        //                        newChestDetailForInsert.Add(newChestDetail);
        //                        oldChestDetailKey = new ChestDetailKey()
        //                        {
        //                            ChestNo = item.Key.ChestNo,                        //柜号
        //                            ObjectNumber = item.Key.ObjectNumber,              //托号
        //                            ObjectType = EnumChestObjectType.PackageNo         //PackageNo 包装号
        //                        };
        //                        oldChestDetailKeyForDelete.Add(oldChestDetailKey);
        //                    }
        //                    else
        //                    {
        //                        result.Code = 2003;
        //                        result.Message = string.Format("托号[{0}]不存在！", item.Key.ObjectNumber);

        //                        return result;
        //                    }
        //                }
        //                // 创建柜对象及删除旧柜号
        //                newChestForInsert = new Chest()
        //                {
        //                    Key = realChestNo,                          //主键柜号 
        //                    ChestState = oldChest.ChestState,           //包装状态
        //                    MaterialCode = oldChest.MaterialCode,       //物料代码
        //                    IsLastPackage = oldChest.IsLastPackage,     //是否尾包
        //                    Quantity = oldChest.Quantity,               //包装数量
        //                    Description = oldChest.Description,         //描述
        //                    Creator = oldChest.Creator,                 //创建人
        //                    CreateTime = oldChest.CreateTime,           //创建时间                        
        //                    Editor = oldChest.Editor,                   //编辑人             
        //                    EditTime = oldChest.EditTime,               //编辑时间                        
        //                };
        //                oldChestNoForDelete = chestNo;
        //            }
        //            else
        //            {
        //                result.Code = 2003;
        //                result.Message = string.Format("柜号[{0}]明细不存在！", chestNo);

        //                return result;
        //            }
        //            #endregion                  
        //        }
        //        else
        //        {
        //            result.Code = 2003;
        //            result.Message = string.Format("柜号[{0}]不存在！", chestNo);

        //            return result;
        //        }
        //        #endregion

        //        #region 2 事务处理
        //        session = this.LotDataEngine.SessionFactory.OpenSession();
        //        transaction = session.BeginTransaction();

        //        try
        //        {
        //            //原托号信息更新
        //            foreach(Package item in newPackageForUpdate)
        //            {
        //                this.PackageDataEngine.Update(item, session);
        //            }


        //            //原柜号明细信息删除
        //            foreach (ChestDetailKey item in oldChestDetailKeyForDelete)
        //            {
        //                this.ChestDetailDataEngine.Delete(item, session);
        //            }
        //            //原柜号信息删除
        //            this.ChestDataEngine.Delete(oldChestNoForDelete,session);

        //            //新柜号信息插入
        //            this.ChestDataEngine.Insert(newChestForInsert, session);
        //            //新柜号明细信息插入
        //            foreach (ChestDetail item in newChestDetailForInsert)
        //            {
        //                this.ChestDetailDataEngine.Insert(item, session);
        //            }

        //            transaction.Commit();
        //            session.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            session.Close();

        //            result.Code = 2000;
        //            result.Message = string.Format(StringResource.Error, ex.Message);
        //        }                
        //        #endregion

        //        //返回新柜号信息
        //        result.ObjectNo = realChestNo;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Code = 1000;
        //        result.Message = string.Format(StringResource.Error, ex.Message);
        //        result.Detail = ex.ToString();
        //    }
        //    return result;

        //}
        #endregion

        #region 注释--对于尾柜和空柜不判断控制条件
        //if (!chest.IsLastPackage && chest.Quantity > 0)
        //{
        //    #region 1.1.2.2 判断产品是否一致，必须一致方可入柜
        //    if (oemLot != null)
        //    {
        //        if (chest.MaterialCode != oemWorkOrder.MaterialCode && chest.MaterialCode != "")
        //        {
        //            result.Code = 2009;
        //            result.Message = string.Format("柜物料[{0}]与托物料[{1}]不一致！",
        //                                            chest.MaterialCode,
        //                                            oemWorkOrder.MaterialCode);
        //            return result;
        //        }
        //    }
        //    else
        //    {
        //        if (chest.MaterialCode != lot.MaterialCode && chest.MaterialCode != "")
        //        {
        //            result.Code = 2009;
        //            result.Message = string.Format("柜物料[{0}]与托物料[{1}]不一致！",
        //                                            chest.MaterialCode,
        //                                            lot.MaterialCode);
        //            return result;
        //        }
        //    }                                       
        //    #endregion

        //    #region 1.1.2.3 校验等级、花色、分档、电流规则
        //    cfg = new PagingConfig()
        //    {
        //        Where = string.Format(@"Key.ChestNo='{0}'", p.ChestNo),
        //        OrderBy = "ItemNo ASC"
        //    };
        //    lstChestDetail = this.ChestDetailDataEngine.Get(cfg);
        //    if (lstChestDetail != null && lstChestDetail.Count > 0)
        //    {
        //        if (lstChestDetail[0].Key.ObjectNumber != "" && lstChestDetail[0].Key.ObjectNumber != null)
        //        {
        //            cfg = new PagingConfig()
        //            {
        //                Where = string.Format(@"Key.PackageNo='{0}'", lstChestDetail[0].Key.ObjectNumber),
        //                OrderBy = "ItemNo ASC"
        //            };
        //            //取得柜内首托对象
        //            Package firstPackage = this.PackageDataEngine.Get(lstChestDetail[0].Key.ObjectNumber);
        //            //取得托明细对象
        //            lstPackageDetail1 = this.PackageDetailDataEngine.Get(cfg);
        //            if (lstPackageDetail1 != null && lstPackageDetail1.Count > 0)
        //            {
        //                //取得柜内首托带属性对象
        //                firstInChestPackageAttr = GetAttrOfPackage(firstPackage).Data;
        //                result = CheckLotInPackage(haveAttrPackage, firstInChestPackageAttr);

        //                if (result.Code > 0)
        //                {
        //                    //查找符合条件的柜号，若无则生成新柜号
        //                    newChestNo = GetChestNo(haveAttrPackage.Key);
        //                    chest = this.ChestDataEngine.Get(newChestNo.Data);
        //                    if (chest == null)
        //                    {
        //                        p.ChestNo = newChestNo.Data;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                result.Code = 2003;
        //                result.Message = string.Format("托[{0}]明细不存在！", lstChestDetail[0].Key.ObjectNumber);
        //                return result;
        //            }
        //        }
        //        else
        //        {
        //            result.Code = 2009;
        //            result.Message = string.Format("柜[{0}]明细不存在！",
        //                                           p.ChestNo);
        //            return result;
        //        }
        //    }
        //    #endregion
        //}
        #endregion

        #region 注释--原成柜控制参数获取方法
        ////花色控制
        //cfg = new PagingConfig()
        //{
        //    IsPaging = false,
        //    Where = string.Format("Key.CategoryName='ChestInParameters' AND Key.AttributeName='ColorLimt'")
        //};
        //IList<BaseAttributeValue> lstColorLimt = this.BaseAttributeValueDataEngine.Get(cfg);
        //if (lstColorLimt != null && lstColorLimt.Count > 0)
        //{
        //    colorLimt = Convert.ToBoolean(lstColorLimt[0].Value);
        //}
        ////等级控制
        //cfg = new PagingConfig()
        //{
        //    IsPaging = false,
        //    Where = string.Format("Key.CategoryName='ChestInParameters' AND Key.AttributeName='GradeLimt'")
        //};
        //IList<BaseAttributeValue> lstGradeLimt = this.BaseAttributeValueDataEngine.Get(cfg);
        //if (lstGradeLimt != null && lstGradeLimt.Count > 0)
        //{
        //    gradeLimt = Convert.ToBoolean(lstGradeLimt[0].Value);
        //}
        ////电流控制
        //cfg = new PagingConfig()
        //{
        //    IsPaging = false,
        //    Where = string.Format("Key.CategoryName='ChestInParameters' AND Key.AttributeName='IscLimt'")
        //};
        //IList<BaseAttributeValue> lstIscLimt = this.BaseAttributeValueDataEngine.Get(cfg);
        //if (lstIscLimt != null && lstIscLimt.Count > 0)
        //{
        //    iscLimt = Convert.ToBoolean(lstIscLimt[0].Value);
        //}
        ////功率控制
        //cfg = new PagingConfig()
        //{
        //    IsPaging = false,
        //    Where = string.Format("Key.CategoryName='ChestInParameters' AND Key.AttributeName='PowerLimt'")
        //};
        //IList<BaseAttributeValue> lstPowerLimt = this.BaseAttributeValueDataEngine.Get(cfg);
        //if (lstPowerLimt != null && lstPowerLimt.Count > 0)
        //{
        //    powerLimt = Convert.ToBoolean(lstPowerLimt[0].Value);
        //}
        ////工单控制
        //cfg = new PagingConfig()
        //{
        //    IsPaging = false,
        //    Where = string.Format("Key.CategoryName='ChestInParameters' AND Key.AttributeName='OrderNumberLimt'")
        //};
        //IList<BaseAttributeValue> lstOrderNumberLimt = this.BaseAttributeValueDataEngine.Get(cfg);
        //if (lstOrderNumberLimt != null && lstOrderNumberLimt.Count > 0)
        //{
        //    ordernumberLimt = Convert.ToBoolean(lstOrderNumberLimt[0].Value);
        //}
        #endregion

        #region 注释--原设置尾柜是否卡控产品编码
        //cfg = new PagingConfig()
        //{
        //    IsPaging = false,
        //    Where = string.Format("Key.CategoryName='LastChestControlParameters' AND Key.AttributeName='ByMaterialCode'")
        //};
        //IList<BaseAttributeValue> lstMaterialCodeLimt = this.BaseAttributeValueDataEngine.Get(cfg);
        //if (lstMaterialCodeLimt != null && lstMaterialCodeLimt.Count > 0)
        //{
        //    materialCodeLimt = Convert.ToBoolean(lstMaterialCodeLimt[0].Value);
        //}
        #endregion

        #region 注释--原包装工序是否允许入柜控制
        //cfg = new PagingConfig()
        //{
        //    IsPaging = false,
        //    Where = string.Format("Key.CategoryName='ChestInParameters' AND Key.AttributeName='PackagingInChest'")
        //};
        //IList<BaseAttributeValue> lstColorLimt = this.BaseAttributeValueDataEngine.Get(cfg);
        //if (lstColorLimt != null && lstColorLimt.Count > 0)
        //{
        //    routeControll = Convert.ToBoolean(lstColorLimt[0].Value);
        //}
        #endregion

        #region 注释--原生成新柜号方法
        //DateTime now = DateTime.Now;
        //string minChestNo = "";
        //string maxChestNo = "";
        //string prefixChestNo = "";
        //int seqNo = 1;
        //string year = Convert.ToInt32(now.ToString("yy")).ToString("00");
        //string month = now.Month.ToString("00");
        //PagingConfig cfg = new PagingConfig();
        //try
        //{
        //    #region 柜号生成
        //    if (packageNo != null && packageNo != "")
        //    {
        //        packageNo = packageNo.ToUpper();
        //        if (packageNo.Substring(0, 3) == "05M" || packageNo.Substring(0, 2) == "05P")
        //        {
        //            #region 协鑫永能P660柜号生成规则
        //            prefixChestNo = string.Format("0000");
        //            if (localName == "G01")
        //            {
        //                seqNo = 840;
        //                //晋中柜号流水码限制
        //                minChestNo = string.Format("{0}0840", prefixChestNo);
        //                maxChestNo = string.Format("{0}0870", prefixChestNo);
        //            }
        //            if (localName == "K01")
        //            {
        //                seqNo = 871;
        //                //文水柜号流水码限制
        //                minChestNo = string.Format("{0}0871", prefixChestNo);
        //                maxChestNo = string.Format("{0}0910", prefixChestNo);
        //            }

        //            cfg = new PagingConfig()
        //            {
        //                PageNo = 0,
        //                PageSize = 1,
        //                Where = string.Format(@"Key >= '{0}' AND Key < '{1}'"
        //                                                    , minChestNo
        //                                                    , maxChestNo),
        //                OrderBy = "Key DESC"
        //            };

        //            IList<Chest> lstChest = this.ChestDataEngine.Get(cfg);

        //            if (lstChest.Count > 0)
        //            {
        //                string maxSeqNo = lstChest[0].Key.Replace(prefixChestNo, "");
        //                if (int.TryParse(maxSeqNo, out seqNo))
        //                {
        //                    seqNo = seqNo + 1;
        //                }
        //            }
        //            result.Data = string.Format("{0}{1}", prefixChestNo, seqNo.ToString("0000"));
        //            #endregion
        //        }

        //        #region 注释-原协鑫晋能M660柜号生成规则
        //        //else if (packageNo.Substring(0, 2) == "64")
        //        //{
        //        //    #region 协鑫晋能M660柜号生成规则
        //        //    prefixChestNo = string.Format("0000");
        //        //    if (localName == "G01")
        //        //    {
        //        //        seqNo = 910;
        //        //        //晋中柜号流水码限制
        //        //        minChestNo = string.Format("{0}0910", prefixChestNo);
        //        //        maxChestNo = string.Format("{0}0940", prefixChestNo);
        //        //    }
        //        //    if (localName == "K01")
        //        //    {
        //        //        seqNo = 1;
        //        //        //文水柜号流水码限制
        //        //        minChestNo = string.Format("{0}0001", prefixChestNo);
        //        //        maxChestNo = string.Format("{0}0840", prefixChestNo);
        //        //    }

        //        //    cfg = new PagingConfig()
        //        //    {
        //        //        PageNo = 0,
        //        //        PageSize = 1,
        //        //        Where = string.Format(@"Key >= '{0}' AND Key < '{1}' AND MaterialCode <> '2512020201' "
        //        //                                            , minChestNo
        //        //                                            , maxChestNo),
        //        //        OrderBy = "Key DESC"
        //        //    };

        //        //    IList<Chest> lstChest = this.ChestDataEngine.Get(cfg);

        //        //    if (lstChest.Count > 0)
        //        //    {
        //        //        string maxSeqNo = lstChest[0].Key.Replace(prefixChestNo, "");
        //        //        if (int.TryParse(maxSeqNo, out seqNo))
        //        //        {
        //        //            seqNo = seqNo + 1;
        //        //        }
        //        //    }
        //        //    result.Data = string.Format("{0}{1}", prefixChestNo, seqNo.ToString("0000"));
        //        //    #endregion
        //        //}
        //        #endregion

        //        else if (packageNo.Substring(0, 3) == "27M" || packageNo.Substring(0, 3) == "27P"
        //              || packageNo.Substring(0, 3) == "64M" || packageNo.Substring(0, 3) == "64P")
        //        {
        //            #region 协鑫晋能/张家港M672柜号生成规则
        //            prefixChestNo = string.Format("{0}{1}",year,month);
        //            if (localName == "G01")
        //            {                           
        //                //晋中柜号流水码限制
        //                if (packageNo.Substring(0, 3) == "27M" || packageNo.Substring(0, 3) == "27P")
        //                {
        //                    seqNo = 1;
        //                    minChestNo = string.Format("{0}0001", prefixChestNo);
        //                    maxChestNo = string.Format("{0}4000", prefixChestNo);
        //                }
        //                else
        //                {
        //                    seqNo = 4001;
        //                    minChestNo = string.Format("{0}4001", prefixChestNo);
        //                    maxChestNo = string.Format("{0}5000", prefixChestNo);
        //                }
        //            }
        //            if (localName == "K01")
        //            {
        //                seqNo = 5001;
        //                //文水柜号流水码限制
        //                minChestNo = string.Format("{0}5001", prefixChestNo);
        //                maxChestNo = string.Format("{0}9999", prefixChestNo);
        //            }

        //            cfg = new PagingConfig()
        //            {
        //                PageNo = 0,
        //                PageSize = 1,
        //                Where = string.Format(@"Key >= '{0}' AND Key <= '{1}' AND MaterialCode <> '2512020201' "
        //                                                    , minChestNo
        //                                                    , maxChestNo),
        //                OrderBy = "Key DESC"
        //            };

        //            IList<Chest> lstChest = this.ChestDataEngine.Get(cfg);

        //            if (lstChest.Count > 0)
        //            {
        //                string maxSeqNo = lstChest[0].Key.Replace(prefixChestNo, "");
        //                if (int.TryParse(maxSeqNo, out seqNo))
        //                {
        //                    seqNo = seqNo + 1;
        //                }
        //            }
        //            result.Data = string.Format("{0}{1}", prefixChestNo, seqNo.ToString("0000"));
        //            #endregion
        //        }
        //        else
        //        {
        //            #region 晋能柜号生成规则
        //            if (localName == "K01")
        //            {
        //                prefixChestNo = string.Format("1G{0}{1}", year, month);
        //            }
        //            else if (localName == "G01")
        //            {
        //                prefixChestNo = string.Format("2G{0}{1}", year, month);
        //            }
        //            seqNo = 1;
        //            cfg = new PagingConfig()
        //            {
        //                PageNo = 0,
        //                PageSize = 1,
        //                Where = string.Format(@"Key LIKE '{0}%'"
        //                                    , prefixChestNo),
        //                OrderBy = "Key DESC"
        //            };
        //            IList<Chest> lstChest = this.ChestDataEngine.Get(cfg);
        //            if (lstChest.Count > 0)
        //            {
        //                string maxSeqNo = lstChest[0].Key.Replace(prefixChestNo, "");
        //                if (int.TryParse(maxSeqNo, out seqNo))
        //                {
        //                    seqNo = seqNo + 1;
        //                }
        //            }
        //            result.Data = string.Format("{0}{1}", prefixChestNo, seqNo.ToString("0000"));

        //            #endregion
        //        }
        //    }
        //    else
        //    {
        //        result.Code = 2000;
        //        result.Message = "托号不可为空";
        //    }
        //    #endregion
        //}
        //catch (Exception ex)
        //{
        //    result.Code = 2000;
        //    result.Message = ex.Message;
        //    result.Detail = ex.ToString();
        //}
        #endregion

        #region 注释--原获取最佳柜号方法
        //try
        //{
        //    #region 1.定义变量

        //    PagingConfig cfg = new PagingConfig();
        //    IList<Chest> lstChestExist = null;                  //界面柜号符合当前托号入柜条件
        //    IList<Chest> lstChestNoExist = null;                //界面柜号不符合当前托号入柜条件                
        //    Package package = null;                             //无属性托号
        //    Chest chest = null;                                 //界面柜号信息
        //    MethodReturnResult<Package> packageAttr = null;     //有属性托号
        //    bool colorLimt = false;                             //入柜花色控制
        //    bool gradeLimt = false;                             //入柜等级控制
        //    bool iscLimt = false;                               //入柜电流档控制
        //    bool powerLimt = false;                             //入柜功率档控制
        //    bool ordernumberLimt = false;                       //入柜工单控制
        //    bool materialCodeLimt = true;                       //入柜产品编码控制（默认都不可混产品编码，尾柜可设置混产品编码）
        //    bool lastChestLimt = false;                         //尾柜控制筛选条件
        //    StringBuilder where = new StringBuilder();          //含柜号筛选条件
        //    string where1 = "";                                 //不含柜号筛选条件
        //    Package firstInChestPackageAttr = null;             //柜内第一托组件的带属性托信息
        //    IList<ChestDetail> lstChestDetail = null;           //柜内已入柜明细对象
        //    IList<PackageDetail> lstPackageDetail = null;       //柜内首托组件明细
        //    MethodReturnResult resultOfCheck = new MethodReturnResult();
        //    MethodReturnResult resultOfRePackage = new MethodReturnResult();
        //    lstChestDataForUpdate = new List<Chest>();
        //    MethodReturnResult<MaterialChestParameter> resultOfMaCP = null;

        //    #endregion   

        //    #region 修复柜无工单号BUG-注释
        //    //where.AppendFormat(" {0} Key like '1901%'"
        //    //                        , where.Length > 0 ? "AND" : string.Empty);
        //    //cfg = new PagingConfig()
        //    //{
        //    //    IsPaging = false,
        //    //    OrderBy = "CreateTime ASC",
        //    //    Where = where.ToString()
        //    //};
        //    //lstChestNoExist = this.ChestDataEngine.Get(cfg);
        //    //if (lstChestNoExist != null && lstChestNoExist.Count > 0)
        //    //{
        //    //    foreach (Chest item in lstChestNoExist)
        //    //    {
        //    //        //1.获取柜首托带属性对象
        //    //        cfg = new PagingConfig()
        //    //        {
        //    //            Where = string.Format(@"Key.ChestNo='{0}'", item.Key),
        //    //            OrderBy = "ItemNo ASC"
        //    //        };
        //    //        lstChestDetail = this.ChestDetailDataEngine.Get(cfg);
        //    //        if (lstChestDetail != null && lstChestDetail.Count > 0)
        //    //        {
        //    //            if (lstChestDetail[0].Key.ObjectNumber != "" && lstChestDetail[0].Key.ObjectNumber != null)
        //    //            {
        //    //                cfg = new PagingConfig()
        //    //                {
        //    //                    Where = string.Format(@"Key.PackageNo='{0}'", lstChestDetail[0].Key.ObjectNumber),
        //    //                    OrderBy = "ItemNo ASC"
        //    //                };
        //    //                //取得柜内首托对象
        //    //                Package firstPackage = this.PackageDataEngine.Get(lstChestDetail[0].Key.ObjectNumber);
        //    //                item.OrderNumber = firstPackage.OrderNumber;
        //    //                lstChestDataForUpdate.Add(item);
        //    //            }
        //    //        }
        //    //    }
        //    //    ISession session = this.LotDataEngine.SessionFactory.OpenSession();
        //    //    ITransaction transaction = session.BeginTransaction();
        //    //    try
        //    //    {
        //    //        foreach (Chest item in lstChestDataForUpdate)
        //    //        {
        //    //            this.ChestDataEngine.Update(item, session);
        //    //        }
        //    //        transaction.Commit();
        //    //        session.Close();
        //    //    }
        //    //    catch (Exception err)
        //    //    {
        //    //        transaction.Rollback();
        //    //        session.Close();
        //    //        result.Code = 1000;
        //    //        result.Message += string.Format(StringResource.Error, err.Message);
        //    //        result.Detail = err.ToString();
        //    //        return result;
        //    //    }                    
        //    //}

        //    #endregion

        //    #region 2.托号/柜号合规性检查
        //    if (packageNo != null && packageNo != "")
        //    {
        //        //取得托对象
        //        package = this.PackageDataEngine.Get(packageNo);

        //        //当包装对象在当前表不存在时，从历史数据库提取数据
        //        if (package == null)
        //        {
        //            //返回已归档的(WIP_PACKAGE表)数据
        //            REbackdataParameter pre = new REbackdataParameter();
        //            pre.PackageNo = packageNo;
        //            pre.ErrorMsg = "";
        //            pre.ReType = 1;
        //            pre.IsDelete = 0;
        //            resultOfRePackage = GetREbackdata(pre);

        //            if (resultOfRePackage.Code > 0)
        //            {
        //                result.Code = resultOfRePackage.Code;
        //                result.Message = resultOfRePackage.Message;
        //                return result;
        //            }
        //            else
        //            {
        //                //提取其他归档表数据到当前库，并删除从归档库
        //                pre = new REbackdataParameter();
        //                pre.PackageNo = packageNo;
        //                pre.ReType = 2;
        //                pre.IsDelete = 1;
        //                resultOfRePackage = GetREbackdata(pre);
        //                if (resultOfRePackage.Code > 0)
        //                {
        //                    result.Code = resultOfRePackage.Code;
        //                    result.Message = resultOfRePackage.Message;
        //                    return result;
        //                }
        //            }

        //            //重新取得包装对象
        //            package = this.PackageDataEngine.Get(packageNo);

        //            if (package == null)
        //            {
        //                result.Code = 2000;
        //                result.Message = string.Format("托号{0}不存在！", packageNo);
        //                return result;
        //            }                                            
        //        }
        //        if (package != null)
        //        {
        //            //获取将入包装号属性信息及包装状态
        //            packageAttr = GetAttrOfPackage(package);
        //            if (packageAttr.Data.ContainerNo != null && packageAttr.Data.ContainerNo != "")
        //            {
        //                result.Code = 2002;
        //                result.Message = string.Format("托号[{0}]已入柜[{1}]。", package.Key, package.ContainerNo);
        //                return result;
        //            }
        //            else if (packageAttr.Data.PackageState == EnumPackageState.Packaging
        //                    || packageAttr.Data.PackageState == EnumPackageState.InFabStore
        //                    || packageAttr.Data.PackageState == EnumPackageState.Shipped
        //                    || packageAttr.Data.PackageState == EnumPackageState.Checked)
        //            {
        //                result.Code = 2003;
        //                result.Message = string.Format("托号[{0}]当前状态[{1}],不可入柜！", package.Key, package.PackageState.GetDisplayName());
        //                return result;
        //            }
        //        }   
        //    }
        //    else
        //    {
        //        result.Code = 2000;
        //        result.Message = "托号不可为空";
        //        return result;
        //    }

        //    if (chestNo != null && chestNo != "")
        //    {
        //        //取得柜对象
        //        chest = this.ChestDataEngine.Get(chestNo);                   
        //    }
        //    #endregion

        //    #region 3.设置是否尾柜
        //    if (chest != null)
        //    {
        //        lastChestLimt = chest.IsLastPackage;
        //    }
        //    #endregion

        //    #region 4.获取入柜控制参数
        //    resultOfMaCP = new MethodReturnResult<MaterialChestParameter>();
        //    resultOfMaCP.Data = this.MaterialChestParameterDataEngine.Get(packageAttr.Data.MaterialCode);
        //    if (resultOfMaCP != null && resultOfMaCP.Data != null)
        //    {
        //        if (!lastChestLimt && !isLastChest)
        //        {
        //            #region 4.1成柜控制参数获取（电流/等级/功率/颜色/工单）
        //            colorLimt = Convert.ToBoolean(resultOfMaCP.Data.ColorLimit);
        //            gradeLimt = Convert.ToBoolean(resultOfMaCP.Data.GradeLimit);
        //            powerLimt = Convert.ToBoolean(resultOfMaCP.Data.PowerLimit);
        //            iscLimt = Convert.ToBoolean(resultOfMaCP.Data.IscLimit);
        //            ordernumberLimt = Convert.ToBoolean(resultOfMaCP.Data.OrderNumberLimit);
        //            #endregion
        //        }
        //        else
        //        {
        //            //现设置尾柜是否卡控产品编码
        //            materialCodeLimt = Convert.ToBoolean(resultOfMaCP.Data.LastChestMaterialLimit);                       
        //        }
        //    }
        //    else
        //    {
        //        result.Code = 2000;
        //        result.Message = string.Format(@"产品编码（{0}）成柜规则不存在，请找工艺人员进行设置！", packageAttr.Data.MaterialCode);
        //        return result;
        //    }                           
        //    #endregion

        //    #region 5.设置筛选条件并设置柜号
        //    where.AppendFormat(" {0} ChestState = {1}"
        //                            , where.Length > 0 ? "AND" : string.Empty
        //                            , Convert.ToInt32(EnumChestState.Packaging));
        //    if (colorLimt)
        //    {
        //        where.AppendFormat(" {0} Color = '{1}'"
        //                            , where.Length > 0 ? "AND" : string.Empty
        //                            , packageAttr.Data.Color);
        //    }
        //    if (gradeLimt)
        //    {
        //        where.AppendFormat(" {0} Grade = '{1}'"
        //                            , where.Length > 0 ? "AND" : string.Empty
        //                            , packageAttr.Data.Grade);
        //    }
        //    if (iscLimt)
        //    {
        //        where.AppendFormat(" {0} PowerSubCode = '{1}'"
        //                            , where.Length > 0 ? "AND" : string.Empty
        //                            , packageAttr.Data.PowerSubCode);
        //    }
        //    if (powerLimt)
        //    {
        //        where.AppendFormat(" {0} PowerName = '{1}'"
        //                            , where.Length > 0 ? "AND" : string.Empty
        //                            , packageAttr.Data.PowerName);
        //    }
        //    if (ordernumberLimt)
        //    {
        //        where.AppendFormat(" {0} OrderNumber = '{1}'"
        //                            , where.Length > 0 ? "AND" : string.Empty
        //                            , packageAttr.Data.OrderNumber);
        //    }
        //    if (materialCodeLimt)
        //    {
        //        where.AppendFormat(" {0} MaterialCode = '{1}'"
        //                            , where.Length > 0 ? "AND" : string.Empty
        //                            , package.MaterialCode);
        //    }
        //    where1 = where.ToString();
        //    //界面录入柜号是有效柜号时
        //    if (chest != null)
        //    {
        //        if (isLastChest || lastChestLimt)
        //        {
        //            //是否产品编码控制
        //            if (materialCodeLimt)
        //            {
        //                if (packageAttr.Data.MaterialCode != chest.MaterialCode)
        //                {
        //                    result.Code = 2000;
        //                    result.Message = string.Format(@"已设置尾柜料号卡控,柜号{0}料号{1}与托号{2}料号{3}不一致！", 
        //                                    chest.Key,chest.MaterialCode,packageAttr.Data.Key,packageAttr.Data.MaterialCode);
        //                    return result;
        //                }
        //            }
        //            result.Data = chest.Key;
        //            return result;
        //        }
        //        where.AppendFormat(" {0} Key = '{1}'"
        //                            , where.Length > 0 ? "AND" : string.Empty
        //                            , chest.Key);
        //    }
        //    else
        //    {
        //        if (isLastChest)
        //        {
        //            where.AppendFormat(" {0} IsLastPackage = 'TRUE'"
        //                                , where.Length > 0 ? "AND" : string.Empty);
        //        }
        //    }                
        //    #endregion

        //    #region 6.获得柜号

        //    #region 两种模式
        //    //手动入柜模式--检验界面当前柜号是否符合托号入柜条件
        //    if (isManual)
        //    {
        //        if (chest != null)
        //        {
        //            //非尾柜
        //            if (!lastChestLimt && !isLastChest)
        //            {
        //                #region 检验柜号是否符合将入托号入柜条件
        //                //1.获取柜首托带属性对象
        //                cfg = new PagingConfig()
        //                {
        //                    Where = string.Format(@"Key.ChestNo='{0}'", chest.Key),
        //                    OrderBy = "ItemNo ASC"
        //                };
        //                lstChestDetail = this.ChestDetailDataEngine.Get(cfg);
        //                if (lstChestDetail != null && lstChestDetail.Count > 0)
        //                {
        //                    if (lstChestDetail[0].Key.ObjectNumber != "" && lstChestDetail[0].Key.ObjectNumber != null)
        //                    {
        //                        cfg = new PagingConfig()
        //                        {
        //                            Where = string.Format(@"Key.PackageNo='{0}'", lstChestDetail[0].Key.ObjectNumber),
        //                            OrderBy = "ItemNo ASC"
        //                        };
        //                        //取得柜内首托对象
        //                        Package firstPackage = this.PackageDataEngine.Get(lstChestDetail[0].Key.ObjectNumber);
        //                        //取得首托明细对象
        //                        lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
        //                        if (lstPackageDetail != null && lstPackageDetail.Count > 0)
        //                        {
        //                            //取得柜内首托带属性对象
        //                            firstInChestPackageAttr = GetAttrOfPackage(firstPackage).Data;
        //                            resultOfCheck = CheckLotInPackage(packageAttr.Data, firstInChestPackageAttr);

        //                            if (resultOfCheck.Code > 0)
        //                            {
        //                                result.Code = resultOfCheck.Code;
        //                                result.Message = resultOfCheck.Message;
        //                                return result;
        //                            }
        //                            else
        //                            {
        //                                result.Data = chest.Key;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            result.Code = 2003;
        //                            result.Message = string.Format("托[{0}]明细不存在！", lstChestDetail[0].Key.ObjectNumber);
        //                            return result;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        result.Code = 2009;
        //                        result.Message = string.Format("柜[{0}]明细不存在！", chest.Key);
        //                        return result;
        //                    }
        //                }

        //                #endregion
        //            }                       
        //        }
        //        else
        //        {
        //            if (chestNo != null && chestNo != "")
        //            {
        //                result.Code = 2000;
        //                result.Message = string.Format(@"柜号{0}不存在,请确认是否选择手动入柜模式,若确认手动,请清空界面录入柜号或输入有效柜号！", chestNo);
        //                return result;
        //            }
        //            else
        //            {
        //                cfg = new PagingConfig()
        //                {
        //                    IsPaging = false,
        //                    OrderBy = "CreateTime ASC",
        //                    Where = where.ToString()
        //                };
        //                lstChestExist = this.ChestDataEngine.Get(cfg);
        //                //如果找到符合条件的柜号则返回柜号值，如果没找到生成新柜号
        //                if (lstChestExist != null && lstChestExist.Count > 0)
        //                {
        //                    result.Data = lstChestExist[0].Key;
        //                }
        //                else
        //                {
        //                    result = CreateChestNo(package.Key);
        //                }
        //            }
        //        }
        //    }
        //    //自动入柜模式--找到符合条件的柜号 OR 生成新柜号
        //    else
        //    {
        //        cfg = new PagingConfig()
        //        {
        //            IsPaging = false,
        //            OrderBy = "CreateTime ASC",
        //            Where = where.ToString()
        //        };
        //        lstChestExist = this.ChestDataEngine.Get(cfg);
        //        //如果界面柜号符合入柜条件 OR 如果界面柜号为空并找到符合条件得柜号
        //        if (lstChestExist != null && lstChestExist.Count > 0)
        //        {
        //            result.Data = lstChestExist[0].Key;
        //        }
        //        //如果界面柜号不符合入柜条件 OR 如果界面柜号为空且没找到符合条件得柜号
        //        else
        //        {
        //            cfg = new PagingConfig()
        //            {
        //                IsPaging = false,
        //                OrderBy = "CreateTime ASC",
        //                Where = where1.ToString()
        //            };
        //            lstChestNoExist = this.ChestDataEngine.Get(cfg);

        //            //如果找到符合条件的柜号则返回柜号值，如果没找到生成新柜号
        //            if (lstChestNoExist != null && lstChestNoExist.Count > 0)
        //            {
        //                result.Data = lstChestNoExist[0].Key;
        //            }
        //            else
        //            {
        //                result = CreateChestNo(package.Key);
        //            }
        //        }                   
        //    }
        //    #endregion

        //    #endregion
        //}
        //catch (Exception ex)
        //{
        //    result.Code = 1000;
        //    result.Message = string.Format(StringResource.Error, ex.Message);
        //    result.Detail = ex.ToString();
        //}
        #endregion

        #region 注释--原入柜方法
        //try
        //{
        //    string strChestNo = "";
        //    PagingConfig cfg = null;
        //    ISession session = null;
        //    ITransaction transaction = null;
        //    DateTime now = DateTime.Now;                         //当前时间
        //    Chest chest = null;
        //    Package package = null;
        //    IList<PackageDetail> lstPackageDetail = null;
        //    ChestDetail chestDetail = null;                     //待入柜托明细
        //    bool routeControll = false;
        //    bool isNewChest = false;                            //柜是否为新建
        //    Package haveAttrPackage = null;                     //带包装属性的将入托号
        //    Lot lotCurr = null;
        //    Lot lot = null;
        //    WorkOrder oemWorkOrder = null;  
        //    OemData oemLot = null;
        //    MethodReturnResult<MaterialChestParameter> resultOfMaCP = null;

        //    if (p == null)
        //    {
        //        result.Code = 2001;
        //        result.Message = StringResource.ParameterIsNull;
        //        return result;
        //    }

        //    #region 0.托号及托号内第一块组件明细合规性检查
        //    if (p.PackageNo != "" && p.PackageNo != null)
        //    {
        //        cfg = new PagingConfig()
        //        {
        //            Where = string.Format(@"Key.PackageNo='{0}'", p.PackageNo),
        //            OrderBy = "ItemNo ASC"
        //        };
        //        //取得托对象
        //        package = this.PackageDataEngine.Get(p.PackageNo);
        //        if (package != null)
        //        {                        
        //            if (package.ContainerNo != null && package.ContainerNo != "")
        //            {
        //                result.Code = 2003;
        //                result.Message = string.Format("托号[{0}]已入柜[{1}]！", package.Key,package.ContainerNo);
        //                return result;
        //            }
        //            else
        //            {
        //                haveAttrPackage = GetAttrOfPackage(package).Data;

        //                #region 取得托明细对象
        //                lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
        //                if (lstPackageDetail != null && lstPackageDetail.Count > 0)
        //                {
        //                    oemLot = this.OemDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
        //                    if (oemLot == null)
        //                    {
        //                        //取得批次信息
        //                        lotCurr = this.LotDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
        //                        if (lotCurr == null)
        //                        {
        //                            result.Code = 2003;
        //                            result.Message = string.Format("批次[{0}]不存在！", lstPackageDetail[0].Key.ObjectNumber);
        //                            return result;
        //                        }

        //                        lot = lotCurr.Clone() as Lot;
        //                    }
        //                    else
        //                    {
        //                        oemWorkOrder = this.WorkOrderDataEngine.Get(oemLot.OrderNumber.ToString().Trim().ToUpper());
        //                        if (oemWorkOrder == null)
        //                        {
        //                            result.Code = 2005;
        //                            result.Message = string.Format("批次[{0}]工单[{1}]不存在！",
        //                                                            oemLot.Key.ToString(),
        //                                                            oemLot.OrderNumber.ToString().Trim().ToUpper());
        //                            return result;
        //                        }
        //                    }
        //                }
        //                #endregion
        //            }                        
        //        }
        //        else
        //        {
        //            result.Code = 2003;
        //            result.Message = string.Format("托号[{0}]不存在！", p.PackageNo);

        //            return result;
        //        }
        //    }
        //    else
        //    {
        //        result.Code = 2003;
        //        result.Message = string.Format("托号不可为空！");

        //        return result;
        //    }
        //    #endregion

        //    #region 1.生成新柜和取得柜信息
        //    if (lot != null || oemLot != null)
        //    {
        //        //获取包装工序是否允许入柜参数值
        //        resultOfMaCP = new MethodReturnResult<MaterialChestParameter>();
        //        resultOfMaCP.Data = this.MaterialChestParameterDataEngine.Get(haveAttrPackage.MaterialCode);
        //        if (resultOfMaCP != null && resultOfMaCP.Data != null)
        //        {
        //            #region 2.1.1成柜控制参数获取（包装工序是否允许成柜）
        //            routeControll = Convert.ToBoolean(resultOfMaCP.Data.IsPackagedChest);
        //            #endregion
        //        }
        //        else
        //        {
        //            result.Code = 2000;
        //            result.Message = string.Format(@"产品编码（{0}）成柜规则不存在，请找工艺人员进行设置！", haveAttrPackage.MaterialCode);
        //            return result;
        //        }  
        //        //包装工序不允许成柜
        //        if (!routeControll)
        //        {
        //            //如果未完成包装不允许入柜
        //            if (haveAttrPackage.PackageState != EnumPackageState.Packaged)
        //            {
        //                routeControll = true;
        //            }
        //            else
        //            {
        //                result.Code = 2003;
        //                result.Message = string.Format("托号[{0}]当前状态[{1}]，已设置未入库但已完成包装的托号不允许入柜，请知悉！",
        //                                p.PackageNo, haveAttrPackage.PackageState.GetDisplayName());
        //                return result;
        //            }
        //        }
        //        else
        //        {
        //            #region 1.1 取得柜信息
        //            if (p.ChestNo != "" && p.ChestNo != null)
        //            {
        //                //取得柜对象                    
        //                chest = this.ChestDataEngine.Get(p.ChestNo);
        //                if (chest != null)
        //                {
        //                    bool conditiion = false;
        //                    if (p.isManual)
        //                    {
        //                        conditiion = (chest.ChestState == EnumChestState.Packaging || chest.ChestState == EnumChestState.InFabStore);
        //                    }
        //                    else
        //                    {
        //                        conditiion = (chest.ChestState == EnumChestState.Packaging);
        //                    }
        //                    #region 1.1.2 判断柜与将入柜托信息
        //                    if (conditiion)
        //                    {
        //                        if (!chest.IsLastPackage)
        //                        {
        //                            chest.IsLastPackage = p.IsLastestPackageInChest;
        //                        }

        //                        #region 1.1.2.0 处理空柜
        //                        if (chest.Quantity == 0)
        //                        {
        //                            chest.IsLastPackage = p.IsLastestPackageInChest;     //设置尾柜状态
        //                            chest.MaterialCode = haveAttrPackage.MaterialCode;   //产品编码
        //                            chest.Grade = haveAttrPackage.Grade;                 //等级
        //                            chest.Color = haveAttrPackage.Color;                 //花色
        //                            chest.PowerName = haveAttrPackage.PowerName;         //功率
        //                            chest.PowerSubCode = haveAttrPackage.PowerSubCode;   //电流档
        //                            chest.StoreLocation = p.StoreLocation;               //库位
        //                            chest.OrderNumber = haveAttrPackage.OrderNumber;     //工单号
        //                        }
        //                        #endregion

        //                        #region 1.1.2.1 判断柜内数量是否超过满柜数量
        //                        if (chest.Quantity >= p.ChestFullQty)
        //                        {
        //                            result.Code = 2003;
        //                            result.Message = string.Format("柜号[{0}]已入柜数量[{1}]超过满柜数量[{2}]不存在！", p.ChestNo, chest.Quantity, p.ChestFullQty);
        //                            return result;
        //                        }
        //                        #endregion                                  

        //                        //设置柜属性
        //                        chest.Quantity += 1;                      //包装数量                    
        //                        chest.Editor = p.Editor;                  //编辑人
        //                        chest.EditTime = now;                     //编辑日期 
        //                    }
        //                    else
        //                    {
        //                        result.Code = 2006;
        //                        result.Message = string.Format("柜号[{0}]状态[{1}]非入柜中状态,不可入！", chest.Key, chest.ChestState.GetDisplayName());
        //                        return result;
        //                    }
        //                    #endregion
        //                }
        //                else
        //                {
        //                    strChestNo = p.ChestNo;

        //                    //创建柜对象
        //                    chest = new Chest()
        //                    {
        //                        Key = strChestNo,                           //主键柜号 
        //                        ChestState = EnumChestState.Packaging,      //包装状态                                    
        //                        IsLastPackage = p.IsLastestPackageInChest,  //是否尾包
        //                        Quantity = 1,                               //包装数量
        //                        Description = "",                           //描述
        //                        Creator = p.Editor,                         //创建人
        //                        CreateTime = now,                           //创建时间                        
        //                        Editor = p.Editor,                          //编辑人             
        //                        EditTime = now,                             //编辑时间
        //                        MaterialCode = haveAttrPackage.MaterialCode,   //产品编码
        //                        Grade = haveAttrPackage.Grade,                 //等级
        //                        Color = haveAttrPackage.Color,                 //花色
        //                        PowerName = haveAttrPackage.PowerName,         //功率
        //                        PowerSubCode = haveAttrPackage.PowerSubCode,   //电流档
        //                        StoreLocation = p.StoreLocation,               //库位
        //                        OrderNumber = haveAttrPackage.OrderNumber      //工单号
        //                    };
        //                    isNewChest = true;                            //柜为新建标志
        //                }
        //                //判断柜状态
        //                if (p.ChestFullQty > chest.Quantity)
        //                {
        //                    chest.ChestState = EnumChestState.Packaging;
        //                }
        //                else
        //                {
        //                    chest.ChestState = EnumChestState.Packaged;                                
        //                }
        //                package.ContainerNo = chest.Key;
        //                package.Grade = haveAttrPackage.Grade;
        //                package.Color = haveAttrPackage.Color;
        //                package.PowerSubCode = haveAttrPackage.PowerSubCode;
        //                package.PowerName = haveAttrPackage.PowerName;
        //            }
        //            else
        //            {
        //                result.Code = 2003;
        //                result.Message = string.Format("柜号[{0}]不存在！", p.ChestNo);
        //                return result;
        //            }
        //            #endregion
        //        }
        //    }
        //    #endregion

        //    #region 2 创建柜明细对象
        //    chestDetail = new ChestDetail()
        //    {
        //        Key = new ChestDetailKey()
        //        {
        //            ChestNo = chest.Key,                         //柜号
        //            ObjectNumber = package.Key,                  //托号
        //            ObjectType = EnumChestObjectType.PackageNo   //PackageNo 包装号类型
        //        },
        //        ItemNo = Convert.ToInt32(chest.Quantity),        //入柜项目号（入柜顺序）
        //        Creator = p.Editor,                              //创建人
        //        CreateTime = now                                 //创建时间
        //    };
        //    if (oemLot != null)
        //    {
        //        chestDetail.MaterialCode = oemWorkOrder.MaterialCode;   //物料代码
        //    }
        //    else
        //    {
        //        chestDetail.MaterialCode = lot.MaterialCode;            //物料代码
        //    }
        //    #endregion

        //    #region 3 开始事物处理
        //    session = this.LotDataEngine.SessionFactory.OpenSession();
        //    transaction = session.BeginTransaction();

        //    try
        //    {
        //        #region 3.1 柜数据

        //        //柜数据
        //        if (isNewChest)        //新柜号
        //        {
        //            this.ChestDataEngine.Insert(chest, session);
        //        }
        //        else
        //        {
        //            this.ChestDataEngine.Update(chest, session);
        //        }

        //        //柜明细数据
        //        this.ChestDetailDataEngine.Insert(chestDetail, session);

        //        #endregion

        //        #region 3.2 托数据

        //        this.PackageDataEngine.Update(package, session);

        //        #endregion

        //        transaction.Commit();
        //        session.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction.Rollback();
        //        session.Close();

        //        result.Code = 2000;
        //        result.Message = string.Format(StringResource.Error, ex.Message);
        //    }               
        //    #endregion

        //    //返回柜信息
        //    result.ObjectNo = chest.Key;
        //    result.Detail = chest.ChestState.GetHashCode().ToString() + "-" + chest.StoreLocation;
        //}
        //catch (Exception ex)
        //{
        //    result.Code = 1000;
        //    result.Message = string.Format(StringResource.Error, ex.Message);
        //    result.Detail = ex.ToString();
        //}
        #endregion

        #region 注释--原包装获取包装属性及状态方法
        //try
        //{
        //    IList<PackageDetail> lstPackageDetail = null;
        //    IList<IVTestData> lstLotIVTestData = null;                  //自制组件IV测试数据
        //    IList<WorkOrderPowerset> lstWorkOrderPowerset = null;       //自制组件工单分档规则
        //    PagingConfig cfg = new PagingConfig();
        //    OemData packageFirstLot = null;
        //    Lot lot = null;
        //    MethodReturnResult<DataSet> erpOutData = null;              //ERP中托号出货记录

        //    #region 1.获取包装号属性信息
        //    //如果包装属性值不为空
        //    if (package.PowerName != null && package.PowerName != "")
        //    {
        //        package.PowerName = package.PowerName;
        //        package.PowerSubCode = package.PowerSubCode;
        //        package.Grade = package.Grade;
        //        package.Color = package.Color;
        //    }
        //    //如果包装属性值为空，获取包装托内第一块组件的信息
        //    else
        //    {
        //        cfg = new PagingConfig()
        //        {
        //            IsPaging = false,
        //            Where = string.Format(@"Key.PackageNo='{0}'", package.Key),
        //            OrderBy = "ItemNo ASC"
        //        };
        //        //取得托明细对象
        //        lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
        //        if (lstPackageDetail != null && lstPackageDetail.Count > 0)
        //        {
        //            packageFirstLot = this.OemDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
        //            if (packageFirstLot != null)
        //            {
        //                package.PowerName = packageFirstLot.PnName;
        //                package.PowerSubCode = packageFirstLot.PsSubCode;
        //                package.Grade = packageFirstLot.Grade;
        //                package.Color = packageFirstLot.Color;
        //            }
        //            else
        //            {
        //                //取得第一块批次信息
        //                lot = this.LotDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
        //                if (lot == null)
        //                {
        //                    result.Code = 2003;
        //                    result.Message = string.Format("批次[{0}]不存在！", lstPackageDetail[0].Key.ObjectNumber);
        //                    return result;
        //                }
        //                cfg = new PagingConfig()
        //                {
        //                    PageNo = 0,
        //                    PageSize = 1,
        //                    Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lot.Key)
        //                };
        //                lstLotIVTestData = this.IVTestDataDataEngine.Get(cfg);
        //                if (lstLotIVTestData != null && lstLotIVTestData.Count > 0)
        //                {
        //                    #region 取得批次工单分档规则
        //                    cfg = new PagingConfig()
        //                    {
        //                        PageNo = 0,
        //                        PageSize = 1,
        //                        Where = string.Format("Key.OrderNumber = '{0}' AND Key.Code='{1}' AND Key.ItemNo = '{2}'", lot.OrderNumber, lstLotIVTestData[0].PowersetCode, lstLotIVTestData[0].PowersetItemNo)
        //                    };
        //                    lstWorkOrderPowerset = this.WorkOrderPowersetDataEngine.Get(cfg);
        //                    if (lstWorkOrderPowerset == null || lstWorkOrderPowerset.Count <= 0)
        //                    {
        //                        result.Code = 3001;
        //                        result.Message = string.Format("批次[{0}]所在工单[{1}]分档规则[{3}-{4}]不存在！",
        //                            lot.Key, lot.OrderNumber, lstLotIVTestData[0].PowersetCode, lstLotIVTestData[0].PowersetItemNo);
        //                        return result;
        //                    }
        //                    #endregion
        //                }
        //                else
        //                {
        //                    result.Code = 3001;
        //                    result.Message = string.Format("提取批次[{0}]测试数据失败！", lot.Key);
        //                    return result;
        //                }
        //                package.PowerName = lstWorkOrderPowerset[0].PowerName;
        //                package.PowerSubCode = lstLotIVTestData[0].PowersetSubCode;
        //                package.Grade = lot.Grade;
        //                package.Color = lot.Color;
        //            }
        //        }
        //    }
        //    #endregion

        //    #region 2.获取包装号状态信息
        //    //获取报表服务器物料出货表数据
        //    ChestParameter chestParameter = new ChestParameter()
        //    {
        //        PackageNo = package.Key,
        //        PageNo = 0,
        //        PageSize = 20
        //    };
        //    erpOutData = GetErpOutOfPackage(ref chestParameter);
        //    if (erpOutData.Data != null && erpOutData.Data.Tables[0].Rows.Count > 0)
        //    {
        //        //查到出货记录设置托号状态为已出货
        //        package.PackageState = EnumPackageState.Shipped;
        //    }
        //    #endregion

        //    result.Data = package;
        //}
        //catch (Exception ex)
        //{
        //    result.Code = 1000;
        //    result.Message = string.Format(StringResource.Error, ex.Message);
        //    result.Detail = ex.ToString();
        //}
        #endregion

        #region 注释--原提取托号相关数据方法
        //string strErrorMessage = string.Empty;
        //try
        //{
        //    if (!string.IsNullOrEmpty(p.PackageNo))
        //    {
        //        using (DbConnection con = this._db.CreateConnection())
        //        {
        //            DbCommand cmd = con.CreateCommand();
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.CommandText = "sp_BK_ReBackData";
        //            this._db.AddInParameter(cmd, "PackageNo", DbType.String, p.PackageNo);
        //            this._db.AddInParameter(cmd, "ReType", DbType.Int32, p.ReType);
        //            this._db.AddInParameter(cmd, "IsDelete", DbType.Int32, p.IsDelete);
        //            cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
        //            cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

        //            SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
        //            parReturn.Direction = ParameterDirection.ReturnValue;
        //            cmd.Parameters.Add(parReturn);
        //            this._db.ExecuteNonQuery(cmd);
        //            int i = (int)cmd.Parameters["@return"].Value;

        //            if (i == -1)
        //            {
        //                strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
        //                result.Code = 1000;
        //                result.Message = strErrorMessage;
        //                result.Detail = strErrorMessage;
        //            }
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    result.Code = 1000;
        //    result.Message = ex.Message;
        //    result.Detail = ex.ToString();
        //}
        #endregion

        #region 注释--原获取托号出货数据方法
        //string strErrorMessage = string.Empty;
        //try
        //{
        //    using (DbConnection con = this.query_db.CreateConnection())
        //    {
        //        DbCommand cmd = con.CreateCommand();
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.CommandText = "sp_Query_ErpOutOfPackage";
        //        this.query_db.AddInParameter(cmd, "@PackageNoList", DbType.String, p.PackageNo);
        //        this.query_db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
        //        this.query_db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);

        //        //返回总记录数
        //        this.query_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
        //        cmd.Parameters["@Records"].Direction = ParameterDirection.Output;

        //        //错误信息
        //        cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
        //        cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

        //        //返回参数
        //        SqlParameter parReturn = new SqlParameter("@return_value", SqlDbType.Int);
        //        parReturn.Direction = ParameterDirection.ReturnValue;
        //        cmd.Parameters.Add(parReturn);

        //        cmd.CommandTimeout = 120;

        //        //执行
        //        result.Data = this.query_db.ExecuteDataSet(cmd);

        //        //返回总记录数
        //        p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);
        //        int i = (int)cmd.Parameters["@return_value"].Value;

        //        if (i == -1)
        //        {
        //            strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
        //            result.Code = 1000;
        //            result.Message = strErrorMessage;
        //            result.Detail = strErrorMessage;
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    result.Code = 1000;
        //    result.Message = ex.Message;
        //    result.Detail = ex.ToString();
        //}
        #endregion

        #region 注释--原检验托号是否满足入柜规则方法
        //PagingConfig cfg = new PagingConfig();
        //MethodReturnResult<MaterialChestParameter> resultOfMaCP = null;
        //try
        //{
        //    #region 1.判断产品编码是否一致
        //    if (packageWait.MaterialCode != packageInChest.MaterialCode)
        //    {
        //        result.Code = 3002;
        //        result.Message = string.Format("待入柜托[{0}]料号[{1}]与已入柜第一托组件[{2}]料号[{3}]不一致！",
        //                                            packageWait.Key,
        //                                            packageWait.MaterialCode,
        //                                            packageInChest.Key,
        //                                            packageInChest.MaterialCode);

        //        return result;
        //    }
        //    #endregion

        //    #region 2.校验等级/花色/分档/电流规则/工单

        //    #region 2.0参数定义
        //    bool colorLimt = false;
        //    bool gradeLimt = false;
        //    bool iscLimt = false;
        //    bool powerLimt = false;
        //    bool ordernumberLimt = false;
        //    #endregion

        //    #region 2.1获取入柜控制参数
        //    resultOfMaCP = new MethodReturnResult<MaterialChestParameter>();
        //    resultOfMaCP.Data = this.MaterialChestParameterDataEngine.Get(packageWait.MaterialCode);
        //    if (resultOfMaCP != null && resultOfMaCP.Data != null)
        //    {
        //        #region 2.1.1成柜控制参数获取（电流/等级/功率/颜色/工单）
        //        colorLimt = Convert.ToBoolean(resultOfMaCP.Data.ColorLimit);
        //        gradeLimt = Convert.ToBoolean(resultOfMaCP.Data.GradeLimit);
        //        powerLimt = Convert.ToBoolean(resultOfMaCP.Data.PowerLimit);
        //        iscLimt = Convert.ToBoolean(resultOfMaCP.Data.IscLimit);
        //        ordernumberLimt = Convert.ToBoolean(resultOfMaCP.Data.OrderNumberLimit);
        //        #endregion
        //    }
        //    else
        //    {
        //        result.Code = 2000;
        //        result.Message = string.Format(@"产品编码（{0}）成柜规则不存在，请找工艺人员进行设置！", packageWait.MaterialCode);
        //        return result;
        //    }
        //    #endregion

        //    #region 2.2判段控制参数是否一致
        //    //判断等级是否一致
        //    if (packageWait.Grade != packageInChest.Grade)
        //    {
        //        if (gradeLimt)
        //        {
        //            result.Code = 3002;
        //            result.Message = string.Format("待入柜托[{0}]的等级[{1}]与已入柜第一托组件[{2}]等级[{3}]不一致！",
        //                                            packageWait.Key,
        //                                            packageWait.Grade,
        //                                            packageInChest.Key,
        //                                            packageInChest.Grade);

        //            return result;
        //        }
        //    }

        //    //判断花色是否一致
        //    if (packageWait.Color != packageInChest.Color)
        //    {
        //        if (colorLimt)
        //        {
        //            result.Code = 3003;
        //            result.Message = string.Format("待入柜托[{0}]的花色[{1}]与已入柜第一托组件[{2}]花色[{3}]不一致！",
        //                                            packageWait.Key,
        //                                            packageWait.Color,
        //                                            packageInChest.Key,
        //                                            packageInChest.Color);

        //            return result;
        //        }
        //    }

        //    //判断功率是否一致
        //    if (packageWait.PowerName != packageInChest.PowerName)
        //    {
        //        if (powerLimt)
        //        {
        //            result.Code = 3003;
        //            result.Message = string.Format("待入柜托[{0}]的功率[{1}]与已入柜第一托组件[{2}]功率[{3}]不一致！",
        //                                            packageWait.Key,
        //                                            packageWait.PowerName,
        //                                            packageInChest.Key,
        //                                            packageInChest.PowerName);

        //            return result;
        //        }
        //    }

        //    //判断电流档是否一致
        //    if (packageWait.PowerSubCode != packageInChest.PowerSubCode)
        //    {
        //        if (iscLimt)
        //        {
        //            result.Code = 3003;
        //            result.Message = string.Format("待入柜托[{0}]的电流档[{1}]与已入柜第一托组件[{2}]电流档[{3}]不一致！",
        //                                            packageWait.Key,
        //                                            packageWait.PowerSubCode,
        //                                            packageInChest.Key,
        //                                            packageInChest.PowerSubCode);

        //            return result;
        //        }
        //    }

        //    //判断工单是否一致
        //    if (packageWait.OrderNumber != packageInChest.OrderNumber)
        //    {
        //        if (ordernumberLimt)
        //        {
        //            result.Code = 3003;
        //            result.Message = string.Format("待入柜托[{0}]的工单号[{1}]与已入柜第一托组件[{2}]工单号[{3}]不一致！",
        //                                            packageWait.Key,
        //                                            packageWait.OrderNumber,
        //                                            packageInChest.Key,
        //                                            packageInChest.OrderNumber);

        //            return result;
        //        }
        //    }
        //    #endregion

        //    #endregion
        //}
        //catch (Exception ex)
        //{
        //    result.Code = 1000;
        //    result.Message = ex.Message;
        //    result.Detail = ex.ToString();
        //}
        #endregion

        #region 注释--原获取柜数据方法
        //if (!this.ChestDataEngine.IsExists(key))
        //{
        //    result.Code = 1002;
        //    result.Message = String.Format(@"柜[{0}]不存在。", key);
        //    return result;
        //}
        //try
        //{
        //    result.Data = this.ChestDataEngine.Get(key);
        //}
        //catch (Exception ex)
        //{
        //    result.Code = 1000;
        //    result.Message = String.Format(StringResource.Error, ex.Message);
        //    result.Detail = ex.ToString();
        //}
        #endregion

        #endregion

        #region 入柜相关操作

        // 包装入柜操作--代码表示：0：成功，其他失败
        // 托号入柜则托号表头会添加属性
        public MethodReturnResult Chest(ChestParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            using(PackageInChestCommom packageInChestCommom = new PackageInChestCommom()) 
            {
                result = packageInChestCommom.Chest(p);
            }                     
            return result;
        }

        // 根据包装号创建新柜号
        public MethodReturnResult<string> CreateChestNo(string packageNo)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>();
            using (PackageInChestCommom packageInChestCommom = new PackageInChestCommom())
            {
                result = packageInChestCommom.CreateChestNo(packageNo);
            }           
            return result;
        }

        //获取符合条件的柜号，若无则新生成
        public MethodReturnResult<string> GetChestNo(string packageNo, string chestNo, bool isLastChest, bool isManual)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>();
            using (PackageInChestCommom packageInChestCommom = new PackageInChestCommom())
            {
                result = packageInChestCommom.GetChestNo(packageNo, chestNo, isLastChest, isManual);
            }           
            return result;
        }

        //根据包装号获取包装属性及状态
        public MethodReturnResult<Package> GetAttrOfPackage(Package package)
        {
            MethodReturnResult<Package> result = new MethodReturnResult<Package>();
            using (PackageInChestCommom packageInChestCommom = new PackageInChestCommom())
            {
                result = packageInChestCommom.GetAttrOfPackage(package);
            }           
            return result;
        }

        /// <summary>
        /// 提取托号相关数据
        /// </summary>
        /// <param name="p">
        /// 1.提取（WIP_PACKAGE）表到当前库{p.ReType = 1,p.IsDelete = 0}
        /// 2.提取其他归档表数据到当前库，并删除从归档库{p.ReType = 2,p.IsDelete = 1}
        /// </param>
        /// <returns></returns>
        public MethodReturnResult GetREbackdata(REbackdataParameter p)
        {           
            MethodReturnResult result = new MethodReturnResult();
            using (PackageInChestCommom packageInChestCommom = new PackageInChestCommom())
            {
                result = packageInChestCommom.GetREbackdata(p);
            }          
            return result;
        }

        // 托号出货数据查询
        public MethodReturnResult<DataSet> GetErpOutOfPackage(ref ChestParameter p)
        {         
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            using (PackageInChestCommom packageInChestCommom = new PackageInChestCommom())
            {
                result = packageInChestCommom.GetErpOutOfPackage(ref p);
            }            
            return result;
        }

        // 检验托号是否满足入柜规则
        public MethodReturnResult CheckLotInPackage(Package packageWait, Package packageInChest)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (PackageInChestCommom packageInChestCommom = new PackageInChestCommom())
            {
                result = packageInChestCommom.CheckLotInPackage(packageWait, packageInChest);
            }           
            return result;
        }

        // 获取柜数据
        public MethodReturnResult<Chest> Get(string key)
        {
            MethodReturnResult<Chest> result = new MethodReturnResult<Chest>();
            using (PackageInChestCommom packageInChestCommom = new PackageInChestCommom())
            {
                result = packageInChestCommom.Get(key);
            }            
            return result;
        }

        //自动完成入柜
        public MethodReturnResult ChangeChest(string chestNo, string userName)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession session = null;
            ITransaction transaction = null; 
            try
            {
                #region 1 获取柜信息
                // 获取柜信息
                Chest chest = this.ChestDataEngine.Get(chestNo);
                if (chest != null)
                {
                    chest.ChestState = EnumChestState.Packaged;                   
                    chest.Editor = userName;                          //编辑人             
                    chest.EditTime = DateTime.Now;                    //编辑时间
                }
                else
                {
                    result.Code = 2003;
                    result.Message = string.Format("柜号[{0}]不存在！", chestNo);

                    return result;
                }
                #endregion                               

                #region 2 事务处理
                session = this.LotDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                try
                {
                    this.ChestDataEngine.Update(chest, session);

                    transaction.Commit();
                    session.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();

                    result.Code = 2000;
                    result.Message = string.Format(StringResource.Error, ex.Message);
                }
                #endregion

                //返回新柜号信息
                result.ObjectNo = chest.Key;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        //手动完成入柜
        public MethodReturnResult FinishChest(ChestParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                #region 1 获取柜信息
                // 获取柜信息
                Chest chest = this.ChestDataEngine.Get(p.ChestNo);
                if (chest != null)
                {
                    chest.ChestState = EnumChestState.Packaged;
                    chest.StoreLocation = p.StoreLocation;
                    chest.IsLastPackage = p.IsLastestPackageInChest;
                    chest.Editor = p.Editor;                          //编辑人             
                    chest.EditTime = DateTime.Now;                    //编辑时间
                }
                else
                {
                    result.Code = 2003;
                    result.Message = string.Format("柜号[{0}]不存在！", p.ChestNo);

                    return result;
                }
                #endregion

                #region 2 事务处理
                session = this.LotDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                try
                {
                    this.ChestDataEngine.Update(chest, session);

                    transaction.Commit();
                    session.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();

                    result.Code = 2000;
                    result.Message = string.Format(StringResource.Error, ex.Message);
                }
                #endregion

                //返回新柜号信息
                result.ObjectNo = chest.Key;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 出柜操作
        public MethodReturnResult UnPackageInChest(ChestParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (p == null)
            {
                result.Code = 1001;
                result.Message = StringResource.ParameterIsNull;
                return result;
            }
            try
            {
                result = this.ExecuteUnPackage(p);
                if (result.Code > 0)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 执行出柜
        public MethodReturnResult ExecuteUnPackage(ChestParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();

            #region 1.合规性检查
            if (p == null)
            {
                result.Code = 1001;
                result.Message = StringResource.ParameterIsNull;
                return result;
            }
            DateTime now = DateTime.Now;
            Chest chestObj = null;

            chestObj = this.ChestDataEngine.Get(p.ChestNo);
            if (chestObj == null)
            {
                result.Code = 1001;
                result.Message = string.Format("柜{0}不存在。", p.ChestNo);
                return result;
            }

            if (chestObj.ChestState != EnumChestState.Packaged 
                && chestObj.ChestState != EnumChestState.Packaging
                && chestObj.ChestState != EnumChestState.Checking)
            {
                result.Code = 1002;
                result.Message = string.Format("柜[{0}]当前状态[{1}]不允许出柜操作！", p.ChestNo, chestObj.ChestState.GetDisplayName());
                return result;
            }

            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format("Key.ChestNo='{0}'", p.ChestNo),
                OrderBy = "ItemNo ASC"
            };
            IList<ChestDetail> lstChestDetail = this.ChestDetailDataEngine.Get(cfg);
            if (lstChestDetail == null || lstChestDetail.Count == 0)
            {
                result.Code = 1003;
                result.Message = string.Format("柜{0}的明细为空。", p.ChestNo);
                return result;
            }
            #endregion

            #region 2.定义更新列表对象
            lstChestDataForUpdate = new List<Chest>();
            lstChestDetailForDelete = new List<ChestDetail>();
            lstChestDetailForInsert = new List<ChestDetail>();
            lstChestDetailForUpdate = new List<ChestDetail>();
            ChestLog chestLog = null;
            #endregion

            #region 3.出柜操作
            Chest chestUpdate = null;
            //更新柜数据。
            chestUpdate = chestObj.Clone() as Chest;
            chestUpdate.Editor = p.Editor;
            chestUpdate.EditTime = now;
            chestUpdate.ChestState = EnumChestState.Packaging;

            Package package = null;
            //取得托对象
            package = this.PackageDataEngine.Get(p.PackageNo);
            if (package != null)
            {
                if (package.PackageState == EnumPackageState.Checked
                    || package.PackageState == EnumPackageState.InFabStore
                    || package.PackageState == EnumPackageState.Packaging
                    || package.PackageState == EnumPackageState.Shipped)  //已检验/线边仓待投料/包装中/已出货的托号不允许出柜操作，已检验的除非执行取消检验操作
                {
                    result.Code = 1002;
                    result.Message = string.Format("托号[{0}]当前状态[{1}]不允许出柜操作！", package.Key, package.PackageState.GetDisplayName());
                    return result;
                }
                //更新WIP_PACKAGE中托号对应的柜号为空
                package.ContainerNo = "";
            }

            ChestDetail chestDetail = null;
            //校验托号是否在柜明细项中
            var lstPackageInChestDetail = (from item in lstChestDetail
                                        where item.Key.ObjectNumber.Trim() == p.PackageNo.Trim()
                                        select item);
            if (lstPackageInChestDetail == null || lstPackageInChestDetail.Count() == 0)
            {
                result.Code = 1003;
                result.Message = string.Format("柜号（{0}）明细项中不存在托号（{1}）。",p.ChestNo, p.PackageNo);
                return result;
            }
            else
            {
                chestDetail = lstPackageInChestDetail.FirstOrDefault();
            }
            
            //记录出柜数量
            chestUpdate.Quantity -= 1;
            if (chestUpdate.Quantity < 0)
            {
                chestUpdate.Quantity = 0;
            }

            //删除入柜明细记录。                
            this.lstChestDetailForDelete.Add(chestDetail);                                                
            
            //移除出柜的托号
            foreach (ChestDetail item in lstChestDetailForDelete)
            {
                lstChestDetail.Remove(item);
            }

            //重新定义Item次序
            int itemNo = 0;
            foreach (ChestDetail chestDetailObj in lstChestDetail)
            {
                itemNo++;
                if (chestDetailObj.ItemNo == itemNo)
                {
                    continue;
                }
                ChestDetail chestDetailObjUpdate = chestDetailObj.Clone() as ChestDetail;
                chestDetailObjUpdate.ItemNo = itemNo;
                this.lstChestDetailForUpdate.Add(chestDetailObjUpdate);
            }

            if (chestUpdate != null)
            {
                lstChestDataForUpdate.Add(chestUpdate);
            }
            #endregion

            #region 4.记录出柜日志
            ChestLogKey chestKey = new ChestLogKey()
            {
                ChestNo = chestUpdate.Key,
                PackageNo = package.Key,
                CreateTime = now,
                ChestActivity = EnumChestActivity.OutChest
            };
            chestLog = new ChestLog()
            {
                Key = chestKey,                
                Creator = p.Editor,
                ModelType = p.ModelType
            };
            #endregion

            #region 5.事务处理
            ISession session = this.LotDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
                #region 开始事物处理

                #region 5.1更新Package及柜基本信息

                this.PackageDataEngine.Update(package, session);

                foreach (Chest chest in lstChestDataForUpdate)
                {
                    this.ChestDataEngine.Update(chest, session);
                }

                foreach (ChestDetail item in lstChestDetailForDelete)
                {
                    this.ChestDetailDataEngine.Delete(item.Key, session);
                }

                foreach (ChestDetail item in lstChestDetailForUpdate)
                {
                    this.ChestDetailDataEngine.Update(item, session);
                }               

                #endregion

                #region 5.2新增出柜动作日志

                if (chestLog != null)
                {
                    this.ChestLogDataEngine.Insert(chestLog, session);
                }
                
                #endregion

                transaction.Commit();
                session.Close();
                #endregion
            }
            catch (Exception err)
            {
                transaction.Rollback();
                session.Close();

                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }
            #endregion

            return result;
        }

        // 操作前检查
        public MethodReturnResult Check(ChestParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };            

            if (p.IsLastestPackageInChest == false
                && (p.PackageNo == null || p.PackageNo == ""))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "托号"
                                                , StringResource.ParameterIsNull);
                return result;
            }                       
            return result;
        }                    

        // 获取柜数据
        public MethodReturnResult<IList<Chest>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Chest>> result = new MethodReturnResult<IList<Chest>>();
            try
            {
                result.Data = this.ChestDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 获取柜明细数据
        public MethodReturnResult<ChestDetail> GetDetail(ChestDetailKey key)
        {
            MethodReturnResult<ChestDetail> result = new MethodReturnResult<ChestDetail>();            
            if (!this.ChestDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(@"柜明细[{0}]不存在。", key);
                return result;
            }
            try
            {
                result.Data = this.ChestDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 获取柜明细数据
        public MethodReturnResult<IList<ChestDetail>> GetDetail(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ChestDetail>> result = new MethodReturnResult<IList<ChestDetail>>();
            try
            {
                result.Data = this.ChestDetailDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 获取柜号明细数据（存储过程）
        public MethodReturnResult<DataSet> GetChestDetailByDB(ref ChestParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this.query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "sp_WIP_ChestDetail";
                    this.query_db.AddInParameter(cmd, "@ChestNo", DbType.String, p.ChestNo);
                    this.query_db.AddInParameter(cmd, "@PackageNo", DbType.String, p.PackageNo);
                    this.query_db.AddInParameter(cmd, "@MaterialCode", DbType.String, p.LotNumber);
                    this.query_db.AddInParameter(cmd, "@OrderNumber", DbType.String, p.OrderNumber);
                    this.query_db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
                    this.query_db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);

                    //返回总记录数
                    this.query_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;

                    //错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //返回参数
                    SqlParameter parReturn = new SqlParameter("@return_value", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    cmd.CommandTimeout = 960;

                    //执行
                    result.Data = this.query_db.ExecuteDataSet(cmd);

                    //返回总记录数
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);
                    int i = (int)cmd.Parameters["@return_value"].Value;

                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 刷新柜号明细数据（存储过程）
        public MethodReturnResult<DataSet> GetRefreshChestDetailByDB(ref ChestParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "sp_Refresh_ChestDetail";
                    this._db.AddInParameter(cmd, "@ChestNo", DbType.String, p.ChestNo);
                    this._db.AddInParameter(cmd, "@PageNo", DbType.Int32, 0);
                    this._db.AddInParameter(cmd, "@PageSize", DbType.Int32, 20);

                    //返回总记录数
                    this._db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;

                    //错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //返回参数
                    SqlParameter parReturn = new SqlParameter("@return_value", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    cmd.CommandTimeout = 960;

                    //执行
                    result.Data = this._db.ExecuteDataSet(cmd);

                    //返回总记录数
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);
                    int i = (int)cmd.Parameters["@return_value"].Value;

                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 查询检验后柜号明细数据（存储过程）
        public MethodReturnResult<DataSet> GetCheckedChestDetailByDB(ref ChestParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "sp_Checked_ChestDetail";
                    this._db.AddInParameter(cmd, "@ChestNo", DbType.String, p.ChestNo);
                    this._db.AddInParameter(cmd, "@PageNo", DbType.Int32, 0);
                    this._db.AddInParameter(cmd, "@PageSize", DbType.Int32, 20);

                    //返回总记录数
                    this._db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;

                    //错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //返回参数
                    SqlParameter parReturn = new SqlParameter("@return_value", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    cmd.CommandTimeout = 960;

                    //执行
                    result.Data = this._db.ExecuteDataSet(cmd);

                    //返回总记录数
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);
                    int i = (int)cmd.Parameters["@return_value"].Value;

                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }

        // 报检--柜号明细查询
        public MethodReturnResult<DataSet> GetChestDetail(ref ChestParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this.query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "sp_WIP_ChestDetail_list";
                    this.query_db.AddInParameter(cmd, "@ChestNo", DbType.String, p.ChestNo);
                    this.query_db.AddInParameter(cmd, "@PackageNo", DbType.String, p.PackageNo);
                    this.query_db.AddInParameter(cmd, "@LotNumber", DbType.String, p.LotNumber);
                    this.query_db.AddInParameter(cmd, "@OrderNumber", DbType.String, p.OrderNumber);
                    this.query_db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
                    this.query_db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);

                    //返回总记录数
                    this.query_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;

                    //错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //返回参数
                    SqlParameter parReturn = new SqlParameter("@return_value", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    cmd.CommandTimeout = 960;

                    //执行
                    result.Data = this.query_db.ExecuteDataSet(cmd);

                    //返回总记录数
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);
                    int i = (int)cmd.Parameters["@return_value"].Value;

                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }
                
        //执行柜明细检验
        public MethodReturnResult CheckPackageInChest(string packageNo, string chestNo,string userName)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                ISession session = null;
                ITransaction transaction = null;
                PagingConfig cfg = new PagingConfig();
                Chest chest = null;
                IList<ChestDetail> lstChestDetail = null;
                Package package = null;
                MethodReturnResult<Package> packageAttr = null;        //有属性托号
                MethodReturnResult resultOfRePackage = new MethodReturnResult();
                double checkedQtyOfChest = 0;                          //柜内已检验托数量
                double fullChestQty = 0;                               //满柜数量

                #region 0.获取柜对象
                if (chestNo == null || chestNo == "")
                {
                    //获取托号所在柜明细
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.ObjectNumber = '{0}'", packageNo)
                    };
                    lstChestDetail = this.ChestDetailDataEngine.Get(cfg);
                    if (lstChestDetail != null && lstChestDetail.Count > 0)
                    {
                        //获取柜对象信息
                        chest = this.ChestDataEngine.Get(lstChestDetail[0].Key.ChestNo);
                    }
                    else
                    {
                        result.Code = 2000;
                        result.Message = string.Format(@"托号[{0}]柜号不存在", packageNo);
                        return result;
                    }
                }
                else
                {
                    chest = this.ChestDataEngine.Get(chestNo);                    
                }
                if (chest != null)
                {
                    if (chest.ChestState != EnumChestState.Packaged
                        && chest.ChestState != EnumChestState.Checking
                        && chest.ChestState != EnumChestState.Checked)
                    {
                        result.Code = 2003;
                        result.Message = string.Format("托号[{0}]所在柜号[{1}]当前状态[{2}],不可执行托号检验！",packageNo, chest.Key, chest.ChestState.GetDisplayName());
                        return result;
                    }
                }
                else
                {
                    result.Code = 2000;
                    result.Message = string.Format(@"柜号[{0}]不存在", chestNo);
                    return result;
                }
                #endregion

                #region 1.托号合规性检查
                if (packageNo != null && packageNo != "")
                {
                    package = this.PackageDataEngine.Get(packageNo);
                    //当包装对象在当前表不存在时，从历史数据库提取数据
                    if (package == null)
                    {
                        //返回已归档的(WIP_PACKAGE表)数据
                        REbackdataParameter pre = new REbackdataParameter();
                        pre.PackageNo = packageNo;
                        pre.ErrorMsg = "";
                        pre.ReType = 1;
                        pre.IsDelete = 0;
                        resultOfRePackage = GetREbackdata(pre);

                        if (resultOfRePackage.Code > 0)
                        {
                            result.Code = resultOfRePackage.Code;
                            result.Message = resultOfRePackage.Message;
                            return result;
                        }
                        else
                        {
                            //提取其他归档表数据到当前库，并删除从归档库
                            pre = new REbackdataParameter();
                            pre.PackageNo = packageNo;
                            pre.ReType = 2;
                            pre.IsDelete = 1;
                            resultOfRePackage = GetREbackdata(pre);
                            if (resultOfRePackage.Code > 0)
                            {
                                result.Code = resultOfRePackage.Code;
                                result.Message = resultOfRePackage.Message;
                                return result;
                            }
                        }

                        //重新取得包装对象
                        package = this.PackageDataEngine.Get(packageNo);

                        if (package == null)
                        {
                            result.Code = 2000;
                            result.Message = string.Format("托号{0}不存在！", packageNo);
                            return result;
                        }
                    }
                    if (package != null)
                    {
                        //获取将入包装号属性信息及包装状态
                        packageAttr = GetAttrOfPackage(package);
                        if (packageAttr.Data.ContainerNo == null || packageAttr.Data.ContainerNo == "")
                        {
                            result.Code = 2002;
                            result.Message = string.Format("托号[{0}]还未入柜，不可检验", package.Key);
                            return result;
                        }
                        //else if (packageAttr.Data.PackageState == EnumPackageState.Packaging
                        //        || packageAttr.Data.PackageState == EnumPackageState.InFabStore
                        //        || packageAttr.Data.PackageState == EnumPackageState.Shipped
                        //        || packageAttr.Data.PackageState == EnumPackageState.Checked)
                        else if (packageAttr.Data.PackageState != EnumPackageState.ToWarehouse)   //只有托号状态为已完成入库才可检验
                        {
                            result.Code = 2003;
                            result.Message = string.Format("托号[{0}]当前状态[{1}],非已入库状态,不可检验！", package.Key, package.PackageState.GetDisplayName());
                            return result;
                        }
                    }   
                }
                else
                {
                    result.Code = 2000;
                    result.Message = "托号不可为空";
                    return result;
                }
                #endregion

                #region 2.柜明细检验
                if (chest != null)
                {
                    if (chestNo != null && chestNo != "")
                    {
                        if (packageAttr.Data.ContainerNo != chestNo)
                        {
                            result.Code = 2000;
                            result.Message = string.Format(@"托号[{0}]检验失败,托号所在柜[{1}]非当前界面柜号[{2}]", package.Key, packageAttr.Data.ContainerNo, chestNo);
                            return result;
                        }
                    }
                    fullChestQty = chest.Quantity;
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.ChestNo = '{0}'
                                                        AND EXISTS(FROM Package as p
                                                           WHERE p.Key=self.Key.ObjectNumber
                                                           AND p.PackageState='{1}')", package.Key, Convert.ToInt32(EnumPackageState.Checked))
                    };
                    lstChestDetail = this.ChestDetailDataEngine.Get(cfg);
                    if (lstChestDetail != null && lstChestDetail.Count > 0)
                    {
                        checkedQtyOfChest = lstChestDetail.Count;
                    }
                    //更改托号状态为已检验，更改柜状态为检验中或检验完成
                    package.PackageState = EnumPackageState.Checked;
                    package.Editor = userName;                          //编辑人             
                    package.EditTime = DateTime.Now;                    //编辑时间
                    if ((checkedQtyOfChest + 1) == fullChestQty)
                    {
                        //更改柜状态为检验完成
                        chest.ChestState = EnumChestState.Checked;
                    }
                    else
                    {
                        //更改柜状态为检验中
                        chest.ChestState = EnumChestState.Checking;
                    }
                    chest.Editor = userName;                          //编辑人             
                    chest.EditTime = DateTime.Now;                    //编辑时间
                }                
                #endregion

                #region 3.事务处理
                session = this.ChestDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                try
                {
                    //柜数据
                    this.ChestDataEngine.Update(chest, session);

                    //托数据
                    this.PackageDataEngine.Update(package, session);

                    transaction.Commit();
                    session.Close();
                    result.Detail = chest.Key;
                    result.ObjectNo = Convert.ToInt32(chest.ChestState).ToString();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();

                    result.Code = 2000;
                    result.Message = string.Format(StringResource.Error, ex.Message);
                }  
                #endregion
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        //执行柜明细取消检验
        public MethodReturnResult UnCheckPackageInChest(string packageNo, string chestNo, string userName)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                ISession session = null;
                ITransaction transaction = null;
                PagingConfig cfg = new PagingConfig();
                Chest chest = null;
                IList<ChestDetail> lstChestDetail = null;
                Package package = null;
                MethodReturnResult<Package> packageAttr = null;     //有属性托号
                MethodReturnResult resultOfRePackage = new MethodReturnResult();
                double checkedQtyOfChest = 0;                          //柜内已检验托数量
                double fullChestQty = 0;                               //满柜数量

                #region 0.获取柜对象
                chest = this.ChestDataEngine.Get(chestNo);
                if (chest != null)
                {
                    if (chest.ChestState != EnumChestState.Packaged
                        && chest.ChestState != EnumChestState.Checking
                        && chest.ChestState != EnumChestState.Checked)
                    {
                        result.Code = 2003;
                        result.Message = string.Format("柜号[{0}]当前状态[{1}],不可执行托号取消检验！", chest.Key, chest.ChestState.GetDisplayName());
                        return result;
                    }
                }
                else
                {
                    result.Code = 2000;
                    result.Message = string.Format(@"柜号[{0}]不存在", chestNo);
                    return result;
                }
                #endregion

                #region 1.托号合规性检查
                if (packageNo != null && packageNo != "")
                {
                    package = this.PackageDataEngine.Get(packageNo);
                    //当包装对象在当前表不存在时，从历史数据库提取数据
                    if (package == null)
                    {
                        //返回已归档的(WIP_PACKAGE表)数据
                        REbackdataParameter pre = new REbackdataParameter();
                        pre.PackageNo = packageNo;
                        pre.ErrorMsg = "";
                        pre.ReType = 1;
                        pre.IsDelete = 0;
                        resultOfRePackage = GetREbackdata(pre);

                        if (resultOfRePackage.Code > 0)
                        {
                            result.Code = resultOfRePackage.Code;
                            result.Message = resultOfRePackage.Message;
                            return result;
                        }
                        else
                        {
                            //提取其他归档表数据到当前库，并删除从归档库
                            pre = new REbackdataParameter();
                            pre.PackageNo = packageNo;
                            pre.ReType = 2;
                            pre.IsDelete = 1;
                            resultOfRePackage = GetREbackdata(pre);
                            if (resultOfRePackage.Code > 0)
                            {
                                result.Code = resultOfRePackage.Code;
                                result.Message = resultOfRePackage.Message;
                                return result;
                            }
                        }

                        //重新取得包装对象
                        package = this.PackageDataEngine.Get(packageNo);

                        if (package == null)
                        {
                            result.Code = 2000;
                            result.Message = string.Format("托号{0}不存在！", packageNo);
                            return result;
                        }
                    }
                    if (package != null)
                    {
                        //获取将入包装号属性信息及包装状态
                        packageAttr = GetAttrOfPackage(package);
                        if (packageAttr.Data.ContainerNo == null || packageAttr.Data.ContainerNo == "")
                        {
                            result.Code = 2002;
                            result.Message = string.Format("托号[{0}]还未入柜，不可执行取消检验", package.Key);
                            return result; 
                        }
                        else if (packageAttr.Data.PackageState != EnumPackageState.Checked)
                        {
                            result.Code = 2003;
                            result.Message = string.Format("托号[{0}]当前状态[{1}]，非已检验状态,不可执行取消检验！", package.Key, package.PackageState.GetDisplayName());
                            return result;
                        }
                    }
                }
                else
                {
                    result.Code = 2000;
                    result.Message = "托号不可为空";
                    return result;
                }
                #endregion               

                #region 2.柜明细取消检验
                if (chest != null)
                {                   
                    fullChestQty = chest.Quantity;
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.ChestNo = '{0}'
                                                        AND EXISTS(FROM Package as p
                                                           WHERE p.Key=self.Key.ObjectNumber
                                                           AND p.PackageState='{1}')", chest.Key, Convert.ToInt32(EnumPackageState.Checked))
                    };
                    lstChestDetail = this.ChestDetailDataEngine.Get(cfg);
                    if (lstChestDetail != null && lstChestDetail.Count > 0)
                    {
                        checkedQtyOfChest = lstChestDetail.Count;
                    }
                    //更改托号状态为已入库，更改柜状态为检验中/入柜完成
                    package.PackageState = EnumPackageState.ToWarehouse;
                    package.Editor = userName;                          //编辑人             
                    package.EditTime = DateTime.Now;                    //编辑时间
                    if (checkedQtyOfChest == fullChestQty)
                    {
                        //更改柜状态为检验中
                        chest.ChestState = EnumChestState.Checking;
                    }
                    if (checkedQtyOfChest - 1 == 0)
                    {
                        //更改柜状态为入柜完成
                        chest.ChestState = EnumChestState.Packaged;
                    }
                    chest.Editor = userName;                          //编辑人             
                    chest.EditTime = DateTime.Now;                    //编辑时间
                }                
                #endregion

                #region 3.事务处理
                session = this.ChestDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                try
                {
                    //柜数据
                    this.ChestDataEngine.Update(chest, session);

                    //托数据
                    this.PackageDataEngine.Update(package, session);
                   
                    transaction.Commit();
                    session.Close();
                    result.Message = string.Format(@"托号[{0}]取消检验成功！", package.Key);
                    result.ObjectNo = Convert.ToInt32(chest.ChestState).ToString();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();

                    result.Code = 2000;
                    result.Message = string.Format(StringResource.Error, ex.Message);
                }
                #endregion
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        #endregion
    }
}