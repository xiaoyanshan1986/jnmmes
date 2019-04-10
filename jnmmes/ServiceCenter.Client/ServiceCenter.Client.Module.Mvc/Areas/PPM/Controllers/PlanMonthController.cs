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
    public class PlanMonthController : Controller
    {
        // GET: /PPM/PlanMonth/
        public async Task<ActionResult> Index()
        {
            //using (PlanMonthServiceClient client = new PlanMonthServiceClient())
            //{
            //    await Task.Run(() =>
            //    {
            //        PagingConfig cfg = new PagingConfig()
            //        {
            //            OrderBy = "Key"
            //        };
            //        MethodReturnResult<IList<PlanMonth>> result = client.Get(ref cfg);

            //        if (result.Code == 0)
            //        {
            //            ViewBag.PagingConfig = cfg;
            //            ViewBag.List = result.Data;
            //        }
            //    });
            //}

            PlanMonthQueryViewModel model = new PlanMonthQueryViewModel
            {
                //初始化参数
                Year = System.DateTime.Now.ToString("yyyy"),
                Month = System.DateTime.Now.ToString("MM")
            };
            return View(model);
        }

        //
        //POST: /PPM/PlanMonth/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(PlanMonthQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (PlanMonthServiceClient client = new PlanMonthServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
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
                            if (!string.IsNullOrEmpty(model.LocationName))
                            {
                                where.AppendFormat(" {0} Key.LocationName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.LocationName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<PlanMonth>> result = client.Get(ref cfg);

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
        //POST: /PPM/PlanMonth/PagingQuery
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

                using (PlanMonthServiceClient client = new PlanMonthServiceClient())
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
                        MethodReturnResult<IList<PlanMonth>> result = client.Get(ref cfg);
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
        // POST: /PPM/PlanMonth/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(PlanMonthViewModel model)
        {
            using (PlanMonthServiceClient client = new PlanMonthServiceClient())
            {
                PlanMonth obj = new PlanMonth()
                {
                    Key = new PlanMonthKey()
                    {
                        Year = model.Year,
                        Month = model.Month,
                        LocationName = model.LocationName
                    },
                    PlanQty = model.PlanQty,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(PPMResources.StringResource.PlanMonth_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }

        //
        // POST: /PPM/PlanMonth/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string year, string month, string locationname)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                //判断是否存在日明细记录
                using (PlanDayServiceClient client = new PlanDayServiceClient())
                {
                    StringBuilder where = new StringBuilder();

                    //年度条件
                    if (!string.IsNullOrEmpty(year))
                    {
                        where.AppendFormat(" {0} Key.Year = '{1}'"
                                            , where.Length > 0 ? "AND" : string.Empty
                                            , year);
                    }

                    //月度条件
                    if (!string.IsNullOrEmpty(month))
                    {
                        where.AppendFormat(" {0} Key.Month = '{1}'"
                                            , where.Length > 0 ? "and" : string.Empty
                                            , month);
                    }

                    //车间条件
                    if (!string.IsNullOrEmpty(locationname))
                    {
                        where.AppendFormat(" {0} Key.LocationName = '{1}'"
                                            , where.Length > 0 ? "AND" : string.Empty
                                            , locationname);
                    }

                    //设置参数
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key",
                        Where = where.ToString()
                    };

                    //取得数据
                    MethodReturnResult<IList<PlanDay>> resultlist = client.Get(ref cfg);

                    if (resultlist.Code == 0)
                    {
                        if (resultlist.Data.Count > 0)
                        {
                            //数据错误
                            result.Code = 1000;                     //错误代码
                            result.Message = "存在日计划";    //错误信息

                            return Json(result);
                        }
                    }
                    else
                    {
                        //数据错误
                        result.Code = resultlist.Code;          //错误代码
                        result.Message = resultlist.Message;    //错误信息
                        result.Detail = resultlist.Message;     //错误明细

                        return Json(result);
                    }
                }

                using (PlanMonthServiceClient client = new PlanMonthServiceClient())
                {
                    PlanMonthKey key = new PlanMonthKey()
                    {
                        Year = year,
                        Month = month, 
                        LocationName = locationname == null? "":locationname
                    };

                    result = await client.DeleteAsync(key);

                    if (result.Code == 0)
                    {
                        result.Message = string.Format(PPMResources.StringResource.PlanMonth_Delete_Success
                                                       ,key);
                    }
                    
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();

                return Json(result);
            }
        }
    }
}