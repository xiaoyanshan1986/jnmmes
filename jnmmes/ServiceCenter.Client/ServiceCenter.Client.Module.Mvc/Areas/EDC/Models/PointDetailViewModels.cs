
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
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Models
{
    public class PointDetailQueryViewModel
    {
        public PointDetailQueryViewModel()
        {

        }
        [Display(Name = "PointDetailQueryViewModel_PointKey", ResourceType = typeof(EDCResources.StringResource))]
        public string PointKey { get; set; }

        [Display(Name = "PointDetailQueryViewModel_GroupName", ResourceType = typeof(EDCResources.StringResource))]
        public string GroupName { get; set; }

        [Display(Name = "PointDetailQueryViewModel_ParameterName", ResourceType = typeof(EDCResources.StringResource))]
        public string ParameterName { get; set; }
    }

    public class PointDetailViewModel
    {
        public PointDetailViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        public string PointKey { get; set; }

        [Required]
        [Display(Name = "PointDetailViewModel_ParameterName", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ParameterName { get; set; }

        [Required]
        [Display(Name = "PointDetailViewModel_ItemNo", ResourceType = typeof(EDCResources.StringResource))]
        public int ItemNo { get; set; }

        [Required]
        [Display(Name = "PointDetailViewModel_ParameterCount", ResourceType = typeof(EDCResources.StringResource))]
        [RegularExpression("[0-9]+"
                            , ErrorMessageResourceName = "ValidateInt"
                            , ErrorMessageResourceType = typeof(StringResource))]
        public int ParameterCount { get; set; }

        [Required]
        [Display(Name = "PointDetailViewModel_ParameterType", ResourceType = typeof(EDCResources.StringResource))]
        public EnumParameterType ParameterType { get; set; }

        [Display(Name = "PointDetailViewModel_DataType", ResourceType = typeof(EDCResources.StringResource))]
        public EnumDataType DataType { get; set; }

        [Display(Name = "PointDetailViewModel_DeviceType", ResourceType = typeof(EDCResources.StringResource))]
        public EnumDeviceType DeviceType { get; set; }


        [Display(Name = "PointDetailViewModel_Mandatory", ResourceType = typeof(EDCResources.StringResource))]
        public bool Mandatory { get; set; }


        [Display(Name = "PointDetailViewModel_IsDerived", ResourceType = typeof(EDCResources.StringResource))]
        public bool IsDerived { get; set; }

        [Display(Name = "PointDetailViewModel_DerivedFormula", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(255, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string DerivedFormula { get; set; }

        [Display(Name = "PointDetailViewModel_UpperBoundary", ResourceType = typeof(EDCResources.StringResource))]
        public double? UpperBoundary { get; set; }

        [Display(Name = "PointDetailViewModel_UpperSpecification", ResourceType = typeof(EDCResources.StringResource))]
        public double? UpperSpecification { get; set; }

        [Display(Name = "PointDetailViewModel_UpperControl", ResourceType = typeof(EDCResources.StringResource))]
        public double? UpperControl { get; set; }


        [Display(Name = "PointDetailViewModel_Target", ResourceType = typeof(EDCResources.StringResource))]
        public double? Target { get; set; }

        [Display(Name = "PointDetailViewModel_LowerControl", ResourceType = typeof(EDCResources.StringResource))]
        public double? LowerControl { get; set; }

        [Display(Name = "PointDetailViewModel_LowerSpecification", ResourceType = typeof(EDCResources.StringResource))]
        public double? LowerSpecification { get; set; }
        [Display(Name = "PointDetailViewModel_LowerBoundary", ResourceType = typeof(EDCResources.StringResource))]
        public double? LowerBoundary { get; set; }


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