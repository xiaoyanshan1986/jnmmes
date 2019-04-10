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
    public class ScheduleMonthController : Controller
    {

        //
        // GET: /FMM/ScheduleMonth/
        public async Task<ActionResult> Index()
        {
            using (ScheduleMonthServiceClient client = new ScheduleMonthServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<ScheduleMonth>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new ScheduleMonthQueryViewModel());
        }

        //
        //POST: /FMM/ScheduleMonth/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(ScheduleMonthQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ScheduleMonthServiceClient client = new ScheduleMonthServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.LocationName))
                            {
                                where.AppendFormat(" {0} Key.LocationName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.LocationName);
                            }
                            if (!string.IsNullOrEmpty(model.RouteOperationName))
                            {
                                where.AppendFormat(" {0} Key.RouteOperationName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteOperationName);
                            }
                            if (!string.IsNullOrEmpty(model.Year))
                            {
                                where.AppendFormat(" {0} Key.Year LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Year);
                            }
                            if (!string.IsNullOrEmpty(model.Month))
                            {
                                where.AppendFormat(" {0} Key.Month LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Month);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<ScheduleMonth>> result = client.Get(ref cfg);

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
        //POST: /FMM/ScheduleMonth/PagingQuery
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

                using (ScheduleMonthServiceClient client = new ScheduleMonthServiceClient())
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
                        MethodReturnResult<IList<ScheduleMonth>> result = client.Get(ref cfg);
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
        // POST: /FMM/ScheduleMonth/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ScheduleMonthViewModel model)
        {
            using (ScheduleMonthServiceClient client = new ScheduleMonthServiceClient())
            {
                ScheduleMonth obj = new ScheduleMonth()
                {
                    Key = new ScheduleMonthKey()
                    {
                        LocationName=model.LocationName,
                        RouteOperationName=model.RouteOperationName,
                        Year=model.Year,
                        Month=model.Month
                    },
                    ScheduleName=model.ScheduleName,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.ScheduleMonth_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }

        //
        // POST: /FMM/ScheduleMonth/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string locationName,string routeOperationName,string year,string month)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ScheduleMonthServiceClient client = new ScheduleMonthServiceClient())
            {
                ScheduleMonthKey key=new ScheduleMonthKey()
                {
                    LocationName = locationName,
                    RouteOperationName = routeOperationName,
                    Year = year,
                    Month = month
                };
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.ScheduleMonth_Delete_Success
                                                   ,key);
                }
                return Json(result);
            }
        }
    }
}