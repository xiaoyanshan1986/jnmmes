
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
    public class RuleQueryViewModel
    {
        public RuleQueryViewModel()
        {

        }

        [Display(Name = "RuleQueryViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Code { get; set; }

        [Display(Name = "RuleQueryViewModel_Name", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Name { get; set; }

    }

    public class RuleViewModel
    {
        public RuleViewModel()
        {
            this.IsUsed = true;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "RuleViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "RuleViewModel_Name", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "RuleViewModel_FullPackageQty", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? FullPackageQty { get; set; }

        [Required]
        [Display(Name = "RuleViewModel_PowersetCode", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string PowersetCode { get; set; }

        [Required]
        [Display(Name = "RuleViewModel_PowerDegree", ResourceType = typeof(ZPVMResources.StringResource))]
        public int PowerDegree { get; set; }

        [Required]
        [Display(Name = "RuleViewModel_MinPower", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? MinPower { get; set; }

        [Required]
        [Display(Name = "RuleViewModel_MaxPower", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? MaxPower { get; set; }

        [Display(Name = "RuleViewModel_CalibrationType", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                       , ErrorMessageResourceName = "ValidateStringLength"
                       , ErrorMessageResourceType = typeof(StringResource))]
        public string CalibrationType { get; set; }

        [Display(Name = "RuleViewModel_CalibrationCycle", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? CalibrationCycle { get; set; }

        [Display(Name = "RuleViewModel_FixCycle", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? FixCycle { get; set; }
        /// <summary>
        /// 是否可用。
        /// </summary>
        [Display(Name = "RuleViewModel_IsUsed", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool IsUsed { get; set; }

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

    }
}