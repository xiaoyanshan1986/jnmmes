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
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Controllers
{
    public class DefectController : Controller
    {
        // GET: /PPM/Defect/
        /// <summary>
        /// 页面开始处理事物
        /// </summary>
        /// <param name="year">         年</param>
        /// <param name="month">        月</param>
        /// <param name="locationname"> 车间</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string year,string month,string locationname)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                //初始化参数
                string sYear = System.DateTime.Now.ToString("yyyy");
                string sMonth = System.DateTime.Now.ToString("MM");

                //获取日生产不良。
                using (DefectServiceClient client = new DefectServiceClient())
                {
                    await Task.Run(() =>
                    {
                        //设置查询条件
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@" Key.Year = '{0}' 
                                                    AND Key.Month = '{1}'",
                                                    sYear,
                                                    sMonth
                                                    )
                        };

                        //取得列表数据
                        MethodReturnResult<IList<Defect>> resultlist = client.Get(ref cfg);

                        if (resultlist.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = resultlist.Data;
                        }
                        else
                        {
                            //数据错误
                            result.Code = resultlist.Code;          //错误代码
                            result.Message = resultlist.Message;    //错误信息
                            result.Detail = resultlist.Message;     //错误明细                       
                        }
                    });

                    //处理错误信息
                    if (result.Code > 0)
                    {
                        return Json(result);            //终止并返回程序
                    }
                }

                DefectQueryViewModel model = new DefectQueryViewModel
                {
                    //初始化参数
                    qYear = sYear,                        //年
                    qMonth = sMonth,                      //月
                };

                return View(model);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();

                return Json(result);
            }            
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> Query(DefectQueryViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();            

            try
            {
                using (DefectServiceClient client = new DefectServiceClient())
                {
                    //取得数据
                    await Task.Run(() =>
                    {
                        //取数条件
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            //年度条件
                            if (!string.IsNullOrEmpty(model.qYear))
                            {
                                where.AppendFormat(" {0} Key.Year = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.qYear);
                            }

                            //月度条件
                            if (!string.IsNullOrEmpty(model.qMonth))
                            {
                                where.AppendFormat(" {0} Key.Month = '{1}'"
                                                    , where.Length > 0 ? "and" : string.Empty
                                                    , model.qMonth);
                            }

                            //车间条件
                            if (!string.IsNullOrEmpty(model.LocationName))
                            {
                                where.AppendFormat(" {0} Key.LocationName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.LocationName);
                            }

                            //班别条件
                            if (!string.IsNullOrEmpty(model.ShiftName))
                            {
                                where.AppendFormat(" {0} Key.ShiftName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ShiftName);
                            }
                        }

                        //设置参数
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };

                        //取得数据
                        MethodReturnResult<IList<Defect>> resultlist = client.Get(ref cfg);

                        if (resultlist.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = resultlist.Data;
                        }
                        else
                        {
                            //数据错误
                            result.Code = resultlist.Code;          //错误代码
                            result.Message = resultlist.Message;    //错误信息
                            result.Detail = resultlist.Message;     //错误明细
                        }
                    });

                    //处理错误信息
                    if (result.Code > 0)
                    {
                        return Json(result);                        //终止并返回程序
                    }
                }

                return PartialView("_ListPartial");
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }            
        }

        //POST: /PPM/Defect/PagingQuery
        /// <summary>
        /// 页面刷新
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="currentPageNo"></param>
        /// <param name="currentPageSize"></param>
        /// <returns></returns>
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

                using (DefectServiceClient client = new DefectServiceClient())
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
                        MethodReturnResult<IList<Defect>> result = client.Get(ref cfg);
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
        
        /// <summary>
        /// 新增日不良
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> Add(DefectViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                using (DefectServiceClient client = new DefectServiceClient())
                {
                    Defect obj = new Defect()
                    {
                        Key = new DefectKey()
                        {
                            Year = model.Year,
                            Month = model.Month,
                            Day = model.Day,
                            LocationName = model.LocationName,
                            ShiftName = model.ShiftName,
                            ReasonCodeCategoryName = model.ReasonCodeCategoryName,
                            ReasonCodeName = model.ReasonCodeName
                        },

                        Qty = model.Qty,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        Creator = User.Identity.Name
                    };

                    MethodReturnResult rst = await client.AddAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(PPMResources.StringResource.Defect_Add_Success,obj.Key);
                    }
                    else
                    {
                        //数据错误
                        result.Code = rst.Code;          //错误代码
                        result.Message = rst.Message;    //错误信息
                        result.Detail = rst.Message;     //错误明细

                        return Json(result);
                    }

                    return Json(rst);
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }
        }
        
        /// <summary>
        /// 删除不良
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <param name="locationname">车间</param>
        /// <param name="shiftname">班别</param>
        /// <param name="reasoncodecategoryname">不良组</param>
        /// <param name="reasoncodename">不良原因</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string year, string month, string day, string locationname, string shiftname, string reasoncodecategoryname, string reasoncodename)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                using (DefectServiceClient client = new DefectServiceClient())
                {
                    DefectKey key = new DefectKey()
                    {
                        Year = year,
                        Month = month,
                        Day = day,
                        LocationName = locationname == null ? "" : locationname,
                        ShiftName = shiftname,
                        ReasonCodeCategoryName = reasoncodecategoryname,
                        ReasonCodeName = reasoncodename
                    };

                    result = await client.DeleteAsync(key);

                    if (result.Code == 0)
                    {
                        result.Message = string.Format(PPMResources.StringResource.Defect_Delete_Success, key);
                    }
                    else
                    {
                        //数据错误
                        result.Code = result.Code;          //错误代码
                        result.Message = result.Message;    //错误信息
                        result.Detail = result.Message;     //错误明细

                        return Json(result);
                    }

                    return Json(result);
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }
        }

        /// <summary>
        /// 修改窗体
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <param name="locationname">车间</param>
        /// <param name="shiftname">班别</param>
        /// <param name="reasoncodecategoryname">不良组</param>
        /// <param name="reasoncodename">不良原因</param>
        /// <returns></returns>
        public async Task<ActionResult> Modify(string year, string month, string day, string locationname, string shiftname, string reasoncodecategoryname, string reasoncodename)
        {
            MethodReturnResult<Defect> result = new MethodReturnResult<Defect>();

            try
            {
                DefectViewModel viewModel = new DefectViewModel();

                using (DefectServiceClient client = new DefectServiceClient())
                {
                    DefectKey key = new DefectKey()
                    {
                        Year = year,
                        Month = month,
                        Day = day,
                        LocationName = locationname == null ? "" : locationname,
                        ShiftName = shiftname,
                        ReasonCodeCategoryName = reasoncodecategoryname,
                        ReasonCodeName = reasoncodename
                    };

                    result = await client.GetAsync(key);

                    if (result.Code == 0)
                    {
                        viewModel = new DefectViewModel()
                        {
                            Year = result.Data.Key.Year,
                            Month = result.Data.Key.Month,
                            Day = result.Data.Key.Day,
                            LocationName = result.Data.Key.LocationName,
                            ShiftName = result.Data.Key.ShiftName,
                            ReasonCodeCategoryName = result.Data.Key.ReasonCodeCategoryName,
                            ReasonCodeName = result.Data.Key.ReasonCodeName,
                            Qty = result.Data.Qty,
                            CreateTime = result.Data.CreateTime,
                            Creator = result.Data.Creator,
                            Editor = result.Data.Editor,
                            EditTime = result.Data.EditTime
                        };

                        return PartialView("_ModifyPartial", viewModel);
                    }
                    else
                    {
                        //数据错误
                        result.Code = result.Code;          //错误代码
                        result.Message = result.Message;    //错误信息
                        result.Detail = result.Message;     //错误明细

                        return Json(result);
                    }
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }
        }

        /// <summary>
        /// 修改保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(DefectViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                using (DefectServiceClient client = new DefectServiceClient())
                {
                    Defect obj = new Defect()
                    {
                        Key = new DefectKey()
                        {
                            Year = model.Year,
                            Month = model.Month,
                            Day = model.Day,
                            LocationName = model.LocationName,
                            ShiftName = model.ShiftName,
                            ReasonCodeCategoryName = model.ReasonCodeCategoryName,
                            ReasonCodeName = model.ReasonCodeName
                        },

                        Qty = model.Qty,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now,
                        CreateTime = model.CreateTime,
                        Creator = model.Creator
                    };

                    MethodReturnResult rst = await client.ModifyAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(PPMResources.StringResource.Defect_Modify_Success, obj.Key);
                    }
                    else
                    {
                        //数据错误
                        result.Code = rst.Code;          //错误代码
                        result.Message = rst.Message;    //错误信息
                        result.Detail = rst.Message;     //错误明细

                        return Json(result);
                    }

                    return Json(rst);
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }
        }

        /// <summary>
        /// 浏览信息
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <param name="locationname">车间</param>
        /// <param name="shiftname">班别</param>
        /// <param name="reasoncodecategoryname">不良组</param>
        /// <param name="reasoncodename">不良原因</param>
        /// <returns></returns>
        public async Task<ActionResult> Detail(string year, string month, string day, string locationname, string shiftname, string reasoncodecategoryname, string reasoncodename)
        {
            MethodReturnResult<Defect> result = new MethodReturnResult<Defect>();

            try
            {
                DefectViewModel viewModel = new DefectViewModel();

                using (DefectServiceClient client = new DefectServiceClient())
                {
                    DefectKey key = new DefectKey()
                    {
                        Year = year,
                        Month = month,
                        Day = day,
                        LocationName = locationname == null ? "" : locationname,
                        ShiftName = shiftname,
                        ReasonCodeCategoryName = reasoncodecategoryname,
                        ReasonCodeName = reasoncodename
                    };

                    //取得数据
                    result = await client.GetAsync(key);

                    if (result.Code == 0)
                    {
                        viewModel = new DefectViewModel()
                        {
                            Year = result.Data.Key.Year,
                            Month = result.Data.Key.Month,
                            Day = result.Data.Key.Day,
                            LocationName = result.Data.Key.LocationName,
                            ShiftName = result.Data.Key.ShiftName,
                            Qty = result.Data.Qty,
                            CreateTime = result.Data.CreateTime,
                            Creator = result.Data.Creator,
                            Editor = result.Data.Editor,
                            EditTime = result.Data.EditTime
                        };

                        return PartialView("_InfoPartial", viewModel);
                    }
                    else
                    {
                        //数据错误
                        result.Code = result.Code;          //错误代码
                        result.Message = result.Message;    //错误信息
                        result.Detail = result.Message;     //错误明细

                        return Json(result);
                    }
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }            
        }

        /// <summary>
        /// 取得当月的日列表
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public ActionResult GetDays(string year, string month)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                List<SelectListItem> lst = new List<SelectListItem>();
                if (year != null)
                {
                    string sDate = year + month + "01";
                    DateTime dtData = DateTime.ParseExact(sDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture); ;

                    int days = DateTime.DaysInMonth(dtData.Year, dtData.Month);

                    for (int i = 1; i <= days; i++)
                    {
                        string value = i.ToString("00");
                        lst.Add(new SelectListItem()
                        {
                            Text = value,
                            Value = value
                        });
                    }
                }

                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }
        }

        public ActionResult GetReasonCodeName(string reasonCodeCategoryName)
        {
            MethodReturnResult<IList<ReasonCodeCategoryDetail>> result = new MethodReturnResult<IList<ReasonCodeCategoryDetail>>();

            try
            {
                using (ReasonCodeCategoryDetailServiceClient client = new ReasonCodeCategoryDetailServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("REASON_CODE_CATEGORY_NAME='{0}'", reasonCodeCategoryName)
                    };

                    result = client.Get(ref cfg);
                    if (result.Code <= 0)
                    {
                        List<SelectListItem> lst = new List<SelectListItem>();

                        foreach (ReasonCodeCategoryDetail item in result.Data)
                        {
                            lst.Add(new SelectListItem() { Text = item.Key.ReasonCodeName, Value = item.Key.ReasonCodeName });
                        }

                        return Json(lst, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(result);
                    }
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }
        }

    }
}