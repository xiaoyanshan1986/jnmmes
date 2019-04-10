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
    public class ShiftController : Controller
    {
        //
        // GET: /FMM/Shift/
        public async Task<ActionResult> Index()
        {
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<Shift>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new ShiftQueryViewModel());
        }

        //
        //POST: /FMM/Shift/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(ShiftQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ShiftServiceClient client = new ShiftServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" Key LIKE '{0}%'", model.Name);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Shift>> result = client.Get(ref cfg);

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
        //POST: /FMM/Shift/PagingQuery
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

                using (ShiftServiceClient client = new ShiftServiceClient())
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
                        MethodReturnResult<IList<Shift>> result = client.Get(ref cfg);
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
        // POST: /FMM/Shift/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ShiftViewModel model)
        {
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                Shift obj = new Shift()
                {
                    Key = model.Name,
                    EndTime=model.EndTime,
                    IsOverDay=model.IsOverDay,
                    StartTime=model.StartTime,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.Shift_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/Shift/Modify
        public async Task<ActionResult> Modify(string key)
        {
            ShiftViewModel viewModel = new ShiftViewModel();
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                MethodReturnResult<Shift> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new ShiftViewModel()
                    {
                        Name = result.Data.Key,
                        StartTime = result.Data.StartTime,
                        EndTime = result.Data.EndTime,
                        IsOverDay=result.Data.IsOverDay,
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
        // POST: /FMM/Shift/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(ShiftViewModel model)
        {
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                MethodReturnResult<Shift> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.StartTime= model.StartTime;
                    result.Data.EndTime = model.EndTime;
                    result.Data.IsOverDay = model.IsOverDay;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.Shift_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/Shift/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                MethodReturnResult<Shift> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    ShiftViewModel viewModel = new ShiftViewModel()
                    {
                        Name = result.Data.Key,
                        StartTime = result.Data.StartTime,
                        EndTime = result.Data.EndTime,
                        IsOverDay = result.Data.IsOverDay,
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
        // POST: /FMM/Shift/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.Shift_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}