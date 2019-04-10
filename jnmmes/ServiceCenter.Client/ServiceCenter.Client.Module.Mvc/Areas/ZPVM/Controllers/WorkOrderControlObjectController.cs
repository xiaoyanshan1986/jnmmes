using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class WorkOrderControlObjectController : Controller
    {
        //
        // GET: /ZPVM/WorkOrderControlObject/
        public async Task<ActionResult> Index(string OrderNumber, string MaterialCode)
        {
            if (string.IsNullOrEmpty(OrderNumber) || string.IsNullOrEmpty(MaterialCode))
            {
                return RedirectToAction("Index", "WorkOrderRule");
            }

            using (WorkOrderRuleServiceClient client = new WorkOrderRuleServiceClient())
            {
                MethodReturnResult<WorkOrderRule> result = await client.GetAsync(new WorkOrderRuleKey()
                {
                   OrderNumber=OrderNumber,
                   MaterialCode=MaterialCode
                });

                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "WorkOrderRule");
                }
                ViewBag.Rule = result.Data;
            }

            using (WorkOrderControlObjectServiceClient client = new WorkOrderControlObjectServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.OrderNumber = '{0}' AND Key.MaterialCode='{1}'"
                                              , OrderNumber
                                              , MaterialCode),
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<WorkOrderControlObject>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new WorkOrderControlObjectQueryViewModel() { OrderNumber = OrderNumber, MaterialCode = MaterialCode });
        }

        //
        //POST: /ZPVM/WorkOrderControlObject/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderControlObjectQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderControlObjectServiceClient client = new WorkOrderControlObjectServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" Key.OrderNumber = '{0}' AND Key.MaterialCode='{1}'"
                                              , model.OrderNumber
                                              , model.MaterialCode);

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrderControlObject>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/WorkOrderControlObject/PagingQuery
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

                using (WorkOrderControlObjectServiceClient client = new WorkOrderControlObjectServiceClient())
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
                        MethodReturnResult<IList<WorkOrderControlObject>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/WorkOrderControlObject/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderControlObjectViewModel model)
        {
            using (WorkOrderControlObjectServiceClient client = new WorkOrderControlObjectServiceClient())
            {
                WorkOrderControlObject obj = new WorkOrderControlObject()
                {
                    Key = new WorkOrderControlObjectKey(){
                        MaterialCode=model.MaterialCode,
                        OrderNumber=model.OrderNumber,
                        Object=model.Object,
                        Type=model.Type
                    },
                    Value=model.Value,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderControlObject_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/WorkOrderControlObject/Modify
        public async Task<ActionResult> Modify(string OrderNumber, string MaterialCode, EnumPVMTestDataType obj, string type)
        {
            WorkOrderControlObjectViewModel viewModel = new WorkOrderControlObjectViewModel();
            using (WorkOrderControlObjectServiceClient client = new WorkOrderControlObjectServiceClient())
            {
                MethodReturnResult<WorkOrderControlObject> result = await client.GetAsync(new WorkOrderControlObjectKey()
                {
                    MaterialCode = MaterialCode,
                    OrderNumber = OrderNumber,
                    Object=obj,
                    Type=type
                });
                if (result.Code == 0)
                {
                    viewModel = new WorkOrderControlObjectViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        OrderNumber = result.Data.Key.OrderNumber,
                        Value=result.Data.Value,
                        Type=result.Data.Key.Type,
                        Object=result.Data.Key.Object,
                        IsUsed=result.Data.IsUsed,
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
            return PartialView("_ModifyPartial", new WorkOrderControlObjectViewModel());
        }

        //
        // POST: /ZPVM/WorkOrderControlObject/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderControlObjectViewModel model)
        {
            using (WorkOrderControlObjectServiceClient client = new WorkOrderControlObjectServiceClient())
            {
                WorkOrderControlObjectKey key = new WorkOrderControlObjectKey()
                {
                    MaterialCode = model.MaterialCode,
                    OrderNumber = model.OrderNumber,
                    Type=model.Type,
                    Object=model.Object
                };
                MethodReturnResult<WorkOrderControlObject> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.Value = model.Value;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderControlObject_SaveModify_Success
                                                    , key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/WorkOrderControlObject/Detail
        public async Task<ActionResult> Detail(string OrderNumber, string MaterialCode, EnumPVMTestDataType obj, string type)
        {
            using (WorkOrderControlObjectServiceClient client = new WorkOrderControlObjectServiceClient())
            {
                WorkOrderControlObjectKey key = new WorkOrderControlObjectKey()
                {
                    MaterialCode = MaterialCode,
                    OrderNumber = OrderNumber,
                    Object=obj,
                    Type=type
                };
                MethodReturnResult<WorkOrderControlObject> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    WorkOrderControlObjectViewModel viewModel = new WorkOrderControlObjectViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        OrderNumber = result.Data.Key.OrderNumber,
                        Value = result.Data.Value,
                        Type = result.Data.Key.Type,
                        Object = result.Data.Key.Object,
                        IsUsed = result.Data.IsUsed,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
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
        // POST: /ZPVM/WorkOrderControlObject/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string OrderNumber, string MaterialCode, EnumPVMTestDataType obj, string type)
        {
            MethodReturnResult result = new MethodReturnResult();
            WorkOrderControlObjectKey key = new WorkOrderControlObjectKey()
            {
                MaterialCode = MaterialCode,
                OrderNumber = OrderNumber,
                Object=obj,
                Type=type
            };
            using (WorkOrderControlObjectServiceClient client = new WorkOrderControlObjectServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.WorkOrderControlObject_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}