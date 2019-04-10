
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
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.ZPVM;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class EquipmentControlObjectQueryViewModel
    {
        public EquipmentControlObjectQueryViewModel()
        {

        }

        [Display(Name = "RuleControlObjectQueryViewModel_Code", ResourceType = typeof(FMMResources.StringResource))]
        public string Code { get; set; }

    }

    public class EquipmentControlObjectViewModel
    {
        public EquipmentControlObjectViewModel()
        {
            this.IsUsed = true;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "EquipmentControlObjectViewModel_Code", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "EquipmentControlObjectViewModel_Object", ResourceType = typeof(FMMResources.StringResource))]
        public EnumPVMTestDataType Object { get; set; }

        [Required]
        [Display(Name = "EquipmentControlObjectViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Type { get; set; }

        [Required]
        [Display(Name = "EquipmentControlObjectViewModel_Value", ResourceType = typeof(FMMResources.StringResource))]
        public double Value { get; set; }

        /// <summary>
        /// 是否可用。
        /// </summary>
        [Display(Name = "EquipmentControlObjectViewModel_IsUsed", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsUsed { get; set; }


        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }


        public IEnumerable<SelectListItem> GetControlObjectList()
        {
            IDictionary<EnumPVMTestDataType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumPVMTestDataType>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetControlTypeList()
        {
            string[] types = new string[]{ 
                ">",
                ">=",
                "=",
                "<",
                "<=",
                "<>"
            };
            IEnumerable<SelectListItem> lst = from item in types
                                              select new SelectListItem()
                                              {
                                                  Text = item,
                                                  Value = item
                                              };
            return lst;
        }
    }
}