
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
    public class EquipmentStateQueryViewModel
    {
        public EquipmentStateQueryViewModel()
        {

        }

        [Display(Name = "EquipmentStateQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }
        [Display(Name = "EquipmentStateQueryViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public EnumEquipmentStateType? Type { get; set; }
        [Display(Name = "EquipmentStateQueryViewModel_Category", ResourceType = typeof(FMMResources.StringResource))]
        public EnumEquipmentStateCategory? Category { get; set; }

        public IEnumerable<SelectListItem> GetEquipmentStateTypeList()
        {
            IDictionary<EnumEquipmentStateType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumEquipmentStateType>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetEquipmentStateCategoryList()
        {
            IDictionary<EnumEquipmentStateCategory, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumEquipmentStateCategory>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }
    }

    public class EquipmentStateViewModel
    {
        public EquipmentStateViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.Type = EnumEquipmentStateType.Run;
            this.Category = EnumEquipmentStateCategory.UpTime;
        }

        [Required]
        [Display(Name = "EquipmentStateViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(20, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "EquipmentStateViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public EnumEquipmentStateType Type { get; set; }

        [Required]
        [Display(Name = "EquipmentStateViewModel_Category", ResourceType = typeof(FMMResources.StringResource))]
        public EnumEquipmentStateCategory Category { get; set; }

        [Display(Name = "EquipmentStateViewModel_StateColor", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(10, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string StateColor { get; set; }

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

        public IEnumerable<SelectListItem> GetEquipmentStateTypeList()
        {
            IDictionary<EnumEquipmentStateType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumEquipmentStateType>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetEquipmentStateCategoryList()
        {
            IDictionary<EnumEquipmentStateCategory, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumEquipmentStateCategory>();

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