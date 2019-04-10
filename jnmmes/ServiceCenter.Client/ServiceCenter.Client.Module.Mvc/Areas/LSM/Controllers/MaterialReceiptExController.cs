using ServiceCenter.Client.Mvc.Areas.LSM.Models;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LSMResources = ServiceCenter.Client.Mvc.Resources.LSM;

namespace ServiceCenter.Client.Mvc.Areas.LSM.Controllers
{
    public class MaterialReceiptExController : Controller
    {
        //
        // GET: /LSM/MaterialReceiptEx/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Query(MaterialReceiptExQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
                {
                    StringBuilder where = new StringBuilder();
                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.ReceiptNo))
                        {
                            where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.ReceiptNo);
                        }
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "CreateTime desc",
                        Where = where.ToString()
                    };

                    MethodReturnResult<IList<MaterialReceipt>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                }
            }
            return PartialView("_ListPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(MaterialReceiptExViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
                {
                    MethodReturnResult<MaterialReceipt> resultOfAddMaterial = new MethodReturnResult<MaterialReceipt>();

                    MaterialReceipt materialReceipt = new MaterialReceipt()
                    {
                        Key = model.ReceiptNo,
                        ReceiptDate = model.ReceiptDate,
                        Description = model.Description,
                        OrderNumber = model.OrderNumber,
                        LineStore = model.LineStore,
                        Editor = User.Identity.Name,
                        Creator = User.Identity.Name,
                    };
                    
                    resultOfAddMaterial = client.AddMaterialReceipt(materialReceipt);
                    
                    if (resultOfAddMaterial.Code == 0)
                    {
                        result.Message = string.Format(LSMResources.StringResource.MaterialReceipt_Save_Success
                                                    , resultOfAddMaterial.Data.Key);
                        result.Detail = resultOfAddMaterial.Data.Key;
                    }
                    else
                    {
                        result.Code = resultOfAddMaterial.Code;
                        result.Message = resultOfAddMaterial.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return Json(result);
        }

        public ActionResult Modify(string key)
        {
            MaterialReceiptExViewModel model = new MaterialReceiptExViewModel();
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                MethodReturnResult<MaterialReceipt> result = client.Get(key);
                if (result.Code == 0)
                {
                    model = new MaterialReceiptExViewModel()
                    {
                        ReceiptNo = result.Data.Key,
                        ReceiptDate = result.Data.ReceiptDate,
                        OrderNumber = result.Data.OrderNumber,
                        LineStore = result.Data.LineStore,
                        Creator = result.Data.Creator,
                        Editor = User.Identity.Name                       
                    };
                    return PartialView("_ModifyPartial", model);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            return PartialView("_ModifyPartial");
        }

        [HttpPost]
        public ActionResult SaveModify(MaterialReceiptExViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                MaterialReceipt MaterialReceipt = new MaterialReceipt()
                {
                    Key = model.ReceiptNo,
                    ReceiptDate = model.ReceiptDate,
                    OrderNumber = model.OrderNumber,
                    Description = model.Description,
                    LineStore = model.LineStore,
                    Creator = model.Creator,
                    Editor = User.Identity.Name
                };

                result = client.ModifyMaterialReceipt(MaterialReceipt);

                if (result.Code == 0)
                {
                    result.Message = string.Format(LSMResources.StringResource.MaterialReceipt_Save_Success, model.ReceiptNo);
                }

            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                result = client.DeleteMaterialReceipt(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(LSMResources.StringResource.MaterialReceipt_Save_Success, key);
                }
            }

            return Json(result);
        }

        public ActionResult GetOrderNumbers()
        {
            IList<WorkOrder> lstWorkOrder = new List<WorkOrder>();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"CloseType='{0}' "
                                           , Convert.ToInt32(EnumCloseType.None))
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lstWorkOrder = result.Data;
                }
            }
            return Json(from item in lstWorkOrder
                        select new
                        {
                            Text = item.Key,
                            Value = item.Key
                        }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMaterialCodes(string orderNumber)
        {
            //根据物料类型获取物料。
            //IList<WorkOrder> lst = new List<WorkOrder>();
            List<string> lstMaterial = new List<string>();

            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = client.Get(orderNumber);
                if (result.Code <= 0 && result.Data != null)
                {
                    string materialCode = result.Data.MaterialCode;
                    lstMaterial.Add(materialCode);
                }
            }
            var lnq = from item in lstMaterial
                      select item;

            return Json(lnq.Distinct(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
        {
            if (ModelState.IsValid)
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

                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = pageNo,
                            PageSize = pageSize,
                            Where = where ?? string.Empty,
                            OrderBy = orderBy ?? string.Empty
                        };
                        MethodReturnResult<IList<MaterialReceipt>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial");
        }
        public ActionResult GetReceiptNo()
        {
            string prefix = string.Format("LMK{0:yyMMdd}", DateTime.Now);
            int itemNo = 0;
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key LIKE '{0}%'", prefix),
                    OrderBy = "Key Desc"
                };
                MethodReturnResult<IList<MaterialReceipt>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    string sItemNo = result.Data[0].Key.Replace(prefix, "");
                    int.TryParse(sItemNo, out itemNo);
                }
            }
            return Json(prefix + (itemNo + 1).ToString("0000"), JsonRequestBehavior.AllowGet);
        }

	}
}