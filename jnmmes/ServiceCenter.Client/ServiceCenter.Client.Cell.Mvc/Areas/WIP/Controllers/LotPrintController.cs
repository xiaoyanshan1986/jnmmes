using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
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

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotPrintController : Controller
    {
        //
        // GET: /WIP/LotPrint/
        public ActionResult Index()
        {
            return View(new LotPrintViewModel());
        }
        //
        // POST: /WIP/LotPrint/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotPrintViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //获取批值。
                result = GetLot(model);
                if (result.Code > 0)
                {
                    return Json(result);
                }
                //获取打印机名称
                ClientConfig printer = null;
                using (ClientConfigServiceClient client = new ClientConfigServiceClient())
                {
                    MethodReturnResult<ClientConfig> rst = client.Get(model.PrinterName);
                    if (rst.Code > 0)
                    {
                        return Json(rst);
                    }
                    printer = rst.Data;
                }
                //获取打印条码内容
                PrintLabel label = null;
                using (PrintLabelServiceClient client = new PrintLabelServiceClient())
                {
                    MethodReturnResult<PrintLabel> rst = client.Get(model.PrintLabelCode);
                    if (rst.Code > 0)
                    {
                        return Json(rst);
                    }
                    label = rst.Data;
                }

                IList<Lot> lst = (result as MethodReturnResult<IList<Lot>>).Data;
                //打印动态内容。
                dynamic d = new ExpandoObject();
                d.PrintQty = model.PrintQty;
                bool bSuccess = false;
                using (IPrintHelper helper = PrintHelperFactory.CreatePrintHelper(label.Content))
                {
                    //根据打印机类型，调用不同的打印方法。
                    if (printer.ClientType == EnumClientType.NetworkPrinter)
                    {
                        string[] vals = printer.IPAddress.Split(':');
                        string port = "9100";
                        if (vals.Length > 1)
                        {
                            port = vals[1];
                        }
                        foreach(Lot obj in lst)
                        {
                            d.LotNumber = obj.Key;
                            bSuccess = helper.NetworkPrint(vals[0], port, label.Content, d);
                        }
                    }
                    else if (printer.ClientType == EnumClientType.RawPrinter)
                    {
                        foreach (Lot obj in lst)
                        {
                            d.LotNumber = obj.Key;
                            bSuccess = helper.RAWPrint(printer.IPAddress, label.Content, d);
                        }
                    }
                    else
                    {
                        result.Code = 1001;
                        result.Message = "打印失败,打印机类型不正确。";
                        return Json(result);
                    }
                }
                //返回打印结果。
                if (bSuccess == false)
                {
                    result.Code = 1001;
                    result.Message = string.Format("批次 {0} - {1} 打印失败。", model.LotNumber, model.LotNumber1);
                }
                else
                {
                    result.Message = string.Format("批次 {0} - {1} 打印操作成功。", model.LotNumber,model.LotNumber1);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return Json(result);
        }

        public MethodReturnResult GetLot(LotPrintViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<IList<Lot>> rst = null;
            IList<Lot> lst = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging=false,
                    OrderBy="Key"
                };
                if (string.IsNullOrEmpty(model.LotNumber1))
                {
                    cfg.Where = string.Format("Key='{0}'", model.LotNumber);
                }
                else
                {
                    cfg.Where = string.Format("Key>='{0}' AND Key<='{1}'", model.LotNumber,model.LotNumber1);
                }
                rst = client.Get(ref cfg);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    lst = rst.Data;
                }
                else
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.Detail = rst.Detail;
                    return result;
                }
            }
            
            if (lst == null || lst.Count==0)
            {
                result.Code = 2001;
                string message = string.Format("{0}-{1}", model.LotNumber, model.LotNumber1);
                result.Message = string.Format(WIPResources.StringResource.LotIsNotExists, message);
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