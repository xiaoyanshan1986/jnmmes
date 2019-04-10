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
    public class ScheduleDayController : Controller
    {

        //
        // GET: /FMM/ScheduleDay/
        public async Task<ActionResult> Index(string locationName,string routeOperationName,string year,string month)
        {
            string sStartDate = string.Format("{0}-{1}-01", year, month);
            DateTime startDate = DateTime.Now;
            if (!DateTime.TryParse(sStartDate,out startDate))
            {
                return RedirectToAction("Index", "ScheduleMonth");
            }
            string scheduleName = string.Empty;
            //获取月排班计划。
            using (ScheduleMonthServiceClient client = new ScheduleMonthServiceClient())
            {
                MethodReturnResult<ScheduleMonth> result = await client.GetAsync(new ScheduleMonthKey
                {
                        LocationName=locationName,
                        RouteOperationName=routeOperationName,
                        Year=year,
                        Month=month
                });
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "ScheduleMonth");
                }
                scheduleName = result.Data.ScheduleName;
                ViewBag.ScheduleMonth = result.Data;
            }
            //获取排班计划明细。
            using (ScheduleDetailServiceClient client = new ScheduleDetailServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.ScheduleName='{0}'",scheduleName)
                };
                MethodReturnResult<IList<ScheduleDetail>> result = client.Get(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.ScheduleDetailList = result.Data;
                }
            }
            //获取日排班计划。
            using (ScheduleDayServiceClient client = new ScheduleDayServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        Where = string.Format(@"Key.LocationName='{0}' 
                                                AND Key.RouteOperationName='{1}' 
                                                AND Key.Day>='{2}' 
                                                AND Key.Day<='{3}'",
                                                locationName,
                                                routeOperationName,
                                                startDate.ToString("yyyy-MM-dd"),
                                                startDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"))
                    };
                    MethodReturnResult<IList<ScheduleDay>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.ScheduleDayList = result.Data;
                    }
                });
            }
            return View();
        }
        //
        // POST: /FMM/ScheduleDay/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save()
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                string LocationName = Request["LocationName"];
                string RouteOperationName = Request["RouteOperationName"];
                string[] SeqNo = Request["SeqNo"].Split(',');
                string[] Day = Request["Day"].Split(',');
                string[] ShiftName = Request["ShiftName"].Split(',');
                string[] StartTime = Request["StartTime"].Split(',');
                string[] EndTime = Request["EndTime"].Split(',');
                string[] ShiftValue = Request["ShiftValue"].Split(',');
                DateTime now=DateTime.Now;
                string userName=User.Identity.Name;
                List<ScheduleDay> lst = new List<ScheduleDay>();

                for (int i = 0; i < SeqNo.Length;i++)
                {
                    lst.Add(new ScheduleDay()
                    {
                        CreateTime = now,
                        Creator = userName,
                        Editor = userName,
                        EditTime = now,
                        EndTime = Convert.ToDateTime(EndTime[i]),
                        StartTime = Convert.ToDateTime(StartTime[i]),
                        SeqNo = Convert.ToInt32(SeqNo[i]),
                        ShiftValue = ShiftValue[i],
                        Key = new ScheduleDayKey()
                        {
                            Day = Convert.ToDateTime(Day[i]),
                            LocationName = LocationName,
                            RouteOperationName = RouteOperationName,
                            ShiftName = ShiftName[i]
                        }
                    });
                }

                using (ScheduleDayServiceClient client = new ScheduleDayServiceClient())
                {
                    result = await client.ModifyAsync(lst);
                }
                if (result.Code == 0)
                {
                    string name = string.Format("{0}{1}", LocationName, RouteOperationName);
                    result.Message = string.Format(FMMResources.StringResource.ScheduleDay_Save_Success, name);
                }
            }
            catch(Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return Json(result);
        }

    }
}