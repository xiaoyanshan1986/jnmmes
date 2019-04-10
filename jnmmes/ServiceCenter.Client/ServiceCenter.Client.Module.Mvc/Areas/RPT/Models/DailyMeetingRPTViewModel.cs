using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using RPTResources = ServiceCenter.Client.Mvc.Resources.RPT;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.RPT;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Models
{
    /// <summary>
    /// 组件日运营查询视图模型类。
    /// Author:		    方军
    /// Create date:    2016-01-12 13:43:52.250
    /// 
    /// </summary>
    public class DailyMeetingRPTViewModel
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public DailyMeetingRPTViewModel()
        {
            PageNo = 0;
            PageSize = 20;
        }

        /// <summary>
        /// 报表代码
        /// </summary>
        [Display(Name = "DailyMeetingRPTViewModel_ReportCode", ResourceType = typeof(RPTResources.StringResource))]
        public string ReportCode { get; set; }

        /// <summary>
        /// 报表名称
        /// </summary>
        [Display(Name = "DailyMeetingRPTViewModel_ReportName", ResourceType = typeof(RPTResources.StringResource))]
        public string ReportName { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [Display(Name = "DailyMeetingRPTViewModel_StartDate", ResourceType = typeof(RPTResources.StringResource))]
        public string StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [Display(Name = "DailyMeetingRPTViewModel_EndDate", ResourceType = typeof(RPTResources.StringResource))]
        public string EndDate { get; set; }

        /// <summary>
        /// 车间代码
        /// </summary>
        [Display(Name = "DailyMeetingRPTViewModel_LocationName", ResourceType = typeof(RPTResources.StringResource))]
        public string LocationName { get; set; }

        /// <summary>
        /// 线别代码
        /// </summary>
        [Display(Name = "DailyMeetingRPTViewModel_LineCode", ResourceType = typeof(RPTResources.StringResource))]
        public string LineCode { get; set; }

        /// <summary>
        /// 工单
        /// </summary>
        [Display(Name = "DailyMeetingRPTViewModel_OrderNumber", ResourceType = typeof(RPTResources.StringResource))]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        [Display(Name = "DailyMeetingRPTViewModel_ProductID", ResourceType = typeof(RPTResources.StringResource))]
        public string ProductID { get; set; }

        /// <summary>
        /// 班别
        /// </summary>
        [Display(Name = "DailyMeetingRPTViewModel_SchedulingCode", ResourceType = typeof(RPTResources.StringResource))]
        public string SchedulingCode { get; set; }

        /// <summary>
        /// 月数据显示数量（当前月向前）
        /// </summary>
        [Display(Name = "RPTDailyDataViewModel_MonthDataNumber", ResourceType = typeof(RPTResources.StringResource))]
        public int MonthDataNumber { get; set; }

        /// <summary>
        /// 年数据显示数量（当前月向前）
        /// </summary>
        [Display(Name = "RPTDailyDataViewModel_YearDataNumber", ResourceType = typeof(RPTResources.StringResource))]
        public int YearDataNumber { get; set; }

        /// <summary>
        /// 数据单页面大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int Records { get; set; }

        /// <summary>
        /// 取得车间代码
        /// </summary>
        public IEnumerable<SelectListItem> GetLocations()
        {
            using (LocationServiceClient client = new LocationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Level='{0}'", Convert.ToInt32(LocationLevel.Room))
                };

                MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }

        /// <summary>
        /// 取得线别代码
        /// </summary>
        public IEnumerable<SelectListItem> GetProductionLineList()
        {
            IList<ProductionLine> lst = new List<ProductionLine>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false
            };
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }
            return from item in lst
                   select new SelectListItem()
                   {
                       Text = item.LocationName,
                       Value = item.Key
                   };
        }

        /// <summary>
        /// 取得排班列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetSchedulingList()
        {
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = ""
                };

                MethodReturnResult<IList<Shift>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }

        /// <summary>
        /// 取得产品代码
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetProductName()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName = 'RPTPerCapitaEfficiencyRatio' AND Key.AttributeName = 'ProcuctCode'"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data != null)
                {
                    lstValues = result.Data;
                }
            }

            return from item in lstValues
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Value
                   };
        }
    }
}