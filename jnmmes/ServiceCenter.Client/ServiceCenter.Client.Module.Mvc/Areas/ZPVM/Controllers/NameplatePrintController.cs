using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Common.Print;
using System.Dynamic;
using System.Text;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.Common;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class NameplatePrintController : Controller
    {
        //
        // GET: /ZPVM/NameplatePrint/
        public ActionResult Index()
        {
            return View(new NameplatePrintViewModel());
        }
        //
        // POST: /ZPVM/NameplatePrint/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(NameplatePrintViewModel model)
        {
            System.DateTime dateStartTime ;
            System.DateTime dateEndTime;
            TimeSpan tSpan;
            string lotNumber = "";
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                dateStartTime = System.DateTime.Now;
                dateEndTime = System.DateTime.Now;                
                //获取批值。
                lotNumber = model.LotNumber.ToUpper();
                result = GetLot(lotNumber);
                if (result.Code > 0)
                {
                    return Json(result);
                }
                Lot obj = (result as MethodReturnResult<Lot>).Data;
                //获取打印机名称
                ClientConfig printer = null;
                if (!string.IsNullOrEmpty(model.PrinterName))
                {
                    using (ClientConfigServiceClient client = new ClientConfigServiceClient())
                    {
                        MethodReturnResult<ClientConfig> rst = client.Get(model.PrinterName);
                        if (rst.Code > 0)
                        {
                            return Json(rst);
                        }
                        printer = rst.Data;
                    }
                }
                //获取打印条码内容
                PrintLabel label = null;
                if (!string.IsNullOrEmpty(model.PrintLabelCode))
                {
                    using (PrintLabelServiceClient client = new PrintLabelServiceClient())
                    {
                        MethodReturnResult<PrintLabel> rst = client.Get(model.PrintLabelCode);
                        if (rst.Code > 0)
                        {
                            return Json(rst);
                        }
                        label = rst.Data;
                    }
                }
                //获取IV测试数据及其对应的分档数据。
                IVTestData ivtest = null;
                using (IVTestDataServiceClient client = new IVTestDataServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.LotNumber='{0}' AND IsDefault=1"
                                               , model.LotNumber),
                        OrderBy="Key.TestTime Desc"
                    };
                    MethodReturnResult<IList<IVTestData>> rst = client.Get(ref cfg);
                    if (rst.Code > 0)
                    {
                        return Json(rst);
                    }
                    if (rst.Data.Count <= 0)
                    {
                        LogHelper.WriteLogInfo(string.Format("GetIVDataError>{0}IV测试数据不存在。", model.LotNumber));
                        rst.Message = string.Format("{0} IV测试数据不存在。", model.LotNumber);
                        rst.Code = 1000;
                        return Json(rst);
                    }
                    ivtest = rst.Data[0];
                }
                //获取工单分档规则
                WorkOrderPowerset wop = null;
                using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
                {
                    MethodReturnResult<WorkOrderPowerset> rst = client.Get(new WorkOrderPowersetKey()
                    {
                        Code = ivtest.PowersetCode,
                        ItemNo = ivtest.PowersetItemNo ?? -1,
                        OrderNumber = obj.OrderNumber,
                        MaterialCode = obj.MaterialCode
                    });
                    if (rst.Code > 0)
                    {
                        return Json(rst);
                    }
                    wop = rst.Data;
                }
               
                //根据物料编码获取物料数据，进一步获取产品类型。
                Material material = null;
                using (MaterialServiceClient client = new MaterialServiceClient())
                {
                    MethodReturnResult<Material> rst = client.Get(obj.MaterialCode);
                    if (rst.Code > 0)
                    {
                        return Json(rst);
                    }
                    material = rst.Data;
                }

                #region //检查打印铭牌的LOG日志
                using (IVTestDataPrintLogServiceClient client = new IVTestDataPrintLogServiceClient())
                {
                    int itemNo=1;
                    if (model.IsRepeatPrint==false)
                    { 
                        MethodReturnResult<IVTestDataPrintLog> rst = client.Get(new IVTestDataPrintLogKey()
                        {
                            LotNumber = ivtest.Key.LotNumber,
                            EquipmentCode = ivtest.Key.EquipmentCode,
                            TestTime = ivtest.Key.TestTime,
                            ItemNo = itemNo
                        });
                        if (rst.Code == 0 && rst.Data!=null)
                        {
                            //表示重复打印过
                            result = new MethodReturnResult()
                            {
                                Code = 1001,
                                Message = string.Format("批次（{0}）已打印铭牌。", ivtest.Key.LotNumber)
                            };
                            return Json(result);
                        }
                    }
                }
                #endregion //End 检查打印铭牌的LOG日志

                bool bSuccess = true;
                //打印动态内容。
                if (printer != null && label != null)
                {
                    //获取工单打印规则
                    WorkOrderPrintSet woprint = null;
                    using (WorkOrderPrintSetServiceClient client = new WorkOrderPrintSetServiceClient())
                    {
                        MethodReturnResult<WorkOrderPrintSet> rst = client.Get(new WorkOrderPrintSetKey()
                        {
                            OrderNumber = obj.OrderNumber,
                            MaterialCode = obj.MaterialCode,
                            LabelCode = model.PrintLabelCode
                        });
                        if (rst.Code > 0)
                        {
                            return Json(rst);
                        }
                        woprint = rst.Data;
                    }

                    dynamic d = new ExpandoObject();
                    d.PrintQty = woprint.Qty;
                    d.LotNumber = lotNumber;
                    d.PM = ivtest.CoefPM;
                    d.ISC = ivtest.CoefISC;
                    d.VOC = ivtest.CoefVOC;
                    d.IPM = ivtest.CoefIPM;
                    d.VPM = ivtest.CoefVPM;
                    d.FF = ivtest.CoefFF;
                    d.StandardPower = string.Format("{0:0.0}", wop.StandardPower);
                    d.StandardFuse = wop.StandardFuse;
                    d.StandardIsc = wop.StandardIsc;
                    d.StandardVoc = wop.StandardVoc;
                    d.StandardVPM = wop.StandardVPM;
                    d.StandardIPM = wop.StandardIPM;
                    d.PowerName = wop.PowerName;
                    d.PowerDifference = wop.PowerDifference;
                    d.ProductType = string.Format("JNM{1}{2}-{0}"
                                                 , wop == null ? string.Empty : Convert.ToString(wop.StandardPower)
                                                 , material.Key.StartsWith("1201") ? "M" : "P"
                                                 , material.MainRawQtyPerLot);
                    d.ProductSpec = material.Spec.Replace("*", "x");

                    dateEndTime = System.DateTime.Now;
                    tSpan = dateEndTime - dateStartTime;
                    dateStartTime = dateEndTime;

                    LogHelper.WriteLogInfo(string.Format(@"BeforePrintLabel{3}>从{0:yyyy-MM-dd HH:mm:ss} 到 {1:yyyy-MM-dd HH:mm:ss} ,时间间隔{2}",
                                            dateStartTime.ToString(),dateEndTime.ToString(),tSpan.TotalMilliseconds,lotNumber
                                           )
                        );

                    //根据打印机类型，调用不同的打印方法。
                    using (IPrintHelper helper = PrintHelperFactory.CreatePrintHelper(label.Content))
                    {
                        if (printer.ClientType == EnumClientType.NetworkPrinter)
                        {
                            string[] vals = printer.IPAddress.Split(':');
                            string port = "9100";
                            if (vals.Length > 1)
                            {
                                port = vals[1];
                            }
                            bSuccess = helper.NetworkPrint(vals[0], port, label.Content, d);
                        }
                        else if (printer.ClientType == EnumClientType.RawPrinter)
                        {
                            bSuccess = helper.RAWPrint(printer.IPAddress, label.Content, d);
                        }
                        else
                        {
                            bSuccess = false;
                            result.Code = 1001;
                            result.Message = "打印失败,打印机类型不正确。";
                            return Json(result);
                        }
                    }

                    if(bSuccess)
                    {
                        #region //Add Print Log
                        using (IVTestDataPrintLogServiceClient client = new IVTestDataPrintLogServiceClient())
                        {
                            int itemNo=1;
                            IVTestDataPrintLog ivdpl = new IVTestDataPrintLog()
                            {
                                CoefFF=ivtest.CoefFF,
                                CoefIPM=ivtest.CoefIPM,
                                CoefISC=ivtest.CoefISC,
                                CoefPM=ivtest.CoefPM,
                                CoefVOC=ivtest.CoefVOC,
                                CoefVPM=ivtest.CoefVPM,
                                CreateTime=DateTime.Now,
                                Creator=User.Identity.Name,
                                CTM=ivtest.CTM,
                                LabelCode=model.PrintLabelCode,
                                PowersetCode=ivtest.PowersetCode,
                                PowersetItemNo=ivtest.PowersetItemNo.Value,
                                PowersetSubCode=ivtest.PowersetSubCode,
                                PrintTime=DateTime.Now,
                                Key = new IVTestDataPrintLogKey()
                                {
                                    LotNumber=ivtest.Key.LotNumber,
                                    EquipmentCode=ivtest.Key.EquipmentCode,
                                    TestTime=ivtest.Key.TestTime,
                                    ItemNo = itemNo
                                }
                            };
                            client.Add(ivdpl);
                        }
                        #endregion
                    }
                }
                else
                {
                    bSuccess = false;
                }

                //返回打印结果。
                StringBuilder sbMessage = new StringBuilder();
                if (bSuccess == false)
                {
                    result.Code = 1001;
                    sbMessage.AppendFormat("批次 {0} 打印操作失败。<br/>", model.LotNumber);
                }
                else
                {
                    sbMessage.AppendFormat("批次 {0} 打印操作成功。<br/>", model.LotNumber);
                }
                sbMessage.Append("<table border='1px' width='100%'><tr><td>");
                sbMessage.AppendFormat("<font size='14px' color='red'>档位：{0}</font>", wop.PowerName);
                sbMessage.Append("</td><td>");
                sbMessage.AppendFormat("<font size='14px' color='red'>子档位：{0}</font>  ", ivtest.PowersetSubCode);
                sbMessage.AppendFormat("<img src='/ZPVM/WorkOrderPowersetDetail/ShowPicture?OrderNumber={0}&MaterialCode={1}&Code={2}&ItemNo={3}&SubCode={4}&TimeStamp={5}' width='150px'/>"
                                        , obj.OrderNumber
                                        , obj.MaterialCode
                                        , ivtest.PowersetCode
                                        , ivtest.PowersetItemNo
                                        , ivtest.PowersetSubCode
                                        , DateTime.Now.Ticks);
                sbMessage.Append("</td><td>");
                sbMessage.AppendFormat("<font size='14px' color='blue'>花色：{0}</font><br/>", obj.Color);
                sbMessage.Append("</td><td>");
                sbMessage.AppendFormat("<br/><font size='14px' color='blue'>等级：{0}</font><br/>", obj.Grade);
                sbMessage.Append("</td><tr></table>");

                sbMessage.AppendFormat("<font size='10px' color='blue'>功率：{0}</font><br/>", ivtest.CoefPM);
                sbMessage.AppendFormat("<font size='10px' color='blue'>ISC：{0}</font><br/>", ivtest.CoefISC);
                sbMessage.AppendFormat("<font size='10px' color='blue'>IPM：{0}</font><br/>", ivtest.CoefIPM);
                sbMessage.AppendFormat("<font size='10px' color='blue'>VOC：{0}</font><br/>", ivtest.CoefVOC);
                sbMessage.AppendFormat("<font size='10px' color='blue'>VPM：{0}</font><br/>", ivtest.CoefVPM);
                result.Message = sbMessage.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError(string.Format(@"PrintLabel{0}失败",lotNumber ),ex);
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return Json(result);
        }

        public MethodReturnResult GetLot(string lotNumber)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<Lot> rst = null;
            Lot obj = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                rst = client.Get(lotNumber);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    obj = rst.Data;
                }
                else
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.Detail = rst.Detail;
                    return result;
                }
            }
            if (obj == null || obj.Status == EnumObjectStatus.Disabled)
            {
                result.Code = 2001;
                result.Message = string.Format(WIPResources.StringResource.LotIsNotExists, lotNumber);
                return result;
            }
            //else if (obj.StateFlag == EnumLotState.Finished)
            //{
            //    result.Code = 2002;
            //    result.Message = string.Format("批次({0})已完成。", lotNumber);
            //    return result;
            //}
            //else if (obj.Status == EnumObjectStatus.Disabled || obj.DeletedFlag == true)
            //{
            //    result.Code = 2003;
            //    result.Message = string.Format("批次({0})已结束。", lotNumber);
            //    return result;
            //}
            return rst;
        }



	}
}