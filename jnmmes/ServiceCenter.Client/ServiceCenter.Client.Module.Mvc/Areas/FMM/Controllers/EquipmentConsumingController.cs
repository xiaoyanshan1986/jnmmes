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
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Controllers
{
    public class EquipmentConsumingController : Controller
    {
        // GET: /FMM/EquipmentConsuming/
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

                //获取设备异常耗时数据。
                using ( EquipmentConsumingServiceClient client = new  EquipmentConsumingServiceClient())
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
                        MethodReturnResult<IList< EquipmentConsuming>> resultlist = client.Get(ref cfg);

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

                 EquipmentConsumingQueryViewModel model = new  EquipmentConsumingQueryViewModel
                {
                    //初始化参数
                    Year = sYear,                        //年
                    Month = sMonth,                      //月
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
        public async Task<ActionResult> Query(EquipmentConsumingQueryViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();            

            try
            {
                using ( EquipmentConsumingServiceClient client = new  EquipmentConsumingServiceClient())
                {
                    //取得数据
                    await Task.Run(() =>
                    {
                        //取数条件
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            //年度条件
                            if (!string.IsNullOrEmpty(model.Year))
                            {
                                where.AppendFormat("{0} Key.Year = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Year);
                            }

                            //月度条件
                            if (!string.IsNullOrEmpty(model.Month))
                            {
                                where.AppendFormat("{0} Key.Month = '{1}'"
                                                    , where.Length > 0 ? "and" : string.Empty
                                                    , model.Month);
                            }

                            //车间条件
                            if (!string.IsNullOrEmpty(model.LocationName))
                            {
                                where.AppendFormat("{0} Key.LocationName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.LocationName);
                            }

                            //班别条件
                            if (!string.IsNullOrEmpty(model.ShiftName))
                            {
                                where.AppendFormat("{0} Key.ShiftName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ShiftName);
                            }

                            //线别条件
                            if (!string.IsNullOrEmpty(model.LineCode))
                            {
                                where.AppendFormat("{0} Key.LineCode = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.LineCode);
                            }

                            //工序条件
                            if (!string.IsNullOrEmpty(model.RouteStepName))
                            {
                                where.AppendFormat("{0} Key.RouteStepName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteStepName);
                            }

                            //原因代码
                            if (!string.IsNullOrEmpty(model.ReasonCodeName))
                            {
                                where.AppendFormat("{0} Key.ReasonCodeName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ReasonCodeName);
                            }
                        }

                        //设置参数
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };

                        //取得数据
                        MethodReturnResult<IList< EquipmentConsuming>> resultlist = client.Get(ref cfg);

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

        //POST: /FMM/ EquipmentConsuming/PagingQuery
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

                using ( EquipmentConsumingServiceClient client = new  EquipmentConsumingServiceClient())
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
                        MethodReturnResult<IList< EquipmentConsuming>> result = client.Get(ref cfg);
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
        public async Task<ActionResult> Add( EquipmentConsumingViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                using ( EquipmentConsumingServiceClient client = new  EquipmentConsumingServiceClient())
                {
                     EquipmentConsuming obj = new  EquipmentConsuming()
                    {
                        Key = new  EquipmentConsumingKey()
                        {
                            Year = model.Year,
                            Month = model.Month,
                            Day = model.Day,
                            ShiftName = model.ShiftName,
                            LocationName = model.LocationName,
                            LineCode=model.LineCode,
                            RouteStepName=model.RouteStepName,
                            EquipmentCode=model.EquipmentCode,
                            ReasonCodeName=model.ReasonCodeName

                            
                        },
                      
                        Consuming=model.Consuming,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        Creator = User.Identity.Name
                    };

                    MethodReturnResult rst = await client.AddAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource. EquipmentConsuming_Add_Success,obj.Key);
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
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <param name="locationname">车间</param>
        /// <param name="shiftname">班别</param>
        /// <param name="linecode">线别</param>
        /// <param name="routestepname">工序</param>
        /// <param name="equipmentcode">设备代码</param>
        /// <param name="reasoncodename">原因代码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string year, string month, string day, string locationname, string shiftname, string linecode, string routestepname, string equipmentcode, string reasoncodename)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                using ( EquipmentConsumingServiceClient client = new  EquipmentConsumingServiceClient())
                {
                     EquipmentConsumingKey key = new  EquipmentConsumingKey()
                    {
                        Year = year,
                        Month = month,
                        Day = day,
                        ShiftName = shiftname,
                        LocationName = locationname == null ? "" : locationname,
                        LineCode=linecode,
                        //RouteStepName=routestepname,
                        EquipmentCode=equipmentcode,
                        ReasonCodeName = reasoncodename
                        
                    };

                    result = await client.DeleteAsync(key);

                    if (result.Code == 0)
                    {
                        result.Message = string.Format(FMMResources.StringResource. EquipmentConsuming_Delete_Success, key);
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
        /// <param name="linecode">线别</param>
        /// <param name="routestepname">工序</param>
        /// <param name="equipmentcode">设备代码</param>
        /// <param name="reasoncodename">原因代码</param>
        /// <returns></returns>
        public async Task<ActionResult> Modify(string year, string month, string day, string locationname, string shiftname, string linecode, string routestepname, string equipmentcode, string reasoncodename)
        {
            MethodReturnResult< EquipmentConsuming> result = new MethodReturnResult< EquipmentConsuming>();

            try
            {
                 EquipmentConsumingViewModel viewModel = new  EquipmentConsumingViewModel();
                using ( EquipmentConsumingServiceClient client = new  EquipmentConsumingServiceClient())
                {
                     EquipmentConsumingKey key = new  EquipmentConsumingKey()
                    {
                        Year = year,
                        Month = month,
                        Day = day,
                        ShiftName = shiftname,
                        LocationName = locationname == null ? "" : locationname,
                        LineCode = linecode,
                        RouteStepName = routestepname,
                        EquipmentCode = equipmentcode,
                        ReasonCodeName = reasoncodename
                    };

                    result = await client.GetAsync(key);

                    if (result.Code == 0)
                    {
                        viewModel.Year = result.Data.Key.Year;
                        viewModel.Month = result.Data.Key.Month;
                        viewModel.Day = result.Data.Key.Day;
                        viewModel.LocationName = result.Data.Key.LocationName;
                        viewModel.ShiftName = result.Data.Key.ShiftName;
                        viewModel.LineCode = result.Data.Key.LineCode;
                        viewModel.RouteStepName = result.Data.Key.RouteStepName;
                        viewModel.EquipmentCode = result.Data.Key.EquipmentCode;
                        viewModel.ReasonCodeName = result.Data.Key.ReasonCodeName; 
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
        public async Task<ActionResult> SaveModify( EquipmentConsumingViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                using ( EquipmentConsumingServiceClient client = new  EquipmentConsumingServiceClient())
                {
                     EquipmentConsuming obj = new  EquipmentConsuming()
                    {
                        Key = new  EquipmentConsumingKey()
                        {
                            Year = model.Year,
                            Month = model.Month,
                            Day = model.Day,
                            ShiftName = model.ShiftName,
                            LocationName = model.LocationName,
                            LineCode = model.LineCode,
                            RouteStepName = model.RouteStepName,
                            EquipmentCode = model.EquipmentCode,
                            ReasonCodeName = model.ReasonCodeName
                        },
                        Consuming = model.Consuming,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        Creator = User.Identity.Name
                    };

                    MethodReturnResult rst = await client.ModifyAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource. EquipmentConsuming_Modify_Success, obj.Key);
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
      /// <param name="linecode">线别</param>
      /// <param name="routestepname">工序</param>
      /// <param name="equipmentcode">设备代码</param>
      /// <param name="reasoncodename">原因代码</param>
      /// <returns></returns>
        public async Task<ActionResult> Detail(string year, string month, string day, string locationname, string shiftname, string linecode, string routestepname, string equipmentcode, string reasoncodename)
        {
            MethodReturnResult< EquipmentConsuming> result = new MethodReturnResult< EquipmentConsuming>();

            try
            {
                 EquipmentConsumingViewModel viewModel = new  EquipmentConsumingViewModel();

                using ( EquipmentConsumingServiceClient client = new  EquipmentConsumingServiceClient())
                {
                     EquipmentConsumingKey key = new  EquipmentConsumingKey()
                    {
                        Year = year,
                        Month = month,
                        Day = day,
                        ShiftName = shiftname,
                        LocationName = locationname == null ? "" : locationname,
                        LineCode = linecode,
                        RouteStepName = routestepname,
                        EquipmentCode = equipmentcode,
                        ReasonCodeName = reasoncodename
                    };

                    //取得数据
                    result = await client.GetAsync(key);

                    if (result.Code == 0)
                    {

                        viewModel = new  EquipmentConsumingViewModel()
                        {
                            Year = result.Data.Key.Year,
                            Month = result.Data.Key.Month,
                            Day = result.Data.Key.Day,
                            LocationName = result.Data.Key.LocationName,
                            ShiftName = result.Data.Key.ShiftName,
                            LineCode=result.Data.Key.LineCode,
                            EquipmentCode=result.Data.Key.EquipmentCode,
                            RouteStepName = result.Data.Key.RouteStepName,
                            ReasonCodeName = result.Data.Key.ReasonCodeName,
                            Consuming = result.Data.Consuming,
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
        /// 取得设备代码
        /// </summary>
        /// <param name="routestepname">工序</param>
        /// <param name="productionLineCode">线别</param>
        /// <returns></returns>
        public ActionResult GetEquipments(string routestepname, string linecode)
        {
            IList<Equipment> lstEquipments = new List<Equipment>();
            //根据生产线和工序获取设备。
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"LineCode='{0}' AND EXISTS(FROM RouteOperationEquipment as p 
                                                                      WHERE p.Key.EquipmentCode=self.Key 
                                                                      AND p.Key.RouteOperationName='{1}')"
                                            , linecode
                                            , routestepname)
                };
                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstEquipments = result.Data;
                }
            }

            var lnq = from item in lstEquipments
                      select new
                      {
                          Key = item.Key,
                          Text = item.Name,
                          Value=item.Name

                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 取得设备代码
        /// </summary>
        /// <param name="routestepname">工序</param>
        /// <param name="productionLineCode">线别</param>
        /// <returns></returns>
        public ActionResult GetEquipmentName(string EquipmentCode, string LineCode)
        {
            IList<Equipment> lstEquipments = new List<Equipment>();
            //根据生产线和工序获取设备。
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {

                  PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "LineCode",
                        Where = string.Format(@" Key = '{0}'and LineCode='{1}' ",
                                                    EquipmentCode,
                                                    LineCode)
                    };
                    MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);

                    if (result.Code <= 0 && result.Data != null)
                    {
                        lstEquipments = result.Data;
                    }
               
            }

            var lnq = from item in lstEquipments
                      select new
                      {
                          Value = item.Name,
                          Text = item.Name
                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
        }

    }
}