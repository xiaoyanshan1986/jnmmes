
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
    /// 在制品数据分布查询视图模型类。
    /// </summary>
    public class WIPDisplayQueryViewModel
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public WIPDisplayQueryViewModel()
        {
            this.OnlineTime = 0;
        }
        /// <summary>
        /// 车间名称
        /// </summary>
        [Display(Name = "WIPDisplayQueryViewModel_LocationName", ResourceType = typeof(RPTResources.StringResource))]
        public string LocationName { get; set; }
        /// <summary>
        /// 工单号
        /// </summary>
        [Display(Name = "WIPDisplayQueryViewModel_OrderNumber", ResourceType = typeof(RPTResources.StringResource))]
        public string OrderNumber { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [Display(Name = "WIPDisplayQueryViewModel_MaterialCode", ResourceType = typeof(RPTResources.StringResource))]
        public string MaterialCode { get; set; }
        /// <summary>
        /// 在线时长（分钟）
        /// </summary>
        [Required]
        [Display(Name = "WIPDisplayQueryViewModel_OnlineTime", ResourceType = typeof(RPTResources.StringResource))]
        [Range(0, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        public double OnlineTime { get; set; }


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
    }


    public class WIPDisplayDetailQueryViewModel:WIPDisplayQueryViewModel
    {
        public string RouteOperationName { get; set; }
        public string Status { get; set; }
    }
}