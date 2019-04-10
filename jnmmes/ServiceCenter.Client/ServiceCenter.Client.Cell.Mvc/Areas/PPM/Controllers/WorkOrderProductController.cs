using ServiceCenter.Client.Mvc.Areas.PPM.Models;
using PPMResources = ServiceCenter.Client.Mvc.Resources.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Controllers
{
    public class WorkOrderProductController : Controller
    {

        //
        // GET: /PPM/WorkOrderProduct/
        public async Task<ActionResult> Index(string orderNumber)
        {
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = await client.GetAsync(orderNumber ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "WorkOrder");
                }
                ViewBag.WorkOrder = result.Data;
            }

            using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.OrderNumber = '{0}'"
                                                    , orderNumber),
                        OrderBy = "ItemNo"
                    };
                    MethodReturnResult<IList<WorkOrderProduct>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new WorkOrderProductQueryViewModel() { OrderNumber = orderNumber });
        }

        //
        //POST: /PPM/WorkOrderProduct/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderProductQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.OrderNumber = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);

                            if (!string.IsNullOrEmpty(model.MaterialCode))
                            {
                                where.AppendFormat(" {0} Key.MaterialCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.MaterialCode);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "ItemNo",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrderProduct>> result = client.Get(ref cfg);

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
        //
        //POST: /PPM/WorkOrderProduct/PagingQuery
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

                using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
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
                        MethodReturnResult<IList<WorkOrderProduct>> result = client.Get(ref cfg);
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
        //
        // POST: /PPM/WorkOrderProduct/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderProductViewModel model)
        {
            using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
            {
                WorkOrderProduct obj = new WorkOrderProduct()
                {
                    Key = new WorkOrderProductKey()
                    {
                        OrderNumber = model.OrderNumber.ToUpper(),
                        MaterialCode = model.MaterialCode.ToUpper()
                    },
                    ItemNo = model.ItemNo,
                    IsMain = model.IsMain,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(PPMResources.StringResource.WorkOrderProduct_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /PPM/WorkOrderProduct/Modify
        public async Task<ActionResult> Modify(string orderNumber, string materialCode)
        {
            WorkOrderProductViewModel viewModel = new WorkOrderProductViewModel();
            using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
            {
                MethodReturnResult<WorkOrderProduct> result = await client.GetAsync(new WorkOrderProductKey()
                {
                    OrderNumber = orderNumber,
                    MaterialCode = materialCode
                });
                if (result.Code == 0)
                {
                    viewModel = new WorkOrderProductViewModel()
                    {
                        OrderNumber = result.Data.Key.OrderNumber,
                        MaterialCode = result.Data.Key.MaterialCode,
                        ItemNo = result.Data.ItemNo,
                        IsMain = result.Data.IsMain,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    return PartialView("_ModifyPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_ModifyPartial");
        }

        //
        // POST: /PPM/WorkOrderProduct/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderProductViewModel model)
        {
            using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
            {
                MethodReturnResult<WorkOrderProduct> result = await client.GetAsync(new WorkOrderProductKey()
                {
                    OrderNumber = model.OrderNumber,
                    MaterialCode = model.MaterialCode
                });

                if (result.Code == 0)
                {
                    result.Data.ItemNo = model.ItemNo;
                    result.Data.IsMain = model.IsMain;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(PPMResources.StringResource.WorkOrderProduct_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /PPM/WorkOrderProduct/Detail
        public async Task<ActionResult> Detail(string orderNumber, string materialCode)
        {
            using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
            {
                MethodReturnResult<WorkOrderProduct> result = await client.GetAsync(new WorkOrderProductKey()
                {
                    OrderNumber = orderNumber,
                    MaterialCode = materialCode
                });
                if (result.Code == 0)
                {
                    WorkOrderProductViewModel viewModel = new WorkOrderProductViewModel()
                    {
                        OrderNumber = result.Data.Key.OrderNumber,
                        MaterialCode = result.Data.Key.MaterialCode,
                        ItemNo = result.Data.ItemNo,
                        IsMain = result.Data.IsMain,
                        Creator = result.Data.Creator,
                        CreateTime = result.Data.CreateTime,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    return PartialView("_InfoPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_InfoPartial");
        }
        //
        // POST: /PPM/WorkOrderProduct/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string orderNumber, string materialCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
            {
                result = await client.DeleteAsync(new WorkOrderProductKey()
                {
                    OrderNumber = orderNumber,
                    MaterialCode = materialCode
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(PPMResources.StringResource.WorkOrderProduct_Delete_Success
                                                    , materialCode);
                }
                return Json(result);
            }
        }

        public ActionResult GetMaxItemNo(string orderNumber)
        {
            using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.OrderNumber='{0}'", orderNumber),
                    OrderBy = "ItemNo Desc"
                };
                MethodReturnResult<IList<WorkOrderProduct>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    return Json(result.Data[0].ItemNo + 1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }

    }
}