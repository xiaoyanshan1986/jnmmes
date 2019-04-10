
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
    public class WorkOrderRuleQueryViewModel
    {
        public WorkOrderRuleQueryViewModel()
        {

        }

        [Display(Name = "WorkOrderRuleQueryViewModel_OrderNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "WorkOrderRuleQueryViewModel_MaterialCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string MaterialCode { get; set; }
    }

    public class WorkOrderRuleViewModel
    {
        public WorkOrderRuleViewModel()
        {
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
        [Display(Name = "WorkOrderRuleViewModel_RuleCode", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RuleCode { get; set; }

        [Required]
        [Display(Name = "WorkOrderRuleViewModel_RuleName", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RuleName { get; set; }

        [Required]
        [Display(Name = "RuleViewModel_FullPackageQty", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? FullPackageQty { get; set; }

        [Required]
        [Display(Name = "WorkOrderRuleViewModel_PowerDegree", ResourceType = typeof(ZPVMResources.StringResource))]
        public int PowerDegree { get; set; }

        [Required]
        [Display(Name = "WorkOrderRuleViewModel_MinPower", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? MinPower { get; set; }

        [Required]
        [Display(Name = "WorkOrderRuleViewModel_MaxPower", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? MaxPower { get; set; }

        [Display(Name = "WorkOrderRuleViewModel_CalibrationType", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                       , ErrorMessageResourceName = "ValidateStringLength"
                       , ErrorMessageResourceType = typeof(StringResource))]
        public string CalibrationType { get; set; }

        [Display(Name = "WorkOrderRuleViewModel_CalibrationCycle", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? CalibrationCycle { get; set; }

        [Display(Name = "WorkOrderRuleViewModel_FixCycle", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? FixCycle { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                    , ErrorMessageResourceName = "ValidateMaxStringLength"
                    , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }


        public WorkOrderRule GetWorkOrderRule(string orderNumber, string materialCode)
        {
            using (WorkOrderRuleServiceClient client = new WorkOrderRuleServiceClient())
            {
                MethodReturnResult<WorkOrderRule> result = client.Get(new WorkOrderRuleKey()
                {
                     MaterialCode=materialCode,
                     OrderNumber=orderNumber
                });
                if (result.Code <= 0)
                {
                    return result.Data;
                }
            }
            return null;
        }
    }
}