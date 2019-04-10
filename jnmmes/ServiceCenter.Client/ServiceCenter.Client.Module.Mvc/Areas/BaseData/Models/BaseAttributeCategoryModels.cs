
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.Client.Mvc.Resources.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;

namespace ServiceCenter.Client.Mvc.Areas.BaseData.Models
{
    public class BaseAttributeCategoryQueryViewModel
    {
        public BaseAttributeCategoryQueryViewModel()
        {
        }

        [Display(Name = "BaseAttributeCategoryQueryViewModel_CategoryName", ResourceType = typeof(StringResource))]
        public string CategoryName { get; set; }

    }

    public class BaseAttributeCategoryViewModel
    {
        public BaseAttributeCategoryViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            
        }

        [Required]
        [Display(Name = "BaseAttributeCategoryViewModel_Name", ResourceType = typeof(StringResource))]
        [Editable(false)]
        [StringLength(50, MinimumLength = 3
                        , ErrorMessageResourceName = "BaseAttributeCategoryViewModel_ValidateStringLength"
                        , ErrorMessageResourceType=typeof(StringResource))]
        public string Name { get; set; }


        [Display(Name = "BaseAttributeCategoryViewModel_Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "BaseAttributeCategoryViewModel_MaxValidateStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }


        [Display(Name = "BaseAttributeCategoryViewModel_Editor", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public string Editor { get; set; }


        [Display(Name = "BaseAttributeCategoryViewModel_EditTime", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public DateTime? EditTime { get; set; }


        [Display(Name = "BaseAttributeCategoryViewModel_Creator", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public string Creator { get; set; }


        [Display(Name = "BaseAttributeCategoryViewModel_CreateTime", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public DateTime? CreateTime { get; set; }
    }
}