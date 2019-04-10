using ServiceCenter.Client.Mvc.Areas.ZPVC.Models;
using ServiceCenter.Common.Print;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Client.ZPVC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.ZPVC.Controllers
{
    public class PackageController : Controller
    {
        //
        // GET: /ZPVC/Package/
        public ActionResult Index()
        {
            return View("Index", new PackageViewModel());
        }
        //
        //POST: /ZPVC/Package/Query
        [HttpPost]
        public ActionResult Query(string lineCode)
        {
            using (PackageInfoServiceClient client = new PackageInfoServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "Key Desc",
                    Where = string.Format(@"LineCode='{0}' 
                                            AND EXISTS(FROM Package as p
                                                       WHERE p.Key=self.Key
                                                       AND p.PackageState=0)"
                                            , lineCode)
                };
                MethodReturnResult<IList<PackageInfo>> result = client.Get(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", new PackageViewModel());
            }
            else
            {
                return View("Index", new PackageViewModel());
            }
        }
        //
        //POST: /ZPVC/Package/PagingQuery
        [HttpPost]
        public ActionResult PagingQuery(string where
                                        , string orderBy
                                        , int? currentPageNo
                                        , int? currentPageSize)
        {
            int pageNo = currentPageNo ?? 0;
            int pageSize = currentPageSize ?? 20;
            if (Request["PageNo"] != null)
            {
                pageNo = Convert.ToInt32(Request["PageNo"]);
            }
            if (Request["PageSize"] != null)
            {
                pageSize = Convert.ToInt32(Request["PageSize"]);
            }

            using (PackageInfoServiceClient client = new PackageInfoServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = pageNo,
                    PageSize = pageSize,
                    Where = where ?? string.Empty,
                    OrderBy = orderBy ?? string.Empty
                };
                MethodReturnResult<IList<PackageInfo>> result = client.Get(ref cfg);
                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }
            return PartialView("_ListPartial", new PackageViewModel());
        }

        //
        // POST: /ZPVC/Package/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(PackageViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                if (!string.IsNullOrEmpty(model.PackageNo))
                {
                    model.PackageNo = model.PackageNo.Trim().ToUpper();
                }

                Package p = new Package()
                {
                    Key = model.PackageNo,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name,
                    IsLastPackage = false,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    MaterialCode = model.MaterialCode,
                    OrderNumber = model.OrderNumber,
                    PackageState = EnumPackageState.Packaging,
                    PackageType = EnumPackageType.Packet,
                    Quantity = model.Qty.Value
                };

                PackageInfo obj = new PackageInfo()
                {
                    Key = model.PackageNo,
                    Color = model.Color,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    ConfigGroup = model.Group,
                    ConfigCode = model.Code.Trim(),
                    Grade = model.Grade,
                    LineCode = model.LineCode,
                    EfficiencyLower = model.Lower,
                    EfficiencyUpper = model.Upper,
                    EfficiencyName = model.Name,
                    PNType=model.PNType
                };
                //根据物料号获取产品编号。
                Material m = null;
                using (MaterialServiceClient client = new MaterialServiceClient())
                {
                    MethodReturnResult<Material> rst = client.Get(model.MaterialCode);
                    if (rst.Code == 0)
                    {
                        m = rst.Data;
                    }
                }

                if (m != null)
                {
                    obj.ProductId = string.Format("{0}{1}{2}",m.Spec,model.Style,model.Technology);
                }

                //新增包装数据。
                using (PackageInfoServiceClient client = new PackageInfoServiceClient())
                {
                    result = client.Add(p, obj);
                    if (result.Code <= 0)
                    {
                        result.Message = string.Format("保存 {0} 成功。" , p.Key);
                    }
                    else
                    {
                        return Json(result);
                    }
                }
                MethodReturnResult result1 = PrintPrivate(model,obj);
                result.Message += result1.Message;
                result.Code = result1.Code;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Detail = ex.ToString();
                result.Message = ex.Message;
            }
            return Json(result);
        }

        private MethodReturnResult PrintPrivate(PackageViewModel model,PackageInfo obj)
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
            //根据打印数量设置打印机模板。
            using (IPrintHelper helper = PrintHelperFactory.CreatePrintHelper(label.Content))
            {
                //打印动态内容。
                dynamic d = new ExpandoObject();
                d.PackageNo = model.PackageNo.ToUpper();
                d.ProdId = obj.ProductId;
                d.PartNo = obj.ConfigCode;
                d.Grade = obj.Grade;
                d.Color = obj.Color;
                d.PNType = obj.PNType;
                d.Eff = obj.EfficiencyName;
                d.Qty = string.Format("{0}PCS",model.Qty);
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
                    result.Message = "包装标签打印失败。";
                    return result;
                }
            }
            return result;
        }

        //
        // POST: /FMM/Location/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (PackageInfoServiceClient client = new PackageInfoServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format("删除包（{0}) 数据成功。"
                                                    , key);
                }
                return Json(result);
            }
        }

        public ActionResult GetCode(string q,string group,string lineCode,bool isChange)
        {
            string packageNo = string.Empty;
            //生成包装号。
            if (isChange)
            {
                string line = string.Empty;
                using (ProductionLineServiceClient client = new ProductionLineServiceClient())
                {
                    MethodReturnResult<ProductionLine> result = client.Get(lineCode);
                    if (result.Code <= 0)
                    {
                        line = result.Data.Attr1;
                    }
                }
                if (!string.IsNullOrEmpty(line))
                {
                    string prefix = string.Format("C{0:yyMMdd}{1}", DateTime.Now, line);
                    int itemNo = 0;
                    using (PackageInfoServiceClient client = new PackageInfoServiceClient())
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo=0,
                            PageSize=1,
                            Where = string.Format("Key LIKE '{0}%'"
                                                    , prefix),
                            OrderBy="Key Desc"
                        };
                        MethodReturnResult<IList<PackageInfo>> result = client.Get(ref cfg);
                        if (result.Code <= 0 && result.Data.Count > 0)
                        {
                            string maxPackageNo = result.Data[0].Key.Replace(prefix, "");
                            int.TryParse(maxPackageNo, out itemNo);
                        }
                        itemNo++;
                    }
                    packageNo = prefix + itemNo.ToString("0000");
                }
            }
            //获取配置信息。
            using (EfficiencyConfigurationServiceClient client = new EfficiencyConfigurationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.Code LIKE '{0}%' AND Key.Group='{1}'"
                                            , q
                                            , group)
                };
                MethodReturnResult<IList<EfficiencyConfiguration>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return Json(from item in result.Data
                                select new
                                {
                                    @label = item.Key.Code,
                                    @value = item.Key.Code,
                                    @Name=item.Name,
                                    @Lower=item.Lower,
                                    @Upper=item.Upper,
                                    @Grade=item.Grade,
                                    @Color=item.Color,
                                    @PackageNo=packageNo,
                                    @MaterialCode=item.MaterialCode??string.Empty
                                }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetProductNumber(string orderNumber)
        {
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = client.Get(orderNumber);
                if (result.Code <= 0)
                {
                    return Json(result.Data.MaterialCode, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
	}
}