using ServiceCenter.Client.Mvc.Areas.ZPVC.Models;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.ZPVC;
using ServiceCenter.MES.Service.Contract.ZPVC;
using ZPVCResources = ServiceCenter.Client.Mvc.Resources.ZPVC;
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

namespace ServiceCenter.Client.Mvc.Areas.ZPVC.Controllers
{
    public class BoxPrintController : Controller
    {
        //
        // GET: /ZPVC/BoxPrint/
        public ActionResult Index()
        {
            return View(new BoxPrintViewModel());
        }
        //
        // POST: /ZPVC/BoxPrint/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(BoxPrintViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                model.BoxNo1 = model.BoxNo1!=null?model.BoxNo1.Trim().ToUpper():model.BoxNo1;
                model.BoxNo = model.BoxNo != null ? model.BoxNo.Trim().ToUpper() : model.BoxNo;

                //获取包装数据。
                result = GetBox(model);
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
                IList<Package> lst = (result as MethodReturnResult<IList<Package>>).Data;
                //打印动态内容。
                dynamic d = new ExpandoObject();
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
                        foreach (Package obj in lst)
                        {
                            if (SetPrintObject(obj, ref d) == false)
                            {
                                continue;
                            }
                            bSuccess = helper.NetworkPrint(vals[0], port, label.Content, d);
                        }
                    }
                    else if (printer.ClientType == EnumClientType.RawPrinter)
                    {
                        foreach (Package obj in lst)
                        {
                            if (SetPrintObject(obj, ref d) == false)
                            {
                                continue;
                            }
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
                    result.Message = string.Format("电池箱 {0}-{1} 标签打印失败。", model.BoxNo, model.BoxNo1);
                }
                else
                {
                    result.Message = string.Format("电池箱 {0}-{1} 标签打印操作成功。", model.BoxNo,model.BoxNo1);
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

        private bool SetPrintObject(Package obj, ref dynamic d)
        {
            //获取箱第一包的数据
            string packageNo = string.Empty;
            PackageInfo packageInfo = new PackageInfo();
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.PackageNo='{0}' AND Key.ObjectType='{1}'"
                                        , obj.Key
                                        , Convert.ToInt32(EnumPackageObjectType.Packet)),
                    OrderBy = "ItemNo"
                };
                MethodReturnResult<IList<PackageDetail>> rst = client.GetDetail(ref cfg);
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Count > 0)
                {
                    packageNo = rst.Data[0].Key.ObjectNumber;
                }
            }
            if (string.IsNullOrEmpty(packageNo))
            {
                return false;
            }

            using (PackageInfoServiceClient client = new PackageInfoServiceClient())
            {
                MethodReturnResult<PackageInfo> rst = client.Get(packageNo);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    packageInfo = rst.Data;
                }
                else
                {
                    return false;
                }
            }

            d.CartonNo = obj.Key.ToUpper();
            d.Color = packageInfo.Color;
            d.Date = obj.CreateTime.Value.ToString("yyyy.MM.dd");
            d.Eff = packageInfo.EfficiencyName;
            d.Grade = packageInfo.Grade;
            d.PartNo = packageInfo.ConfigCode;
            d.PNType = packageInfo.PNType;
            d.ProdID = packageInfo.ProductId;
            d.Qty = string.Format("{0}PCS", obj.Quantity);

            return true;
        }

        public MethodReturnResult GetBox(BoxPrintViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<IList<Package>> rst = null;
            IList<Package> lst = null;
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "Key"
                };


                cfg.Where = string.Format("PackageType='{0}'", Convert.ToInt32(EnumPackageType.Box));

                if (!string.IsNullOrEmpty(model.BoxNo1) && !string.IsNullOrEmpty(model.BoxNo))
                {
                    cfg.Where += string.Format(" AND Key>='{0}' AND Key<='{1}'", model.BoxNo, model.BoxNo1);
                }
                else
                {
                    cfg.Where += string.Format(" AND Key='{0}'", model.BoxNo);
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

            if (lst == null || lst.Count == 0)
            {
                result.Code = 2001;
                string message = string.Format("{0}-{1}", model.BoxNo, model.BoxNo1);
                result.Message = string.Format("电池箱数据 {0} 不存在。", message);
                return result;
            }
            return rst;
        }
	}
}