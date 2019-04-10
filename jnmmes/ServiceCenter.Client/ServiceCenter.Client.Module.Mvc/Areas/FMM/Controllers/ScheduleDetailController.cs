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
    public class ScheduleDetailController : Controller
    {

        //
        // GET: /FMM/ScheduleDetail/
        public async Task<ActionResult> Index(string scheduleName)
        {
            using (ScheduleServiceClient client = new ScheduleServiceClient())
            {
                MethodReturnResult<Schedule> result = await client.GetAsync(scheduleName ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "Schedule");
                }
                ViewBag.Schedule = result.Data;
            }

            using (ScheduleDetailServiceClient client = new ScheduleDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.ScheduleName = '{0}'"
                                                    , scheduleName)
                    };
                    MethodReturnResult<IList<ScheduleDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new ScheduleDetailQueryViewModel() { ScheduleName = scheduleName });
        }

        //
        //POST: /FMM/ScheduleDetail/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(ScheduleDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ScheduleDetailServiceClient client = new ScheduleDetailServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.ScheduleName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ScheduleName);

                            if (!string.IsNullOrEmpty(model.ShiftName))
                            {
                                where.AppendFormat(" {0} Key.ShiftName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ShiftName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<ScheduleDetail>> result = client.Get(ref cfg);

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
        //POST: /FMM/ScheduleDetail/PagingQuery
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

                using (ScheduleDetailServiceClient client = new ScheduleDetailServiceClient())
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
                        MethodReturnResult<IList<ScheduleDetail>> result = client.Get(ref cfg);
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
        // POST: /FMM/ScheduleDetail/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ScheduleDetailViewModel model)
        {
            using (ScheduleDetailServiceClient client = new ScheduleDetailServiceClient())
            {
                ScheduleDetail obj = new ScheduleDetail()
                {
                    Key = new ScheduleDetailKey()
                    {
                        ScheduleName = model.ScheduleName,
                        ShiftName = model.ShiftName
                    },
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime=DateTime.Now,
                    Creator=User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.ScheduleDetail_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
       
        //
        // POST: /FMM/ScheduleDetail/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string scheduleName, string shiftName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ScheduleDetailServiceClient client = new ScheduleDetailServiceClient())
            {
                result = await client.DeleteAsync(new ScheduleDetailKey()
                {
                    ScheduleName = scheduleName,
                    ShiftName = shiftName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.ScheduleDetail_Delete_Success
                                                    , shiftName);
                }
                return Json(result);
            }
        }
    }
}