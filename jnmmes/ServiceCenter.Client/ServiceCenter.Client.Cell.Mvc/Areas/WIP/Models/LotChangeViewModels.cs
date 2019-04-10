
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.WIP;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class LotChangeViewModel
    {
        public LotChangeViewModel()
        {
        }
        [Required]
        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string LotNumber { get; set; }
        [Required]
        [Display(Name = "LotChangeViewModel_NewOrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string NewOrderNumber { get; set; }

        [Required]
        [Display(Name = "LotChangeViewModel_NewMaterialCode", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
             , ErrorMessageResourceName = "ValidateString"
             , ErrorMessageResourceType = typeof(StringResource))]
        public string NewMaterialCode { get; set; }

        [Required]
        [Display(Name = "LotChangeViewModel_RouteEnterpriseName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteEnterpriseName { get; set; }
        [Required]
        [Display(Name = "LotChangeViewModel_RouteName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteName { get; set; }
        [Required]
        [Display(Name = "LotChangeViewModel_RouteStepName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteStepName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }



    }
}