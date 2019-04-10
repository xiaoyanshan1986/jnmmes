using Microsoft.Reporting.WebForms;
using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.Client.Mvc.RDLC;
using ServiceCenter.Common.Print;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class PackingListPrintAppointController : Controller
    {
        //
        // GET: /ZPVM/PackingListPrint/
        public ActionResult Index()
        {
            return View(new PackingListPrintQueryViewModel());
        }

        public ActionResult PrintAutoPackList()
        {
            return View(new PackingListPrintQueryViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShowReport(PackingListPrintQueryViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.PackageNo))
                {
                    return Content(string.Empty);
                }
                return ShowPackageListReportAppoint(model);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShowAutoPackList(PackingListPrintQueryViewModel model)
        {
            try
            {
                model.IsAutoPackageNo = true;
                model.PackageNo = "";
                return ShowPackageListReport(model);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        [AllowAnonymous]
        public ActionResult ShowPackageListReport(PackingListPrintQueryViewModel model)
        {
            model.PackageNo = model.PackageNo;
            MethodReturnResult result = new MethodReturnResult();
            PackageListDataSet ds = new PackageListDataSet();
            IList<PackageDetail> lstPackageDetail = new List<PackageDetail>();
            IList<IVTestData> lstIVTestData = new List<IVTestData>();
            IList<Lot> lstLot = new List<Lot>();

            #region
            try
            {
                string orderNumber = null;
                string materialCode = null;
                double qty = 0;

                //判断是否自动包装
                if (model.IsAutoPackageNo)
                {
                    //获取批次号工单的满包数量
                    using (LotQueryServiceClient client = new LotQueryServiceClient())
                    {
                        MethodReturnResult<Lot> resultLot = client.Get(model.LotNumber1);
                        if (resultLot.Code > 0 || resultLot.Data == null)
                        {
                            return Content(string.Format("未找到批次号{0}的信息。", model.LotNumber1));
                        }
                        orderNumber = resultLot.Data.OrderNumber;
                        materialCode = resultLot.Data.MaterialCode;
                    }

                    if (orderNumber == null || materialCode == null)
                    {
                        return Content(string.Format("未找到批次号{0}的工单信息。", model.LotNumber1));
                    }

                    //获取工单规则
                    using (WorkOrderRuleServiceClient clientWorkOrderProduct = new WorkOrderRuleServiceClient())
                    {
                        WorkOrderRuleKey WorkOrderRuleKey = new WorkOrderRuleKey()
                        {
                            OrderNumber = orderNumber,
                            MaterialCode = materialCode
                        };
                        MethodReturnResult<WorkOrderRule> resultWorkOrderRule = clientWorkOrderProduct.Get(WorkOrderRuleKey);
                        qty = resultWorkOrderRule.Data.FullPackageQty;
                    }

                    if (qty == 0)
                    {
                        return Content(string.Format("未找到批次号{0}的满包数量。", model.LotNumber1));
                    }

                    //获取包装明细数据。
                    using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("( ItemNo='1' or ItemNo='{2}') and ( Key.ObjectNumber='{0}' or  Key.ObjectNumber='{1}')", model.LotNumber1, model.LotNumber2, Convert.ToInt32(qty)),
                            OrderBy = "Key.ObjectNumber"
                        };
                        MethodReturnResult<IList<PackageDetail>> result1 = client.GetDetail(ref cfg);
                        if (result1.Code > 0 || result1.Data == null || result1.Data.Count == 0)
                        {
                            return Content(result.Message);
                        }
                        else
                        {
                            if (result1.Data.Count != 2)
                            {
                                return Content(string.Format("请确认批次号({0},{1})是整托的第一块和最后一块。", model.LotNumber1, model.LotNumber2));
                            }
                            PackageDetail obj = result1.Data.FirstOrDefault();
                            if (obj != null)
                            {
                                model.PackageNo = obj.Key.PackageNo;
                            }
                        }
                    }
                }

                //获取打印包装清单相关的数据
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    RPTpackagelistParameter param = new RPTpackagelistParameter();  //参数
                    if (model.PackageNo != null || model.PackageNo != "")
                    {
                        param.PackageNo = model.PackageNo; //包装号
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key.PackageNo,ItemNo ASC",
                    };
                    MethodReturnResult<DataSet> dt = client.GetRPTpackagelist(param);
                    ViewBag.HistoryList = dt;

                    //判断批次是否存在
                    if (dt.Code > 0 || dt.Data == null)
                    {
                        return Content(string.Format("未找到批次号{0}的信息。", model.LotNumber1));
                    }

                    orderNumber = dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString();  //工单号
                    materialCode = dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString();//物料编码

                    //判断物料编码或是工单是否为空
                    if (orderNumber == null || materialCode == null)
                    {
                        return Content(string.Format("未找到批次号{0}的工单信息。", model.LotNumber1));
                    }

                    //根据工单规则获取满包数量
                    using (WorkOrderRuleServiceClient clientWorkOrderProduct = new WorkOrderRuleServiceClient())
                    {
                        WorkOrderRuleKey WorkOrderRuleKey = new WorkOrderRuleKey()
                        {
                            OrderNumber = orderNumber,
                            MaterialCode = materialCode
                        };
                        MethodReturnResult<WorkOrderRule> resultWorkOrderRule = clientWorkOrderProduct.Get(WorkOrderRuleKey);
                        qty = resultWorkOrderRule.Data.FullPackageQty;
                    }

                    if (qty == 0)
                    {
                        return Content(string.Format("未找到批次号{0}的满包数量。", model.LotNumber1));
                    }

                    //获取包装数据清单
                    string strPackagePrintLabel = this.GetPackagePrintLabel(dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString(), dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString());
                    bool blIsEnglish = false;
                    if (strPackagePrintLabel != "" && strPackagePrintLabel.Length > 0)
                    {
                        if (strPackagePrintLabel.IndexOf("EN") > 0)
                        {
                            blIsEnglish = true;
                        }
                    }

                    //获取工单分档规则
                    WorkOrderPowerset wop = null;
                    if (dt.Data != null && dt.Data.Tables[0].Rows[0]["PS_CODE"].ToString() != null)
                    {
                        using (WorkOrderPowersetServiceClient client1 = new WorkOrderPowersetServiceClient())
                        {
                            MethodReturnResult<WorkOrderPowerset> result1 = client1.Get(new WorkOrderPowersetKey()
                            {
                                Code = dt.Data.Tables[0].Rows[0]["PS_CODE"].ToString(),
                                ItemNo = Convert.ToInt16(dt.Data.Tables[0].Rows[0]["PS_ITEM_NO"].ToString()),
                                OrderNumber = dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString(),
                                MaterialCode = dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString()
                            });
                            wop = result1.Data;
                        }
                    }

                    //根据物料编码获取物料数据，进一步获取产品类型。
                    Material material = null;
                    using (MaterialServiceClient client2 = new MaterialServiceClient())
                    {
                        MethodReturnResult<Material> result2 = client2.Get(dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString());
                        if (result2.Code > 0)
                        {
                            return Content(result.Message);
                        }
                        material = result2.Data;
                    }

                    //组件包装数据。
                    PackageListDataSet.PackageRow row = ds.Package.NewPackageRow();
                    if (strPackagePrintLabel == "PackageList_SunEN.rdlc")
                    {
                        row.ProductType = string.Format("SE-{1}{0}NPB-A4"
                                                    , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
                                                    , material.Key.StartsWith("1201") ? "M" : "P");
                    }
                    if (strPackagePrintLabel == "PackageList_PERC.rdlc")
                    {
                        row.ProductType = string.Format("{3}{1}{2}-{0}"
                                                        , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
                                                        , material.Key.StartsWith("1201") ? "M" : "M"
                                                        , material.MainRawQtyPerLot
                                                        , material.Name.Substring(0, 3));

                    }
                    else
                    {
                        row.ProductType = string.Format("{3}{1}{2}-{0}"
                                                    , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
                                                    , material.Key.StartsWith("1201") ? "M" : "P"
                                                    , material.MainRawQtyPerLot
                                                    , material.Name.Substring(0, 3));
                    }
                    row.MaterialClass = material.Class;                                      //物料类型
                    row.ProductSpec = material.Spec;                                         //产品规格
                    row.Description = material.Description;                                  //描述
                    row.OrderNumber = dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString();  //工单
                    row.PackageNo = dt.Data.Tables[0].Rows[0]["PACKAGE_NO"].ToString();      //包装号
                    row.Count = dt.Data.Tables[0].Rows.Count.ToString();                     //总数
                    row.PowerName = wop == null ? string.Empty : wop.PowerName;
                    row.PowerTolerance = wop == null ? string.Empty : wop.PowerDifference;
                    row.Color = dt.Data.Tables[0].Rows[0]["COLOR"].ToString();
                    if (blIsEnglish)
                    {
                        //if (row.Color == "深蓝")
                        //{
                        //    row.Color = "Dark Blue";
                        //}
                        //if (row.Color == "浅蓝")
                        //{
                        //    row.Color = "Light Blue";
                        //}
                        //if (row.Color == "正蓝")
                        //{
                        //    row.Color = "Blue";
                        //}

                        row.OrderNumber = orderNumber.Substring(4, 8);
                        row.PowerName = wop == null ? string.Empty : wop.PowerName.Substring(0, 3);

                        //Light blue，Blue，Dark blue（3选1）
                        if (row.Color == "深蓝")
                        {
                            row.Color = "Navy Blue";
                        }
                        if (row.Color == "浅蓝")
                        {
                            row.Color = "baby Blue";
                        }
                        if (row.Color == "正蓝")
                        {
                            row.Color = "Blue";
                        }

                    }
                    else
                    {
                        if (row.Color == "深蓝")
                        {
                            row.Color = "Dark Blue/深蓝";
                        }
                        if (row.Color == "浅蓝")
                        {
                            row.Color = "Light Blue/浅蓝";
                        }
                        if (row.Color == "正蓝")
                        {
                            row.Color = "Blue/正蓝";
                        }
                    }
                    row.PowerSubCode = dt.Data.Tables[0].Rows[0]["PS_SUBCODE"].ToString();

                    row.MaterialCode = dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString();
                    ds.Package.AddPackageRow(row);

                    for (int j = 0; j < dt.Data.Tables[0].Rows.Count; j++)
                    { //添加包装明细。

                        PackageListDataSet.PackageDetailRow detailRow = ds.PackageDetail.NewPackageDetailRow();
                        detailRow.PackageNo = dt.Data.Tables[0].Rows[j]["PACKAGE_NO"].ToString();
                        detailRow.ItemNo = Convert.ToInt16(dt.Data.Tables[0].Rows[j]["ITEM_NO"].ToString());
                        detailRow.LotNo = dt.Data.Tables[0].Rows[j]["OBJECT_NUMBER"].ToString();
                        detailRow.IMP = dt.Data.Tables[0].Rows[j]["COEF_IMAX"].ToString();
                        detailRow.ISC = dt.Data.Tables[0].Rows[j]["COEF_ISC"].ToString();
                        detailRow.PM = dt.Data.Tables[0].Rows[j]["COEF_PMAX"].ToString();
                        detailRow.VMP = dt.Data.Tables[0].Rows[j]["COEF_VMAX"].ToString();
                        detailRow.VOC = dt.Data.Tables[0].Rows[j]["COEF_VOC"].ToString();
                        detailRow.FF = dt.Data.Tables[0].Rows[j]["COEF_FF"].ToString();
                        ds.PackageDetail.AddPackageDetailRow(detailRow);
                    }

                    if (dt.Data.Tables.Count < 29)
                    {
                        for (int i = ds.PackageDetail.Rows.Count; i < 29; i++)
                        {
                            PackageListDataSet.PackageDetailRow detailRow = ds.PackageDetail.NewPackageDetailRow();
                            detailRow.PackageNo = row.PackageNo;
                            detailRow.ItemNo = i + 1;
                            detailRow.LotNo = null;
                            ds.PackageDetail.AddPackageDetailRow(detailRow);
                        }
                    }

                    using (LocalReport localReport = new LocalReport())
                    {
                        if (model.PackageListType == EnumPackageListType.Normal)
                        {
                            if (string.IsNullOrEmpty(strPackagePrintLabel) == false && strPackagePrintLabel.Length > 0)
                            {
                                localReport.ReportPath = Server.MapPath(string.Format("~/RDLC/{0}", strPackagePrintLabel));
                            }
                            else
                            {
                                localReport.ReportPath = Server.MapPath("~/RDLC/PackageList.rdlc");
                            }
                        }
                        else
                        {
                            //localReport.ReportPath = Server.MapPath("~/RDLC/PackageList_P.rdlc");
                            localReport.ReportPath = Server.MapPath("~/RDLC/PackageList_PackageNo.rdlc");
                        }
                        ReportDataSource reportDataSourcePackage = new ReportDataSource("Package", ds.Tables[ds.Package.TableName]);
                        localReport.DataSources.Add(reportDataSourcePackage);
                        ReportDataSource reportDataSourcePackageDetail = new ReportDataSource("PackageDetail", ds.Tables[ds.PackageDetail.TableName]);
                        localReport.DataSources.Add(reportDataSourcePackageDetail);
                        string reportType = "PDF";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;
                        //The DeviceInfo settings should be changed based on the reportType
                        //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                        string deviceInfo =
                                        "<DeviceInfo>" +
                                        "  <OutputFormat>PDF</OutputFormat>" +
                            //"  <PageWidth>8.5in</PageWidth>" +
                            //"  <PageHeight>11in</PageHeight>" +
                            //"  <MarginTop>0.5in</MarginTop>" +
                            //"  <MarginLeft>1in</MarginLeft>" +
                            //"  <MarginRight>1in</MarginRight>" +
                            //"  <MarginBottom>0.5in</MarginBottom>" +
                                        "</DeviceInfo>";
                        Warning[] warnings;
                        string[] streams;
                        byte[] renderedBytes;
                        //Render the report
                        renderedBytes = localReport.Render(
                            reportType,
                            deviceInfo,
                            out mimeType,
                            out encoding,
                            out fileNameExtension,
                            out streams,
                            out warnings);
                        //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
                        return File(renderedBytes, mimeType);
                    }
                }
            }
            #endregion

            catch (Exception err)
            {
                result.Code = 1005;
                result.Message = err.Message;
            }
            return Json(result);
        }
           
        public string GetPackagePrintLabel(string strWorkOrder,string strMaterialCode)
        {
            string strPackagePrintLabel = "";
            string strPrintLabelCode="";
            using (WorkOrderPrintSetServiceClient client = new WorkOrderPrintSetServiceClient())
            {
                PagingConfig cfg = new PagingConfig
                {
                    IsPaging=false,
                    Where = string.Format(@" Key.OrderNumber='{0}' and Key.MaterialCode='{1}' and Key.LabelCode like 'JNW%'", strWorkOrder, strMaterialCode),
                    OrderBy = " Key.LabelCode desc "
                   
                };
                MethodReturnResult<IList<WorkOrderPrintSet>> result = client.Get(ref cfg);
                if(result!=null && result.Data!=null && result.Data.Count>0)
                {
                    strPrintLabelCode = result.Data[0].Key.LabelCode;
                }
            }

            if (string.IsNullOrEmpty(strPrintLabelCode) == false && strPrintLabelCode.Length > 0)
            {
                using (PrintLabelServiceClient client = new PrintLabelServiceClient())
                {
                    MethodReturnResult<PrintLabel> resultOfPrintLabel = client.Get(strPrintLabelCode);
                    if(resultOfPrintLabel!=null && resultOfPrintLabel.Code==0)
                    {
                        PrintLabel printLabel = resultOfPrintLabel.Data;
                        if(printLabel!=null)
                        {
                            strPackagePrintLabel = printLabel.Content;
                        }
                    }
                }                
            }
            return strPackagePrintLabel;
        }

        /// <summary> 2016-8-17 11:26:20  武子靖备份 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 

        //        [AllowAnonymous]
//        public ActionResult ShowPackageListReport(PackingListPrintQueryViewModel model)
//        {
//            model.PackageNo = model.PackageNo;

//            PackageListDataSet ds = new PackageListDataSet();
//            //#if DEBUG
//            //           //组织包装数据
//            //            PackageListDataSet.PackageRow row = ds.Package.NewPackageRow();
//            //            row.OrderNumber="WMK1409110001";
//            //            row.PackageNo = "091100010001";
//            //            row.PowerName="250W";
//            //            row.PowerTolerance="0~+5W";
//            //            row.ProductType = "JNM250P60";
//            //            row.Color = "深蓝";
//            //            row.ProductSpec = "1650*992*35";
//            //            ds.Package.AddPackageRow(row);
//            //            for (int i = 0; i < 29; i++)
//            //            {
//            //                PackageListDataSet.PackageDetailRow detailRow = ds.PackageDetail.NewPackageDetailRow();
//            //                detailRow.IMP = "8.20";
//            //                detailRow.ISC = "8.86";
//            //                detailRow.PM = "250.69";
//            //                detailRow.VMP = "36.38";
//            //                detailRow.VOC = "40.63";
//            //                detailRow.FF = "15";
//            //                detailRow.PackageNo = "091100010001";
//            //                detailRow.ItemNo = i + 1;
//            //                detailRow.LotNo = "JN122172221" + (i+1).ToString("0000");
//            //                ds.PackageDetail.AddPackageDetailRow(detailRow);
//            //            }
//            //#else
//            #region

//            Package packageObj = null;

//            IList<PackageDetail> lstPackageDetail = new List<PackageDetail>();
//            IList<IVTestData> lstIVTestData = new List<IVTestData>();
//            IList<Lot> lstLot = new List<Lot>();

//            if (model.IsAutoPackageNo)
//            {
//                string orderNumber = null;
//                string materialCode = null;
//                double qty = 0;
//                //获取批次号工单的满包数量
//                using (LotQueryServiceClient client = new LotQueryServiceClient())
//                {
//                    MethodReturnResult<Lot> resultLot = client.Get(model.LotNumber1);
//                    if (resultLot.Code > 0 || resultLot.Data == null)
//                    {
//                        return Content(string.Format("未找到批次号{0}的信息。", model.LotNumber1));
//                    }
//                    orderNumber = resultLot.Data.OrderNumber;
//                    materialCode = resultLot.Data.MaterialCode;
//                }
//                if (orderNumber == null || materialCode == null)
//                {
//                    return Content(string.Format("未找到批次号{0}的工单信息。", model.LotNumber1));
//                }
//                using (WorkOrderRuleServiceClient clientWorkOrderProduct = new WorkOrderRuleServiceClient())
//                {
//                    WorkOrderRuleKey WorkOrderRuleKey = new WorkOrderRuleKey()
//                    {
//                        OrderNumber = orderNumber,
//                        MaterialCode = materialCode
//                    };
//                    MethodReturnResult<WorkOrderRule> resultWorkOrderRule = clientWorkOrderProduct.Get(WorkOrderRuleKey);
//                    qty = resultWorkOrderRule.Data.FullPackageQty;
//                }

//                if (qty == 0)
//                {
//                    return Content(string.Format("未找到批次号{0}的满包数量。", model.LotNumber1));
//                }
        //                //获取包装明细数据。
        //                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
        //                {
        //                    PagingConfig cfg = new PagingConfig()
        //                    {
        //                        IsPaging = false,
        //                        Where = string.Format("( ItemNo='1' or ItemNo='{2}') and ( Key.ObjectNumber='{0}' or  Key.ObjectNumber='{1}')", model.LotNumber1, model.LotNumber2, Convert.ToInt32(qty)),
        //                        OrderBy = "Key.ObjectNumber"
        //                    };
        //                    MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);
        //                    if (result.Code > 0 || result.Data == null || result.Data.Count == 0)
        //                    {
        //                        return Content(result.Message);
        //                    }
        //                    else
        //                    {
        //                        if (result.Data.Count != 2)
        //                        {
        //                            return Content(string.Format("请确认批次号({0},{1})是整托的第一块和最后一块。", model.LotNumber1, model.LotNumber2));
        //                        }
        //                        PackageDetail obj = result.Data.FirstOrDefault();
        //                        if (obj != null)
        //                        {
        //                            model.PackageNo = obj.Key.PackageNo;
        //                        }
        //                    }
        //                }
//            }
//            if (model.IsLotNumber)
//            {
//                //获取包装明细数据。
//                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
//                {
//                    PagingConfig cfg = new PagingConfig()
//                    {
//                        IsPaging = false,
//                        Where = string.Format("Key.ObjectNumber='{0}'", model.PackageNo),
//                        OrderBy = "Key.ObjectNumber"
//                    };
//                    MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);
//                    if (result.Code > 0 || result.Data == null || result.Data.Count == 0)
//                    {
//                        return Content(result.Message);
//                    }
//                    else
//                    {
//                        PackageDetail obj = result.Data.FirstOrDefault();
//                        if (obj != null)
//                        {
//                            model.PackageNo = obj.Key.PackageNo;
//                        }
//                    }
//                }
//            }
//            //获取包装数据。
//            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
//            {
//                MethodReturnResult<Package> result = client.Get(model.PackageNo);
//                if (result.Code > 0 || result.Data == null || result.Data.Quantity <= 0)
//                {
//                    return Content(result.Message);
//                }
//                packageObj = result.Data;
//            }

//            //工单规则维护特殊的包装清单
//            string strPackagePrintLabel = this.GetPackagePrintLabel(packageObj.OrderNumber, packageObj.MaterialCode);
//            bool blIsEnglish = false;
//            if (strPackagePrintLabel != "" && strPackagePrintLabel.Length > 0)
//            {
//                if (strPackagePrintLabel.IndexOf("EN") > 0)
//                {
//                    blIsEnglish = true;
//                }
//            }
//            //获取包装明细数据。
//            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format("Key.PackageNo='{0}'", model.PackageNo),
//                    OrderBy = "ItemNo"
//                };
//                MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);
//                if (result.Code > 0 || result.Data == null || result.Data.Count == 0)
//                {
//                    return Content(result.Message);
//                }
//                lstPackageDetail = result.Data;
//            }
//            //获取IV测试数据。
//            using (IVTestDataServiceClient client = new IVTestDataServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"EXISTS(SELECT p.Key 
//                                                   FROM Lot as p
//                                                   WHERE p.PackageNo='{0}'
//                                                   AND p.Key=self.Key.LotNumber)
//                                            AND IsDefault=1"
//                                           , model.PackageNo)
//                };
//                MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);
//                if (result.Code > 0)
//                {
//                    return Content(result.Message);
//                }
//                lstIVTestData = result.Data;
//            }
//            //获取批次数据。
//            using (LotQueryServiceClient client = new LotQueryServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"PackageNo='{0}'"
//                                           , model.PackageNo)
//                };
//                MethodReturnResult<IList<Lot>> result = client.Get(ref cfg);
//                if (result.Code > 0)
//                {
//                    return Content(result.Message);
//                }
//                lstLot = result.Data;
//            }
//            //根据包装第一块组件的数据获取分档方式和功率档位。
//            string firstLotNumber = lstPackageDetail[0].Key.ObjectNumber;

//            Lot firstLot = (from item in lstLot
//                            where item.Key.ToUpper() == firstLotNumber.ToUpper()
//                            select item).FirstOrDefault();

//            IVTestData firstIVTestData = (from item in lstIVTestData
//                                          where item.Key.LotNumber.ToUpper() == firstLotNumber.ToUpper()
//                                          select item).FirstOrDefault();
//            WorkOrderPowerset wop = null;
//            if (firstIVTestData != null && firstIVTestData.PowersetCode != null)
//            {
//                using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
//                {
//                    MethodReturnResult<WorkOrderPowerset> result = client.Get(new WorkOrderPowersetKey()
//                    {
//                        Code = firstIVTestData.PowersetCode,
//                        ItemNo = firstIVTestData.PowersetItemNo ?? -1,
//                        OrderNumber = firstLot.OrderNumber,
//                        MaterialCode = firstLot.MaterialCode
//                    });
//                    wop = result.Data;
//                }
//            }

//            //根据物料编码获取物料数据，进一步获取产品类型。
//            Material material = null;
//            using (MaterialServiceClient client = new MaterialServiceClient())
//            {
//                MethodReturnResult<Material> result = client.Get(firstLot.MaterialCode);
//                if (result.Code > 0)
//                {
//                    return Content(result.Message);
//                }
//                material = result.Data;
//            }
//            //组件包装数据。
//            PackageListDataSet.PackageRow row = ds.Package.NewPackageRow();
//            if (strPackagePrintLabel == "PackageList_SunEN.rdlc")
//            {
//                row.ProductType = string.Format("SE-{1}{0}NPB-A4"
//                                           , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
//                                           , material.Key.StartsWith("1201") ? "M" : "P");
//            }
//            if (strPackagePrintLabel == "PackageList_PERC.rdlc")
//            {
//                row.ProductType = string.Format("{3}{1}{2}-{0}"
//                                              , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
//                                              , material.Key.StartsWith("1201") ? "M" : "M"
//                                              , material.MainRawQtyPerLot
//                                              , material.Name.Substring(0, 3));

//            }
//            else
//            {
//                row.ProductType = string.Format("{3}{1}{2}-{0}"
//                                            , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
//                                            , material.Key.StartsWith("1201") ? "M" : "P"
//                                            , material.MainRawQtyPerLot
//                                            , material.Name.Substring(0, 3));
//            }


//            row.MaterialClass = material.Class;
//            row.ProductSpec = material.Spec;
//            row.Description = material.Description;//DESCRIPTION
//            row.OrderNumber = packageObj.OrderNumber;
//            row.PackageNo = packageObj.Key;
//            row.Count = packageObj.Quantity.ToString();
//            row.PowerName = wop == null ? string.Empty : wop.PowerName;
//            row.PowerTolerance = wop == null ? string.Empty : wop.PowerDifference;
//            row.Color = firstLot.Color;
//            if (blIsEnglish)
//            {
//                //Light blue，Blue，Dark blue（3选1）
//                if (row.Color == "深蓝")
//                {
//                    row.Color = "Dark Blue";
//                }
//                if (row.Color == "浅蓝")
//                {
//                    row.Color = "Light Blue";
//                }
//                if (row.Color == "正蓝")
//                {
//                    row.Color = "Blue";
//                }
//            }
//            else
//            {
//                if (row.Color == "深蓝")
//                {
//                    row.Color = "Dark Blue/深蓝";
//                }
//                if (row.Color == "浅蓝")
//                {
//                    row.Color = "Light Blue/浅蓝";
//                }
//                if (row.Color == "正蓝")
//                {
//                    row.Color = "Blue/正蓝";
//                }
//            }
//            row.PowerSubCode = firstIVTestData.PowersetSubCode;

//            row.MaterialCode = packageObj.MaterialCode;
//            ds.Package.AddPackageRow(row);
//            //添加包装明细。
//            foreach (PackageDetail item in lstPackageDetail)
//            {
//                PackageListDataSet.PackageDetailRow detailRow = ds.PackageDetail.NewPackageDetailRow();
//                detailRow.PackageNo = item.Key.PackageNo;
//                detailRow.ItemNo = item.ItemNo;
//                detailRow.LotNo = item.Key.ObjectNumber;

//                IVTestData tempIVTestData = lstIVTestData.Where(m => m.Key.LotNumber.ToUpper() == item.Key.ObjectNumber.ToUpper())
//                                                       .FirstOrDefault();
//                if (tempIVTestData != null)
//                {
//                    detailRow.IMP = tempIVTestData.CoefIPM.ToString("0.00");
//                    detailRow.ISC = tempIVTestData.CoefISC.ToString("0.00");
//                    detailRow.PM = tempIVTestData.CoefPM.ToString("0.00");
//                    detailRow.VMP = tempIVTestData.CoefVPM.ToString("0.00");
//                    detailRow.VOC = tempIVTestData.CoefVOC.ToString("0.00");
//                    detailRow.FF = tempIVTestData.CoefFF.ToString("0.00%");
//                }
//                ds.PackageDetail.AddPackageDetailRow(detailRow);
//            }

//            if (ds.PackageDetail.Rows.Count < 29)
//            {
//                for (int i = ds.PackageDetail.Rows.Count; i < 29; i++)
//                {
//                    PackageListDataSet.PackageDetailRow detailRow = ds.PackageDetail.NewPackageDetailRow();
//                    detailRow.PackageNo = row.PackageNo;
//                    detailRow.ItemNo = i + 1;
//                    detailRow.LotNo = null;
//                    ds.PackageDetail.AddPackageDetailRow(detailRow);
//                }
//            }
//            #endregion
//            //#endif

//            using (LocalReport localReport = new LocalReport())
//            {

//                //if (string.IsNullOrEmpty(strPackagePrintLabel) == false && strPackagePrintLabel.Length>0)
//                //{
//                //    localReport.ReportPath = Server.MapPath( string.Format("~/RDLC/{0}",strPackagePrintLabel));
//                //}
//                //else
//                //{ 
//                //    if (model.PackageListType == EnumPackageListType.Normal)
//                //    {
//                //        localReport.ReportPath = Server.MapPath("~/RDLC/PackageList.rdlc");
//                //    }
//                //    else
//                //    {
//                //        //localReport.ReportPath = Server.MapPath("~/RDLC/PackageList_P.rdlc");
//                //        localReport.ReportPath = Server.MapPath("~/RDLC/PackageList_PackageNo.rdlc");
//                //    }
//                //}


//                if (model.PackageListType == EnumPackageListType.Normal)
//                {
//                    if (string.IsNullOrEmpty(strPackagePrintLabel) == false && strPackagePrintLabel.Length > 0)
//                    {
//                        localReport.ReportPath = Server.MapPath(string.Format("~/RDLC/{0}", strPackagePrintLabel));
//                    }
//                    else
//                    {
//                        localReport.ReportPath = Server.MapPath("~/RDLC/PackageList.rdlc");
//                    }
//                }
//                else
//                {
//                    //localReport.ReportPath = Server.MapPath("~/RDLC/PackageList_P.rdlc");
//                    localReport.ReportPath = Server.MapPath("~/RDLC/PackageList_PackageNo.rdlc");
//                }









//                ReportDataSource reportDataSourcePackage = new ReportDataSource("Package", ds.Tables[ds.Package.TableName]);
//                localReport.DataSources.Add(reportDataSourcePackage);
//                ReportDataSource reportDataSourcePackageDetail = new ReportDataSource("PackageDetail", ds.Tables[ds.PackageDetail.TableName]);
//                localReport.DataSources.Add(reportDataSourcePackageDetail);
//                string reportType = "PDF";
//                string mimeType;
//                string encoding;
//                string fileNameExtension;
//                //The DeviceInfo settings should be changed based on the reportType
//                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
//                string deviceInfo =
//                                "<DeviceInfo>" +
//                                "  <OutputFormat>PDF</OutputFormat>" +
//                    //"  <PageWidth>8.5in</PageWidth>" +
//                    //"  <PageHeight>11in</PageHeight>" +
//                    //"  <MarginTop>0.5in</MarginTop>" +
//                    //"  <MarginLeft>1in</MarginLeft>" +
//                    //"  <MarginRight>1in</MarginRight>" +
//                    //"  <MarginBottom>0.5in</MarginBottom>" +
//                                "</DeviceInfo>";
//                Warning[] warnings;
//                string[] streams;
//                byte[] renderedBytes;
//                //Render the report
//                renderedBytes = localReport.Render(
//                    reportType,
//                    deviceInfo,
//                    out mimeType,
//                    out encoding,
//                    out fileNameExtension,
//                    out streams,
//                    out warnings);
//                //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
//                return File(renderedBytes, mimeType);
//            }
        //        }

        [AllowAnonymous]
        public ActionResult ShowPackageListReportAppoint(PackingListPrintQueryViewModel model)
        {
            model.PackageNo = model.PackageNo;
            MethodReturnResult result = new MethodReturnResult();
            PackageListDataSet ds = new PackageListDataSet();
            IList<PackageDetail> lstPackageDetail = new List<PackageDetail>();
            IList<IVTestData> lstIVTestData = new List<IVTestData>();
            IList<Lot> lstLot = new List<Lot>();

            #region
            try
            {
                string orderNumber = null;
                string materialCode = null;
                double qty = 0;

                //判断是否自动包装
                if (model.IsAutoPackageNo)
                {
                    //获取批次号工单的满包数量
                    using (LotQueryServiceClient client = new LotQueryServiceClient())
                    {
                        MethodReturnResult<Lot> resultLot = client.Get(model.LotNumber1);
                        if (resultLot.Code > 0 || resultLot.Data == null)
                        {
                            return Content(string.Format("未找到批次号{0}的信息。", model.LotNumber1));
                        }
                        orderNumber = resultLot.Data.OrderNumber;
                        materialCode = resultLot.Data.MaterialCode;
                    }

                    if (orderNumber == null || materialCode == null)
                    {
                        return Content(string.Format("未找到批次号{0}的工单信息。", model.LotNumber1));
                    }

                    //获取工单规则
                    using (WorkOrderRuleServiceClient clientWorkOrderProduct = new WorkOrderRuleServiceClient())
                    {
                        WorkOrderRuleKey WorkOrderRuleKey = new WorkOrderRuleKey()
                        {
                            OrderNumber = orderNumber,
                            MaterialCode = materialCode
                        };
                        MethodReturnResult<WorkOrderRule> resultWorkOrderRule = clientWorkOrderProduct.Get(WorkOrderRuleKey);
                        qty = resultWorkOrderRule.Data.FullPackageQty;
                    }

                    if (qty == 0)
                    {
                        return Content(string.Format("未找到批次号{0}的满包数量。", model.LotNumber1));
                    }

                    //获取包装明细数据。
                    using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("( ItemNo='1' or ItemNo='{2}') and ( Key.ObjectNumber='{0}' or  Key.ObjectNumber='{1}')", model.LotNumber1, model.LotNumber2, Convert.ToInt32(qty)),
                            OrderBy = "Key.ObjectNumber"
                        };
                        MethodReturnResult<IList<PackageDetail>> result1 = client.GetDetail(ref cfg);
                        if (result1.Code > 0 || result1.Data == null || result1.Data.Count == 0)
                        {
                            return Content(result.Message);
                        }
                        else
                        {
                            if (result1.Data.Count != 2)
                            {
                                return Content(string.Format("请确认批次号({0},{1})是整托的第一块和最后一块。", model.LotNumber1, model.LotNumber2));
                            }
                            PackageDetail obj = result1.Data.FirstOrDefault();
                            if (obj != null)
                            {
                                model.PackageNo = obj.Key.PackageNo;
                            }
                        }
                    }
                }

                //获取打印包装清单相关的数据
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    RPTpackagelistParameter param = new RPTpackagelistParameter();  //参数
                    if (model.PackageNo != null || model.PackageNo != "")
                    {
                        param.PackageNo = model.PackageNo; //包装号
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key.PackageNo,ItemNo ASC",
                    };
                    MethodReturnResult<DataSet> dt = client.GetRPTpackagelist(param);
                    ViewBag.HistoryList = dt;

                    //判断批次是否存在
                    if (dt.Code > 0 || dt.Data == null)
                    {
                        return Content(string.Format("未找到批次号{0}的信息。", model.LotNumber1));
                    }

                    orderNumber = dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString();  //工单号
                    materialCode = dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString();//物料编码

                    //判断物料编码或是工单是否为空
                    if (orderNumber == null || materialCode == null)
                    {
                        return Content(string.Format("未找到批次号{0}的工单信息。", model.LotNumber1));
                    }

                    //根据工单规则获取满包数量
                    using (WorkOrderRuleServiceClient clientWorkOrderProduct = new WorkOrderRuleServiceClient())
                    {
                        WorkOrderRuleKey WorkOrderRuleKey = new WorkOrderRuleKey()
                        {
                            OrderNumber = orderNumber,
                            MaterialCode = materialCode
                        };
                        MethodReturnResult<WorkOrderRule> resultWorkOrderRule = clientWorkOrderProduct.Get(WorkOrderRuleKey);
                        qty = resultWorkOrderRule.Data.FullPackageQty;
                    }

                    if (qty == 0)
                    {
                        return Content(string.Format("未找到批次号{0}的满包数量。", model.LotNumber1));
                    }

                    //获取包装数据清单
                    //string strPackagePrintLabel = this.GetPackagePrintLabel(dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString(), dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString());
                    string strPackagePrintLabel = "";
                    bool blIsEnglish = false;
                    if (strPackagePrintLabel != "" && strPackagePrintLabel.Length > 0)
                    {
                        if (strPackagePrintLabel.IndexOf("EN") > 0)
                        {
                            blIsEnglish = true;
                        }
                    }

                    //获取工单分档规则
                    WorkOrderPowerset wop = null;
                    if (dt.Data != null && dt.Data.Tables[0].Rows[0]["PS_CODE"].ToString() != null)
                    {
                        using (WorkOrderPowersetServiceClient client1 = new WorkOrderPowersetServiceClient())
                        {
                            MethodReturnResult<WorkOrderPowerset> result1 = client1.Get(new WorkOrderPowersetKey()
                            {
                                Code = dt.Data.Tables[0].Rows[0]["PS_CODE"].ToString(),
                                ItemNo = Convert.ToInt16(dt.Data.Tables[0].Rows[0]["PS_ITEM_NO"].ToString()),
                                OrderNumber = dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString(),
                                MaterialCode = dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString()
                            });
                            wop = result1.Data;
                        }
                    }

                    //根据物料编码获取物料数据，进一步获取产品类型。
                    Material material = null;
                    using (MaterialServiceClient client2 = new MaterialServiceClient())
                    {
                        MethodReturnResult<Material> result2 = client2.Get(dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString());
                        if (result2.Code > 0)
                        {
                            return Content(result.Message);
                        }
                        material = result2.Data;
                    }

                    //组件包装数据。
                    PackageListDataSet.PackageRow row = ds.Package.NewPackageRow();
                    if (strPackagePrintLabel == "PackageList_SunEN.rdlc")
                    {
                        row.ProductType = string.Format("SE-{1}{0}NPB-A4"
                                                    , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
                                                    , material.Key.StartsWith("1201") ? "M" : "P");
                    }
                    if (strPackagePrintLabel == "PackageList_PERC.rdlc")
                    {
                        row.ProductType = string.Format("{3}{1}{2}-{0}"
                                                        , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
                                                        , material.Key.StartsWith("1201") ? "M" : "M"
                                                        , material.MainRawQtyPerLot
                                                        , material.Name.Substring(0, 3));

                    }
                    else
                    {
                        //row.ProductType = string.Format("{3}{1}{2}-{0}"
                        //                            , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
                        //                            , material.Key.StartsWith("1201") ? "M" : "P"
                        //                            , material.MainRawQtyPerLot
                        //                            , material.Name.Substring(0, 3));

                        row.ProductType = string.Format("P{0}-{1}"
                                              , material.MainRawQtyPerLot
                                              , wop == null ? string.Empty : Convert.ToString(wop.StandardPower));
                    }

                    row.MaterialClass = material.Class;                                      //物料类型
                    row.ProductSpec = material.Spec;                                         //产品规格
                    row.Description = material.Description;                                  //描述
                    row.OrderNumber = dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString();  //工单
                    row.PackageNo = dt.Data.Tables[0].Rows[0]["PACKAGE_NO"].ToString();      //包装号
                    row.Count = dt.Data.Tables[0].Rows.Count.ToString();                     //总数
                    row.PowerName = wop == null ? string.Empty : wop.PowerName;
                    row.PowerTolerance = wop == null ? string.Empty : wop.PowerDifference;
                    row.Color = dt.Data.Tables[0].Rows[0]["COLOR"].ToString();
                    if (blIsEnglish)
                    {
                        //if (row.Color == "深蓝")
                        //{
                        //    row.Color = "Dark Blue";
                        //}
                        //if (row.Color == "浅蓝")
                        //{
                        //    row.Color = "Light Blue";
                        //}
                        //if (row.Color == "正蓝")
                        //{
                        //    row.Color = "Blue";
                        //}

                        row.OrderNumber = orderNumber.Substring(4, 8);
                        row.PowerName = wop == null ? string.Empty : wop.PowerName.Substring(0, 3);

                        //Light blue，Blue，Dark blue（3选1）
                        if (row.Color == "深蓝")
                        {
                            row.Color = "Navy Blue";
                        }
                        if (row.Color == "浅蓝")
                        {
                            row.Color = "baby Blue";
                        }
                        if (row.Color == "正蓝")
                        {
                            row.Color = "Blue";
                        }

                    }
                    else
                    {
                        if (row.Color == "深蓝")
                        {
                            row.Color = "Dark Blue/深蓝";
                        }
                        if (row.Color == "浅蓝")
                        {
                            row.Color = "Light Blue/浅蓝";
                        }
                        if (row.Color == "正蓝")
                        {
                            row.Color = "Blue/正蓝";
                        }
                    }
                    row.PowerSubCode = dt.Data.Tables[0].Rows[0]["PS_SUBCODE"].ToString();

                    row.MaterialCode = dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString();
                    ds.Package.AddPackageRow(row);

                    for (int j = 0; j < dt.Data.Tables[0].Rows.Count; j++)
                    { //添加包装明细。

                        PackageListDataSet.PackageDetailRow detailRow = ds.PackageDetail.NewPackageDetailRow();
                        detailRow.PackageNo = dt.Data.Tables[0].Rows[j]["PACKAGE_NO"].ToString();
                        detailRow.ItemNo = Convert.ToInt16(dt.Data.Tables[0].Rows[j]["ITEM_NO"].ToString());
                        detailRow.LotNo = dt.Data.Tables[0].Rows[j]["OBJECT_NUMBER"].ToString();
                        detailRow.IMP = dt.Data.Tables[0].Rows[j]["COEF_IMAX"].ToString();
                        detailRow.ISC = dt.Data.Tables[0].Rows[j]["COEF_ISC"].ToString();
                        detailRow.PM = dt.Data.Tables[0].Rows[j]["COEF_PMAX"].ToString();
                        detailRow.VMP = dt.Data.Tables[0].Rows[j]["COEF_VMAX"].ToString();
                        detailRow.VOC = dt.Data.Tables[0].Rows[j]["COEF_VOC"].ToString();
                        detailRow.FF = dt.Data.Tables[0].Rows[j]["COEF_FF"].ToString();
                        ds.PackageDetail.AddPackageDetailRow(detailRow);
                    }

                    if (dt.Data.Tables.Count < 29)
                    {
                        for (int i = ds.PackageDetail.Rows.Count; i < 29; i++)
                        {
                            PackageListDataSet.PackageDetailRow detailRow = ds.PackageDetail.NewPackageDetailRow();
                            detailRow.PackageNo = row.PackageNo;
                            detailRow.ItemNo = i + 1;
                            detailRow.LotNo = null;
                            ds.PackageDetail.AddPackageDetailRow(detailRow);
                        }
                    }

                    using (LocalReport localReport = new LocalReport())
                    {
                        if (model.PackageListType == EnumPackageListType.Normal)
                        {
                            if (string.IsNullOrEmpty(strPackagePrintLabel) == false && strPackagePrintLabel.Length > 0)
                            {
                                localReport.ReportPath = Server.MapPath(string.Format("~/RDLC/{0}", strPackagePrintLabel));
                            }
                            else
                            {
                                localReport.ReportPath = Server.MapPath("~/RDLC/PackageList_RSM.rdlc");
                            }
                        }
                        else
                        {
                            //localReport.ReportPath = Server.MapPath("~/RDLC/PackageList_P.rdlc");
                            localReport.ReportPath = Server.MapPath("~/RDLC/PackageList_PackageNo.rdlc");
                        }
                        ReportDataSource reportDataSourcePackage = new ReportDataSource("Package", ds.Tables[ds.Package.TableName]);
                        localReport.DataSources.Add(reportDataSourcePackage);
                        ReportDataSource reportDataSourcePackageDetail = new ReportDataSource("PackageDetail", ds.Tables[ds.PackageDetail.TableName]);
                        localReport.DataSources.Add(reportDataSourcePackageDetail);
                        string reportType = "PDF";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;
                        //The DeviceInfo settings should be changed based on the reportType
                        //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                        string deviceInfo =
                                        "<DeviceInfo>" +
                                        "  <OutputFormat>PDF</OutputFormat>" +
                            //"  <PageWidth>8.5in</PageWidth>" +
                            //"  <PageHeight>11in</PageHeight>" +
                            //"  <MarginTop>0.5in</MarginTop>" +
                            //"  <MarginLeft>1in</MarginLeft>" +
                            //"  <MarginRight>1in</MarginRight>" +
                            //"  <MarginBottom>0.5in</MarginBottom>" +
                                        "</DeviceInfo>";
                        Warning[] warnings;
                        string[] streams;
                        byte[] renderedBytes;
                        //Render the report
                        renderedBytes = localReport.Render(
                            reportType,
                            deviceInfo,
                            out mimeType,
                            out encoding,
                            out fileNameExtension,
                            out streams,
                            out warnings);
                        //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
                        return File(renderedBytes, mimeType);
                    }
                }
            }
            #endregion

            catch (Exception err)
            {
                result.Code = 1005;
                result.Message = err.Message;
            }
            return Json(result);
        }
        
    }
}