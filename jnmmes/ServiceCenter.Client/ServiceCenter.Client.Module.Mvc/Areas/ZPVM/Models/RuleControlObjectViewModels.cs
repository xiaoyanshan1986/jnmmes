
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
    public class RuleControlObjectQueryViewModel
    {
        public RuleControlObjectQueryViewModel()
        {

        }

        [Display(Name = "RuleControlObjectQueryViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Code { get; set; }

    }

    public class RuleControlObjectViewModel
    {
        public RuleControlObjectViewModel()
        {
            this.IsUsed = true;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "RuleControlObjectViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "RuleControlObjectViewModel_Object", ResourceType = typeof(ZPVMResources.StringResource))]
        public EnumPVMTestDataType Object { get; set; }

        [Required]
        [Display(Name = "RuleControlObjectViewModel_Type", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Type { get; set; }

        [Required]
        [Display(Name = "RuleControlObjectViewModel_Value", ResourceType = typeof(ZPVMResources.StringResource))]
        public double Value { get; set; }

        /// <summary>
        /// 是否可用。
        /// </summary>
        [Display(Name = "RuleControlObjectViewModel_IsUsed", ResourceType = typeof(ZPVMResources.StringResource))]
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