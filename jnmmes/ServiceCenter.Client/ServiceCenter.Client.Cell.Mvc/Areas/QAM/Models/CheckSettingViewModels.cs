
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
    public class CheckSettingQueryViewModel
    {
        public CheckSettingQueryViewModel()
        {

        }
        [Display(Name = "CheckSettingQueryViewModel_GroupName", ResourceType = typeof(QAMResources.StringResource))]
        public string GroupName { get; set; }

        [Display(Name = "CheckSettingQueryViewModel_MaterialType", ResourceType = typeof(QAMResources.StringResource))]
        public string MaterialType { get; set; }

        [Display(Name = "CheckSettingQueryViewModel_MaterialCode", ResourceType = typeof(QAMResources.StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "CheckSettingQueryViewModel_RouteEnterpriseName", ResourceType = typeof(QAMResources.StringResource))]
        public string RouteEnterpriseName { get; set; }

        [Display(Name = "CheckSettingQueryViewModel_RouteName", ResourceType = typeof(QAMResources.StringResource))]
        public string RouteName { get; set; }

        [Display(Name = "CheckSettingQueryViewModel_RouteStepName", ResourceType = typeof(QAMResources.StringResource))]
        public string RouteStepName { get; set; }

        [Display(Name = "CheckSettingQueryViewModel_RouteOperationName", ResourceType = typeof(QAMResources.StringResource))]
        public string RouteOperationName { get; set; }

        [Display(Name = "CheckSettingQueryViewModel_ProductionLineCode", ResourceType = typeof(QAMResources.StringResource))]
        public string ProductionLineCode { get; set; }

        [Display(Name = "CheckSettingQueryViewModel_EquipmentCode", ResourceType = typeof(QAMResources.StringResource))]
        public string EquipmentCode { get; set; }

    }

    public class CheckSettingViewModel
    {
        public CheckSettingViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.Status = EnumObjectStatus.Available;
        }

        [Required]
        public string Key { get; set; }

        [Required]
        [Display(Name = "CheckSettingViewModel_GroupName", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string GroupName { get; set; }

        [Required]
        [Display(Name = "CheckSettingViewModel_ActionName", ResourceType = typeof(QAMResources.StringResource))]
        public EnumCheckAction ActionName { get; set; }

        [Required]
        [Display(Name = "CheckSettingViewModel_RouteOperationName", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteOperationName { get; set; }

        [Display(Name = "CheckSettingViewModel_MaterialType", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialType { get; set; }


        [Display(Name = "CheckSettingViewModel_MaterialCode", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "CheckSettingViewModel_ProductionLineCode", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ProductionLineCode { get; set; }


        [Display(Name = "CheckSettingViewModel_EquipmentCode", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string EquipmentCode { get; set; }



        [Display(Name = "CheckSettingViewModel_RouteEnterpriseName", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteEnterpriseName { get; set; }

        [Display(Name = "CheckSettingViewModel_RouteName", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteName { get; set; }

        [Display(Name = "CheckSettingViewModel_RouteStepName", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteStepName { get; set; }
       


        [Display(Name = "Status", ResourceType = typeof(StringResource))]
        public EnumObjectStatus Status { get; set; }

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


        public IEnumerable<SelectListItem> GetObjectStatusList()
        {
            IDictionary<EnumObjectStatus, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumObjectStatus>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetActionList()
        {
            IDictionary<EnumCheckAction, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumCheckAction>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }


        public IEnumerable<SelectListItem> GetProductionLineCodeList()
        {
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return from item in result.Data
                           select new SelectListItem()
                           {
                               Text = item.Key,
                               Value = item.Key
                           };
                }
            }
            return new List<SelectListItem>();
        }

        public IEnumerable<SelectListItem> GetRouteOperationNameList()
        {
            IList<RouteOperation> lstRouteOperation = new List<RouteOperation>();
            //获取工序名称。
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lstRouteOperation = result.Data;
                }
            }

            return from item in lstRouteOperation
                   orderby item.SortSeq
                   select new SelectListItem()
                   {
                       Text = item.Key,
                       Value = item.Key
                   };
        }

        public IEnumerable<SelectListItem> GetMaterialTypeList()
        {
            IList<MaterialType> lst = new List<MaterialType>();
            //获取物料类型。
            using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<MaterialType>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lst = result.Data;
                }
            }

            return from item in lst
                   select new SelectListItem()
                   {
                       Text = item.Key,
                       Value = item.Key
                   };
        }

        public IEnumerable<SelectListItem> GetRouteEnterpriseNameList()
        {
            using (RouteEnterpriseServiceClient client = new RouteEnterpriseServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<RouteEnterprise>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return from item in result.Data
                           select new SelectListItem()
                           {
                               Text = item.Key,
                               Value = item.Key
                           };
                }
            }
            return new List<SelectListItem>();
        }
    }
}