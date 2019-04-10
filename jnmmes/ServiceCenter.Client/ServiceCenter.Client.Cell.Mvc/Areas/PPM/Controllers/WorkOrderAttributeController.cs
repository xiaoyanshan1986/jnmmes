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

namespace ServiceCenter.Client.Mvc.Areas.PPM.Controllers
{
    public class WorkOrderAttributeController : Controller
    {

        //
        // GET: /PPM/WorkOrderAttribute/
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

            using (WorkOrderAttributeServiceClient client = new WorkOrderAttributeServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.OrderNumber = '{0}'"
                                                    , orderNumber),
                        OrderBy = "Key.OrderNumber,Key.AttributeName"
                    };
                    MethodReturnResult<IList<WorkOrderAttribute>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new WorkOrderAttributeQueryViewModel() { OrderNumber=orderNumber });
        }

        //
        //POST: /PPM/WorkOrderAttribute/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderAttributeQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderAttributeServiceClient client = new WorkOrderAttributeServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.OrderNumber = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);

                            if (!string.IsNullOrEmpty(model.AttributeName))
                            {
                                where.AppendFormat(" {0} Key.AttributeName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.AttributeName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrderAttribute>> result = client.Get(ref cfg);

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
        //POST: /PPM/WorkOrderAttribute/PagingQuery
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

                using (WorkOrderAttributeServiceClient client = new WorkOrderAttributeServiceClient())
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
                        MethodReturnResult<IList<WorkOrderAttribute>> result = client.Get(ref cfg);
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
        // POST: /PPM/WorkOrderAttribute/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderAttributeViewModel model)
        {
            using (WorkOrderAttributeServiceClient client = new WorkOrderAttributeServiceClient())
            {
                WorkOrderAttribute obj = new WorkOrderAttribute()
                {
                    Key = new WorkOrderAttributeKey() { 
                         OrderNumber=model.OrderNumber,
                         AttributeName=model.AttributeName
                    },
                    AttributeValue=model.Value,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(PPMResources.StringResource.WorkOrderAttribute_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /PPM/WorkOrderAttribute/Modify
        public async Task<ActionResult> Modify(string orderNumber,string attributeName)
        {
            WorkOrderAttributeViewModel viewModel = new WorkOrderAttributeViewModel();
            using (WorkOrderAttributeServiceClient client = new WorkOrderAttributeServiceClient())
            {
                MethodReturnResult<WorkOrderAttribute> result = await client.GetAsync(new WorkOrderAttributeKey()
                {
                    OrderNumber=orderNumber,
                    AttributeName=attributeName
                });
                if (result.Code == 0)
                {
                    viewModel = new WorkOrderAttributeViewModel()
                    {
                        OrderNumber = result.Data.Key.OrderNumber,
                        AttributeName = result.Data.Key.AttributeName,
                        Value = result.Data.AttributeValue,
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
        // POST: /PPM/WorkOrderAttribute/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderAttributeViewModel model)
        {
            using (WorkOrderAttributeServiceClient client = new WorkOrderAttributeServiceClient())
            {
                MethodReturnResult<WorkOrderAttribute> result = await client.GetAsync(new WorkOrderAttributeKey()
                {
                    OrderNumber = model.OrderNumber,
                    AttributeName = model.AttributeName
                });

                if (result.Code == 0)
                {
                    result.Data.AttributeValue = model.Value;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(PPMResources.StringResource.WorkOrderAttribute_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /PPM/WorkOrderAttribute/Detail
        public async Task<ActionResult> Detail(string orderNumber, string attributeName)
        {
            using (WorkOrderAttributeServiceClient client = new WorkOrderAttributeServiceClient())
            {
                MethodReturnResult<WorkOrderAttribute> result = await client.GetAsync(new WorkOrderAttributeKey()
                {
                    OrderNumber = orderNumber,
                    AttributeName = attributeName
                });
                if (result.Code == 0)
                {
                    WorkOrderAttributeViewModel viewModel = new WorkOrderAttributeViewModel()
                    {
                        OrderNumber = result.Data.Key.OrderNumber,
                        AttributeName = result.Data.Key.AttributeName,
                        Value = result.Data.AttributeValue,
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
        // POST: /PPM/WorkOrderAttribute/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string orderNumber, string attributeName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WorkOrderAttributeServiceClient client = new WorkOrderAttributeServiceClient())
            {
                result = await client.DeleteAsync(new WorkOrderAttributeKey()
                {
                    OrderNumber = orderNumber,
                    AttributeName = attributeName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(PPMResources.StringResource.WorkOrderAttribute_Delete_Success
                                                    , attributeName);
                }
                return Json(result);
            }
        }
    }
}