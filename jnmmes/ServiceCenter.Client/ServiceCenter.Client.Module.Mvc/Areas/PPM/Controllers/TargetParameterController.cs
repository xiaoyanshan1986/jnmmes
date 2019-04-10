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
    public class TargetParameterController : Controller
    {
        // GET: /PPM/TargetParameter/
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

                //获取日生产计划。
                using (TargetParameterServiceClient client = new TargetParameterServiceClient())
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
                        MethodReturnResult<IList<TargetParameter>> resultlist = client.Get(ref cfg);

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

                TargetParameterQueryViewModel model = new TargetParameterQueryViewModel
                {
                    //初始化参数
                    qYear = sYear,                        //年
                    qMonth = sMonth,                      //月
                    ItemType = "0"
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
        public async Task<ActionResult> Query(TargetParameterQueryViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();            

            try
            {
                using (TargetParameterServiceClient client = new TargetParameterServiceClient())
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

                            //项目类型
                            if (!string.IsNullOrEmpty(model.ItemType))
                            {
                                where.AppendFormat(" {0} Key.ItemType = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ItemType);
                            }

                            //项目
                            if (!string.IsNullOrEmpty(model.ItemCode))
                            {
                                where.AppendFormat(" {0} Key.ItemCode = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ItemCode);
                            }
                        }

                        //设置参数
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };

                        //取得数据
                        MethodReturnResult<IList<TargetParameter>> resultlist = client.Get(ref cfg);

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

        //POST: /PPM/TargetParameter/PagingQuery
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

                using (TargetParameterServiceClient client = new TargetParameterServiceClient())
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
                        MethodReturnResult<IList<TargetParameter>> result = client.Get(ref cfg);
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
        /// 新增日计划
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> Add(TargetParameterViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                model.ItemType = "0";

                using (TargetParameterServiceClient client = new TargetParameterServiceClient())
                {
                    TargetParameter obj = new TargetParameter()
                    {
                        Key = new TargetParameterKey()
                        {
                            Year = model.Year,
                            Month = model.Month,
                            Day = model.Day,
                            LocationName = model.LocationName,
                            ItemType = model.ItemType,
                            ItemCode = model.ItemCode
                        },
                        ValueData = model.ValueData,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        Creator = User.Identity.Name
                    };

                    MethodReturnResult rst = await client.AddAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(PPMResources.StringResource.TargetParameter_Add_Success,obj.Key);
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

        /// <summary>
        /// 删除计划
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="locationname"></param>
        /// <param name="ShiftName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string year, string month, string day, string locationname, string itemtype, string itemcode)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                using (TargetParameterServiceClient client = new TargetParameterServiceClient())
                {
                    TargetParameterKey key = new TargetParameterKey()
                    {
                        Year = year,
                        Month = month,
                        Day = day,
                        LocationName = locationname == null ? "" : locationname,
                        ItemType = itemtype,
                        ItemCode = itemcode
                    };

                    result = await client.DeleteAsync(key);

                    if (result.Code == 0)
                    {
                        result.Message = string.Format(PPMResources.StringResource.TargetParameter_Delete_Success, key);
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
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="locationname"></param>
        /// <param name="shiftname"></param>
        /// <returns></returns>
        public async Task<ActionResult> Modify(string year, string month, string day, string locationname, string itemtype, string itemcode)
        {
            MethodReturnResult<TargetParameter> result = new MethodReturnResult<TargetParameter>();

            try
            {
                TargetParameterViewModel viewModel = new TargetParameterViewModel();
                using (TargetParameterServiceClient client = new TargetParameterServiceClient())
                {
                    TargetParameterKey key = new TargetParameterKey()
                    {
                        Year = year,
                        Month = month,
                        Day = day,
                        LocationName = locationname == null ? "" : locationname,
                        ItemType = itemtype,
                        ItemCode = itemcode
                    };

                    result = await client.GetAsync(key);

                    if (result.Code == 0)
                    {
                        viewModel.Year = result.Data.Key.Year;
                        viewModel.Month = result.Data.Key.Month;
                        viewModel.Day = result.Data.Key.Day;
                        viewModel.LocationName = result.Data.Key.LocationName;
                        viewModel.ItemType = result.Data.Key.ItemType;
                        viewModel.ItemCode = result.Data.Key.ItemCode;
                        viewModel.ValueData = result.Data.ValueData;
                        viewModel.CreateTime = result.Data.CreateTime;
                        viewModel.Creator = result.Data.Creator;
                        viewModel.Editor = result.Data.Editor;
                        viewModel.EditTime = result.Data.EditTime;

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
        public async Task<ActionResult> SaveModify(TargetParameterViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                using (TargetParameterServiceClient client = new TargetParameterServiceClient())
                {
                    TargetParameter obj = new TargetParameter()
                    {
                        Key = new TargetParameterKey()
                        {
                            Year = model.Year,
                            Month = model.Month,
                            Day = model.Day,
                            LocationName = model.LocationName,
                            ItemType = model.ItemType,
                            ItemCode = model.ItemCode
                        },
                        ValueData = model.ValueData,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now,
                        CreateTime = model.CreateTime,
                        Creator = model.Creator
                    };

                    MethodReturnResult rst = await client.ModifyAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(PPMResources.StringResource.TargetParameter_Modify_Success, obj.Key);
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
        /// <returns></returns>
        public async Task<ActionResult> Detail(string year, string month, string day, string locationname, string itemtype, string itemcode)
        {
            MethodReturnResult<TargetParameter> result = new MethodReturnResult<TargetParameter>();

            try
            {
                TargetParameterViewModel viewModel = new TargetParameterViewModel();

                using (TargetParameterServiceClient client = new TargetParameterServiceClient())
                {
                    TargetParameterKey key = new TargetParameterKey()
                    {
                        Year = year,
                        Month = month,
                        Day = day,
                        LocationName = locationname == null ? "" : locationname,
                        ItemType = itemtype,
                        ItemCode = itemcode
                    };

                    //取得数据
                    result = await client.GetAsync(key);

                    if (result.Code == 0)
                    {                        
                        viewModel = new TargetParameterViewModel()
                        {
                            Year = result.Data.Key.Year,
                            Month = result.Data.Key.Month,
                            Day = result.Data.Key.Day,
                            LocationName = result.Data.Key.LocationName,
                            ItemType = result.Data.Key.ItemType,
                            ItemCode = result.Data.Key.ItemCode,
                            ValueData = result.Data.ValueData,
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
    }
}