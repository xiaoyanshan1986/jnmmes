
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

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class ShiftQueryViewModel
    {
        public ShiftQueryViewModel()
        {

        }
        [Display(Name = "ShiftQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

    }

    public class ShiftViewModel
    {
        public ShiftViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;

        }

        [Required]
        [Display(Name = "ShiftViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "ShiftViewModel_StartTime", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(5, MinimumLength = 5
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("(0\\d{1}|1\\d{1}|2[0-3]):([0-5]\\d{1})"
                          , ErrorMessageResourceName = "ValidateTimeHHMM"
                          , ErrorMessageResourceType = typeof(StringResource))]
        public string StartTime { get; set; }


        [Required]
        [Display(Name = "ShiftViewModel_EndTime", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(5, MinimumLength = 5
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("(0\\d{1}|1\\d{1}|2[0-3]):([0-5]\\d{1})"
                        , ErrorMessageResourceName = "ValidateTimeHHMM"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string EndTime { get; set; }

        [Display(Name = "ShiftViewModel_IsOverDay", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsOverDay { get; set; }


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