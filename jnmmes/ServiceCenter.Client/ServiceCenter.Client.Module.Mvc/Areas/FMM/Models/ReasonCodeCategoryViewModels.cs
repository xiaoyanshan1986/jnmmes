
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
    public class ReasonCodeCategoryQueryViewModel
    {
        public ReasonCodeCategoryQueryViewModel()
        {

        }
        [Display(Name = "ReasonCodeCategoryQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

        [Display(Name = "ReasonCodeCategoryQueryViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public EnumReasonCodeType? Type { get; set; }

        public IEnumerable<SelectListItem> GetReasonCodeTypeList()
        {
            IDictionary<EnumReasonCodeType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumReasonCodeType>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }
    }

    public class ReasonCodeCategoryViewModel
    {
        public ReasonCodeCategoryViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;

        }

        [Required]
        [Display(Name = "ReasonCodeCategoryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }
        /// <summary>
        /// 类别。
        /// </summary>
        [Display(Name = "ReasonCodeCategoryViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public virtual EnumReasonCodeType Type { get; set; }
        
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


        public IEnumerable<SelectListItem> GetReasonCodeTypeList()
        {
            IDictionary<EnumReasonCodeType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumReasonCodeType>();

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