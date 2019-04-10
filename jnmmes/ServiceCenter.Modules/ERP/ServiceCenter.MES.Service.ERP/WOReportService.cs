using Microsoft.Practices.EnterpriseLibrary.Data;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.DataAccess.Interface.ERP;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.ERP;
using ServiceCenter.MES.Model.ERP.Resources;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.ERP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Data.SqlClient;
using ServiceCenter.Common;


namespace ServiceCenter.MES.Service.ERP
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WOReportService : IWOReportContract
    {
        #region 定义全局变量
        string ErpDBName = ConfigurationManager.AppSettings["ErpDBName"].ToString();
        string ErpAccount = ConfigurationManager.AppSettings["ErpAccount"].ToString();
        string ErpGroupCode = ConfigurationManager.AppSettings["ErpGroupCode"].ToString();
        string ErpORGCode = ConfigurationManager.AppSettings["ErpORGCode"].ToString();
        #endregion

        #region 构造函数
        public WOReportService(ISessionFactory sf)
        {
            this._db = DatabaseFactory.CreateDatabase();
            this.Ora_db = DatabaseFactory.CreateDatabase("ERPDB");
            this.SessionFactory = sf;
        }

        #endregion

        #region 定义数据库实例
       
        protected Database _db;
        protected Database Ora_db;
        public ISessionFactory SessionFactory { get; set; }

        #endregion

        #region 定义数据访问对象

        //入库单表头数据访问对象
        public IWOReportDataEngine WOReportDataEngine { get; set; }

        // Oem批次数据访问对象
        public IOemDataEngine OemDataEngine { get; set; }

        //工单数据访问对象
        public IWorkOrderDataEngine WorkOrderDataEngine { get; set; }

        //入库单明细数据访问对象
        public IWOReportDetailDataEngine WOReportDetailDataEngine { get; set; }

        //基础数据访问对象
        public IBaseAttributeValueDataEngine BaseAttributeValueDataEngine { get; set; }

        //托数据访问对象
        public IPackageDataEngine PackageDataEngine { get; set; }

        //批次数据访问对象
        public ILotDataEngine LotDataEngine { get; set; }

        //托明细数据访问对象
        public IPackageDetailDataEngine PackageDetailDataEngine { get; set; }

        // 批次操作数据访问对象
        public ILotTransactionDataEngine LotTransactionDataEngine { get; set; }

        // 批次历史数据访问对象
        public ILotTransactionHistoryDataEngine LotTransactionHistoryDataEngine{ get; set; }

        #endregion

        #region 注释方法

        //原入库申请撤销方法
        //public MethodReturnResult AntiState(WOReportParameter p)
        //{
        //    MethodReturnResult result = new MethodReturnResult()
        //    {
        //        Code = 0
        //    };

        //    WOReport woReport = this.WOReportDataEngine.Get(p.BillCode);

        //    if (woReport != null)
        //    {
        //        woReport.BillState = p.BillState;
        //        woReport.WRCode = p.WRCode;
        //        woReport.Editor = p.Editor;
        //        woReport.EditTime = DateTime.Now;
        //    }

        //    PagingConfig cfg = new PagingConfig()
        //    {
        //        IsPaging = false,
        //        OrderBy = "ItemNo",
        //        Where = string.Format(" Key.BillCode = '{0}'"
        //                                    , p.BillCode)
        //    };
        //    IList<WOReportDetail> lstWOReportDetail = this.WOReportDetailDataEngine.Get(cfg);

        //    //更新托信息为不在库
        //    List<Package> lstPackage = new List<Package>();
        //    foreach (var item in lstWOReportDetail)
        //    {
        //        Package package = this.PackageDataEngine.Get(item.ObjectNumber);
        //        package.PackageState = EnumPackageState.Packaged;
        //        package.InOrder = 1;
        //        lstPackage.Add(package);
        //    }

        //    ISession db = this.SessionFactory.OpenSession();
        //    ITransaction transaction = db.BeginTransaction();
        //    try
        //    {
        //        this.WOReportDataEngine.Update(woReport, db);

        //        foreach (var item in lstPackage)
        //        {
        //            this.PackageDataEngine.Update(item, db);
        //        }

        //        transaction.Commit();
        //        db.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction.Rollback();
        //        db.Close();
        //        result.Code = 1000;
        //        result.Message += string.Format("报工单撤销失败！", ex.Message);
        //        result.Detail = ex.ToString();
        //    }
        //    return result;

        //}

        #endregion

        #region ERP入库相关操作

        //获取入库单表头数据
        public MethodReturnResult<WOReport> GetWOReport(string Key)
        {
            MethodReturnResult<WOReport> result = new MethodReturnResult<WOReport>();

            if (!this.WOReportDataEngine.IsExists(Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WOReportService_IsNotExists, Key);
                return result;
            }
            try
            {
                result.Data = this.WOReportDataEngine.Get(Key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        //获取入库单表头数据列表
        public MethodReturnResult<IList<WOReport>> GetWOReport(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WOReport>> result = new MethodReturnResult<IList<WOReport>>();
            try
            {
                result.Data = this.WOReportDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        //新增入库单表头数据
        public MethodReturnResult AddWOReport(WOReport model)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            try
            {
                if (this.WOReportDataEngine.IsExists(model.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.WOReportService_IsExists, model.Key);
                    return result;
                }

                this.WOReportDataEngine.Insert(model);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message += string.Format(StringResource.WOReport_Error_Save, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }

        //编辑入库单表头数据
        public MethodReturnResult EditWOReport(WOReport model)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            try
            {
                if (!this.WOReportDataEngine.IsExists(model.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.WOReportService_IsNotExists, model.Key);
                    return result;
                }

                this.WOReportDataEngine.Update(model);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }

            return result;
        }
                
        //删除入库单表头数据
        public MethodReturnResult DeleteWOReport(WOReport model, string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            WOReport woReport = null;
            List<LotTransaction> lstLotTransInsert = new List<LotTransaction>();                    //批次插入事物列表
            List<LotTransaction> lstLotTransUpdate = new List<LotTransaction>();                    //批次更新事物列表
            List<LotTransactionHistory> lstLotTransHisInsert = new List<LotTransactionHistory>();   //批次历史事物列表
            string transactionKey = "";
            ISession session = null;
            ITransaction transaction = null;
            Lot lotupdate = null;
            List<Lot> lstLotUpdate = new List<Lot>();                                               //批次更新事物列表

            try
            {
                //初始化操作时间
                model.CreateTime = DateTime.Now;
                model.EditTime = model.CreateTime;

                #region 1.入库单对象合规检查

                //取得入库单对象
                woReport = this.WOReportDataEngine.Get(key);

                //判断入库单是否存在
                if (woReport == null)
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.WOReportService_IsNotExists, key);
                    return result;
                }

                //判断入库单是否已经申报存在
                if (woReport.BillState != EnumBillState.Create)
                {
                    result.Code = 1002;
                    result.Message = String.Format("入库单[{0}]当前状态为[{1}],不能删除！", 
                                                    key,
                                                    woReport.BillState.GetDisplayName());
                    return result;
                }
                #endregion

                #region 2.获取入库单明细托对象并更新
                //取得入库单明细
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "ItemNo",
                    Where = string.Format(" Key.BillCode = '{0}'"
                                                , key)
                };

                IList<WOReportDetail> lstWOReportDetail = this.WOReportDetailDataEngine.Get(cfg);
                if (lstWOReportDetail != null && lstWOReportDetail.Count > 0)
                {
                    result.Code = 1002;
                    result.Message = String.Format("入库单[{0}]中存在明细,请执行逐条删除！",key);
                    return result;
                }

                IList<Package> lstPackageForUpdate = new List<Package>();
                string strPackageCode = "";

                //循环取得入库单明细
                List<WOReportDetailKey> lstWOReportDetailKey = new List<WOReportDetailKey>();

                foreach (var item in lstWOReportDetail)
                {
                    WOReportDetailKey woReportDetailKey = new WOReportDetailKey()
                    {
                        BillCode = item.Key.BillCode,
                        ItemNo = item.Key.ItemNo
                    };

                    lstWOReportDetailKey.Add(woReportDetailKey);

                    //获取托属性更新对象
                    if (strPackageCode != item.ObjectNumber)
                    {
                        strPackageCode = item.ObjectNumber;
                        //报废
                        if (woReport.ScrapType == EnumScrapType.True)
                        {
                            lotupdate = this.LotDataEngine.Get(item.ObjectNumber);
                            if (lotupdate == null)
                            {
                                result.Code = 1003;
                                result.Message = String.Format("批次号[{0}]不存在！",item.ObjectNumber);

                                return result;
                            }

                            //取得事物主键
                            transactionKey = Guid.NewGuid().ToString();

                            //创建批次历史事物
                            LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lotupdate);
                            lstLotTransHisInsert.Add(lotHistory);

                            //创建事物参数
                            TrackParameter transp = new TrackParameter()
                            {
                                Operator = model.Creator,                    //操作人
                                OperateComputer = model.OperateComputer,     //操作客户端
                                RouteOperationName = "入库",                 //工序名称
                                RouteName = "",                              //工艺流程名称
                                LineCode = ""                                //线别                                                  
                            };

                            transp.TransactionKeys.Add(lotupdate.Key, transactionKey);        //批次事物主键

                            //事物状态
                            transp.Activity = EnumLotActivity.Delete;

                            //创建撤销批次事物
                            result = CreateUndoLotTransaction(transp, ref lotupdate, transactionKey, lstLotTransInsert, lstLotTransUpdate);
                            lstLotUpdate.Add(lotupdate);
                        }
                        //非报废
                        else
                        {
                            Package packageupdate = this.PackageDataEngine.Get(strPackageCode);

                            packageupdate.PackageState = EnumPackageState.Packaged;     //设置托属性为包装完成状态
                            packageupdate.InOrder = 0;                                  //设置托号不在入库单中
                            packageupdate.Editor = model.Editor;                        //编辑人
                            packageupdate.EditTime = model.EditTime;                    //编辑日期

                            lstPackageForUpdate.Add(packageupdate);

                            #region 循环设置批次事物信息
                            DateTime now = DateTime.Now;                            //当前时间
                            Lot lot = null;
                            
                            //取得托批次列表
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                OrderBy = "ItemNo",
                                Where = string.Format(" Key.PackageNo = '{0}'", packageupdate.Key)
                            };

                            IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
                            if (lstPackageDetail != null && lstPackageDetail.Count > 0)
                            {
                                //循环设置批次事物信息
                                foreach (PackageDetail packageDetail in lstPackageDetail)
                                {
                                    //取得批次信息。
                                    lot = this.LotDataEngine.Get(packageDetail.Key.ObjectNumber);

                                    //取得事物主键
                                    transactionKey = Guid.NewGuid().ToString();

                                    //创建批次历史事物
                                    LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                                    lstLotTransHisInsert.Add(lotHistory);

                                    //创建事物参数
                                    TrackParameter transp = new TrackParameter()
                                    {
                                        Operator = model.Creator,                    //操作人
                                        OperateComputer = model.OperateComputer,     //操作客户端
                                        RouteOperationName = "入库",                 //工序名称
                                        RouteName = "",                              //工艺流程名称
                                        LineCode = ""                                //线别                                                  
                                    };

                                    transp.TransactionKeys.Add(lot.Key, transactionKey);        //批次事物主键

                                    //事物状态
                                    transp.Activity = EnumLotActivity.Delete;

                                    //创建撤销批次事物
                                    result = CreateUndoLotTransaction(transp, ref lot, transactionKey, lstLotTransInsert, lstLotTransUpdate);
                                    lstLotUpdate.Add(lot);
                                }

                            }

                            #endregion
                        }                                               
                    }
                }
                #endregion

                #region 3.事务处理
                //创建事物开始删除数据
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                //1.插入批次事物信息
                foreach (LotTransaction lotTransaction in lstLotTransInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //2.更新批次事物信息
                foreach (LotTransaction lotTransaction in lstLotTransUpdate)
                {
                    this.LotTransactionDataEngine.Update(lotTransaction, session);
                }

                //3.批次信息更新
                foreach (Lot lotNew in lstLotUpdate)
                {
                    this.LotDataEngine.Update(lotNew, session);
                }

                //4.删除入库单主表信息
                this.WOReportDataEngine.Delete(key, session);

                //5.删除入库单明细表信息
                foreach (var item in lstWOReportDetailKey)
                {  
                    this.WOReportDetailDataEngine.Delete(item, session);
                }

                if (woReport.ScrapType == EnumScrapType.False)
                {
                    //6.更新托属性
                    foreach (var item in lstPackageForUpdate)
                    {
                        this.PackageDataEngine.Update(item, session);
                    }
                }
                
                transaction.Commit();
                session.Close();
                #endregion
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                session.Close();
                
                result.Code = 1000;
                result.Message += string.Format("入库单删除失败！", ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }

        //获取入库单明细数据
        public MethodReturnResult<WOReportDetail> GetWOReportDetail(WOReportDetailKey Key)
        {
            MethodReturnResult<WOReportDetail> result = new MethodReturnResult<WOReportDetail>();

            if (!this.WOReportDetailDataEngine.IsExists(Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WOReportService_IsNotExists, Key);
                return result;
            }
            try
            {
                result.Data = this.WOReportDetailDataEngine.Get(Key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        //获取入库单明细数据列表
        public MethodReturnResult<IList<WOReportDetail>> GetWOReportDetail(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WOReportDetail>> result = new MethodReturnResult<IList<WOReportDetail>>();
            try
            {
                result.Data = this.WOReportDetailDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary> 
        /// 新增入库单明细（含报废）
        /// </summary>
        /// <param name="model">入库单明细对象</param>
        /// <param name="ScrapType">是否报废</param>
        /// <returns></returns>
        public MethodReturnResult AddWOReportDetail(WOReportDetail model, EnumScrapType ScrapType)
        {
            List<LotTransaction> lstLotTransInsert = new List<LotTransaction>();                    //批次插入事物列表
            List<LotTransactionHistory> lstLotTransHisInsert = new List<LotTransactionHistory>();   //批次历史事物列表
            string transactionKey = "";
            MethodReturnResult result = new MethodReturnResult();
            List<WOReportDetail> lstIWDetail = new List<WOReportDetail>();                          //入库单明细列表
            Package packageupdate = null;
            Lot lotupdate = null;
            ITransaction transaction = null;
            ISession session = null;
            List<Lot> lstLotUpdate = new List<Lot>();                                               //批次更逊事物列表
            List<OemData> lstOemLotForUpdate = new List<OemData>();                                 //OEM组件事物列表

            try
            {
                PagingConfig cfg = null;
                IList<WOReportDetail> lstWOReportDetail = null;

                //初始化操作时间
                model.CreateTime = DateTime.Now;
                model.EditTime = model.CreateTime;

                #region 1.合规性校验
                
                //1.1取得入库单表头对象
                WOReport woReport = WOReportDataEngine.Get(model.Key.BillCode);
                if (woReport == null)
                {
                    result.Code = 1002;
                    result.Message = String.Format("入库单[{0}]不存在！",
                                                   model.Key.BillCode );

                    return result;
                }

                //1.2.判断入库单是否属于未提交状态
                if (woReport.BillState != EnumBillState.Create )
                {
                    result.Code = 1002;
                    result.Message = String.Format("入库单状态[{0}]，不能添加明细！",
                                                    woReport.BillState.GetDisplayName());

                    return result;
                }

                //1.2.1判断入库单明细是否已存在该项目
                PagingConfig cfg0 = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "ItemNo DESC",
                    Where = string.Format(" ObjectNumber = '{0}'  and Key.BillCode = '{1}'"
                                            , model.ObjectNumber, model.Key.BillCode)
                };
                IList<WOReportDetail> lstWOReportDetail0 = this.WOReportDetailDataEngine.Get(cfg0);
                if (lstWOReportDetail0.Count > 0)
                {
                    result.Code = 1002;
                    result.Message = String.Format("入库单[{0}]中已存在项目[{1}]",
                                                    model.Key.BillCode, model.ObjectNumber);

                    return result;
                }

                if (ScrapType == EnumScrapType.False)
                {
                    //1.3.1获取托对象
                    packageupdate = this.PackageDataEngine.Get(model.ObjectNumber);
                    if (packageupdate == null)
                    {
                        result.Code = 1003;
                        result.Message = String.Format("托号[{0}]不存在！",
                                                       model.ObjectNumber);

                        return result;
                    }

                    //1.4.1判断托状态是否完成包装状态
                    if (packageupdate.PackageState != EnumPackageState.Packaged)
                    {
                        if(packageupdate.PackageState == EnumPackageState.InFabStore
                            || packageupdate.PackageState == EnumPackageState.Packaging)
                        {
                            result.Code = 1004;
                            result.Message = String.Format("托号状态[{0}]不符合入库单要求！",
                                                           packageupdate.PackageState.GetDisplayName());

                            return result;
                        }
                    }
                }
                else
                {
                    //1.3.2获取批次对象
                    lotupdate = this.LotDataEngine.Get(model.ObjectNumber);
                    if (lotupdate == null)
                    {
                        result.Code = 1003;
                        result.Message = String.Format("批次号[{0}]不存在！",
                                                       model.ObjectNumber);

                        return result;
                    }

                    //1.4.2判断批次状态是否已经批次报废
                    if (!lotupdate.DeletedFlag)
                    {
                        result.Code = 1004;
                        result.Message = String.Format("批次号[{0}]未报废结束，不符合入库单要求！",model.ObjectNumber);

                        return result;
                    }
                }
                
                //1.5判断托号或批次号是否已经存在其他入库单中
                PagingConfig cfg1 = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "ItemNo DESC",
                    Where = string.Format(" ObjectNumber = '{0}'  and Key.BillCode <> '{1}'"
                                            , model.ObjectNumber, model.Key.BillCode)
                };

                IList<WOReportDetail> lstWOReportDetail1 = this.WOReportDetailDataEngine.Get(cfg1);
                if (lstWOReportDetail1.Count > 0)
                {
                    #region 原逻辑
                    ////返工工单入库
                    //if (packageupdate.OrderNumber.ToString().ToUpper().Contains("2MO"))
                    //{
                    //    PagingConfig cfg2 = new PagingConfig()
                    //    {
                    //        IsPaging = false,
                    //        OrderBy = "ItemNo DESC",
                    //        Where = string.Format(" ObjectNumber = '{0}' and OrderNumber like '%2MO%'  and Key.BillCode <> '{1}'"
                    //                                , model.ObjectNumber,model.Key.BillCode)
                    //    };
                    //    IList<WOReportDetail> lstWOReportDetail2 = this.WOReportDetailDataEngine.Get(cfg2);
                    //    if (lstWOReportDetail2.Count > 0)
                    //    {
                    //        PagingConfig cfg3 = new PagingConfig()
                    //        {
                    //            IsPaging = false,
                    //            OrderBy = "EditTime DESC",
                    //            Where = string.Format(" Key = '{0}' and (BillState = 0 or BillState = 1)"
                    //                                    , lstWOReportDetail2[0].Key.BillCode)
                    //        };

                    //        IList<WOReport> lstWOReport = this.WOReportDataEngine.Get(cfg3);
                    //        if (lstWOReport != null && lstWOReport.Count > 0)
                    //        {
                    //            result.Code = 1003;
                    //            result.Message = String.Format("项目[{0}]已经在入库单{1}中！",
                    //                                           model.ObjectNumber,
                    //                                           lstWOReportDetail2[0].Key.BillCode);

                    //            return result;
                    //        }                              
                    //    }
                    //}  
                    ////正常工单入库
                    //else
                    //{
                    //    result.Code = 1003;
                    //    result.Message = String.Format("项目[{0}]已经在入库单{1}中！",
                    //                                    model.ObjectNumber,
                    //                                    lstWOReportDetail1[0].Key.BillCode);

                    //    return result;
                    //}
                    #endregion

                    #region 现逻辑
                    //非报废单据
                    if (ScrapType == EnumScrapType.False)
                    {
                        if (packageupdate.InOrder != 0)
                        {
                            for (int i = 0; i < lstWOReportDetail1.Count; i++)
                            {
                                PagingConfig cfg3 = new PagingConfig()
                                {
                                    IsPaging = false,
                                    OrderBy = "EditTime DESC",
                                    Where = string.Format(" Key = '{0}' and (BillState = 0 or BillState = 1 or BillState = 2)"
                                                            , lstWOReportDetail1[i].Key.BillCode)
                                };

                                IList<WOReport> lstWOReport = this.WOReportDataEngine.Get(cfg3);
                                if (lstWOReport != null && lstWOReport.Count > 0)
                                {
                                    result.Code = 1003;
                                    result.Message = String.Format("项目[{0}]已经在入库单[{1}]中！",
                                                                   model.ObjectNumber,
                                                                   lstWOReportDetail1[i].Key.BillCode);

                                    return result;
                                }
                            }
                        }
                    }
                    //报废单据
                    else
                    {
                        if (lotupdate.InOrder != 0)
                        {
                            for (int i = 0; i < lstWOReportDetail1.Count; i++)
                            {
                                PagingConfig cfg3 = new PagingConfig()
                                {
                                    IsPaging = false,
                                    OrderBy = "EditTime DESC",
                                    Where = string.Format(" Key = '{0}' and (BillState = 0 or BillState = 1 or BillState = 2)"
                                                            , lstWOReportDetail1[i].Key.BillCode)
                                };

                                IList<WOReport> lstWOReport = this.WOReportDataEngine.Get(cfg3);
                                if (lstWOReport != null && lstWOReport.Count > 0)
                                {
                                    result.Code = 1003;
                                    result.Message = String.Format("项目[{0}]已经在入库单[{1}]中！",
                                                                   model.ObjectNumber,
                                                                   lstWOReportDetail1[i].Key.BillCode);

                                    return result;
                                }
                            }
                        }
                    }
                                       
                    #endregion
                }
                
                #endregion

                #region 2.取得入库单最大项目号
                int maxItemNo = 0;
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "ItemNo DESC",
                    Where = string.Format(" Key.BillCode = '{0}'"
                                            , model.Key.BillCode)
                };

                lstWOReportDetail = this.WOReportDetailDataEngine.Get(cfg);

                if (lstWOReportDetail.Count > 0)
                {
                    maxItemNo = lstWOReportDetail[0].Key.ItemNo;
                }
                #endregion

                #region 3.生成入库明细对象

                #region 3.1报废入库
                if (ScrapType == EnumScrapType.True)
                {
                    MethodReturnResult<DataSet> resultlotgroupdetail = new MethodReturnResult<DataSet>();       //报废批次数据汇总
                    DataSet dsResult = new DataSet();
                    resultlotgroupdetail = GetPackageInfoEx(model.ObjectNumber, ScrapType);

                    //入库单明细
                    if (resultlotgroupdetail.Data != null && resultlotgroupdetail.Data.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < resultlotgroupdetail.Data.Tables[0].Rows.Count; i++)
                        {
                            string pmName = resultlotgroupdetail.Data.Tables[0].Rows[i]["PM_NAME"].ToString();                      //功率名称
                            string PsSubcode = resultlotgroupdetail.Data.Tables[0].Rows[i]["PS_SUBCODE"].ToString();                //子档位                            
                            string Grade = resultlotgroupdetail.Data.Tables[0].Rows[i]["GRADE"].ToString();                         //产品等级
                            string StandPMAX = resultlotgroupdetail.Data.Tables[0].Rows[i]["STAND_PMAX"].ToString();                //标称功率
                            decimal SumCoefPMax = Convert.ToDecimal(resultlotgroupdetail.Data.Tables[0].Rows[i]["SumCOEF_PMAX"]);   //实际功率合计

                            maxItemNo++;                //最大项目号

                            //入库单明细对象
                            WOReportDetail IWDetial = new WOReportDetail()
                            {
                                Key = new WOReportDetailKey()
                                {
                                    BillCode = model.Key.BillCode,  //入库单号
                                    ItemNo = maxItemNo              //项目号
                                },
                                ObjectNumber = model.ObjectNumber,                                                                  //托号
                                OrderNumber = resultlotgroupdetail.Data.Tables[0].Rows[i]["ORDER_NUMBER"].ToString(),           //工单号  
                                MaterialCode = resultlotgroupdetail.Data.Tables[0].Rows[i]["MATERIAL_CODE"].ToString(),         //产品物料号
                                EffiName = pmName,                  //功率名称
                                PsSubcode = PsSubcode,              //子档位                                
                                Grade = Grade,                      //产品等级
                                EffiCode = StandPMAX,               //标称功率
                                SumCoefPMax = SumCoefPMax,          //实际功率合计
                                Qty = Convert.ToDecimal(resultlotgroupdetail.Data.Tables[0].Rows[i]["QTY"]),                    //分组数量
                                CreateTime = model.CreateTime,      //创建日期                                         
                                Creator = model.Creator,            //创建人
                                Editor = model.Editor,              //编辑人
                                EditTime = model.CreateTime         //编辑日期
                            };

                            lstIWDetail.Add(IWDetial);
                        }
                    }
                }
                #endregion

                #region 3.2常规入库
                else
                {
                    //取得托信息汇总数据（根据工单、物料、等级、效率档分组汇总）
                    MethodReturnResult<DataSet> resultpackagegroupdetail = new MethodReturnResult<DataSet>();       //包装入库数据汇总
                    DataSet dsResult = new DataSet();

                    using (DbConnection con = this._db.CreateConnection())
                    {
                        int iReturn;
                        string strErrorMessage = string.Empty;
                        DbCommand cmd = con.CreateCommand();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sp_ERP_PackageGroupSum";
                        this._db.AddInParameter(cmd, "PackageNo", DbType.String, string.IsNullOrEmpty(model.ObjectNumber) ? "%" : model.ObjectNumber);

                        //设置返回错误信息
                        cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 512));
                        cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                        //设置返回值
                        SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                        parReturn.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(parReturn);

                        cmd.CommandTimeout = 960;

                        //执行存储过程
                        dsResult = this._db.ExecuteDataSet(cmd);

                        //取得返回值
                        iReturn = (int)cmd.Parameters["@return"].Value;

                        if (iReturn == -1)                              //调用失败返回错误信息
                        {
                            strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                            result.Code = 2000;
                            result.Message = strErrorMessage;
                            result.Detail = strErrorMessage;

                            return result;
                        }

                        //判断托信息汇总数据（根据工单、物料、等级、效率档分组汇总）是否异常
                        if (dsResult.Tables[0].Rows.Count <= 0)
                        {
                            result.Code = 2001;
                            result.Message = string.Format(@"托明细汇总数据异常！[存储过程：sp_ERP_PackageGroupSum结果为{0}]，
                                                             请进行IV数据修正或找工艺查看工单分档规则与批次IV数据中分档规则是否一致！",
                                                            dsResult.Tables[0].Rows.Count);
                            
                            return result;
                        }
                    }

                    resultpackagegroupdetail.Data = dsResult;

                    //入库单明细
                    if (resultpackagegroupdetail.Data != null && resultpackagegroupdetail.Data.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < resultpackagegroupdetail.Data.Tables[0].Rows.Count; i++)
                        {
                            string pmName = resultpackagegroupdetail.Data.Tables[0].Rows[i]["PM_NAME"].ToString();                      //功率名称
                            string PsSubcode = resultpackagegroupdetail.Data.Tables[0].Rows[i]["PS_SUBCODE"].ToString();                //子档位                            
                            string Grade = resultpackagegroupdetail.Data.Tables[0].Rows[i]["GRADE"].ToString();                         //产品等级
                            string StandPMAX = resultpackagegroupdetail.Data.Tables[0].Rows[i]["STAND_PMAX"].ToString();                //标称功率
                            decimal SumCoefPMax = Convert.ToDecimal(resultpackagegroupdetail.Data.Tables[0].Rows[i]["SumCOEF_PMAX"]);   //实际功率合计

                            if (Grade == "D")
                            {
                                pmName = "0W";          //功率名称
                                PsSubcode = "待定";     //子档位 
                                StandPMAX = "000";      //标称功率
                                SumCoefPMax = 0;        //实际功率合计
                            }

                            maxItemNo++;                //最大项目号

                            //入库单明细对象
                            WOReportDetail IWDetial = new WOReportDetail()
                            {
                                Key = new WOReportDetailKey()
                                {
                                    BillCode = model.Key.BillCode,  //入库单号
                                    ItemNo = maxItemNo              //项目号
                                },
                                ObjectNumber = model.ObjectNumber,                                                                  //托号
                                OrderNumber = resultpackagegroupdetail.Data.Tables[0].Rows[i]["ORDER_NUMBER"].ToString(),           //工单号  
                                MaterialCode = resultpackagegroupdetail.Data.Tables[0].Rows[i]["MATERIAL_CODE"].ToString(),         //产品物料号
                                EffiName = pmName,                  //功率名称
                                PsSubcode = PsSubcode,              //子档位                                
                                Grade = Grade,                      //产品等级
                                EffiCode = StandPMAX,               //标称功率
                                SumCoefPMax = SumCoefPMax,          //实际功率合计
                                Qty = Convert.ToDecimal(resultpackagegroupdetail.Data.Tables[0].Rows[i]["QTY"]),                    //分组数量
                                CreateTime = model.CreateTime,      //创建日期                                         
                                Creator = model.Creator,            //创建人
                                Editor = model.Editor,              //编辑人
                                EditTime = model.CreateTime         //编辑日期
                            };

                            lstIWDetail.Add(IWDetial);
                        }
                    }
                }
                #endregion               

                #endregion

                #region 4.循环设置批次事物信息
                DateTime now = DateTime.Now;                            //当前时间
                Lot lot = null;
                
                #region 报废
                if (ScrapType == EnumScrapType.True)
                {
                    //取得事物主键
                    transactionKey = Guid.NewGuid().ToString();

                    //创建批次历史事物
                    LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lotupdate);
                    lstLotTransHisInsert.Add(lotHistory);

                    //创建事物参数
                    TrackParameter transp = new TrackParameter()
                    {
                        Operator = model.Creator,                    //操作人
                        OperateComputer = model.OperateComputer,     //操作客户端
                        RouteOperationName = "入库",                 //工序名称
                        RouteName = "",                              //工艺流程名称
                        LineCode = ""                                //线别                                                  
                    };

                    transp.TransactionKeys.Add(lotupdate.Key, transactionKey);        //批次事物主键

                    //事物状态
                    transp.Activity = EnumLotActivity.AddInErp;

                    //创建批次事物
                    result = CreateLotTransaction(transp, lotupdate, transactionKey, lstLotTransInsert);

                    //增加批次属性                            
                    lotupdate.RouteName = transp.RouteName;                   //工艺流程
                    lotupdate.RouteStepName = transp.RouteOperationName;      //工步
                    lotupdate.StateFlag = EnumLotState.InStoragelist;         //入库单中未申请
                    lotupdate.EquipmentCode = transp.EquipmentCode;           //设备代码
                    lotupdate.StartWaitTime = now;                            //开始等待时间
                    lotupdate.StartProcessTime = now;                         //开始处理时间
                    lotupdate.Editor = transp.Operator;                       //编辑人
                    lotupdate.EditTime = now;                                 //编辑日期
                }
                #endregion

                #region 非报废
                else
                {
                    //取得托批次列表
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "ItemNo",
                        Where = string.Format(" Key.PackageNo = '{0}'", packageupdate.Key)
                    };

                    IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
                    if (lstPackageDetail != null && lstPackageDetail.Count > 0)
                    {                                                                     
                        //循环设置批次事物信息
                        foreach (PackageDetail packageDetail in lstPackageDetail)
                        {
                            OemData oemLot = this.OemDataEngine.Get(packageDetail.Key.ObjectNumber);
                            if (oemLot != null)
                            {
                                #region OEM组件
                                oemLot.Status = EnumOemStatus.InStoragelist;
                                oemLot.Editor = model.Editor;
                                oemLot.EditTime = now;
                                
                                lstOemLotForUpdate.Add(oemLot);
                                #endregion
                            }
                            else
                            {
                                #region 常规自制组件
                                //取得批次信息。
                                lot = this.LotDataEngine.Get(packageDetail.Key.ObjectNumber);
                                //判断批次是否存在。
                                if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                                {
                                    result.Code = 2003;
                                    result.Message = string.Format("批次：（{0}）不存在！", lot.Key);
                                    return result;
                                }

                                //批次已撤销
                                if (lot.DeletedFlag == true)
                                {
                                    result.Code = 2004;
                                    result.Message = string.Format("批次：（{0}）已删除！", lot.Key);
                                    return result;
                                }

                                //批次已暂停
                                if (lot.HoldFlag == true)
                                {
                                    result.Code = 2005;
                                    result.Message = string.Format("批次：（{0}）已暂停！", lot.Key);
                                    return result;
                                }

                                #region 批次事务
                                ////取得事物主键
                                //transactionKey = Guid.NewGuid().ToString();

                                ////创建批次历史事物
                                //LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                                //lstLotTransHisInsert.Add(lotHistory);

                                ////创建事物参数
                                //TrackParameter transp = new TrackParameter()
                                //{
                                //    Operator = model.Creator,                    //操作人
                                //    OperateComputer = model.OperateComputer,     //操作客户端
                                //    RouteOperationName = "入库",                 //工序名称
                                //    RouteName = "",                              //工艺流程名称
                                //    LineCode = ""                                //线别                                                  
                                //};

                                //transp.TransactionKeys.Add(lot.Key, transactionKey);        //批次事物主键

                                ////事物状态
                                //transp.Activity = EnumLotActivity.AddInErp;

                                ////创建批次事物
                                //result = CreateLotTransaction(transp, lot, transactionKey, lstLotTransInsert);

                                ////增加批次属性                            
                                //lot.RouteName = transp.RouteName;                   //工艺流程
                                //lot.RouteStepName = transp.RouteOperationName;      //工步
                                //lot.StateFlag = EnumLotState.InStoragelist;         //入库单中未申请
                                //lot.EquipmentCode = transp.EquipmentCode;           //设备代码
                                //lot.StartWaitTime = now;                            //开始等待时间
                                //lot.StartProcessTime = now;                         //开始处理时间
                                //lot.Editor = transp.Operator;                       //编辑人
                                //lot.EditTime = now;                                 //编辑日期
                                //lot.LastTransactionKey = transactionKey;            //最后操作事务主键

                                //lstLotUpdate.Add(lot);
                                #endregion
                               
                                #endregion
                            }                            
                        }                        
                    }
                }
                #endregion

                #endregion

                #region 5.生成托或报废批次属性事物对象
                if (ScrapType == EnumScrapType.False)
                {
                    //4.1托属性
                    packageupdate.PackageState = EnumPackageState.InStoragelist;    //设置托属性为入库单状态
                    packageupdate.InOrder = 1;                                      //设置托标志为已在入库单中
                    packageupdate.Editor = model.Creator;                           //编辑人
                    packageupdate.EditTime = model.CreateTime;                      //编辑日期
                }
                else
                {
                    //4.2报废批次属性
                    lotupdate.StateFlag = EnumLotState.InStoragelist;           //设置报废批次属性为入库单状态
                    lotupdate.InOrder = 1;                                      //设置报废批次状态标志为已在入库单中
                    lotupdate.Editor = model.Creator;                           //编辑人
                    lotupdate.EditTime = model.CreateTime;                      //编辑日期
                }                              
                #endregion

                #region 6.修改入库单表头总数量
                if (woReport.TotalQty >= 0)
                {
                    woReport.TotalQty += lstIWDetail.Count;
                }               
                woReport.Editor = model.Creator;                                //编辑人
                woReport.EditTime = model.CreateTime;                           //编辑日期
                #endregion

                #region 7.事务处理
                //创建事物对象
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                //1.更新事物信息
                foreach (LotTransaction lotTransaction in lstLotTransInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //2.更新批次事物信息
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransHisInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                }

                if (ScrapType == EnumScrapType.False)
                {
                    //3.1处理托属性
                    this.PackageDataEngine.Update(packageupdate, session);
                }
                else
                {
                    //3.2处理报废批次属性
                    this.LotDataEngine.Update(lotupdate, session);
                }

                //4.处理入库明细列表
                foreach (WOReportDetail IWDetial in lstIWDetail)
                {
                    this.WOReportDetailDataEngine.Insert(IWDetial, session);
                }

                //5.处理入库单表头总数量
                this.WOReportDataEngine.Update(woReport, session);

                //6.批次信息更新
                foreach (Lot lotNew in lstLotUpdate)
                {
                    this.LotDataEngine.Update(lotNew, session);
                }

                //7.更新OEM组件信息
                foreach (OemData oemLot in lstOemLotForUpdate)
                {
                    this.OemDataEngine.Update(oemLot, session);
                }
                
                //开始事物处理                                
                transaction.Commit();
                session.Close();
                #endregion
            }
            catch (Exception ex)
            {
                //回滚事物处理                                
                transaction.Rollback();
                session.Close(); 
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// 删除入库单明细（含报废）
        /// </summary>
        /// <param name="model">入库单明细对象</param>
        /// <param name="key">需要删除的入库单明细主键</param>
        /// <returns></returns>
        public MethodReturnResult DeleteWOReportDetail(WOReportDetail model, WOReportDetailKey key )
        {
            List<LotTransaction> lstLotTransInsert = new List<LotTransaction>();                    //批次插入事物列表
            List<LotTransaction> lstLotTransUpdate = new List<LotTransaction>();                    //批次更新事物列表
            List<LotTransactionHistory> lstLotTransHisInsert = new List<LotTransactionHistory>();   //批次历史事物列表
            string transactionKey = "";
            MethodReturnResult result = new MethodReturnResult();
            ISession session = null;
            ITransaction transaction = null;
            List<WOReportDetail> lstIWDetail = new List<WOReportDetail>();                          //入库单明细列表
            Package packageupdate = null;
            Lot lotupdate = null;
            List<Lot> lstLotUpdate = new List<Lot>();                                               //批次更新事物列表
            List<OemData> lstOemLotForUpdate = new List<OemData>();                                 //OEM组件事物列表

            try
            {
                //初始化操作时间
                model.CreateTime = DateTime.Now;
                model.EditTime = model.CreateTime;

                #region 1.入库单表头合规性校验
                //取得入库单表头对象
                WOReport woReport = WOReportDataEngine.Get(key.BillCode);
                EnumScrapType scrapType = woReport.ScrapType;

                if (woReport == null)
                {
                    result.Code = 1002;
                    result.Message = String.Format("入库单[{0}]不存在！",
                                                   key.BillCode );

                    return result;
                }

                //判断是否属于未提交状态
                if (woReport.BillState != EnumBillState.Create )
                {
                    result.Code = 1002;
                    result.Message = String.Format("入库单状态[{0}]，不能修改！",
                                                    woReport.BillState.ToString());

                    return result;
                }
                
                //取得明细对象判断是否存在
                WOReportDetail woReportDetail = WOReportDetailDataEngine.Get(key);

                if (woReportDetail == null)
                {
                    result.Code = 1002;
                    result.Message = String.Format("入库单明细[{0}]不存在！",
                                                   key.ItemNo.ToString());

                    return result;
                }
                #endregion

                #region 2.获取需要删除的托号或报废批次入库单明细
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "ItemNo",
                    Where = string.Format(" Key.BillCode = '{0}' and ObjectNumber = '{1}'"
                                            , key.BillCode
                                            , woReportDetail.ObjectNumber)
                };

                IList<WOReportDetail> lstWOReportDetail = this.WOReportDetailDataEngine.Get(cfg);
                #endregion

                #region 3.获取托或报废批次属性及合规性检查
                if (woReport.ScrapType != EnumScrapType.True)
                {
                    packageupdate = this.PackageDataEngine.Get(woReportDetail.ObjectNumber);
                    if (packageupdate == null)
                    {
                        result.Code = 1003;
                        result.Message = String.Format("托号[{0}]不存在！",
                                                       woReportDetail.ObjectNumber);

                        return result;
                    }
                }
                else
                {
                    lotupdate = this.LotDataEngine.Get(woReportDetail.ObjectNumber);
                    if (lotupdate == null)
                    {
                        result.Code = 1003;
                        result.Message = String.Format("批次号[{0}]不存在！",
                                                       woReportDetail.ObjectNumber);

                        return result;
                    }
                }
                
                #endregion

                #region 4.循环设置批次事物信息
                DateTime now = DateTime.Now;                            //当前时间
                Lot lot = null;

                #region 报废
                if (scrapType == EnumScrapType.True)
                {
                    //取得事物主键
                    transactionKey = Guid.NewGuid().ToString();

                    //创建批次历史事物
                    LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lotupdate);
                    lstLotTransHisInsert.Add(lotHistory);

                    //创建事物参数
                    TrackParameter transp = new TrackParameter()
                    {
                        Operator = model.Creator,                    //操作人
                        OperateComputer = model.OperateComputer,     //操作客户端
                        RouteOperationName = "入库",                 //工序名称
                        RouteName = "",                              //工艺流程名称
                        LineCode = ""                                //线别                                                  
                    };

                    transp.TransactionKeys.Add(lotupdate.Key, transactionKey);        //批次事物主键

                    //事物状态
                    transp.Activity = EnumLotActivity.Delete;

                    //创建撤销批次事物
                    result = CreateUndoLotTransaction(transp, ref lotupdate, transactionKey, lstLotTransInsert, lstLotTransUpdate);
                }
                #endregion

                #region 非报废
                else
                {
                    //取得托批次列表
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "ItemNo",
                        Where = string.Format(" Key.PackageNo = '{0}'", packageupdate.Key)
                    };

                    IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
                    if (lstPackageDetail != null && lstPackageDetail.Count > 0)
                    {
                        //循环设置批次事物信息
                        foreach (PackageDetail packageDetail in lstPackageDetail)
                        {
                            OemData oemLot = this.OemDataEngine.Get(packageDetail.Key.ObjectNumber);
                            if (oemLot != null)
                            {
                                #region OEM组件
                                oemLot.Status = EnumOemStatus.Packaged;
                                oemLot.Editor = model.Editor;
                                oemLot.EditTime = now;

                                lstOemLotForUpdate.Add(oemLot);
                                #endregion
                            }
                            else
                            {
                                #region 常规自制组件
                                //取得批次信息。
                                lot = this.LotDataEngine.Get(packageDetail.Key.ObjectNumber);
                                //判断批次是否存在。
                                if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                                {
                                    result.Code = 2003;
                                    result.Message = string.Format("批次：（{0}）不存在！", lot.Key);
                                    return result;
                                }

                                //批次已撤销
                                if (lot.DeletedFlag == true)
                                {
                                    result.Code = 2004;
                                    result.Message = string.Format("批次：（{0}）已删除！", lot.Key);
                                    return result;
                                }

                                //批次已暂停
                                if (lot.HoldFlag == true)
                                {
                                    result.Code = 2005;
                                    result.Message = string.Format("批次：（{0}）已暂停！", lot.Key);
                                    return result;
                                }

                                #region 批次事务
                                ////取得事物主键
                                //transactionKey = Guid.NewGuid().ToString();

                                ////创建批次历史事物
                                //LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                                //lstLotTransHisInsert.Add(lotHistory);

                                ////创建事物参数
                                //TrackParameter transp = new TrackParameter()
                                //{
                                //    Operator = model.Creator,                    //操作人
                                //    OperateComputer = model.OperateComputer,     //操作客户端
                                //    RouteOperationName = "入库",                 //工序名称
                                //    RouteName = "",                              //工艺流程名称
                                //    LineCode = ""                                //线别                                                  
                                //};

                                //transp.TransactionKeys.Add(lot.Key, transactionKey);        //批次事物主键

                                ////事物状态
                                //transp.Activity = EnumLotActivity.Delete;

                                ////创建撤销批次事物
                                //result = CreateUndoLotTransaction(transp, ref lot, transactionKey, lstLotTransInsert, lstLotTransUpdate);
                                //lstLotUpdate.Add(lot);
                                #endregion

                                #endregion
                            }                           
                        }
                    }
                }
                #endregion

                #endregion

                #region 5.修改托或批次状态
                if (woReport.ScrapType != EnumScrapType.True)
                {
                    packageupdate.PackageState = EnumPackageState.Packaged;         //设置托属性为已包装状态
                    packageupdate.InOrder = 0;                                      //设置托标志为未在入库单中
                    packageupdate.Editor = model.Creator;                           //编辑人
                    packageupdate.EditTime = model.CreateTime;                      //编辑日期
                }
                else
                {
                    lotupdate.StateFlag = EnumLotState.ScrapWaitToErp;          //设置报废批次属性为报废待入库状态
                    lotupdate.InOrder = 0;                                      //设置报废批次状态标志为未在入库单中
                    lotupdate.Editor = model.Creator;                           //编辑人
                    lotupdate.EditTime = model.CreateTime;                      //编辑日期
                }
                
                #endregion

                #region 6.修改入库单表头数量
                if (woReport.TotalQty > 0)
                {
                    woReport.TotalQty -= lstWOReportDetail.Count;
                }
                woReport.Editor = model.Creator;                                //编辑人
                woReport.EditTime = model.CreateTime;                           //编辑日期
                #endregion

                #region 7.事物处理
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();               

                //1.插入批次事物信息
                foreach (LotTransaction lotTransaction in lstLotTransInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //2.更新批次事物信息
                foreach (LotTransaction lotTransaction in lstLotTransUpdate)
                {
                    this.LotTransactionDataEngine.Update(lotTransaction, session);
                }

                //3.批次信息更新
                foreach (Lot lotNew in lstLotUpdate)
                {
                    this.LotDataEngine.Update(lotNew, session);
                }

                if (woReport.ScrapType == EnumScrapType.False)
                {
                    //4.1处理托属性
                    this.PackageDataEngine.Update(packageupdate, session);
                }
                else
                {
                    //4.2处理报废批次属性
                    this.LotDataEngine.Update(lotupdate, session);
                }               

                //5.删除入库单明细
                foreach (WOReportDetail detial in lstWOReportDetail)
                {
                    this.WOReportDetailDataEngine.Delete(detial.Key, session);
                }

                //6.更新OEM组件信息
                foreach (OemData oemLot in lstOemLotForUpdate)
                {
                    this.OemDataEngine.Update(oemLot, session);
                }

                //7.处理入库单表头数量
                this.WOReportDataEngine.Update(woReport, session);

                transaction.Commit();
                session.Close();
                #endregion
            }
            catch (Exception ex)
            {
                //回滚事务处理
                transaction.Rollback();
                session.Close();                
                result.Code = 1000;
                result.Message += string.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 根据MES系统入库单号从ERP生产报告视图中获取报工单号
        /// </summary>
        /// <param name="v_mm_wr">ERP生产报告视图</param>
        /// <param name="stockInBillCode">MES系统入库单号</param>
        /// <param name="ErpAccount">ERP账套</param>
        /// <param name="ErpGroupCode">ERP组</param>
        /// <param name="ErpORGCode">ERP组织代码</param>
        /// <returns></returns>
        public MethodReturnResult GetERPWorkReprotBillCode(string stockInBillCode)
        {
            MethodReturnResult result = new MethodReturnResult() { Code = 0 };

            try
            {
                DataSet dtERPWorkReprot = new DataSet();

                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    //取得ERP报工单号及主键
                    cmd.CommandText = string.Format(@"select pk_wr,vbillcode " +                 
                                                      " from " + ErpDBName + ".v_mm_wr " +
                                                      " where groupcode = '{0}' and orgcode='{1}' and mescode = '{2}' AND (MATERIALCODE LIKE '12%' OR MATERIALCODE LIKE '2512%') " +
                                                      " group by pk_wr,vbillcode",
                                                      ErpGroupCode,
                                                      ErpORGCode,
                                                      stockInBillCode);
     
                    dtERPWorkReprot = Ora_db.ExecuteDataSet(cmd);

                    if (dtERPWorkReprot.Tables.Count > 0 && dtERPWorkReprot.Tables[0].Rows.Count > 0)
                    {
                        result.ObjectNo = dtERPWorkReprot.Tables[0].Rows[0]["vbillcode"].ToString();
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

        /// <summary>
        /// 入库申请单申报或申报撤销（含报废）
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult StockInApply(WOReportParameter p) 
        {
            MethodReturnResult result = new MethodReturnResult();

            List<WOReportDetail> lstIWDetail = new List<WOReportDetail>();                          //入库单明细列表
            List<Lot> lstLot = new List<Lot>();                                                     //批次对象列表
            List<LotTransaction> lstLotTransInsert = new List<LotTransaction>();                    //批次插入事物列表
            List<LotTransactionHistory> lstLotTransHisInsert = new List<LotTransactionHistory>();   //批次历史事物列表
            List<LotTransaction> lstLotTransUpdate = new List<LotTransaction>();                    //批次更新事物列表
            List<Package> lstPackageForUpdate = new List<Package>();                                //托对象列表
            List<OemData> lstOemLotForUpdate = new List<OemData>();                                 //OEM组件事物列表
            List<Lot> lstLotForUpdate = new List<Lot>();                                            //报废批次对象列表

            ITransaction transaction = null;
            ISession session = null;

            try
            {
                #region 1.处理入库单表头事物对象
                WOReport woReport = WOReportDataEngine.Get(p.BillCode);

                if (woReport == null)
                {
                    result.Code = 1001;
                    result.Message = String.Format("入库单[{0}]不存在！");
                    return result;
                }

                //入库申报
                if(p.OperationType == 0)
                {
                    ////已申报成功报错
                    //if (!string.IsNullOrEmpty(woReport.WRCode))
                    //{
                    //    result.Code = 1002;
                    //    result.Message = String.Format(StringResource.ERPWOReportDetail_Error_WACode);
                    //    return result;
                    //}

                    woReport.WRCode = p.ERPWorkReportCode;      //ERP报工单号
                    woReport.ERPWRKey = p.ERPWorkReportKey;     //ERP报工单主键
                    woReport.BillState = p.BillState;           //入库申请操作状态
                }
                //申报撤销
                else
                {                    
                    woReport.BillState = EnumBillState.Create;  //状态
                    woReport.WRCode = "";                       //ERP报工单号
                    woReport.ERPWRKey = "";                     //ERP报工单主键
                }
               
                woReport.Editor = p.Editor;                     //编辑人
                woReport.EditTime = DateTime.Now;               //编辑日期

                #endregion

                #region 2.处理入库单明细对象
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(" Key.BillCode = '{0}'"
                                                , p.BillCode)
                };

                MethodReturnResult<IList<WOReportDetail>> resultWORDetail = this.GetWOReportDetail(ref cfg);

                if (resultWORDetail.Code > 0)
                {
                    return resultWORDetail;
                }

                string packageCode = "";
                string lotCode = "";
                bool isProcess = false;
                string transactionKey = "";
                DateTime now = DateTime.Now;                            //当前时间

                if (woReport.ScrapType == EnumScrapType.True)
                {
                    #region 2.1处理报废批次信息
                    foreach (WOReportDetail worDetial in resultWORDetail.Data)
                    {
                        //取得批次信息。
                        lotCode = worDetial.ObjectNumber;
                        //判断是否已经处理
                        isProcess = false;
                        for (int i = lstPackageForUpdate.Count - 1; i >= 0; i--)
                        {
                            if (packageCode == lstPackageForUpdate[i].Key)
                            {
                                isProcess = true;

                                break;
                            }
                        }

                        if (isProcess)
                        {
                            continue;
                        }

                        //取得批次信息
                        Lot lot = this.LotDataEngine.Get(lotCode);

                        //取得事物主键
                        transactionKey = Guid.NewGuid().ToString();

                        //创建批次历史事物
                        LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                        lstLotTransHisInsert.Add(lotHistory);

                        //创建事物参数
                        TrackParameter transp = new TrackParameter()
                        {
                            Operator = p.Editor,                    //操作人
                            OperateComputer = p.OperateComputer,    //操作客户端
                            RouteOperationName = "入库",            //工序名称
                            RouteName = "",                         //工艺流程名称
                            LineCode = ""                           //线别                                                  
                        };

                        transp.TransactionKeys.Add(lot.Key, transactionKey);        //批次事物主键

                        //处理入库操作
                        if (p.OperationType == 0)
                        {
                            //事物状态
                            transp.Activity = EnumLotActivity.TrackIn;

                            //创建批次事物
                            result = CreateLotTransaction(transp, lot, transactionKey, lstLotTransInsert);

                            //增加批次属性                            
                            lot.RouteName = transp.RouteName;                   //工艺流程
                            lot.RouteStepName = transp.RouteOperationName;      //工步
                            lot.StateFlag = EnumLotState.Apply;                 //批次状态：入库申请
                            lot.EquipmentCode = transp.EquipmentCode;           //设备代码
                            lot.StartWaitTime = now;                            //开始等待时间
                            lot.StartProcessTime = now;                         //开始处理时间
                            lot.Editor = transp.Operator;                       //编辑人
                            lot.EditTime = now;                                 //编辑日期
                        }
                        else
                        {
                            //创建撤销批次事物
                            result = CreateUndoLotTransaction(transp, ref lot, transactionKey, lstLotTransInsert, lstLotTransUpdate);
                        }

                        if (result.Code > 0)
                        {
                            return result;
                        }

                        //加入报废批次更新列表
                        lstLotForUpdate.Add(lot);
                    }
                    
                    #endregion
                }
                else
                {
                    #region 2.2循环设置明细托号对应批次信息
                    foreach (WOReportDetail worDetial in resultWORDetail.Data)
                    {
                        //取得托批次列表
                        packageCode = worDetial.ObjectNumber;

                        //判断是否已经处理
                        isProcess = false;

                        for (int i = lstPackageForUpdate.Count - 1; i >= 0; i--)
                        {
                            if (packageCode == lstPackageForUpdate[i].Key)
                            {
                                isProcess = true;

                                break;
                            }
                        }

                        if (isProcess)
                        {
                            continue;
                        }

                        //取得托对象
                        Package package = PackageDataEngine.Get(packageCode);

                        if (p.OperationType == 0)
                        {
                            //入库申报属性
                            package.PackageState = EnumPackageState.Apply;
                        }
                        else
                        {
                            #region 判断托号状态
                            if (package.PackageState != EnumPackageState.Apply)
                            {
                                result.Code = 1004;
                                result.Message = String.Format("托号[{0}]状态[{1}]非入库申报状态，不可执行撤销动作！",
                                                               package.Key, package.PackageState);

                                return result;
                            }

                            #endregion

                            //申报撤销属性
                            package.PackageState = EnumPackageState.InStoragelist;
                        }

                        package.Editor = p.Editor;      //编辑人
                        package.EditTime = now;         //编辑时间

                        //加入托更新列表
                        lstPackageForUpdate.Add(package);

                        //取得托批次列表
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            OrderBy = "ItemNo",
                            Where = string.Format(" Key.PackageNo = '{0}'"
                                                        , packageCode)
                        };

                        IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);

                        //循环设置批次事物信息
                        foreach (PackageDetail packageDetail in lstPackageDetail)
                        {
                            OemData oemLot = this.OemDataEngine.Get(packageDetail.Key.ObjectNumber);
                            if (oemLot != null)
                            {
                                #region oem组件
                                //处理入库操作
                                if (p.OperationType == 0)
                                {
                                    oemLot.Status = EnumOemStatus.Apply;
                                    oemLot.Editor = p.Editor;
                                    oemLot.EditTime = now;
                                }
                                //入库撤销
                                else
                                {
                                    oemLot.Status = EnumOemStatus.Packaged;
                                    oemLot.Editor = p.Editor;
                                    oemLot.EditTime = now;
                                }
                                lstOemLotForUpdate.Add(oemLot);
                                #endregion
                            }
                            else
                            {
                                #region 自制组件
                                //取得批次信息。
                                Lot lot = this.LotDataEngine.Get(packageDetail.Key.ObjectNumber);
                                //判断批次是否存在。
                                if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                                {
                                    result.Code = 2003;
                                    result.Message = string.Format("批次：（{0}）不存在！", lot.Key);
                                    return result;
                                }

                                //批次已撤销
                                if (lot.DeletedFlag == true)
                                {
                                    result.Code = 2004;
                                    result.Message = string.Format("批次：（{0}）已删除！", lot.Key);
                                    return result;
                                }

                                //批次已暂停
                                if (lot.HoldFlag == true)
                                {
                                    result.Code = 2005;
                                    result.Message = string.Format("批次：（{0}）已暂停！", lot.Key);
                                    return result;
                                }

                                //取得事物主键
                                transactionKey = Guid.NewGuid().ToString();

                                //创建批次历史事物
                                LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                                lstLotTransHisInsert.Add(lotHistory);

                                //创建事物参数
                                TrackParameter transp = new TrackParameter()
                                {
                                    Operator = p.Editor,                    //操作人
                                    OperateComputer = p.OperateComputer,    //操作客户端
                                    RouteOperationName = "入库",            //工序名称
                                    RouteName = "",                         //工艺流程名称
                                    LineCode = ""                           //线别                                                  
                                };

                                transp.TransactionKeys.Add(lot.Key, transactionKey);        //批次事物主键

                                //处理入库操作
                                if (p.OperationType == 0)
                                {
                                    //事物状态
                                    transp.Activity = EnumLotActivity.Apply;

                                    //创建批次事物
                                    result = CreateLotTransaction(transp, lot, transactionKey, lstLotTransInsert);

                                    //增加批次属性                            
                                    lot.RouteName = transp.RouteName;                   //工艺流程
                                    lot.RouteStepName = transp.RouteOperationName;      //工步
                                    lot.StateFlag = EnumLotState.WaitTrackOut;          //批次状态（等待出站）
                                    lot.EquipmentCode = transp.EquipmentCode;           //设备代码
                                    lot.StartWaitTime = now;                            //开始等待时间
                                    lot.StartProcessTime = now;                         //开始处理时间
                                    lot.Editor = transp.Operator;                       //编辑人
                                    lot.EditTime = now;                                 //编辑日期
                                }
                                else
                                {
                                    //创建撤销批次事物
                                    result = CreateUndoLotTransaction(transp, ref lot, transactionKey, lstLotTransInsert, lstLotTransUpdate);
                                }

                                if (result.Code > 0)
                                {
                                    return result;
                                }

                                lstLot.Add(lot);
                                #endregion
                            }
                        }
                    }
                    #endregion
                }                
                #endregion

                #region 3.事物处理
                //创建事物对象
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                //1.1更新非报废批次信息
                foreach (Lot lot in lstLot)
                {
                    this.LotDataEngine.Update(lot, session);
                }

                //1.2更新报废批次信息
                foreach (Lot lot in lstLotForUpdate)
                {
                    this.LotDataEngine.Update(lot, session);
                }

                //2.更新事物信息
                foreach (LotTransaction lotTransaction in lstLotTransInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //3.更新批次事物信息
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransHisInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                }

                //4.更新历史事物信息
                foreach (LotTransaction lotTrans in lstLotTransUpdate)
                {
                    this.LotTransactionDataEngine.Update(lotTrans, session);
                }

                //5.更托信息
                foreach (Package package in lstPackageForUpdate)
                {
                    this.PackageDataEngine.Update(package, session);
                }

                //6.更新OEM组件信息
                foreach (OemData oemLot in lstOemLotForUpdate)
                {
                    this.OemDataEngine.Update(oemLot, session);
                }

                //7.更新入库单
                this.WOReportDataEngine.Update(woReport, session);

                //开始事物处理                                
                transaction.Commit();
                session.Close();
                #endregion
                 
                return result;
            }
            catch (Exception e)
            { 
                result.Code = 1002;
                result.Message = e.Message + e.Source;

                transaction.Rollback();
                session.Close();

                return result;
            }
        }
        
        /// <summary>
        /// 创建批次事物
        /// </summary>
        /// <param name="p">事物参数</param>
        /// <param name="lot">批次信息</param>
        /// <param name="transactionKey">事物主键</param>
        /// <param name="lstLotTransInsert">批次插入事物列表</param>
        /// <returns></returns>
        public MethodReturnResult CreateLotTransaction(TrackParameter p, Lot lot, string transactionKey, List<LotTransaction> lstLotTransInsert)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                DateTime now = DateTime.Now;

                //记录操作事物数据
                LotTransaction transLot = new LotTransaction()
                {
                    Key = transactionKey,                               //事物主键     
                    Activity = p.Activity,                              //批次状态  
                    CreateTime = now,                                   //创建时间
                    Creator = p.Operator,                               //创建人
                    Description = "",                                   //描述
                    Editor = p.Operator,                                //编辑人
                    EditTime = now,                                     //编辑时间
                    InQuantity = lot.Quantity,                          //数量
                    LotNumber = lot.Key,                                //组件批次号
                    LocationName = lot.LocationName,                    //车间
                    LineCode = p.LineCode,                              //线别
                    OperateComputer = p.OperateComputer,                //操作电脑
                    OrderNumber = lot.OrderNumber,                      //工单
                    OutQuantity = lot.Quantity,                         //出站数量
                    RouteEnterpriseName = "",                           //工艺流程组
                    RouteName = p.RouteName,                            //工艺流程
                    RouteStepName = p.RouteOperationName,               //工序名称
                    ShiftName = "",                                     //班别
                    UndoFlag = false,                                   //撤销标识
                    UndoTransactionKey = "",                            //撤销主键
                    Grade = lot.Grade,                                  //等级
                    Color = lot.Color,                                  //花色
                    Attr1 = lot.Attr1,                                  //批次属性1
                    Attr2 = lot.Attr2,                                  //批次属性2
                    Attr3 = "",                                  //批次属性3
                    Attr4 = "",                                  //批次属性4
                    Attr5 = "",                                  //批次属性5
                    OriginalOrderNumber = ""                     //原始工单
                };

                //批次状态事物
                //LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);

                //批次事物属性修改                
                //lotHistory.OperateComputer = p.OperateComputer;             //操作电脑
                //lotHistory.Creator = p.Operator;                            //创建人
                //lotHistory.CreateTime = now;                                //创建时间
                //lotHistory.Editor = p.Operator;                             //编辑人
                //lotHistory.EditTime = now;                                  //编辑时间

                //if (p.BillState == EnumBillState.Apply)
                //{
                //    transLot.Activity = EnumLotActivity.TrackIn;            //批次进站
                //    lotHistory.StateFlag = EnumLotState.WaitTrackOut;       //进状态
                //}
                //else
                //{
                //    transLot.Activity = EnumLotActivity.Terminal;           //批次结束
                //    lotHistory.StateFlag = EnumLotState.Finished;           //批次完成作业

                //    //完成接收后设置批次状态为入库
                //    lot.StateFlag = EnumLotState.ToWarehouse;               //完成入库状态
                //}

                //增加事物列表
                lstLotTransInsert.Add(transLot);

                //增加批次事物列表
                //lstLotTransHisInsert.Add(lotHistory);

                return result;
            }
            catch (Exception e)
            {
                result.Code = 1000;
                result.Message = e.Message + e.Source;

                return result;
            }
        }

        /// <summary>
        /// 创建撤销批次事物
        /// </summary>
        /// <param name="p">事物参数</param>
        /// <param name="lot">批次信息</param>
        /// <param name="transactionKey">事物主键</param>
        /// <param name="lstLotTransInsert">批次插入事物列表</param>
        /// <param name="lstLotTransUpdate">批次更新事物列表</param>
        /// <returns></returns>
        public MethodReturnResult CreateUndoLotTransaction(TrackParameter p,ref Lot lot, string transactionKey,
                List<LotTransaction> lstLotTransInsert, List<LotTransaction> lstLotTransUpdate)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                DateTime now = DateTime.Now;
                LotTransaction lotTransLast = new LotTransaction();
                LotTransactionHistory lotHistoryLast = new LotTransactionHistory();

                #region 1.取得最后批次事物对象(不包含已撤销事物及撤销操作事物)
                //1.1按时间序列取得事物列表
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(" LotNumber = '{0}' " +
                                          " and UndoFlag = 0 and Activity <> -1 and Activity <> 7 and RouteStepName='入库'"
                                          , lot.Key),
                    //Where = string.Format(" Key = '{0}' " ,lot.LastTransactionKey),
                    OrderBy = "EditTime DESC"
                };

                IList<LotTransaction> lstLotTrans = this.LotTransactionDataEngine.Get(cfg);
                DateTime newTime = DateTime.Now;

                //1.2判断事物是否存在
                if (lstLotTrans.Count <= 0)
                {
                    result.Code = 1001;
                    result.Message = String.Format("批次[{0}]事物不存在或入库动作无法撤销！", lot.Key);

                    return result;
                }
                
                //1.3取得事物对象
                lotTransLast = lstLotTrans[0];

                //1.4判断当前事物与批次站是否一致
                //if (lotTransLast.RouteStepName != lot.RouteStepName)
                //{
                //    result.Code = 1001;
                //    result.Message = String.Format("批次所在站[{0}]与当前事物站[{1}]不一致！",
                //                                    lot.Key,
                //                                    lotTransLast.RouteStepName);

                //    return result;
                //}

                //修改事物撤销属性
                lotTransLast.UndoFlag = true;                           //撤销标识
                lotTransLast.UndoTransactionKey = transactionKey;       //撤销事物主键
                lotTransLast.Editor = p.Operator;
                lotTransLast.EditTime = now;

                lstLotTransUpdate.Add(lotTransLast);
                #endregion

                #region 2.创建当前批次状态历史纪录
                //2.1根据批次创建批次状态记录事物
                LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);

                //批次事物属性修改
                //lotHistory.OperateComputer = p.OperateComputer;             //操作电脑
                //lotHistory.Creator = p.Operator;                            //创建人
                //lotHistory.CreateTime = now;                                //创建时间
                //lotHistory.Editor = p.Operator;                             //编辑人
                //lotHistory.EditTime = now;                                  //编辑时间

                #endregion
                
                #region 3.创建撤销事物对象
                //记录操作事物数据
                LotTransaction transLot = new LotTransaction()
                {
                    Key = transactionKey,                               //事物主键     
                    Activity = EnumLotActivity.Undo,                    //批次操作（撤销）  
                    CreateTime = now,                                   //创建时间
                    Creator = p.Operator,                               //创建人
                    Description = "",                                   //描述
                    Editor = p.Operator,                                //编辑人
                    EditTime = now,                                     //编辑时间
                    InQuantity = lot.Quantity,                          //数量
                    LotNumber = lot.Key,                                //组件批次号
                    LocationName = lot.LocationName,                    //车间
                    LineCode = p.LineCode,                              //线别
                    OperateComputer = p.OperateComputer,                //操作电脑
                    OrderNumber = lot.OrderNumber,                      //工单
                    OutQuantity = lot.Quantity,                         //出站数量
                    RouteEnterpriseName = "",                           //工艺流程组
                    RouteName = p.RouteName,                            //工艺流程
                    RouteStepName = p.RouteOperationName,               //工序名称
                    ShiftName = "",                                     //班别
                    UndoFlag = false,                                   //撤销标识
                    UndoTransactionKey = "",                            //撤销主键
                    Grade = lot.Grade,                                  //等级
                    Color = lot.Color,                                  //花色
                    Attr1 = lot.Attr1,                                  //批次属性1
                    Attr2 = "",                                         //批次属性2
                    Attr3 = "",                                         //批次属性3
                    Attr4 = "",                                         //批次属性4
                    Attr5 = "",                                         //批次属性5
                    OriginalOrderNumber = ""                            //原始工单
                };
                
                //增加事物列表
                lstLotTransInsert.Add(transLot);

                //增加批次事物列表
                //lstLotTransHisInsert.Add(lotHistory);
                #endregion
                
                #region 4.恢复批次历史信息
                //4.1取得对应历史数据
                lotHistoryLast = this.LotTransactionHistoryDataEngine.Get(lotTransLast.Key);

                if (lotHistoryLast == null)
                {
                    result.Code = 1001;
                    result.Message = String.Format("批次[{0}]历史事物不存在！", lot.Key);

                    return result;
                }

                //4.2恢复批次历史信息
                lot = new Lot(lotHistoryLast);

                //lot.OperateComputer = p.OperateComputer;             //操作电脑
                //lot.Creator = p.Operator;                            //创建人
                //lot.CreateTime = now;                                //创建时间
                //lot.Editor = p.Operator;                             //编辑人
                //lot.EditTime = now;                                  //编辑时间

                #endregion

                return result;
            }
            catch (Exception e)
            {
                result.Code = 1000;
                result.Message = e.Message + e.Source;

                return result;
            }
        }

        /// <summary>
        /// 入库申请单接收或撤销接收
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult StockIn(WOReportParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();

            List<Lot> lstLot = new List<Lot>();                                                     //批次对象列表
            List<LotTransaction> lstLotTransInsert = new List<LotTransaction>();                    //批次事物列表
            List<LotTransactionHistory> lstLotTransHisInsert = new List<LotTransactionHistory>();   //批次历史事物列表
            List<LotTransaction> lstLotTransUpdate = new List<LotTransaction>();                    //批次事物列表
            List<Package> lstPackageForUpdate = new List<Package>();                                //托对象列表
            List<OemData> lstOemLotForUpdate = new List<OemData>();                                 //OEM组件事物列表
            string ERPStockInBillCode = "";             //ERP入库单号
            string ERPStockInBillKey = "";              //ERP入库单主键

            ITransaction transaction = null;
            ISession session = null;

            try
            {
                #region 1.处理入库单事物对象
                WOReport woReport = WOReportDataEngine.Get(p.BillCode);

                if (woReport == null)
                {
                    result.Code = 1001;
                    result.Message = String.Format("入库单[{0}]不存在！");

                    return result;
                }

                if (p.OperationType == 0)
                {
                    //入库申报
                    if (woReport.BillState == EnumBillState.Receive)
                    {
                        result.Code = 1002;
                        result.Message = String.Format("ERP入库单已经生成，请核查！");

                        return result;
                    }

                    woReport.BillState = p.BillState;           //入库申请操作状态
                    woReport.Store = p.Store;                   //仓库代码
                }
                else
                {
                    //入库撤销
                    woReport.BillState = EnumBillState.Apply;   //状态
                    woReport.Store = "";                        //仓库代码
                }

                woReport.Editor = p.Editor;                     //编辑人
                woReport.EditTime = DateTime.Now;               //编辑日期

                #endregion

                #region 2.处理入库单明细对象
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(" Key.BillCode = '{0}'"
                                                , p.BillCode)
                };

                MethodReturnResult<IList<WOReportDetail>> resultWORDetail = this.GetWOReportDetail(ref cfg);

                if (resultWORDetail.Code > 0)
                {
                    return resultWORDetail;
                }

                string packageCode = "";
                bool isProcess = false;
                string transactionKey = "";
                DateTime now = DateTime.Now;                            //当前时间

                if (woReport.ScrapType == EnumScrapType.True)
                {
                    #region 2.1设置报废批次的批次信息
                    #endregion
                }
                else
                {
                    #region 2.2循环设置明细托号对应批次信息
                    for (int i = 0; i < resultWORDetail.Data.Count; i++)
                    {
                        #region 设置入库单明细（根据项目号）对应的入库单号及主键
                        if (p.OperationType == 0)
                        {
                            //1.根据行号取得对应的ERP入库单主键及入库单号
                            ERPStockInBillCode = p.ERPStockInCodes[resultWORDetail.Data[i].Key.ItemNo.ToString()];
                            ERPStockInBillKey = p.ERPStockInKeys[resultWORDetail.Data[i].Key.ItemNo.ToString()];

                            //判断ERP入库单号
                            if (ERPStockInBillCode == "")
                            {
                                result.Code = 1003;
                                result.Message = String.Format("第[{0}]行明细数据ERP入库单号未设置！",
                                                                resultWORDetail.Data[i].Key.ItemNo.ToString());

                                return result;
                            }

                            resultWORDetail.Data[i].ERPStockInCode = ERPStockInBillCode;      //ERP入库单号

                            //判断ERP入库单主键
                            if (ERPStockInBillKey == "")
                            {
                                result.Code = 1004;
                                result.Message = String.Format("第[{0}]行明细数据ERP入库单主键未设置！",
                                                                resultWORDetail.Data[i].Key.ItemNo.ToString());

                                return result;
                            }

                            resultWORDetail.Data[i].ERPStockInKey = ERPStockInBillKey;      //ERP入库单主键 
                        }
                        else
                        {
                            resultWORDetail.Data[i].ERPStockInCode = "";                    //ERP入库单号
                            resultWORDetail.Data[i].ERPStockInKey = "";                     //ERP入库单主键
                        }

                        resultWORDetail.Data[i].Editor = p.Editor;                      //编辑人
                        resultWORDetail.Data[i].EditTime = now;                         //编辑日期
                        #endregion

                        //取得托批次列表
                        packageCode = resultWORDetail.Data[i].ObjectNumber;

                        //判断批次托是否已经处理
                        isProcess = false;

                        for (int n = lstPackageForUpdate.Count - 1; n >= 0; n--)
                        {
                            if (packageCode == lstPackageForUpdate[n].Key)
                            {
                                isProcess = true;

                                break;
                            }
                        }

                        if (isProcess)
                        {
                            continue;
                        }

                        //取得托对象
                        Package package = PackageDataEngine.Get(packageCode);
                       
                        

                        if (p.OperationType == 0)
                        {
                            //入库接收属性
                            package.PackageState = EnumPackageState.ToWarehouse;
                        }
                        else
                        {
                            #region 判断托号状态
                            if (package.PackageState != EnumPackageState.ToWarehouse)
                            {
                                result.Code = 1004;
                                result.Message = String.Format("托号[{0}]状态[{1}]非已入库状态，不可执行撤销动作！",
                                                               package.Key, package.PackageState);

                                return result;
                            }

                            #endregion

                            //入库撤销属性
                            package.PackageState = EnumPackageState.Apply;
                        }

                        package.Editor = p.Editor;      //编辑人
                        package.EditTime = now;         //编辑时间

                        //加入托更新列表
                        lstPackageForUpdate.Add(package);

                        //取得托批次列表
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            OrderBy = "ItemNo",
                            Where = string.Format(" Key.PackageNo = '{0}'"
                                                        , packageCode)
                        };

                        IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);

                        //循环设置批次事物信息
                        foreach (PackageDetail packageDetail in lstPackageDetail)
                        {
                            OemData oemLot = this.OemDataEngine.Get(packageDetail.Key.ObjectNumber);
                            if (oemLot != null)
                            {
                                #region oem组件
                                //处理入库操作
                                if (p.OperationType == 0)
                                {
                                    oemLot.Status = EnumOemStatus.ToWarehouse;
                                    oemLot.Editor = p.Editor;
                                    oemLot.EditTime = now;
                                }
                                //入库撤销
                                else
                                {
                                    oemLot.Status = EnumOemStatus.Apply;
                                    oemLot.Editor = p.Editor;
                                    oemLot.EditTime = now;
                                }
                                lstOemLotForUpdate.Add(oemLot);
                                #endregion
                            }
                            else
                            {
                                #region 自制组件
                                //取得批次信息。
                                Lot lot = this.LotDataEngine.Get(packageDetail.Key.ObjectNumber);
                                //判断批次是否存在。
                                if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                                {
                                    result.Code = 2003;
                                    result.Message = string.Format("批次：（{0}）不存在！", lot.Key);
                                    return result;
                                }

                                //批次已撤销
                                if (lot.DeletedFlag == true)
                                {
                                    result.Code = 2004;
                                    result.Message = string.Format("批次：（{0}）已删除！", lot.Key);
                                    return result;
                                }

                                //批次已暂停
                                if (lot.HoldFlag == true)
                                {
                                    result.Code = 2005;
                                    result.Message = string.Format("批次：（{0}）已暂停！", lot.Key);
                                    return result;
                                }

                                //取得事物主键
                                transactionKey = Guid.NewGuid().ToString();

                                //创建批次历史事物
                                LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                                lstLotTransHisInsert.Add(lotHistory);

                                //创建事物参数
                                TrackParameter transp = new TrackParameter()
                                {
                                    Activity = EnumLotActivity.Terminal,    //结束事物标志
                                    Operator = p.Editor,                    //操作人
                                    OperateComputer = p.OperateComputer,    //操作客户端
                                    RouteOperationName = "入库",            //工序名称
                                    RouteName = "",                         //工艺流程名称
                                    LineCode = ""                           //线别 

                                };

                                transp.TransactionKeys.Add(lot.Key, transactionKey);        //批次事物主键

                                //处理撤销操作
                                if (p.OperationType == 0)
                                {
                                    //事物状态
                                    transp.Activity = EnumLotActivity.Terminal;

                                    //创建批次事物
                                    result = CreateLotTransaction(transp, lot, transactionKey, lstLotTransInsert);

                                    //增加批次属性                            
                                    lot.RouteName = transp.RouteName;                   //工艺流程
                                    lot.RouteStepName = transp.RouteOperationName;      //工步
                                    lot.StateFlag = EnumLotState.Finished;              //批次状态（完成）
                                    lot.EquipmentCode = transp.EquipmentCode;           //设备代码
                                    lot.StartWaitTime = now;                            //开始等待时间
                                    lot.StartProcessTime = now;                         //开始处理时间
                                    lot.Editor = transp.Operator;                       //编辑人
                                    lot.EditTime = now;                                 //编辑日期
                                }
                                else
                                {
                                    //创建撤销批次事物
                                    result = CreateUndoLotTransaction(transp, ref lot, transactionKey, lstLotTransInsert, lstLotTransUpdate);
                                }

                                if (result.Code > 0)
                                {
                                    return result;
                                }

                                lstLot.Add(lot);
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
                
                #endregion

                #region 3.事物处理
                //创建事物对象
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                //1.更新入库单
                this.WOReportDataEngine.Update(woReport, session);

                //2.更新入库单明细
                foreach (WOReportDetail worDetial in resultWORDetail.Data)
                {
                    this.WOReportDetailDataEngine.Update(worDetial, session);
                }

                //3.更新批次信息
                foreach (Lot lot in lstLot)
                {
                    this.LotDataEngine.Update(lot, session);
                }

                //4.更新事物信息
                foreach (LotTransaction lotTransaction in lstLotTransInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //5.更新批次事物信息
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransHisInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                }

                //6.更新历史事物信息
                foreach (LotTransaction lotTrans in lstLotTransUpdate)
                {
                    this.LotTransactionDataEngine.Update(lotTrans, session);
                }

                //7.更托信息
                foreach (Package package in lstPackageForUpdate)
                {
                    this.PackageDataEngine.Update(package, session);
                }

                //8.更新OEM组件信息
                foreach (OemData oemLot in lstOemLotForUpdate)
                {
                    this.OemDataEngine.Update(oemLot, session);
                }

                //开始事物处理                                
                transaction.Commit();
                session.Close();
                #endregion

                return result;
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message + e.Source;

                if (transaction != null)
                {
                    transaction.Rollback();
                    session.Close();
                }

                return result;
            }
        }

        /// <summary>
        /// 根据ERP产成品入库单表头主键取得ERP入库单号
        /// </summary>
        /// <param name="ERPStockInKey">产成品入库单表头主键</param>
        /// <param name="V_IC_FINPRODIN_H">产成品入库单表头视图</param>
        /// <returns></returns>
        public MethodReturnResult GetERPStockInBillCodeByKey(string ERPStockInKey)
        {
            MethodReturnResult<DataSet> resultORLData = new MethodReturnResult<DataSet>();
            MethodReturnResult result = new MethodReturnResult();
            
            string ERPStockInBillCode = "";        //ERP入库单号

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())//根据报工单回执查询入库单回执
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select vbillcode " +
                                                       "from " + ErpDBName + ".V_IC_FINPRODIN_H " +
                                                       "where groupcode = '{0}' and orgcode='{1}' and CGENERALHID = '{2}' ",
                                                       ErpGroupCode,
                                                       ErpORGCode,
                                                       ERPStockInKey);

                    resultORLData.Data = Ora_db.ExecuteDataSet(cmd);

                    if (resultORLData.Code > 0)
                    {
                        result.Code = resultORLData.Code;
                        result.Message = resultORLData.Message;

                        return result;
                    }

                    if (resultORLData.Data.Tables[0].Rows.Count == 0)
                    {
                        result.Code = 2000;
                        result.Message = string.Format("入库主键[{0}]对应ERP入库单号提取失败！",
                                                       ERPStockInKey);
                        return result;
                    }
                    else
                    {
                        //取得生成的ERP入库单号
                        ERPStockInBillCode = resultORLData.Data.Tables[0].Rows[0]["vbillcode"].ToString();
                    }

                    result.ObjectNo = ERPStockInBillCode;
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

        //根据V_Package视图获取包装信息（产品编码/产品名称/包装号/等级/标准功率/功率档/包装数量/托内功率合计）
        public MethodReturnResult<DataSet> GetPackageInfo(string ObjectNumber)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandText = string.Format(@" select * from V_Package t1
                                                        where t1.PACKAGE_NO = '{0}'", ObjectNumber);
                    result.Data = _db.ExecuteDataSet(cmd);
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

        //如果是报废获取批次信息，否则获取包装信息（产品编码/产品名称/包装号/等级/标准功率/功率档/包装数量/托内功率合计）
        public MethodReturnResult<DataSet> GetPackageInfoEx(string ObjectNumber, EnumScrapType ScrapType)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    if (ScrapType == EnumScrapType.True)
                    {
                        cmd.CommandText = string.Format(@" select * from V_LotNumber t1
                                                        where t1.LOT_NUMBER = '{0}'", ObjectNumber);
                    }
                    else
                    { 
                        cmd.CommandText = string.Format(@" select * from V_Package t1
                                                        where t1.PACKAGE_NO = '{0}'", ObjectNumber);
                    }

                    result.Data = _db.ExecuteDataSet(cmd);

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

        //MES数据【报废】入库申请执行操作
        public MethodReturnResult WO(WOReportParameter p, string ScrapType)
        {
            MethodReturnResult result = new MethodReturnResult(){ Code = 0 };
            ISession db = null;
            ITransaction transaction = null;
            try
            {
                #region 1.处理入库单表头数据
                WOReport woReport = this.WOReportDataEngine.Get(p.BillCode);
                if (woReport != null)
                {
                    woReport.BillState = p.BillState;
                    woReport.WRCode = p.ERPWorkReportCode;      //ERP报工单号
                    woReport.ERPWRKey = p.ERPWorkReportKey;     //ERP报工单主键
                    woReport.Editor = p.Editor;
                    woReport.EditTime = DateTime.Now;
                }
                #endregion

                #region 2.处理入库单明细中托及托内数据
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "ItemNo",
                    Where = string.Format(" Key.BillCode = '{0}'"
                                                , p.BillCode)
                };
                IList<WOReportDetail> lstWOReportDetail = this.WOReportDetailDataEngine.Get(cfg);

                //更新托信息为在库
                List<Package> lstPackage = new List<Package>();
                List<Lot> lstLot = new List<Lot>();
                foreach (var item in lstWOReportDetail)
                {
                    if (ScrapType == "True")
                    {
                        Lot lot = this.LotDataEngine.Get(item.ObjectNumber);
                        lot.LotState = (int)EnumLotState.Apply;
                        lot.InOrder = 1;
                        lstLot.Add(lot);

                    }
                    else
                    {
                        Package package = this.PackageDataEngine.Get(item.ObjectNumber);
                        package.PackageState = EnumPackageState.Apply;
                        package.InOrder = 1;
                        lstPackage.Add(package);
                    }

                }
                #endregion

                #region 3.事务处理
                db = this.SessionFactory.OpenSession();
                transaction = db.BeginTransaction();

                this.WOReportDataEngine.Update(woReport, db);

                if (lstLot != null && lstLot.Count > 0)
                {
                    foreach (var item in lstLot)
                    {
                        this.LotDataEngine.Update(item, db);
                    }
                }

                foreach (var item in lstPackage)
                {
                    this.PackageDataEngine.Update(item, db);
                }

                transaction.Commit();
                db.Close();
                #endregion
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += string.Format("报工单回填状态失败！", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;            
        }

        //获取ERP系统内组件电流/组件等级/组件功率对应代码
        public MethodReturnResult<DataSet> GetCodeByName(string Name,string ListCode)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_defdoc_batteryncomp where groupcode = '{0}' and name = '{1}' and listcode='{2}' ",
                                                    ErpGroupCode,                            
                                                    Name,
                                                    ListCode);

                    result.Data = Ora_db.ExecuteDataSet(cmd);
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
        
        /// <summary>
        /// 根据MES入库单号取得ERP系统产成品入库单表体入库单主键及入库单号
        /// </summary>
        /// <param name="BillCode">MES入库单号</param>
        /// <param name="ic_finprodin_b">产成品入库单表体</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPCodeByBillCode(string BillCode)
        {
            MethodReturnResult<DataSet> resultEx = new MethodReturnResult<DataSet>();
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            string erpERPWorkReportKey = "";        //ERP报工单主键

            try
            {
                using (DbConnection con = this._db.CreateConnection())//根据入库单号查询报工单回执号
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" SELECT * FROM dbo.ERP_WO_REPORT WHERE BILL_CODE = '{0}'", BillCode);
                    resultEx.Data = _db.ExecuteDataSet(cmd);

                    erpERPWorkReportKey = resultEx.Data.Tables[0].Rows[0]["ERP_WR_KEY"].ToString(); 
                }

                using (DbConnection con = this.Ora_db.CreateConnection())//根据报工单回执查询入库单回执
                {
                    DbCommand cmd = con.CreateCommand();
                    //根据ERP报工单主键取得ERP系统产成品入库单表体入库单主键及入库单号
                    cmd.CommandText = string.Format(@" select cgeneralhid,vbillcode from " + ErpDBName + ".v_ic_finprodin_b where groupcode = '{0}' and orgcode='{1}' and csourcebillhid = '{2}' ",
                                                ErpGroupCode,
                                                ErpORGCode,                            
                                                erpERPWorkReportKey);
                    
                    result.Data = Ora_db.ExecuteDataSet(cmd);
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

        /// <summary>
        /// 获取ERP系统组件成品入库的仓库列表
        /// </summary>
        /// <param name="v_bd_stordoc">ERP仓库视图</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetStore()
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_bd_stordoc t where t.groupcode = '{0}' and t.orgcode='{1}' and (t.storname like '%成品%' or t.storname like '%受托加工物资%')",
                                                      ErpGroupCode,
                                                      ErpORGCode);                    

                    result.Data = Ora_db.ExecuteDataSet(cmd);
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

        /// <summary>
        /// 获取ERP系统组件报废入库的仓库列表
        /// </summary>
        /// <param name="v_bd_stordoc">ERP仓库视图</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> sGetStore()
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_bd_stordoc t where t.groupcode = '{0}' and t.orgcode='{1}' and t.storname like '%废料%'",
                                                      ErpGroupCode,
                                                      ErpORGCode);

                    result.Data = Ora_db.ExecuteDataSet(cmd);
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

        /// <summary>
        /// 获取ERP系统组件在制品入库的仓库列表
        /// </summary>
        /// <param name="v_bd_stordoc">ERP仓库视图</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> wGetStore()
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_bd_stordoc t where t.groupcode = '{0}' and t.orgcode='{1}' and t.storname like '%组件在制%'",
                                                      ErpGroupCode,
                                                      ErpORGCode);

                    result.Data = Ora_db.ExecuteDataSet(cmd);
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

        /// <summary>
        /// 根据物料编码获取ERP系统内物料主辅单位
        /// </summary>
        /// <param name="MaterialCode">物料编码</param>
        /// <param name="v_material_convert">物料主辅单位视图</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetUnitByMaterialCode(string MaterialCode)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_material_convert  where groupcode = '{0}' and orgcode='{1}' and invcode = '{2}' ",
                                                    ErpGroupCode,
                                                    ErpORGCode,
                                                    MaterialCode);

                    result.Data = Ora_db.ExecuteDataSet(cmd);
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

        //MES数据入库接收执行操作
        public MethodReturnResult WI(WOReportParameter p)
        {
            List<Lot> lstLotDataEngineForUpdate = new List<Lot>();
            MethodReturnResult result = new MethodReturnResult() { Code = 0 };
            ISession db = null;
            ITransaction transaction = null;

            try
            {
                #region 1.处理入库单表头数据
                WOReport woReport = this.WOReportDataEngine.Get(p.BillCode);
                if (woReport != null)
                {
                    //woReport.INCode = p.INCode;
                    woReport.Store = p.Store;
                    woReport.Editor = p.Editor;
                    woReport.EditTime = DateTime.Now;
                    woReport.BillState = EnumBillState.Receive;

                }
                #endregion

                #region 2.处理入库单明细中托及托内数据
                //if (woReport.ScrapType == EnumScrapType.True)

                //if (!string.IsNullOrEmpty(p.INCode))
                //{
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.BillCode='{0}'", p.BillCode)
                };
                IList<WOReportDetail> woReportDetail = this.WOReportDetailDataEngine.Get(cfg);
                if (woReportDetail.Count > 0)
                {
                    foreach (var data in woReportDetail)
                    {
                        cfg = new PagingConfig()
                                {
                                    IsPaging = false,
                                    Where = string.Format("PackageNo='{0}'", data.ObjectNumber)
                                };
                        IList<Lot> lstlot = this.LotDataEngine.Get(cfg);
                        if (lstlot.Count > 0)
                        {
                            foreach (var item in lstlot)
                            {
                                item.StateFlag = EnumLotState.Finished;
                                lstLotDataEngineForUpdate.Add(item);
                            }
                        }
                    }
                }
                //}
                #endregion

                #region 3.事务处理
                db = this.SessionFactory.OpenSession();
                transaction = db.BeginTransaction();

                //处理入库单表头数据
                this.WOReportDataEngine.Update(woReport, db);

                //处理入库单明细之托内批次号
                if (lstLotDataEngineForUpdate.Count > 0)
                {
                    foreach (var data in lstLotDataEngineForUpdate)
                    {
                        this.LotDataEngine.Update(data, db);
                    }
                }

                transaction.Commit();
                db.Close();
                #endregion
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += string.Format("入库单单回填状态失败！", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        
        //从MES系统获取入库单表头及表体明细
        public MethodReturnResult<DataSet> GetWOReportFromDB(string BillCode)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            DataSet dsResult = new DataSet();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" SELECT [BILL_CODE]
                      ,[BILL_DATE],[NOTE],[BILL_MAKER],[BILL_MAKEDDATE]
                      ,[MIXTYPE],[ORDER_NUMBER],[STORE],[MATERIAL_CODE]
                      ,[TOTAL_QTY] ,[BILL_STATE] ,[ERP_IN_CODE],[ERP_WR_CODE]
                      ,[CREATOR],[CREATE_TIME],[EDITOR] ,[EDIT_TIME],[SCRAP_TYPE]
                      FROM [dbo].[ERP_WO_REPORT] where BILL_CODE='{0}' and BILL_STATE>0 ", BillCode);
                    ds = _db.ExecuteDataSet(cmd);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        dt.TableName = "StgIn";
                        dsResult.Tables.Add(dt.Copy());
                        cmd = con.CreateCommand();
                        if (dt.Rows[0]["SCRAP_TYPE"].ToString() == "1")
                        {
                            cmd.CommandText = string.Format(
                                @"select t2.MATERIAL_CODE,t2.ORDER_NUMBER, t2.MATERIAL_NAME, t2.LOT_NUMBER PACKAGE_NO,t2.GRADE ,
                        		                                                t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.[COEF_PMAX] sumCOEF_PMAX,count(t2.Lot_Number) as QTY
                                                                                from MM_StockIn_Detail t1
                                                                                inner join [dbo].[V_LotNumber_Detail] t2 on t1.OBJECT_NUMBER=t2.LOT_NUMBER 
                                                                                WHERE t1.BILL_CODE='{0}' group by
                                                                                t2.MATERIAL_CODE, t2.MATERIAL_NAME, t2.LOT_NUMBER,t2.GRADE ,
                        		                                                t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.ORDER_NUMBER,t2.COEF_PMAX ", BillCode);
                        }
                        else if (dt.Rows[0]["SCRAP_TYPE"].ToString() == "0")
                        {

//                            cmd.CommandText = string.Format(
//                                @"select t2.MATERIAL_CODE,t2.ORDER_NUMBER, t2.MATERIAL_NAME, t2.PACKAGE_NO ,t2.GRADE ,
//		                      t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,SUM(t2.[COEF_PMAX]) sumCOEF_PMAX,count(t2.Lot_Number) as QTY
//                              from MM_StockIn_Detail t1
//                              inner join [dbo].[V_Package_Detail] t2 on t1.OBJECT_NUMBER=t2.PACKAGE_NO
//                              WHERE t1.BILL_CODE='{0}' group by
//                              t2.MATERIAL_CODE, t2.MATERIAL_NAME, t2.PACKAGE_NO ,t2.GRADE ,
//		                      t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.ORDER_NUMBER ", BillCode);

                            cmd.CommandText = string.Format(@"select ItemNo,stockin.MATERIAL_CODE,
	                                                                material.MATERIAL_NAME,
	                                                                order_number,
	                                                                OBJECT_NUMBER PACKAGE_NO,
	                                                                EFFI_NAME PM_NAME,
	                                                                EFFI_CODE SPM_VALUE,
	                                                                PS_SUBCODE PS_SUBCODE_Name,
	                                                                GRADE,
	                                                                QUANTITY QTY,
	                                                                sumCOEF_PMAX
                                                            from MM_StockIn_Detail stockin
                                                            inner join FMM_MATERIAL material
	                                                            on stockin.MATERIAL_CODE = material.MATERIAL_CODE
                                                            WHERE BILL_CODE = '{0}' 
                                                            order by ItemNo ",
                                                                   BillCode);
                        }


                        ds = _db.ExecuteDataSet(cmd);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            dt = ds.Tables[0];
                            dt.TableName = "StgInDetail";
                            dsResult.Tables.Add(dt.Copy());
                        }
                    }
                    result.Data = dsResult;
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
        
        public MethodReturnResult<DataSet> GetERPReportCodeById(string strId)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_mm_wr where groupcode = '{0}' and orgcode='{1}' and pk_wr = '{2}' ",
                                                      ErpGroupCode,
                                                      ErpORGCode,                            
                                                      strId);

                    result.Data = Ora_db.ExecuteDataSet(cmd);
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

        public MethodReturnResult<DataSet> GetReportDetailByObjectNumber(string BillCode, string Scrap_Type)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    if (Scrap_Type == "True")
                    {

                        cmd.CommandText = string.Format(@"select t2.MATERIAL_CODE,t2.ORDER_NUMBER, t2.MATERIAL_NAME, t2.LOT_NUMBER PACKAGE_NO,t2.GRADE ,
                        		                                                t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.[COEF_PMAX] sumCOEF_PMAX,count(t2.Lot_Number) as QTY
                                                                                from ERP_WO_REPORT_DETAIL t1
                                                                                inner join [dbo].[V_LotNumber_Detail] t2 on t1.OBJECT_NUMBER=t2.LOT_NUMBER 
                                                                                WHERE t1.BILL_CODE='{0}' group by
                                                                                t2.MATERIAL_CODE, t2.MATERIAL_NAME, t2.LOT_NUMBER,t2.GRADE ,
                        		                                                t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.ORDER_NUMBER,t2.COEF_PMAX", BillCode);
                    }
                    else
                    {

                        cmd.CommandText = string.Format(@" select t2.MATERIAL_CODE,t2.ORDER_NUMBER, t2.MATERIAL_NAME, t2.PACKAGE_NO,t2.GRADE ,
                        		                                                t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,SUM(t2.[COEF_PMAX]) sumCOEF_PMAX,count(t2.Lot_Number) as QTY
                                                                                from ERP_WO_REPORT_DETAIL t1
                                                                                inner join [dbo].[V_Package_Detail] t2 on t1.OBJECT_NUMBER=t2.PACKAGE_NO
                                                                                WHERE t1.BILL_CODE='{0}' group by
                                                                                t2.MATERIAL_CODE, t2.MATERIAL_NAME, t2.PACKAGE_NO,t2.GRADE ,
                        		                                                t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.ORDER_NUMBER", BillCode);

                        //cmd.CommandText = string.Format(@" SELECT * FROM dbo.TEST3", BillCode);
                    
                    }

                    result.Data = _db.ExecuteDataSet(cmd);
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
              
        /// <summary>
        /// 校验入库单明细是否合理（建议删除！！！，对于已经在入库单中托号不允许拆托）
        /// </summary>
        /// <param name="BillCode"></param>
        /// <returns></returns>
        public MethodReturnResult<bool> CheckReportDetail(string BillCode)
        {
            MethodReturnResult<bool> result = new MethodReturnResult<bool>();
            DataSet pacageSet = new DataSet();
            DataSet billSet = new DataSet();
            int pacageQty = 0;
            int billQty = 0;

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" SELECT count(t2.Lot_Number) as QTY
                                                          from ERP_WO_REPORT_DETAIL t1
                                                          inner join [dbo].[V_Package_Detail] t2 on t1.OBJECT_NUMBER=t2.PACKAGE_NO
                                                          WHERE t1.BILL_CODE='{0}' ",
                                                          BillCode);
                    //cmd.CommandText = string.Format(@" SELECT * FROM dbo.TEST2 ",
                    //                                      BillCode);

                    pacageSet = _db.ExecuteDataSet(cmd);

                    if (pacageSet.Tables[0] != null && pacageSet.Tables[0].Rows.Count > 0)
                    {
                        pacageQty = Convert.ToInt32(pacageSet.Tables[0].Rows[0][0].ToString());
                    };
                    cmd.CommandText = "";
                    cmd.CommandText = string.Format(@" SELECT SUM(QUANTITY) AS QTY FROM  ERP_WO_REPORT_DETAIL WHERE BILL_CODE='{0}' ",
                                                        BillCode);
                    billSet = _db.ExecuteDataSet(cmd);
                    if (billSet.Tables[0] != null && billSet.Tables[0].Rows.Count > 0)
                    {
                        billQty = Convert.ToInt32(Double.Parse(billSet.Tables[0].Rows[0][0].ToString()));
                    };
                    if (pacageQty == billQty)
                    {
                        result.Data = true;
                    }
                    else
                    {
                        result.Data = false;
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

        //执行入库单明细接收核对
        public MethodReturnResult CheckPackageInWIReportDetail(string packageNo, string billCode, string userName)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                ISession session = null;
                ITransaction transaction = null;
                PagingConfig cfg = new PagingConfig();
                WOReport woReport = null;
                IList<WOReportDetail> lstWOReportDetailOfPackage = null;
                IList<WOReportDetail> lstWOReportDetailOfCheck = null;
                IList<WOReportDetail> lstWOReportDetailAll = null;
                Package package = null;
                MethodReturnResult resultOfRePackage = new MethodReturnResult();
                decimal checkedQtyOfWoReport = 0;                          //入库单内已检验托数量
                decimal fullWoReportQty = 0;                               //入库单内总数量
                
                #region 1.托号合规性检查
                if (packageNo != null && packageNo != "")
                {
                    package = this.PackageDataEngine.Get(packageNo);
                    if (package == null)
                    {
                        result.Code = 2000;
                        result.Message = string.Format("托号{0}不存在！", packageNo);
                        return result;
                    }
                }
                else
                {
                    result.Code = 2000;
                    result.Message = "托号不可为空";
                    return result;
                }
                #endregion

                #region 2.获取托号所在入库单明细及入库单所有明细及入库单/明细合规检查
                //获取托号所在入库单明细
                if (billCode == null || billCode == "")
                {
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" ObjectNumber = '{0}' AND (ERPStockInCode IS NULL OR ERPStockInKey IS NULL)", packageNo),
                        OrderBy = "CreateTime desc"
                    };
                    lstWOReportDetailOfPackage = this.WOReportDetailDataEngine.Get(cfg);
                    if (lstWOReportDetailOfPackage == null || lstWOReportDetailOfPackage.Count <= 0)
                    {
                        result.Code = 2000;
                        result.Message = string.Format(@"托号[{0}]入库单号不存在或已入库接收，不可核对", packageNo);
                        return result;
                    }
                }
                else
                {
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" ObjectNumber = '{0}' AND Key.BillCode = '{1}'", packageNo,billCode),
                        OrderBy = "CreateTime desc"
                    };
                    lstWOReportDetailOfPackage = this.WOReportDetailDataEngine.Get(cfg);
                    if (lstWOReportDetailOfPackage == null || lstWOReportDetailOfPackage.Count <= 0)
                    {
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@" ObjectNumber = '{0}' AND (ERPStockInCode IS NULL OR ERPStockInKey IS NULL)", packageNo),
                            OrderBy = "CreateTime desc"
                        };
                        lstWOReportDetailOfPackage = this.WOReportDetailDataEngine.Get(cfg);
                        if (lstWOReportDetailOfPackage == null || lstWOReportDetailOfPackage.Count <= 0)
                        {
                            result.Code = 2000;
                            result.Message = string.Format(@"托号[{0}]入库单号不存在或已入库接收，不可核对", packageNo);
                            return result;
                        }
                        else
                        {
                            result.Code = 2000;
                            result.Message = string.Format(@"托号[{0}]入库单号为[{1}]，非[{2}]", packageNo, lstWOReportDetailOfPackage[0].Key.BillCode,billCode);
                            return result;
                        }
                    }
                }
                
                //入库单据对象合规性检查
                woReport = this.WOReportDataEngine.Get(lstWOReportDetailOfPackage[0].Key.BillCode);
                if (woReport != null)
                {
                    if (woReport.BillState != EnumBillState.Apply)
                    {
                        result.Code = 2003;
                        result.Message = string.Format("托号[{0}]所在入库单号[{1}]当前单据状态[{2}]非入库申请状态,不可执行托号接收核对！",
                                         packageNo, woReport.Key, woReport.BillState.GetDisplayName());
                        return result;
                    }
                    if (woReport.BillCheckState == EnumBillCheckState.Checked)
                    {
                        result.Code = 2003;
                        result.Message = string.Format("托号[{0}]所在入库单号[{1}]当前单据状态[{2}]及接收核对状态为[{3}],已完成核对,不可执行托号接收核对！",
                                         packageNo, woReport.Key, woReport.BillState.GetDisplayName(), woReport.BillCheckState.GetDisplayName());
                        return result;
                    }

                }
                else
                {
                    result.Code = 2000;
                    result.Message = string.Format(@"入库单号[{0}]不存在", lstWOReportDetailOfPackage[0].Key.BillCode);
                    return result;
                }

                //托号所在入库单明细合规性检验
                foreach (WOReportDetail woReportDetail in lstWOReportDetailOfPackage)
                {
                    if (woReportDetail.PackageCheckState == EnumPackageCheckState.Checked)
                    {
                        //result.Code = 2000;
                        result.Message = string.Format(@"入库单[{0}]中托号[{1}]已核对过，并成功！" ,woReportDetail.Key.BillCode, woReportDetail.ObjectNumber);
                        result.Detail = woReport.Key;
                        result.ObjectNo = Convert.ToInt32(woReport.BillCheckState).ToString();
                        return result;
                    }
                }                
                //获取托号所在入库单内所有托号明细
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@" Key.BillCode = '{0}'", lstWOReportDetailOfPackage[0].Key.BillCode)
                };
                lstWOReportDetailAll = this.WOReportDetailDataEngine.Get(cfg);
                if (lstWOReportDetailAll == null || lstWOReportDetailAll.Count <= 0)
                {
                    result.Code = 2000;
                    result.Message = string.Format(@"托号[{0}]入库单号不存在", packageNo);
                    return result;
                }
                #endregion
                            
                #region 3.入库单明细接收核对
                if (woReport != null)
                {
                    if (woReport.TotalQty > 0)
                    {
                        fullWoReportQty = woReport.TotalQty;
                    }
                    else
                    {
                        fullWoReportQty = lstWOReportDetailAll.Count;
                    }
                    
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.BillCode = '{0}' AND PackageCheckState ={1}", woReport.Key, Convert.ToInt32(EnumPackageCheckState.Checked))
                    };
                    lstWOReportDetailOfCheck = this.WOReportDetailDataEngine.Get(cfg);
                    if (lstWOReportDetailOfCheck != null && lstWOReportDetailOfCheck.Count > 0)
                    {
                        checkedQtyOfWoReport = lstWOReportDetailOfCheck.Count;
                    }
                    foreach (WOReportDetail woReportDetail in lstWOReportDetailOfPackage)
                    {
                        //更改入库单明细中托号接收核对状态为已核对，更改入库单状态为核对中或已核对
                        woReportDetail.PackageCheckState = EnumPackageCheckState.Checked;
                        woReportDetail.Editor = userName;                          //编辑人             
                        woReportDetail.EditTime = DateTime.Now;                    //编辑时间
                    }

                    if (checkedQtyOfWoReport + lstWOReportDetailOfPackage.Count == fullWoReportQty)
                    {
                        //更改入库单接收核对状态为已核对
                        woReport.BillCheckState = EnumBillCheckState.Checked;
                    }
                    else
                    {
                        //更改入库单接收核对状态为核对中
                        woReport.BillCheckState = EnumBillCheckState.Checking;
                    }
                    woReport.Editor = userName;                          //编辑人             
                    woReport.EditTime = DateTime.Now;                    //编辑时间
                }
                #endregion

                #region 4.事务处理
                session = this.WOReportDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                try
                {
                    //入库单数据
                    this.WOReportDataEngine.Update(woReport, session);

                    //入库单明细数据
                    foreach (WOReportDetail woReportDetail in lstWOReportDetailOfPackage)
                    {
                        this.WOReportDetailDataEngine.Update(woReportDetail, session);
                    }                    

                    transaction.Commit();
                    session.Close();
                    result.Message = string.Format(@"入库单[{0}]中托号[{1}]接收核对成功！", woReport.Key, package.Key);
                    result.Detail = woReport.Key;
                    result.ObjectNo = Convert.ToInt32(woReport.BillCheckState).ToString();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();

                    result.Code = 2000;
                    result.Message = string.Format(@"错误：{0}", ex.Message);
                }
                #endregion
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(@"错误：{0}", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        //执行入库单明细取消接收核对
        public MethodReturnResult UnCheckPackageInWIReportDetail(string packageNo, string billCode, string userName)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                ISession session = null;
                ITransaction transaction = null;
                PagingConfig cfg = new PagingConfig();
                WOReport woReport = null;
                //WOReportDetail woReportDetail = null;                      //托号所在入库单明细
                IList<WOReportDetail> lstWOReportDetailOfPackage = null;
                IList<WOReportDetail> lstWOReportDetailOfCheck = null;
                IList<WOReportDetail> lstWOReportDetailAll = null;
                Package package = null;
                MethodReturnResult resultOfRePackage = new MethodReturnResult();
                decimal checkedQtyOfWoReport = 0;                          //入库单内已检验托数量
                decimal fullWoReportQty = 0;                               //入库单内总数量

                #region 1.托号合规性检查
                if (packageNo != null && packageNo != "")
                {
                    package = this.PackageDataEngine.Get(packageNo);
                    if (package == null)
                    {
                        result.Code = 2000;
                        result.Message = string.Format("托号{0}不存在！", packageNo);
                        return result;
                    }
                }
                else
                {
                    result.Code = 2000;
                    result.Message = "托号不可为空";
                    return result;
                }
                #endregion

                #region 2.获取托号所在入库单明细及入库单所有明细
                //获取托号所在入库单明细
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@" ObjectNumber = '{0}' AND Key.BillCode = '{1}'", packageNo, billCode),
                    OrderBy = "CreateTime desc"
                };
                lstWOReportDetailOfPackage = this.WOReportDetailDataEngine.Get(cfg);
                if (lstWOReportDetailOfPackage == null || lstWOReportDetailOfPackage.Count <= 0)
                {
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" ObjectNumber = '{0}' AND (ERPStockInCode IS NULL OR ERPStockInKey IS NULL)", packageNo),
                        OrderBy = "CreateTime desc"
                    };
                    lstWOReportDetailOfPackage = this.WOReportDetailDataEngine.Get(cfg);
                    if (lstWOReportDetailOfPackage == null || lstWOReportDetailOfPackage.Count <= 0)
                    {
                        result.Code = 2000;
                        result.Message = string.Format(@"托号[{0}]入库单号不存在或已入库接收，不可取消核对", packageNo);
                        return result;
                    }
                    else
                    {
                        result.Code = 2000;
                        result.Message = string.Format(@"托号[{0}]入库单号为[{1}]，非[{2}]", packageNo, lstWOReportDetailOfPackage[0].Key.BillCode, billCode);
                        return result;
                    }
                }
                //托号所在入库单明细合规性检验
                foreach (WOReportDetail woReportDetail in lstWOReportDetailOfPackage)
                {
                    if (woReportDetail.PackageCheckState == EnumPackageCheckState.NoCheck)
                    {
                        result.Code = 2000;
                        result.Message = string.Format(@"入库单[{0}]中托号[{1}]还未核对过！", woReportDetail.Key.BillCode, woReportDetail.ObjectNumber);
                        return result;
                    }
                }
                //获取托号所在入库单内所有托号明细
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@" Key.BillCode = '{0}'", lstWOReportDetailOfPackage[0].Key.BillCode)
                };
                lstWOReportDetailAll = this.WOReportDetailDataEngine.Get(cfg);
                if (lstWOReportDetailAll == null || lstWOReportDetailAll.Count <= 0)
                {
                    result.Code = 2000;
                    result.Message = string.Format(@"托号[{0}]入库单号不存在", packageNo);
                    return result;
                }
                #endregion

                #region 3.获取入库单对象
                woReport = this.WOReportDataEngine.Get(billCode);
                if (woReport != null)
                {
                    if (woReport.BillState != EnumBillState.Apply)
                    {
                        result.Code = 2003;
                        result.Message = string.Format("托号[{0}]所在入库单号[{1}]当前单据状态[{2}]非入库申请状态,不可执行托号取消接收核对！",
                                         packageNo, woReport.Key, woReport.BillState.GetDisplayName());
                        return result;
                    }
                    if (woReport.BillCheckState == EnumBillCheckState.NoCheck)
                    {
                        result.Code = 2003;
                        result.Message = string.Format("托号[{0}]所在入库单号[{1}]当前单据状态[{2}]及接收核对状态[{3}],不可执行托号取消接收核对！",
                                         woReport.Key, woReport.BillState.GetDisplayName(),woReport.BillCheckState.GetDisplayName());
                        return result;
                    }
                }
                else
                {
                    result.Code = 2000;
                    result.Message = string.Format(@"入库单号[{0}]不存在", billCode);
                    return result;
                }
                #endregion
                
                #region 4.入库单明细接收取消核对
                if (woReport != null)
                {
                    if (woReport.TotalQty > 0)
                    {
                        fullWoReportQty = woReport.TotalQty;
                    }
                    else
                    {
                        fullWoReportQty = lstWOReportDetailAll.Count;
                    }

                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.BillCode = '{0}' AND PackageCheckState={1}", woReport.Key, Convert.ToInt32(EnumPackageCheckState.Checked))
                    };
                    lstWOReportDetailOfCheck = this.WOReportDetailDataEngine.Get(cfg);
                    if (lstWOReportDetailOfCheck != null && lstWOReportDetailOfCheck.Count > 0)
                    {
                        checkedQtyOfWoReport = lstWOReportDetailOfCheck.Count;
                    }
                    foreach (WOReportDetail woReportDetail in lstWOReportDetailOfPackage)
                    {
                        //更改入库单明细中托号接收核对状态为未核对，更改入库单状态为核对中或未核对
                        woReportDetail.PackageCheckState = EnumPackageCheckState.NoCheck;
                        woReportDetail.Editor = userName;                          //编辑人             
                        woReportDetail.EditTime = DateTime.Now;                    //编辑时间
                    }
                    
                    if (checkedQtyOfWoReport == fullWoReportQty
                        || woReport.BillCheckState == EnumBillCheckState.Checked)
                    {
                        //更改入库单接收核对状态为核对中
                        woReport.BillCheckState = EnumBillCheckState.Checking;
                    }
                    if (checkedQtyOfWoReport == 1)
                    {
                        //更改入库单接收核对状态为未核对
                        woReport.BillCheckState = EnumBillCheckState.NoCheck;
                    }
                    woReport.Editor = userName;                          //编辑人             
                    woReport.EditTime = DateTime.Now;                    //编辑时间
                }
                #endregion

                #region 5.事务处理
                session = this.WOReportDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                try
                {
                    //入库单数据
                    this.WOReportDataEngine.Update(woReport, session);

                    //入库单明细数据
                    foreach (WOReportDetail woReportDetail in lstWOReportDetailOfPackage)
                    {
                        this.WOReportDetailDataEngine.Update(woReportDetail, session);
                    }                    
                    
                    transaction.Commit();
                    session.Close();
                    result.Message = string.Format(@"入库单[{0}]中托号[{1}]取消接收核对成功！",billCode, package.Key);
                    result.ObjectNo = Convert.ToInt32(woReport.BillCheckState).ToString();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();

                    result.Code = 2000;
                    result.Message = string.Format(@"错误：{0}", ex.Message);
                }
                #endregion
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(@"错误：{0}", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        #endregion
    }
}

