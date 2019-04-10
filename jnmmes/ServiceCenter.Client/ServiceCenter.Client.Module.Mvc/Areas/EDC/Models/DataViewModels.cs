
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
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.RBAC;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class DataViewModel
    {
        public DataViewModel()
        {
        }

        [Required]
        [Display(Name = "DataViewModel_LocationName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string LocationName { get; set; }

        [Required]
        [Display(Name = "DataViewModel_ProductionLineCode", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string ProductionLineCode { get; set; }

        [Required]
        [Display(Name = "DataViewModel_RouteOperationName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteOperationName { get; set; }

        [Required]
        [Display(Name = "DataViewModel_EquipmentCode", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string EquipmentCode { get; set; }

        [Required]
        [Display(Name = "DataViewModel_OrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "DataViewModel_MaterialCode", ResourceType = typeof(WIPResources.StringResource))]
        public string MaterialCode { get; set; }

    }


}