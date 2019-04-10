
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.ZPVM;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.ZPVM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{
    public class WorkOrderDecayQueryViewModel
    {
        public WorkOrderDecayQueryViewModel()
        {

        }

        [Display(Name = "WorkOrderRuleQueryViewModel_OrderNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "WorkOrderRuleQueryViewModel_MaterialCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string MaterialCode { get; set; }

    }

    public class WorkOrderDecayViewModel
    {
        public WorkOrderDecayViewModel()
        {
            this.IsUsed = true;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "WorkOrderRuleViewModel_OrderNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "WorkOrderRuleViewModel_MaterialCode", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        [Required]
        [Display(Name = "RuleDecayViewModel_MinPower", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? MinPower { get; set; }

        [Required]
        [Display(Name = "RuleDecayViewModel_MaxPower", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? MaxPower { get; set; }

        [Required]
        [Display(Name = "RuleDecayViewModel_DecayCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string DecayCode { get; set; }

        /// <summary>
        /// 是否可用。
        /// </summary>
        [Display(Name = "RuleDecayViewModel_IsUsed", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool IsUsed { get; set; }


        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }


    }
}