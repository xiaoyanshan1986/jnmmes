
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.QAM;
using QAMResources = ServiceCenter.Client.Mvc.Resources.QAM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.QAM;
using System.Web.Mvc;
using ServiceCenter.Common;

namespace ServiceCenter.Client.Mvc.Areas.QAM.Models
{
    public class CheckPlanQueryViewModel
    {
        public CheckPlanQueryViewModel()
        {

        }
        [Display(Name = "CheckPlanQueryViewModel_Name", ResourceType = typeof(QAMResources.StringResource))]
        public string Name { get; set; }

    }

    public class CheckPlanViewModel
    {
        public CheckPlanViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.Status = EnumObjectStatus.Available;
            this.Type = EnumCheckType.Normal;
            this.Mode = EnumCheckMode.Interval;
        }

        [Required]
        [Display(Name = "CheckPlanViewModel_Name", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "CheckPlanViewModel_Type", ResourceType = typeof(QAMResources.StringResource))]
        public EnumCheckType Type { get; set; }

        [Required]
        [Display(Name = "CheckPlanViewModel_Mode", ResourceType = typeof(QAMResources.StringResource))]
        public EnumCheckMode Mode { get; set; }

        [Required]
        [Display(Name = "CheckPlanViewModel_Size", ResourceType = typeof(QAMResources.StringResource))]
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

        public IEnumerable<SelectListItem> GetCheckModeList()
        {
            IDictionary<EnumCheckMode, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumCheckMode>();

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