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
    public class PackagePrintController : Controller
    {
        //
        // GET: /WIP/PackagePrint/
        public ActionResult Index()
        {
            return View(new PackagePrintViewModel());
        }
        //
        // POST: /WIP/PackagePrint/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(PackagePrintViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //获取包装数据。
                string packageNo = model.PackageNo.ToUpper();
                result = GetPackage(packageNo);
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
                //打印动态内容。
                dynamic d = new ExpandoObject();
                d.PackageNo = packageNo;
                d.PrintQty = model.PrintQty;
                bool bSuccess = false;
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
                        result.Code = 1001;
                        result.Message = "打印失败,打印机类型不正确。";
                        return Json(result);
                    }
                }
                //返回打印结果。
                if (bSuccess == false)
                {
                    result.Code = 1001;
                    result.Message = "包装 {0} 标签打印失败。";
                }
                else
                {
                    result.Message = string.Format("包装 {0} 标签打印操作成功。", model.PackageNo);
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

        public MethodReturnResult GetPackage(string packageNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<Package> rst = null;
            Package obj = null;
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                rst = client.Get(packageNo);
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
            if (obj == null || obj.Quantity<=0)
            {
                result.Code = 2001;
                result.Message = string.Format("包装 {0} 不存在。", packageNo);
                return result;
            }
            return rst;
        }
	}
}