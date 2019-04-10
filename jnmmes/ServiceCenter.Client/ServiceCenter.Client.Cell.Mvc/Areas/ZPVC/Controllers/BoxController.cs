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
using ServiceCenter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Common.Print;
using System.Dynamic;

namespace ServiceCenter.Client.Mvc.Areas.ZPVC.Controllers
{
    public class BoxController : Controller
    {
        //
        // GET: /ZPVC/Box/
        /// <summary>
        /// 显示装箱作业界面。
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(new BoxViewModel());
        }

        //
        //POST: /ZPVC/Box/Query
        [HttpPost]
        public ActionResult Query(string boxNo)
        {
            if (!string.IsNullOrEmpty(boxNo))
            {
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "ItemNo",
                        Where = string.Format(@"Key.PackageNo='{0}' 
                                                AND EXISTS(FROM Package as p
                                                           WHERE p.Key=self.Key.PackageNo
                                                           AND PackageState='{1}' 
                                                           AND PackageType='{2}')"
                                            , boxNo
                                            ,Convert.ToInt32(EnumPackageState.Packaging)
                                            ,Convert.ToInt32(EnumPackageType.Box))
                    };
                    MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PackageDetailList = result.Data;
                    }
                }
            }
            return PartialView("_ListPartial", new PackageViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(BoxViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(model.PackageNo))
                {
                    model.PackageNo = model.PackageNo.Trim().ToUpper();
                }
                if (!string.IsNullOrEmpty(model.BoxNo))
                {
                    model.BoxNo = model.BoxNo.Trim().ToUpper();
                }
                //获取电池小包数据。
                string packageNo = model.PackageNo.ToUpper();
                result = GetPackage(packageNo);
                if (result.Code > 0)
                {
                    if (result.Code == 1002)
                    {
                        result.Message = string.Format("电池小包 {0} 数据不存在。",packageNo);
                    }
                    return Json(result);
                }
                MethodReturnResult<Package> rst = result as MethodReturnResult<Package>;
                Package obj = rst.Data;

                //如果装箱号为空。生成装箱号。
                if(string.IsNullOrEmpty(model.BoxNo))
                {
                    string prefix = string.Format("JNC{0:yyMMdd}", DateTime.Now);
                    int itemNo = 0;
                    using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key LIKE '{0}%' AND PackageType='{1}'"
                                                    , prefix
                                                    , Convert.ToInt32(EnumPackageType.Box)),
                            OrderBy = "Key Desc"
                        };
                        MethodReturnResult<IList<Package>> rst1 = client.Get(ref cfg);
                        if (rst1.Code <= 0 && rst1.Data.Count > 0)
                        {
                            string maxBoxNo = rst1.Data[0].Key.Replace(prefix, "");
                            int.TryParse(maxBoxNo, out itemNo);
                        }
                        itemNo++;
                    }
                    model.BoxNo = prefix + itemNo.ToString("000");
                }
                model.BoxNo = model.BoxNo.ToUpper();
                //重新获取当前数量。
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    MethodReturnResult<Package> rst2 = client.Get(model.BoxNo);
                    if (rst2.Code == 1000)
                    {
                        return Json(rst2);
                    }
                    //检查装箱状态
                    if (rst2.Data!=null && rst2.Data.PackageState != EnumPackageState.Packaging)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("箱 {0} 非 [{1}] 状态，不能装箱。"
                                                        , model.BoxNo.ToUpper()
                                                        , EnumPackageState.Packaging.GetDisplayName());
                        return Json(result);
                    }
                    //设置当前数量。
                    if (rst2.Code <= 0 && rst2.Data != null)
                    {
                        model.CurrentQuantity = rst2.Data.Quantity;
                    }
                }

                double newCurrentQuantity = model.CurrentQuantity + obj.Quantity;
                //当前数量超过满箱数量，不能继续装箱。
                if (newCurrentQuantity > model.FullQuantity)
                {
                    result.Code = 1;
                    result.Message = string.Format("箱({0}) 当前数量({1})加上小包（{2}）数量（{3}），超过满箱数量。"
                                                    , model.BoxNo.ToUpper()
                                                    , model.CurrentQuantity
                                                    , obj.Key
                                                    , obj.Quantity);
                    return Json(result);
                }
                model.CurrentQuantity = newCurrentQuantity;

                result = Box(model);
                //返回装箱结果。
                if (result.Code <= 0)
                {
                    if (model.CurrentQuantity == model.FullQuantity)
                    {
                        MethodReturnResult result1 = PrintPrivate(model);
                        result.Message += result1.Message;
                        result.Code = result1.Code;
                    }

                    MethodReturnResult<BoxViewModel> rstFinal = new MethodReturnResult<BoxViewModel>()
                    {
                        Code = result.Code,
                        Data = model,
                        Detail = result.Detail,
                        HelpLink = result.HelpLink,
                        Message = result.Message
                    };

                    return Json(rstFinal);
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

        /// <summary>
        /// 装箱作业。
        /// </summary>
        /// <param name="model">装箱模型对象。</param>
        /// <returns>返回结果。</returns>
        private MethodReturnResult Box(BoxViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            //进行装箱作业。
            BoxParameter p = new BoxParameter()
            {
                Creator = User.Identity.Name,
                PackageNo = model.PackageNo.ToUpper(),
                BoxNo=model.BoxNo
            };
            using (BoxServiceClient client = new BoxServiceClient())
            {
                result = client.Box(p);

                if (result.Code == 0)
                {
                    result.Message = string.Format("电池小包 {0} 成功装箱到（{1}）。"
                                                   , model.PackageNo.ToUpper()
                                                   , model.BoxNo);
                }
            }
            return result;
        }


        //
        // POST: /WIP/LotCreate/Print
        [HttpPost]
        public ActionResult Print(BoxViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                result = PrintPrivate(model);
                if (result.Code == 0)
                {
                    result.Message = string.Format("打印箱 {0} 标签成功。",model.BoxNo);
                }
                return Json(result);
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

        private MethodReturnResult PrintPrivate(BoxViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            //不需要进行标签打印。
            if (model.PrintQty <= 0
                || string.IsNullOrEmpty(model.PrinterName)
                || string.IsNullOrEmpty(model.PrintLabelCode))
            {
                return result;
            }
            //获取打印机名称
            ClientConfig printer = null;
            using (ClientConfigServiceClient client = new ClientConfigServiceClient())
            {
                MethodReturnResult<ClientConfig> rst = client.Get(model.PrinterName);
                if (rst.Code > 0)
                {
                    return rst;
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
                    return rst;
                }
                label = rst.Data;
            }
            //获取箱数据
            Package box = new Package();
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                MethodReturnResult<Package> rst = client.Get(model.BoxNo);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    box = rst.Data;
                }
                else
                {
                    result=rst;
                    if (result.Code == 1002)
                    {
                        result.Message = string.Format("箱 {0} 数据不存在。", model.BoxNo);
                    }
                    return result;
                }
            }
            if (box.Quantity <= 0)
            {
                result.Code = 2001;
                result.Message = string.Format("箱 {0} 中数量为零，请确认。", model.BoxNo);
                return result;
            }
            //获取箱第一包的数据
            string packageNo = string.Empty;
            PackageInfo obj = new PackageInfo();
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.PackageNo='{0}' AND Key.ObjectType='{1}'"
                                        , model.BoxNo
                                        , Convert.ToInt32(EnumPackageObjectType.Packet)),
                    OrderBy = "ItemNo"
                };
                MethodReturnResult<IList<PackageDetail>> rst = client.GetDetail(ref cfg);
                if (rst.Code <= 0 && rst.Data!=null && rst.Data.Count>0)
                {
                    packageNo = rst.Data[0].Key.ObjectNumber;
                }
            }
            if (!string.IsNullOrEmpty(packageNo))
            {
                using (PackageInfoServiceClient client = new PackageInfoServiceClient())
                {
                    MethodReturnResult<PackageInfo> rst = client.Get(packageNo);
                    if (rst.Code <= 0 && rst.Data != null)
                    {
                        obj = rst.Data;
                    }
                    else
                    {
                        result = rst;
                        return result;
                    }
                }
            }
            //根据打印数量设置打印机模板。
            using (IPrintHelper helper = PrintHelperFactory.CreatePrintHelper(label.Content))
            {
                //打印动态内容。
                dynamic d = new ExpandoObject();
                d.CartonNo = model.BoxNo.ToUpper();
                d.Color = obj.Color;
                d.Date = box.CreateTime.Value.ToString("yyyy.MM.dd");
                d.Eff = obj.EfficiencyName;
                d.Grade = obj.Grade;
                d.PartNo = obj.ConfigCode;
                d.PNType = obj.PNType;
                d.ProdID = obj.ProductId;
                d.Qty = string.Format("{0}PCS", box.Quantity);
                d.PrintQty = model.PrintQty;
                bool bSuccess = false;
                //根据打印机类型，调用不同的打印方法。
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
                    return result;
                }
                //返回打印结果。
                if (bSuccess == false)
                {
                    result.Code = 1001;
                    result.Message = "箱标签打印失败。";
                    return result;
                }
            }
            return result;
        }


        [HttpPost]
        public ActionResult Delete(string boxNo,string packageNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //拆箱
                UnboxParameter p = new UnboxParameter()
                {
                    Creator=User.Identity.Name,
                    BoxNo=boxNo,
                    PackageNo=packageNo
                };
                using (BoxServiceClient client = new BoxServiceClient())
                {
                    result = client.Unbox(p);
                    if (result.Code == 0)
                    {
                        result.Message = string.Format("电池小包 ({0}) 从箱（{1}) 中移除。"
                                                        ,packageNo,boxNo);
                    }
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

        private MethodReturnResult GetPackage(string packageNo)
        {
            //如果本次请求有成功获取到电池小包对象，直接返回。
            if (ViewBag.Package != null)
            {
                return ViewBag.Package;
            }

            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<Package> rst = null;
            Package obj = null;
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                rst = client.Get(packageNo);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    obj = rst.Data;
                    ViewBag.Package = rst;
                }
                else
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.Detail = rst.Detail;
                    return result;
                }
            }
            if (obj == null)
            {
                result.Code = 2001;
                result.Message = string.Format("电池小包 {0} 不存在。", packageNo);
                return result;
            }
            else if (obj.PackageState == EnumPackageState.Packaged)
            {
                result.Code = 2002;
                result.Message = string.Format("电池小包 {0} 已装箱。", packageNo);
                return result;
            }
            return rst;
        }
        /// <summary>
        /// 获取装箱信息。
        /// </summary>
        /// <param name="boxNo"></param>
        /// <returns></returns>
        public ActionResult GetBoxInfo(string boxNo)
        {
            double currentQuantity = 0;

            if (!string.IsNullOrEmpty(boxNo))
            {
                //获取当前数量
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    MethodReturnResult<Package> rst2 = client.Get(boxNo);
                    if (rst2.Code == 1000)
                    {
                        return Json(rst2);
                    }
                    if (rst2.Code <= 0 && rst2.Data != null && rst2.Data.PackageState==EnumPackageState.Packaging)
                    {
                        currentQuantity = rst2.Data.Quantity;
                    }
                }
            }

            return Json(new{ 
                              CurrentQuantity = currentQuantity
                            }, JsonRequestBehavior.AllowGet);
            
        }

	}
}