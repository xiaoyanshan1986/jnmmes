
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
    public class DecayQueryViewModel
    {
        public DecayQueryViewModel()
        {

        }

        [Display(Name = "DecayQueryViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Code { get; set; }

    }

    public class DecayViewModel
    {
        public DecayViewModel()
        {
            this.IsUsed = true;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "DecayViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "DecayViewModel_Object", ResourceType = typeof(ZPVMResources.StringResource))]
        public EnumPVMTestDataType Object { get; set; }


        [Required]
        [Display(Name = "DecayViewModel_Value", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? Value { get; set; }


        [Required]
        [Display(Name = "DecayViewModel_Type", ResourceType = typeof(ZPVMResources.StringResource))]
        public EnumDecayType Type { get; set; }


        /// <summary>
        /// 是否可用。
        /// </summary>
        [Display(Name = "DecayViewModel_IsUsed", ResourceType = typeof(ZPVMResources.StringResource))]
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

        public IEnumerable<SelectListItem> GetDecayObjectList()
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

        public IEnumerable<SelectListItem> GetDecayTypeList()
        {
            IDictionary<EnumDecayType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumDecayType>();

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