using Microsoft.Practices.EnterpriseLibrary.Data;
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.DataAccess.FMM;
using ServiceCenter.MES.DataAccess.ZPVM;
using ServiceCenter.MES.DataAccess.WIP;
using ServiceCenter.MES.DataAccess.BaseData;
using ServiceCenter.MES.DataAccess.PPM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Contract.ERP;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Common;
using NHibernate;
using NHibernate.Cfg;
using System.Web;

namespace ServiceCenter.MES.Service.Class.COMMON
{
    public class PackageInChestCommom: IDisposable
    {
        #region 定义全局变量
        string localName = System.Configuration.ConfigurationSettings.AppSettings["LocalName"];
        #endregion

        #region 定义数据库实例

        protected Database _db;
        protected Database query_db;
        ISessionFactory sessionFactory = null;

        #endregion

        #region 构造函数
        public PackageInChestCommom()
        {
            this.query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
            this._db = DatabaseFactory.CreateDatabase();
            setDataEngine();
        }
       
        #endregion

        #region 定义数据访问对象

        //柜动作日志数据访问对象
        public IChestLogDataEngine ChestLogDataEngine { get; set; }

        //工单规则数据访问对象
        public IWorkOrderRuleDataEngine WorkOrderRuleDataEngine { get; set; }

        //混工单组规则数据访问对象
        public IWorkOrderGroupDetailDataEngine WorkOrderGroupDetailDataEngine { get; set; }

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

        // 工单属性数据访问对象
        public IWorkOrderAttributeDataEngine WorkOrderAttributeDataEngine { get; set; }

        public void setDataEngine()
        {
            if (sessionFactory == null)
            {
                string cfgFileName = HttpContext.Current.Server.MapPath("~/config/module.debug.hibernate.cfg.xml");
                Configuration cfg = new Configuration().Configure(cfgFileName);
                sessionFactory = cfg.BuildSessionFactory();
            }

            MaterialChestParameterDataEngine = new MaterialChestParameterDataEngine(sessionFactory);
            IVTestDataDataEngine = new IVTestDataDataEngine(sessionFactory);
            WorkOrderGradeDataEngine = new WorkOrderGradeDataEngine(sessionFactory);
            LotDataEngine = new LotDataEngine(sessionFactory);
            PackageDataEngine = new PackageDataEngine(sessionFactory);
            ChestDataEngine = new ChestDataEngine(sessionFactory);
            PackageDetailDataEngine = new PackageDetailDataEngine(sessionFactory);
            ChestDetailDataEngine = new ChestDetailDataEngine(sessionFactory);
            RouteOperationAttributeDataEngine = new RouteOperationAttributeDataEngine(sessionFactory);
            BaseAttributeValueDataEngine = new BaseAttributeValueDataEngine(sessionFactory);
            OemDataEngine = new OemDataEngine(sessionFactory);
            WorkOrderPowersetDataEngine = new WorkOrderPowersetDataEngine(sessionFactory);
            WorkOrderDataEngine = new WorkOrderDataEngine(sessionFactory);
            WorkOrderGroupDetailDataEngine = new WorkOrderGroupDetailDataEngine(sessionFactory);
            WorkOrderAttributeDataEngine = new WorkOrderAttributeDataEngine(sessionFactory);
            WorkOrderRuleDataEngine = new WorkOrderRuleDataEngine(sessionFactory);
            ChestLogDataEngine = new ChestLogDataEngine(sessionFactory);
        }
        #endregion

        #region 定义数据访问对象的列表

        List<Chest> lstChestDataForUpdate = new List<Chest>();
        List<Chest> lstChestDataForInsert = new List<Chest>();
        List<ChestDetail> lstChestDetailForInsert = new List<ChestDetail>();
        List<ChestDetail> lstChestDetailForUpdate = new List<ChestDetail>();
        List<ChestDetail> lstChestDetailForDelete = new List<ChestDetail>();

        #endregion

        #region 入柜相关操作
        public MethodReturnResult Chest(ChestParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                string strChestNo = "";
                PagingConfig cfg = null;
                ISession session = null;
                ITransaction transaction = null;
                DateTime now = DateTime.Now;                         //当前时间
                Chest chest = null;
                Package package = null;
                IList<PackageDetail> lstPackageDetail = null;
                ChestDetail chestDetail = null;                     //待入柜托明细
                bool routeControll = false;
                bool isNewChest = false;                            //柜是否为新建
                Package haveAttrPackage = null;                     //带包装属性的将入托号
                Lot lotCurr = null;
                Lot lot = null;
                WorkOrder oemWorkOrder = null;
                OemData oemLot = null;
                MethodReturnResult<MaterialChestParameter> resultOfMaCP = null;
                ChestLog chestLog = null;

                if (p == null)
                {
                    result.Code = 2001;
                    result.Message = "参数为空。";
                    return result;
                }

                #region 0.托号及托号内第一块组件明细合规性检查
                if (p.PackageNo != "" && p.PackageNo != null)
                {
                    cfg = new PagingConfig()
                    {
                        Where = string.Format(@"Key.PackageNo='{0}'", p.PackageNo),
                        OrderBy = "ItemNo ASC"
                    };
                    //取得托对象
                    package = this.PackageDataEngine.Get(p.PackageNo);
                    if (package != null)
                    {
                        if (package.ContainerNo != null && package.ContainerNo != "")
                        {
                            result.Code = 2003;
                            result.Message = string.Format("托号[{0}]已入柜[{1}]！", package.Key, package.ContainerNo);
                            return result;
                        }
                        else
                        {
                            if (package.Quantity <= 0)
                            {
                                result.Code = 2002;
                                result.Message = string.Format("托号[{0}]内数量为0，不可入柜！", package.Key);
                                return result;
                            }
                            haveAttrPackage = GetAttrOfPackage(package).Data;

                            #region 取得托明细对象
                            lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
                            if (lstPackageDetail != null && lstPackageDetail.Count > 0)
                            {
                                oemLot = this.OemDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
                                if (oemLot == null)
                                {
                                    //取得批次信息
                                    lotCurr = this.LotDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
                                    if (lotCurr == null)
                                    {
                                        result.Code = 2003;
                                        result.Message = string.Format("批次[{0}]不存在！", lstPackageDetail[0].Key.ObjectNumber);
                                        return result;
                                    }

                                    lot = lotCurr.Clone() as Lot;
                                }
                                else
                                {
                                    oemWorkOrder = this.WorkOrderDataEngine.Get(oemLot.OrderNumber.ToString().Trim().ToUpper());
                                    if (oemWorkOrder == null)
                                    {
                                        result.Code = 2005;
                                        result.Message = string.Format("批次[{0}]工单[{1}]不存在！",
                                                                        oemLot.Key.ToString(),
                                                                        oemLot.OrderNumber.ToString().Trim().ToUpper());
                                        return result;
                                    }
                                }
                            }
                            else
                            {
                                result.Code = 2003;
                                result.Message = string.Format("托号[{0}]明细不存在！", haveAttrPackage.Key);
                                return result;
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        result.Code = 2003;
                        result.Message = string.Format("托号[{0}]不存在！", p.PackageNo);

                        return result;
                    }
                }
                else
                {
                    result.Code = 2003;
                    result.Message = string.Format("托号不可为空！");

                    return result;
                }
                #endregion

                #region 1.生成新柜和取得柜信息
                if (lot != null || oemLot != null)
                {
                    //获取包装工序是否允许入柜参数值
                    resultOfMaCP = new MethodReturnResult<MaterialChestParameter>();
                    resultOfMaCP.Data = this.MaterialChestParameterDataEngine.Get(haveAttrPackage.MaterialCode);
                    if (resultOfMaCP != null && resultOfMaCP.Data != null)
                    {
                        #region 2.1.1成柜控制参数获取（包装工序是否允许成柜）
                        routeControll = Convert.ToBoolean(resultOfMaCP.Data.IsPackagedChest);
                        #endregion
                    }
                    else
                    {
                        result.Code = 2000;
                        result.Message = string.Format(@"产品编码（{0}）成柜规则不存在，请找成柜规则制定人员进行设置！", haveAttrPackage.MaterialCode);
                        return result;
                    }
                    //包装工序不允许成柜
                    if (!routeControll)
                    {
                        //如果未完成包装不允许入柜
                        if (haveAttrPackage.PackageState != EnumPackageState.Packaged)
                        {
                            routeControll = true;
                        }
                        else
                        {
                            result.Code = 2003;
                            result.Message = string.Format("托号[{0}]当前状态[{1}]，已设置未入库但已完成包装的托号不允许入柜，请知悉！",
                                            p.PackageNo, haveAttrPackage.PackageState.GetDisplayName());
                            return result;
                        }
                    }
                    else
                    {
                        #region 1.1 取得柜信息
                        if (p.ChestNo != "" && p.ChestNo != null)
                        {
                            //取得柜对象                    
                            chest = this.ChestDataEngine.Get(p.ChestNo);
                            if (chest != null)
                            {
                                bool conditiion = false;
                                conditiion = (chest.ChestState == EnumChestState.Packaging || chest.ChestState == EnumChestState.InFabStore);
                                //if (p.isManual)
                                //{
                                //    conditiion = (chest.ChestState == EnumChestState.Packaging || chest.ChestState == EnumChestState.InFabStore);
                                //}
                                //else
                                //{
                                //    conditiion = (chest.ChestState == EnumChestState.Packaging);
                                //}
                                #region 1.1.2 判断柜与将入柜托信息
                                if (conditiion)
                                {
                                    if (!chest.IsLastPackage)
                                    {
                                        chest.IsLastPackage = p.IsLastestPackageInChest;
                                    }

                                    #region 1.1.2.0 处理空柜
                                    if (chest.Quantity == 0)
                                    {
                                        chest.IsLastPackage = p.IsLastestPackageInChest;     //设置尾柜状态
                                        chest.MaterialCode = haveAttrPackage.MaterialCode;   //产品编码
                                        chest.Grade = haveAttrPackage.Grade;                 //等级
                                        chest.Color = haveAttrPackage.Color;                 //花色
                                        chest.PowerName = haveAttrPackage.PowerName;         //功率
                                        chest.PowerSubCode = haveAttrPackage.PowerSubCode;   //电流档
                                        chest.StoreLocation = p.StoreLocation;               //库位
                                        chest.OrderNumber = haveAttrPackage.OrderNumber;     //工单号
                                    }
                                    #endregion

                                    #region 1.1.2.1 判断柜内数量是否超过满柜数量
                                    if (chest.Quantity >= p.ChestFullQty)
                                    {
                                        result.Code = 2003;
                                        result.Message = string.Format("柜号[{0}]已入柜数量[{1}]超过满柜数量[{2}]不存在！", p.ChestNo, chest.Quantity, p.ChestFullQty);
                                        return result;
                                    }
                                    #endregion

                                    //设置柜属性
                                    chest.Quantity += 1;                      //包装数量                    
                                    chest.Editor = p.Editor;                  //编辑人
                                    chest.EditTime = now;                     //编辑日期 
                                }
                                else
                                {
                                    result.Code = 2006;
                                    result.Message = string.Format("柜号[{0}]状态[{1}]非入柜中状态,不可入！", chest.Key, chest.ChestState.GetDisplayName());
                                    return result;
                                }
                                #endregion
                            }
                            else
                            {
                                strChestNo = p.ChestNo;

                                //创建柜对象
                                chest = new Chest()
                                {
                                    Key = strChestNo,                           //主键柜号 
                                    ChestState = EnumChestState.Packaging,      //包装状态                                    
                                    IsLastPackage = p.IsLastestPackageInChest,  //是否尾包
                                    Quantity = 1,                               //包装数量
                                    Description = "",                           //描述
                                    Creator = p.Editor,                         //创建人
                                    CreateTime = now,                           //创建时间                        
                                    Editor = p.Editor,                          //编辑人             
                                    EditTime = now,                             //编辑时间
                                    MaterialCode = haveAttrPackage.MaterialCode,   //产品编码
                                    Grade = haveAttrPackage.Grade,                 //等级
                                    Color = haveAttrPackage.Color,                 //花色
                                    PowerName = haveAttrPackage.PowerName,         //功率
                                    PowerSubCode = haveAttrPackage.PowerSubCode,   //电流档
                                    StoreLocation = p.StoreLocation,               //库位
                                    OrderNumber = haveAttrPackage.OrderNumber      //工单号
                                };
                                isNewChest = true;                            //柜为新建标志
                            }
                            //判断柜状态
                            if (p.ChestFullQty > chest.Quantity)
                            {
                                chest.ChestState = EnumChestState.Packaging;
                            }
                            else
                            {
                                chest.ChestState = EnumChestState.Packaged;
                            }
                            package.ContainerNo = chest.Key;
                            package.Grade = haveAttrPackage.Grade;
                            package.Color = haveAttrPackage.Color;
                            package.PowerSubCode = haveAttrPackage.PowerSubCode;
                            package.PowerName = haveAttrPackage.PowerName;
                        }
                        else
                        {
                            result.Code = 2003;
                            result.Message = string.Format("柜号[{0}]不存在！", p.ChestNo);
                            return result;
                        }
                        #endregion
                    }
                }
                #endregion

                #region 2 创建柜明细对象
                chestDetail = new ChestDetail()
                {
                    Key = new ChestDetailKey()
                    {
                        ChestNo = chest.Key,                         //柜号
                        ObjectNumber = package.Key,                  //托号
                        ObjectType = EnumChestObjectType.PackageNo   //PackageNo 包装号类型
                    },
                    ItemNo = Convert.ToInt32(chest.Quantity),        //入柜项目号（入柜顺序）
                    Creator = p.Editor,                              //创建人
                    CreateTime = now                                 //创建时间
                };
                if (oemLot != null)
                {
                    chestDetail.MaterialCode = oemWorkOrder.MaterialCode;   //物料代码
                }
                else
                {
                    chestDetail.MaterialCode = lot.MaterialCode;            //物料代码
                }
                #endregion

                #region 3 记录入柜日志
                ChestLogKey chestKey = new ChestLogKey()
                {
                    ChestNo = chest.Key,
                    PackageNo = package.Key,
                    ChestActivity = EnumChestActivity.InChest,
                    CreateTime = now
                };
                chestLog = new ChestLog()
                {
                    Key = chestKey,
                    Creator = p.Editor,
                    ModelType = p.ModelType
                };
                #endregion

                #region 4 开始事物处理
                session = this.LotDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                try
                {
                    #region 3.1 柜数据

                    //柜数据
                    if (isNewChest)        //新柜号
                    {
                        this.ChestDataEngine.Insert(chest, session);
                    }
                    else
                    {
                        this.ChestDataEngine.Update(chest, session);
                    }

                    //柜明细数据
                    this.ChestDetailDataEngine.Insert(chestDetail, session);

                    #endregion

                    #region 3.2 托数据

                    this.PackageDataEngine.Update(package, session);

                    #endregion

                    #region 3.3 柜动作日志

                    if (chestLog != null)
                    {
                        this.ChestLogDataEngine.Insert(chestLog, session);
                    }                   

                    #endregion

                    transaction.Commit();
                    session.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();

                    result.Code = 2000;
                    result.Message = string.Format(@"错误：{0}", ex.Message);
                }
                #endregion               

                //返回柜信息
                result.ObjectNo = chest.Key;
                result.Detail = chest.ChestState.GetHashCode().ToString() + "-" + chest.StoreLocation;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(@"错误：{0}", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }     

        public MethodReturnResult<string> GetChestNo(string packageNo, string chestNo, bool isLastChest, bool isManual)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>();
            try
            {
                #region 1.定义变量

                PagingConfig cfg = new PagingConfig();
                IList<Chest> lstChestExist = null;                  //界面柜号符合当前托号入柜条件
                IList<Chest> lstChestNoExist = null;                //界面柜号不符合当前托号入柜条件                
                Package package = null;                             //无属性托号
                Chest chest = null;                                 //界面柜号信息
                MethodReturnResult<Package> packageAttr = null;     //有属性托号
                bool colorLimt = false;                             //入柜花色控制
                bool gradeLimt = false;                             //入柜等级控制
                bool iscLimt = false;                               //入柜电流档控制
                bool powerLimt = false;                             //入柜功率档控制
                bool ordernumberLimt = false;                       //入柜工单控制
                int fullChestQty = -1;                              //柜最大满包数量
                int InChestFullPackageQty = -1;                     //柜最大满包数量
                bool materialCodeLimt = true;                       //入柜产品编码控制（默认都不可混产品编码，尾柜可设置混产品编码）
                bool lastChestLimt = false;                         //尾柜控制筛选条件
                StringBuilder where = new StringBuilder();          //含柜号筛选条件
                //string where1 = "";                               //不含柜号筛选条件
                Package firstInChestPackageAttr = null;             //柜内第一托组件的带属性托信息
                IList<ChestDetail> lstChestDetail = null;           //柜内已入柜明细对象
                IList<PackageDetail> lstPackageDetail = null;       //柜内首托组件明细
                MethodReturnResult resultOfCheck = new MethodReturnResult();
                MethodReturnResult resultOfRePackage = new MethodReturnResult();
                lstChestDataForUpdate = new List<Chest>();
                MethodReturnResult<MaterialChestParameter> resultOfMaCP = null;
                int fullPackageQty = 0;

                #endregion                

                #region 2.托号/柜号合规性检查
                if (packageNo != null && packageNo != "")
                {
                    //取得托对象
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
                        if (package.Quantity <= 0)
                        {
                            result.Code = 2002;
                            result.Message = string.Format("托号[{0}]内数量为0，不可入柜！", package.Key);
                            return result;
                        }
                        //获取将入包装号属性信息及包装状态
                        packageAttr = GetAttrOfPackage(package);
                        if (packageAttr.Code > 0)
                        {
                            result.Code = packageAttr.Code;
                            result.Message = packageAttr.Message;
                            return result;
                        }
                        if (packageAttr.Data.ContainerNo != null && packageAttr.Data.ContainerNo != "")
                        {
                            result.Code = 2002;
                            result.Message = string.Format("托号[{0}]已入柜[{1}]。", package.Key, package.ContainerNo);
                            return result;
                        }
                        else if (packageAttr.Data.PackageState == EnumPackageState.Packaging
                                || packageAttr.Data.PackageState == EnumPackageState.InFabStore
                                || packageAttr.Data.PackageState == EnumPackageState.Shipped
                                || packageAttr.Data.PackageState == EnumPackageState.Checked)
                        {
                            result.Code = 2003;
                            result.Message = string.Format("托号[{0}]当前状态[{1}],不可入柜！", package.Key, package.PackageState.GetDisplayName());
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

                //获取托号满包数量
                WorkOrderRuleKey WorkOrderRuleKey = new WorkOrderRuleKey()
                {
                    OrderNumber = packageAttr.Data.OrderNumber,
                    MaterialCode = packageAttr.Data.MaterialCode
                };
                WorkOrderRule resultWorkOrderRule = this.WorkOrderRuleDataEngine.Get(WorkOrderRuleKey);
                if (resultWorkOrderRule == null)
                {
                    result.Code = 3003;
                    result.Message = string.Format(@"工单[{0}]未设置满包数量！", packageAttr.Data.OrderNumber);

                    return result;
                }
                fullPackageQty = Convert.ToInt32(resultWorkOrderRule.FullPackageQty);

                //获取柜信息
                if (chestNo != null && chestNo != "")
                {
                    //取得柜对象
                    chest = this.ChestDataEngine.Get(chestNo);
                }
                #endregion

                #region 3.设置是否尾柜
                if (chest != null)
                {
                    lastChestLimt = chest.IsLastPackage;
                }
                #endregion

                #region 4.获取入柜控制参数
                resultOfMaCP = new MethodReturnResult<MaterialChestParameter>();
                resultOfMaCP.Data = this.MaterialChestParameterDataEngine.Get(packageAttr.Data.MaterialCode);
                if (resultOfMaCP != null && resultOfMaCP.Data != null)
                {
                    if (!lastChestLimt && !isLastChest)
                    {
                        #region 4.1成柜控制参数获取（电流/等级/功率/颜色/工单）
                        colorLimt = Convert.ToBoolean(resultOfMaCP.Data.ColorLimit);
                        gradeLimt = Convert.ToBoolean(resultOfMaCP.Data.GradeLimit);
                        powerLimt = Convert.ToBoolean(resultOfMaCP.Data.PowerLimit);
                        iscLimt = Convert.ToBoolean(resultOfMaCP.Data.IscLimit);
                        ordernumberLimt = Convert.ToBoolean(resultOfMaCP.Data.OrderNumberLimit);                       
                        #endregion
                    }
                    else
                    {
                        //现设置尾柜是否卡控产品编码
                        materialCodeLimt = Convert.ToBoolean(resultOfMaCP.Data.LastChestMaterialLimit);
                    }
                    //柜最大满包数量及满柜数量
                    InChestFullPackageQty = Convert.ToInt32(resultOfMaCP.Data.InChestFullPackageQty);
                    fullChestQty = Convert.ToInt32(resultOfMaCP.Data.FullChestQty);
                }
                else
                {
                    result.Code = 2000;
                    result.Message = string.Format(@"产品编码（{0}）成柜规则不存在，请找工艺人员进行设置！", packageAttr.Data.MaterialCode);
                    return result;
                }
                #endregion

                #region 5.设置筛选条件并设置柜号
                where.AppendFormat(" {0} (ChestState = {1} OR ChestState = {2})"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , Convert.ToInt32(EnumChestState.Packaging)
                                        , Convert.ToInt32(EnumChestState.InFabStore));
                if (colorLimt)
                {
                    where.AppendFormat(" {0} Color = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , packageAttr.Data.Color);
                }
                if (gradeLimt)
                {
                    where.AppendFormat(" {0} Grade = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , packageAttr.Data.Grade);
                }
                if (iscLimt)
                {
                    where.AppendFormat(" {0} PowerSubCode = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , packageAttr.Data.PowerSubCode);
                }
                if (powerLimt)
                {
                    where.AppendFormat(" {0} PowerName = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , packageAttr.Data.PowerName);
                }

                if (ordernumberLimt)
                {
                    where.AppendFormat(" {0} OrderNumber = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , packageAttr.Data.OrderNumber);
                }
                else
                {
                    //获取该工单是否设置允许混工单
                    WorkOrderAttributeKey workAttrKey = new WorkOrderAttributeKey()
                    {
                        OrderNumber = packageAttr.Data.OrderNumber,
                        AttributeName = "PackageLimited"
                    };
                    WorkOrderAttribute workAttr = WorkOrderAttributeDataEngine.Get(workAttrKey);
                    
                    //设置允许混工单
                    if (workAttr == null || workAttr.AttributeValue.ToUpper() != "TRUE" )
                    {
                        #region 混工单组校验
                        cfg = new PagingConfig()
                        {
                            Where = string.Format(@"Key.OrderNumber = '{0}'", packageAttr.Data.OrderNumber)
                        };
                        IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);

                        #region 1.查找与该工单在同一工单组内的工单且工单未设置不可混包属性
                        //待入柜托工单设置了混工单组
                        if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                        {
                            //获取混工单组组内所有工单列表
                            cfg = new PagingConfig()
                            {
                                Where = string.Format(@"Key.WorkOrderGroupNo = '{0}'", lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo)
                            };
                            lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);

                            List<string> lstOrderNumber = new List<string>();     //混工单组组内所有工单列表
                            List<string> lstOrderNumberLast = new List<string>(); //最终满足条件工单列表
                            foreach (WorkOrderGroupDetail item in lstPackageWorkOrderGroupDetail)
                            {
                                lstOrderNumber.Add(item.Key.OrderNumber);
                            }
                            foreach (string orderNum in lstOrderNumber)
                            {
                                //判断工单组内其他工单是否允许混工单
                                WorkOrderAttributeKey workAttrKeyItem = new WorkOrderAttributeKey()
                                {
                                    OrderNumber = orderNum,
                                    AttributeName = "PackageLimited"
                                };
                                WorkOrderAttribute workAttrItem = WorkOrderAttributeDataEngine.Get(workAttrKeyItem);
                                //混工单组组内所有工单列表设置允许混工单则添加到最终满足条件工单列表
                                if (workAttrItem == null || workAttrItem.AttributeValue.ToUpper() != "TRUE")
                                {
                                    lstOrderNumberLast.Add(orderNum);
                                }
                            }
                            where.AppendFormat(" {0} ( OrderNumber = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , lstOrderNumberLast[0]);
                            //循环设置工单筛选条件
                            for (int i = 1; i < lstOrderNumberLast.Count; i++)
                            {
                                where.AppendFormat(" OR OrderNumber = '{0}' ", lstOrderNumberLast[i]);
                            }
                            where.AppendFormat(" ) ");
                        }
                        #endregion

                        #region 2.若未设置混工单组，查找与该工单料号一致的工单且未设置混工单组且工单未设置不可混包属性
                        //待入柜托工单没设置混工单组
                        else
                        {
                            //获取该料号下所有工单
                            cfg = new PagingConfig()
                            {
                                Where = string.Format(@"Key.MaterialCode = '{0}'", packageAttr.Data.MaterialCode)
                            };
                            IList<WorkOrder> lstWorkOrder = WorkOrderDataEngine.Get(cfg);

                            //获取该料号设置混工单组的工单列表
                            cfg = new PagingConfig()
                            {
                                Where = string.Format(@"Key.ProductCode = '{0}'", packageAttr.Data.MaterialCode)
                            };
                            lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);

                            //获取该料号同样没设置混工单组的工单列表
                            List<string> lstOrderNumber = new List<string>();
                            string lstWorkGroup = "+";                           
                            foreach (WorkOrderGroupDetail item in lstPackageWorkOrderGroupDetail)
                            {
                                lstWorkGroup += item.Key.OrderNumber + "+";
                            }
                            foreach (WorkOrder workOrder in lstWorkOrder)
                            {
                                if (!lstWorkGroup.Contains("+" + workOrder.Key + "+"))
                                {
                                    lstOrderNumber.Add(workOrder.Key);
                                }
                            }

                            //获取同样没设置混工单组的工单列表中未设置不可混包属性的工单列表
                            List<string> lstOrderNumberLast = new List<string>();
                            foreach (string orderNum in lstOrderNumber)
                            {
                                //判断该料号同样没设置混工单组的工单列表内工单是否允许混工单
                                WorkOrderAttributeKey workAttrKeyItem = new WorkOrderAttributeKey()
                                {
                                    OrderNumber = orderNum,
                                    AttributeName = "PackageLimited"
                                };
                                WorkOrderAttribute workAttrItem = WorkOrderAttributeDataEngine.Get(workAttrKeyItem);

                                if (workAttrItem == null || workAttrItem.AttributeValue.ToUpper() != "TRUE")
                                {
                                    lstOrderNumberLast.Add(orderNum);
                                }
                            }

                            where.AppendFormat(" {0} ( OrderNumber = '{1}'"
                                            , where.Length > 0 ? "AND" : string.Empty
                                            , lstOrderNumberLast[0]);
                            //循环设置工单筛选条件
                            for (int i = 1; i < lstOrderNumberLast.Count; i++)
                            {
                                where.AppendFormat(" OR OrderNumber = '{0}' ", lstOrderNumberLast[i]);
                            }
                            where.AppendFormat(" ) ");
                        }
                        #endregion

                        #endregion
                    }
                    //不允许混工单
                    else
                    {
                        where.AppendFormat(" {0} OrderNumber = '{1}'"
                                    , where.Length > 0 ? "AND" : string.Empty
                                    , packageAttr.Data.OrderNumber);
                    }                    
                }

                if (materialCodeLimt)
                {
                    where.AppendFormat(" {0} MaterialCode = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , package.MaterialCode);
                }

                if (isLastChest)
                {
                    where.AppendFormat(" {0} IsLastPackage = 'TRUE'"
                                        , where.Length > 0 ? "AND" : string.Empty);
                }

                //where1 = where.ToString();
                //界面录入柜号是有效柜号时
                if (chest != null)
                {
                    if (isLastChest || lastChestLimt)
                    {
                        //尾柜是否产品编码控制
                        if (materialCodeLimt)
                        {
                            if (packageAttr.Data.MaterialCode != chest.MaterialCode)
                            {
                                result.Code = 2000;
                                result.Message = string.Format(@"已设置尾柜料号卡控,柜号{0}料号{1}与托号{2}料号{3}不一致！",
                                                chest.Key, chest.MaterialCode, packageAttr.Data.Key, packageAttr.Data.MaterialCode);
                                return result;
                            }
                        }
                        result.Data = chest.Key;
                        return result;
                    }
                    //where.AppendFormat(" {0} Key = '{1}'"
                    //                    , where.Length > 0 ? "AND" : string.Empty
                    //                    , chest.Key);
                }
                else
                {
                    //if (isLastChest)
                    //{
                    //    where.AppendFormat(" {0} IsLastPackage = 'TRUE'"
                    //                        , where.Length > 0 ? "AND" : string.Empty);
                    //}
                }
                #endregion

                #region 6.获得柜号

                #region 两种模式

                #region 1.手动入柜模式--检验界面当前柜号是否符合托号入柜条件
                if (isManual)
                {
                    if (chest != null)
                    {
                        //非尾柜
                        if (!lastChestLimt && !isLastChest)
                        {
                            #region 检验柜号是否符合将入托号入柜条件
                            //1.获取柜首托带属性对象
                            cfg = new PagingConfig()
                            {
                                Where = string.Format(@"Key.ChestNo='{0}'", chest.Key),
                                OrderBy = "ItemNo ASC"
                            };
                            lstChestDetail = this.ChestDetailDataEngine.Get(cfg);
                            if (lstChestDetail != null && lstChestDetail.Count > 0)
                            {
                                if (lstChestDetail[0].Key.ObjectNumber != "" && lstChestDetail[0].Key.ObjectNumber != null)
                                {
                                    cfg = new PagingConfig()
                                    {
                                        Where = string.Format(@"Key.PackageNo='{0}'", lstChestDetail[0].Key.ObjectNumber),
                                        OrderBy = "ItemNo ASC"
                                    };
                                    //取得柜内首托对象
                                    Package firstPackage = this.PackageDataEngine.Get(lstChestDetail[0].Key.ObjectNumber);
                                    //取得首托明细对象
                                    lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
                                    if (lstPackageDetail != null && lstPackageDetail.Count > 0)
                                    {
                                        //取得柜内首托带属性对象
                                        firstInChestPackageAttr = GetAttrOfPackage(firstPackage).Data;
                                        resultOfCheck = CheckLotInPackage(packageAttr.Data, firstInChestPackageAttr);

                                        if (resultOfCheck.Code > 0)
                                        {
                                            result.Code = resultOfCheck.Code;
                                            result.Message = resultOfCheck.Message;
                                            return result;
                                        }
                                        else
                                        {
                                            result.Data = chest.Key;
                                        }
                                    }
                                    else
                                    {
                                        result.Code = 2003;
                                        result.Message = string.Format("托[{0}]明细不存在！", lstChestDetail[0].Key.ObjectNumber);
                                        return result;
                                    }
                                }
                                else
                                {
                                    result.Code = 2009;
                                    result.Message = string.Format("柜[{0}]明细不存在！", chest.Key);
                                    return result;
                                }
                            }

                            #endregion
                        }
                        //尾柜
                        if (materialCodeLimt)
                        {
                            if (packageAttr.Data.MaterialCode != chest.MaterialCode)
                            {
                                result.Code = 2000;
                                result.Message = string.Format(@"托号{0}产品编码{1}与柜号{2}产品编码{3}不一致,
                                                 请确认是否选择手动入柜模式,若确认手动,请清空界面录入柜号或输入有效柜号！"
                                                 , packageAttr.Data.Key
                                                 , packageAttr.Data.MaterialCode
                                                 , chest.Key
                                                 , chest.MaterialCode);
                                return result;
                            }
                        }
                        result.Data = chest.Key;
                    }
                    else
                    {
                        if (chestNo != null && chestNo != "")
                        {
                            result.Code = 2000;
                            result.Message = string.Format(@"柜号{0}不存在,请确认是否选择手动入柜模式,若确认手动,请清空界面录入柜号或输入有效柜号！", chestNo);
                            return result;
                        }
                        else
                        {
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                OrderBy = "CreateTime ASC",
                                Where = where.ToString()
                            };
                            lstChestExist = this.ChestDataEngine.Get(cfg);
                            //如果找到符合条件的柜号则返回柜号值，如果没找到生成新柜号
                            if (lstChestExist != null && lstChestExist.Count > 0)
                            {
                                //非尾柜
                                if (!lastChestLimt && !isLastChest)
                                {
                                    #region 校验柜最大满包数量
                                    //已设置柜最大满包数量
                                    if (InChestFullPackageQty != -1)
                                    {
                                        //托号为满包
                                        if (packageAttr.Data.Quantity == fullPackageQty)
                                        {
                                            PagingConfig cfgChest = new PagingConfig();
                                            IList<ChestDetail> lstChestDetailFullPack = null;
                                            foreach (Chest chestEx in lstChestExist)
                                            {
                                                //获取柜号信息
                                                cfgChest = new PagingConfig()
                                                {
                                                    IsPaging = false,
                                                    Where = string.Format(@" Key.ChestNo = '{0}'    
                                                 AND EXISTS(FROM Package as package
                                                 WHERE package.Key=self.Key.ObjectNumber
                                                 AND package.Quantity = '{1}')", chestEx.Key, fullPackageQty)
                                                };
                                                lstChestDetailFullPack = this.ChestDetailDataEngine.Get(cfgChest);
                                                if (lstChestDetailFullPack != null)
                                                {
                                                    if (InChestFullPackageQty != 0)
                                                    {
                                                        if (lstChestDetailFullPack.Count < InChestFullPackageQty)
                                                        {
                                                            result.Data = chestEx.Key;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            result.Data = "";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        result.Data = chestEx.Key;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        //托号非满包
                                        else
                                        {
                                            PagingConfig cfgChest = new PagingConfig();
                                            IList<ChestDetail> lstChestDetailFullPack = null;
                                            foreach (Chest chestEx in lstChestExist)
                                            {
                                                //获取柜号信息
                                                cfgChest = new PagingConfig()
                                                {
                                                    Where = string.Format(@" Key.ChestNo = '{0}'    
                                                 AND EXISTS(FROM Package as package
                                                 WHERE package.Key=self.Key.ObjectNumber
                                                 AND package.Quantity <> '{1}')", chestEx.Key, fullPackageQty)
                                                };
                                                lstChestDetailFullPack = this.ChestDetailDataEngine.Get(cfgChest);
                                                if (lstChestDetailFullPack != null)
                                                {
                                                    if (InChestFullPackageQty != 0)
                                                    {
                                                        if (lstChestDetailFullPack.Count < (fullChestQty - InChestFullPackageQty))
                                                        {
                                                            result.Data = chestEx.Key;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            result.Data = "";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        result.Data = chestEx.Key;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        //如果没有满足柜条件得，创建新柜号
                                        if (result.Data == "")
                                        {
                                            result = CreateChestNo(package.Key);
                                        }
                                    }
                                    //未设置柜最大满柜数量
                                    else
                                    {
                                        result.Data = lstChestExist[0].Key;
                                    }
                                    #endregion
                                }
                                //尾柜
                                else
                                {
                                    result.Data = lstChestExist[0].Key;
                                }                               
                            }
                            else
                            {
                                result = CreateChestNo(package.Key);
                            }
                        }
                    }
                }
                #endregion

                #region 2.自动入柜模式--找到符合条件的柜号 OR 无满足条件柜号则生成新柜号
                else
                {
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "CreateTime ASC",
                        Where = where.ToString()
                    };
                    lstChestExist = this.ChestDataEngine.Get(cfg);
                    ////如果界面柜号符合入柜条件 OR 界面柜号为空但
                    //找到符合条件柜号
                    if (lstChestExist != null && lstChestExist.Count > 0)
                    {
                        //非尾柜
                        if (!lastChestLimt && !isLastChest)
                        {
                            #region 校验柜最大满包数量
                            //已设置柜最大满包数量
                            if (InChestFullPackageQty != -1)
                            {
                                //托号为满包
                                if (packageAttr.Data.Quantity == fullPackageQty)
                                {
                                    PagingConfig cfgChest = new PagingConfig();
                                    IList<ChestDetail> lstChestDetailFullPack = null;
                                    foreach (Chest chestEx in lstChestExist)
                                    {
                                        //获取柜号信息
                                        cfgChest = new PagingConfig()
                                        {
                                            IsPaging = false,
                                            Where = string.Format(@" Key.ChestNo = '{0}'    
                                                 AND EXISTS(FROM Package as package
                                                 WHERE package.Key=self.Key.ObjectNumber
                                                 AND package.Quantity = '{1}')", chestEx.Key, fullPackageQty)
                                        };
                                        lstChestDetailFullPack = this.ChestDetailDataEngine.Get(cfgChest);
                                        if (lstChestDetailFullPack != null)
                                        {
                                            if (InChestFullPackageQty != 0)
                                            {
                                                if (lstChestDetailFullPack.Count < InChestFullPackageQty)
                                                {
                                                    result.Data = chestEx.Key;
                                                    break;
                                                }
                                                else
                                                {
                                                    result.Data = "";
                                                }
                                            }
                                            else
                                            {
                                                result.Data = chestEx.Key;
                                                break;
                                            }
                                        }
                                    }
                                }
                                //托号非满包
                                else
                                {
                                    PagingConfig cfgChest = new PagingConfig();
                                    IList<ChestDetail> lstChestDetailFullPack = null;
                                    foreach (Chest chestEx in lstChestExist)
                                    {
                                        //获取柜号信息
                                        cfgChest = new PagingConfig()
                                        {
                                            IsPaging = false,
                                            Where = string.Format(@" Key.ChestNo = '{0}'    
                                                 AND EXISTS(FROM Package as package
                                                 WHERE package.Key=self.Key.ObjectNumber
                                                 AND package.Quantity <> '{1}')", chestEx.Key, fullPackageQty)
                                        };
                                        lstChestDetailFullPack = this.ChestDetailDataEngine.Get(cfgChest);
                                        if (lstChestDetailFullPack != null)
                                        {
                                            if (InChestFullPackageQty != 0)
                                            {
                                                if (lstChestDetailFullPack.Count < (fullChestQty - InChestFullPackageQty))
                                                {
                                                    result.Data = chestEx.Key;
                                                    break;
                                                }
                                                else
                                                {
                                                    result.Data = "";
                                                }
                                            }
                                            else
                                            {
                                                result.Data = chestEx.Key;
                                                break;
                                            }
                                        }
                                    }
                                }

                                //如果没有满足柜条件得，创建新柜号
                                if (result.Data == "")
                                {
                                    result = CreateChestNo(package.Key);
                                }
                            }
                            //未设置柜最大满柜数量
                            else
                            {
                                result.Data = lstChestExist[0].Key;
                            }
                            #endregion                     
                        }
                        //尾柜
                        else
                        {
                            result.Data = lstChestExist[0].Key;
                        }                        
                    }
                    //如果界面柜号不符合入柜条件 OR 界面柜号为空且
                    //没找到符合条件得柜号
                    else
                    {
                        result = CreateChestNo(package.Key);

                        #region 注释
                        //                        cfg = new PagingConfig()
//                        {
//                            IsPaging = false,
//                            OrderBy = "CreateTime ASC",
//                            Where = where.ToString()
//                        };
//                        lstChestNoExist = this.ChestDataEngine.Get(cfg);

//                        //如果找到界面柜号为空符合条件的柜号则返回柜号值，如果没找到生成新柜号
//                        if (lstChestNoExist != null && lstChestNoExist.Count > 0)
//                        {
//                            //非尾柜
//                            if (!lastChestLimt && !isLastChest)
//                            {
//                                #region 校验柜最大满包数量
//                                //已设置柜最大满包数量
//                                if (InChestFullPackageQty != -1)
//                                {
//                                    //托号为满包
//                                    if (packageAttr.Data.Quantity == fullPackageQty)
//                                    {
//                                        PagingConfig cfgChest = new PagingConfig();
//                                        IList<ChestDetail> lstChestDetailFullPack = null;
//                                        foreach (Chest chestEx in lstChestNoExist)
//                                        {
//                                            //获取柜号信息
//                                            cfgChest = new PagingConfig()
//                                            {
//                                                IsPaging = false,
//                                                Where = string.Format(@" Key.ChestNo = '{0}'    
//                                                 AND EXISTS(FROM Package as package
//                                                 WHERE package.Key=self.Key.ObjectNumber
//                                                 AND package.Quantity = '{1}')", chestEx.Key, fullPackageQty)
//                                            };
//                                            lstChestDetailFullPack = this.ChestDetailDataEngine.Get(cfgChest);
//                                            if (lstChestDetailFullPack != null)
//                                            {
//                                                if (InChestFullPackageQty != 0)
//                                                {
//                                                    if (lstChestDetailFullPack.Count < InChestFullPackageQty)
//                                                    {
//                                                        result.Data = chestEx.Key;
//                                                        break;
//                                                    }
//                                                    else
//                                                    {
//                                                        result.Data = "";
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    result.Data = chestEx.Key;
//                                                    break;
//                                                }                                                
//                                            }
//                                        }
//                                    }
//                                    //托号非满包
//                                    else
//                                    {
//                                        PagingConfig cfgChest = new PagingConfig();
//                                        IList<ChestDetail> lstChestDetailFullPack = null;
//                                        foreach (Chest chestEx in lstChestNoExist)
//                                        {
//                                            //获取柜号信息
//                                            cfgChest = new PagingConfig()
//                                            {
//                                                IsPaging = false,
//                                                Where = string.Format(@" Key.ChestNo = '{0}'    
//                                                 AND EXISTS(FROM Package as package
//                                                 WHERE package.Key=self.Key.ObjectNumber
//                                                 AND package.Quantity <> '{1}')", chestEx.Key, fullPackageQty)
//                                            };
//                                            lstChestDetailFullPack = this.ChestDetailDataEngine.Get(cfgChest);
//                                            if (lstChestDetailFullPack != null)
//                                            {
//                                                if (InChestFullPackageQty != 0)
//                                                {
//                                                    if (lstChestDetailFullPack.Count < (fullChestQty - InChestFullPackageQty))
//                                                    {
//                                                        result.Data = chestEx.Key;
//                                                        break;
//                                                    }
//                                                    else
//                                                    {
//                                                        result.Data = "";
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    result.Data = chestEx.Key;
//                                                    break;
//                                                }
//                                            }
//                                        }
//                                    }

//                                    //如果没有满足柜条件得，创建新柜号
//                                    if (result.Data == "")
//                                    {
//                                        result = CreateChestNo(package.Key);
//                                    }
//                                }
//                                //未设置柜最大满柜数量
//                                else
//                                {
//                                    result.Data = lstChestNoExist[0].Key;
//                                }
//                                #endregion
//                            }
//                            //尾柜
//                            else
//                            {
//                                result.Data = lstChestNoExist[0].Key;
//                            }                            
//                        }
//                        else
//                        {
//                            result = CreateChestNo(package.Key);
                        //                        }
                        #endregion
                    }
                }
                #endregion

                #endregion

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

        public MethodReturnResult GetREbackdata(REbackdataParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(p.PackageNo))
                {
                    using (DbConnection con = this._db.CreateConnection())
                    {
                        DbCommand cmd = con.CreateCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sp_BK_ReBackData";
                        this._db.AddInParameter(cmd, "PackageNo", DbType.String, p.PackageNo);
                        this._db.AddInParameter(cmd, "ReType", DbType.Int32, p.ReType);
                        this._db.AddInParameter(cmd, "IsDelete", DbType.Int32, p.IsDelete);
                        cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                        cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                        SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                        parReturn.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(parReturn);
                        this._db.ExecuteNonQuery(cmd);
                        int i = (int)cmd.Parameters["@return"].Value;

                        if (i == -1)
                        {
                            strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                            result.Code = 1000;
                            result.Message = strErrorMessage;
                            result.Detail = strErrorMessage;
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
            return result;
        }

        public MethodReturnResult<string> CreateChestNo(string packageNo)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>();
            DateTime now = DateTime.Now;
            string minChestNo = "";
            string maxChestNo = "";
            string prefixChestNo = "";
            int seqNo = 1;
            string year = Convert.ToInt32(now.ToString("yy")).ToString("00");
            string month = now.Month.ToString("00");
            PagingConfig cfg = new PagingConfig();
            try
            {
                #region 柜号生成
                if (packageNo != null && packageNo != "")
                {
                    packageNo = packageNo.ToUpper();
                    if (packageNo.Substring(0, 3) == "05M" || packageNo.Substring(0, 2) == "05P")
                    {
                        #region 协鑫永能P660柜号生成规则
                        prefixChestNo = string.Format("0000");
                        if (localName == "G01")
                        {
                            seqNo = 840;
                            //晋中柜号流水码限制
                            minChestNo = string.Format("{0}0840", prefixChestNo);
                            maxChestNo = string.Format("{0}0870", prefixChestNo);
                        }
                        if (localName == "K01")
                        {
                            seqNo = 871;
                            //文水柜号流水码限制
                            minChestNo = string.Format("{0}0871", prefixChestNo);
                            maxChestNo = string.Format("{0}0910", prefixChestNo);
                        }

                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format(@"Key >= '{0}' AND Key < '{1}'"
                                                                , minChestNo
                                                                , maxChestNo),
                            OrderBy = "Key DESC"
                        };

                        IList<Chest> lstChest = this.ChestDataEngine.Get(cfg);

                        if (lstChest.Count > 0)
                        {
                            string maxSeqNo = lstChest[0].Key.Replace(prefixChestNo, "");
                            if (int.TryParse(maxSeqNo, out seqNo))
                            {
                                seqNo = seqNo + 1;
                            }
                        }
                        result.Data = string.Format("{0}{1}", prefixChestNo, seqNo.ToString("0000"));
                        #endregion
                    }
                    else if (packageNo.Substring(0, 3) == "27M" || packageNo.Substring(0, 3) == "27P"
                          || packageNo.Substring(0, 3) == "64M" || packageNo.Substring(0, 3) == "64P")
                    {
                        #region 协鑫晋能/张家港M672柜号生成规则
                        prefixChestNo = string.Format("{0}{1}", year, month);
                        if (localName == "G01")
                        {
                            //晋中柜号流水码限制
                            if (packageNo.Substring(0, 3) == "27M" || packageNo.Substring(0, 3) == "27P")
                            {
                                seqNo = 1;
                                minChestNo = string.Format("{0}0001", prefixChestNo);
                                maxChestNo = string.Format("{0}4000", prefixChestNo);
                            }
                            else
                            {
                                seqNo = 4001;
                                minChestNo = string.Format("{0}4001", prefixChestNo);
                                maxChestNo = string.Format("{0}5000", prefixChestNo);
                            }
                        }
                        if (localName == "K01")
                        {
                            seqNo = 5001;
                            //文水柜号流水码限制
                            minChestNo = string.Format("{0}5001", prefixChestNo);
                            maxChestNo = string.Format("{0}9999", prefixChestNo);
                        }

                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format(@"Key >= '{0}' AND Key <= '{1}' AND MaterialCode <> '2512020201' "
                                                                , minChestNo
                                                                , maxChestNo),
                            OrderBy = "Key DESC"
                        };

                        IList<Chest> lstChest = this.ChestDataEngine.Get(cfg);

                        if (lstChest.Count > 0)
                        {
                            string maxSeqNo = lstChest[0].Key.Replace(prefixChestNo, "");
                            if (int.TryParse(maxSeqNo, out seqNo))
                            {
                                seqNo = seqNo + 1;
                            }
                        }
                        result.Data = string.Format("{0}{1}", prefixChestNo, seqNo.ToString("0000"));
                        #endregion
                    }
                    else
                    {
                        #region 晋能柜号生成规则
                        if (localName == "K01")
                        {
                            prefixChestNo = string.Format("1G{0}{1}", year, month);
                        }
                        else if (localName == "G01")
                        {
                            prefixChestNo = string.Format("2G{0}{1}", year, month);
                        }
                        seqNo = 1;
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format(@"Key LIKE '{0}%'"
                                                , prefixChestNo),
                            OrderBy = "Key DESC"
                        };
                        IList<Chest> lstChest = this.ChestDataEngine.Get(cfg);
                        if (lstChest.Count > 0)
                        {
                            string maxSeqNo = lstChest[0].Key.Replace(prefixChestNo, "");
                            if (int.TryParse(maxSeqNo, out seqNo))
                            {
                                seqNo = seqNo + 1;
                            }
                        }
                        result.Data = string.Format("{0}{1}", prefixChestNo, seqNo.ToString("0000"));

                        #endregion
                    }
                }
                else
                {
                    result.Code = 2000;
                    result.Message = "托号不可为空";
                }
                #endregion
            }
            catch (Exception ex)
            {
                result.Code = 2000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;
        }

        public MethodReturnResult<Package> GetAttrOfPackage(Package package)
        {
            MethodReturnResult<Package> result = new MethodReturnResult<Package>();
            try
            {
                IList<PackageDetail> lstPackageDetail = null;
                IList<IVTestData> lstLotIVTestData = null;                  //自制组件IV测试数据
                IList<WorkOrderPowerset> lstWorkOrderPowerset = null;       //自制组件工单分档规则
                PagingConfig cfg = new PagingConfig();
                OemData packageFirstLot = null;
                Lot lot = null;
                MethodReturnResult<DataSet> erpOutData = null;              //ERP中托号出货记录

                #region 1.获取包装号属性信息
                //如果包装属性值不为空
                if (package.PowerName != null && package.PowerName != "")
                {
                    package.PowerName = package.PowerName;
                    package.PowerSubCode = package.PowerSubCode;
                    package.Grade = package.Grade;
                    package.Color = package.Color;
                }
                //如果包装属性值为空，获取包装托内第一块组件的信息
                else
                {
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.PackageNo='{0}'", package.Key),
                        OrderBy = "ItemNo ASC"
                    };
                    //取得托明细对象
                    lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
                    if (lstPackageDetail != null && lstPackageDetail.Count > 0)
                    {
                        packageFirstLot = this.OemDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
                        if (packageFirstLot != null)
                        {
                            package.PowerName = packageFirstLot.PnName;
                            package.PowerSubCode = packageFirstLot.PsSubCode;
                            package.Grade = packageFirstLot.Grade;
                            package.Color = packageFirstLot.Color;
                        }
                        else
                        {
                            //取得第一块批次信息
                            lot = this.LotDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
                            if (lot == null)
                            {
                                result.Code = 2003;
                                result.Message = string.Format("批次[{0}]不存在！", lstPackageDetail[0].Key.ObjectNumber);
                                return result;
                            }
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lot.Key)
                            };
                            lstLotIVTestData = this.IVTestDataDataEngine.Get(cfg);
                            if (lstLotIVTestData != null && lstLotIVTestData.Count > 0)
                            {
                                #region 取得批次工单分档规则
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format("Key.OrderNumber = '{0}' AND Key.Code='{1}' AND Key.ItemNo = '{2}'", lot.OrderNumber, lstLotIVTestData[0].PowersetCode, lstLotIVTestData[0].PowersetItemNo)
                                };
                                lstWorkOrderPowerset = this.WorkOrderPowersetDataEngine.Get(cfg);
                                if (lstWorkOrderPowerset == null || lstWorkOrderPowerset.Count <= 0)
                                {
                                    result.Code = 3001;
                                    result.Message = string.Format("批次[{0}]所在工单[{1}]分档规则[{3}-{4}]不存在！",
                                        lot.Key, lot.OrderNumber, lstLotIVTestData[0].PowersetCode, lstLotIVTestData[0].PowersetItemNo);
                                    return result;
                                }
                                #endregion
                            }
                            else
                            {
                                result.Code = 3001;
                                result.Message = string.Format("提取批次[{0}]测试数据失败！", lot.Key);
                                return result;
                            }
                            package.PowerName = lstWorkOrderPowerset[0].PowerName;
                            package.PowerSubCode = lstLotIVTestData[0].PowersetSubCode;
                            package.Grade = lot.Grade;
                            package.Color = lot.Color;
                        }
                    }
                    else
                    {
                        result.Code = 2003;
                        result.Message = string.Format("托号[{0}]明细不存在！", package.Key);
                        return result;
                    }
                }
                #endregion

                #region 2.获取包装号状态信息
                //获取报表服务器物料出货表数据
                ChestParameter chestParameter = new ChestParameter()
                {
                    PackageNo = package.Key,
                    PageNo = 0,
                    PageSize = 20
                };
                erpOutData = GetErpOutOfPackage(ref chestParameter);
                if (erpOutData.Data != null && erpOutData.Data.Tables[0].Rows.Count > 0)
                {
                    //查到出货记录设置托号状态为已出货
                    package.PackageState = EnumPackageState.Shipped;
                }
                #endregion

                result.Data = package;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(@"错误：{0}", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        public MethodReturnResult<DataSet> GetErpOutOfPackage(ref ChestParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this.query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "sp_Query_ErpOutOfPackage";
                    this.query_db.AddInParameter(cmd, "@PackageNoList", DbType.String, p.PackageNo);
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

                    cmd.CommandTimeout = 120;

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

        public MethodReturnResult CheckLotInPackage(Package packageWait, Package packageInChest)
        {
            MethodReturnResult result = new MethodReturnResult();
            PagingConfig cfg = new PagingConfig();
            MethodReturnResult<MaterialChestParameter> resultOfMaCP = null;
            try
            {
                #region 1.判断产品编码是否一致
                if (packageWait.MaterialCode != packageInChest.MaterialCode)
                {
                    result.Code = 3002;
                    result.Message = string.Format("待入柜托[{0}]料号[{1}]与已入柜第一托组件[{2}]料号[{3}]不一致！",
                                                        packageWait.Key,
                                                        packageWait.MaterialCode,
                                                        packageInChest.Key,
                                                        packageInChest.MaterialCode);

                    return result;
                }
                #endregion

                #region 2.校验等级/花色/分档/电流规则/工单/柜最大满包数量

                #region 2.0参数定义
                bool colorLimt = false;
                bool gradeLimt = false;
                bool iscLimt = false;
                bool powerLimt = false;
                bool ordernumberLimt = false;
                int InChestFullPackageQty = -1;
                int fullChestQty = -1;
                int fullPackageQty = 0;

                #endregion

                #region 2.1获取入柜控制参数
                resultOfMaCP = new MethodReturnResult<MaterialChestParameter>();
                resultOfMaCP.Data = this.MaterialChestParameterDataEngine.Get(packageWait.MaterialCode);
                if (resultOfMaCP != null && resultOfMaCP.Data != null)
                {
                    #region 2.1.1成柜控制参数获取（电流/等级/功率/颜色/工单/柜内最大满包数量）
                    colorLimt = Convert.ToBoolean(resultOfMaCP.Data.ColorLimit);
                    gradeLimt = Convert.ToBoolean(resultOfMaCP.Data.GradeLimit);
                    powerLimt = Convert.ToBoolean(resultOfMaCP.Data.PowerLimit);
                    iscLimt = Convert.ToBoolean(resultOfMaCP.Data.IscLimit);
                    ordernumberLimt = Convert.ToBoolean(resultOfMaCP.Data.OrderNumberLimit);
                    fullChestQty = Convert.ToInt32(resultOfMaCP.Data.FullChestQty);
                    InChestFullPackageQty = Convert.ToInt32(resultOfMaCP.Data.InChestFullPackageQty);
                    #endregion
                }
                else
                {
                    result.Code = 2000;
                    result.Message = string.Format(@"产品编码（{0}）成柜规则不存在，请找成柜规则制定人员进行设置！", packageWait.MaterialCode);
                    return result;
                }
                #endregion

                #region 2.2判段控制参数是否一致
                //判断等级是否一致
                if (packageWait.Grade != packageInChest.Grade)
                {
                    if (gradeLimt)
                    {
                        result.Code = 3002;
                        result.Message = string.Format("待入柜托[{0}]的等级[{1}]与已入柜第一托组件[{2}]等级[{3}]不一致！",
                                                        packageWait.Key,
                                                        packageWait.Grade,
                                                        packageInChest.Key,
                                                        packageInChest.Grade);

                        return result;
                    }
                }

                //判断花色是否一致
                if (packageWait.Color != packageInChest.Color)
                {
                    if (colorLimt)
                    {
                        result.Code = 3003;
                        result.Message = string.Format("待入柜托[{0}]的花色[{1}]与已入柜第一托组件[{2}]花色[{3}]不一致！",
                                                        packageWait.Key,
                                                        packageWait.Color,
                                                        packageInChest.Key,
                                                        packageInChest.Color);

                        return result;
                    }
                }

                //判断功率是否一致
                if (packageWait.PowerName != packageInChest.PowerName)
                {
                    if (powerLimt)
                    {
                        result.Code = 3003;
                        result.Message = string.Format("待入柜托[{0}]的功率[{1}]与已入柜第一托组件[{2}]功率[{3}]不一致！",
                                                        packageWait.Key,
                                                        packageWait.PowerName,
                                                        packageInChest.Key,
                                                        packageInChest.PowerName);

                        return result;
                    }
                }

                //判断电流档是否一致
                if (packageWait.PowerSubCode != packageInChest.PowerSubCode)
                {
                    if (iscLimt)
                    {
                        result.Code = 3003;
                        result.Message = string.Format("待入柜托[{0}]的电流档[{1}]与已入柜第一托组件[{2}]电流档[{3}]不一致！",
                                                        packageWait.Key,
                                                        packageWait.PowerSubCode,
                                                        packageInChest.Key,
                                                        packageInChest.PowerSubCode);

                        return result;
                    }
                }

                //判断工单是否一致
                if (packageWait.OrderNumber != packageInChest.OrderNumber)
                {
                    if (ordernumberLimt)
                    {
                        result.Code = 3003;
                        result.Message = string.Format("待入柜托[{0}]的工单号[{1}]与已入柜第一托组件[{2}]工单号[{3}]不一致！",
                                                        packageWait.Key,
                                                        packageWait.OrderNumber,
                                                        packageInChest.Key,
                                                        packageInChest.OrderNumber);

                        return result;
                    }
                    else
                    {
                        #region 1.判断待入柜托是否设置不可混工单
                        //获取该工单是非设置允许混工单
                        WorkOrderAttributeKey workAttrKey = new WorkOrderAttributeKey()
                        {
                            OrderNumber = packageWait.OrderNumber,
                            AttributeName = "PackageLimited"
                        };
                        WorkOrderAttribute workAttr = WorkOrderAttributeDataEngine.Get(workAttrKey);
                        //设置不允许混工单
                        if (workAttr != null && workAttr.AttributeValue.ToUpper() == "TRUE")
                        {
                            result.Code = 2008;
                            result.Message = string.Format("待入柜托（{0}）所在工单（{1}）设置了不可混工单！",
                                                            packageWait.Key, packageWait.OrderNumber);
                            return result;
                        }
                        #endregion

                        #region 2.判断已入柜第一托是否设置不可混工单
                        workAttrKey = new WorkOrderAttributeKey()
                        {
                            OrderNumber = packageWait.OrderNumber,
                            AttributeName = "PackageLimited"
                        };
                        workAttr = WorkOrderAttributeDataEngine.Get(workAttrKey);
                        //设置不允许混工单
                        if (workAttr != null && workAttr.AttributeValue.ToUpper() == "TRUE")
                        {
                            result.Code = 2008;
                            result.Message = string.Format("柜号（{0}）已入柜第一托（{1}）所在工单（{2}）设置了不可混工单！",
                                                            packageInChest.ContainerNo, packageInChest.Key, packageInChest.OrderNumber);
                            return result;
                        }
                        #endregion

                        #region 3.判断待入柜托工单及已入柜第一托是否设置混工单组
                        cfg = new PagingConfig()
                        {
                            Where = string.Format(@"Key.OrderNumber = '{0}'", packageWait.OrderNumber)
                        };
                        IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                        //待入柜托工单设置了混工单组
                        if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                        {
                            #region 判断已入柜第一托工单是否设置混工单组
                            cfg = new PagingConfig()
                            {
                                Where = string.Format(@"Key.OrderNumber = '{0}'", packageInChest.OrderNumber)
                            };
                            IList<WorkOrderGroupDetail> lstChestWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                            //已入柜第一托工单设置了混工单组
                            if (lstChestWorkOrderGroupDetail != null && lstChestWorkOrderGroupDetail.Count > 0)
                            {
                                if (lstChestWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString() != lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString())
                                {
                                    result.Code = 2008;
                                    result.Message = string.Format("待入柜托：（{0}）所在工单（{1} 设置的混工单组（{2}）与已入柜第一托（{3}）所在工单（{4}）设置的混工单组（{5}）不一致！",
                                                                    packageWait.Key, packageWait.OrderNumber, lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString(),
                                                                    packageInChest.Key, packageInChest.OrderNumber, lstChestWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString());
                                    return result;
                                }
                            }
                            //已入柜第一托工单没设混工单组
                            else
                            {
                                result.Code = 2008;
                                result.Message = string.Format("已入柜第一托：（{0}）所在工单（{1} 未设置混工单组规则，但待入柜托（{2}）所在工单（{3}）设置了混工单组！",
                                                                packageInChest.Key, packageInChest.OrderNumber, packageWait.Key, packageWait.OrderNumber);
                                return result;
                            }
                            #endregion
                        }
                        //待入柜托工单没设混工单组
                        else
                        {
                            #region 判断已入柜第一托工单是否设置混工单组
                            cfg = new PagingConfig()
                            {
                                Where = string.Format(@"Key.OrderNumber = '{0}'", packageInChest.OrderNumber)
                            };
                            IList<WorkOrderGroupDetail> lstChestWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                            //已入柜第一托工单设置了混工单组
                            if (lstChestWorkOrderGroupDetail != null && lstChestWorkOrderGroupDetail.Count > 0)
                            {
                                result.Code = 2008;
                                result.Message = string.Format("已入柜第一托：（{0}）所在工单（{1} 已设置混工单组规则，但待入柜托（{2}）所在工单（{3}）未设置混工单组！",
                                                                packageInChest.Key, packageInChest.OrderNumber, packageWait.Key, packageWait.OrderNumber);
                                return result;
                            }
                            #endregion
                        }
                        #endregion
                    }
                }

                #region 校验柜内满包数量
                //获取托号满包数量
                WorkOrderRuleKey WorkOrderRuleKey = new WorkOrderRuleKey()
                {
                    OrderNumber = packageWait.OrderNumber,
                    MaterialCode = packageWait.MaterialCode
                };
                WorkOrderRule resultWorkOrderRule = this.WorkOrderRuleDataEngine.Get(WorkOrderRuleKey);
                if (resultWorkOrderRule == null)
                {
                    result.Code = 3003;
                    result.Message = string.Format(@"工单[{0}]未设置满包数量！",packageWait.OrderNumber);

                    return result;
                }
                fullPackageQty = Convert.ToInt32(resultWorkOrderRule.FullPackageQty);
                
                if (InChestFullPackageQty != -1)
                {
                    if (packageWait.Quantity == fullPackageQty)
                    {
                        //获取柜号信息
                        Chest chestOfGet = this.ChestDataEngine.Get(packageInChest.ContainerNo);
                        PagingConfig cfgChest = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@" Key.ChestNo = '{0}'    
                                                 AND EXISTS(FROM Package as package
                                                 WHERE package.Key=self.Key.ObjectNumber
                                                 AND package.Quantity = '{1}')", chestOfGet.Key, fullPackageQty)
                        };
                        IList<ChestDetail> lstChestDetail = this.ChestDetailDataEngine.Get(cfgChest);
                        if (lstChestDetail != null)
                        {
                            if (InChestFullPackageQty != 0)
                            {
                                if (lstChestDetail.Count >= InChestFullPackageQty)
                                {
                                    result.Code = 3003;
                                    result.Message = string.Format(@"待入柜托[{0}]为满包托号，
                                                                    但柜号[{1}]已入到柜内的满包数量[{2}]已达到成柜规则设定柜内最大满包数量[{3}]，不可入该柜！",
                                                                    packageWait.Key,
                                                                    packageInChest.ContainerNo,
                                                                    lstChestDetail.Count,
                                                                    InChestFullPackageQty);

                                    return result;
                                }
                            }                            
                        }
                    }
                    else
                    {
                        //获取柜号信息
                        Chest chestOfGet = this.ChestDataEngine.Get(packageInChest.ContainerNo);
                        PagingConfig cfgChest = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@" Key.ChestNo = '{0}'    
                                                 AND EXISTS(FROM Package as package
                                                 WHERE package.Key=self.Key.ObjectNumber
                                                 AND package.Quantity <> '{1}')", chestOfGet.Key,  fullPackageQty)
                        };
                        IList<ChestDetail> lstChestDetail = this.ChestDetailDataEngine.Get(cfgChest);
                        if (lstChestDetail != null)
                        {
                            if (InChestFullPackageQty != 0)
                            {
                                if (lstChestDetail.Count >= (fullChestQty - InChestFullPackageQty))
                                {
                                    result.Code = 3003;
                                    result.Message = string.Format(@"待入柜托[{0}]为非满包托号，
                                                                    但柜号[{1}]已入到柜内的非满包数量[{2}]已达到成柜规则设定非瞒包数量[{3}]，不可入该柜！",
                                                                    packageWait.Key,
                                                                    packageInChest.ContainerNo,
                                                                    lstChestDetail.Count,
                                                                    fullChestQty - InChestFullPackageQty);

                                    return result;
                                }
                            }                           
                        }
                    }
                }
                #endregion

                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }

        public MethodReturnResult<Chest> Get(string key)
        {
            MethodReturnResult<Chest> result = new MethodReturnResult<Chest>();
            if (!this.ChestDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(@"柜[{0}]不存在。", key);
                return result;
            }
            try
            {
                result.Data = this.ChestDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(@"错误：{0}", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        #endregion

        public void Dispose()
        {
            this.sessionFactory.Dispose();
        }
    }
}
