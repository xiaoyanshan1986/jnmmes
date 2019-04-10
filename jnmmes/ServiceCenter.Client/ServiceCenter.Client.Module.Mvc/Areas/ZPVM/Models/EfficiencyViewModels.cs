
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

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{
    public class EfficiencyQueryViewModel
    {
        public EfficiencyQueryViewModel()
        {

        }
        [Display(Name = "EfficiencyQueryViewModel_Group", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Group { get; set; }
        [Display(Name = "EfficiencyQueryViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Code { get; set; }

    }

    public class EfficiencyViewModel
    {
        public EfficiencyViewModel()
        {
            this.IsUsed = true;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "EfficiencyViewModel_Group", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Group { get; set; }

        [Required]
        [Display(Name = "EfficiencyViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "EfficiencyViewModel_Name", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                    , ErrorMessageResourceName = "ValidateStringLength"
                    , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }


        [Required]
        [Display(Name = "EfficiencyViewModel_Lower", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? Lower { get; set; }


        [Display(Name = "EfficiencyViewModel_Upper", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? Upper { get; set; }


        /// <summary>
        /// 是否可用。
        /// </summary>
        [Display(Name = "EfficiencyViewModel_IsUsed", ResourceType = typeof(ZPVMResources.StringResource))]
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