
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
    public class MaterialRouteQueryViewModel
    {
        public MaterialRouteQueryViewModel()
        {

        }

        /// <summary>
        /// 产品代码
        /// </summary>
        [Display(Name = "MaterialRouteViewModel_MaterialCode", ResourceType = typeof(FMMResources.StringResource))]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 车间代码
        /// </summary>
        [Display(Name = "MaterialRouteViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }
    }

    public class MaterialRouteViewModel
    {
        public MaterialRouteViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        /// <summary>
        /// 产品代码
        /// </summary>
        [Required]
        [Display(Name = "MaterialRouteViewModel_MaterialCode", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 车间名称
        /// </summary>
        [Required]
        [Display(Name = "MaterialRouteViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }

        /// <summary>
        /// 工艺流程组
        /// </summary>
        [Required]
        [Display(Name = "MaterialRouteViewModel_RouteEnterpriseName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteEnterpriseName { get; set; }

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

        /// <summary>
        /// 取得车间列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetLocationNameList()
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
        /// 取得工艺流程组列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRouteEnterpriseNameList()
        {
            using (RouteEnterpriseServiceClient client = new RouteEnterpriseServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'",Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<RouteEnterprise>> result = client.Get(ref cfg);
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
}