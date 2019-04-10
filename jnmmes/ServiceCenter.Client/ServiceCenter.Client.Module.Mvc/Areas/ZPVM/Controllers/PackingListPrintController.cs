using Microsoft.Reporting.WebForms;
using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.Client.Mvc.RDLC;
using ServiceCenter.Common.Print;
using ServiceCenter.Common;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using System.Net.Sockets;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class PackingListPrintController : Controller
    {
        //获取箱号打印模板
        string YLPrintPackageLab = System.Configuration.ConfigurationSettings.AppSettings["YLPrintPackageLab"];
        //获取档位打印模板参数
        string YLPrintPowerNumberLab = System.Configuration.ConfigurationSettings.AppSettings["YLPrintPowerNumberLab"];

        /// <summary>
        /// 包装清单打印
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(new PackingListPrintQueryViewModel());
        }

        /// <summary>
        /// 自动包装线包装清单打印
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintAutoPackList()
        {
            return View(new PackingListPrintQueryViewModel());
        }

        /// <summary>
        /// 出货包装清单打印
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintDeliverPackList()
        {
            return View(new PackingListPrintQueryViewModel());
        }


        /// <summary>
        /// 返工包装清单打印
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintXXPackList()
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
                return ShowPackageListReport(model);
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

        /// <summary>
        /// 显示出库包装清单格式
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShowDeliverReport(PackingListPrintQueryViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.PackageNo))
                {
                    return Content(string.Empty);
                }

                return ShowDeliverPackageListReport(model);
            }

            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        /// <summary>
        /// 显示返工包装清单格式
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShowXXReport(PackingListPrintQueryViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.PackageNo))
                {
                    return Content(string.Empty);
                }

                return ShowXXPackageListReport(model);
            }

            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        //包装清单查询显示
        [AllowAnonymous]
        public ActionResult ShowPackageListReport(PackingListPrintQueryViewModel model)
        {
            model.PackageNo = model.PackageNo.Trim();
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
                string color = null;
                string pscode = null;
                string psitemno = null;
                string pssubcode = null;
                double qty = 0;
                string grade = null;

                #region 判断是否自动包装
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

                    //qty = 31;

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
                #endregion

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
                    MethodReturnResult<DataSet> dtOfOEM = client.GetOEMpackagelist(param);
                    if (dtOfOEM.Data != null && dtOfOEM.Data.Tables.Count > 0 && dtOfOEM.Data.Tables[0].Rows.Count > 0)
                    {
                        DataTable oemDataTable = new DataTable();
                        oemDataTable = dtOfOEM.Data.Tables[0];
                        oemDataTable.TableName = "OemDataTable";
                        dt.Data.Tables.Add(oemDataTable.Copy());
                    }

                    //判断批次是否存在
                    if (dt.Code > 0 || dt.Data == null)
                    {
                        return Content(string.Format("未找到批次号{0}的信息。", model.LotNumber1));
                    }
                    else
                    {
                        if (dt.Data.Tables[0] != null && dt.Data.Tables[0].Rows.Count > 0)
                        {
                            orderNumber = dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString();  //工单号(第一行数据的工单号)
                            materialCode = dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString();//物料编码
                            color = dt.Data.Tables[0].Rows[0]["COLOR"].ToString();
                            pscode = dt.Data.Tables[0].Rows[0]["PS_CODE"].ToString();
                            psitemno = dt.Data.Tables[0].Rows[0]["PS_ITEM_NO"].ToString();
                            pssubcode = dt.Data.Tables[0].Rows[0]["PS_SUBCODE"].ToString();
                            grade = dt.Data.Tables[0].Rows[0]["GRADE"].ToString();
                        }
                        else if (dtOfOEM.Data != null && dtOfOEM.Data.Tables.Count > 0 && dtOfOEM.Data.Tables[0].Rows.Count > 0)
                        {
                            orderNumber = dt.Data.Tables[1].Rows[0]["ORDER_NUMBER"].ToString();  //工单号(第一行数据的工单号)
                            materialCode = dt.Data.Tables[1].Rows[0]["MATERIAL_CODE"].ToString();//物料编码
                            color = dt.Data.Tables[1].Rows[0]["COLOR"].ToString();
                            pscode = dt.Data.Tables[1].Rows[0]["PS_CODE"].ToString();
                            psitemno = dt.Data.Tables[1].Rows[0]["PS_ITEM_NO"].ToString();
                            pssubcode = dt.Data.Tables[1].Rows[0]["PS_SUBCODE"].ToString();
                            grade = dt.Data.Tables[1].Rows[0]["GRADE"].ToString();
                        }
                        else
                        {
                            return Content(string.Format("请进行IV修正或检查批次{0}所在工单分档代码与批次IV数据中分档代码是否一致。",
                                model.LotNumber1));
                        }
                    }


                    //判断物料编码或是工单是否为空
                    if (orderNumber == null || materialCode == null)
                    {
                        return Content(string.Format("未找到批次号{0}的工单信息。", model.LotNumber1));
                    }

                    WorkOrder workOrder = null;
                    //获取工单信息
                    using (WorkOrderServiceClient clientWorkOrder = new WorkOrderServiceClient())
                    {
                        workOrder = clientWorkOrder.Get(orderNumber).Data;
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
                    string strPackagePrintLabel = this.GetPackagePrintLabel(dt, orderNumber, materialCode);
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
                    if (dt.Data != null && pscode != null)
                    {
                        using (WorkOrderPowersetServiceClient client1 = new WorkOrderPowersetServiceClient())
                        {
                            MethodReturnResult<WorkOrderPowerset> result1 = client1.Get(new WorkOrderPowersetKey()
                            {
                                Code = pscode,
                                ItemNo = Convert.ToInt16(psitemno),
                                OrderNumber = orderNumber,
                                MaterialCode = materialCode
                            });
                            wop = result1.Data;
                        }
                    }
                    else
                    {
                        return Content(string.Format("批次{0}所在工单未设置分档", model.LotNumber1));
                    }

                    //根据物料编码获取物料数据，进一步获取产品类型。
                    Material material = null;
                    using (MaterialServiceClient client2 = new MaterialServiceClient())
                    {
                        MethodReturnResult<Material> result2 = client2.Get(materialCode);
                        if (result2.Code > 0)
                        {
                            return Content(result2.Message);
                        }
                        material = result2.Data;
                    }

                    //根据物料编码获取物料电池片类型属性数据
                    //MaterialAttribute materialAttribute = null;
                    //using (MaterialAttributeServiceClient clientOfMattr = new MaterialAttributeServiceClient())
                    //{
                    //    MaterialAttributeKey materialAttributeKey = new MaterialAttributeKey()
                    //    {
                    //        MaterialCode = materialCode,
                    //        AttributeName = "CellType"
                    //    };

                    //    MethodReturnResult<MaterialAttribute> result2 = clientOfMattr.Get(materialAttributeKey);
                    //    materialAttribute = result2.Data;
                    //}

                    //根据工单获取工单电池片类型属性数据
                    WorkOrderAttribute workOrderAttribute = null;
                    using (WorkOrderAttributeServiceClient clientOfOrderMattr = new WorkOrderAttributeServiceClient())
                    {
                        WorkOrderAttributeKey workOrderAttributeKey = new WorkOrderAttributeKey()
                        {
                            OrderNumber = orderNumber,
                            AttributeName = "CellType"
                        };

                        MethodReturnResult<WorkOrderAttribute> result2 = clientOfOrderMattr.Get(workOrderAttributeKey);
                        workOrderAttribute = result2.Data;
                    }

                    //根据包装号获取柜号信息
                    Package packageToChest = null;
                    string chestOfPackage = "";
                    using (PackageQueryServiceClient clientOfPackage = new PackageQueryServiceClient())
                    {
                        MethodReturnResult<Package> result2 = clientOfPackage.Get(model.PackageNo);
                        packageToChest = result2.Data;
                        if (packageToChest != null
                            && packageToChest.ContainerNo != null
                            && packageToChest.ContainerNo != "")
                        {
                            chestOfPackage = packageToChest.ContainerNo.Trim();
                        }
                    }


                    //组件包装数据。
                    PackageListDataSet.PackageRow row = ds.Package.NewPackageRow();
                    if (strPackagePrintLabel == "PackageList_SunEN.rdlc")
                    {
                        row.ProductType = string.Format("SE-{1}{0}NPB-A4"
                                                    , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
                                                    , material.Key.StartsWith("1201") ? "M" : "P");
                    }
                    else if (strPackagePrintLabel == "PackageList_PERC.rdlc")
                    {
                        //row.ProductType = string.Format("{3}{1}{2}-{0}"
                        //                                , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
                        //                                , material.Key.StartsWith("1201") ? "M" : "M"
                        //                                , material.MainRawQtyPerLot
                        //                                , material.Name.Substring(0, 3));

                        //row.ProductType = string.Format("{0}{1}-{2}"
                        //                            , material.Name.Substring(0, 4)
                        //                            , material.MainRawQtyPerLot
                        //                            , wop.PowerName.Substring(0, 3));

                        int indexOfType = material.Name.IndexOf('-');
                        row.ProductType = string.Format("{0}-{1}"
                                                    , material.Name.Substring(0, indexOfType)
                                                    , wop.PowerName.Substring(0, 3));

                    }
                    //协鑫72包装清单 《产品规格》
                    else if (strPackagePrintLabel == "XXPackageList.rdlc")
                    {
                        if (wop.PowerName == "325W")
                        {
                            row.ProductType = "GCL-P6/72325";
                        }
                        else if (wop.PowerName == "320W")
                        {
                            row.ProductType = "ZKX-320P-24";
                        }
                        else
                        {
                            row.ProductType = "Test123:" + wop.PowerName;
                        }
                    }
                    //协鑫60包装清单
                    else if (strPackagePrintLabel == "协鑫60.rdlc")
                    {
                        //row.ProductType = "txtx：" + wop.PowerName;
                    }
                    //晋能档位转换清单
                    else if (strPackagePrintLabel == "晋能.rdlc")
                    {
                        if (wop.PowerName == "特定功率")
                        {
                            wop.PowerName = "特定功率";
                            row.ProductType = string.Format("{0}{1}-{2}"
                                                    , material.Name.Substring(0, 4)
                                                    , material.MainRawQtyPerLot
                                                    , wop.PowerName.Substring(0, 3));
                        }
                        else
                        {
                            row.ProductType = string.Format("{0}{1}-{2}"
                                                    , material.Name.Substring(0, 4)
                                                    , material.MainRawQtyPerLot
                                                    , wop.PowerName.Substring(0, 3));
                        }
                    }
                    //SE单晶60包装清单
                    else if (strPackagePrintLabel == "SEPackageList-EN.rdlc")
                    {
                        MaterialAttribute materialAttributes = null;
                        using (MaterialAttributeServiceClient clientOfMaterialAttribute = new MaterialAttributeServiceClient())
                        {
                            MaterialAttributeKey materialAttributeKey = new MaterialAttributeKey()
                            {
                                MaterialCode = materialCode,
                                AttributeName = "ProductType"
                            };
                            MethodReturnResult<MaterialAttribute> resultOfMaterialAttribute = clientOfMaterialAttribute.Get(materialAttributeKey);
                            if (resultOfMaterialAttribute.Code > 0)
                            {
                                return Content(resultOfMaterialAttribute.Message);
                            }
                            materialAttributes = resultOfMaterialAttribute.Data;
                        }
                        //获取包装第一块明细数据。
                        using (PackageQueryServiceClient packageClientOfSE = new PackageQueryServiceClient())
                        {
                            PagingConfig cfgSE = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("ItemNo='1' AND Key.PackageNo='{0}'", model.PackageNo),
                                OrderBy = "ItemNo"
                            };
                            MethodReturnResult<IList<PackageDetail>> packageDetailresultOfSE = packageClientOfSE.GetDetail(ref cfgSE);
                            if (packageDetailresultOfSE.Code > 0 || packageDetailresultOfSE.Data == null || packageDetailresultOfSE.Data.Count == 0)
                            {
                                return Content(string.Format("未获取到包装号{0}的第一块组件包装明细信息", model.PackageNo));
                            }
                            else
                            {
                                PackageDetail obj = packageDetailresultOfSE.Data.FirstOrDefault();
                                if (obj != null)
                                {
                                    using (LotQueryServiceClient lotClientOfSE = new LotQueryServiceClient())
                                    {
                                        MethodReturnResult<Lot> lotResultOfSE = lotClientOfSE.Get(obj.Key.ObjectNumber);

                                        if (lotResultOfSE.Code == 0 && lotResultOfSE.Data != null)
                                        {
                                            string valueOf = materialAttributes.Value.Trim();
                                            int indexOfType1 = valueOf.IndexOf('*');
                                            if (lotResultOfSE.Data.Attr3 != null && lotResultOfSE.Data.Attr3 != "")
                                            {
                                                row.ProductType = string.Format("S{0}{1}-{2}{3}-1WA"
                                                                    , valueOf.Substring(0, indexOfType1)
                                                                    , wop.PowerName.Substring(0, 3)
                                                                    , material.MainRawQtyPerLot
                                                                    , valueOf.Substring(valueOf.Length - 3)
                                                                );
                                            }
                                            else
                                            {
                                                row.ProductType = string.Format("{0}{1}-{2}{3}-1WA"
                                                                    , valueOf.Substring(0, indexOfType1)
                                                                    , wop.PowerName.Substring(0, 3)
                                                                    , material.MainRawQtyPerLot
                                                                    , valueOf.Substring(valueOf.Length - 3)
                                                                );
                                            }
                                        }
                                        else
                                        {
                                            return Content(string.Format("未获取到包装号{0}的第一块组件{1}批次信息", model.PackageNo, obj.Key.ObjectNumber));
                                        }
                                    }
                                }
                                else
                                {
                                    return Content(string.Format("未获取到包装号{0}的第一块组件包装明细信息", model.PackageNo));
                                }
                            }
                        }
                    }
                    
                    else if (material.Key == "1202020119" || material.Key == "1202020121")
                    {
                        row.ProductType = string.Format("YL-{0}P-29b"
                                                    , wop.PowerName.Substring(0, 3));
                    }
                    //晋能常规清单
                    else
                    {
                        //row.ProductType = string.Format("{0}{1}-{2}"
                        //                            , material.Name.Substring(0, 4)
                        //                            , material.MainRawQtyPerLot
                        //                            , wop.PowerName.Substring(0, 3));
                        int indexOfType = material.Name.IndexOf('-');
                        if (indexOfType > 0)
                        {
                            row.ProductType = string.Format("{0}-{1}"
                                                    , material.Name.Substring(0, indexOfType)
                                                    , wop.PowerName.Substring(0, 3));
                        }
                        else
                        {
                            row.ProductType = string.Format("{0}-{1}"
                                                    , material.Name.Substring(0, 6)
                                                    , wop.PowerName.Substring(0, 3));
                        }
                    }

                    row.MaterialClass = material.Class;                                      //物料类型
                    row.ProductSpec = material.Spec;                                         //产品规格
                    row.Description = material.Description;                                  //描述
                    row.OrderNumber = orderNumber;                                           //工单
                    row.PackageNo = model.PackageNo;                                         //包装号

                    if (dt.Data.Tables.Count > 1 && dt.Data.Tables[1] != null && dt.Data.Tables[1].Rows.Count > 0)       //总数
                    {
                        //int counts = dt.Data.Tables[0].Rows.Count + dt.Data.Tables[1].Rows.Count;
                        row.Count = (dt.Data.Tables[0].Rows.Count + dt.Data.Tables[1].Rows.Count).ToString();
                    }
                    else
                    {
                        row.Count = dt.Data.Tables[0].Rows.Count.ToString();
                    }

                    row.PowerName = wop == null ? string.Empty : wop.PowerName;
                    row.PowerTolerance = wop == null ? string.Empty : wop.PowerDifference;

                    row.Color = color;

                    if (blIsEnglish)
                    {
                        row.OrderNumber = orderNumber.Substring(4, 8);
                        if (strPackagePrintLabel == "SEPackageList-EN.rdlc")
                        {
                            if (row.Color == "深蓝")
                            {
                                row.Color = "Dark Blue";
                            }
                            if (row.Color == "浅蓝")
                            {
                                row.Color = "Light Blue";
                            }
                            if (row.Color == "正蓝")
                            {
                                row.Color = "Blue";
                            }
                        }
                        else
                        {
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
                            row.PowerName = wop == null ? string.Empty : wop.PowerName.Substring(0, 3);
                        }
                    }
                    else
                    {
                        if (strPackagePrintLabel == "YL-JN-PackageList-2.rdlc")
                        {
                            if (row.Color == "深蓝")
                            {
                                row.Color = "C3";
                            }
                            if (row.Color == "浅蓝")
                            {
                                row.Color = "C2";
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
                    }

                    #region 协鑫永能/晋能/张家港电池片类型加颜色
                    if (workOrderAttribute != null)
                    {
                        string attrOfOrderCell = workOrderAttribute.AttributeValue.Trim();
                        if (row.Color == "Dark Blue/深蓝")
                        {
                            row.CellType = attrOfOrderCell + "D";
                        }
                        else if (row.Color == "Light Blue/浅蓝")
                        {
                            row.CellType = attrOfOrderCell + "L";
                        }
                        else
                        {
                            row.CellType = attrOfOrderCell;
                        }
                    }
                    #endregion

                    #region 协鑫永能/晋能/张家港柜号流水号
                    row.ChestNo = chestOfPackage;
                    #endregion

                    #region 协鑫张家港或晋能ORDER NO: 103A车间：D001 / 102A车间：D002 / 102B车间：D003
                    if (row.PackageNo.Substring(0, 3) == "27M" || row.PackageNo.Substring(0, 3) == "27P"
                        || row.PackageNo.Substring(0, 3) == "64M" || row.PackageNo.Substring(0, 3) == "64P")
                    {
                        if (workOrder.LocationName == "103A")
                        {
                            row.OrderNumber = "D001";
                        }
                        if (workOrder.LocationName == "102A")
                        {
                            row.OrderNumber = "D002";
                        }
                        if (workOrder.LocationName == "102B")
                        {
                            row.OrderNumber = "D003";
                        }
                    }
                    #endregion

                    row.PowerSubCode = pssubcode;
                    row.MaterialCode = materialCode;
                    IList<string> lstColor = new List<string>();    //托内颜色数组
                    string sColor = "";
                    string sFullColor = "";
                    bool isfind = false;
                    string packageLots = "";
                    string sss = "";

                    #region 德国
                    if (strPackagePrintLabel == "GERMAN.rdlc")
                    {
                        if (dt.Data != null && dt.Data.Tables.Count > 1)
                        {
                            if (dt.Data.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dt.Data.Tables[0].Rows.Count; j++)
                                {
                                    packageLots += dt.Data.Tables[0].Rows[j]["OBJECT_NUMBER"].ToString() + ";";
                                }
                            }
                            if (dt.Data.Tables[1].Rows.Count > 0)
                            {
                                for (int j = 0; j < dt.Data.Tables[1].Rows.Count; j++)
                                {
                                    packageLots += dt.Data.Tables[1].Rows[j]["OBJECT_NUMBER"].ToString() + ";";
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < dt.Data.Tables[0].Rows.Count; j++)
                            {
                                packageLots += dt.Data.Tables[0].Rows[j]["OBJECT_NUMBER"].ToString() + ";";
                            }
                        }
                        Image img = QRCodeSave(packageLots);
                        byte[] packageLotNosOfByte = ImageToBytes(img);
                        sss = ConvertImageToBase64(img, ImageFormat.Png);
                        row.PackageLotNos = packageLotNosOfByte;
                        //row.PackageLotNos = sss;
                    }
                    #endregion

                    for (int j = 0; j < dt.Data.Tables[0].Rows.Count; j++)
                    {
                        //添加包装明细
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
                        detailRow.PowerSubCode = dt.Data.Tables[0].Rows[j]["PS_SUBCODE"].ToString();
                        ds.PackageDetail.AddPackageDetailRow(detailRow);

                        //将不同的颜色加入颜色数组
                        #region
                        sColor = dt.Data.Tables[0].Rows[j]["COLOR"].ToString();

                        if (blIsEnglish)
                        {
                            if (strPackagePrintLabel == "SEPackageList-EN.rdlc")
                            {
                                if (sColor == "深蓝")
                                {
                                    sColor = "Dark Blue";
                                }
                                if (sColor == "浅蓝")
                                {
                                    sColor = "Light Blue";
                                }
                                if (sColor == "正蓝")
                                {
                                    sColor = "Blue";
                                }
                            }
                            else
                            {
                                //Light blue，Blue，Dark blue（3选1）
                                if (sColor == "深蓝")
                                {
                                    sColor = "Navy Blue";
                                }
                                if (sColor == "浅蓝")
                                {
                                    sColor = "baby Blue";
                                }
                                if (sColor == "正蓝")
                                {
                                    sColor = "Blue";
                                }
                            }
                        }
                        else
                        {
                            if (sColor == "深蓝")
                            {
                                sColor = "Dark Blue/深蓝";
                            }
                            if (sColor == "浅蓝")
                            {
                                sColor = "Light Blue/浅蓝";
                            }
                            if (sColor == "正蓝")
                            {
                                sColor = "Blue/正蓝";
                            }
                        }

                        isfind = false;

                        //判断颜色是否存在
                        foreach (string s in lstColor)
                        {
                            if (s == sColor)
                            {
                                isfind = true;

                                break;
                            }
                        }

                        if (!isfind)
                        {
                            lstColor.Add(sColor);

                            if (sFullColor == "")
                            {
                                sFullColor = sColor;
                            }
                            else
                            {
                                sFullColor += "\\" + sColor;
                            }

                        }
                        #endregion
                    }

                    if (dt.Data.Tables.Count > 1)
                    {
                        for (int j = 0; j < dt.Data.Tables[1].Rows.Count; j++)
                        {
                            //添加包装明细
                            PackageListDataSet.PackageDetailRow detailRow = ds.PackageDetail.NewPackageDetailRow();
                            detailRow.PackageNo = dt.Data.Tables[1].Rows[j]["PACKAGE_NO"].ToString();
                            detailRow.ItemNo = Convert.ToInt16(dt.Data.Tables[1].Rows[j]["ITEM_NO"].ToString());
                            detailRow.LotNo = dt.Data.Tables[1].Rows[j]["OBJECT_NUMBER"].ToString();
                            detailRow.IMP = dt.Data.Tables[1].Rows[j]["COEF_IMAX"].ToString();
                            detailRow.ISC = dt.Data.Tables[1].Rows[j]["COEF_ISC"].ToString();
                            detailRow.PM = dt.Data.Tables[1].Rows[j]["COEF_PMAX"].ToString();
                            detailRow.VMP = dt.Data.Tables[1].Rows[j]["COEF_VMAX"].ToString();
                            detailRow.VOC = dt.Data.Tables[1].Rows[j]["COEF_VOC"].ToString();
                            detailRow.FF = dt.Data.Tables[1].Rows[j]["COEF_FF"].ToString();
                            detailRow.PowerSubCode = dt.Data.Tables[1].Rows[j]["PS_SUBCODE"].ToString();
                            ds.PackageDetail.AddPackageDetailRow(detailRow);

                            //将不同的颜色加入颜色数组
                            #region
                            sColor = dt.Data.Tables[1].Rows[j]["COLOR"].ToString();

                            if (blIsEnglish)
                            {
                                if (strPackagePrintLabel == "SEPackageList-EN.rdlc")
                                {
                                    if (sColor == "深蓝")
                                    {
                                        sColor = "Dark Blue";
                                    }
                                    if (sColor == "浅蓝")
                                    {
                                        sColor = "Light Blue";
                                    }
                                    if (sColor == "正蓝")
                                    {
                                        sColor = "Blue";
                                    }
                                }
                                else
                                {
                                    //Light blue，Blue，Dark blue（3选1）
                                    if (sColor == "深蓝")
                                    {
                                        sColor = "Navy Blue";
                                    }
                                    if (sColor == "浅蓝")
                                    {
                                        sColor = "baby Blue";
                                    }
                                    if (sColor == "正蓝")
                                    {
                                        sColor = "Blue";
                                    }
                                }
                            }
                            else
                            {
                                if (sColor == "深蓝")
                                {
                                    sColor = "Dark Blue/深蓝";
                                }
                                if (sColor == "浅蓝")
                                {
                                    sColor = "Light Blue/浅蓝";
                                }
                                if (sColor == "正蓝")
                                {
                                    sColor = "Blue/正蓝";
                                }
                            }

                            isfind = false;

                            //判断颜色是否存在
                            foreach (string s in lstColor)
                            {
                                if (s == sColor)
                                {
                                    isfind = true;

                                    break;
                                }
                            }

                            if (!isfind)
                            {
                                lstColor.Add(sColor);

                                if (sFullColor == "")
                                {
                                    sFullColor = sColor;
                                }
                                else
                                {
                                    sFullColor += "\\" + sColor;
                                }

                            }

                            #endregion
                        }
                    }
                    
                    if (strPackagePrintLabel == "PackageList_DongFangEN.rdlc")
                    {
                        row.Color = sFullColor;
                    }

                    ds.Package.AddPackageRow(row);

                    if (dt.Data.Tables.Count >= 1)
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
                            if (grade != "A")
                            {
                                localReport.ReportPath = Server.MapPath(string.Format("~/RDLC/{0}", "PackageList_BD.rdlc"));
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(strPackagePrintLabel) == false && strPackagePrintLabel.Length > 0)
                                {
                                    localReport.ReportPath = Server.MapPath(string.Format("~/RDLC/{0}", strPackagePrintLabel));
                                }
                                else
                                {
                                    localReport.ReportPath = Server.MapPath("~/RDLC/PackageList_New.rdlc");
                                }
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

        /// <summary>
        /// 显示出库包装清单打印(年月日变更)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ShowDeliverPackageListReport(PackingListPrintQueryViewModel model)
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
                    string strPackagePrintLabel = this.GetPackagePrintLabel(dt, dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString(), dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString());
                    bool blIsEnglish = false;

                    //strPackagePrintLabel = "PackageList_DongFangEN.rdlc";

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
                        row.ProductType = string.Format("{0}{1}-{2}"
                                                    , material.Name.Substring(0, 4)
                                                    , material.MainRawQtyPerLot
                                                    , wop.PowerName.Substring(0, 3));

                    }
                    //SE单晶60包装清单
                    else if (strPackagePrintLabel == "SEPackageList.rdlc")
                    {
                        //获取包装第一块明细数据。
                        using (PackageQueryServiceClient packageClientOfSE = new PackageQueryServiceClient())
                        {
                            PagingConfig cfgSE = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("ItemNo='1' or Key.PackageNo='{0}'", model.PackageNo),
                                OrderBy = "Key.ItemNo"
                            };
                            MethodReturnResult<IList<PackageDetail>> packageDetailresultOfSE = packageClientOfSE.GetDetail(ref cfgSE);
                            if (packageDetailresultOfSE.Code > 0 || packageDetailresultOfSE.Data == null || packageDetailresultOfSE.Data.Count == 0)
                            {
                                return Content(string.Format("未获取到包装号{0}的第一块组件包装明细信息", model.PackageNo));
                            }
                            else
                            {
                                PackageDetail obj = packageDetailresultOfSE.Data.FirstOrDefault();
                                if (obj != null)
                                {
                                    using (LotQueryServiceClient lotClientOfSE = new LotQueryServiceClient())
                                    {
                                        MethodReturnResult<Lot> lotResultOfSE = lotClientOfSE.Get(obj.Key.ObjectNumber);

                                        if (lotResultOfSE.Code == 0 && lotResultOfSE.Data != null)
                                        {
                                            if (lotResultOfSE.Data.Attr3 != null && lotResultOfSE.Data.Attr3 != "")
                                            {
                                                row.ProductType = string.Format("SPV{0}-{1}MMJ"
                                                    , wop.PowerName.Substring(0, 3)
                                                    , material.MainRawQtyPerLot);
                                            }
                                            else
                                            {
                                                row.ProductType = string.Format("PV{0}-{1}MMJ"
                                                    , wop.PowerName.Substring(0, 3)
                                                    , material.MainRawQtyPerLot);
                                            }
                                        }
                                        else
                                        {
                                            return Content(string.Format("未获取到包装号{0}的第一块组件{1}批次信息", model.PackageNo, obj.Key.ObjectNumber));
                                        }
                                    }
                                }
                                else
                                {
                                    return Content(string.Format("未获取到包装号{0}的第一块组件包装明细信息", model.PackageNo));
                                }
                            }
                        }
                    }
                    else
                    {
                        row.ProductType = string.Format("{0}{1}-{2}"
                                                    , material.Name.Substring(0, 4)
                                                    , material.MainRawQtyPerLot
                                                    , wop.PowerName.Substring(0, 3));
                    }
                    row.MaterialClass = material.Class;                                      //物料类型
                    row.ProductSpec = material.Spec;                                         //产品规格
                    row.Description = material.Description;                                  //描述
                    row.OrderNumber = dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString();  //工单

                    //处理包装好年月（16年10月1日之前）                    
                    string packageNo = dt.Data.Tables[0].Rows[0]["PACKAGE_NO"].ToString();      //包装号

                    //取得年份                    
                    if (packageNo.Substring(0, 2) == "15" || (packageNo.Substring(0, 2) == "16" && int.Parse(packageNo.Substring(2, 2)) <= 10))
                    {
                        packageNo = (int.Parse(packageNo.Substring(0, 2).ToString()) + 8).ToString() +
                                    (int.Parse(packageNo.Substring(2, 2).ToString()) + 8).ToString("00") +
                                    packageNo.Substring(4, packageNo.Length - 4).ToString();
                    }

                    row.PackageNo = packageNo;      //包装号                    

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

                    //ds.Package.AddPackageRow(row);

                    IList<string> lstColor = new List<string>();    //托内颜色数组
                    string sColor = "";
                    string sFullColor = "";
                    bool isfind = false;

                    for (int j = 0; j < dt.Data.Tables[0].Rows.Count; j++)
                    {
                        //添加包装明细
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

                        //将不同的颜色加入颜色数组
                        #region
                        sColor = dt.Data.Tables[0].Rows[j]["COLOR"].ToString();

                        if (blIsEnglish)
                        {
                            //Light blue，Blue，Dark blue（3选1）
                            if (sColor == "深蓝")
                            {
                                sColor = "Navy Blue";
                            }
                            if (sColor == "浅蓝")
                            {
                                sColor = "baby Blue";
                            }
                            if (sColor == "正蓝")
                            {
                                sColor = "Blue";
                            }
                        }
                        else
                        {
                            if (sColor == "深蓝")
                            {
                                sColor = "Dark Blue/深蓝";
                            }
                            if (sColor == "浅蓝")
                            {
                                sColor = "Light Blue/浅蓝";
                            }
                            if (sColor == "正蓝")
                            {
                                sColor = "Blue/正蓝";
                            }
                        }

                        isfind = false;

                        //判断颜色是否存在
                        foreach (string s in lstColor)
                        {
                            if (s == sColor)
                            {
                                isfind = true;

                                break;
                            }
                        }

                        if (!isfind)
                        {
                            lstColor.Add(sColor);

                            if (sFullColor == "")
                            {
                                sFullColor = sColor;
                            }
                            else
                            {
                                sFullColor += "\\" + sColor;
                            }

                        }

                        #endregion
                    }

                    if (strPackagePrintLabel == "PackageList_DongFangEN.rdlc")
                    {
                        row.Color = sFullColor;
                    }

                    ds.Package.AddPackageRow(row);

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

        public string GetPackagePrintLabel(MethodReturnResult<DataSet> dt, string strWorkOrder, string strMaterialCode)
        {
            string strPackagePrintLabel = "";
            string strPrintLabelCode = "";
            using (WorkOrderPrintSetServiceClient client = new WorkOrderPrintSetServiceClient())
            {
                PagingConfig cfg = new PagingConfig
                {
                    IsPaging = false,
                    Where = string.Format(@" Key.OrderNumber='{0}' and Key.MaterialCode='{1}' and Key.LabelCode like 'JNW%'", strWorkOrder, strMaterialCode),
                    OrderBy = " Key.LabelCode desc "
                };
                MethodReturnResult<IList<WorkOrderPrintSet>> result = client.Get(ref cfg);
                if (result != null && result.Data != null && result.Data.Count > 0)
                {
                    //如果特定工单挂了两个包装清单
                    if (result.Data.Count > 1 && strWorkOrder == "特定工单")
                    {
                        //如果包装号档位信息为275W
                        if (dt.Data.Tables[0].Rows[0]["PM_NAME"].ToString().Trim() == "特定功率" || dt.Data.Tables[1].Rows[0]["PM_NAME"].ToString().Trim() == "特定功率")
                        {
                            strPrintLabelCode = result.Data[0].Key.LabelCode;
                        }
                        else
                        {
                            strPrintLabelCode = result.Data[1].Key.LabelCode;
                        }
                    }
                    else
                    {
                        strPrintLabelCode = result.Data[0].Key.LabelCode;
                    }
                }
            }

            if (string.IsNullOrEmpty(strPrintLabelCode) == false && strPrintLabelCode.Length > 0)
            {
                using (PrintLabelServiceClient client = new PrintLabelServiceClient())
                {
                    MethodReturnResult<PrintLabel> resultOfPrintLabel = client.Get(strPrintLabelCode);
                    if (resultOfPrintLabel != null && resultOfPrintLabel.Code == 0)
                    {
                        PrintLabel printLabel = resultOfPrintLabel.Data;
                        if (printLabel != null)
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
                            if (result1.Data.Count != 2 || result1.Data[0].Key.PackageNo != result1.Data[1].Key.PackageNo)
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
                    string strPackagePrintLabel = this.GetPackagePrintLabel(dt, dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString(), dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString());
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
                        row.ProductType = string.Format("{0}{1}-{2}"
                                                    , material.Name.Substring(0, 4)
                                                    , material.MainRawQtyPerLot
                                                    , wop.PowerName.Substring(0, 3));

                    }
                    //SE单晶60包装清单
                    else if (strPackagePrintLabel == "SEPackageList.rdlc")
                    {
                        //获取包装第一块明细数据。
                        using (PackageQueryServiceClient packageClientOfSE = new PackageQueryServiceClient())
                        {
                            PagingConfig cfgSE = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("ItemNo='1' or Key.PackageNo='{0}'", model.PackageNo),
                                OrderBy = "Key.ItemNo"
                            };
                            MethodReturnResult<IList<PackageDetail>> packageDetailresultOfSE = packageClientOfSE.GetDetail(ref cfgSE);
                            if (packageDetailresultOfSE.Code > 0 || packageDetailresultOfSE.Data == null || packageDetailresultOfSE.Data.Count == 0)
                            {
                                return Content(string.Format("未获取到包装号{0}的第一块组件包装明细信息", model.PackageNo));
                            }
                            else
                            {
                                PackageDetail obj = packageDetailresultOfSE.Data.FirstOrDefault();
                                if (obj != null)
                                {
                                    using (LotQueryServiceClient lotClientOfSE = new LotQueryServiceClient())
                                    {
                                        MethodReturnResult<Lot> lotResultOfSE = lotClientOfSE.Get(obj.Key.ObjectNumber);

                                        if (lotResultOfSE.Code == 0 && lotResultOfSE.Data != null)
                                        {
                                            if (lotResultOfSE.Data.Attr3 != null && lotResultOfSE.Data.Attr3 != "")
                                            {
                                                row.ProductType = string.Format("SPV{0}-{1}MMJ"
                                                    , wop.PowerName.Substring(0, 3)
                                                    , material.MainRawQtyPerLot);
                                            }
                                            else
                                            {
                                                row.ProductType = string.Format("PV{0}-{1}MMJ"
                                                    , wop.PowerName.Substring(0, 3)
                                                    , material.MainRawQtyPerLot);
                                            }
                                        }
                                        else
                                        {
                                            return Content(string.Format("未获取到包装号{0}的第一块组件{1}批次信息", model.PackageNo, obj.Key.ObjectNumber));
                                        }
                                    }
                                }
                                else
                                {
                                    return Content(string.Format("未获取到包装号{0}的第一块组件包装明细信息", model.PackageNo));
                                }
                            }
                        }
                    }
                    else
                    {
                        row.ProductType = string.Format("{0}{1}-{2}"
                                                    , material.Name.Substring(0, 4)
                                                    , material.MainRawQtyPerLot
                                                    , wop.PowerName.Substring(0, 3));
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

        /// <summary>
        /// 显示返工包装清单打印
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ShowXXPackageListReport(PackingListPrintQueryViewModel model)
        {
            model.PackageNo = model.PackageNo;
            MethodReturnResult result = new MethodReturnResult();
            PackageListDataSet ds = new PackageListDataSet();
            IList<PackageDetail> lstPackageDetail = new List<PackageDetail>();
            IList<IVTestData> lstIVTestData = new List<IVTestData>();
            IList<Lot> lstLot = new List<Lot>();

            try
            {
                #region 打印清单逻辑
                string orderNumber = null;
                string materialCode = null;
                string color = null;
                string pscode = null;
                string psitemno = null;
                string pssubcode = null;
                double qty = 0;
                string grade = null;

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
                    MethodReturnResult<DataSet> dtOfOEM = client.GetOEMpackagelist(param);
                    if (dtOfOEM.Data != null && dtOfOEM.Data.Tables.Count > 0 && dtOfOEM.Data.Tables[0].Rows.Count > 0)
                    {
                        DataTable oemDataTable = new DataTable();
                        oemDataTable = dtOfOEM.Data.Tables[0];
                        oemDataTable.TableName = "OemDataTable";
                        dt.Data.Tables.Add(oemDataTable.Copy());
                    }

                    //判断批次是否存在
                    if (dt.Code > 0 || dt.Data == null)
                    {
                        return Content(string.Format("未找到批次号{0}的信息。", model.LotNumber1));
                    }
                    else
                    {
                        if (dt.Data.Tables[0] != null && dt.Data.Tables[0].Rows.Count > 0)
                        {
                            orderNumber = dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString();  //工单号(第一行数据的工单号)
                            materialCode = dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString();//物料编码
                            color = dt.Data.Tables[0].Rows[0]["COLOR"].ToString();
                            pscode = dt.Data.Tables[0].Rows[0]["PS_CODE"].ToString();
                            psitemno = dt.Data.Tables[0].Rows[0]["PS_ITEM_NO"].ToString();
                            pssubcode = dt.Data.Tables[0].Rows[0]["PS_SUBCODE"].ToString();
                            grade = dt.Data.Tables[0].Rows[0]["GRADE"].ToString();
                        }
                        else if (dtOfOEM.Data != null && dtOfOEM.Data.Tables.Count > 0 && dtOfOEM.Data.Tables[0].Rows.Count > 0)
                        {
                            orderNumber = dt.Data.Tables[1].Rows[0]["ORDER_NUMBER"].ToString();  //工单号(第一行数据的工单号)
                            materialCode = dt.Data.Tables[1].Rows[0]["MATERIAL_CODE"].ToString();//物料编码
                            color = dt.Data.Tables[1].Rows[0]["COLOR"].ToString();
                            pscode = dt.Data.Tables[1].Rows[0]["PS_CODE"].ToString();
                            psitemno = dt.Data.Tables[1].Rows[0]["PS_ITEM_NO"].ToString();
                            pssubcode = dt.Data.Tables[1].Rows[0]["PS_SUBCODE"].ToString();
                            grade = dt.Data.Tables[1].Rows[0]["GRADE"].ToString();
                        }
                        else
                        {
                            return Content(string.Format("请进行IV修正或检查批次{0}所在工单分档代码与批次IV数据中分档代码是否一致。",
                                model.LotNumber1));
                        }
                    }

                    //判断物料编码或是工单是否为空
                    if (orderNumber == null || materialCode == null)
                    {
                        return Content(string.Format("未找到批次号{0}的工单信息。", model.LotNumber1));
                    }

                    WorkOrder workOrder = null;
                    //获取工单信息
                    using (WorkOrderServiceClient clientWorkOrder = new WorkOrderServiceClient())
                    {
                        workOrder = clientWorkOrder.Get(orderNumber).Data;
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
                    //string strPackagePrintLabel = this.GetPackagePrintLabel(dt, dt.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString(), dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString());
                    string strPackagePrintLabel = "";
                    bool blIsEnglish = false;

                    using (BaseAttributeValueServiceClient labNameClient = new BaseAttributeValueServiceClient())
                    {
                        IList<BaseAttributeValue> lstBaseAttributeValue = new List<BaseAttributeValue>();
                        PagingConfig pg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("Key.AttributeName='Available' and Value = 'true'")
                        };
                        MethodReturnResult<IList<BaseAttributeValue>> r = labNameClient.Get(ref pg);
                        if (r.Code <= 0 && r.Data != null)
                        {
                            lstBaseAttributeValue = r.Data;
                            foreach (BaseAttributeValue item in lstBaseAttributeValue)
                            {
                                BaseAttributeValueKey BaseAttributeValueKey = new BaseAttributeValueKey()
                                {
                                    CategoryName = item.Key.CategoryName,
                                    AttributeName = "Name",
                                    ItemOrder = item.Key.ItemOrder
                                };
                                MethodReturnResult<BaseAttributeValue> rs = labNameClient.Get(BaseAttributeValueKey);
                                if (rs.Data != null && rs.Code <= 0)
                                {
                                    strPackagePrintLabel = rs.Data.Value.ToString();
                                    //strPackagePrintLabel = "XX-YN-P660-TEST.rdlc";
                                }
                            }
                        }
                    }

                    if (strPackagePrintLabel != "" && strPackagePrintLabel.Length > 0)
                    {
                        if (strPackagePrintLabel.IndexOf("EN") > 0)
                        {
                            blIsEnglish = true;
                        }
                    }

                    //获取工单分档规则
                    WorkOrderPowerset wop = null;
                    if (dt.Data != null&& dt.Data.Tables[0].Rows.Count>0 )
                    {
                        if (dt.Data.Tables[0].Rows[0]["PS_CODE"].ToString() != null)
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
                    }
                    if (dt.Data != null && dt.Data.Tables.Count > 1)
                    {
                        if (dt.Data.Tables[1].Rows[0]["PS_CODE"].ToString() != null)
                        {
                            using (WorkOrderPowersetServiceClient client1 = new WorkOrderPowersetServiceClient())
                            {
                                MethodReturnResult<WorkOrderPowerset> result1 = client1.Get(new WorkOrderPowersetKey()
                                {
                                    Code = dt.Data.Tables[1].Rows[0]["PS_CODE"].ToString(),
                                    ItemNo = Convert.ToInt16(dt.Data.Tables[1].Rows[0]["PS_ITEM_NO"].ToString()),
                                    OrderNumber = dt.Data.Tables[1].Rows[0]["ORDER_NUMBER"].ToString(),
                                    MaterialCode = dt.Data.Tables[1].Rows[0]["MATERIAL_CODE"].ToString()
                                });
                                wop = result1.Data;
                            }
                        }
                    }


                    //根据物料编码获取物料数据，进一步获取产品类型。
                    Material material = null;
                    using (MaterialServiceClient client2 = new MaterialServiceClient())
                    {
                        MethodReturnResult<Material> result2 = new MethodReturnResult<Material>();
                        if (dt.Data.Tables[0].Rows.Count > 0)
                        {
                            result2 = client2.Get(dt.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString());
                        }
                        else if (dt.Data.Tables[1].Rows.Count > 0)
                        {
                            result2 = client2.Get(dt.Data.Tables[1].Rows[0]["MATERIAL_CODE"].ToString());
                        }
                        if (result2.Code > 0)
                        {
                            return Content(result.Message);
                        }
                        material = result2.Data;
                    }

                    //根据工单获取工单电池片类型属性数据
                    WorkOrderAttribute workOrderAttribute = null;
                    using (WorkOrderAttributeServiceClient clientOfOrderMattr = new WorkOrderAttributeServiceClient())
                    {
                        WorkOrderAttributeKey workOrderAttributeKey = new WorkOrderAttributeKey()
                        {
                            OrderNumber = orderNumber,
                            AttributeName = "CellType"
                        };

                        MethodReturnResult<WorkOrderAttribute> result2 = clientOfOrderMattr.Get(workOrderAttributeKey);
                        workOrderAttribute = result2.Data;
                    }

                    //根据包装号获取柜号信息
                    Package packageToChest = null;
                    string chestOfPackage = "";
                    using (PackageQueryServiceClient clientOfPackage = new PackageQueryServiceClient())
                    {
                        MethodReturnResult<Package> result2 = clientOfPackage.Get(model.PackageNo);
                        packageToChest = result2.Data;
                        if (packageToChest != null
                            && packageToChest.ContainerNo != null
                            && packageToChest.ContainerNo != "")
                        {
                            chestOfPackage = packageToChest.ContainerNo.Trim();
                        }
                    }

                    //组件包装数据。
                    PackageListDataSet.PackageRow row = ds.Package.NewPackageRow();
                    if (strPackagePrintLabel == "PackageList_SunEN.rdlc")
                    {
                        row.ProductType = string.Format("SE-{1}{0}NPB-A4"
                                                    , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
                                                    , material.Key.StartsWith("1201") ? "M" : "P");
                    }
                    else if (strPackagePrintLabel == "PackageList_PERC.rdlc")
                    {
                        row.ProductType = string.Format("{0}{1}-{2}"
                                                    , material.Name.Substring(0, 4)
                                                    , material.MainRawQtyPerLot
                                                    , wop.PowerName.Substring(0, 3));

                    }
                    //SE单晶60包装清单
                    else if (strPackagePrintLabel == "SEPackageList-EN.rdlc")
                    {
                        MaterialAttribute materialAttributes = null;
                        using (MaterialAttributeServiceClient clientOfMaterialAttribute = new MaterialAttributeServiceClient())
                        {
                            MaterialAttributeKey materialAttributeKey = new MaterialAttributeKey()
                            {
                                MaterialCode = materialCode,
                                AttributeName = "ProductType"
                            };
                            MethodReturnResult<MaterialAttribute> resultOfMaterialAttribute = clientOfMaterialAttribute.Get(materialAttributeKey);
                            if (resultOfMaterialAttribute.Code > 0)
                            {
                                return Content(resultOfMaterialAttribute.Message);
                            }
                            materialAttributes = resultOfMaterialAttribute.Data;
                        }
                        //获取包装第一块明细数据。
                        using (PackageQueryServiceClient packageClientOfSE = new PackageQueryServiceClient())
                        {
                            PagingConfig cfgSE = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("ItemNo='1' AND Key.PackageNo='{0}'", model.PackageNo),
                                OrderBy = "ItemNo"
                            };
                            MethodReturnResult<IList<PackageDetail>> packageDetailresultOfSE = packageClientOfSE.GetDetail(ref cfgSE);
                            if (packageDetailresultOfSE.Code > 0 || packageDetailresultOfSE.Data == null || packageDetailresultOfSE.Data.Count == 0)
                            {
                                return Content(string.Format("未获取到包装号{0}的第一块组件包装明细信息", model.PackageNo));
                            }
                            else
                            {
                                PackageDetail obj = packageDetailresultOfSE.Data.FirstOrDefault();
                                if (obj != null)
                                {
                                    using (LotQueryServiceClient lotClientOfSE = new LotQueryServiceClient())
                                    {
                                        MethodReturnResult<Lot> lotResultOfSE = lotClientOfSE.Get(obj.Key.ObjectNumber);

                                        if (lotResultOfSE.Code == 0 && lotResultOfSE.Data != null)
                                        {
                                            string valueOf = materialAttributes.Value.Trim();
                                            int indexOfType1 = valueOf.IndexOf('*');
                                            if (lotResultOfSE.Data.Attr3 != null && lotResultOfSE.Data.Attr3 != "")
                                            {
                                                row.ProductType = string.Format("S{0}{1}-{2}{3}-1WA"
                                                                    , valueOf.Substring(0, indexOfType1)
                                                                    , wop.PowerName.Substring(0, 3)
                                                                    , material.MainRawQtyPerLot
                                                                    , valueOf.Substring(valueOf.Length - 3)
                                                                );
                                            }
                                            else
                                            {
                                                row.ProductType = string.Format("{0}{1}-{2}{3}-1WA"
                                                                    , valueOf.Substring(0, indexOfType1)
                                                                    , wop.PowerName.Substring(0, 3)
                                                                    , material.MainRawQtyPerLot
                                                                    , valueOf.Substring(valueOf.Length - 3)
                                                                );
                                            }
                                        }
                                        else
                                        {
                                            return Content(string.Format("未获取到包装号{0}的第一块组件{1}批次信息", model.PackageNo, obj.Key.ObjectNumber));
                                        }
                                    }
                                }
                                else
                                {
                                    return Content(string.Format("未获取到包装号{0}的第一块组件包装明细信息", model.PackageNo));
                                }
                            }
                        }
                    }                    
                    else if (material.Key == "1202020119" || material.Key == "1202020121")
                    {
                        row.ProductType = string.Format("YL-{0}P-29b"
                                                    , wop.PowerName.Substring(0, 3));
                    }
                    else
                    {
                        row.ProductType = string.Format("{0}{1}-{2}"
                                                    , material.Name.Substring(0, 4)
                                                    , material.MainRawQtyPerLot
                                                    , wop.PowerName.Substring(0, 3));
                    }
                    row.MaterialClass = material.Class;                                      //物料类型
                    row.ProductSpec = material.Spec;                                         //产品规格
                    row.Description = material.Description;                                  //描述
                    row.OrderNumber = orderNumber;                                           //工单

                    //处理包装好年月（16年10月1日之前）                    
                    string packageNo = model.PackageNo;      //包装号

                    ////取得年份                    
                    //if (packageNo.Substring(0, 2) == "15" || (packageNo.Substring(0, 2) == "16" && int.Parse(packageNo.Substring(2, 2)) <= 10))
                    //{
                    //    packageNo = (int.Parse(packageNo.Substring(0, 2).ToString()) + 8).ToString() +
                    //                (int.Parse(packageNo.Substring(2, 2).ToString()) + 8).ToString("00") +
                    //                packageNo.Substring(4, packageNo.Length - 4).ToString();
                    //}

                    #region 协鑫275 特殊包装规则
                    if (strPackagePrintLabel == "XXBACK275PackageListZhong.rdlc")
                    {
                        if (wop.PowerName == "275W")
                        {
                            int yearOfPackage = Convert.ToInt32(packageNo.Substring(0, 2)) - 8;
                            packageNo = string.Format("64P660275{0}{1}", (yearOfPackage).ToString("00"), packageNo.Substring(packageNo.Length - 8, 8));
                            //if (packageNo == "64P6602751710280035"
                            // || packageNo == "64P6602751710280048"
                            // || packageNo == "64P6602751710280023"
                            // || packageNo == "64P6602751710280011"
                            // || packageNo == "64P6602751708400003"
                            // || packageNo == "64P6602751708400013"
                            // || packageNo == "64P6602751710280005"
                            // || packageNo == "64P6602751710280007"
                            // || packageNo == "64P6602751710280017"
                            // || packageNo == "64P6602751710280021"
                            // || packageNo == "64P6602751710280057"
                            // || packageNo == "64P6602751710280077"
                            // || packageNo == "64P6602751710280080"
                            // || packageNo == "64P6602751708400013")
                            //{
                            //    int pex = Convert.ToInt32(packageNo.Substring(packageNo.Length - 4, 4)) + 1000;
                            //    packageNo = packageNo.Substring(0, packageNo.Length - 4) + pex.ToString();
                            //}
                        }
                        else
                        {
                            return Content(string.Format("包装号{0}对应功率档非275W。", packageNo));
                        }
                    }
                    #endregion

                    row.PackageNo = packageNo;      //包装号                      

                    if (dt.Data.Tables.Count>1 && dt.Data.Tables[1] != null && dt.Data.Tables[1].Rows.Count > 0)       //总数
                    {
                        //int counts = dt.Data.Tables[0].Rows.Count + dt.Data.Tables[1].Rows.Count;
                        row.Count = (dt.Data.Tables[0].Rows.Count + dt.Data.Tables[1].Rows.Count).ToString();
                    }
                    else
                    {
                        row.Count = dt.Data.Tables[0].Rows.Count.ToString();
                    }
                    row.PowerName = wop == null ? string.Empty : wop.PowerName;
                    row.PowerTolerance = wop == null ? string.Empty : wop.PowerDifference;
                    row.Color = color;

                    if (blIsEnglish)
                    {
                        row.OrderNumber = orderNumber.Substring(4, 8);
                        if (strPackagePrintLabel == "SEPackageList-EN.rdlc")
                        {
                            if (row.Color == "深蓝")
                            {
                                row.Color = "Dark Blue";
                            }
                            if (row.Color == "浅蓝")
                            {
                                row.Color = "Light Blue";
                            }
                            if (row.Color == "正蓝")
                            {
                                row.Color = "Blue";
                            }
                        }
                        else
                        {
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
                            row.PowerName = wop == null ? string.Empty : wop.PowerName.Substring(0, 3);
                        }
                    }
                    else
                    {
                        if (strPackagePrintLabel == "YL-JN-PackageList-2.rdlc")
                        {
                            if (row.Color == "深蓝")
                            {
                                row.Color = "C3";
                            }
                            if (row.Color == "浅蓝")
                            {
                                row.Color = "C2";
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
                    }

                    #region 协鑫永能/晋能电池片类型加颜色
                    if (workOrderAttribute != null)
                    {
                        string attrOfOrderCell = workOrderAttribute.AttributeValue.Trim();
                        if (row.Color == "Dark Blue/深蓝")
                        {
                            row.CellType = attrOfOrderCell + "D";
                        }
                        else if (row.Color == "Light Blue/浅蓝")
                        {
                            row.CellType = attrOfOrderCell + "L";
                        }
                        else
                        {
                            row.CellType = attrOfOrderCell;
                        }
                    }
                    #endregion

                    #region 协鑫永能/晋能柜号流水号
                    row.ChestNo = chestOfPackage;
                    #endregion

                    #region 协鑫晋能/张家港ORDER NO: 103A车间：D001 / 102A车间：D002 / 102B车间：D003
                    if (row.PackageNo.Substring(0, 3) == "27M" || row.PackageNo.Substring(0, 3) == "27P"
                        || row.PackageNo.Substring(0, 3) == "64M" || row.PackageNo.Substring(0, 3) == "64P")
                    {
                        if (workOrder.LocationName == "103A")
                        {
                            row.OrderNumber = "D001";
                        }
                        if (workOrder.LocationName == "102A")
                        {
                            row.OrderNumber = "D002";
                        }
                        if (workOrder.LocationName == "102B")
                        {
                            row.OrderNumber = "D003";
                        }
                    }
                    #endregion

                    row.PowerSubCode = pssubcode;
                    row.MaterialCode = materialCode;
                    IList<string> lstColor = new List<string>();    //托内颜色数组
                    string sColor = "";
                    string sFullColor = "";
                    bool isfind = false;
                    string packageLots = "";
                    string sss = "";
                 
                    #region 德国
                    if (strPackagePrintLabel == "GERMAN.rdlc")
                    {
                        if (dt.Data != null && dt.Data.Tables.Count > 1)
                        {
                            if (dt.Data.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dt.Data.Tables[0].Rows.Count; j++)
                                {
                                    packageLots += dt.Data.Tables[0].Rows[j]["OBJECT_NUMBER"].ToString() + ";";
                                }
                            }
                            if (dt.Data.Tables[1].Rows.Count > 0)
                            {
                                for (int j = 0; j < dt.Data.Tables[1].Rows.Count; j++)
                                {
                                    packageLots += dt.Data.Tables[1].Rows[j]["OBJECT_NUMBER"].ToString() + ";";
                                }
                            }
                        }
                        else 
                        {
                            for (int j = 0; j < dt.Data.Tables[0].Rows.Count; j++)
                            {
                                packageLots += dt.Data.Tables[0].Rows[j]["OBJECT_NUMBER"].ToString() + ";";
                            }
                        }
                        
                        Image img = QRCodeSave(packageLots);
                        byte[] packageLotNosOfByte = ImageToBytes(img);
                        sss = ConvertImageToBase64(img, ImageFormat.Png);
                        row.PackageLotNos = packageLotNosOfByte;
                        //row.PackageLotNos = sss;
                    }
                    #endregion

                    for (int j = 0; j < dt.Data.Tables[0].Rows.Count; j++)
                    {
                        //添加包装明细
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
                        detailRow.PowerSubCode = dt.Data.Tables[0].Rows[j]["PS_SUBCODE"].ToString();
                        ds.PackageDetail.AddPackageDetailRow(detailRow);

                        //将不同的颜色加入颜色数组
                        #region
                        sColor = dt.Data.Tables[0].Rows[j]["COLOR"].ToString();

                        if (blIsEnglish)
                        {
                            if (strPackagePrintLabel == "SEPackageList-EN.rdlc")
                            {
                                if (sColor == "深蓝")
                                {
                                    sColor = "Dark Blue";
                                }
                                if (sColor == "浅蓝")
                                {
                                    sColor = "Light Blue";
                                }
                                if (sColor == "正蓝")
                                {
                                    sColor = "Blue";
                                }
                            }
                            else
                            {
                                //Light blue，Blue，Dark blue（3选1）
                                if (sColor == "深蓝")
                                {
                                    sColor = "Navy Blue";
                                }
                                if (sColor == "浅蓝")
                                {
                                    sColor = "baby Blue";
                                }
                                if (sColor == "正蓝")
                                {
                                    sColor = "Blue";
                                }
                            }
                        }
                        else
                        {
                            if (sColor == "深蓝")
                            {
                                sColor = "Dark Blue/深蓝";
                            }
                            if (sColor == "浅蓝")
                            {
                                sColor = "Light Blue/浅蓝";
                            }
                            if (sColor == "正蓝")
                            {
                                sColor = "Blue/正蓝";
                            }
                        }

                        isfind = false;

                        //判断颜色是否存在
                        foreach (string s in lstColor)
                        {
                            if (s == sColor)
                            {
                                isfind = true;

                                break;
                            }
                        }

                        if (!isfind)
                        {
                            lstColor.Add(sColor);

                            if (sFullColor == "")
                            {
                                sFullColor = sColor;
                            }
                            else
                            {
                                sFullColor += "\\" + sColor;
                            }

                        }
                        #endregion
                    }

                    if (dt.Data.Tables.Count > 1)
                    {
                        for (int j = 0; j < dt.Data.Tables[1].Rows.Count; j++)
                        {
                            //添加包装明细
                            PackageListDataSet.PackageDetailRow detailRow = ds.PackageDetail.NewPackageDetailRow();
                            detailRow.PackageNo = dt.Data.Tables[1].Rows[j]["PACKAGE_NO"].ToString();
                            detailRow.ItemNo = Convert.ToInt16(dt.Data.Tables[1].Rows[j]["ITEM_NO"].ToString());
                            detailRow.LotNo = dt.Data.Tables[1].Rows[j]["OBJECT_NUMBER"].ToString();
                            detailRow.IMP = dt.Data.Tables[1].Rows[j]["COEF_IMAX"].ToString();
                            detailRow.ISC = dt.Data.Tables[1].Rows[j]["COEF_ISC"].ToString();
                            detailRow.PM = dt.Data.Tables[1].Rows[j]["COEF_PMAX"].ToString();
                            detailRow.VMP = dt.Data.Tables[1].Rows[j]["COEF_VMAX"].ToString();
                            detailRow.VOC = dt.Data.Tables[1].Rows[j]["COEF_VOC"].ToString();
                            detailRow.FF = dt.Data.Tables[1].Rows[j]["COEF_FF"].ToString();
                            detailRow.PowerSubCode = dt.Data.Tables[1].Rows[j]["PS_SUBCODE"].ToString();
                            ds.PackageDetail.AddPackageDetailRow(detailRow);

                            //将不同的颜色加入颜色数组
                            #region
                            sColor = dt.Data.Tables[1].Rows[j]["COLOR"].ToString();

                            if (blIsEnglish)
                            {
                                if (strPackagePrintLabel == "SEPackageList-EN.rdlc")
                                {
                                    if (sColor == "深蓝")
                                    {
                                        sColor = "Dark Blue";
                                    }
                                    if (sColor == "浅蓝")
                                    {
                                        sColor = "Light Blue";
                                    }
                                    if (sColor == "正蓝")
                                    {
                                        sColor = "Blue";
                                    }
                                }
                                else
                                {
                                    //Light blue，Blue，Dark blue（3选1）
                                    if (sColor == "深蓝")
                                    {
                                        sColor = "Navy Blue";
                                    }
                                    if (sColor == "浅蓝")
                                    {
                                        sColor = "baby Blue";
                                    }
                                    if (sColor == "正蓝")
                                    {
                                        sColor = "Blue";
                                    }
                                }
                            }
                            else
                            {
                                if (sColor == "深蓝")
                                {
                                    sColor = "Dark Blue/深蓝";
                                }
                                if (sColor == "浅蓝")
                                {
                                    sColor = "Light Blue/浅蓝";
                                }
                                if (sColor == "正蓝")
                                {
                                    sColor = "Blue/正蓝";
                                }
                            }

                            isfind = false;

                            //判断颜色是否存在
                            foreach (string s in lstColor)
                            {
                                if (s == sColor)
                                {
                                    isfind = true;

                                    break;
                                }
                            }

                            if (!isfind)
                            {
                                lstColor.Add(sColor);

                                if (sFullColor == "")
                                {
                                    sFullColor = sColor;
                                }
                                else
                                {
                                    sFullColor += "\\" + sColor;
                                }

                            }

                            #endregion
                        }
                    }
                    
                    if (strPackagePrintLabel == "PackageList_DongFangEN.rdlc")
                    {
                        row.Color = sFullColor;
                    }

                    ds.Package.AddPackageRow(row);

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
                        //ReportParameter QRCode = new ReportParameter("QRCode", sss);
                        //localReport.SetParameters(QRCode);
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

                        #region 英利2期项目打印箱号标签及档位号标签
                        if (strPackagePrintLabel == "YL-JN-PackageList-2.rdlc")
                        {
                            string dateOfNow= DateTime.Now.ToString("yyyy-MM-dd");
                            #region 打印箱号标签及档位号标签
                            result = Print(row.PackageNo, row.PowerSubCode, row.PowerName.Substring(0, 3), row.Color,dateOfNow);
                            if (result.Code != 0)
                            {
                                //result.Message = "打印错误。";
                                return Json(result);
                            }
                            #endregion
                        }
                        #endregion

                        return File(renderedBytes, mimeType);
                    }
                }
                #endregion
            }

            catch (Exception err)
            {
                result.Code = 1005;
                result.Message = err.Message;
            }
            return Json(result);
        }

        public MethodReturnResult Print(string packageNo, string powerSubCode, string powerName,string color, string dateOfNow)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                result = PrintPrivate(packageNo, powerSubCode, powerName,color, dateOfNow);
                if (result.Code == 0)
                {
                    result.Message = string.Format("打印箱号{0}标签及档位{1}标签成功。",packageNo,color);
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return result;
        }

        private MethodReturnResult PrintPrivate(string packageNo, string powerSubCode, string powerName, string color, string dateOfNow)
        {
            MethodReturnResult result = new MethodReturnResult();
            result.Code = 0;

            try
            {
                IList<ClientConfigAttribute> lst = new List<ClientConfigAttribute>();
                string hostName = HttpContext.Request.UserHostName;
                string attributeName = "PrinterName";

                using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.ClientName='{0}' AND Key.AttributeName LIKE '{1}%'"
                                              , hostName
                                              , attributeName),
                        OrderBy = "Key.AttributeName"
                    };
                    MethodReturnResult<IList<ClientConfigAttribute>> resultOfPrint = client.Get(ref cfg);
                    if (resultOfPrint.Code <= 0 && resultOfPrint.Data != null && resultOfPrint.Data.Count > 0)
                    {
                        lst = resultOfPrint.Data;
                    }
                    else
                    {
                        result.Code = 1000;
                        result.Message = string.Format("当前电脑{0}未挂载网络打印机。"
                                                , hostName);
                        return result;
                    }
                }

                //获取当前电脑IP所挂载的网络打印机名称
                ClientConfig printer = null;
                using (ClientConfigServiceClient client = new ClientConfigServiceClient())
                {
                    MethodReturnResult<ClientConfig> rst = client.Get(lst[0].Value);
                    if (rst.Code > 0)
                    {
                        return rst;
                    }
                    printer = rst.Data;
                }

                //获取打印条码内容
                PrintLabel labelOfPackage = null;
                PrintLabel labelOfPowerNumber = null;
                using (PrintLabelServiceClient client = new PrintLabelServiceClient())
                {
                    MethodReturnResult<PrintLabel> rst = client.Get(YLPrintPackageLab);
                    if (rst.Code > 0)
                    {
                        return rst;
                    }
                    labelOfPackage = rst.Data;
                }

                using (PrintLabelServiceClient client = new PrintLabelServiceClient())
                {
                    MethodReturnResult<PrintLabel> rst = client.Get(YLPrintPowerNumberLab);
                    if (rst.Code > 0)
                    {
                        return rst;
                    }
                    labelOfPowerNumber = rst.Data;
                }

                DateTime PrintStart = DateTime.Now;

                #region 箱号标签CS网络打印
                //根据打印数量设置打印机模板。
                using (IPrintHelper helper = PrintHelperFactory.CreatePrintHelper(labelOfPackage.Content))
                {
                    PrintStart = DateTime.Now;          //打印开始时间

                    //打印动态内容。
                    dynamic printData = new ExpandoObject();
                    printData.PackageNo = packageNo;
                    printData.PowerSubCode = powerSubCode;
                    printData.DateOfNow = dateOfNow;
                    printData.PrintQty = 1;
                    bool bSuccess = false;

                    //根据打印机类型，调用不同的打印方法。
                    if (printer.ClientType == EnumClientType.NetworkPrinter)        //网络打印机
                    {
                        string[] vals = printer.IPAddress.Split(':');
                        string port = "9100";

                        if (vals.Length > 1)
                        {
                            port = vals[1];
                        }

                        bSuccess = helper.NetworkPrint(vals[0], port, labelOfPackage.Content, printData);                       
                    }
                    else if (printer.ClientType == EnumClientType.RawPrinter)       //本地打印机
                    {
                        bSuccess = helper.RAWPrint(printer.IPAddress, labelOfPackage.Content, printData);
                    }
                    else
                    {
                        result.Code = 1001;
                        result.Message = "打印失败,打印机类型不正确。";
                        return result;
                    }

                    //返回打印结果。
                    if (bSuccess == false)
                    {
                        result.Code = 1001;
                        result.Message = "箱号标签打印失败。";
                        return result;
                    }

                    //打印日志
                    using (PrintLogServiceClient client = new PrintLogServiceClient())
                    {
                        PrintLog obj = new PrintLog()
                        {
                            LotNumber = packageNo,                              //批次号
                            ClientName = Request.UserHostAddress,               //客户端
                            PrintQty = 1,                                       //打印数量 
                            PrintLabelCode = YLPrintPackageLab,                 //打印标签代码
                            PrinterName = lst[0].Value,                         //打印机名称
                            PrintType = printer.ClientType.GetDisplayName(),    //打印机类型
                            IsSucceed = true,                                   //打印是否成功
                            PrintData = packageNo,                              //打印数据
                            Creator = User.Identity.Name,                       //创建人
                            CreateTime = PrintStart,                            //创建日期                               
                            FinishTime = DateTime.Now                           //编辑日期     
                        };

                        result = client.Add(obj);

                        if (result.Code > 0)
                        {
                            return result;
                        }
                    }
                }
                #endregion

                #region 档位标签CS网络打印
                //根据打印数量设置打印机模板。
                using (IPrintHelper helper = PrintHelperFactory.CreatePrintHelper(labelOfPowerNumber.Content))
                {
                    PrintStart = DateTime.Now;          //打印开始时间

                    //打印动态内容。
                    dynamic printData = new ExpandoObject();
                    printData.Color = color;
                    printData.PowerName = powerName;
                    printData.PrintQty = 1;
                    bool bSuccess = false;

                    //根据打印机类型，调用不同的打印方法。
                    if (printer.ClientType == EnumClientType.NetworkPrinter)        //网络打印机
                    {
                        string[] vals = printer.IPAddress.Split(':');
                        string port = "9100";

                        if (vals.Length > 1)
                        {
                            port = vals[1];
                        }

                        bSuccess = helper.NetworkPrint(vals[0], port, labelOfPowerNumber.Content, printData);
                    }
                    else if (printer.ClientType == EnumClientType.RawPrinter)       //本地打印机
                    {
                        bSuccess = helper.RAWPrint(printer.IPAddress, labelOfPowerNumber.Content, printData);
                    }
                    else
                    {
                        result.Code = 1001;
                        result.Message = "打印失败,打印机类型不正确。";
                        return result;
                    }

                    //返回打印结果。
                    if (bSuccess == false)
                    {
                        result.Code = 1001;
                        result.Message = "档位标签打印失败。";
                        return result;
                    }

                    //打印日志
                    using (PrintLogServiceClient client = new PrintLogServiceClient())
                    {
                        PrintLog obj = new PrintLog()
                        {
                            LotNumber = packageNo,                              //批次号
                            ClientName = Request.UserHostAddress,               //客户端
                            PrintQty = 1,                                       //打印数量 
                            PrintLabelCode = YLPrintPowerNumberLab,             //打印标签代码
                            PrinterName = lst[0].Value,                         //打印机名称
                            PrintType = printer.ClientType.GetDisplayName(),    //打印机类型
                            IsSucceed = true,                                   //打印是否成功
                            PrintData = powerName+color,                        //打印数据
                            Creator = User.Identity.Name,                       //创建人
                            CreateTime = PrintStart,                            //创建日期                               
                            FinishTime = DateTime.Now                           //编辑日期     
                        };

                        result = client.Add(obj);

                        if (result.Code > 0)
                        {
                            return result;
                        }
                    }
                }
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

        public bool NetworkPrintBySelf(string name, string port, string template, ExpandoObject obj)
        {
            string content = this.GetContent(template, obj);
            int result = 1;
            if (obj != null)
            {
                KeyValuePair<string, object> pair = obj.FirstOrDefault<KeyValuePair<string, object>>(delegate(KeyValuePair<string, object> item)
                {
                    return item.Key == "PrintQty";
                });
                if (pair.Value != null)
                {
                    int.TryParse(Convert.ToString(pair.Value), out result);
                }
            }
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(name), int.Parse(port));
                int num2 = 0;
                do
                {
                    try
                    {
                        socket.Connect(remoteEP);
                    }
                    catch (Exception exception)
                    {
                        num2++;
                        if (!((num2 <= 2) || socket.Connected))
                        {
                            throw exception;
                        }
                    }
                }
                while (!socket.Connected);
                if (socket.Connected)
                {
                    byte[] bytes = Encoding.Default.GetBytes(content);
                    while (result > 0)
                    {
                        socket.Send(bytes);
                        result--;
                    }
                    socket.Close();
                    return true;
                }
                return false;
            }
        }

        public string GetContent(string template, ExpandoObject obj)
        {
            string input = template;
            if (obj != null)
            {
                for (Match match = Regex.Match(input, "({#([a-zA-Z0-9_]+)})"); match.Success; match = match.NextMatch())
                {
                    string name = match.Result("$2");
                    string oldValue = match.Result("$1");
                    KeyValuePair<string, object> pair = obj.FirstOrDefault<KeyValuePair<string, object>>(delegate(KeyValuePair<string, object> item)
                    {
                        return item.Key.ToUpper() == name.ToUpper();
                    });
                    if (pair.Value != null)
                    {
                        input = input.Replace(oldValue, Convert.ToString(pair.Value));
                    }
                }
            }
            return input;
        }

        public Bitmap QRCodeBitmapForStringAnother(string nr)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode = qrEncoder.Encode(nr);
            //保存成png文件
            //string filename = @"E:\HelloWorld.png";
            //ModuleSize 设置图片大小  
            //QuietZoneModules 设置周边padding
            /*
                * 5----150*150    padding:5
                * 10----300*300   padding:10
                */
            GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(4, QuietZoneModules.Four), Brushes.Black, Brushes.White);

            Point padding = new Point(4, 4);
            DrawingSize dSize = render.SizeCalculator.GetSize(qrCode.Matrix.Width);
            Bitmap map = new Bitmap(dSize.CodeWidth + padding.X, dSize.CodeWidth + padding.Y);
            Graphics g = Graphics.FromImage(map);
            render.Draw(g, qrCode.Matrix, padding);
            return map;
        }

        /// <summary>
        /// 封装方法返回Bitmap
        /// </summary>
        /// <param name="nr"></param>
        /// <returns></returns>
        public Bitmap QRCodeBimapForString(string nr)
        {
            string enCodeString = nr;

            //QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            //QrCode qrCode = qrEncoder.Encode(enCodeString);
            //GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(4, QuietZoneModules.Four), Brushes.Black, Brushes.White);

            //Point padding = new Point(4, 4);
            //DrawingSize dSize = render.SizeCalculator.GetSize(qrCode.Matrix.Width);
            //Bitmap map = new Bitmap(dSize.CodeWidth + padding.X, dSize.CodeWidth + padding.Y);
            //Graphics g = Graphics.FromImage(map);
            //render.Draw(g, qrCode.Matrix, padding);

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            
            //编码方式(注意：BYTE能支持中文，ALPHA_NUMERIC扫描出来的都是数字)
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;//大小(值越大生成的二维码图片像素越高)
            //版本(注意：设置为0主要是防止编码的字符串太长时发生错误)
            qrCodeEncoder.QRCodeVersion = 0;
            //错误效验、错误更正(有4个等级)
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            return qrCodeEncoder.Encode(enCodeString, Encoding.GetEncoding("GB2312"));
        }

        /// <summary>
        /// 保存到本地
        /// </summary>
        /// <param name="nr"></param>
        /// <returns></returns>
        public Image QRCodeSave(string nr)
        {
            //Bitmap bt = QRCodeBimapForString(nr);
            Bitmap bt = QRCodeBitmapForStringAnother(nr);
            Image img = bt;
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Image\\";
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string path = Path.Combine(filePath, fileName);
            bt.Save(path);
            //如果要显示图片就要有返回值
            return img;
        }

        /// <summary>
        /// image对象转byte数组
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image img)
        {
            ImageConverter imgconv = new ImageConverter();
            byte[] b = (byte[])imgconv.ConvertTo(img, typeof(byte[]));
            return b;
        }

        /// <summary>
        /// 转换base64
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private static string ConvertImageToBase64(Image image, ImageFormat format)
        {
            byte[] imageArray;

            using (System.IO.MemoryStream imageStream = new System.IO.MemoryStream())
            {
                image.Save(imageStream, format);
                imageArray = new byte[imageStream.Length];
                imageStream.Seek(0, System.IO.SeekOrigin.Begin);
                imageStream.Read(imageArray, 0,Convert.ToInt32(imageStream.Length));
            }

            return Convert.ToBase64String(imageArray);
        }
    }
}