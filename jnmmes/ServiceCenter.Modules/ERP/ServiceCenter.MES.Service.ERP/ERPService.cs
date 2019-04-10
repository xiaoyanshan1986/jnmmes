using Microsoft.Practices.EnterpriseLibrary.Data;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.LSM;
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Model.ERP;
using ServiceCenter.MES.Service.Contract.ERP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.Contract.PPM;
using ServiceCenter.Common.Model;
using System.Data.SqlClient;
using System.Configuration;

namespace ServiceCenter.MES.Service.ERP
{
    /// <summary> 从ERP获取工单信息 </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ERPService : IERPContract
    {
        string localName = System.Configuration.ConfigurationSettings.AppSettings["LocalName"];
        string ErpDBName = string.Empty;
        string ErpAccount = string.Empty;
        string ErpGroupCode = string.Empty;
        string ErpORGCode = string.Empty;

        #region define List of DataEngine
        List<Lot> lstLotDataEngineForUpdate = new List<Lot>();
        List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();
        List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();
        List<LotTransactionParameter> lstLotTransactionParameterDataEngineForInsert = new List<LotTransactionParameter>();
        List<Package> lstPackageDataForUpdate = new List<Package>();

        //List<PackageBin> lstPackageBinForUpdate = new List<PackageBin>();
        //List<PackageBin> lstPackageBinForInsert = new List<PackageBin>();
        List<PackageDetail> lstPackageDetailForInsert = new List<PackageDetail>();
        List<PackageDetail> lstPackageDetailForUpdate = new List<PackageDetail>();
        List<PackageDetail> lstPackageDetailForDelete = new List<PackageDetail>();

        List<LotTransactionPackage> lstLotTransactionPackageForInsert = new List<LotTransactionPackage>();
        #endregion

        /// <summary>
        /// 数据库对象。
        /// </summary>
        protected Database Ora_db;
        protected Database _db;
        public IWorkOrderDataEngine WorkOrderDataEngine { get; set; }

        public IWorkOrderBOMDataEngine WorkOrderBOMDataEngine { get; set; }

        public IMaterialDataEngine MaterialDataEngine { get; set; }

        public IMaterialRouteDataEngine MaterialRouteDataEngine { get; set; }

        /// <summary>
        /// 物料属性数据访问对象。
        /// </summary>
        public IMaterialAttributeDataEngine MaterialAttributeDataEngine { get; set; }

        public IMaterialReceiptDataEngine MaterialReceiptDataEngine { get; set; }

        public IMaterialReceiptDetailDataEngine MaterialReceiptDetailDataEngine { get; set; }

        public ILineStoreMaterialDataEngine LineStoreMaterialDataEngine { get; set; }

        public ILineStoreMaterialDetailDataEngine LineStoreMaterialDetailDataEngine { get; set; }

        public IMaterialTypeDataEngine MaterialTypeDataEngine { get; set; }

        public IPackageDataEngine PackageDataEngine { get; set; }

        public IWorkOrderRouteDataEngine WorkOrderRouteDataEngine { get; set; }

        /// <summary>
        /// 工单属性数据访问对象
        /// </summary>
        public IWorkOrderAttributeDataEngine WorkOrderAttributeDataEngine { get; set; }

        public IPackageDetailDataEngine PackageDetailDataEngine { get; set; }

        public ILotDataEngine LotDataEngine { get; set; }

        public ILotTransactionDataEngine LotTransactionDataEngine { get; set; }

        public ILotTransactionHistoryDataEngine LotTransactionHistoryDataEngine { get; set; }

        public ILotTransactionParameterDataEngine LotTransactionParameterDataEngine { get; set; }

        public IWorkOrderProductDataEngine WorkOrderProductDataEngine { get; set; }

        public IMaterialTypeRouteDataEngine MaterialTypeRouteDataEngine { get; set; }

        public IRouteEnterpriseDetailDataEngine RouteEnterpriseDetailDataEngine { get; set; }

        public IRouteStepDataEngine RouteStepDataEngine { get; set; }

        public IRouteDataEngine RouteDataEngine { get; set; }

        public ISupplierDataEngine SupplierDataEngine { get; set; }

        public IManufacturerDataEngine ManufacturerDataEngine { get; set; }

        public ISupplierToManufacturerDataEngine SupplierToManufacturerDataEngine { get; set; }

        /// <summary> 系统参数属性 </summary>
        public IBaseAttributeValueDataEngine BaseAttributeValueDataEngine { get; set; }

        /// <summary> 区域属性 </summary>
        public ILocationDataEngine LocationDataEngine { get; set; }

        /// <summary> 物料替换规则 </summary>
        public IMaterialReplaceDataEngine MaterialReplaceDataEngine { get; set; }

        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        public ERPService(ISessionFactory sf)
        {
            //ErpDBName = ConfigurationSettings.AppSettings["ErpDBName"].ToString();
            ErpDBName = ConfigurationManager.AppSettings["ErpDBName"].ToString();
            ErpAccount = ConfigurationManager.AppSettings["ErpAccount"].ToString();
            ErpGroupCode = ConfigurationManager.AppSettings["ErpGroupCode"].ToString();
            ErpORGCode = ConfigurationManager.AppSettings["ErpORGCode"].ToString();

            this.Ora_db = DatabaseFactory.CreateDatabase("ERPDB");
            this.SessionFactory = sf;
            this._db = DatabaseFactory.CreateDatabase();
        }

        /// <summary>
        /// 查询ERP销售单号
        /// </summary>
        /// <param name="SalesNo">销售单号</param>
        /// <returns></returns>  
        public MethodReturnResult<DataSet> GetERPSaleOut(string SalesNo)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select t1.vbatchcode from " + ErpDBName + ".v_ic_saleout_b t1 where t1.groupcode = '{0}' and t1.orgcode='{1}' and t1.xyhcode = '{2}'",
                                                    ErpGroupCode,
                                                    ErpORGCode,
                                                    SalesNo);
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

        /// <summary> 取得ERP工单信息 </summary>
        /// <param name="OrderNumber">工单号</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPWorkOrder(string OrderNumber)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandText = string.Format("select * " + 
                                                    "from " + ErpDBName + ".v_mm_dmo " +
                                                    "where groupcode = '{0}' and orgcode='{1}' and (materialcode like '12%' or materialcode like '2512%') and vbillcode = '{2}'",
                                                    ErpGroupCode,
                                                    ErpORGCode,
                                                    OrderNumber);

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

        /// <summary> 根据工单号从ERP系统里获取工单的BOM信息 </summary>
        /// <param name="OrderNumber">工单号</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPWorkOrderBOM(string OrderNumber)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandText = string.Format(@"select ROW_NUMBER() OVER ( ORDER BY CAST(t1.序号 AS INT) ) AS 行号,t1.* FROM ( "+
                                                             "select mPlandtail.vrowno AS 序号," +                  //ERP行号
                                                             " mPlandtail.materialcode," +                          //物料代码
                                                             " mPlandtail.materialname," +                          //物料名称
                                                             " mPlandtail.meas," +                                  //单位
                                                             " case when mPlandtail.minunit is null then '0' " +
                                                             "      when mPlandtail.minunit = '~' then '0' " +
                                                             "      else mPlandtail.minunit " +
                                                             "      end minunit," +                                 //最小扣料单位
                                                             " mPlandtail.nunitnum," +                              //消耗数量
                                                             " material.materialspec, " +                           //物料规格
                                                             " material.materialtype, " +                           //物料型号
                                                             " material.maclasscode," +                             //物料分类
                                                             " material.maclassname" +                              //物料分类名称
                                                      " from " + ErpDBName + ".v_mm_pickm materialPlan " +
                                                         "left join " + ErpDBName + ".v_mm_pickm_b mPlandtail on mPlandtail.groupcode = '{0}' and mPlandtail.orgcode='{1}' and mPlandtail.pickcode = materialPlan.vbillcode " +
                                                         "left join " + ErpDBName + ".v_bd_material material  on material.groupcode = '{0}' and material.orgcode='{1}' and material.code = mPlandtail.materialcode " +
                                                      " where materialPlan.groupcode = '{0}' and materialPlan.orgcode='{1}' and materialPlan.vsourcemocode = '{2}') t1 ",
                                                       ErpGroupCode,
                                                       ErpORGCode,
                                                       OrderNumber);
                    
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

        /// <summary> 根据物料编码从ERP里获取相应所有供应商信息？？？ </summary>
        /// <param name="SupplierCode"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPSupplier(string SupplierCode)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"select * from " + ErpDBName + ".V_BD_CUST_SUPPLIER where groupcode = '{0}' and orgcode='{1}' and cussuptype='供应商' and cuscode='{2}'",
                                                    ErpGroupCode,
                                                    ErpORGCode,                     
                                                    SupplierCode);

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

        /// <summary> 根据生产厂商名称从ERP里获取相应所有生产厂商信息？？？ </summary>
        /// <param name="SupplierCode"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetByNameERPManufacturer(string ManufacturerName) 
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"select * from " + ErpDBName + ".V_SCCS where csname='{0}'",
                                                    ManufacturerName);

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

        /// <summary> 根据生产厂商代码从ERP里获取相应所有生产厂商信息？？？ </summary>
        /// <param name="SupplierCode"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetByCodeERPManufacturer(string ManufacturerCode)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"select * from " + ErpDBName + ".V_SCCS where cscode={0}",
                                                    ManufacturerCode);

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
  
        /// <summary> 新增ERP工单信息 </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult AddERPWorkOrder(ERPWorkOrderParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            ISession session = null;
            ITransaction transaction = null;

            try
            {
                string strWOState = "";
                string strERPOrderType = "";
                string strOrderType = "";
                string strERPDept = "";
                string strLocationName = "";
                bool isUpdate = false;              //更新数据标志
                PagingConfig cfg = null;
                List<WorkOrderRoute> lstWORoute = new List<WorkOrderRoute>();
                double ERPWorkOrderQuantity = 0;
                WorkOrderProduct woproduct = null;
                List<Material> lstmaterial = null;
                List<MaterialType> lstmaterialType = null;
                List<WorkOrderBOM> lstbom = null;
                List<WorkOrderAttribute> lstWorkOrderAttributeForInsert = new List<WorkOrderAttribute>();

                #region 1.属性判断
                //1.检验工单是否已经存在
                WorkOrder workOrder = WorkOrderDataEngine.Get(p.OrderNumber);

                if (workOrder != null)
                {
                    isUpdate = true;
                }

                //2.取得ERP工单信息
                MethodReturnResult<DataSet> resultERPWO = GetERPWorkOrder(p.OrderNumber);

                if ((resultERPWO.Code == 0 && resultERPWO.Data.Tables[0].Rows.Count > 0) == false)
                {
                    result.Code = 1001;
                    result.Message = "ERP系统中工单[" + p.OrderNumber + "]不存在！";

                    return result;
                }

                //3.根据ERP工单状态取得MES工单状态
                strWOState = resultERPWO.Data.Tables[0].Rows[0]["FBILLSTATE"].ToString();               //ERP工单状态

                //订单需要是投放状态（自由 - -1、审批 - 1、投放 - 2、完工 - 3、关闭 - 4）
                if (!(strWOState == "2" || strWOState == "3" || strWOState == "4"))
                {
                    result.Code = 1002;
                    result.Message = "订单需要为投放状态！";
                    
                    return result;
                }

                //4.根据ERP工单类型取得MES工单类型
                strERPOrderType = resultERPWO.Data.Tables[0].Rows[0]["VTRANTYPECODE"].ToString();       //ERP工单类型

                strOrderType = GetMESOrderType(strERPOrderType);

                if (strOrderType == "")
                {
                    result.Code = 1003;
                    result.Message = "ERP订单类型[" + strERPOrderType + "]在系统中无对应类型！";

                    return result;
                }

                //5.根据ERP部门代码取得MES对应的工厂代码
                strERPDept = resultERPWO.Data.Tables[0].Rows[0]["CJCODE"].ToString();

                strLocationName = GetLocationName(strERPDept);

                if (strLocationName == "")
                {
                    result.Code = 1003;
                    result.Message = "ERP车间代码[" + strERPDept + "]在系统中无对应车间！";

                    return result;
                }
                #endregion

                #region 2.创建工单对象
                if (isUpdate)
                {
                    //判断工单数量是否满足已投批数量
                    ERPWorkOrderQuantity = Convert.ToDouble(resultERPWO.Data.Tables[0].Rows[0]["NNUM"].ToString());                 //ERP工单数量

                    if (workOrder.LeftQuantity + (ERPWorkOrderQuantity - workOrder.OrderQuantity) < 0)
                    {
                        result.Code = 1000;
                        result.Message = string.Format("工单数量（{0}）小于已创批数量（{1}）！",
                                                    ERPWorkOrderQuantity,
                                                    workOrder.OrderQuantity - workOrder.LeftQuantity);
                        
                        return result;
                    }

                    workOrder.LeftQuantity = workOrder.LeftQuantity + (ERPWorkOrderQuantity - workOrder.OrderQuantity);             //剩余数量
                    workOrder.OrderQuantity = ERPWorkOrderQuantity;                                                                 //生产数量
                    workOrder.PlanFinishTime = DateTime.Parse(resultERPWO.Data.Tables[0].Rows[0]["TPLANENDTIME"].ToString());       //计划完工日期
                    workOrder.Description = resultERPWO.Data.Tables[0].Rows[0]["VNOTE"].ToString();                                 //备注 
                    workOrder.EditTime = DateTime.Now;                                                                              //编辑时间
                    workOrder.Editor = p.Creator;                                                                                   //编辑人

                    //工单完工状态
                    if (resultERPWO.Data.Tables[0].Rows[0]["FBILLSTATE"].ToString() == "4")
                    {
                        workOrder.OrderState = EnumWorkOrderState.Close;        //订单状态为关闭状态
                        workOrder.CloseType = EnumWorkOrderCloseType.Normal;    //订单关闭类型
                    }
                }
                else
                {
                    workOrder = new WorkOrder()
                    {
                        Key = resultERPWO.Data.Tables[0].Rows[0]["VBILLCODE"].ToString(),                                   //工单号
                        ERPWorkOrderKey = resultERPWO.Data.Tables[0].Rows[0]["PK_DMO"].ToString(),                          //ERP工单号主键
                        MaterialCode = resultERPWO.Data.Tables[0].Rows[0]["MATERIALCODE"].ToString(),                       //产品代码
                        OrderQuantity = Convert.ToDouble(resultERPWO.Data.Tables[0].Rows[0]["NNUM"].ToString()),            //生产数量
                        OrderType = strOrderType,                                                                           //工单类型                        
                        LocationName = strLocationName,                                                                     //生产车间
                        PlanStartTime = DateTime.Parse(resultERPWO.Data.Tables[0].Rows[0]["TPLANSTARTTIME"].ToString()),    //计划开工日期
                        PlanFinishTime = DateTime.Parse(resultERPWO.Data.Tables[0].Rows[0]["TPLANENDTIME"].ToString()),     //计划完工日期
                        StartTime = DateTime.Parse(resultERPWO.Data.Tables[0].Rows[0]["TPLANSTARTTIME"].ToString()),        //开始时间
                        FinishTime = DateTime.Parse(resultERPWO.Data.Tables[0].Rows[0]["TPLANENDTIME"].ToString()),         //完工时间
                        LeftQuantity = Convert.ToDouble(resultERPWO.Data.Tables[0].Rows[0]["NNUM"].ToString()),             //剩余数量
                        FinishQuantity = 0,                             //完工数量
                        ReworkQuantity = 0,                             //返工数量
                        RepairQuantity = 0,                             //返修数量
                        CloseType = EnumWorkOrderCloseType.None,        //关闭状态
                        OrderState = EnumWorkOrderState.Open,           //工单状态      
                        Priority = EnumWorkOrderPriority.Normal,        //优先级                        
                        Description = resultERPWO.Data.Tables[0].Rows[0]["VNOTE"].ToString(),                               //备注
                        CreateType = EnumWorkOrderCreateType.ERP,       //工单创建来源
                        CreateTime = DateTime.Now,                      //创建时间
                        Creator = p.Creator,                            //创建人
                        EditTime = DateTime.Now,                        //编辑时间
                        Editor = p.Creator                              //编辑人
                    };
                }
                #endregion

                if (!isUpdate)
                {
                    #region 3.添加工单产品
                    //判断产品是否存在
                    Material productMaterial = MaterialDataEngine.Get(resultERPWO.Data.Tables[0].Rows[0]["MATERIALCODE"].ToString());

                    if (productMaterial == null)
                    {
                        result.Code = 1006;
                        result.Message = string.Format("工单产品（{0}）尚未设置，请先设置产品信息！",
                                                    resultERPWO.Data.Tables[0].Rows[0]["MATERIALCODE"].ToString());

                        return result;
                    }

                    //设置工单产品。
                    woproduct = new WorkOrderProduct()
                    {
                        Key = new WorkOrderProductKey()
                        {
                            OrderNumber = resultERPWO.Data.Tables[0].Rows[0]["VBILLCODE"].ToString(),
                            MaterialCode = resultERPWO.Data.Tables[0].Rows[0]["MATERIALCODE"].ToString()
                        },
                        IsMain = true,
                        ItemNo = 0,
                        CreateTime = DateTime.Now,
                        Creator = p.Creator,
                        Editor = p.Creator,
                        EditTime = DateTime.Now,
                    };
                    #endregion
                       
                    #region 4.添加工单BOM及新物料
                    lstmaterial = new List<Material>();
                    lstmaterialType = new List<MaterialType>();
                    lstbom = new List<WorkOrderBOM>();

                    //4.1取得ERP工单BOM列表
                    MethodReturnResult<DataSet> resultERPBOM = GetERPWorkOrderBOM(p.OrderNumber);

                    if (resultERPBOM.Data != null && resultERPBOM.Data.Tables.Count > 0)
                    {
                        for (int i = 0; i < resultERPBOM.Data.Tables[0].Rows.Count; i++)
                        {
                            //1.循环创建工单BOM对象//ReplaceMaterial = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString();           //可替换料
                            WorkOrderBOM woBOM = new WorkOrderBOM()
                            {
                                Key = new WorkOrderBOMKey()
                                {
                                    OrderNumber = workOrder.Key,          //工单号

                                    ItemNo = Int32.Parse(resultERPBOM.Data.Tables[0].Rows[i]["序号"].ToString())          //项目号
                                },
                                MaterialCode = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString(),              //物料代码
                                Qty = Convert.ToDecimal(resultERPBOM.Data.Tables[0].Rows[i]["NUNITNUM"].ToString()),        //消耗量
                                MaterialUnit = resultERPBOM.Data.Tables[0].Rows[i]["MEAS"].ToString(),                      //计量单位
                                MinUnit = Convert.ToDecimal(resultERPBOM.Data.Tables[0].Rows[i]["MINUNIT"].ToString()),     //最小扣料单位
                                CreateTime = DateTime.Now,      //创建时间
                                Creator = p.Creator,            //创建人
                                Editor = p.Creator,             //编辑人
                                EditTime = DateTime.Now         //编辑时间
                            };

                            lstbom.Add(woBOM);

                            //2.判断BOM中的物料物料信息表中是否存在，如不存在则添加
                            Material material = MaterialDataEngine.Get(resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString());

                            if (material == null)
                            {
                                //判断物料是否已经在列表中
                                int count = lstmaterial.Count(m => m.Key == resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString());

                                if (count == 0)
                                {
                                    material = new Material()
                                    {
                                        Key = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString(),           //物料代码
                                        Name = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALNAME"].ToString(),          //物料名称
                                        Spec = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALSPEC"].ToString(),          //规格
                                        ModelName = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALTYPE"].ToString(),     //型号
                                        Type = resultERPBOM.Data.Tables[0].Rows[i]["MACLASSCODE"].ToString(),           //类型
                                        //Class = resultERPBOM.Data.Tables[0].Rows[i]["MACLASSNAME"].ToString(),          //物料分类
                                        Unit = resultERPBOM.Data.Tables[0].Rows[i]["MEAS"].ToString(),                  //计量单位
                                        CreateTime = DateTime.Now,
                                        EditTime = DateTime.Now,
                                        Creator = p.Creator,
                                        Editor = p.Creator,
                                        IsProduct = false,
                                        IsRaw = true,
                                        Status = EnumObjectStatus.Available,
                                        Description = ""
                                    };

                                    lstmaterial.Add(material);

                                    //判断物料类型是否存在
                                    MaterialType materialType = MaterialTypeDataEngine.Get(material.Type);

                                    if (materialType == null)
                                    {
                                        materialType = new MaterialType()
                                        {
                                            Key = material.Type,
                                            Description = "",
                                            CreateTime = DateTime.Now,
                                            EditTime = DateTime.Now,
                                            Creator = p.Creator,
                                            Editor = p.Creator
                                        };

                                        List<string> lstKey = new List<string>();
                                        foreach (MaterialType item in lstmaterialType)
                                        {
                                            lstKey.Add(item.Key);
                                        }
                                        if (!lstKey.Contains(material.Type))
                                        {
                                            lstmaterialType.Add(materialType);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region 5.设置生产工单工单生产工艺（根据产品）
                    if (workOrder.OrderType == "1")
                    {
                        //5.1根据生产车间取得产品加工工艺流程组
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@"Key.MaterialCode = '{0}' 
                                                    AND Key.LocationName = '{1}'"
                                , workOrder.MaterialCode
                                , workOrder.LocationName )
                        };

                        IList<MaterialRoute> lstMaterialRoute = this.MaterialRouteDataEngine.Get(cfg);

                        if ( lstMaterialRoute.Count > 0 )
                        {
                            int j = 1;      //工单工艺流程项目号计数器

                            foreach( MaterialRoute mRoute in lstMaterialRoute )
                            {
                                //取得工艺流程组对应的工艺流程
                                cfg = new PagingConfig()
                                {
                                    IsPaging = false,
                                    Where = string.Format(@"Key.RouteEnterpriseName = '{0}'"
                                        , mRoute.Key.RouteEnterpriseName)
                                };

                                IList<RouteEnterpriseDetail> lstRouteEnterpriseDetail = this.RouteEnterpriseDetailDataEngine.Get(cfg);

                                if (lstRouteEnterpriseDetail.Count > 0)
                                {
                                    //循环取得工艺流程
                                    foreach (RouteEnterpriseDetail routeEnterpriseDetail in lstRouteEnterpriseDetail)
                                    {
                                        //取得工艺流程第一个工步对象
                                        cfg = new PagingConfig()
                                        {
                                            IsPaging = false,
                                            Where = string.Format(@"Key.RouteName = '{0}'"
                                                , routeEnterpriseDetail.Key.RouteName),
                                            OrderBy = "SortSeq"
                                        };

                                        IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);

                                        if (lstRouteStep.Count > 0)
                                        {
                                            //取得工艺流程对象,取得工艺流程类型（主流程、返修流程）
                                            Route route = this.RouteDataEngine.Get(routeEnterpriseDetail.Key.RouteName);

                                            if (route == null)
                                            {
                                                result.Code = 2001;
                                                result.Message = string.Format("工艺流程（{0}）不存在！"
                                                                                , lstRouteStep[0].Key.RouteName);
                                                return result;
                                            }

                                            bool isRework = false;

                                            if (route.RouteType == EnumRouteType.Repair)
                                            {
                                                isRework = true;
                                            }

                                            //创建工单工艺流程
                                            WorkOrderRoute woRoute = new WorkOrderRoute()
                                            {
                                                Key = new WorkOrderRouteKey()
                                                {
                                                    OrderNumber = workOrder.Key,
                                                    ItemNo = j
                                                },
                                                RouteEnterpriseName = routeEnterpriseDetail.Key.RouteEnterpriseName,
                                                RouteName = lstRouteStep[0].Key.RouteName,
                                                RouteStepName = lstRouteStep[0].Key.RouteStepName,
                                                Creator = p.Creator,
                                                CreateTime = DateTime.Now,
                                                Editor = p.Creator,
                                                EditTime = DateTime.Now,
                                                IsRework = isRework
                                            };

                                            lstWORoute.Add(woRoute);

                                            j++;
                                        }
                                    }
                                }
                            } 
                        }
                    }         
                    #endregion

                    #region 6.工单属性设置(根据产品设置取得对应属性)
                    //1.取得产品对应属性设置
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.MaterialCode = '{0}'",
                                                workOrder.MaterialCode)
                    };

                    IList<MaterialAttribute> lstMaterialAttribute = this.MaterialAttributeDataEngine.Get(cfg);

                    foreach (MaterialAttribute ma in lstMaterialAttribute)
                    {
                        //创建工单属性列表
                        WorkOrderAttribute wora = new WorkOrderAttribute()
                        {
                            Key = new WorkOrderAttributeKey()
                            {
                                OrderNumber = workOrder.Key,
                                AttributeName = ma.Key.AttributeName
                            },
                            AttributeValue = ma.Value,
                            Editor = p.Creator,
                            EditTime = DateTime.Now
                        };

                        lstWorkOrderAttributeForInsert.Add(wora);
                    }
                    #endregion
                }

                #region 6.数据提交
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                //6.1 工单
                if (isUpdate)
                {
                    this.WorkOrderDataEngine.Update(workOrder, session);
                }
                else
                {
                    this.WorkOrderDataEngine.Insert(workOrder, session);

                    //6.2 工单产品
                    this.WorkOrderProductDataEngine.Insert(woproduct, session);

                    //6.3 工单BOM
                    if (lstbom.Count > 0)
                    {
                        foreach (WorkOrderBOM woBOM in lstbom)
                        {
                            this.WorkOrderBOMDataEngine.Insert(woBOM, session);
                        }
                    }

                    //6.4 新增物料类型
                    if (lstmaterialType.Count > 0)
                    {
                        foreach (MaterialType materialType in lstmaterialType)
                        {
                            this.MaterialTypeDataEngine.Insert(materialType, session);
                        }
                    }

                    //6.5 新增物料
                    if (lstmaterial.Count > 0)
                    {
                        foreach (Material material in lstmaterial)
                        {
                            this.MaterialDataEngine.Insert(material, session);
                        }
                    }

                    //6.6 工单加工工艺
                    if (lstWORoute.Count > 0)
                    {
                        foreach (WorkOrderRoute woRoute in lstWORoute)
                        {
                            this.WorkOrderRouteDataEngine.Insert(woRoute, session);
                        }
                    }

                    //6.7 工单属性
                    foreach (WorkOrderAttribute wora in lstWorkOrderAttributeForInsert)
                    {
                        this.WorkOrderAttributeDataEngine.Insert(wora, session);
                    }
                }
                
                transaction.Commit();
                session.Close();

                #endregion
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    session.Close();
                }
                
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// 更新ERP工单信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult UpdateERPWorkOrder(ERPWorkOrderParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            ISession session = null;
            ITransaction transaction = null;

            try
            {
                string strWOState = "";
                string strERPOrderType = "";
                string strOrderType = "";
                string strERPDept = "";
                string strLocationName = "";

                PagingConfig cfg = null;

                List<WorkOrderRoute> lstWORoute = new List<WorkOrderRoute>();

                #region 1.属性判断
                //1.检验工单是否已经存在
                WorkOrder workOrder = WorkOrderDataEngine.Get(p.OrderNumber);

                if (workOrder == null)
                {
                    result.Code = 1001;
                    result.Message = "工单[" + p.OrderNumber + "]不存在！";

                    return result;
                }

                //2.取得ERP工单信息
                MethodReturnResult<DataSet> resultERPWO = GetERPWorkOrder(p.OrderNumber);

                if ((resultERPWO.Code == 0 && resultERPWO.Data.Tables[0].Rows.Count > 0) == false)
                {
                    result.Code = 1001;
                    result.Message = "ERP系统中工单[" + p.OrderNumber + "]不存在！";

                    return result;
                }

                //3.根据ERP工单状态取得MES工单状态
                strWOState = resultERPWO.Data.Tables[0].Rows[0]["FBILLSTATE"].ToString();               //ERP工单状态

                //订单需要是投放状态（自由 - -1、审批 - 1、投放 - 2、完工 - 3、关闭 - 4）
                if (strWOState != "2")
                {
                    result.Code = 1002;
                    result.Message = "订单需要为投放状态！";

                    return result;
                }

                //4.根据ERP工单类型取得MES工单类型
                strERPOrderType = resultERPWO.Data.Tables[0].Rows[0]["VTRANTYPECODE"].ToString();       //ERP工单类型

                strOrderType = GetMESOrderType(strERPOrderType);

                if (strOrderType == "")
                {
                    result.Code = 1003;
                    result.Message = "ERP订单类型[" + strERPOrderType + "]在系统中无对应类型！";

                    return result;
                }

                //5.根据ERP部门代码取得MES对应的工厂代码
                strERPDept = resultERPWO.Data.Tables[0].Rows[0]["CJCODE"].ToString();

                strLocationName = GetLocationName(strERPDept);

                if (strLocationName == "")
                {
                    result.Code = 1003;
                    result.Message = "ERP车间代码[" + strERPDept + "]在系统中无对应车间！";

                    return result;
                }
                #endregion

                #region 2.创建工单对象
                workOrder = new WorkOrder()
                {
                    Key = resultERPWO.Data.Tables[0].Rows[0]["VBILLCODE"].ToString(),                                   //工单号
                    ERPWorkOrderKey = resultERPWO.Data.Tables[0].Rows[0]["PK_DMO"].ToString(),                          //ERP工单号主键
                    MaterialCode = resultERPWO.Data.Tables[0].Rows[0]["MATERIALCODE"].ToString(),                       //产品代码
                    OrderQuantity = Convert.ToDouble(resultERPWO.Data.Tables[0].Rows[0]["NNUM"].ToString()),            //生产数量
                    OrderType = strOrderType,                                                                           //工单类型                        
                    LocationName = strLocationName,                                                                     //生产车间
                    PlanStartTime = DateTime.Parse(resultERPWO.Data.Tables[0].Rows[0]["TPLANSTARTTIME"].ToString()),    //计划开工日期
                    PlanFinishTime = DateTime.Parse(resultERPWO.Data.Tables[0].Rows[0]["TPLANENDTIME"].ToString()),     //计划完工日期
                    StartTime = DateTime.Parse(resultERPWO.Data.Tables[0].Rows[0]["TPLANSTARTTIME"].ToString()),        //开始时间
                    FinishTime = DateTime.Parse(resultERPWO.Data.Tables[0].Rows[0]["TPLANENDTIME"].ToString()),         //完工时间
                    LeftQuantity = Convert.ToDouble(resultERPWO.Data.Tables[0].Rows[0]["NNUM"].ToString()),             //剩余数量
                    FinishQuantity = 0,                             //完工数量
                    ReworkQuantity = 0,                             //返工数量
                    RepairQuantity = 0,                             //返修数量
                    CloseType = EnumWorkOrderCloseType.None,        //关闭状态
                    OrderState = EnumWorkOrderState.Open,           //工单状态      
                    Priority = EnumWorkOrderPriority.Normal,        //优先级                        
                    Description = resultERPWO.Data.Tables[0].Rows[0]["VNOTE"].ToString(),                               //备注
                    CreateType = EnumWorkOrderCreateType.ERP,       //工单创建来源
                    CreateTime = DateTime.Now,                      //创建时间
                    Creator = p.Creator,                            //创建人
                    EditTime = DateTime.Now,                        //编辑时间
                    Editor = p.Creator                              //编辑人
                };
                #endregion

                #region 3.添加工单产品
                //设置工单产品。
                WorkOrderProduct woproduct = new WorkOrderProduct()
                {
                    Key = new WorkOrderProductKey()
                    {
                        OrderNumber = resultERPWO.Data.Tables[0].Rows[0]["VBILLCODE"].ToString(),
                        MaterialCode = resultERPWO.Data.Tables[0].Rows[0]["MATERIALCODE"].ToString()
                    },
                    IsMain = true,
                    ItemNo = 0,
                    CreateTime = DateTime.Now,
                    Creator = p.Creator,
                    Editor = p.Creator,
                    EditTime = DateTime.Now,
                };
                #endregion

                #region 4.添加工单BOM及新物料
                List<Material> lstmaterial = new List<Material>();
                List<WorkOrderBOM> lstbom = new List<WorkOrderBOM>();

                //4.1取得ERP工单BOM列表
                MethodReturnResult<DataSet> resultERPBOM = GetERPWorkOrderBOM(p.OrderNumber);

                if (resultERPBOM.Data != null && resultERPBOM.Data.Tables.Count > 0)
                {
                    for (int i = 0; i < resultERPBOM.Data.Tables[0].Rows.Count; i++)
                    {
                        //1.循环创建工单BOM对象//ReplaceMaterial = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString();           //可替换料
                        WorkOrderBOM woBOM = new WorkOrderBOM()
                        {
                            Key = new WorkOrderBOMKey()
                            {
                                OrderNumber = workOrder.Key,          //工单号

                                ItemNo = Int32.Parse(resultERPBOM.Data.Tables[0].Rows[i]["序号"].ToString())          //项目号
                            },
                            MaterialCode = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString(),              //物料代码
                            Qty = Convert.ToDecimal(resultERPBOM.Data.Tables[0].Rows[i]["NUNITNUM"].ToString()),        //消耗量
                            MaterialUnit = resultERPBOM.Data.Tables[0].Rows[i]["MEAS"].ToString(),                      //计量单位
                            MinUnit = Convert.ToDecimal(resultERPBOM.Data.Tables[0].Rows[i]["MINUNIT"].ToString()),     //最小扣料单位
                            CreateTime = DateTime.Now,      //创建时间
                            Creator = p.Creator,            //创建人
                            Editor = p.Creator,             //编辑人
                            EditTime = DateTime.Now         //编辑时间
                        };

                        lstbom.Add(woBOM);

                        //2.判断BOM中的物料物料信息表中是否存在，如不存在则添加
                        Material material = MaterialDataEngine.Get(resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString());

                        if (material == null)
                        {
                            int count = lstmaterial.Count(m => m.Key == resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString());

                            if (count == 0)
                            {
                                material = new Material()
                                {
                                    Key = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString(),           //物料代码
                                    Name = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALNAME"].ToString(),          //物料名称
                                    Spec = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALSPEC"].ToString(),          //规格
                                    ModelName = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALTYPE"].ToString(),     //型号
                                    Type = resultERPBOM.Data.Tables[0].Rows[i]["MACLASSCODE"].ToString(),           //类型
                                    //Class = resultERPBOM.Data.Tables[0].Rows[i]["MACLASSNAME"].ToString(),          //物料分类
                                    Unit = resultERPBOM.Data.Tables[0].Rows[i]["MEAS"].ToString(),                  //计量单位
                                    CreateTime = DateTime.Now,
                                    EditTime = DateTime.Now,
                                    Creator = p.Creator,
                                    Editor = p.Creator,
                                    IsProduct = false,
                                    IsRaw = true,
                                    Status = EnumObjectStatus.Available,
                                    Description = ""
                                };

                                lstmaterial.Add(material);
                            }
                        }
                    }
                }
                #endregion

                #region 5.设置工单生产工艺（根据产品）
                //5.1根据生产车间取得产品加工工艺流程组
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.MaterialCode = '{0}' 
                                            AND Key.LocationName = '{1}'"
                        , workOrder.MaterialCode
                        , workOrder.LocationName)
                };

                IList<MaterialRoute> lstMaterialRoute = this.MaterialRouteDataEngine.Get(cfg);

                if (lstMaterialRoute.Count > 0)
                {
                    int j = 1;      //工单工艺流程项目号计数器

                    foreach (MaterialRoute mRoute in lstMaterialRoute)
                    {
                        //取得工艺流程组对应的工艺流程
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@"Key.RouteEnterpriseName = '{0}'"
                                , mRoute.Key.RouteEnterpriseName)
                        };

                        IList<RouteEnterpriseDetail> lstRouteEnterpriseDetail = this.RouteEnterpriseDetailDataEngine.Get(cfg);

                        if (lstRouteEnterpriseDetail.Count > 0)
                        {
                            //循环取得工艺流程
                            foreach (RouteEnterpriseDetail routeEnterpriseDetail in lstRouteEnterpriseDetail)
                            {
                                //取得工艺流程第一个工步对象
                                cfg = new PagingConfig()
                                {
                                    IsPaging = false,
                                    Where = string.Format(@"Key.RouteName = '{0}'"
                                        , routeEnterpriseDetail.Key.RouteName),
                                    OrderBy = "SortSeq"
                                };

                                IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);

                                if (lstRouteStep.Count > 0)
                                {
                                    //取得工艺流程对象,取得工艺流程类型（主流程、返修流程）
                                    Route route = this.RouteDataEngine.Get(routeEnterpriseDetail.Key.RouteName);

                                    if (route == null)
                                    {
                                        result.Code = 2001;
                                        result.Message = string.Format("工艺流程（{0}）不存在！"
                                                                        , lstRouteStep[0].Key.RouteName);
                                        return result;
                                    }

                                    bool isRework = false;

                                    if (route.RouteType == EnumRouteType.Repair)
                                    {
                                        isRework = true;
                                    }

                                    //创建工单工艺流程
                                    WorkOrderRoute woRoute = new WorkOrderRoute()
                                    {
                                        Key = new WorkOrderRouteKey()
                                        {
                                            OrderNumber = workOrder.Key,
                                            ItemNo = j
                                        },
                                        RouteEnterpriseName = routeEnterpriseDetail.Key.RouteEnterpriseName,
                                        RouteName = lstRouteStep[0].Key.RouteName,
                                        RouteStepName = lstRouteStep[0].Key.RouteStepName,
                                        Creator = p.Creator,
                                        CreateTime = DateTime.Now,
                                        Editor = p.Creator,
                                        EditTime = DateTime.Now,
                                        IsRework = isRework
                                    };

                                    lstWORoute.Add(woRoute);

                                    j++;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region 6.数据提交
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                //6.1 工单
                this.WorkOrderDataEngine.Insert(workOrder, session);

                //6.2 工单产品
                this.WorkOrderProductDataEngine.Insert(woproduct, session);

                //6.3 工单BOM
                if (lstbom.Count > 0)
                {
                    foreach (WorkOrderBOM woBOM in lstbom)
                    {
                        this.WorkOrderBOMDataEngine.Insert(woBOM, session);
                    }
                }

                //6.4 新增物料
                if (lstmaterial.Count > 0)
                {
                    foreach (Material material in lstmaterial)
                    {
                        this.MaterialDataEngine.Insert(material, session);
                    }
                }

                //6.5 工单加工工艺
                if (lstWORoute.Count > 0)
                {
                    foreach (WorkOrderRoute woRoute in lstWORoute)
                    {
                        this.WorkOrderRouteDataEngine.Insert(woRoute, session);
                    }
                }

                transaction.Commit();
                session.Close();

                #endregion
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    session.Close();
                }

                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;
        }

        /// <summary> 更新BOM、物料信息信息 </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult UpdateBaseInfo(ERPWorkOrderParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            ISession session = null;
            ITransaction transaction = null;

            try
            {
                #region 1.属性判断
                //1.检验工单是否已经存在
                WorkOrder workOrder = WorkOrderDataEngine.Get(p.OrderNumber);

                if (workOrder == null)
                {
                    result.Code = 1001;
                    result.Message = "工单[" + p.OrderNumber + "]不存在！";

                    return result;
                }               
                #endregion
                
                #region 2.添加工单BOM及新物料
                List<Material> lstmaterial = new List<Material>();
                List<MaterialType> lstmaterialTypeInsert = new List<MaterialType>();
                List<WorkOrderBOM> lstWOBom = new List<WorkOrderBOM>();
                List<WorkOrderBOMKey> lstWOBomForKeyDelete = new List<WorkOrderBOMKey>();
                List<WorkOrderBOM> lstWOBomForInsert = new List<WorkOrderBOM>();

                MethodReturnResult<IList<WorkOrderBOM>> resultBom = new MethodReturnResult<IList<WorkOrderBOM>>();
                MethodReturnResult<IList<WorkOrderBOM>> resultBomByMaterial = new MethodReturnResult<IList<WorkOrderBOM>>();

                //2.1取得ERP工单BOM列表
                MethodReturnResult<DataSet> resultERPBOM = GetERPWorkOrderBOM(p.OrderNumber);

                if (resultERPBOM.Data != null && resultERPBOM.Data.Tables.Count > 0)
                {
                    for (int i = 0; i < resultERPBOM.Data.Tables[0].Rows.Count; i++)
                    {
                        //判断BOM数据是否存在
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("Key.OrderNumber = '{0}' AND MaterialCode = '{1}' AND Key.ItemNo = {2}"
                                                    , workOrder.Key
                                                    , resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString()
                                                    , Convert.ToInt32(resultERPBOM.Data.Tables[0].Rows[i]["序号"]))
                        };

                        resultBom.Data = this.WorkOrderBOMDataEngine.Get(cfg);
                        //Bom数据不存在
                        if (resultBom.Data.Count == 0)
                        {
                            //判断BOM中该料号是否存在
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.OrderNumber = '{0}' AND MaterialCode = '{1}'"
                                                        , workOrder.Key
                                                        , resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString())
                            };
                            resultBomByMaterial.Data = this.WorkOrderBOMDataEngine.Get(cfg);

                            #region 1.新增BOM
                            if (resultBomByMaterial.Data.Count == 0)
                            {
                                //1.循环创建工单BOM对象//ReplaceMaterial = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString();           //可替换料
                                WorkOrderBOM woBOM = new WorkOrderBOM()
                                {
                                    Key = new WorkOrderBOMKey()
                                    {
                                        OrderNumber = workOrder.Key,          //工单号

                                        ItemNo = Int32.Parse(resultERPBOM.Data.Tables[0].Rows[i]["序号"].ToString())          //序号
                                    },
                                    MaterialCode = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString(),              //物料代码
                                    Qty = Convert.ToDecimal(resultERPBOM.Data.Tables[0].Rows[i]["NUNITNUM"].ToString()),        //消耗量
                                    MaterialUnit = resultERPBOM.Data.Tables[0].Rows[i]["MEAS"].ToString(),                      //计量单位
                                    MinUnit = Convert.ToDecimal(resultERPBOM.Data.Tables[0].Rows[i]["MINUNIT"].ToString()),     //最小扣料单位
                                    CreateTime = DateTime.Now,      //创建时间
                                    Creator = p.Creator,            //创建人
                                    Editor = p.Creator,             //编辑人
                                    EditTime = DateTime.Now         //编辑时间
                                };

                                lstWOBom.Add(woBOM);

                                //2.判断BOM中的物料物料信息表中是否存在，如不存在则添加
                                Material material = MaterialDataEngine.Get(resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString());

                                if (material == null)
                                {
                                    int count = lstmaterial.Count(m => m.Key == resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString());

                                    if (count == 0)
                                    {
                                        MaterialType materialType = MaterialTypeDataEngine.Get(resultERPBOM.Data.Tables[0].Rows[i]["MACLASSCODE"].ToString());
                                        if (materialType == null)
                                        {
                                            materialType = new MaterialType();
                                            materialType.Key = resultERPBOM.Data.Tables[0].Rows[i]["MACLASSCODE"].ToString();
                                            materialType.Description = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALNAME"].ToString();
                                            materialType.CreateTime = DateTime.Now;
                                            materialType.EditTime = DateTime.Now;
                                            materialType.Creator = p.Creator;
                                            materialType.Editor = p.Creator;

                                            lstmaterialTypeInsert.Add(materialType);
                                        }

                                        material = new Material()
                                        {
                                            Key = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString(),
                                            Name = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALNAME"].ToString(),
                                            Spec = resultERPBOM.Data.Tables[0].Rows[i]["MATERIALSPEC"].ToString(),
                                            Type = resultERPBOM.Data.Tables[0].Rows[i]["MACLASSCODE"].ToString(),
                                            Class = resultERPBOM.Data.Tables[0].Rows[i]["MACLASSNAME"].ToString(),
                                            Unit = resultERPBOM.Data.Tables[0].Rows[i]["MEAS"].ToString(),
                                            CreateTime = DateTime.Now,
                                            EditTime = DateTime.Now,
                                            Creator = p.Creator,
                                            Editor = p.Creator,
                                            IsProduct = false,
                                            IsRaw = true,
                                            Status = EnumObjectStatus.Available,
                                            Description = ""
                                        };

                                        lstmaterial.Add(material);
                                    }
                                }
                            }
                            #endregion

                            #region 2.更新BOM中料号序号信息
                            else
                            {
                                WorkOrderBOM workOrderBomDelete = resultBomByMaterial.Data[0];
                                WorkOrderBOMKey workOrderBomKeyDelete = workOrderBomDelete.Key;
                                lstWOBomForKeyDelete.Add(workOrderBomKeyDelete);

                                WorkOrderBOM workOrderBomInsert = resultBomByMaterial.Data[0];
                                WorkOrderBOMKey workOrderBomKey = new WorkOrderBOMKey()
                                {
                                    OrderNumber = workOrderBomDelete.Key.OrderNumber,
                                    ItemNo = Convert.ToInt32(resultERPBOM.Data.Tables[0].Rows[i]["序号"])
                                };
                                workOrderBomInsert.Key = workOrderBomKey;
                                workOrderBomInsert.EditTime = DateTime.Now;
                                workOrderBomInsert.Editor = p.Creator;
                                lstWOBomForInsert.Add(workOrderBomInsert);
                            }
                            #endregion
                        }
                    }
                }
                #endregion
                
                #region 3.数据提交
                session = this.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                //3.1 工单BOM新增
                if (lstWOBom.Count > 0)
                {
                    foreach (WorkOrderBOM woBOM in lstWOBom)
                    {
                        this.WorkOrderBOMDataEngine.Insert(woBOM, session);
                    }
                }

                //3.2 工单BOM更新
                if (lstWOBomForKeyDelete.Count > 0)
                {
                    foreach (WorkOrderBOMKey workOrderBomKey in lstWOBomForKeyDelete)
                    {
                        this.WorkOrderBOMDataEngine.Delete(workOrderBomKey, session);
                    }
                }
                if (lstWOBomForInsert.Count > 0)
                {
                    foreach (WorkOrderBOM workOrderBom in lstWOBomForInsert)
                    {
                        this.WorkOrderBOMDataEngine.Insert(workOrderBom, session);
                    }
                }

                //3.3 新增物料类型
                if (lstmaterialTypeInsert.Count > 0)
                {
                    foreach (MaterialType materialType in lstmaterialTypeInsert)
                    {
                        this.MaterialTypeDataEngine.Insert(materialType, session);
                    }
                }

                //3.4 新增物料
                if (lstmaterial.Count > 0)
                {
                    foreach (Material material in lstmaterial)
                    {
                        this.MaterialDataEngine.Insert(material, session);
                    }
                }

                transaction.Commit();
                session.Close();

                #endregion
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    session.Close();
                }

                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;

            //MethodReturnResult<IList<WorkOrderBOM>> resultBom = new MethodReturnResult<IList<WorkOrderBOM>>();
            //MethodReturnResult result = new MethodReturnResult();
            //MethodReturnResult<DataSet> ds_bom = GetERPWorkOrderBOM(p.OrderNumber);
            //List<Material> lstmaterial = new List<Material>();
            //List<WorkOrderBOM> lstbom = new List<WorkOrderBOM>();
            //if (ds_bom.Data != null && ds_bom.Data.Tables[0].Rows.Count > 0)
            //{
            //    for (int i = 0; i < ds_bom.Data.Tables[0].Rows.Count; i++)
            //    {
            //            PagingConfig cfg = new PagingConfig()
            //            {
            //                IsPaging = false,
            //                //OrderBy = "SortSeq",
            //                Where = string.Format("Key.OrderNumber='{0}' AND MaterialCode='{1}'"
            //                                        , p.OrderNumber, ds_bom.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString())
            //            };
            //     resultBom.Data = this.WorkOrderBOMDataEngine.Get(cfg);
            //        //根据工单号及物料编码查找该工单BOM数据是否存在，如果不存在，则添加BOM到该工单即可
            //            if (resultBom.Data.Count <= 0)
            //            {

            //                WorkOrderBOM bom = new WorkOrderBOM()
            //                {
            //                    Key = new WorkOrderBOMKey()
            //                    {
            //                        OrderNumber = ds_bom.Data.Tables[0].Rows[i]["VSOURCEMOCODE"].ToString(),
            //                        ItemNo = i + 1
            //                    },
            //                    MaterialCode = ds_bom.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString(),
            //                    MaterialUnit = ds_bom.Data.Tables[0].Rows[i]["MEAS"].ToString(),
            //                    Qty = Convert.ToDecimal(ds_bom.Data.Tables[0].Rows[i]["NUNITNUM"].ToString()),
            //                    Description = ds_bom.Data.Tables[0].Rows[i]["MATERIALNAME"].ToString(),
            //                    CreateTime = DateTime.Now,
            //                    Creator = p.Creator,
            //                    Editor = p.Creator,
            //                    EditTime = DateTime.Now
            //                };
            //                lstbom.Add(bom);
            //                //判断BOM中的物料物料信息表中是否存在，如果不存在，则添加
            //                Material material = MaterialDataEngine.Get(ds_bom.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString());
            //                if (material == null)
            //                {
            //                    int count = lstmaterial.Count(m => m.Key == ds_bom.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString());

            //                    if (count == 0)
            //                    {
            //                        material = new Material()
            //                        {
            //                            Key = ds_bom.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString(),
            //                            Name = ds_bom.Data.Tables[0].Rows[i]["MATERIALNAME"].ToString(),
            //                            Spec = ds_bom.Data.Tables[0].Rows[i]["MATERIALSPEC"].ToString(),
            //                            Type = ds_bom.Data.Tables[0].Rows[i]["MACLASSCODE"].ToString(),
            //                            Class = ds_bom.Data.Tables[0].Rows[i]["MACLASSNAME"].ToString(),
            //                            Unit = ds_bom.Data.Tables[0].Rows[i]["MEAS"].ToString(),
            //                            CreateTime = DateTime.Now,
            //                            EditTime = DateTime.Now,
            //                            Creator = p.Creator,
            //                            Editor = p.Creator,
            //                            IsProduct = false,
            //                            IsRaw = true,
            //                            Status = EnumObjectStatus.Available,
            //                            Description = ""

            //                        };
            //                        lstmaterial.Add(material);
            //                    }
            //                }

            //            }
            //    }
            //}
            //ISession db = this.SessionFactory.OpenSession();
            //ITransaction transaction = db.BeginTransaction();
            //try
            //{
            //    if (lstmaterial.Count > 0)
            //    {
            //        foreach (Material item in lstmaterial)
            //        {
            //            this.MaterialDataEngine.Insert(item, db);
            //        }
            //    }
            //    if (lstbom.Count > 0)
            //    {
            //        foreach (WorkOrderBOM item in lstbom)
            //        {
            //            this.WorkOrderBOMDataEngine.Insert(item, db);
            //        }
            //    }

            //    transaction.Commit();
            //    db.Close();
            //}
            //catch (Exception ex)
            //{
            //    transaction.Rollback();
            //    db.Close();
            //    result.Code = 1000;
            //    result.Message += string.Format("更新工单{0}BOM失败！",p.OrderNumber);
            //    result.Detail = ex.ToString();
            //}
            //return result;
        }

        /// <summary> 从ERP获取领料单信息 </summary>
        /// <param name="ReceiptNO">领料单号</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPMaterialReceipt(string ReceiptNO)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select t1.vbillcode, t1.dbilldate, t2.vproductbatch
                                                       from " + ErpDBName + ".v_ic_material_h t1 " + 
                                                      "left join " + ErpDBName + ".v_ic_material_b t2 " +
                                                      "  on t2.groupcode = '{0}' and t2.orgcode='{1}' and t2.imhcode = t1.vbillcode " +
                                                      "where t1.groupcode = '{0}' and t1.orgcode='{1}'" +
                                                      "group by t1.vbillcode, t1.dbilldate, t2.vproductbatch " + 
                                                      "having t1.vbillcode = '{2}'",
                                                      ErpGroupCode,
                                                      ErpORGCode,
                                                      ReceiptNO);

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
        /// 从ERP获取领料单明细信息
        /// </summary>
        /// <param name="ReceiptNO">领料单号</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPMaterialReceiptDetail(string ReceiptNO)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandText = string.Format(@" select imhcode,t1.crowno,
                                                              t1.materialcode,
                                                              t1.vbatchcode,
                                                              sum(t1.nnum) as nnum,
                                                              t1.suppliercode,
                                                              t1.suppliername,
                                                              t1.sccscode,
                                                              t1.sccs,
                                                              t1.battransrate,
                                                              t1.batcolor,
                                                              t1.batlvl
                                                        from " + ErpDBName + ".v_ic_material_b t1 " +
                                                    "   where t1.groupcode = '{0}' and t1.orgcode='{1}' and imhcode = '{2}'" +
                                                    "   group by imhcode,t1.crowno,t1.materialcode, t1.vbatchcode, t1.suppliercode,t1.suppliername,t1.sccscode,t1.sccs,t1.battransrate,t1.batcolor,t1.batlvl" +
                                                    "   order by to_number(t1.crowno) asc ",
                                                    ErpGroupCode,
                                                    ErpORGCode,
                                                    ReceiptNO);

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
        /// 获取最终领料明细
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public MethodReturnResult<List<MaterialReceiptReplace>> GetMaterialReceiptReplaceDetail(MethodReturnResult<DataSet> ds_detail,int PageNo,int PageSize)
        {
            MethodReturnResult<List<MaterialReceiptReplace>> result = new MethodReturnResult<List<MaterialReceiptReplace>>();
            List<MaterialReceiptReplace> lstMaterialReceiptReplace = new List<MaterialReceiptReplace>();
            string orderNumber = "";    //工单号            

            #region 1.取得领料单表头信息
            string receiptNo = ds_detail.Data.Tables[0].Rows[0]["imhcode"].ToString();
            MaterialReceipt materialReceipt = new MaterialReceipt();                       
            MethodReturnResult<DataSet> ds_materialReceipt = GetERPMaterialReceipt(receiptNo);
            if (ds_materialReceipt.Data.Tables[0].Rows.Count > 0)
            {
                materialReceipt = new MaterialReceipt()
                {
                    Key = ds_materialReceipt.Data.Tables[0].Rows[0]["VBILLCODE"].ToString(),
                    OrderNumber = ds_materialReceipt.Data.Tables[0].Rows[0]["VPRODUCTBATCH"].ToString(),
                    ReceiptDate = DateTime.Parse(ds_materialReceipt.Data.Tables[0].Rows[0]["DBILLDATE"].ToString())
                };
            }

            //取得工单信息
            WorkOrder workorder = WorkOrderDataEngine.Get(materialReceipt.OrderNumber);
            if (workorder == null)
            {
                result.Code = 1001;
                result.Message = "工单[" + materialReceipt.OrderNumber + "]不存在！";
                return result;
            }
            orderNumber = workorder.Key;
            #endregion

            #region 2.取得领料单表体数据
            if (ds_detail.Data != null && ds_detail.Data.Tables[0].Rows.Count > 0)
            {
                int count = 0;
                if (PageNo == 0 && PageSize == 20)
                {
                    count = ds_detail.Data.Tables[0].Rows.Count;
                }
                else
                {
                    count = (PageNo + 1) * PageSize;
                }
                for (int i = PageNo * PageSize; i < count; i++)
                {
                    string materialCode = "";   //现物料编码
                    string materialLot = "";    //现物料批号
                    double qty = 0;             //数量
                    string cellPower = "";      //电池片效率
                    string cellColor = "";      //现电池片颜色
                    string cellGrade = "";      //电池片等级
                    string oldSupplierCode = "";//原供应商代码
                    string oldSupplierName = "";//原供应商名称
                    string oldMaterialCode = "";//原物料编码
                    string oldCellColor = "";   //原电池片颜色
                    string oldMaterialLot = ""; //原物料批号
                    string supplierCode = "";   //供应商代码
                    string supplierName = "";   //供应商名称
                    string manufacturerCode = "";//生产厂商代码
                    string manufacturerName = "";//生产厂商名称
                    string oldManufacturerCode = "";//原生产厂商代码
                    string oldManufacturerName = "";//原生产厂商名称
                    string supplierMaterialLot = "";//供应商物料批号
                    string description = "";    //描述

                    #region 2.1.供应商及生产厂商信息

                    //ERP供应商代码
                    oldSupplierCode = ds_detail.Data.Tables[0].Rows[i]["SUPPLIERCODE"].ToString();
                    //ERP供应商名称
                    oldSupplierName = ds_detail.Data.Tables[0].Rows[i]["SUPPLIERNAME"].ToString();

                    if (oldSupplierCode == null || oldSupplierName == null)
                    {
                        oldSupplierCode = "";
                        oldSupplierName = "";
                    }

                    //ERP生产厂商代码
                    oldManufacturerCode = ds_detail.Data.Tables[0].Rows[i]["SCCSCODE"].ToString();
                    //ERP生产厂商名称
                    oldManufacturerName = ds_detail.Data.Tables[0].Rows[i]["SCCS"].ToString();

                    if (oldManufacturerCode == null || oldManufacturerName == null)
                    {
                        oldManufacturerCode = "";
                        oldManufacturerName = "";
                    }

                    #region 2.1.1.根据物料替换规则进行ERP物料替换
                    oldMaterialCode = ds_detail.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString();
                    MethodReturnResult<IList<MaterialReplace>> MaterialReplaceRule = GetMaterialReplaceRule(workorder.MaterialCode);
                    if (MaterialReplaceRule.Data.Count > 0)
                    {
                        for (int k = 0; k < MaterialReplaceRule.Data.Count; k++)
                        {
                            //如果工单设置为*
                            if (MaterialReplaceRule.Data[k].Key.OrderNumber == "*")
                            {
                                //如果原供应商代码设置为*
                                if (MaterialReplaceRule.Data[k].Key.OldMaterialSupplier == "*")
                                {
                                    if (workorder.MaterialCode == MaterialReplaceRule.Data[k].Key.ProductCode
                                    && oldMaterialCode == MaterialReplaceRule.Data[k].Key.OldMaterialCode)
                                    {
                                        supplierCode = MaterialReplaceRule.Data[k].NewMaterialSupplier;     //现供应商代码  
                                        MethodReturnResult<DataSet> ds_supplier = GetERPSupplier(supplierCode);
                                        if (ds_supplier != null && ds_supplier.Data.Tables[0].Rows.Count > 0)
                                        {
                                            supplierName = ds_supplier.Data.Tables[0].Rows[0]["CUSNAME"].ToString();             //现供应商名称                                               
                                            MethodReturnResult<DataSet> ds_manufacturer = GetByNameERPManufacturer(supplierName);
                                            if (ds_manufacturer != null && ds_manufacturer.Data.Tables[0].Rows.Count > 0)
                                            {
                                                manufacturerName = supplierName;                                                     //现生产厂商名称
                                                manufacturerCode = ds_manufacturer.Data.Tables[0].Rows[0]["CSCODE"].ToString();      //现生产厂商代码
                                            }
                                        }
                                        materialCode = MaterialReplaceRule.Data[k].NewMaterialCode;         //现物料编码
                                    }
                                }
                                //如果原供应商代码设置不为*
                                else
                                {
                                    if (workorder.MaterialCode == MaterialReplaceRule.Data[k].Key.ProductCode
                                    && oldMaterialCode == MaterialReplaceRule.Data[k].Key.OldMaterialCode
                                    && oldSupplierCode == MaterialReplaceRule.Data[k].Key.OldMaterialSupplier)
                                    {
                                        supplierCode = MaterialReplaceRule.Data[k].NewMaterialSupplier;     //现供应商代码  
                                        MethodReturnResult<DataSet> ds_supplier = GetERPSupplier(supplierCode);
                                        if (ds_supplier != null && ds_supplier.Data.Tables[0].Rows.Count > 0)
                                        {
                                            supplierName = ds_supplier.Data.Tables[0].Rows[0]["CUSNAME"].ToString();             //现供应商名称                                               
                                            MethodReturnResult<DataSet> ds_manufacturer = GetByNameERPManufacturer(supplierName);
                                            if (ds_manufacturer != null && ds_manufacturer.Data.Tables[0].Rows.Count > 0)
                                            {
                                                manufacturerName = supplierName;                                                    //现生产厂商名称
                                                manufacturerCode = ds_manufacturer.Data.Tables[0].Rows[0]["CSCODE"].ToString();     //现生产厂商代码
                                            }
                                        }
                                        materialCode = MaterialReplaceRule.Data[k].NewMaterialCode;         //现物料编码
                                    }
                                }
                            }
                            //如果工单设置不为*
                            else
                            {
                                //如果原供应商代码设置为*
                                if (MaterialReplaceRule.Data[k].Key.OldMaterialSupplier == "*")
                                {
                                    if (workorder.MaterialCode == MaterialReplaceRule.Data[k].Key.ProductCode
                                    && workorder.Key == MaterialReplaceRule.Data[k].Key.OrderNumber
                                    && oldMaterialCode == MaterialReplaceRule.Data[k].Key.OldMaterialCode)
                                    {
                                        supplierCode = MaterialReplaceRule.Data[k].NewMaterialSupplier;     //现供应商代码  
                                        MethodReturnResult<DataSet> ds_supplier = GetERPSupplier(supplierCode);
                                        if (ds_supplier != null && ds_supplier.Data.Tables[0].Rows.Count > 0)
                                        {
                                            supplierName = ds_supplier.Data.Tables[0].Rows[0]["CUSNAME"].ToString();             //现供应商名称                                               
                                            MethodReturnResult<DataSet> ds_manufacturer = GetByNameERPManufacturer(supplierName);
                                            if (ds_manufacturer != null && ds_manufacturer.Data.Tables[0].Rows.Count > 0)
                                            {
                                                manufacturerName = supplierName;                                                    //现生产厂商名称
                                                manufacturerCode = ds_manufacturer.Data.Tables[0].Rows[0]["CSCODE"].ToString();      //现生产厂商代码
                                            }
                                        }
                                        materialCode = MaterialReplaceRule.Data[k].NewMaterialCode;         //现物料编码
                                    }
                                }
                                //如果原供应商代码设置不为*
                                else
                                {
                                    if (workorder.MaterialCode == MaterialReplaceRule.Data[k].Key.ProductCode
                                    && workorder.Key == MaterialReplaceRule.Data[k].Key.OrderNumber
                                    && oldMaterialCode == MaterialReplaceRule.Data[k].Key.OldMaterialCode
                                    && oldSupplierCode == MaterialReplaceRule.Data[k].Key.OldMaterialSupplier)
                                    {
                                        supplierCode = MaterialReplaceRule.Data[k].NewMaterialSupplier;     //现供应商代码  
                                        MethodReturnResult<DataSet> ds_supplier = GetERPSupplier(supplierCode);
                                        if (ds_supplier != null && ds_supplier.Data.Tables[0].Rows.Count > 0)
                                        {
                                            supplierName = ds_supplier.Data.Tables[0].Rows[0]["CUSNAME"].ToString();             //现供应商名称                                               
                                            MethodReturnResult<DataSet> ds_manufacturer = GetByNameERPManufacturer(supplierName);
                                            if (ds_manufacturer != null && ds_manufacturer.Data.Tables[0].Rows.Count > 0)
                                            {
                                                manufacturerName = supplierName;                                                     //现生产厂商名称
                                                manufacturerCode = ds_manufacturer.Data.Tables[0].Rows[0]["CSCODE"].ToString();      //现生产厂商代码
                                            }
                                        }
                                        materialCode = MaterialReplaceRule.Data[k].NewMaterialCode;         //现物料编码
                                    }
                                }
                            }
                        }
                    }
                    #endregion                                       

                    #endregion

                    #region 2.2.物料批号及数量信息
                    oldMaterialLot = ds_detail.Data.Tables[0].Rows[i]["VBATCHCODE"].ToString();
                    if (localName == "K01")
                    {
                        //特殊物料批号变更
                        if (workorder.MaterialCode == "1202020126" || workorder.MaterialCode == "1202020129")
                        {
                            if (oldMaterialCode == "1303040070" || oldMaterialCode == "1303040014" || oldMaterialCode == "1303040058" || oldMaterialCode == "1303040052")
                            {
                                if (oldMaterialLot.Length == 13)
                                {
                                    string prefix = "2-09430.50";
                                    if (oldMaterialCode == "1303040070" || oldMaterialCode == "1303040014")
                                    {
                                        prefix += "974";
                                        materialLot = prefix + oldMaterialLot.Substring(3, 6) + "B01" + oldMaterialLot.Substring(0, 3) + oldMaterialLot.Substring(10, 1) + "-" + oldMaterialLot.Substring(11, 2);
                                    }
                                    else if (oldMaterialCode == "1303040052")
                                    {
                                        prefix += "980";
                                        materialLot = prefix + oldMaterialLot.Substring(3, 6) + "B00" + oldMaterialLot.Substring(0, 3) + oldMaterialLot.Substring(10, 1) + "-" + oldMaterialLot.Substring(11, 2);
                                    }
                                    else if (oldMaterialCode == "1303040058")
                                    {
                                        prefix += "980";
                                        materialLot = prefix + oldMaterialLot.Substring(3, 6) + "A00" + oldMaterialLot.Substring(0, 3) + oldMaterialLot.Substring(10, 1) + "-" + oldMaterialLot.Substring(11, 2);
                                    }
                                }
                            }
                        }
                    }                    
                    qty = Convert.ToDouble(ds_detail.Data.Tables[0].Rows[i]["NNUM"].ToString());
                    #endregion

                    #region 2.3.电池片效率/等级/颜色信息
                    cellPower = ds_detail.Data.Tables[0].Rows[i]["BATTRANSRATE"].ToString();          //效率档 t1.battransrate
                    cellGrade = ds_detail.Data.Tables[0].Rows[i]["BATLVL"].ToString();                //等级  t1.batlvl
                    oldCellColor = ds_detail.Data.Tables[0].Rows[i]["BATCOLOR"].ToString();           //花色  t1.batcolor

                    #region 2.3.1.黑硅料号颜色变更规则
                    if (materialCode == "1101020511" || materialCode == "1101020512" || materialCode == "1101020515")
                    {
                        if (oldCellColor == "深蓝2")
                        {
                            cellColor = "深蓝1";
                        }
                        else if (oldCellColor == "正蓝3" || oldCellColor == "正蓝4")
                        {
                            cellColor = "深蓝2";
                        }
                        else if (oldCellColor == "浅蓝5" || oldCellColor == "浅蓝6")
                        {
                            cellColor = "正蓝4";
                        }
                    }                     
                    #endregion

                    #region 2.3.2.外购单晶电池片料号：1101010013 颜色变更规则
                    if (localName == "K01")
                    {
                        if (materialCode == "1101010013")
                        {
                            if (oldCellColor == "正蓝3" || oldCellColor == "深蓝1")
                            {
                                cellColor = "深蓝2";
                            }
                        }
                    }
                    #endregion

                    #region 2.3.3.外购单晶电池片料号：1803360002/1102020130/1102020131 颜色变更规则
                    if (localName == "K01")
                    {
                        if (materialCode == "1803360002" || materialCode == "1102020130" || materialCode == "1102020131")
                        {
                            if (oldCellColor == "淡蓝1" || oldCellColor == "深蓝2")
                            {
                                cellColor = "正蓝4";
                            }
                            if (oldCellColor == "蓝1" || oldCellColor == "淡蓝2")
                            {
                                cellColor = "正蓝3";
                            }
                            if (oldCellColor == "深蓝1")
                            {
                                cellColor = "深蓝1";
                            }
                            if (oldCellColor == "蓝2")
                            {
                                cellColor = "深蓝2";
                            }
                        }
                    }
                    #endregion

                    #endregion

                    #region 2.4.当前领料项目最终明细
                    MaterialReceiptReplace detail = new MaterialReceiptReplace()
                    {
                        Key = i + 1,
                        OrderNumber = orderNumber,
                        MaterialCode = materialCode,
                        MaterialLot = materialLot,
                        Qty = qty,
                        CellPower = cellPower,
                        CellColor = cellColor,
                        CellGrade = cellGrade,
                        OldSupplierCode = oldSupplierCode,
                        OldSupplierName = oldSupplierName,
                        OldMaterialCode = oldMaterialCode,
                        OldCellColor = oldCellColor,
                        OldMaterialLot = oldMaterialLot,
                        SupplierCode = supplierCode,
                        SupplierName = supplierName,
                        ManufacturerCode = manufacturerCode,
                        ManufacturerName = manufacturerName,
                        OldManufacturerCode = oldManufacturerCode,
                        OldManufacturerName = oldManufacturerName,
                        SupplierMaterialLot = supplierMaterialLot,
                        Description = description
                    };
                    lstMaterialReceiptReplace.Add(detail);
                    #endregion                    
                }
            }
            #endregion

            #region 3.生成最终领料明细
            if (lstMaterialReceiptReplace != null && lstMaterialReceiptReplace.Count > 0)
            {
                result.Code = 0;
                result.Data = lstMaterialReceiptReplace;
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 获取物料替换表规则明细
        /// </summary>
        /// <returns></returns>
        public MethodReturnResult<IList<MaterialReplace>> GetMaterialReplaceRule(string productCode)
        {
            MethodReturnResult<IList<MaterialReplace>> result = new MethodReturnResult<IList<MaterialReplace>>();
            PagingConfig cfg = new PagingConfig
            {
                IsPaging = false,
                Where = string.Format("Key.ProductCode = '{0}'", productCode)
            };
            try
            {
                result.Data = this.MaterialReplaceDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(@"错误：{0}", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 根据供应商名称获取供应商信息
        /// </summary>
        /// <param name="supplierName"></param>
        /// <returns></returns>
        public MethodReturnResult<IList<Supplier>> GetSupplierDetail(string supplierName)
        {
            MethodReturnResult<IList<Supplier>> result = new MethodReturnResult<IList<Supplier>>();
            PagingConfig cfg = new PagingConfig
            {
                IsPaging = false,
                Where = string.Format("Name = '{0}'", supplierName)
            };
            try
            {
                result.Data = this.SupplierDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(@"错误：{0}", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 根据生产厂商名称获取生产厂商信息
        /// </summary>
        /// <param name="supplierName"></param>
        /// <returns></returns>
        public MethodReturnResult<IList<Manufacturer>> GetManufacturerDetail(string manufacturerName)
        {
            MethodReturnResult<IList<Manufacturer>> result = new MethodReturnResult<IList<Manufacturer>>();
            PagingConfig cfg = new PagingConfig
            {
                IsPaging = false,
                Where = string.Format("Name = '{0}'", manufacturerName)
            };
            try
            {
                result.Data = this.ManufacturerDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(@"错误：{0}", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 新增ERP领料单及明细
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult AddERPMaterialReceipt(ERPMaterialReceiptParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            string sManufacturerCode = "";                          //生产厂商编码
            string sManufacturerName = "";                          //生产厂商名称
            string sSupplierCode = "";                              //供应商编码
            string sSupplierName = "";                              //供应商名称
            string sOldSupplierCode = "";                           //原供应商编码
            string sOldMaterialCode = "";                           //原物料编码

            try
            {
                List<Package> lstPackageForUpdate = new List<Package>();
                List<MaterialReceiptDetail> lstdetail = new List<MaterialReceiptDetail>();
                List<LineStoreMaterialDetail> lstlsmdInsert = new List<LineStoreMaterialDetail>();
                List<LineStoreMaterialDetail> lstlsmdUpdate = new List<LineStoreMaterialDetail>();
                List<Supplier> lstsupplierInsert = new List<Supplier>();
                List<Manufacturer> lstmanufacturerInsert = new List<Manufacturer>();
                List<SupplierToManufacturer> lsSupplierToManufacturerInsert = new List<SupplierToManufacturer>();
                Package packageObj = null;
                string packageNo = "";
                DateTime now = DateTime.Now;
                bool bpackageisfind = false;
                string materialLot = "";

                #region 1.判断领料单是否存在
                MaterialReceipt materialReceipt = this.MaterialReceiptDataEngine.Get(p.ReceiptNo);
                if (materialReceipt != null)
                {
                    result.Code = 1001;
                    result.Message = "领料单[" + p.ReceiptNo + "]已导入MES！";

                    return result;
                }
                #endregion

                #region 2.取得领料单表头信息
                MethodReturnResult<DataSet> ds_materialReceipt = GetERPMaterialReceipt(p.ReceiptNo);

                if (ds_materialReceipt.Data.Tables[0].Rows.Count > 0)
                {
                    materialReceipt = new MaterialReceipt()
                    {
                        Key = ds_materialReceipt.Data.Tables[0].Rows[0]["VBILLCODE"].ToString(),
                        OrderNumber = ds_materialReceipt.Data.Tables[0].Rows[0]["VPRODUCTBATCH"].ToString(),
                        ReceiptDate = DateTime.Parse(ds_materialReceipt.Data.Tables[0].Rows[0]["DBILLDATE"].ToString()),
                        Description = p.Description,
                        CreateTime = now,
                        EditTime = now,
                        Creator = p.Creator,
                        Editor = p.Creator
                    };
                }

                //取得工单信息
                WorkOrder workorder = WorkOrderDataEngine.Get(materialReceipt.OrderNumber);

                if (workorder == null)
                {
                    result.Code = 1001;
                    result.Message = "工单[" + materialReceipt.OrderNumber + "]不存在！";
                    return result;
                }
                #endregion

                #region 3.取得领料单表体数据
                MethodReturnResult<DataSet> ds_detail = GetERPMaterialReceiptDetail(p.ReceiptNo);

                if (ds_detail.Data != null && ds_detail.Data.Tables[0].Rows.Count > 0)
                {
                    string materialCode = "";

                    for (int i = 0; i < ds_detail.Data.Tables[0].Rows.Count; i++)
                    {
                        #region 3.1. 处理返工工单（产成品）
                        if (workorder.OrderType == "2")
                        {
                            #region 3.1.1 返回归档托数据
                            Material material = MaterialDataEngine.Get(ds_detail.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString());

                            if (material == null)
                            {
                                result.Code = 1002;
                                result.Message = "物料[" + ds_detail.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString() + "]在系统中不存在！";

                                return result;
                            }

                            if (material.IsProduct)
                            {
                                packageNo = ds_detail.Data.Tables[0].Rows[i]["VBATCHCODE"].ToString();

                                bpackageisfind = false;

                                //判断托记录已经存在
                                for (int j = 0; j < lstPackageForUpdate.Count; j++)
                                {
                                    if (lstPackageForUpdate[j].Key == packageNo)
                                    {
                                        bpackageisfind = true;

                                        break;
                                    }
                                }

                                if (bpackageisfind == false)
                                {
                                    //取得包装对象
                                    packageObj = this.PackageDataEngine.Get(packageNo);

                                    //当包装对象在当前表不存在时，从历史数据库提取数据
                                    if (packageObj == null)
                                    {
                                        //返回已归档的数据WIP_PACKAGE表的数据
                                        REbackdataParameter pre = new REbackdataParameter();
                                        pre.PackageNo = packageNo;
                                        pre.ErrorMsg = "";
                                        pre.ReType = 1;
                                        pre.IsDelete = 0;

                                        result = GetREbackdata(pre);

                                        if (result.Code > 0)
                                        {
                                            return result;
                                        }

                                        //重新取得包装对象
                                        packageObj = this.PackageDataEngine.Get(packageNo);

                                        if (packageObj == null)
                                        {
                                            result.Code = 1005;
                                            result.Message = string.Format("托[{0}]提取失败！", packageNo);
                                            return result;
                                        }
                                    }

                                    #region 3.1.2 更新包装状态
                                    packageObj.PackageState = EnumPackageState.InFabStore;      //托状态线别仓待投料状态
                                    packageObj.OrderNumber = workorder.Key;                     //工单号

                                    lstPackageForUpdate.Add(packageObj);
                                    #endregion
                                }                                
                            }
                            #endregion                            
                        }
                        #endregion

                        #region 3.2.供应商及生产厂商信息

                        //ERP供应商代码
                        sSupplierCode = ds_detail.Data.Tables[0].Rows[i]["SUPPLIERCODE"].ToString();
                        //ERP供应商名称
                        sSupplierName = ds_detail.Data.Tables[0].Rows[i]["SUPPLIERNAME"].ToString();

                        if (sSupplierCode == null || sSupplierName == null)
                        {
                            sSupplierCode = "";
                            sSupplierName = "";
                        }

                        //ERP生产厂商代码
                        sManufacturerCode = ds_detail.Data.Tables[0].Rows[i]["SCCSCODE"].ToString();
                        //ERP生产厂商名称
                        sManufacturerName = ds_detail.Data.Tables[0].Rows[i]["SCCS"].ToString();

                        if (sManufacturerCode == null || sManufacturerName == null)
                        {
                            sManufacturerCode = "";
                            sManufacturerName = "";
                        }

                        #region 3.2.1根据物料替换规则进行ERP物料替换
                        materialCode = ds_detail.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString();
                        sOldMaterialCode = materialCode;                                     //记录原有物料编码
                        MethodReturnResult<IList<MaterialReplace>> MaterialReplaceRule = GetMaterialReplaceRule(workorder.MaterialCode);
                        if (MaterialReplaceRule.Data.Count > 0)
                        {
                            for (int k = 0; k < MaterialReplaceRule.Data.Count; k++)
                            {
                                //如果工单设置为*
                                if (MaterialReplaceRule.Data[k].Key.OrderNumber == "*")
                                {
                                    //如果原供应商代码设置为*
                                    if (MaterialReplaceRule.Data[k].Key.OldMaterialSupplier == "*")
                                    {
                                        if (workorder.MaterialCode == MaterialReplaceRule.Data[k].Key.ProductCode
                                        && materialCode == MaterialReplaceRule.Data[k].Key.OldMaterialCode)
                                        {
                                            sOldSupplierCode = sSupplierCode;                                    //记录原有供应商代码
                                            sSupplierCode = MaterialReplaceRule.Data[k].NewMaterialSupplier;     //现供应商代码  
                                            MethodReturnResult<DataSet> ds_supplier = GetERPSupplier(sSupplierCode);
                                            if (ds_supplier != null && ds_supplier.Data.Tables[0].Rows.Count > 0)
                                            {
                                                sSupplierName = ds_supplier.Data.Tables[0].Rows[0]["CUSNAME"].ToString();             //现供应商名称                                               
                                                MethodReturnResult<DataSet> ds_manufacturer = GetByNameERPManufacturer(sSupplierName);
                                                if (ds_manufacturer != null && ds_manufacturer.Data.Tables[0].Rows.Count > 0)
                                                {
                                                    sManufacturerName = sSupplierName;                                                    //现生产厂商名称
                                                    sManufacturerCode = ds_manufacturer.Data.Tables[0].Rows[0]["CSCODE"].ToString();      //现生产厂商代码
                                                }
                                                else
                                                {
                                                    sManufacturerName = "";
                                                    sManufacturerCode = "";
                                                }
                                            }          
                                            
                                            materialCode = MaterialReplaceRule.Data[k].NewMaterialCode;          //现物料编码
                                        }
                                    }
                                    //如果原供应商代码设置不为*
                                    else
                                    {
                                        if (workorder.MaterialCode == MaterialReplaceRule.Data[k].Key.ProductCode
                                        && materialCode == MaterialReplaceRule.Data[k].Key.OldMaterialCode
                                        && sSupplierCode == MaterialReplaceRule.Data[k].Key.OldMaterialSupplier)
                                        {
                                            sOldSupplierCode = sSupplierCode;                                    //记录原有供应商代码
                                            sSupplierCode = MaterialReplaceRule.Data[k].NewMaterialSupplier;     //现供应商代码  
                                            MethodReturnResult<DataSet> ds_supplier = GetERPSupplier(sSupplierCode);
                                            if (ds_supplier != null && ds_supplier.Data.Tables[0].Rows.Count > 0)
                                            {
                                                sSupplierName = ds_supplier.Data.Tables[0].Rows[0]["CUSNAME"].ToString();             //现供应商名称                                               
                                                MethodReturnResult<DataSet> ds_manufacturer = GetByNameERPManufacturer(sSupplierName);
                                                if (ds_manufacturer != null && ds_manufacturer.Data.Tables[0].Rows.Count > 0)
                                                {
                                                    sManufacturerName = sSupplierName;                                                    //现生产厂商名称
                                                    sManufacturerCode = ds_manufacturer.Data.Tables[0].Rows[0]["CSCODE"].ToString();      //现生产厂商代码
                                                }
                                                else
                                                {
                                                    sManufacturerName = "";
                                                    sManufacturerCode = "";
                                                }
                                            }                                            

                                            materialCode = MaterialReplaceRule.Data[k].NewMaterialCode;          //现物料编码
                                        }
                                    }
                                }
                                //如果工单设置不为*
                                else
                                {
                                    //如果原供应商代码设置为*
                                    if (MaterialReplaceRule.Data[k].Key.OldMaterialSupplier == "*")
                                    {
                                        if (workorder.MaterialCode == MaterialReplaceRule.Data[k].Key.ProductCode
                                        && workorder.Key == MaterialReplaceRule.Data[k].Key.OrderNumber
                                        && materialCode == MaterialReplaceRule.Data[k].Key.OldMaterialCode)
                                        {
                                            sOldSupplierCode = sSupplierCode;                                    //记录原有供应商代码
                                            sSupplierCode = MaterialReplaceRule.Data[k].NewMaterialSupplier;     //现供应商代码  
                                            MethodReturnResult<DataSet> ds_supplier = GetERPSupplier(sSupplierCode);
                                            if (ds_supplier != null && ds_supplier.Data.Tables[0].Rows.Count > 0)
                                            {
                                                sSupplierName = ds_supplier.Data.Tables[0].Rows[0]["CUSNAME"].ToString();             //现供应商名称                                               
                                                MethodReturnResult<DataSet> ds_manufacturer = GetByNameERPManufacturer(sSupplierName);
                                                if (ds_manufacturer != null && ds_manufacturer.Data.Tables[0].Rows.Count > 0)
                                                {
                                                    sManufacturerName = sSupplierName;                                                    //现生产厂商名称
                                                    sManufacturerCode = ds_manufacturer.Data.Tables[0].Rows[0]["CSCODE"].ToString();      //现生产厂商代码
                                                }
                                                else
                                                {
                                                    sManufacturerName = "";
                                                    sManufacturerCode = "";
                                                }
                                            }          

                                            materialCode = MaterialReplaceRule.Data[k].NewMaterialCode;          //现物料编码
                                        }
                                    }
                                    //如果原供应商代码设置不为*
                                    else
                                    {
                                        if (workorder.MaterialCode == MaterialReplaceRule.Data[k].Key.ProductCode
                                        && workorder.Key == MaterialReplaceRule.Data[k].Key.OrderNumber
                                        && materialCode == MaterialReplaceRule.Data[k].Key.OldMaterialCode
                                        && sSupplierCode == MaterialReplaceRule.Data[k].Key.OldMaterialSupplier)
                                        {
                                            sOldSupplierCode = sSupplierCode;                                    //记录原有供应商代码
                                            sSupplierCode = MaterialReplaceRule.Data[k].NewMaterialSupplier;     //现供应商代码  
                                            MethodReturnResult<DataSet> ds_supplier = GetERPSupplier(sSupplierCode);
                                            if (ds_supplier != null && ds_supplier.Data.Tables[0].Rows.Count > 0)
                                            {
                                                sSupplierName = ds_supplier.Data.Tables[0].Rows[0]["CUSNAME"].ToString();             //现供应商名称                                               
                                                MethodReturnResult<DataSet> ds_manufacturer = GetByNameERPManufacturer(sSupplierName);
                                                if (ds_manufacturer != null && ds_manufacturer.Data.Tables[0].Rows.Count > 0)
                                                {
                                                    sManufacturerName = sSupplierName;                                                    //现生产厂商名称
                                                    sManufacturerCode = ds_manufacturer.Data.Tables[0].Rows[0]["CSCODE"].ToString();      //现生产厂商代码
                                                }
                                                else
                                                {
                                                    sManufacturerName = "";
                                                    sManufacturerCode = "";
                                                }
                                            }          

                                            materialCode = MaterialReplaceRule.Data[k].NewMaterialCode;          //现物料编码
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region 3.2.2新增供应商信息（未维护物料替换规则且MES中无该供应商信息时会新增）
                        //若供应商不存在获取ERP系统中的该物料对应供应商信息
                        if (sSupplierCode != "")
                        {
                            //根据供应商名称获取供应商信息
                            MethodReturnResult<IList<Supplier>> SupplierDetail = GetSupplierDetail(sSupplierName);                            

                            //如果ERP供应商代码在MES中存在
                            if (this.SupplierDataEngine.IsExists(sSupplierCode))
                            {
                                if (sSupplierCode != "000000")
                                {
                                    //ERP供应商名称包含在MES系统
                                    if (this.SupplierDataEngine.Get(sSupplierCode).Name.ToString().Trim() == sSupplierName.Trim())
                                    {

                                    }
                                    else
                                    {
                                        result.Code = 1001;
                                        result.Message = string.Format(@"MES系统已存在 << 供应商：({0} -- {1}) >>,
                                                           无法导入ERP供应商：<< 供应商：({0} -- {2}) >> !",
                                                                   sSupplierCode, this.SupplierDataEngine.Get(sSupplierCode).Name.ToString(), sSupplierName);
                                        return result;
                                    }
                                }                               
                            }
                            //如果ERP供应商代码在MES中不存在
                            else
                            {
                                //ERP供应商名称与MES系统一致
                                if (SupplierDetail.Data.Count > 0)
                                {
                                    result.Code = 1001;
                                    result.Message = string.Format(@"MES系统已存在 << 供应商：({0} -- {1}) >>,
                                                           无法导入ERP供应商：<< 供应商：({0} -- {2}) >> !",
                                                               sSupplierName, SupplierDetail.Data[0].Key, sSupplierCode);
                                    return result;
                                }
                                else
                                {
                                    ////取得ERP供应商信息
                                    //MethodReturnResult<DataSet> ds_supplier = GetERPSupplier(sSupplierCode);
                                    
                                    Supplier supplier = new Supplier()
                                    {
                                        //Key = ds_supplier.Data.Tables[0].Rows[0]["CUSCODE"].ToString(),
                                        //Name = ds_supplier.Data.Tables[0].Rows[0]["CUSNAME"].ToString(),
                                        Key = sSupplierCode,
                                        Name = sSupplierName,
                                        NickName = " ",
                                        CreateTime = now,
                                        EditTime = now,
                                        Creator = p.Creator,
                                        Editor = p.Creator,
                                        Description = ""
                                    };
                                   
                                        List<string> lstKey=new List<string>();
                                  
                                        foreach (Supplier item in lstsupplierInsert)
                                        {
                                            lstKey.Add(item.Key);
                                        }
                                        if (!lstKey.Contains(supplier.Key))
                                        {
                                            lstsupplierInsert.Add(supplier);
                                        }                                                                                                   
                                }
                            }
                        }
                        #endregion

                        #region 3.2.3新增生产厂商信息（未维护转换规则且MES中无该生产厂商信息时会新增）

                        //若生产厂商不存在获取ERP系统中的该物料对应生产厂商信息
                        if (sManufacturerCode != "")
                        {
                            //根据生产厂商名称获取生产厂商信息
                            MethodReturnResult<IList<Manufacturer>> ManufacturerDetail = GetManufacturerDetail(sManufacturerName);

                            //如果ERP生产厂商代码在MES中存在
                            if (this.ManufacturerDataEngine.IsExists(sManufacturerCode))
                            {
                                //ERP生产厂商名称包含在MES系统
                                if (this.ManufacturerDataEngine.Get(sManufacturerCode).Name.ToString().Trim() == sManufacturerName.Trim())
                                {

                                }
                                else
                                {
                                    result.Code = 1001;
                                    result.Message = string.Format(@"MES系统已存在 << 生产厂商：({0} -- {1}) >>,
                                                           无法导入ERP生产厂商：<< 生产厂商：({0} -- {2}) >> !",
                                                               sManufacturerCode, this.ManufacturerDataEngine.Get(sManufacturerCode).Name.ToString(), sManufacturerName);
                                    return result;
                                }
                            }
                            //如果ERP生产厂商代码在MES中不存在
                            else
                            {
                                //ERP生产厂商名称与MES系统一致
                                if (ManufacturerDetail.Data.Count > 0)
                                {
                                    result.Code = 1001;
                                    result.Message = string.Format(@"MES系统已存在 << 生产厂商：({0} -- {1}) >>,
                                                           无法导入ERP生产厂商：<< 生产厂商：({0} -- {2}) >> !",
                                                               sManufacturerName, ManufacturerDetail.Data[0].Key, sManufacturerCode);
                                    return result;
                                }
                                else
                                {
                                    Manufacturer manufacturer = new Manufacturer()
                                    {
                                        Key = sManufacturerCode,
                                        Name = sManufacturerName,
                                        NickName = " ",
                                        CreateTime = now,
                                        EditTime = now,
                                        Creator = p.Creator,
                                        Editor = p.Creator,
                                        Description = ""
                                    };

                                    List<string> lstKey = new List<string>();

                                    foreach (Manufacturer item in lstmanufacturerInsert)
                                    {
                                        lstKey.Add(item.Key);
                                    }
                                    if (!lstKey.Contains(manufacturer.Key))
                                    {
                                        lstmanufacturerInsert.Add(manufacturer);
                                    }
                                }
                            }
                        }
                        #endregion

                        #region 3.2.4新增供应商转换生产厂商规则
                        if (sManufacturerCode != null && sManufacturerCode != "")
                        {
                            MethodReturnResult<IList<SupplierToManufacturer>> ruleData = new MethodReturnResult<IList<SupplierToManufacturer>>();
                            PagingConfig cfgss = new PagingConfig()
                            {
                                Where = string.Format(@"Key.MaterialCode ='{0}'
                                                AND Key.SupplierCode='{1}'
                                                AND (Key.OrderNumber='*' OR Key.OrderNumber='{2}')"
                                                    , materialCode
                                                    , sSupplierCode
                                                    , workorder.Key)
                            };
                            ruleData.Data = this.SupplierToManufacturerDataEngine.Get(cfgss);
                            if (ruleData.Data != null && ruleData.Data.Count > 0)
                            {
                                //if (ruleData.Data[0].ManufacturerCode != sManufacturerCode)
                                //{
                                //    result.Code = 1001;
                                //    result.Message = string.Format(@"MES系统已存在转换规则：({0}:{1}:{2}--{3}),无法导入规则：（{0}:{1}:{2}--{4}）!",
                                //                               materialCode, workorder.Key, sSupplierCode, ruleData.Data[0].ManufacturerCode, sManufacturerCode);
                                //    return result;
                                //}                                
                            }
                            else
                            {
                                SupplierToManufacturer sm = new SupplierToManufacturer()
                                {
                                    Key = new SupplierToManufacturerKey()
                                    {
                                        MaterialCode = materialCode,
                                        OrderNumber = workorder.Key,
                                        SupplierCode = sSupplierCode
                                    },
                                    ManufacturerCode = sManufacturerCode,
                                    CreateTime = now,
                                    EditTime = now,
                                    Creator = p.Creator,
                                    Editor = p.Creator
                                };

                                List<string> lstKey = new List<string>();

                                foreach (SupplierToManufacturer item in lsSupplierToManufacturerInsert)
                                {
                                    lstKey.Add(item.Key.ToString());
                                }
                                if (!lstKey.Contains(sm.Key.ToString()))
                                {
                                    lsSupplierToManufacturerInsert.Add(sm);
                                }
                            }
                        }
                        #endregion

                        #endregion

                        #region 3.3.物料批号信息
                        materialLot = ds_detail.Data.Tables[0].Rows[i]["VBATCHCODE"].ToString();
                        if (localName == "K01")
                        {
                            //特殊物料批号变更
                            if (workorder.MaterialCode == "1202020126" || workorder.MaterialCode == "1202020129")
                            {
                                if (sOldMaterialCode == "1303040070" || sOldMaterialCode == "1303040014" || sOldMaterialCode == "1303040058" || sOldMaterialCode == "1303040052")
                                {
                                    if (materialLot.Length == 13)
                                    {
                                        string prefix = "2-09430.50";
                                        if (sOldMaterialCode == "1303040070" || sOldMaterialCode == "1303040014")
                                        {
                                            prefix += "974";
                                            materialLot = prefix + materialLot.Substring(3, 6) + "B01" + materialLot.Substring(0, 3) + materialLot.Substring(10, 1) + "-" + materialLot.Substring(11, 2);
                                        }
                                        else if (sOldMaterialCode == "1303040052")
                                        {
                                            prefix += "980";
                                            materialLot = prefix + materialLot.Substring(3, 6) + "B00" + materialLot.Substring(0, 3) + materialLot.Substring(10, 1) + "-" + materialLot.Substring(11, 2);
                                        }
                                        else if (sOldMaterialCode == "1303040058")
                                        {
                                            prefix += "980";
                                            materialLot = prefix + materialLot.Substring(3, 6) + "A00" + materialLot.Substring(0, 3) + materialLot.Substring(10, 1) + "-" + materialLot.Substring(11, 2);
                                        }
                                    }
                                }
                            }
                        }                        
                        #endregion

                        #region 3.4.新增领料单明细信息
                        
                        #region /**原物料编码处理**/
                        //#region ***单独处理东方日升及精工项目料号***
                        ////判断是否东方日升1202020029、精工72P项目成品料号：1202020028
                        ////if (workorder.MaterialCode == "1202020029" || workorder.MaterialCode == "1202020028")
                        //if (workorder.MaterialCode == "1202020029")
                        //{
                        //    if (materialCode == "1303110025" || materialCode == "1303110024" || materialCode == "1303110023" || materialCode == "1303110022")
                        //    {
                        //        sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //        sSupplierCode = "10019";                //舒康代码
                        //    }
                        //}

                        //if (workorder.MaterialCode == "1202020035")
                        //{
                        //    if (materialCode == "1303030012")
                        //    {
                        //        if (sSupplierCode == "10051")   //安彩
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90057";                //代码
                        //        }
                        //    }
                        //    else if (materialCode == "1101020010")
                        //    {
                        //        if (sSupplierCode == "" || sSupplierCode == "00000" || sSupplierCode == null)   //晋能
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90085";                //代码
                        //        }
                        //    }
                        //    else if (materialCode == "1303010009")
                        //    {
                        //        if (sSupplierCode == "10063")   //太阳
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90072";                //代码
                        //        }
                        //    }
                        //    else if (materialCode == "1303020059" || materialCode == "1303020060" || materialCode == "1303020061" || materialCode == "1303020062")
                        //    {
                        //        if (sSupplierCode == "10063" || sSupplierCode == "10049")   //太阳、泰力松
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90072";                //代码
                        //        }
                        //    }
                        //    else if (materialCode == "1303110028" || materialCode == "1303110029")
                        //    {
                        //        if (sSupplierCode == "90064" || sSupplierCode == "10019")   //安徽银盾斯金铝业、舒康
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90072";                //代码双宇
                        //        }
                        //    }
                        //}
                        //#endregion

                        //#region ***处理东方日升201709项目料号***
                        //if (workorder.MaterialCode == "1202020054")
                        //{
                        //    if (materialCode == "1303030012")
                        //    {
                        //        if (sSupplierCode == "10051" || sSupplierCode == "10081" || sSupplierCode == "10064")               //安彩、拓日
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90057";                //代码
                        //        }
                        //    }
                        //    else if (materialCode == "1101020508")
                        //    {
                        //        if (sSupplierCode == "" || sSupplierCode == "00000" || sSupplierCode == null)   //晋能
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90085";                //代码
                        //        }
                        //    }
                        //    else if (materialCode == "1303010015" || materialCode == "1303020070"
                        //            || materialCode == "1303020071" || materialCode == "1303020072" || materialCode == "1303020073")
                        //    {
                        //        if (sSupplierCode == "10063")   //太阳
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90072";                //代码
                        //        }
                        //    }
                        //    else if (materialCode == "1303050009")
                        //    {
                        //        if (sSupplierCode == "90061")              //
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90115";                //代码
                        //        }
                        //    }
                        //    else if (materialCode == "1303090024" || materialCode == "1303090025")
                        //    {
                        //        if (sSupplierCode == "10022")   //太阳
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "10040";                //代码
                        //        }
                        //    }
                        //    else if (materialCode == "1303110028")
                        //    {
                        //        if (sSupplierCode == "10019" || sSupplierCode == "10098")   //
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90072";                //代码

                        //            sOldMaterialCode = materialCode;
                        //            materialCode = "1303110044";
                        //        }
                        //    }
                        //    else if (materialCode == "1303110029")
                        //    {
                        //        if (sSupplierCode == "10019" || sSupplierCode == "10098")   //
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90072";                //代码

                        //            sOldMaterialCode = materialCode;
                        //            materialCode = "1303110045";
                        //        }
                        //    }
                        //    else if (materialCode == "1303110044" || materialCode == "1303110045")
                        //    {
                        //        if (sSupplierCode == "10098" || sSupplierCode == "90062" || sSupplierCode == "10108" || sSupplierCode == "10019")   //
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "90072";                //代码
                        //        }
                        //    }
                        //    else if (materialCode == "1303090019")
                        //    {
                        //        if (sSupplierCode == "10055")   //
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "10040";                //代码

                        //            sOldMaterialCode = materialCode;
                        //            materialCode = "1303090024";
                        //        }
                        //    }
                        //    else if (materialCode == "1303040007")
                        //    {
                        //        if (sSupplierCode == "10094")   //
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "10062";                //代码

                        //            sOldMaterialCode = materialCode;
                        //            materialCode = "1303040013";
                        //        }
                        //    }
                        //    else if (materialCode == "1303040020")
                        //    {
                        //        if (sSupplierCode == "10094")   //
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "10062";                //代码

                        //            sOldMaterialCode = materialCode;
                        //            materialCode = "1303040015";
                        //        }
                        //    }
                        //    else if (materialCode == "1303090012")
                        //    {
                        //        if (sSupplierCode == "10055")   //
                        //        {
                        //            sOldMaterialCode = materialCode;
                        //            materialCode = "1303090022";
                        //        }
                        //    }
                        //    else if (materialCode == "1303090013")
                        //    {
                        //        if (sSupplierCode == "10055")   //
                        //        {
                        //            sOldMaterialCode = materialCode;
                        //            materialCode = "1303090023";
                        //        }
                        //    }
                        //    else if (materialCode == "1303090006")
                        //    {
                        //        if (sSupplierCode == "10022")   //
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "10040";                //回天代码

                        //            sOldMaterialCode = materialCode;
                        //            materialCode = "1303090025";
                        //        }
                        //    }
                        //    else if (materialCode == "1303090017")
                        //    {
                        //        if (sSupplierCode == "10055")   //
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "10040";                //回天代码

                        //            sOldMaterialCode = materialCode;
                        //            materialCode = "1303090025";
                        //        }
                        //    }
                        //}
                        //#endregion

                        //#region ***处理BT项目料号***
                        //if (workorder.Key == "1MO-17080041" && workorder.MaterialCode == "1202020047")
                        //{
                        //    if (materialCode == "1303090020")
                        //    {
                        //        sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //        sSupplierCode = "10055";                //回天代码

                        //        sOldMaterialCode = materialCode;
                        //        materialCode = "1303090019";
                        //    }
                        //}
                        //#endregion

                        //#region ***处理协鑫项目料号***
                        //if (workorder.Key == "ZKX170823ZCL1001")
                        //{
                        //    if ((materialCode == "1303110039" || materialCode == "1303110040") && (sSupplierCode == "90112"))
                        //    {
                        //        sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //        sSupplierCode = "90109";                //广跃代码
                        //    }
                        //}
                        //#endregion

                        //#region ***处理晶科1201020011项目料号***
                        //if (workorder.MaterialCode == "1201020011")
                        //{
                        //    if (materialCode == "1303080015")
                        //    {
                        //        if (sSupplierCode == "10030")               //中来
                        //        {
                        //            sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //            sSupplierCode = "10060";                //代码 台虹

                        //            sOldMaterialCode = materialCode;
                        //            materialCode = "1303080005";
                        //        }
                        //    }
                        //    else if (materialCode == "1102010006" || materialCode == "1102010007" || materialCode == "1102010008" || materialCode == "1102010009" || materialCode == "1102010010" || materialCode == "1102010011" || materialCode == "1102010012")
                        //    {
                        //        sOldSupplierCode = sSupplierCode;       //记录原有供应商代码
                        //        sSupplierCode = "000000";                //代码                                
                        //    }
                        //}
                        //#endregion
                        #endregion

                        MaterialReceiptDetail detail = new MaterialReceiptDetail()
                        {
                            Key = new MaterialReceiptDetailKey()
                            {
                                ReceiptNo = p.ReceiptNo,
                                ItemNo = i + 1
                            },

                            LineStoreName = p.LineStore,
                            MaterialCode = materialCode,
                            MaterialLot = materialLot,
                            Qty = Convert.ToDouble(ds_detail.Data.Tables[0].Rows[i]["NNUM"].ToString()),
                            SupplierCode = sSupplierCode,
                            Attr1 = ds_detail.Data.Tables[0].Rows[i]["BATTRANSRATE"].ToString(),            //效率档 t1.battransrate   
                            Attr2 = ds_detail.Data.Tables[0].Rows[i]["BATCOLOR"].ToString(),                //花色  t1.batcolor
                            Attr3 = ds_detail.Data.Tables[0].Rows[i]["BATLVL"].ToString(),                  //等级  t1.batlvl
                            Attr4 = sOldSupplierCode,                                                       //记录变更前的供应商代码
                            Attr5 = sOldMaterialCode,                                                       //记录变更前的物料代码
                            CreateTime = now,
                            EditTime = now,
                            Creator = p.Creator,
                            Editor = p.Creator,
                        };

                        //foreach (MaterialReceiptDetail itemOfMaterial in lstdetail)
                        //{
                        //    if (itemOfMaterial.MaterialCode == detail.MaterialCode && itemOfMaterial.MaterialLot == detail.MaterialLot)
                        //    {
                        //        //double oldQty = itemOfMaterial.Qty;
                        //        //lstdetail.Remove(itemOfMaterial);
                        //        //detail.Qty += oldQty;
                        //        result.Code = 1001;
                        //        result.Message = "领料单[" + p.ReceiptNo + "]明细中存在重复物料批号，请检查！";

                        //        return result;
                        //    }
                        //}

                        #region 黑硅料号颜色变更规则
                        //if (workorder.MaterialCode == "1202020065")
                        //{
                            if (materialCode == "1101020511" || materialCode == "1101020512" || materialCode == "1101020515")
                            {
                                if (detail.Attr2 == "深蓝2")
                                {
                                    detail.Attr2 = "深蓝1";
                                }
                                else if (detail.Attr2 == "正蓝3" || detail.Attr2 == "正蓝4")
                                {
                                    detail.Attr2 = "深蓝2";
                                }
                                else if (detail.Attr2 == "浅蓝5" || detail.Attr2 == "浅蓝6")
                                {
                                    detail.Attr2 = "正蓝4";
                                }
                            }
                        //}                      
                        #endregion

                        #region 外购单晶电池片料号：1101010013 颜色变更规则
                        if (localName == "K01")
                        {
                            if (materialCode == "1101010013")
                            {
                                if (detail.Attr2 == "正蓝3" || detail.Attr2 == "深蓝1")
                                {
                                    detail.Attr2 = "深蓝2";
                                }
                            }
                        }                           
                        #endregion

                        #region 外购单晶电池片料号：1803360002/1102020130/1102020131 颜色变更规则
                        if (localName == "K01")
                        {
                            if (materialCode == "1803360002" || materialCode == "1102020130" || materialCode == "1102020131")
                            {
                                if (detail.Attr2 == "淡蓝1" || detail.Attr2 == "深蓝2")
                                {
                                    detail.Attr2 = "正蓝4";
                                }
                                if (detail.Attr2 == "蓝1" || detail.Attr2 == "淡蓝2")
                                {
                                    detail.Attr2 = "正蓝3";
                                }
                                if (detail.Attr2 == "深蓝1")
                                {
                                    detail.Attr2 = "深蓝1";
                                }
                                if (detail.Attr2 == "蓝2")
                                {
                                    detail.Attr2 = "深蓝2";
                                }
                            }
                        }
                        #endregion

                        lstdetail.Add(detail);
                        #endregion

                        #region 3.5.增加线边仓物料信息

                        LineStoreMaterialDetailKey lsmdKey = new LineStoreMaterialDetailKey()
                        {
                            LineStoreName = p.LineStore,
                            OrderNumber = workorder.Key,
                            MaterialCode = materialCode,
                            //MaterialCode = ds_detail.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString(),
                            MaterialLot = materialLot,
                        };

                        //根据线边仓明细记录主键取得记录数据
                        LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(lsmdKey);

                        //如果对应线边仓中无物料明细数据，则新增线边仓物料明细数据
                        if (lsmd == null)
                        {
                            //判断已有批次信息（针对批次多条领用）
                            bpackageisfind = false;

                            int j = 0;                           

                            //判断托记录已经存在
                            for (j = 0; j < lstlsmdInsert.Count; j++)
                            {
                                if (lstlsmdInsert[j].Key.MaterialLot == materialLot)
                                {
                                    bpackageisfind = true;

                                    break;
                                }
                            }

                            if (bpackageisfind)
                            {
                                //更新线边仓物料明细数据
                                lstlsmdInsert[j].ReceiveQty += Convert.ToDouble(ds_detail.Data.Tables[0].Rows[i]["NNUM"].ToString());   //接收数量       
                                lstlsmdInsert[j].CurrentQty += Convert.ToDouble(ds_detail.Data.Tables[0].Rows[i]["NNUM"].ToString());   //当前数量
                                lstlsmdInsert[j].SupplierCode = sSupplierCode;
                            }
                            else
                            {
                                lsmd = new LineStoreMaterialDetail()
                                        {
                                            Key = lsmdKey,
                                            CurrentQty = Convert.ToDouble(ds_detail.Data.Tables[0].Rows[i]["NNUM"].ToString()),
                                            ReceiveQty = Convert.ToDouble(ds_detail.Data.Tables[0].Rows[i]["NNUM"].ToString()),
                                            LoadingQty = 0,
                                            UnloadingQty = 0,
                                            ReturnQty = 0,
                                            ScrapQty = 0,
                                            SupplierCode = sSupplierCode,                                                   //供应商代码
                                            Attr1 = ds_detail.Data.Tables[0].Rows[i]["BATTRANSRATE"].ToString(),            //效率档 t1.battransrate                                        
                                            Attr2 = ds_detail.Data.Tables[0].Rows[i]["BATCOLOR"].ToString(),                //花色  t1.batcolor
                                            Attr3 = ds_detail.Data.Tables[0].Rows[i]["BATLVL"].ToString(),                  //等级  t1.batlvl
                                            CreateTime = now,
                                            EditTime = now,
                                            Creator = p.Creator,
                                            Editor = p.Creator
                                        };

                                #region 黑硅料号颜色变更规则
                                //if (workorder.MaterialCode == "1202020065")
                                //{
                                    if (materialCode == "1101020511" || materialCode == "1101020512" || materialCode == "1101020515")
                                    {
                                        if (lsmd.Attr2 == "深蓝2")
                                        {
                                            lsmd.Attr2 = "深蓝1";
                                        }
                                        else if (lsmd.Attr2 == "正蓝3" || lsmd.Attr2 == "正蓝4")
                                        {
                                            lsmd.Attr2 = "深蓝2";
                                        }
                                        else if (lsmd.Attr2 == "浅蓝5" || lsmd.Attr2 == "浅蓝6")
                                        {
                                            lsmd.Attr2 = "正蓝4";
                                        }
                                    }
                                //}                              
                                #endregion

                                #region 外购单晶电池片料号：1101010013 颜色变更规则
                                    if (localName == "K01")
                                    {
                                        if (materialCode == "1101010013")
                                        {
                                            if (lsmd.Attr2 == "正蓝3" || lsmd.Attr2 == "深蓝1")
                                            {
                                                lsmd.Attr2 = "深蓝2";
                                            }
                                        }
                                    }                                   
                                #endregion

                                lstlsmdInsert.Add(lsmd);
                            }
                        }
                        else
                        {
                            int m = 0;
                            //判断托记录已经存在
                            for (m = 0; m < lstlsmdUpdate.Count; m++)
                            {
                                if (lstlsmdUpdate[m].Key.MaterialLot == materialLot)
                                {
                                    bpackageisfind = true;

                                    break;
                                }
                            }
                            if (bpackageisfind)
                            {
                                lstlsmdUpdate[m].ReceiveQty += Convert.ToDouble(ds_detail.Data.Tables[0].Rows[i]["NNUM"].ToString());   //接收数量       
                                lstlsmdUpdate[m].CurrentQty += Convert.ToDouble(ds_detail.Data.Tables[0].Rows[i]["NNUM"].ToString());   //当前数量
                                lstlsmdUpdate[m].Editor = p.Creator;
                                lstlsmdUpdate[m].EditTime = now;
                                lstlsmdUpdate[m].SupplierCode = sSupplierCode;
                            }
                            else
                            {
                                //更新线边仓物料明细数据
                                lsmd.ReceiveQty += Convert.ToDouble(ds_detail.Data.Tables[0].Rows[i]["NNUM"].ToString());   //接收数量       
                                lsmd.CurrentQty += Convert.ToDouble(ds_detail.Data.Tables[0].Rows[i]["NNUM"].ToString());   //当前数量
                                lsmd.Editor = p.Creator;
                                lsmd.EditTime = now;
                                lsmd.SupplierCode = sSupplierCode;

                                lstlsmdUpdate.Add(lsmd);
                            }                           
                        }
                        #endregion                                               
                        
                    }
                }
                #endregion

                #region 4.执行事务处理
                ISession session = this.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                ITransaction transactions = session.BeginTransaction();

                try
                {
                    //包装信息更新
                    if (lstPackageForUpdate.Count > 0)
                    {
                        foreach (var data in lstPackageForUpdate)
                        {
                            this.PackageDataEngine.Update(data);
                        }
                    }

                    //新增供应商
                    if (lstsupplierInsert.Count > 0)
                    {
                        foreach (Supplier item in lstsupplierInsert)
                        {
                            this.SupplierDataEngine.Insert(item, session);                            
                        }
                    }

                    
                    //新增生产厂商
                    if (lstmanufacturerInsert.Count > 0)
                    {
                        foreach (Manufacturer item in lstmanufacturerInsert)
                        {
                            this.ManufacturerDataEngine.Insert(item, session);                            
                        }
                    }

                    //新增供应商转换生产厂商规则
                    if (lsSupplierToManufacturerInsert.Count > 0)
                    {
                        foreach (SupplierToManufacturer item in lsSupplierToManufacturerInsert)
                        {
                            this.SupplierToManufacturerDataEngine.Insert(item, session);
                        }
                    }

                    //领料单表头信息
                    this.MaterialReceiptDataEngine.Insert(materialReceipt, session);


                    //领料单表体信息
                    if (lstdetail.Count > 0)
                    {
                        foreach (MaterialReceiptDetail item in lstdetail)
                        {
                            this.MaterialReceiptDetailDataEngine.Insert(item, session);
                        }
                    }
                    
                    //线别仓明细新增
                    if (lstlsmdInsert.Count > 0)
                    {
                        foreach (LineStoreMaterialDetail item in lstlsmdInsert)
                        {
                            this.LineStoreMaterialDetailDataEngine.Insert(item, session);
                        }
                    }

                    //线别仓明细更新
                    if (lstlsmdUpdate.Count > 0)
                    {
                        foreach (LineStoreMaterialDetail updateitem in lstlsmdUpdate)
                        {
                            this.LineStoreMaterialDetailDataEngine.Update(updateitem, session);
                        }
                    }
                    transaction.Commit();
                    session.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();

                    result.Code = 1002;
                    result.Message = ex.Message;
                    result.Detail = ex.ToString();
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

        public MethodReturnResult<DataSet> GetERPWR(string WRCode)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_mm_wr where groupcode = '{0}' and orgcode='{1}' and pk_wr = '{2}'",
                                                    ErpGroupCode,
                                                    ErpORGCode,
                                                    WRCode);
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

        public MethodReturnResult<DataSet> GetERPWRDetail(string ObjectNumber, string WRCode, string OrderNumber)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_mm_wr where groupcode = '{0}' and orgcode='{1}' and vbbatchcode = '{2}' and pk_wr = '{3}' and VBMOBILLCODE='{4}' ",
                                                    ErpGroupCode,
                                                    ErpORGCode,
                                                    ObjectNumber, WRCode, OrderNumber);
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

        public MethodReturnResult<DataSet> GetERPWRDetailInfo(string WRCode)
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
                                                    WRCode);
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
        /// 根据工单交易类型，获取报工单交易PK值，报工单交易类型，入库单交易PK值，入库单交易类型，
        /// </summary>
        /// <param name="ObjectType">工单交易类型</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPOrderType(string ObjectType)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_transtype where groupcode = '{0}' and transtype1 = '{1}'",
                                                    ErpGroupCode,                         
                                                    ObjectType);

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
        /// 根据ERP入库单主键取得ERP入库单号
        /// </summary>
        /// <param name="INCode"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPINCodeById(string INCode)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select * from " + ErpDBName + ".v_ic_finprodin_h where groupcode = '{0}' and orgcode='{1}' and cgeneralhid = '{2}'",
                                                    ErpGroupCode,
                                                    ErpORGCode,                            
                                                    INCode);

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
        /// 查询ERP的物料类型
        /// </summary>
        /// <param name="OrderNumber">工单号</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPMaterialType()
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format("select Code,Name from " + ErpDBName + ".v_bd_marbasclass " +
                                                    "where pk_group = '{0}'" +
                                                    "order by Code ",
                                                    ErpGroupCode);

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
        /// 导入物料类型
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult AddMaterialTypeFromERP(BaseMethodParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            PagingConfig cfg = new PagingConfig
            {
                IsPaging = false,
                Where = " 1=1 "
            };
            IList<MaterialType> lstMaterialType = MaterialTypeDataEngine.Get(cfg);
            List<MaterialType> lstmaterialTypeForInsert = new List<MaterialType>();

            MethodReturnResult<DataSet> ds_MaterialType = this.GetERPMaterialType();

            if (ds_MaterialType != null && ds_MaterialType.Data.Tables[0].Rows.Count > 0)
            {
                string strMaterialTypeCode = "";
                string strMaterialTypeName = "";
                for (int i = 0; i < ds_MaterialType.Data.Tables[0].Rows.Count; i++)
                {
                    strMaterialTypeCode = ds_MaterialType.Data.Tables[0].Rows[i]["Code"].ToString();
                    strMaterialTypeName = ds_MaterialType.Data.Tables[0].Rows[i]["Name"].ToString();

                    int m = (from mType in lstMaterialType
                             where mType.Key == strMaterialTypeCode
                             select mType).Count();
                    if (m == 0)
                    {
                        MaterialType mType = new MaterialType
                        {
                            Key = strMaterialTypeCode,
                            Description = strMaterialTypeName,
                            Creator = p.Creator,
                            Editor = p.Creator
                        };
                        lstmaterialTypeForInsert.Add(mType);
                    }
                }
            }

            ISession db = this.SessionFactory.OpenSession();
            ITransaction transaction = db.BeginTransaction();
            try
            {
                if (lstmaterialTypeForInsert.Count > 0)
                {
                    foreach (MaterialType item in lstmaterialTypeForInsert)
                    {
                        this.MaterialTypeDataEngine.Insert(item, db);
                    }
                }

                transaction.Commit();
                db.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += string.Format("导入物料类型失败！错误原因：{0}", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;

        }

        MethodReturnResult Check(ReworkParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            foreach (string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);
                //判定是否存在批次记录。
                if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                {
                    result.Code = 1002;
                    result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                    return result;
                }
            }
            return result;
        }

        MethodReturnResult Execute(ReworkParameter p)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            Dictionary<string, Package> dictPackageForUpdate = new Dictionary<string, Package>();
            p.TransactionKeys = new Dictionary<string, string>();

            #region Foreach Lot
            //循环批次。
            foreach (string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);

                //生成操作事务主键。
                string transactionKey = Guid.NewGuid().ToString();
                p.TransactionKeys.Add(lotNumber, transactionKey);

                //更新批次记录。
                Lot lotUpdate = lot.Clone() as Lot;
                lotUpdate.OrderNumber = p.OrderNumber;
                lotUpdate.MaterialCode = p.MaterialCode;
                lotUpdate.RouteEnterpriseName = p.RouteEnterpriseName;
                lotUpdate.RouteName = p.RouteName;
                lotUpdate.RouteStepName = p.RouteStepName;
                lotUpdate.StateFlag = EnumLotState.WaitTrackIn;
                lotUpdate.PackageFlag = false;
                lotUpdate.PackageNo = null;
                lotUpdate.ReworkFlag = lot.ReworkFlag + 1;
                lotUpdate.OperateComputer = p.OperateComputer;
                lotUpdate.Editor = p.Creator;
                lotUpdate.EditTime = now;
                lotUpdate.LocationName = p.LocationName;
                lotUpdate.LineCode = null;
                lstLotDataEngineForUpdate.Add(lotUpdate);

                //更新包装记录。
                int itemNo = 1;
                Package packageObjUpadte = null;
                Package packageObj = null;
                string strPackageNo = lot.PackageNo;
                if (dictPackageForUpdate.Count > 0 && dictPackageForUpdate.ContainsKey(strPackageNo))
                {
                    packageObjUpadte = dictPackageForUpdate[strPackageNo];
                }
                else
                {
                    if (string.IsNullOrEmpty(strPackageNo) == false && strPackageNo.Length > 0)
                    {
                        packageObj = this.PackageDataEngine.Get(strPackageNo);
                        if (packageObj != null)
                        {
                            packageObjUpadte = packageObj.Clone() as Package;
                            dictPackageForUpdate.Add(strPackageNo, packageObjUpadte);
                        }
                    }
                }

                if (packageObjUpadte != null)
                {
                    packageObjUpadte.Quantity -= lot.Quantity;
                    if (packageObjUpadte.Quantity <= 0)
                    {
                        packageObjUpadte.Quantity = 0;
                    }
                    packageObjUpadte.PackageState = EnumPackageState.Packaging;
                    //this.lstPackageDataForUpdate.Add(packageObjUpadte);

                    //删除包装明细数据。
                    PackageDetail pdObj = this.PackageDetailDataEngine.Get(new PackageDetailKey()
                    {
                        PackageNo = lot.PackageNo,
                        ObjectType = EnumPackageObjectType.Lot,
                        ObjectNumber = lot.Key
                    });
                    itemNo = pdObj.ItemNo;
                    this.lstPackageDetailForDelete.Add(pdObj);
                }
                #region//记录操作历史。
                LotTransaction transObj = new LotTransaction()
                {
                    Key = transactionKey,
                    Activity = EnumLotActivity.Rework,
                    CreateTime = now,
                    Creator = p.Creator,
                    Description = p.Remark,
                    Editor = p.Creator,
                    EditTime = now,
                    InQuantity = lot.Quantity,
                    LotNumber = lotNumber,
                    LocationName = p.LocationName,
                    LineCode = null,
                    OperateComputer = p.OperateComputer,
                    OrderNumber = p.OrderNumber,
                    OutQuantity = lotUpdate.Quantity,
                    RouteEnterpriseName = p.RouteEnterpriseName,
                    RouteName = p.RouteName,
                    RouteStepName = p.RouteStepName,
                    ShiftName = p.ShiftName,
                    UndoFlag = false,
                    UndoTransactionKey = null,
                    Grade = lot.Grade,
                    Color = lot.Color,
                    Attr1 = lot.Attr1,
                    Attr2 = lot.Attr2,
                    Attr3 = lot.Attr3,
                    Attr4 = lot.Attr4,
                    Attr5 = lot.Attr5,
                    OriginalOrderNumber = lot.OriginalOrderNumber
                };
                this.lstLotTransactionForInsert.Add(transObj);
                //新增批次历史记录。
                LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                lotHistory.Description = Convert.ToString(itemNo); //记录批次在包装中的项目号
                this.lstLotTransactionHistoryForInsert.Add(lotHistory);
                #endregion

                #region //有附加参数记录附加参数数据。
                if (p.Paramters != null && p.Paramters.ContainsKey(lotNumber))
                {
                    foreach (TransactionParameter tp in p.Paramters[lotNumber])
                    {
                        LotTransactionParameter lotParamObj = new LotTransactionParameter()
                        {
                            Key = new LotTransactionParameterKey()
                            {
                                TransactionKey = transactionKey,
                                ParameterName = tp.Name,
                                ItemNo = tp.Index,
                            },
                            ParameterValue = tp.Value,
                            Editor = p.Creator,
                            EditTime = now
                        };
                        this.lstLotTransactionParameterDataEngineForInsert.Add(lotParamObj);
                    }
                }
                #endregion
            }
            #endregion

            ISession session = this.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            try
            {
                #region //开始事物处理

                #region 更新批次LOT 的信息
                //更新批次基本信息
                foreach (Lot lot1 in lstLotDataEngineForUpdate)
                {
                    this.LotDataEngine.Update(lot1, session);
                }

                //更新批次LotTransaction信息,包装及拆包不需要撤销
                foreach (LotTransaction lotTransaction in lstLotTransactionForInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //更新批次TransactionHistory信息
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransactionHistoryForInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                }

                //LotTransactionParameter
                foreach (LotTransactionParameter lotTransactionParameter in lstLotTransactionParameterDataEngineForInsert)
                {
                    this.LotTransactionParameterDataEngine.Insert(lotTransactionParameter, session);
                }

                //foreach (Package item in lstPackageDataForUpdate)
                //{
                //    this.PackageDataEngine.Update(item, session);
                //}

                #endregion


                #region //更新Package基本信息
                foreach (string key in dictPackageForUpdate.Keys)
                {
                    this.PackageDataEngine.Update(dictPackageForUpdate[key], session);
                }
                foreach (PackageDetail packageDetail in lstPackageDetailForDelete)
                {
                    this.PackageDetailDataEngine.Delete(packageDetail.Key, session);
                }
                #endregion
                transaction.Commit();
                session.Close();
                lstLotDataEngineForUpdate.Clear();
                lstLotTransactionForInsert.Clear();
                lstLotTransactionHistoryForInsert.Clear();
                lstLotTransactionParameterDataEngineForInsert.Clear();
                dictPackageForUpdate.Clear();
                lstPackageDetailForDelete.Clear();
                #endregion
            }
            catch (Exception err)
            {
                transaction.Rollback();
                session.Close();
                lstLotDataEngineForUpdate.Clear();
                lstLotTransactionForInsert.Clear();
                lstLotTransactionHistoryForInsert.Clear();
                lstLotTransactionParameterDataEngineForInsert.Clear();
                dictPackageForUpdate.Clear();
                lstPackageDetailForDelete.Clear();
                result.Code = 1000;
                result.Message += string.Format(@"错误：{0}", err.Message);
                return result;
            }
            return result;
        }
        
        MethodReturnResult Rework(ReworkParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (p == null)
            {
                result.Code = 1001;
                result.Message = "参数为空。";
                return result;
            }
            try
            {
                StringBuilder sbMessage = new StringBuilder();
                //操作前检查。
                result = Check(p);
                if (result.Code > 0)
                {
                    return result;
                }
                sbMessage.Append(result.Message);
                //执行操作

                result = Execute(p);
                if (result.Code > 0)
                {
                    return result;
                }
                //result = this.OnExecuted(p);
                //sbMessage.Append(result.Message);
                //if (result.Code > 0)
                //{
                //    return result;
                //}
                //sbMessage.Append(result.Message);

                result.Message = sbMessage.ToString();
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(@"错误：{0}", ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        
        public MethodReturnResult<DataSet> GetReceiptOrderNumberByPackageNo(string PackageNo)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"SELECT TOP 1 t2.ORDER_NUMBER FROM LSM_MATERIAL_RECEIPT_DETAIL t1 
                                                      LEFT JOIN LSM_MATERIAL_RECEIPT t2 ON t1.RECEIPT_NO=t2.RECEIPT_NO
                                                      WHERE t1.MATERIAL_LOT='{0}' ORDER BY t1.CREATE_TIME DESC", PackageNo);
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

        public MethodReturnResult GetREbackdata(REbackdataParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(p.PackageNo))
                {
                    //MethodReturnResult<DataSet> re = new MethodReturnResult<DataSet>();
                    //using (DbConnection con = this._db.CreateConnection())
                    //{
                    //    DbCommand cmd = con.CreateCommand();
                    //    cmd.CommandText = string.Format("select * from WIP_PACKAGE_his where PACKAGE_NO='{0}'", p.PackageNo);
                    //    re.Data = _db.ExecuteDataSet(cmd);
                    //}

                    //if (re.Data != null && re.Data.Tables.Count > 0 && re.Data.Tables[0].Rows.Count > 0)
                    //{
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
                    //}
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
        /// 返回托号中批次归档信息
        /// </summary>
        /// <param name="packeNo">托号</param>
        /// <returns></returns>
        //public MethodReturnResult GetREbackdata(string packeNo)
        //{
        //    MethodReturnResult result = new MethodReturnResult();

        //    try
        //    {
        //        using (DbConnection con = this._db.CreateConnection())
        //        {
        //            DbCommand cmd = con.CreateCommand();
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            //存储过程名
        //            cmd.CommandText = "sp_BK_ReBackData";

        //            this._db.AddInParameter(cmd, "PackageNo", DbType.String, packeNo);      //托号
        //            this._db.AddInParameter(cmd, "ReType", DbType.Int32, 1);                //返回类型 0 -全部返回 1 - 返回第一批代码 2 - 返回第二批
        //            this._db.AddInParameter(cmd, "IsDelete", DbType.Int32, 0);              //是否删除归档数据 0 - 不删除 1 - 删除

        //            cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
        //            cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

        //            SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
        //            parReturn.Direction = ParameterDirection.ReturnValue;

        //            cmd.Parameters.Add(parReturn);

        //            this._db.ExecuteNonQuery(cmd);

        //            int i = (int)cmd.Parameters["@return"].Value;

        //            if (i == -1)
        //            {
        //                string strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();

        //                result.Code = 2000;
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

        #region 根据ERP信息得到MES线边仓信息
        public MethodReturnResult<DataSet> GetERPStore(string ERPPartCode, string ReceiptNO)//参数：ERP中部门编码及出库单号
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            MethodReturnResult<DataSet> reLocation = new MethodReturnResult<DataSet>();
            MethodReturnResult<DataSet> reStore = new MethodReturnResult<DataSet>();
            try
            {
                if (!string.IsNullOrEmpty(ERPPartCode))
                {
                    using (DbConnection con = this._db.CreateConnection())
                    {
                        DbCommand cmd = con.CreateCommand();
                        //cmd.CommandText = string.Format("select * from FMM_LOCATION where ERP_PART_CODE='{0}'", ERPPartCode);
                        cmd.CommandText = string.Format("select * from FMM_LOCATION where ERPDeptCode='{0}'", ERPPartCode);
                        reLocation.Data = _db.ExecuteDataSet(cmd);
                    }
                }
                if (reLocation.Data != null && reLocation.Data.Tables.Count > 0 && reLocation.Data.Tables[0].Rows.Count > 0)
                {
                    //获取ERP出库明细以及出库单对应工单产品的父级编码是否一致
                    DataSet dsMaterial = new DataSet();
                    DataSet dsOrder = new DataSet();
                    DataSet dsMES = new DataSet();
                    DataTable dtMaterial = new DataTable();
                    DataTable dtMES = new DataTable();
                    DataTable dtOrder = new DataTable();
                    string StoreType = null;
                    using (DbConnection con = this.Ora_db.CreateConnection())
                    {
                        DbCommand cmd = con.CreateCommand();
                        cmd.CommandText = string.Format(@"select body.imhcode, code.classcode,body.MATERIALCODE " +
                                                         "from " + ErpDBName + ".v_ic_material_b body " +
                                                         "left join " + ErpDBName + ".v_bd_material code " +
                                                         "  on code.groupcode = '{0}' and code.orgcode='{1}' and body.materialcode = code.code " +
                                                         "where body.groupcode = '{0}' and body.orgcode='{1}' and body.imhcode = '{2}'",
                                                         ErpGroupCode,
                                                         ErpORGCode,
                                                         ReceiptNO);
                        
                        dsMaterial = Ora_db.ExecuteDataSet(cmd);

                        //将领料单父级代码信息添加到结果集中
                        DbCommand cmdOrder = con.CreateCommand();
                        cmdOrder.CommandText = string.Format(@"select ord.vbillcode,mat.imhcode,fat.classcode,ord.materialcode " +
                                                              "from " + ErpDBName + ".v_ic_material_b mat " 
                                                            + "left join " + ErpDBName + ".v_mm_dmo ord "
                                                            + "  on ord.groupcode = '{0}' and ord.orgcode='{1}' and mat.vproductbatch = ord.vbillcode " 
                                                            + "left join " + ErpDBName + ".v_bd_material fat "
                                                            + "  on fat.groupcode = '{0}' and fat.orgcode='{1}' and ord.materialcode = fat.code "
                                                            + "where mat.groupcode = '{0}' and mat.orgcode='{1}' and mat.imhcode= '{2}'",
                                                            ErpGroupCode,
                                                            ErpORGCode,
                                                            ReceiptNO);

                        dsOrder = Ora_db.ExecuteDataSet(cmdOrder);

                        
                        if (dsMaterial.Tables[0].Rows[0]["classcode"].ToString()=="25" && dsOrder.Tables[0].Rows[0]["classcode"].ToString() == "25")
                        {
                            if (dsMaterial.Tables[0].Rows[0]["MATERIALCODE"].ToString() == dsOrder.Tables[0].Rows[0]["materialcode"].ToString())
                            {
                                StoreType = "1";
                            }
                            else
                            {
                                StoreType = "0";
                            }
                        }
                        else
                        {
                            //将领料单父级代码信息添加到结果集中
                            if (dsMaterial.Tables[0].Rows[0]["classcode"].ToString() == dsOrder.Tables[0].Rows[0]["classcode"].ToString())
                            {
                                StoreType = "1";
                            }
                            else
                            {
                                StoreType = "0";
                            }
                        }
                    }
                    //获取该出库单
                    using (DbConnection con = this._db.CreateConnection())
                    {
                        DbCommand cmd = con.CreateCommand();
                        cmd.CommandText = string.Format("SELECT * FROM FMM_LINE_STORE MESStore WHERE MESStore.LOCATION_NAME='{0}' AND MESStore.STORE_TYPE='{1}'", reLocation.Data.Tables[0].Rows[0]["LOCATION_NAME"].ToString(), StoreType);
                        //StoreType);
                        reStore.Data = _db.ExecuteDataSet(cmd);



                    }
                    result.Data = reStore.Data;
                }
                else
                {
                    result.Data = null;
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

        public MethodReturnResult<DataSet> GetERPMaterialReceiptStore(string ReceiptNO)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                DataTable dt = new DataTable();
                DataSet dsResult = new DataSet();
                DataSet ds = new DataSet();
                using (DbConnection con = this.Ora_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select t1.vbillcode, t1.dbilldate,t1.CJCODE， t2.vproductbatch, t1.vtrantypecode ,t1.dept
                                                       from " + ErpDBName + ".v_ic_material_h t1 " + 
                                                      "left join " + ErpDBName + ".v_ic_material_b t2 " +
                                                      "  on t2.groupcode = '{0}' and t2.orgcode='{1}' and t2.imhcode = t1.vbillcode " +
                                                      "where t1.groupcode = '{0}' and t1.orgcode='{1}' " +
                                                      "group by t1.vbillcode, t1.dbilldate, t2.vproductbatch, t1.vtrantypecode ,t1.dept,t1.CJCODE " +
                                                      "having t1.vbillcode = '{2}'",
                                                      ErpGroupCode,
                                                      ErpORGCode,
                                                      ReceiptNO);

//                    cmd.CommandText = string.Format(@" select t1.vbillcode, t1.dbilldate,'K013C5' CJCODE， t2.vproductbatch, t1.vtrantypecode ,t1.dept
//                                                          from " + ErpDBName + ".v_ic_material_h t1 left join " + ErpDBName + ".v_ic_material_b t2 on t2.imhcode = t1.vbillcode group by t1.vbillcode, t1.dbilldate, t2.vproductbatch, t1.vtrantypecode ,t1.dept,t1.CJCODE having   t1.vbillcode = '{0}'", ReceiptNO);
                    ds = Ora_db.ExecuteDataSet(cmd);
                    //将ERP信息添加到结果集中
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        dt.TableName = "dtERP";
                        dsResult.Tables.Add(dt.Copy());
                    }

                }
                MethodReturnResult<DataSet> store = GetERPStore(ds.Tables[0].Rows[0]["CJCODE"].ToString(), ReceiptNO);
                if (store.Data != null && store.Data.Tables.Count > 0 && store.Data.Tables[0].Rows.Count > 0)
                {
                    dt = store.Data.Tables[0];
                    dt.TableName = "dtMES";
                    dsResult.Tables.Add(dt.Copy());
                }
                else
                {
                    MethodReturnResult<DataSet> resultERP = new MethodReturnResult<DataSet>();
                    using (DbConnection con = this.Ora_db.CreateConnection())
                    {
                        DbCommand cmd = con.CreateCommand();
                        cmd.CommandText = string.Format(@" select fathercode
                                                          from " + ErpDBName + ".v_org_dept where groupcode = '{0}' and orgcode='{1}' and CJCODE = '{2}'",
                                                                 ErpGroupCode,
                                                                 ErpORGCode,
                                                                 ds.Tables[0].Rows[0]["CJCODE"].ToString());
                        resultERP.Data = Ora_db.ExecuteDataSet(cmd);
                    }
                    MethodReturnResult<DataSet> Fatherstore = GetERPStore(resultERP.Data.Tables[0].Rows[0]["fathercode"].ToString(), ReceiptNO);
                    if (Fatherstore.Data != null && Fatherstore.Data.Tables.Count > 0 && Fatherstore.Data.Tables[0].Rows.Count > 0)
                    {
                        dt = Fatherstore.Data.Tables[0];
                        dt.TableName = "dtMES";
                        dsResult.Tables.Add(dt.Copy());
                    }
                    else
                    {
                        result.Message = string.Format("请核对出库单（{0}）的准确性。"
                                                    , ReceiptNO);

                    }

                }
                result.Data = dsResult;

            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }

        #endregion

        /// <summary> 根据ERP工单类型取得MES工单类型 </summary>
        /// <param name="orderType">ERP工单类型代码主键</param>
        /// <returns></returns>
        public string GetMESOrderType(string ERPOrderTypeKey)
        {
            string orderType = "";

            //取得对应的ID号
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.CategoryName = '{0}'
                                           AND (Key.AttributeName = '{1}' AND Value = '{2}')"
                                        , "OrderType"
                                        , "ERPOrderTypeKey"
                                        , ERPOrderTypeKey),
                OrderBy = "Key.ItemOrder"
            };

            IList<BaseAttributeValue> lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);

            if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
            {
                //取得对应的ID号
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.CategoryName = '{0}'
                                           AND (Key.AttributeName = '{1}' AND Key.ItemOrder = {2})"
                                            , "OrderType"
                                            , "VALUE"
                                            , lstBaseAttributeValues[0].Key.ItemOrder),
                    OrderBy = "Key.ItemOrder"
                };

                lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);

                if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
                {
                    orderType = lstBaseAttributeValues[0].Value;
                }
            }

            return orderType;
        }

        /// <summary> 根据ERP车间代码取得MES车间代码 </summary>
        /// <param name="orderType">ERP车间代码</param>
        /// <returns></returns>
        public string GetLocationName(string ERPDeptCode)
        {
            string locationName = "";

            //取得对应的ID号
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"ERPDeptCode = '{0}'
                                           AND Level = '2'"
                                       , ERPDeptCode),
                OrderBy = "Key"
            };

            IList<Location> lstLocation = LocationDataEngine.Get(cfg);


            if (lstLocation != null && lstLocation.Count > 0)
            {
                locationName = lstLocation[0].Key;
            }
            
            return locationName;
        }

        /// <summary>
        /// 根据工单类型取得对应的ERP入库单类型
        /// </summary>
        /// <param name="orderType">MES工单类型</param>
        /// <returns></returns>
        public string GetERPStockInType(string orderType)
        {
            string ERPorderType = "";

            //取得对应的ID号
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.CategoryName = '{0}'
                                           AND (Key.AttributeName = '{1}' AND Value = '{2}')"
                                        , "OrderType"
                                        , "VALUE"
                                        , orderType),
                OrderBy = "Key.ItemOrder"
            };

            IList<BaseAttributeValue> lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);

            if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
            {
                //取得对应的ID号
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.CategoryName = '{0}'
                                           AND (Key.AttributeName = '{1}' AND Key.ItemOrder = {2})"
                                            , "OrderType"
                                            , "ERPStockInType"
                                            , lstBaseAttributeValues[0].Key.ItemOrder),
                    OrderBy = "Key.ItemOrder"
                };

                lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);

                if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
                {
                    ERPorderType = lstBaseAttributeValues[0].Value;
                }
            }

            return ERPorderType;
        }

        /// <summary>
        /// 根据工单类型取得对应的ERP入库单类型主键
        /// </summary>
        /// <param name="orderType">MES工单类型</param>
        /// <returns></returns>
        public string GetERPStockInTypeKey(string orderType)
        {
            string ERPorderType = "";

            //取得对应的ID号
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.CategoryName = '{0}'
                                           AND (Key.AttributeName = '{1}' AND Value = '{2}')"
                                        , "OrderType"
                                        , "VALUE"
                                        , orderType),
                OrderBy = "Key.ItemOrder"
            };

            IList<BaseAttributeValue> lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);

            if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
            {
                //取得对应的ID号
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.CategoryName = '{0}'
                                           AND (Key.AttributeName = '{1}' AND Key.ItemOrder = {2})"
                                            , "OrderType"
                                            , "ERPStockInTypeKey"
                                            , lstBaseAttributeValues[0].Key.ItemOrder),
                    OrderBy = "Key.ItemOrder"
                };

                lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);

                if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
                {
                    ERPorderType = lstBaseAttributeValues[0].Value;
                }
            }

            return ERPorderType;
        }
    }
}

