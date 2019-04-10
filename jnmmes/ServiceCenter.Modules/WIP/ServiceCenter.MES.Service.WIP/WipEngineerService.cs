using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.RBAC;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System.ServiceModel.Activation;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.EMS;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.BaseData;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Reflection;
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.Common.DataAccess.NHibernate;
using System.Configuration;
namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 实现批次出站服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class WipEngineerService : IWipEngineerContract
    {
        string specialOrderNumber = System.Configuration.ConfigurationSettings.AppSettings["SpecialOrderNumber"];

        protected Database _db;
        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        /// <summary>
        /// BIN规则数据访问类。
        /// </summary>
        public IBinRuleDataEngine BinRuleDataEngine
        {
            get;
            set;
        }


        /// <summary>
        ///  数据入BIN数据访问类。
        /// </summary>
        public IPackageBinDataEngine PackageBinDataEngine
        {
            get;
            set;
        }

        public IBaseAttributeValueDataEngine BaseAttributeValueDataEngine
        {

            get;
            set;
        }

        //混工单组规则数据访问类
        public IWorkOrderGroupDetailDataEngine WorkOrderGroupDetailDataEngine
        {
            get;
            set;
        }


        /// <summary>
        /// 包装数据访问类。
        /// </summary>
        public IPackageDataEngine PackageDataEngine { get; set; }
        /// <summary>
        /// 包装明细数据访问类。
        /// </summary>
        public IPackageCornerDetailDataEngine PackageCornerDetailDataEngine { get; set; }

        /// <summary>
        /// 工单等级包装规则数据访问类。
        /// </summary>
        public IWorkOrderGradeDataEngine WorkOrderGradeDataEngine
        {
            get;
            set;
        }

        public IWorkOrderAttributeDataEngine WorkOrderAttributeDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工序属性数据访问对象。
        /// </summary>
        public IRouteOperationAttributeDataEngine RouteOperationAttributeDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 铭牌打印日志记录
        /// </summary>
        public IPrintLabelLogDataEngine PrintLabelLogDataEngine { get; set; }

        /// <summary>
        ///  数据入BIN数据访问类。
        /// </summary>
        public IPackageCornerDataEngine PackageCornerDataEngine
        {
            get;
            set;
        }
        public WipEngineerService(ISessionFactory sf)
        {
            this.SessionFactory = sf;
            this._db = DatabaseFactory.CreateDatabase();
        }
        
        public bool CheckControlObject(string type, double value, double controlValue)
        {
            switch (type)
            {
                case ">":
                    return value > controlValue;
                case "<":
                    return value < controlValue;
                case "=":
                case "==":
                    return value == controlValue;
                case ">=":
                    return value >= controlValue;
                case "<=":
                    return value <= controlValue;
                case "<>":
                case "!=":
                    return value != controlValue;
                default:
                    break;
            }
            return false;
        }
        
        private bool getAttributeValueOfBoolean(IList<RouteStepAttribute> lstRouteStepAttributes, string attributeName)
        {
            bool blReturn = false;
            RouteStepAttribute stepAttr = lstRouteStepAttributes.FirstOrDefault(w => w.Key.AttributeName == attributeName);
            if (stepAttr != null)
            {
                if (bool.TryParse(stepAttr.Value, out blReturn) == false)
                {
                    blReturn = false;
                }
            }
            return blReturn;
        }

        private int getAttributeValueOfInt(IList<RouteStepAttribute> lstRouteStepAttributes, string attributeName)
        {
            int nReturn = 0;
            RouteStepAttribute stepAttr = lstRouteStepAttributes.FirstOrDefault(w => w.Key.AttributeName == attributeName);
            if (stepAttr != null)
            {
                if (int.TryParse(stepAttr.Value, out nReturn) == false)
                {
                    nReturn = 0;
                }
            }
            return nReturn;
        }
        
        /// <summary>
        /// 获取批次最新一次领料信息
        /// </summary>
        /// <param name="materialLot">原材料批次号</param>
        /// <param name="materialCode">产品物料号</param>
        /// <param name="orderNumber">工单号</param>
        /// <param name="db"></param>
        /// <returns></returns>
        private MaterialReceiptDetail getMaterialReceiptDetail(string materialLot, string orderNumber)
        {
            MaterialReceiptDetail mReceiptDetail = new MaterialReceiptDetail();

            PagingConfig cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format(@"MaterialLot='{0}'
                                        AND EXISTS(SELECT Key
                                                    FROM MaterialReceipt as p
                                                    WHERE p.OrderNumber='{1}'
                                                    AND p.Key=self.Key.ReceiptNo)"
                                        , materialLot
                                        , orderNumber
                                        ),
                OrderBy = "CreateTime "
            };

            IList<MaterialReceiptDetail> lstMaterialReceiptDetail = this.MaterialReceiptDetailDataEngine.Get(cfg);
            if (lstMaterialReceiptDetail != null || lstMaterialReceiptDetail.Count > 0)
            {
                mReceiptDetail = lstMaterialReceiptDetail.FirstOrDefault();
            }
            return mReceiptDetail;
        }

        public IList<MaterialLoadingDetail> GetMaterialLoadingDetailEx(string MaterialCode, string RouteStepName, string EquipMentCode, string MaterialLot, string OrderNumber)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            IList<MaterialLoadingDetail> lstMaterialLoadingDetail = null;
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"select t.LOADING_TIME,t1.* from [dbo].[LSM_MATERIAL_LOADING] t 
                                                            left join [dbo].[LSM_MATERIAL_LOADING_DETAIL] t1 on t.LOADING_KEY=t1.LOADING_KEY 
                                                            where t.ROUTE_OPERATION_NAME='{4}' and t.EQUIPMENT_CODE='{1}' and  t1.MATERIAL_LOT='{2}' 
                                                            and t1.ORDER_NUMBER='{3}' and t1.MATERILA_CODE LIKE '{0}%' and t1.CURRENT_QTY > 0
                                                            ", MaterialCode, EquipMentCode, MaterialLot, OrderNumber, RouteStepName);
                    result.Data = _db.ExecuteDataSet(cmd);
                    if (result.Data != null && result.Data.Tables.Count > 0 && result.Data.Tables[0] != null && result.Data.Tables[0].Rows.Count > 0)
                    {
                        string strMaterialCode = result.Data.Tables[0].Rows[0]["MATERILA_CODE"].ToString();
                        string strMaterialLot = result.Data.Tables[0].Rows[0]["MATERIAL_LOT"].ToString();
                        string strOrderNumber = result.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString();
                        string strLoadingTime = result.Data.Tables[0].Rows[0]["LOADING_TIME"].ToString();
                        lstMaterialLoadingDetail = GetMaterialLoadingDetail(strMaterialCode, RouteStepName, EquipMentCode, strMaterialLot, strOrderNumber, strLoadingTime);
                    }

                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return lstMaterialLoadingDetail;
        }

        public IList<MaterialLoadingDetail> GetMaterialLoadingDetail(string MaterialCode, string RouteStepName, string EquipMentCode, string MaterialLot, string OrderNumber, string LoadingTime)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            IList<MaterialLoadingDetail> lstMaterialLoadingDetail = null;
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"select T.LOADING_KEY LoadingKey,T.ITEM_NO ItemNo,T.STORE_NAME LineStoreName,T.MATERILA_CODE MaterialCode,T.MATERIAL_LOT MaterialLot,T.LOADING_QTY LoadingQty,
                                                             T.UNLOADING_QTY UnloadingQty,T.CURRENT_QTY CurrentQty,T.CREATOR Creator,T.CREATE_TIME CreateTime,T.EDITOR Editor,T.EDIT_TIME EditTime,T.ORDER_NUMBER OrderNumber from (select  '0' indexMaterial, t.LOADING_TIME,t1.* from [dbo].[LSM_MATERIAL_LOADING] t 
                                                            left join [dbo].[LSM_MATERIAL_LOADING_DETAIL] t1 on t.LOADING_KEY=t1.LOADING_KEY 
                                                            where t.ROUTE_OPERATION_NAME='{4}' and t.EQUIPMENT_CODE='{1}' and  t1.MATERIAL_LOT='{2}' 
                                                            and t1.ORDER_NUMBER='{3}' and t1.MATERILA_CODE LIKE '{0}%' and t1.CURRENT_QTY > 0
                                                            union 
                                                            select '1' indexMaterial, t2.LOADING_TIME,t3.* from [dbo].[LSM_MATERIAL_LOADING] t2 
                                                            left join [dbo].[LSM_MATERIAL_LOADING_DETAIL] t3 on t2.LOADING_KEY =t3.LOADING_KEY
                                                            where t2.ROUTE_OPERATION_NAME='{4}' and t3.ORDER_NUMBER='{3}' and t3.MATERILA_CODE LIKE '{0}%' and t2.EQUIPMENT_CODE='{1}'
                                                            and t3.ORDER_NUMBER='{3}'  AND t3.CURRENT_QTY>0 and t2.LOADING_TIME>= '{5}' and t3.MATERIAL_LOT <>'{2}') T  order by T.indexMaterial,T.LOADING_TIME ", MaterialCode, EquipMentCode, MaterialLot, OrderNumber, RouteStepName, LoadingTime);
                    result.Data = _db.ExecuteDataSet(cmd);
                    if (result.Data != null && result.Data.Tables.Count > 0 && result.Data.Tables[0] != null && result.Data.Tables[0].Rows.Count > 0)
                    {
                        if (result.Data.Tables[0].Rows[0]["MaterialLot"].ToString() != MaterialLot)
                        {
                            lstMaterialLoadingDetail = null;
                        }
                        else
                        {
                            lstMaterialLoadingDetail = DataSetToIList<MaterialLoadingDetail, MaterialLoadingDetailKey>(result.Data, 0);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return lstMaterialLoadingDetail;
        }

        public IList<T> DataSetToIList<T, K>(DataSet ds, int tableIndex)
        {
            if (ds == null || ds.Tables.Count < 0)
                return null;
            if (tableIndex > ds.Tables.Count - 1)
                return null;
            if (tableIndex < 0)
                tableIndex = 0;
            DataTable dt = ds.Tables[tableIndex];

            IList<T> result = new List<T>();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                Object _k = (K)Activator.CreateInstance(typeof(K));

                PropertyInfo[] propertys = _k.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (pi.Name.Equals(dt.Columns[i].ColumnName))
                        {
                            if (pi.PropertyType.Name == "Double")
                            {
                                if (dt.Rows[j][i] != DBNull.Value)
                                    pi.SetValue(_k, Convert.ToDouble(dt.Rows[j][i]), null);
                                else
                                    pi.SetValue(_k, null, null);
                            }
                            else
                            {
                                if (dt.Rows[j][i] != DBNull.Value)
                                    pi.SetValue(_k, dt.Rows[j][i]);
                                else
                                    pi.SetValue(_k, null, null);
                            }
                            break;
                        }
                    }
                }
                K _key = (K)_k;

                propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (pi.Name == "Key")
                    {
                        pi.SetValue(_t, _key, null);
                        continue;
                    }
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (pi.Name.Equals(dt.Columns[i].ColumnName))
                        {
                            if (pi.PropertyType.Name == "Double")
                            {
                                if (dt.Rows[j][i] != DBNull.Value)
                                    pi.SetValue(_t, Convert.ToDouble(dt.Rows[j][i]), null);
                                else
                                    pi.SetValue(_t, null, null);
                            }
                            else
                            {
                                if (dt.Rows[j][i] != DBNull.Value)
                                    pi.SetValue(_t, dt.Rows[j][i], null);
                                else
                                    pi.SetValue(_t, null, null);
                            }
                            break;
                        }
                    }
                }
                result.Add(_t);
            } return result;
        }


        /// <summary>
        /// 获取批次属性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotAttribute&gt;" />,批次属性数据集合。</returns>
        public MethodReturnResult<IList<PrintLabelLog>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PrintLabelLog>> result = new MethodReturnResult<IList<PrintLabelLog>>();
            try
            {
                result.Data = this.PrintLabelLogDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }


        public MethodReturnResult UnDoPackageCorner(string lotNumber)
        {
            IList<PackageCornerDetail> lstPackageCornerDetailForUpdateObj = new List<PackageCornerDetail>();
            List<PackageCornerDetail> lstPackageCornerDetailForDelete = new List<PackageCornerDetail>();
            List<PackageCorner> lstPackageCornerForUpdate = new List<PackageCorner>();
            List<PackageCorner> lstPackageCornerForDelete= new List<PackageCorner>();
            MethodReturnResult result = new MethodReturnResult();
            ISession session = this.LotDataEngine.SessionFactory.OpenSession(); ;
            ITransaction transaction = session.BeginTransaction();
            try
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.LotNumber='{0}'"
                                            , lotNumber),
                    OrderBy = "CreateTime desc"
                };
                IList<PackageCornerDetail> lstPackageCornerDetail = this.PackageCornerDetailDataEngine.Get(cfg);//查找批次号所在托
                if (lstPackageCornerDetail != null && lstPackageCornerDetail.Count > 0)
                {
                    PackageCornerDetail packageCornerDetail = lstPackageCornerDetail.FirstOrDefault();
                    if (packageCornerDetail != null)
                    {
                        string packageKey = packageCornerDetail.Key.PackageKey;
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@"Key.PackageKey='{0}'"
                                                    , packageKey),
                            OrderBy = "ItemNo ASC"
                        };
                        IList<PackageCornerDetail> lstPackageCornerDetailForUpdate = this.PackageCornerDetailDataEngine.Get(cfg);//查找批次号所在托的详细信息
                        PackageCorner packageCorner = this.PackageCornerDataEngine.Get(packageKey);
                        foreach (PackageCornerDetail item in lstPackageCornerDetailForUpdate)
                        {
                            if (item.Key.LotNumber == lotNumber)
                            {
                                lstPackageCornerDetailForDelete.Add(item);
                            }
                        }
                        foreach (PackageCornerDetail item in lstPackageCornerDetailForDelete)
                        {
                            lstPackageCornerDetailForUpdate.Remove(item);//删除要撤销批次
                        }

                        int itemNoCorner = 0;
                        foreach (PackageCornerDetail packageCornerDetailObj in lstPackageCornerDetailForUpdate)//重新排序新的批次
                        {
                            itemNoCorner++;
                            if (packageCornerDetailObj.ItemNo == itemNoCorner)
                            {
                                continue;
                            }
                            PackageCornerDetail packageCornerDetailObjUpdate = packageCornerDetailObj.Clone() as PackageCornerDetail;
                            packageCornerDetailObjUpdate.ItemNo = itemNoCorner;
                            lstPackageCornerDetailForUpdateObj.Add(packageCornerDetailObjUpdate);
                        }
                        if (packageCorner != null)
                        {
                            PackageCorner packageCornerForUpdate = packageCorner.Clone() as PackageCorner;
                            packageCornerForUpdate.BinQty = packageCornerForUpdate.BinQty - 1;
                            if (packageCornerForUpdate.BinQty == 0)
                            {
                               
                                lstPackageCornerForDelete.Add(packageCornerForUpdate);
                            }
                            //packageBinForUpdate.BinPackaged = EnumBinPackaged.UnFinished;
                            else
                            {
                                lstPackageCornerForUpdate.Add(packageCornerForUpdate);
                            }
                        }
                    }
                }
                foreach (PackageCornerDetail packageCornerDetail in lstPackageCornerDetailForDelete)
                {
                    this.PackageCornerDetailDataEngine.Delete(packageCornerDetail.Key, session);
                }

                foreach (PackageCornerDetail item in lstPackageCornerDetailForUpdateObj)
                {
                    this.PackageCornerDetailDataEngine.Update(item, session);
                }

                foreach (PackageCorner item in lstPackageCornerForUpdate)
                {
                    this.PackageCornerDataEngine.Update(item, session);
                }


                foreach (PackageCorner item in lstPackageCornerForDelete)
                {
                    this.PackageCornerDataEngine.Delete(item.Key, session);
                }
               
                transaction.Commit();
                session.Close();
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
                transaction.Rollback();
                session.Close();
            }
            return result;
        }

        public MethodReturnResult UpdatePrintLabelLog(PrintLabelLog printLabelLog)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession session = this.LotDataEngine.SessionFactory.OpenSession(); ;
            ITransaction transaction = session.BeginTransaction();
            try
            {
                PrintLabelLogDataEngine.Update(printLabelLog, session);
                transaction.Commit();
                session.Close();
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
                transaction.Rollback();
                session.Close();
            }
            return result;
        }

        /// <summary>
        /// 检查入的组件是否与该托的信息一致,以第一块组件为例（yanshan.xiao）
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="lotNumber"></param>
        /// <param name="isInPackage"></param>
        /// <returns></returns>

        private MethodReturnResult CheckLotInPackageRule(Lot lot, string key, out bool isInPackage)
        {
            MethodReturnResult result = new MethodReturnResult();
            bool isPackageLimitedForWorkOrder = false;
            PagingConfig cfg = null;

            try
            {
                isInPackage = false;
                //判断批次是否存在。
                if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                {
                    result.Code = 1001;
                    result.Message = string.Format("批次：（{0}）不存在！", lot.Key);
                    return result;
                }

                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.PackageKey='{0}'", key),
                    OrderBy = "ItemNo asc"
                };

                IList<PackageCornerDetail> lstPackageCornerDetail = this.PackageCornerDetailDataEngine.Get(cfg);
                //判断包装号是否存在。
                if (lstPackageCornerDetail == null || lstPackageCornerDetail.Count == 0)
                {
                    result.Code = 1002;
                    result.Message = string.Format("包装号：（{0}）不存在！", key);
                    return result;
                }
                PackageCorner packageCorner = this.PackageCornerDataEngine.Get(key);
                //包装是否包装状态
                if (packageCorner.BinPackaged != EnumCornerPackaged.UnFinished)
                {
                    result.Code = 1000;
                    result.Message = string.Format("包装号：（{0}）已完成包装！", key);
                    return result;
                }

                //判断产品是否一致，必须一致方可入托
                if (lstPackageCornerDetail[0].MaterialCode != lot.MaterialCode && lstPackageCornerDetail[0].MaterialCode != "")
                {
                    result.Code = 0;
                    result.Message = string.Format("包装物料({0})与批次物料({1})不一致！",
                                                   lstPackageCornerDetail[0].MaterialCode,
                                                   lot.MaterialCode);
                    return result;
                }

                #region 判断工单是否一致及是否可以混工单
                if (lot.OrderNumber != lstPackageCornerDetail[0].OrderNumber)
                {
                    //批次工单混工单属性
                    WorkOrderAttribute lotWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                    {
                        OrderNumber = lot.OrderNumber,
                        AttributeName = "PackageLimited"
                    });

                    //未设置默认为允许混工单(false)
                    if (lotWorkOrderAttribute == null || !bool.TryParse(lotWorkOrderAttribute.AttributeValue, out isPackageLimitedForWorkOrder))
                    {
                        isPackageLimitedForWorkOrder = false;
                    }

                    if (isPackageLimitedForWorkOrder == true)
                    {
                        result.Code = 0;
                        result.Message = string.Format("批次：（{0}）所在工单（{1}）不允许混工单！", lot.Key, lot.OrderNumber);

                        return result;
                    }

                    //托工单混托属性
                    WorkOrderAttribute packageWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                    {
                        OrderNumber = lstPackageCornerDetail[0].OrderNumber,
                        AttributeName = "PackageLimited"
                    });

                    //未设置默认为允许混工单(false)
                    if (packageWorkOrderAttribute == null || !bool.TryParse(packageWorkOrderAttribute.AttributeValue, out isPackageLimitedForWorkOrder))
                    {
                        isPackageLimitedForWorkOrder = false;
                    }

                    if (isPackageLimitedForWorkOrder == true)
                    {
                        result.Code = 0;
                        result.Message = string.Format("托：（{0}）所在工单（{1}）不允许混工单！", key, lstPackageCornerDetail[0].OrderNumber);
                        return result;
                    }

                    #region 判断要包装批次的工单是否设置混工单组
                    cfg = new PagingConfig()
                    {
                        Where = string.Format(@"Key.OrderNumber = '{0}'", lot.OrderNumber)
                    };
                    IList<WorkOrderGroupDetail> lstLotWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                    //批次所在工单设置了混工单组
                    if (lstLotWorkOrderGroupDetail != null && lstLotWorkOrderGroupDetail.Count > 0)
                    {
                        #region 判断要托工单是否设置混工单组
                        cfg = new PagingConfig()
                        {
                            Where = string.Format(@"Key.OrderNumber = '{0}'", lstPackageCornerDetail[0].OrderNumber)
                        };
                        IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                        //托工单设置了混工单组
                        if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                        {
                            if (lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString() != lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString())
                            {
                                result.Code = 0;
                                result.Message = string.Format("托：（{0}）所在工单（{1} 设置的混工单组（{2}）与入托批次（{3}）所在工单（{4}）设置的混工单组（{5}）不一致！",
                                                                lstPackageCornerDetail[0].Key.PackageKey, lstPackageCornerDetail[0].OrderNumber, lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString(),
                                                                lot.Key, lot.OrderNumber, lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString());
                                return result;
                            }
                        }
                        //托工单没设混工单组
                        else
                        {
                            result.Code = 0;
                            result.Message = string.Format("托：（{0}）所在工单（{1} 未设置混工单组规则，但要入托批次（{2}）所在工单（{3}）设置了混工单组！",
                                                            lstPackageCornerDetail[0].Key.PackageKey, lstPackageCornerDetail[0].OrderNumber, lot.Key, lot.OrderNumber);
                            return result;
                        }
                        #endregion
                    }
                    //批次所在工单没设混工单组
                    else
                    {
                        #region 判断要托工单是否设置混工单组
                        cfg = new PagingConfig()
                        {
                            Where = string.Format(@"Key.OrderNumber = '{0}'", lstPackageCornerDetail[0].OrderNumber)
                        };
                        IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                        //托工单设置了混工单组
                        if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                        {
                            result.Code = 0;
                            result.Message = string.Format("托：（{0}）所在工单（{1} 已设置混工单组规则，但要入托批次（{2}）所在工单（{3}）未设置混工单组！",
                                                            lstPackageCornerDetail[0].Key.PackageKey, lstPackageCornerDetail[0].OrderNumber, lot.Key, lot.OrderNumber);
                            return result;
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                #region 检查电池片供应商是否可以混装
                //取得是否允许不同的电池片供应商是否可以混装
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.CategoryName='{0}' 
                                        AND Key.AttributeName='{1}'"
                                        , "SystemParameters"
                                        , "PackageChkMaterialSupplier"),
                    OrderBy = "Key.ItemOrder"
                };

                //取得电池片混包标志
                bool blChkSupplierCode = false;
                IList<BaseAttributeValue> lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);

                if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
                {
                    if (String.IsNullOrEmpty(lstBaseAttributeValues[0].Value) != true)
                    {
                        Boolean.TryParse(lstBaseAttributeValues[0].Value, out blChkSupplierCode);
                    }
                }

                if (blChkSupplierCode)      //需要进行供应商的检测
                {
                    //取得批次电池片厂商
                    string strSupplierCodeForLot = "";      //批次电池片厂商
                    string strSupplierCodeForLotKey = "";      //托电池片厂商
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.LotNumber='{0}' and MaterialCode like '%{1}%'",
                                                lot.Key,
                                                "110")

                    };

                    IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg);
                    if (lstLotBom != null && lstLotBom.Count > 0)
                    {
                        //取得批次对应电池片供应商
                        strSupplierCodeForLot = lstLotBom[0].SupplierCode;
                    }

                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.LotNumber='{0}' and MaterialCode like '%{1}%'",
                                                lstPackageCornerDetail[0].Key.LotNumber,
                                                "110")

                    };

                    IList<LotBOM> lstLotBomKey = this.LotBOMDataEngine.Get(cfg);
                    if (lstLotBomKey != null && lstLotBomKey.Count > 0)
                    {
                        //取得批次对应电池片供应商
                        strSupplierCodeForLotKey = lstLotBom[0].SupplierCode;
                    }
                    //判断电池片供应商是否一致
                    if (string.Compare(strSupplierCodeForLotKey, strSupplierCodeForLot, true) != 0)
                    {
                        result.Code = 0;
                        result.Message = string.Format("托（{0}）电池片供应商（{1}）与批次（{2}）电池片供应商（{3}）不一致。",
                                                        key,
                                                       strSupplierCodeForLotKey,
                                                        lot.Key,
                                                        strSupplierCodeForLot);
                        return result;
                    }
                }
                #endregion

                //设置返回参数
                isInPackage = true;                                             //可入托标志
                result.ObjectNo = ((int)packageCorner.BinQty + 1).ToString();    //最后包装序列号
                result.Code = 0;

                return result;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();

                isInPackage = false;

                return result;
            }
        }
        /// <summary>
        /// 异常BIN包装护角操作
        /// </summary>
        /// <param name="lot"></param>
        /// <returns></returns>
        public MethodReturnResult ExecuteInAbnormalBIN(Lot lot, string PackageLine)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code=0
            };
            PagingConfig cfg = null;
            ISession session = this.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            PackageCorner corner = null;//创建新的BIN
            PackageCorner packageCorner = null;//原始BIN数据
            PackageCornerDetail detail=null;//托的详细信息
            int iQtyMax = 0;                                    //BIN最大数量
            try
            {
                //取得工单设置最大入托数
                CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey> commonObjectDataEngine;
                commonObjectDataEngine = new CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey>(LotDataEngine.SessionFactory);

                WorkOrderRuleKey workOrderRuleKey = new WorkOrderRuleKey
                {
                    OrderNumber = lot.OrderNumber,
                    MaterialCode = lot.MaterialCode
                };

                WorkOrderRule workOrderRule = commonObjectDataEngine.Get(workOrderRuleKey);
                if (workOrderRule != null)
                {
                    int.TryParse(workOrderRule.FullPackageQty.ToString(), out iQtyMax);
                }
                else
                {
                    result.Code = 1002;
                    result.Message = "最大满托数未设置！";
                    return result;
                }
                ProductionLine productionLine = this.ProductionLineDataEngine.Get(PackageLine);
                if (productionLine != null && productionLine.Attr2 != "" && productionLine.Attr2 != null)
                {
                    string abnormalBinNo = productionLine.Attr2;//找到异常BIN号
                    cfg = new PagingConfig()
                    {
                        Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}'", abnormalBinNo, PackageLine),
                        OrderBy = "CreateTime desc",
                        IsPaging = false
                    };
                    IList<PackageCorner> listPackageCorner = this.PackageCornerDataEngine.Get(cfg);//找到异常BIN队列当前BIN的数据
                    if (listPackageCorner.Count == 0)
                    {
                         corner=new PackageCorner()
                         {
                             Key =Guid.NewGuid().ToString(),
                            BinMaxQty=iQtyMax,
                            BinNo=abnormalBinNo,
                            BinPackaged=EnumCornerPackaged.UnFinished,
                            BinState=0,
                            BinQty=1,
                            PackageLine = PackageLine,
                            LockFlag = 0,
                            CreateTime=DateTime.Now,
                            Creator = PackageLine,
                            EditTime=DateTime.Now,
                            Editor=PackageLine
                         };
                        PackageCornerDetailKey packageCornerDetailKey=new PackageCornerDetailKey()
                        {
                         LotNumber=lot.Key,
                          PackageKey=corner.Key
                        };
                        detail = new PackageCornerDetail
                        {
                            Key = packageCornerDetailKey,
                            MaterialCode = lot.MaterialCode,
                            OrderNumber = lot.OrderNumber,
                            PackageFlag = 1,
                            PackageLine = PackageLine,
                            ItemNo = corner.BinQty,
                            CreateTime = DateTime.Now,
                            Creator = PackageLine
                        };
                         result.Message = "包";
                    }
                    else
                    {
                        packageCorner = listPackageCorner.FirstOrDefault();
                        if (packageCorner.BinPackaged == EnumCornerPackaged.Finished)
                        {
                            corner = new PackageCorner()
                            {
                                Key = Guid.NewGuid().ToString(),
                                BinMaxQty = iQtyMax,
                                BinNo = abnormalBinNo,
                                BinPackaged = EnumCornerPackaged.UnFinished,
                                BinState = 0,
                                BinQty = 1,
                                PackageLine = PackageLine,
                                LockFlag = 0,
                                CreateTime = DateTime.Now,
                                Creator = PackageLine,
                                EditTime = DateTime.Now,
                                Editor = PackageLine
                            };
                            PackageCornerDetailKey packageCornerDetailKey = new PackageCornerDetailKey()
                            {
                                LotNumber = lot.Key,
                                PackageKey = corner.Key
                            };
                            detail = new PackageCornerDetail
                            {
                                Key = packageCornerDetailKey,
                                MaterialCode = lot.MaterialCode,
                                OrderNumber = lot.OrderNumber,
                                PackageFlag = 1,
                                PackageLine = PackageLine,
                                ItemNo = corner.BinQty,
                                CreateTime = DateTime.Now,
                                Creator = PackageLine
                            };
                            packageCorner.BinState=1;
                            packageCorner.EditTime = DateTime.Now;
                            result.Message = "包";
                        }
                        else
                        {
                            EnumCornerDetailPackaged IsPackaged;
                            packageCorner.BinQty = packageCorner.BinQty + 1;
                            packageCorner.EditTime = DateTime.Now;
                            if (packageCorner.BinQty == packageCorner.BinMaxQty)
                            {
                                packageCorner.BinPackaged = EnumCornerPackaged.Finished;
                                IsPackaged = EnumCornerDetailPackaged.Packaged;
                            }
                            else
                            {
                                if (packageCorner.BinQty % 2 == 0)
                                {
                                    IsPackaged = EnumCornerDetailPackaged.UnPackaged;
                                }
                                else
                                {
                                    IsPackaged = EnumCornerDetailPackaged.Packaged;
                                }

                            }
                            if (IsPackaged == EnumCornerDetailPackaged.UnPackaged)
                            {

                                result.Message = "不包";
                            }
                            else
                            {

                                result.Message = "包";
                            }
                            PackageCornerDetailKey packageCornerDetailKey = new PackageCornerDetailKey()
                            {
                                LotNumber = lot.Key,
                                PackageKey = packageCorner.Key
                            };
                            detail = new PackageCornerDetail
                            {
                                Key = packageCornerDetailKey,
                                MaterialCode = lot.MaterialCode,
                                OrderNumber = lot.OrderNumber,
                                PackageFlag = (int)IsPackaged,
                                PackageLine = PackageLine,
                                ItemNo = packageCorner.BinQty,
                                CreateTime = DateTime.Now,
                                Creator = PackageLine
                            };
                        }
                    }
                    #region ***创建事物对象***



                    if (packageCorner != null)                                                 //更新原BIN
                    {
                        if (corner != null)//插入新BIN
                        {
                            this.PackageCornerDataEngine.Insert(corner, session);
                        }
                        this.PackageCornerDataEngine.Update(packageCorner, session);
                    }
                    else
                    {
                        if (corner != null)//插入新BIN
                        {
                            this.PackageCornerDataEngine.Insert(corner, session);
                        }
                    }
               
                    if(detail!=null)//插入新BIN
                    {
                        this.PackageCornerDetailDataEngine.Insert(detail, session);
                    }
                    transaction.Commit();
                    session.Close();
                    #endregion
                   
                }
                else
                {
                    result.Code = 1000;
                    result.Message =string.Format("请IT部门设置线别{0}的异常BIN号",PackageLine);
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
                transaction.Rollback();
                session.Close();
                return result;
            }

        }

        /// <summary>
        /// 包装护角操作
        /// </summary>
        /// <param name="lot"></param>
        /// <returns></returns>
        public MethodReturnResult ExecuteInPackageDetail(Lot lot,string packageLine)
        {
            #region 取得并判断批次信息

            MethodReturnResult result = new MethodReturnResult();
            //判断批次是否存在。
            if (lot == null || lot.Status == EnumObjectStatus.Disabled)
            {
                result.Code = 1003;
                result.Message = string.Format("批次：（{0}）不存在！", lot.Key);
                return result;
            }

            //批次已撤销
            if (lot.DeletedFlag == true)
            {
                result.Code = 1004;
                result.Message = string.Format("批次：（{0}）已删除！", lot.Key);
                return result;
            }
            //批次已暂停
            if (lot.HoldFlag == true)
            {
                result.Code = 1005;
                result.Message = string.Format("批次：（{0}）已暂停！", lot.Key);
                return result;
            }

            //判断Lot的等级是否是A级
            if (string.IsNullOrEmpty(lot.Grade) || lot.Grade.ToUpper() != "A")
            {
                result.Code = 1006;
                result.Message = string.Format("【异常BIN】批次（{0}）等级不是A级。", lot.Key);
                return result;
            }
            #endregion


            #region 检查工序是否是包装工序
            //取得当前批次所在站别是否包装流程站别属性
            RouteOperationAttribute roAttr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
            {
                RouteOperationName = lot.RouteStepName,     //当前批次所在工序
                AttributeName = "IsPackageOperation"        //包装站属性名
            });

            //如果没有设置为包装工序，则直接返回。
            if (roAttr == null)
            {
                result.Code = 1007;
                result.Message = string.Format("产品：({0})在({1})工序，请确认。", lot.Key, lot.RouteStepName);

                return result;
            }
          

            PagingConfig cfg = null;
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                string strLotNumber;                                //批次代码
                string strPackagekey = string.Empty;                //托号
                int iQtyMax = 0;                                    //BIN最大数量
                EnumCornerPackaged IsFinishPackage = 0;              //BIN是否完成包装 0 - 包装中 1 - 完成包装
                EnumCornerDetailPackaged IsPackaged = 0;             //1包护角、0不包护角
                PackageCorner packageCornerObj = null;               //Bin对象
                PackageCornerDetail packageCornerDetail = null;      //包装护角明细对象
                IList<PackageCorner> lstPackageBin;                  //包装护角对象列表
                string strBinNo = "";                                //Bin号
                DateTime now = DateTime.Now;                         //当前时间
                int intInBinOrder = 1;                               //入Bin序列号
                PackageCorner packageCornerObjHis = null;                 //历史Bin对象
                int intNewBinCount = 0;                     //新规则未创建Bin记录数
                #region 取得批次信息
                strLotNumber = lot.Key;
                //取得工单设置最大入托数
                CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey> commonObjectDataEngine;
                commonObjectDataEngine = new CommonObjectDataEngine<WorkOrderRule, WorkOrderRuleKey>(LotDataEngine.SessionFactory);

                WorkOrderRuleKey workOrderRuleKey = new WorkOrderRuleKey
                {
                    OrderNumber = lot.OrderNumber,
                    MaterialCode = lot.MaterialCode
                };

                WorkOrderRule workOrderRule = commonObjectDataEngine.Get(workOrderRuleKey);
                if (workOrderRule != null)
                {
                    int.TryParse(workOrderRule.FullPackageQty.ToString(), out iQtyMax);
                }
                else
                {
                    result.Code = 1002;
                    result.Message = "最大满托数未设置！";
                    return result;
                }
                #endregion

                #endregion
                #region 判断批次是否执行过包护角动作
                cfg = new PagingConfig()
                {
                    Where = string.Format("Key.LotNumber='{0}'", strLotNumber),

                };
                IList<PackageCornerDetail> lstPackageCornerDetail = this.PackageCornerDetailDataEngine.Get(cfg);
                if (lstPackageCornerDetail.Count > 0)
                {   
                    EnumCornerDetailPackaged enumCornerDetailPackaged=(EnumCornerDetailPackaged)lstPackageCornerDetail[0].PackageFlag;
                    result.Code = 1018;
                    if(enumCornerDetailPackaged==EnumCornerDetailPackaged.Packaged)
                    {
                        result.Message = string.Format("批次：（{0}） 已经进行过包护角<font size='20' color='green'>(包护角)</font>！", strLotNumber);
                    }
                    else
                    {
                        result.Message = string.Format("批次：（{0}） 已经进行过包护角<font size='20' color='green'>(不包护角)</font>！", strLotNumber);
                    }
                 
                    return result;
                }

                #endregion
                #region 获取批次IV测试数据
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", strLotNumber)
                };

                IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);

                //检查批次特性和包装特性是否匹配。
                string powersetCode = string.Empty;
                int powersetCodeItemNo = -1;
                string powersetSubCode = string.Empty;

                if (lstTestData.Count > 0)
                {
                    powersetCode = lstTestData[0].PowersetCode;                 //分档组
                    powersetCodeItemNo = lstTestData[0].PowersetItemNo ?? -1;   //分档代码
                    powersetSubCode = lstTestData[0].PowersetSubCode;           //子分档代码
                }
                else
                {
                    result.Code = 1010;
                    result.Message = string.Format("批次：（{0}） IV测试数据不存在！", strLotNumber);
                    return result;
                }
                #endregion
                //Bin测试规则参数
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.PackageLine='{0}'  
                                          AND Key.PsCode='{1}' 
                                          AND Key.PsItemNo='{2}'  
                                          AND Key.PsSubCode='{3}' 
                                          AND Key.Color='{4}'
                                          AND ( Key.WorkOrderNumber='{5}' or Key.WorkOrderNumber='{6}')",
                                          packageLine,
                                          powersetCode,
                                          powersetCodeItemNo,
                                          powersetSubCode,
                                          lot.Color,
                                          lot.OrderNumber,
                                          "*"),
                    OrderBy = " Key.BinNo "
                };


                //取得Bin规则清单
                IList<BinRule> lstBinRules = this.BinRuleDataEngine.Get(cfg);

 

                if (lstBinRules.Count > 0)
                {
                    //遍历所有的节点后优先入未满Bin，在空Bin中选择编辑时间最晚Bin
                    DateTime? dtEditTime = DateTime.Now;
                    int iCount = 0;
                    bool isInPackage = false;
                    for (int i = 0; i < lstBinRules.Count; i++)
                    {
                        //取得包装Bin信息
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("BinNo='{0}' and PackageLine='{1}'",
                                                    lstBinRules[i].Key.BinNo,
                                                   packageLine),
                            OrderBy = " EditTime desc"
                        };

                        lstPackageBin = PackageCornerDataEngine.Get(cfg);

                        if (lstPackageBin == null || lstPackageBin.Count == 0)  //入Bin记录未生成，在无未满托时优先入新Bin
                        {
                            if (intNewBinCount == 0)
                            {
                                //取得Bin信息
                                strBinNo = lstBinRules[i].Key.BinNo;            //规则设置对应Bin号
                                intInBinOrder = 1;                              //入Bin序列号
                                strPackagekey = Guid.NewGuid().ToString();
                                //结果计数器
                                iCount++;
                            }

                            intNewBinCount++;
                        }
                        else
                        {
                             
                            if (lstPackageBin[0].LockFlag == 1)
                            {
                                result.Code = 1019;
                                result.Message = string.Format("BIN：（{0}） 已经锁定，请抬下组件稍后上线！", lstBinRules[i].Key.BinNo);
                                return result;
                            }
                            //判断Bin是否包装完成
                            if (lstPackageBin[0].BinPackaged == EnumCornerPackaged.Finished)//如果两个批次都满了，则更新编辑时间比较早的托（yanshan.xiao注释）
                            {
                                //判断高优先级新Bin规则是否存在
                                if (intNewBinCount == 0)
                                {
                                    //判断是否是否最晚时间
                                    if (lstPackageBin[0].EditTime <= dtEditTime)
                                    {
                                        //取得Bin信息
                                        strBinNo = lstPackageBin[0].BinNo;              //Bin号
                                        intInBinOrder = 1;                                  //入Bin序列号
                                        dtEditTime = lstPackageBin[0].EditTime;             //最后编辑时间
                                        strPackagekey = Guid.NewGuid().ToString();
                                        //结果计数器
                                        iCount++;
                                    }
                                }
                            }
                            else
                            {
                                //判断是否满足入托规则 
                                result = CheckLotInPackageRule(lot, lstPackageBin[0].Key, out isInPackage);
                                if (result.Code > 0)        //产生错误
                                {
                                    return result;
                                }
                                else
                                {
                                    if (isInPackage)
                                    {
                                        //若为未满Bin优先入未满Bin
                                        //取得Bin信息
                                        strBinNo = lstPackageBin[0].BinNo;          //Bin号
                                        intInBinOrder = int.Parse(result.ObjectNo);     //将入托序列号
                                        strPackagekey = lstPackageBin[0].Key;  //将入托包装号
                                        dtEditTime = lstPackageBin[0].EditTime;         //最后编辑时间

                                        //结果计数器
                                        iCount++;

                                        //退出搜寻
                                        i = lstBinRules.Count;
                                    }
                                    else
                                    {
                                        result.Message = "";
                                    }
                                }
                            }
                        }
                    }

                    if (iCount == 0)    //未找到符合条件Bin
                    {
                        result.Code = 1010;
                        result.Message = string.Format("【异常BIN】批次：（{0}） 无对应Bin！", lot.Key);
                        return result;
                    }
                }
                else
                {
                    result.Code = 1010;
                    result.Message = string.Format("【异常BIN】批次：（{0}） 无对应Bin！", lot.Key);
                    return result;
                }

                #region 创建托包装对象
                //判断托是否完成包装（最后一块入托）
                if (iQtyMax > intInBinOrder)
                {
                    IsFinishPackage = EnumCornerPackaged.UnFinished;
                }
                else
                {
                    IsFinishPackage = EnumCornerPackaged.Finished;
                }

                bool isNotOrderNumberSpecial = true;  //批次工单非使用首尾包护角且隔块包护角
                string[] lstOrderNumber = specialOrderNumber.Split(',');
                foreach (string orderNumber in lstOrderNumber)
                {
                    if (lot.OrderNumber == orderNumber)
                    {
                        isNotOrderNumberSpecial = false;
                        break;
                    }
                }

                //判断托是否包装护角
                if ((iQtyMax == intInBinOrder || intInBinOrder == 1) && isNotOrderNumberSpecial)
                {
                    IsPackaged = EnumCornerDetailPackaged.Packaged;                   
                }
                else
                {
                    if (intInBinOrder % 2 == 0)
                    {
                        IsPackaged = EnumCornerDetailPackaged.UnPackaged;
                    }
                    else
                    {
                        IsPackaged = EnumCornerDetailPackaged.Packaged;
                    }

                }
                #endregion

                #region 创建托包装明细对象
                packageCornerDetail = new PackageCornerDetail()
                {
                    Key = new PackageCornerDetailKey()
                    {
                        PackageKey = strPackagekey,
                        LotNumber = lot.Key
                    },
                    ItemNo = intInBinOrder,                    //入托项目号（入托顺序）
                    Creator = lot.EquipmentCode,                //创建人
                    CreateTime = now,                           //创建时间                   
                    MaterialCode = lot.MaterialCode,            //物料编码
                    OrderNumber = lot.OrderNumber,               //工单代码
                    PackageFlag = (int)IsPackaged,
                    PackageLine = packageLine

                };
                #endregion
                //取得当前Bin信息
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}' ",
                                        strBinNo,
                                        packageLine),
                    OrderBy = " EditTime desc"
                };

                lstPackageBin = PackageCornerDataEngine.Get(cfg);
                #region 创建Bin对象
                if (intInBinOrder == 1)//是否新入托
                {
                    //原Bin对象
                    if (lstPackageBin != null && lstPackageBin.Count > 0)
                    {
                        packageCornerObjHis = lstPackageBin.FirstOrDefault();

                        packageCornerObjHis.BinState = 1;   //Bin状态 0 - 当前Bin 1 - 非当前Bin数据
                    }
                    packageCornerObj = new PackageCorner
                    {
                        Key = strPackagekey,            //Bin主键
                        BinNo = strBinNo,
                        PackageLine = packageLine,
                        BinQty = intInBinOrder,        //Bin数量
                        BinMaxQty = iQtyMax,            //Bin内托最大包装数量
                        BinPackaged = (EnumCornerPackaged)IsFinishPackage,   //Bin包装状态与包装状态相同（UnFinished = 0---未完成，Finished = 1---完成）
                        BinState = 0,                   //Bin状态
                        Creator = packageLine,      //创建人（设备代码）
                        CreateTime = now,               //创建日期         
                        Editor = packageLine,       //编辑人（设备代码）
                        EditTime = now                  //编辑日期
                    };
                }
                else
                {
                    if (lstPackageBin == null || lstPackageBin.Count == 0)
                    {
                        result.Code = 1000;
                        result.Message = string.Format("【异常BIN】包装线（{1}）Bin({0}对应的数据异常，未找到历史数据！) ",
                                                       strBinNo,
                                                       packageLine);
                        return result;
                    }

                    //取得当前Bin对象
                    packageCornerObj = lstPackageBin.FirstOrDefault();

                    //设置属性
                    packageCornerObj.BinPackaged = (EnumCornerPackaged)IsFinishPackage;   //Bin包装状态与包装状态相同（UnFinished = 0---未完成，Finished = 1---完成）
                    packageCornerObj.BinQty = intInBinOrder;                              //Bin数量
                    packageCornerObj.Editor = packageLine;                         //编辑人（设备代码）
                    packageCornerObj.EditTime = now;                                     //编辑日期
                }

                #endregion

                #region ***创建事物对象***

                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();
                #region 2.包装数据


                if (packageCornerObj.LockFlag == 1)
                {
                    result.Code = 1019;
                    result.Message = string.Format("BIN：（{0}） 已经锁定，请抬下组件稍后上线！", packageCornerObj.BinNo);
                    return result;
                }
                if (intInBinOrder == 1)                                                 //新托号
                {
                    this.PackageCornerDataEngine.Insert(packageCornerObj, session);
                    //Bin历史数据,当对象为NULL即不存在历史对象不做处理
                    if (packageCornerObjHis != null)
                    {
                        this.PackageCornerDataEngine.Update(packageCornerObjHis, session);
                    }
                }
                else
                {
                    this.PackageCornerDataEngine.Update(packageCornerObj, session);
                }


                //2.1.包装明细数据
                this.PackageCornerDetailDataEngine.Insert(packageCornerDetail, session);
                #endregion
                //transaction.Rollback();
                transaction.Commit();
                session.Close();
                #endregion
             
                if (IsPackaged == EnumCornerDetailPackaged.UnPackaged)
                {

                    result.Message = "【不包】";
                }
                else
                {
                  
                    result.Message = "【包】";
                    if (intInBinOrder == 1)
                    {
                        result.Message = "【包 -- 首块】";
                    }
                }
              
              
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();

                transaction.Rollback();
                session.Close();
                return result;
            }

        }
    }
}
