
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
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class RouteEnterpriseDetailQueryViewModel
    {
        public RouteEnterpriseDetailQueryViewModel()
        {

        }

        /// <summary>
        /// 工艺流程组
        /// </summary>
        [Display(Name = "RouteEnterpriseDetailQueryViewModel_RouteEnterpriseName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteEnterpriseName { get; set; }

        /// <summary>
        /// 工艺流程
        /// </summary>
        [Display(Name = "RouteEnterpriseDetailQueryViewModel_RouteName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteName { get; set; }
    }

    public class RouteEnterpriseDetailViewModel
    {
        public RouteEnterpriseDetailViewModel()
        {
            this.EditTime = DateTime.Now;
            this.ItemNo = 1;
        }

        /// <summary>
        /// 工艺流程组
        /// </summary>
        [Required]
        [Display(Name = "RouteEnterpriseDetailViewModel_RouteEnterpriseName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteEnterpriseName { get; set; }

        /// <summary>
        /// 工艺流程
        /// </summary>
        [Required]
        [Display(Name = "RouteEnterpriseDetailViewModel_RouteName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteName { get; set; }

        /// <summary>
        /// 项目号
        /// </summary>
        [Required]
        [Display(Name = "RouteEnterpriseDetailViewModel_ItemNo", ResourceType = typeof(FMMResources.StringResource))]
        [Range(0, 65536
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int ItemNo { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        /// <summary>
        /// 编辑日期
        /// </summary>
        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        public IEnumerable<SelectListItem> GetRouteNameList()
        {
            using (RouteServiceClient client = new RouteServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<Route>> result = client.Get(ref cfg);
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
        /// 根据工艺流程代码取得工艺流程类型
        /// </summary>
        /// <param name="RouteName"></param>
        /// <returns></returns>
        public string GetRouteTypeName(string routeName)
        {
            string routeTypeName = "";

            using (RouteServiceClient client = new RouteServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key = '{0}'",
                                            routeName)
                };

                MethodReturnResult<IList<Route>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data.Count > 0)
                {
                    routeTypeName = result.Data[0].RouteType.GetDisplayName();
                }
            }

            return routeTypeName;
        }
    }
}