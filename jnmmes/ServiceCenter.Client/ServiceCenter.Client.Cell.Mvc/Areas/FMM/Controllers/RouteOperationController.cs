using ServiceCenter.Client.Mvc.Areas.FMM.Models;
using FMMResources = ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Controllers
{
    public class RouteOperationController : Controller
    {

        //
        // GET: /FMM/RouteOperation/
        public async Task<ActionResult> Index()
        {
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "SortSeq"
                    };
                    MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RouteOperationQueryViewModel());
        }

        //
        //POST: /FMM/RouteOperation/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RouteOperationQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RouteOperationServiceClient client = new RouteOperationServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "SortSeq",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);

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
        //POST: /FMM/RouteOperation/PagingQuery
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

                using (RouteOperationServiceClient client = new RouteOperationServiceClient())
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
                        MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
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
        // POST: /FMM/RouteOperation/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RouteOperationViewModel model)
        {
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                RouteOperation obj = new RouteOperation()
                {
                    Key = model.Name,
                    DefectReasonCodeCategoryName=model.DefectReasonCodeCategoryName,
                    Duration=model.Duration,
                    ScrapReasonCodeCategoryName=model.ScrapReasonCodeCategoryName,
                    SortSeq=model.SortSeq,
                    Status=model.Status,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.RouteOperation_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/RouteOperation/Modify
        public async Task<ActionResult> Modify(string key)
        {
            RouteOperationViewModel viewModel = new RouteOperationViewModel();
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                MethodReturnResult<RouteOperation> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new RouteOperationViewModel()
                    {
                        Name = result.Data.Key,
                        SortSeq=result.Data.SortSeq,
                        ScrapReasonCodeCategoryName=result.Data.ScrapReasonCodeCategoryName,
                        Duration=result.Data.Duration,
                        DefectReasonCodeCategoryName=result.Data.DefectReasonCodeCategoryName,
                        Status=result.Data.Status,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
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
        // POST: /FMM/RouteOperation/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RouteOperationViewModel model)
        {
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                MethodReturnResult<RouteOperation> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.Status = model.Status;
                    result.Data.DefectReasonCodeCategoryName = model.DefectReasonCodeCategoryName;
                    result.Data.Duration = model.Duration;
                    result.Data.ScrapReasonCodeCategoryName = model.ScrapReasonCodeCategoryName;
                    result.Data.SortSeq = model.SortSeq;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.RouteOperation_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/RouteOperation/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                MethodReturnResult<RouteOperation> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    RouteOperationViewModel viewModel = new RouteOperationViewModel()
                    {
                        Name = result.Data.Key,
                        SortSeq = result.Data.SortSeq,
                        ScrapReasonCodeCategoryName = result.Data.ScrapReasonCodeCategoryName,
                        Duration = result.Data.Duration,
                        DefectReasonCodeCategoryName = result.Data.DefectReasonCodeCategoryName,
                        Status = result.Data.Status,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
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
        // POST: /FMM/RouteOperation/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.RouteOperation_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
        public ActionResult GetMaxSeqNo()
        {
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    OrderBy = "SortSeq Desc"
                };
                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    return Json(result.Data[0].SortSeq + 1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}