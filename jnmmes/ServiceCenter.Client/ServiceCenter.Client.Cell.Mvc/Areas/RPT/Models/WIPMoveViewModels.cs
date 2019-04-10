
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

namespace ServiceCenter.Client.Mvc.Areas.RPT.Models
{
    /// <summary>
    /// 在制品Move数据查询视图模型类。
    /// </summary>
    public class WIPMoveQueryViewModel
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public WIPMoveQueryViewModel()
        {
            this.StartDate = DateTime.Now.Date;
            this.EndDate = DateTime.Now.Date;
        }
        /// <summary>
        /// 车间名称
        /// </summary>
        [Display(Name = "WIPMoveQueryViewModel_LocationName", ResourceType = typeof(RPTResources.StringResource))]
        public string LocationName { get; set; }
        /// <summary>
        /// 工单号
        /// </summary>
        [Display(Name = "WIPMoveQueryViewModel_OrderNumber", ResourceType = typeof(RPTResources.StringResource))]
        public string OrderNumber { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [Display(Name = "WIPMoveQueryViewModel_MaterialCode", ResourceType = typeof(RPTResources.StringResource))]
        public string MaterialCode { get; set; }
        /// <summary>
        /// 开始日期。
        /// </summary>
        [Required]
        [Display(Name = "WIPMoveQueryViewModel_StartDate", ResourceType = typeof(RPTResources.StringResource))]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期。
        /// </summary>
        [Required]
        [Display(Name = "WIPMoveQueryViewModel_EndDate", ResourceType = typeof(RPTResources.StringResource))]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 班别名称。
        /// </summary>
        [Display(Name = "WIPMoveQueryViewModel_ShiftName", ResourceType = typeof(RPTResources.StringResource))]
        public string ShiftName { get; set; }

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

        public IEnumerable<SelectListItem> GetShiftNames()
        {
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
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
    }

    public class WIPMoveDetailQueryViewModel : WIPMoveQueryViewModel
    {
        public WIPMoveDetailQueryViewModel()
        {
            this.PageNo = 0;
            this.PageSize = 20;
            this.TotalRecords = 0;
        }
        public string RouteOperationName { get; set; }
        public string Date { get; set; }
        public string Shift { get; set; }
        public string Activity { get; set; }

        public int PageSize { get; set; }

        public int PageNo { get; set; }

        public int TotalRecords { get; set; }
    }
}