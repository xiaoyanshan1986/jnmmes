
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
    public class MaterialTypeRouteQueryViewModel
    {
        public MaterialTypeRouteQueryViewModel()
        {

        }
        [Display(Name = "MaterialTypeRouteQueryViewModel_MaterialType", ResourceType = typeof(FMMResources.StringResource))]
        public string MaterialType { get; set; }

        [Display(Name = "MaterialTypeRouteQueryViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }
    }

    public class MaterialTypeRouteViewModel
    {
        public MaterialTypeRouteViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "MaterialTypeRouteViewModel_MaterialType", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialType { get; set; }

        [Required]
        [Display(Name = "MaterialTypeRouteViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }

        [Required]
        [Display(Name = "MaterialTypeRouteViewModel_RouteEnterpriseName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteEnterpriseName { get; set; }

        [Required]
        [Display(Name = "MaterialTypeRouteViewModel_IsRework", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsRework { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

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