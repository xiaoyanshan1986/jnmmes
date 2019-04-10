
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
    public class ParameterQueryViewModel
    {
        public ParameterQueryViewModel()
        {

        }
        [Display(Name = "ParameterQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

        [Display(Name = "ParameterQueryViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public EnumParameterType? Type { get; set; }

        public IEnumerable<SelectListItem> GetParameterTypeList()
        {
            IDictionary<EnumParameterType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumParameterType>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }
    }

    public class ParameterViewModel
    {
        public ParameterViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;

        }

        [Required]
        [Display(Name = "ParameterViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }
        /// <summary>
        /// 类别。
        /// </summary>
        [Display(Name = "ParameterViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public virtual EnumParameterType Type { get; set; }
        /// <summary>
        /// 数据类型。
        /// </summary>
        [Display(Name = "ParameterViewModel_DataType", ResourceType = typeof(FMMResources.StringResource))]
        public virtual EnumDataType DataType { get; set; }
        /// <summary>
        /// 设备类型。
        /// </summary>
        [Display(Name = "ParameterViewModel_DeviceType", ResourceType = typeof(FMMResources.StringResource))]
        public virtual EnumDeviceType DeviceType { get; set; }
        /// <summary>
        /// 必须输入
        /// </summary>
        [Display(Name = "ParameterViewModel_Mandatory", ResourceType = typeof(FMMResources.StringResource))]
        public virtual bool Mandatory { get; set; }
        /// <summary>
        /// 是否是衍生推导参数。
        /// </summary>
        [Display(Name = "ParameterViewModel_IsDerived", ResourceType = typeof(FMMResources.StringResource))]
        public virtual bool IsDerived { get; set; }
        /// <summary>
        /// 衍生推导公式。
        /// </summary>
        [Display(Name = "ParameterViewModel_DerivedFormula", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public virtual string DerivedFormula { get; set; }

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


        public IEnumerable<SelectListItem> GetParameterTypeList()
        {
            IDictionary<EnumParameterType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumParameterType>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

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