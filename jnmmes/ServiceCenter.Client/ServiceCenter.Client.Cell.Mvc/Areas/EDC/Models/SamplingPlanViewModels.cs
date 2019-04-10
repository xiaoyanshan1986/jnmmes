
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.EDC;
using EDCResources = ServiceCenter.Client.Mvc.Resources.EDC;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.EDC;
using System.Web.Mvc;
using ServiceCenter.Common;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Models
{
    public class SamplingPlanQueryViewModel
    {
        public SamplingPlanQueryViewModel()
        {

        }
        [Display(Name = "SamplingPlanQueryViewModel_Name", ResourceType = typeof(EDCResources.StringResource))]
        public string Name { get; set; }

    }

    public class SamplingPlanViewModel
    {
        public SamplingPlanViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.Status = EnumObjectStatus.Available;
            this.Type = EnumSamplingPlanType.Normal;
            this.Mode = EnumSamplingPlanMode.Interval;
        }

        [Required]
        [Display(Name = "SamplingPlanViewModel_Name", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "SamplingPlanViewModel_Type", ResourceType = typeof(EDCResources.StringResource))]
        public EnumSamplingPlanType Type { get; set; }

        [Required]
        [Display(Name = "SamplingPlanViewModel_Mode", ResourceType = typeof(EDCResources.StringResource))]
        public EnumSamplingPlanMode Mode { get; set; }

        [Required]
        [Display(Name = "SamplingPlanViewModel_Size", ResourceType = typeof(EDCResources.StringResource))]
        public double? Size { get; set; }

        [Display(Name = "Status", ResourceType = typeof(StringResource))]
        public EnumObjectStatus Status { get; set; }

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


        public IEnumerable<SelectListItem> GetObjectStatusList()
        {
            IDictionary<EnumObjectStatus, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumObjectStatus>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetSamplingPlanModeList()
        {
            IDictionary<EnumSamplingPlanMode, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumSamplingPlanMode>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

    }
}