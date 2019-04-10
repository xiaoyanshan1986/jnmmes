using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using NHibernate;
//using ServiceCenter.Common.DataAccess.NHibernate;
using Newtonsoft.Json;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Model.BaseData;
namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现IV测试数据数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class IVTestDataService : IIVTestDataContract
    {
        protected Database query_db;        

        public IVTestDataService()
        {
            this.query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        /// <summary>
        /// IV测试数据数据数据访问读写。
        /// </summary>
        public IIVTestDataDataEngine IVTestDataDataEngine { get; set; }

        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine { get; set; }

        /// <summary>
        /// 工单打印标签设置数据数据访问读写。
        /// </summary>
        public IWorkOrderPrintSetDataEngine WorkOrderPrintSetDataEngine { get; set; }

        /// <summary>
        /// 打印标签数据访问读写。
        /// </summary>
        public IPrintLabelDataEngine PrintLabelDataEngine { get; set; }

        /// <summary>
        /// 工单分档设置数据数据访问读写。
        /// </summary>
        public IWorkOrderPowersetDataEngine WorkOrderPowersetDataEngine { get; set; }

        /// <summary>
        /// 工单子分档设置数据数据访问读写。
        /// </summary>
        public IWorkOrderPowersetDetailDataEngine WorkOrderPowersetDetailDataEngine { get; set; }

        /// <summary>
        /// 物料数据访问读写。
        /// </summary>
        public IMaterialDataEngine MaterialDataEngine { get; set; }

        /// <summary>
        /// 物料属性数据访问读写。
        /// </summary>
        public IMaterialAttributeDataEngine MaterialAttributeDataEngine { get; set; }

        /// <summary>
        /// 打印日志数据访问读写。
        /// </summary>
        public IZwipLablePrintLogDataEngine ZwipLablePrintLogDataEngine { get; set; }

        /// <summary>
        /// 设备数据访问对象。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine { get; set; }

        /// <summary>
        /// 工步属性数据访问读写。
        /// </summary>
        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine { get; set; }

        /// <summary>
        /// 批次属性数据访问读写。
        /// </summary>
        public ILotAttributeDataEngine LotAttributeDataEngine { get; set; }

        /// <summary>
        /// 基础数据访问读写。
        /// </summary>
        public IBaseAttributeValueDataEngine BaseAttributeValueDataEngine { get; set; }
        

        /// <summary>
        /// 添加IV测试数据数据。
        /// </summary>
        /// <param name="obj">IV测试数据数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(IVTestData obj)
        {
       
            MethodReturnResult result = new MethodReturnResult();
            if (this.IVTestDataDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.IVTestDataService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.IVTestDataDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 修改IV测试数据数据。
        /// </summary>
        /// <param name="obj">IV测试数据数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(IVTestData obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.IVTestDataDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.IVTestDataService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.IVTestDataDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 删除IV测试数据数据。
        /// </summary>
        /// <param name="key">IV测试数据数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(IVTestDataKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.IVTestDataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.IVTestDataService_IsNotExists, key);
                return result;
            }
            try
            {
                this.IVTestDataDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取IV测试数据数据数据。
        /// </summary>
        /// <param name="key">IV测试数据数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;IVTestData&gt;" />,IV测试数据数据数据.</returns>
        public MethodReturnResult<IVTestData> Get(IVTestDataKey key)
        {
            MethodReturnResult<IVTestData> result = new MethodReturnResult<IVTestData>();
            if (!this.IVTestDataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.IVTestDataService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.IVTestDataDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取IV测试数据数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;IVTestData&gt;" />,IV测试数据数据数据集合。</returns>
        public MethodReturnResult<IList<IVTestData>> Get(ref PagingConfig cfg)
        {
            //DbConnection con = this.query_db.CreateConnection();
            //ISession session;
            
            MethodReturnResult<IList<IVTestData>> result = new MethodReturnResult<IList<IVTestData>>();

            try
            {
                //con.Open();
                //session = this.IVTestDataDataEngine.SessionFactory.OpenSession(con);

                result.Data = this.IVTestDataDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>IV数据查询（从存储过程获取）</summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// 
        public MethodReturnResult<DataSet> GetIVdata(ref LotIVdataParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this.query_db.CreateConnection())
                {
                    //调用存储过程
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_IVTest";

                    //存储过程传递的参数
                    this.query_db.AddInParameter(cmd, "@LotList", DbType.String, p.Lotlist);
                    this.query_db.AddInParameter(cmd, "@StratTime", DbType.String, p.StratTime);
                    this.query_db.AddInParameter(cmd, "@EndTime", DbType.String, p.EndTime);
                    this.query_db.AddInParameter(cmd, "@IsDefault", DbType.Int32, p.IsDefault);
                    this.query_db.AddInParameter(cmd, "@IsPrint", DbType.Int32, p.IsPrint);
                    this.query_db.AddInParameter(cmd, "@lineCode", DbType.String, p.LineCode);
                    this.query_db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
                    this.query_db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);
                    //返回总记录数
                    this.query_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;
                    //设置返回错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //设置返回值
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);
                    cmd.CommandTimeout = 960;

                    //执行存储过程
                    result.Data = this.query_db.ExecuteDataSet(cmd);

                    //返回总记录数
                    int a = Convert.ToInt32(cmd.Parameters["@Records"].Value);
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);

                    //取得返回值
                    int i = (int)cmd.Parameters["@return"].Value;

                    //调用失败返回错误信息
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


        //标签数据接口
        //(, string RouteOperationName, string PrinterName)
        public MinPaiData GetMinPaiData(string LotNumber, string lineCode)
        {
            string jsonminPai = string.Empty;
            MinPaiData minPai = new MinPaiData();
            MinPaiPrint minPaiPrint = new MinPaiPrint();
            MethodReturnResult<Lot> result0 = new MethodReturnResult<Lot>();
            result0.Data = this.LotDataEngine.Get(LotNumber);
            if (result0.Data != null)
            {
                if (result0.Data.Grade == null || result0.Data.Grade == "")
                {
                    #region 校验批次终检等待出站
                    if (result0.Data.RouteStepName != "终检" || result0.Data.StateFlag != EnumLotState.WaitTrackOut)
                    {
                        minPai.ErrorMessage = string.Format("批次{0}当前非终检等待出站状态！", LotNumber);
                        minPai.PrintQty = 3;
                        return minPai;
                    }
                    #endregion
                }              

                //获取批次IV数据
                PagingConfig cfg1 = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", LotNumber)
                };
                MethodReturnResult<IList<IVTestData>> result1 = new MethodReturnResult<IList<IVTestData>>();
                result1.Data = this.IVTestDataDataEngine.Get(cfg1);

                #region 存在IV数据
                if (result1 != null && result1.Data.Count > 0)
                {
                    if (result1.Data.Count > 1)
                    {
                        minPai.ErrorMessage = string.Format("批次{0}IV有效数据存在多条！", LotNumber);
                        minPai.PrintQty = 3;
                        return minPai;
                    }

                    #region 校验只有一条有效IV数据
                    else
                    {
                        IList<RouteStepAttribute> lstRouteStepAttribute = new List<RouteStepAttribute>();
                        
                        //获取工序控制属性列表                
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("Key.RouteName='{0}' AND Key.RouteStepName='{1}'"
                                                    , result0.Data.RouteName
                                                    , result0.Data.RouteStepName)
                        };
                        MethodReturnResult<IList<RouteStepAttribute>> rStep = new MethodReturnResult<IList<RouteStepAttribute>>();
                        rStep.Data = this.RouteStepAttributeDataEngine.Get(cfg);
                        if (rStep.Code <= 0 && rStep.Data != null)
                        {
                            lstRouteStepAttribute = rStep.Data;
                        }
                        #region 是否需要验证【IsCheckELImage，IsCheckIVImage】
                        
                        //是否检查EL图片
                        bool isCheckELImage = false;
                        var lnq = from item in lstRouteStepAttribute
                              where item.Key.AttributeName == "IsCheckELImage"
                              select item;
                        RouteStepAttribute rsaTmp = lnq.FirstOrDefault();
                        if (rsaTmp != null)
                        {
                            bool.TryParse(rsaTmp.Value, out isCheckELImage);
                        }

                        //是否检查IV图片。
                        bool isCheckIVImage = false;
                        lnq = from item in lstRouteStepAttribute
                              where item.Key.AttributeName == "IsCheckIVImage"
                              select item;
                        rsaTmp = lnq.FirstOrDefault();
                        if (rsaTmp != null)
                        {
                            bool.TryParse(rsaTmp.Value, out isCheckIVImage);
                        }
                        if (isCheckELImage || isCheckIVImage)
                        {
                            IList<LotAttribute> lstLotAttributes = new List<LotAttribute>();
                            cfg = new PagingConfig()
                                {
                                    IsPaging = false,
                                    Where = string.Format("Key.LotNumber='{0}'"
                                                          , LotNumber)
                                };
                                MethodReturnResult<IList<LotAttribute>> r = new MethodReturnResult<IList<LotAttribute>>();
                                r.Data = this.LotAttributeDataEngine.Get(cfg);
                                if (r.Code <= 0 && r.Data != null)
                                {
                                    lstLotAttributes = r.Data;
                                }
                            //是否显示EL图片
                            bool isExistedELImage = false;
                            LotAttribute rstAttr = new LotAttribute();
                            var lnqOfLotAtts = from item in lstLotAttributes
                                               where item.Key.AttributeName == "ELImagePath"
                                               select item;
                            rstAttr = lnqOfLotAtts.FirstOrDefault();
                            if (rstAttr != null)
                            {
                                isExistedELImage = true;
                            }
                            if (isCheckELImage && isExistedELImage == false)
                            {
                                minPai.ErrorMessage = string.Format("批次（{0}）的EL3图片不存在。" , LotNumber);
                                minPai.PrintQty = 3;
                                return minPai;                                
                            }

                            //是否显示IV图片
                            bool isExistedIVImage = false;
                            lnqOfLotAtts = from item in lstLotAttributes
                                           where item.Key.AttributeName == "IVImagePath"
                                           select item;
                            rstAttr = lnqOfLotAtts.FirstOrDefault();
                            if (rstAttr != null)
                            {
                                isExistedIVImage = true;
                            }

                            if (isCheckIVImage && isExistedIVImage == false)
                            {
                                minPai.ErrorMessage = string.Format("批次（{0}）的IV图片不存在。", LotNumber);
                                minPai.PrintQty = 3;
                                return minPai;
                            }
                        }
                        #endregion
                        
                        //获取工单设置的铭牌模板
                        MethodReturnResult<IList<WorkOrderPrintSet>> result2 = new MethodReturnResult<IList<WorkOrderPrintSet>>();
                        PagingConfig cfg2 = new PagingConfig()
                        {
                           Where = string.Format(" Key.OrderNumber = '{0}' AND IsUsed=1"
                                              ,result0.Data.OrderNumber),
                           OrderBy = "ItemNo"
                        };
                        result2.Data = this.WorkOrderPrintSetDataEngine.Get(cfg2);
                        if(result2 != null && result2.Data.Count > 0)
                        {
                            #region 校验设置的标签为铭牌模板
                            MethodReturnResult<PrintLabel> result3 = new MethodReturnResult<PrintLabel>();
                            for (int i = 0; i < result2.Data.Count; i++)
                            {
                                result3.Data = this.PrintLabelDataEngine.Get(result2.Data[i].Key.LabelCode);                               
                                if (result3.Data.Type == (EnumPrintLabelType)10)
                                {
                                    minPai.LablePath = System.Configuration.ConfigurationSettings.AppSettings["LablePath"] + result2.Data[i].Key.LabelCode + ".btw";
                                }
                            }
                            if (minPai.LablePath == null || minPai.LablePath == "")
                            {
                                minPai.ErrorMessage = string.Format("批次{0}所在工单{1}未设置可用的铭牌模板！", LotNumber);
                                minPai.PrintQty = 3;
                                return minPai;
                            }
                            #endregion

                            minPai.LotNumber = LotNumber;                           
                            minPai.PrintQty = result2.Data[0].Qty;
                            minPai.WorkOrderNumber = result0.Data.OrderNumber;
                            minPai.ProductNumber = result0.Data.MaterialCode;
                        }
                        else
                        {
                            minPai.ErrorMessage = string.Format("批次{0}所在工单{1}未设置可用的铭牌模板！", LotNumber);
                            minPai.PrintQty = 3;
                            return minPai;
                        }

                        //获取工单分档规则
                        MethodReturnResult<IList<WorkOrderPowerset>> result4 = new MethodReturnResult<IList<WorkOrderPowerset>>();
                        PagingConfig cfg4 = new PagingConfig()
                        {
                            Where = string.Format(" Key.OrderNumber = '{0}' AND Key.MaterialCode = '{1}' AND Key.Code = '{2}' AND Key.ItemNo = {3} AND IsUsed=1"
                                               , result0.Data.OrderNumber, result0.Data.MaterialCode, result1.Data[0].PowersetCode, (int)result1.Data[0].PowersetItemNo)
                        };
                        
                        result4.Data = this.WorkOrderPowersetDataEngine.Get(cfg4);
                        if (result4 != null && result4.Data.Count > 0)
                        {
                            //获取工单子分挡规则
                            MethodReturnResult<IList<WorkOrderPowersetDetail>> result5 = new MethodReturnResult<IList<WorkOrderPowersetDetail>>();
                            PagingConfig cfg5 = new PagingConfig()
                            {
                                Where = string.Format(" Key.OrderNumber = '{0}' AND Key.MaterialCode = '{1}' AND Key.Code = '{2}' AND Key.ItemNo = {3} AND Key.SubCode = '{4}' AND IsUsed=1"
                                                   , result0.Data.OrderNumber, result0.Data.MaterialCode, result1.Data[0].PowersetCode, (int)result1.Data[0].PowersetItemNo, result1.Data[0].PowersetSubCode)
                            };
                            result5.Data = this.WorkOrderPowersetDetailDataEngine.Get(cfg5);
                            if (result5 != null && result5.Data.Count > 0)
                            {
                                MethodReturnResult<Material> result6 = new MethodReturnResult<Material>();
                                result6.Data = this.MaterialDataEngine.Get(result0.Data.MaterialCode);
                                MethodReturnResult<MaterialAttribute> resultOfMaterialAttr = new MethodReturnResult<MaterialAttribute>();                               

                                minPai.PowerName = result4.Data[0].PowerName;
                                minPai.Color = result0.Data.Attr2.Substring(0, 2);
                                minPai.StandardPower = Convert.ToDouble(result4.Data[0].StandardPower);
                                minPai.StandardFuse = Convert.ToDouble(result4.Data[0].StandardFuse);
                                minPai.StandardIPM = Convert.ToDouble(result4.Data[0].StandardIPM);
                                minPai.StandardIsc = Convert.ToDouble(result4.Data[0].StandardIsc);
                                minPai.StandardVoc = Convert.ToDouble(result4.Data[0].StandardVoc);
                                minPai.StandardVPM = Convert.ToDouble(result4.Data[0].StandardVPM);
                                minPai.PowerDifference = result4.Data[0].PowerDifference;                               
                                minPai.ProductSpec = result6.Data.Spec;
                                int indexOfType = result6.Data.Name.IndexOf('-');
                                minPai.ProductType = result6.Data.Name.Substring(0, indexOfType) + "-" + result4.Data[0].PowerName.Substring(0, 3);
                                
                                #region SE获取ProductType方法
                                if (LotNumber.ToUpper().Contains("SE") || LotNumber.ToUpper().Contains("SY") || LotNumber.ToUpper().Contains("SN"))
                                {
                                    MaterialAttributeKey materialAttributeKey = new MaterialAttributeKey()
                                    {
                                        MaterialCode = result0.Data.MaterialCode,
                                        AttributeName = "ProductType"
                                    };
                                    resultOfMaterialAttr.Data = this.MaterialAttributeDataEngine.Get(materialAttributeKey);
                                    if(resultOfMaterialAttr.Data!=null)
                                    {
                                        string valueOf = resultOfMaterialAttr.Data.Value.Trim();
                                        int indexOfType1 = valueOf.IndexOf('*');
                                        minPai.ProductType = string.Format("{0}{1}-{2}{3}"
                                                    , valueOf.Substring(0, indexOfType1)
                                                    , result4.Data[0].PowerName.Substring(0, 3)
                                                    , result6.Data.MainRawQtyPerLot
                                                    , valueOf.Substring(valueOf.Length - 3)
                                                );
                                    }
                                    else
                                    {
                                        minPai.ErrorMessage = string.Format("批次{0}所在工单{1}的产品料号{2}未设置料号属性！",
                                                                LotNumber, result0.Data.OrderNumber, result0.Data.MaterialCode);
                                        minPai.PrintQty = 3;
                                        return minPai;
                                    }
                                }
                                #endregion

                                #region 27:协鑫72单晶-晋能-张家港获取ProductType方法 64:协鑫72单晶-晋能（晋中）代加工
                                if (LotNumber.Substring(0, 2) == "27" || LotNumber.Substring(0, 2) == "64")
                                {
                                    //GCL-M6/72H375
                                    minPai.ProductType = string.Format("GCL-M6/72H{0}", result4.Data[0].PowerName.Substring(0, 3));
                                }
                                #endregion

                                minPai.PM = result1.Data[0].CoefPM;
                                minPai.ISC = result1.Data[0].CoefISC;
                                minPai.IPM = result1.Data[0].CoefIPM;
                                minPai.VOC = result1.Data[0].CoefVOC;
                                minPai.VPM = result1.Data[0].CoefVPM;
                                minPai.FF = result1.Data[0].CoefFF;
                                
                                if (result0.Data.Grade != null && result0.Data.Grade != "")
                                {
                                    minPai.Grade = result0.Data.Grade;
                                    if (minPai.Grade == "A")
                                    {
                                        minPai.PowersetCode = result1.Data[0].PowersetCode;
                                        minPai.PowersetItemNo = (int)result1.Data[0].PowersetItemNo;
                                        minPai.PowersetSubCode = result1.Data[0].PowersetSubCode;
                                    }
                                }
                                else
                                {
                                    minPai.Grade = "A";
                                    minPai.PowersetCode = result1.Data[0].PowersetCode;
                                    minPai.PowersetItemNo = (int)result1.Data[0].PowersetItemNo;
                                    minPai.PowersetSubCode = result1.Data[0].PowersetSubCode;
                                }

                                minPaiPrint.LotNumber = minPai.LotNumber;
                                minPaiPrint.PowerDifference = minPai.PowerDifference;
                                minPaiPrint.ProductSpec = minPai.ProductSpec;
                                minPaiPrint.ProductType = minPai.ProductType;
                                minPaiPrint.StandardFuse = minPai.StandardFuse;
                                minPaiPrint.StandardIPM = minPai.StandardIPM;
                                minPaiPrint.StandardIsc = minPai.StandardIsc;
                                minPaiPrint.StandardPower = minPai.StandardPower;
                                if (minPai.ProductNumber == "1201020079")
                                {
                                    if (Convert.ToInt32(result4.Data[0].PowerName.Substring(0, 3)) == 355)
                                    {
                                        minPaiPrint.ModuleEfficiency = 18.3;
                                    }
                                    if (Convert.ToInt32(result4.Data[0].PowerName.Substring(0, 3)) == 360)
                                    {
                                        minPaiPrint.ModuleEfficiency = 18.6;
                                    }
                                    if (Convert.ToInt32(result4.Data[0].PowerName.Substring(0, 3)) == 365)
                                    {
                                        minPaiPrint.ModuleEfficiency = 18.8;
                                    }
                                    if (Convert.ToInt32(result4.Data[0].PowerName.Substring(0, 3)) == 370)
                                    {
                                        minPaiPrint.ModuleEfficiency = 19.1;
                                    }
                                    if (Convert.ToInt32(result4.Data[0].PowerName.Substring(0, 3)) == 375)
                                    {
                                        minPaiPrint.ModuleEfficiency = 19.3;
                                    }
                                }                                
                                minPaiPrint.StandardVoc = minPai.StandardVoc;
                                minPaiPrint.StandardVPM = minPai.StandardVPM;
                                if (LotNumber.ToUpper().Contains("SE") == false && LotNumber.ToUpper().Contains("SY") == false && LotNumber.ToUpper().Contains("SN") == false)
                                {
                                    minPaiPrint.PowersetSubCode = minPai.PowersetSubCode;
                                }                              
                            }
                            else
                            {
                                minPai.ErrorMessage = string.Format("批次{0}IV数据的子分档代码{1}在工单{2}子分档规则中不存在！",
                                                                LotNumber, result1.Data[0].PowersetSubCode, result0.Data.OrderNumber);
                                minPai.PrintQty = 3;
                                return minPai;
                            }
                        }
                        else
                        {
                            minPai.ErrorMessage = string.Format("批次{0}IV数据的分档代码{1}-分档项目号{2}在工单{3}分档规则中不存在！",
                                                                LotNumber, result1.Data[0].PowersetCode, (int)result1.Data[0].PowersetItemNo, result0.Data.OrderNumber);
                            minPai.PrintQty = 3;
                            return minPai;
                        }

                    }
                    #endregion
                }
                #endregion

                //不存在IV数据
                else
                {
                    minPai.ErrorMessage = string.Format("批次{0}IV有效数据不存在！", LotNumber);
                    minPai.PrintQty = 3;
                    return minPai;
                }
            }
            else
            {
                minPai.ErrorMessage = string.Format("批次{0}不存在！", LotNumber);
                minPai.PrintQty = 3;
                return minPai;
            }

            minPai.MinPaiJson = JsonConvert.SerializeObject(minPaiPrint);
            return minPai;
        }

        //打印反馈接口
        public string CallBack(string lotNumber, string Message, string lineCode)
        {
            string Messages = string.Empty;
            if (Message.Contains("成功"))
            {
                MethodReturnResult<Lot> result0 = new MethodReturnResult<Lot>();
                result0.Data = this.LotDataEngine.Get(lotNumber);
                if (result0.Data != null)
                {
                    ZwipLablePrintLogKey key = new ZwipLablePrintLogKey()
                    {
                        LotNumber = lotNumber,
                        LineCode = result0.Data.LineCode
                    };
                    ZwipLablePrintLog logData = new ZwipLablePrintLog();
                    if (this.ZwipLablePrintLogDataEngine.IsExists(key))
                    {
                        #region 更新打印日志
                        logData = this.ZwipLablePrintLogDataEngine.Get(key);
                        logData.ItemNo += 1;
                        logData.EditTime = DateTime.Now;                        
                        MethodReturnResult result1 = new MethodReturnResult();
                        try
                        {
                            this.ZwipLablePrintLogDataEngine.Update(logData);
                        }
                        catch (Exception ex)
                        {
                            result1.Code = 1000;
                            result1.Message = String.Format(StringResource.Error, ex.Message);
                            result1.Detail = ex.ToString();
                            Messages = result1.Message;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 向打印日志表插入日志
                        logData.Key = key;
                        logData.ItemNo = 1;
                        logData.EditTime = DateTime.Now;
                        logData.PrintTime = DateTime.Now;
                        logData.TrackFlag = 0;
                        MethodReturnResult result1 = new MethodReturnResult();
                        try
                        {
                            this.ZwipLablePrintLogDataEngine.Insert(logData);
                        }
                        catch (Exception ex)
                        {
                            result1.Code = 1000;
                            result1.Message = String.Format(StringResource.Error, ex.Message);
                            result1.Detail = ex.ToString();
                            Messages = result1.Message;
                        }
                        #endregion

                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@"LineCode='{0}' AND Name like '终检%'", lineCode)
                        };

                        MethodReturnResult<IList<Equipment>> result3 = new MethodReturnResult<IList<Equipment>>();
                        result3.Data = this.EquipmentDataEngine.Get(cfg);

                        #region 批次终检出站
                        TrackOutParameter p = new TrackOutParameter()
                        {
                            LineCode = result0.Data.LineCode,                   //线别
                            EquipmentCode = result3.Data[0].Key,
                            LotNumbers = new List<string>(),
                            RouteName = result0.Data.RouteName,                 //工艺流程
                            RouteOperationName = result0.Data.RouteStepName,    //工序
                            //Color = result0.Data.Attr2.Substring(0, 2),       //颜色
                            Grade = "A",                                        //等级
                            AutoDeductMaterial = false,                         //自动扣料标识
                            Creator = "SystemAdmin",                            //创建人
                            OperateComputer = "SystemAdmin",                    //操作客户端
                            Operator = "SystemAdmin",                           //操作人
                            IsFinished = false
                        };

                        PagingConfig cfg1 = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@" Key.AttributeName='MDesc'
                                and EXISTS (from BaseAttributeValue as p where p.Key.CategoryName='Color_Cell' AND p.Key.AttributeName='CName' and p.Value='{0}'
                                    and self.Key.ItemOrder = p.Key.ItemOrder
                                    and self.Key.CategoryName = p.Key.CategoryName
                                )", result0.Data.Attr2.Trim()),
                            OrderBy = "Key.ItemOrder"
                        };
                        IList<BaseAttributeValue> resultOfBaseAttributeValue = this.BaseAttributeValueDataEngine.Get(cfg1);
                        if (resultOfBaseAttributeValue != null && resultOfBaseAttributeValue.Count > 0)
                        {
                            p.Color = resultOfBaseAttributeValue[0].Value.ToString();
                            p.LotNumbers.Add(lotNumber);
                            MethodReturnResult result2 = new MethodReturnResult();
                            using (WipEngineerServiceClient client1 = new WipEngineerServiceClient())
                            {
                                result2 = client1.TrackOutLot(p);
                            }                            
                            Messages = result2.Message;
                        }
                        else
                        {
                            Messages = string.Format("反馈的批次号{0}的电池片颜色{1}未在基础数据中录入！", result0.Data.Attr2.Trim(),lotNumber);
                        }
                        
                        #endregion
                    }
                }
                else
                {
                    Messages = string.Format("反馈的批次号{0}有误！", lotNumber);
                }              
            }
            return Messages;
        }
    }
}
