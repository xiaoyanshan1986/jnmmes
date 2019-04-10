
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
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.QAM.Models
{
    public class CheckSettingPointDetailQueryViewModel
    {
        public CheckSettingPointDetailQueryViewModel()
        {

        }
        [Display(Name = "CheckSettingPointDetailQueryViewModel_CheckSettingKey", ResourceType = typeof(QAMResources.StringResource))]
        public string CheckSettingKey { get; set; }

        [Display(Name = "CheckSettingPointDetailQueryViewModel_GroupName", ResourceType = typeof(QAMResources.StringResource))]
        public string GroupName { get; set; }

        [Display(Name = "CheckSettingPointDetailQueryViewModel_ItemNo", ResourceType = typeof(QAMResources.StringResource))]
        public int ItemNo { get; set; }

        [Display(Name = "CheckSettingPointDetailQueryViewModel_ParameterName", ResourceType = typeof(QAMResources.StringResource))]
        public string ParameterName { get; set; }
    }

    public class CheckSettingPointDetailViewModel
    {
        public CheckSettingPointDetailViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        public string CheckSettingKey { get; set; }
        [Required]
        public int ItemNo { get; set; }

        [Required]
        [Display(Name = "CheckSettingPointDetailViewModel_ParameterName", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ParameterName { get; set; }

        [Required]
        [Display(Name = "CheckSettingPointDetailViewModel_ParameterItemNo", ResourceType = typeof(QAMResources.StringResource))]
        public int ParameterItemNo { get; set; }
        [Required]
        [Display(Name = "CheckSettingPointDetailViewModel_ParameterCount", ResourceType = typeof(QAMResources.StringResource))]
        [RegularExpression("[0-9]+"
                            , ErrorMessageResourceName = "ValidateInt"
                            , ErrorMessageResourceType = typeof(StringResource))]
        public int ParameterCount { get; set; }
        [Required]
        [Display(Name = "CheckSettingPointDetailViewModel_ParameterType", ResourceType = typeof(QAMResources.StringResource))]
        public EnumParameterType ParameterType { get; set; }

        [Display(Name = "CheckSettingPointDetailViewModel_DataType", ResourceType = typeof(QAMResources.StringResource))]
        public EnumDataType DataType { get; set; }

        [Display(Name = "CheckSettingPointDetailViewModel_DeviceType", ResourceType = typeof(QAMResources.StringResource))]
        public EnumDeviceType DeviceType { get; set; }


        [Display(Name = "CheckSettingPointDetailViewModel_Mandatory", ResourceType = typeof(QAMResources.StringResource))]
        public bool Mandatory { get; set; }


        [Display(Name = "CheckSettingPointDetailViewModel_IsDerived", ResourceType = typeof(QAMResources.StringResource))]
        public bool IsDerived { get; set; }

        [Display(Name = "CheckSettingPointDetailViewModel_DerivedFormula", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(255, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string DerivedFormula { get; set; }


        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }


        public IEnumerable<SelectListItem> GetDataTypeList()
        {
            IDictionary<EnumDataType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumDataType>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetDeviceTypeList()
        {
            IDictionary<EnumDeviceType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumDeviceType>();

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