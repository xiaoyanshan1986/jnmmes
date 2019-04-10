
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.PPM;
using PPMResources = ServiceCenter.Client.Mvc.Resources.PPM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.PPM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Models
{
    public class WorkOrderRouteQueryViewModel
    {
        public WorkOrderRouteQueryViewModel()
        {

        }
        [Display(Name = "WorkOrderRouteQueryViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderNumber { get; set; }
    }

    public class WorkOrderRouteViewModel
    {
        public WorkOrderRouteViewModel()
        {
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "WorkOrderRouteViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "WorkOrderRouteViewModel_ItemNo", ResourceType = typeof(PPMResources.StringResource))]
        [Range(1, 65536
          , ErrorMessageResourceName = "ValidateRange"
          , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int ItemNo { get; set; }

        [Required]
        [Display(Name = "WorkOrderRouteViewModel_RouteEnterpriseName", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteEnterpriseName { get; set; }

        [Required]
        [Display(Name = "WorkOrderRouteViewModel_RouteName", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteName { get; set; }
        [Required]
        [Display(Name = "WorkOrderRouteViewModel_RouteStepName", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteStepName { get; set; }

        [Required]
        [Display(Name = "WorkOrderRouteViewModel_IsRework", ResourceType = typeof(PPMResources.StringResource))]
        public bool IsRework { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        public IEnumerable<SelectListItem> GetRouteEnterpriseNameList()
        {
            using (RouteEnterpriseServiceClient client = new RouteEnterpriseServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where=string.Format("Status='{0}'",Convert.ToInt32(EnumObjectStatus.Available))
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

        public IEnumerable<SelectListItem> GetRouteNameList()
        {
            return new List<SelectListItem>();
        }

        public IEnumerable<SelectListItem> GetRouteStepNameList()
        {
            return new List<SelectListItem>();
        }
    }
}