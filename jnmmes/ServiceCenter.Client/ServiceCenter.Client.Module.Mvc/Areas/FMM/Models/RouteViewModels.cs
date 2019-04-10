
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.FMM;
using FMMResources = ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.FMM;
using System.Web.Mvc;
using ServiceCenter.Common;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class RouteQueryViewModel
    {
        public RouteQueryViewModel()
        {

        }

        [Display(Name = "RouteQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }
    }

    public class RouteViewModel
    {
        /// <summary>
        /// 初始化业务流程
        /// </summary>
        public RouteViewModel()
        {            
            this.Status = EnumObjectStatus.Available;
            this.RouteType = EnumRouteType.MainFlow;
        }

        /// <summary>
        /// 工艺流程名称
        /// </summary>
        [Required]
        [Display(Name = "RouteViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }

        /// <summary>
        /// 工艺流程状态
        /// </summary>
        [Display(Name = "Status", ResourceType = typeof(StringResource))]
        public EnumObjectStatus Status { get; set; }

        /// <summary>
        /// 工艺流程类型
        /// </summary>
        [Display(Name = "Type", ResourceType = typeof(StringResource))]
        public EnumRouteType RouteType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 被复制的工艺流程名称
        /// </summary>       
        public string ParentName { get; set; }

        /// <summary>
        /// 取得状态列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetObjectStatusList()
        {
            IDictionary<EnumObjectStatus, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumObjectStatus>();

            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = Convert.ToString(item.Key)
                   }; ;
        }

        /// <summary>
        /// 取得工艺流程类型列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRouteTypeList()
        {
            IDictionary<EnumRouteType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumRouteType>();

            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = Convert.ToString(item.Key)
                   }; ;
        }
    }
}