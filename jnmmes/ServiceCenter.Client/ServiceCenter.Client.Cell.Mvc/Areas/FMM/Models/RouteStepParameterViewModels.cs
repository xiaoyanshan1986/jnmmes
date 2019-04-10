
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

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class RouteStepParameterQueryViewModel
    {
        public RouteStepParameterQueryViewModel()
        {

        }

        [Display(Name = "RouteStepParameterQueryViewModel_RouteName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteName { get; set; }

        [Display(Name = "RouteStepParameterQueryViewModel_RouteStepName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteStepName { get; set; }

        [Display(Name = "RouteStepParameterQueryViewModel_ParameterName", ResourceType = typeof(FMMResources.StringResource))]
        public string ParameterName { get; set; }
    }

    public class RouteStepParameterViewModel
    {
        public RouteStepParameterViewModel()
        {
            this.EditTime = DateTime.Now;
            this.ParamIndex = 1;
            
        }
        [Required]
        [Display(Name = "RouteStepParameterViewModel_RouteName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteName { get; set; }
        [Required]
        [Display(Name = "RouteStepParameterViewModel_RouteStepName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteStepName { get; set; }

        [Required]
        [Display(Name = "RouteOperationParameterViewModel_ParameterName", ResourceType = typeof(FMMResources.StringResource))]
        public string ParameterName { get; set; }

        [Required]
        [Display(Name = "RouteOperationParameterViewModel_ParamIndex", ResourceType = typeof(FMMResources.StringResource))]
        [Range(1, 65536
              , ErrorMessageResourceName = "ValidateRange"
              , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int ParamIndex { get; set; }

        [Required]
        [Display(Name = "RouteOperationParameterViewModel_DataType", ResourceType = typeof(FMMResources.StringResource))]
        public EnumDataType DataType { get; set; }

        [Required]
        [Display(Name = "RouteOperationParameterViewModel_IsMustInput", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsMustInput { get; set; }

        [Required]
        [Display(Name = "RouteOperationParameterViewModel_DataFrom", ResourceType = typeof(FMMResources.StringResource))]
        public EnumDataFrom DataFrom { get; set; }

        [Required]
        [Display(Name = "RouteOperationParameterViewModel_IsReadOnly", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsReadOnly { get; set; }

        [Required]
        [Display(Name = "RouteOperationParameterViewModel_DCType", ResourceType = typeof(FMMResources.StringResource))]
        public EnumDataCollectionAction DCType { get; set; }

        [Display(Name = "RouteOperationParameterViewModel_MaterialType", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialType { get; set; }

        [Required]
        [Display(Name = "RouteOperationParameterViewModel_ValidateRule", ResourceType = typeof(FMMResources.StringResource))]
        public EnumValidateRule ValidateRule { get; set; }

        [Required]
        [Display(Name = "RouteOperationParameterViewModel_ValidateFailedRule", ResourceType = typeof(FMMResources.StringResource))]
        public EnumValidateFailedRule ValidateFailedRule { get; set; }

        [Display(Name = "RouteOperationParameterViewModel_ValidateFailedMessage", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(255, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string ValidateFailedMessage { get; set; }

        [Required]
        [Display(Name = "RouteOperationParameterViewModel_IsDeleted", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsDeleted { get; set; }

        [Required]
        [Display(Name = "RouteOperationParameterViewModel_IsUsePreValue", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsUsePreValue { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        public IEnumerable<SelectListItem> GetParameterNameList()
        {
            using (ParameterServiceClient client = new ParameterServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Type='{0}'",Convert.ToInt32(EnumParameterType.Route))
                };

                MethodReturnResult<IList<Parameter>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
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

        public IEnumerable<SelectListItem> GetDataFromList()
        {
            IDictionary<EnumDataFrom, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumDataFrom>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetDataCollectionActionList()
        {
            IDictionary<EnumDataCollectionAction, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumDataCollectionAction>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetValidateRuleList()
        {
            IDictionary<EnumValidateRule, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumValidateRule>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetValidateFailedRuleList()
        {
            IDictionary<EnumValidateFailedRule, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumValidateFailedRule>();

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