
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
    public class PointQueryViewModel
    {
        public PointQueryViewModel()
        {

        }
        [Display(Name = "PointQueryViewModel_GroupName", ResourceType = typeof(EDCResources.StringResource))]
        public string GroupName { get; set; }

        [Display(Name = "PointQueryViewModel_MaterialType", ResourceType = typeof(EDCResources.StringResource))]
        public string MaterialType { get; set; }

        [Display(Name = "PointQueryViewModel_MaterialCode", ResourceType = typeof(EDCResources.StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "PointQueryViewModel_RouteEnterpriseName", ResourceType = typeof(EDCResources.StringResource))]
        public string RouteEnterpriseName { get; set; }

        [Display(Name = "PointQueryViewModel_RouteName", ResourceType = typeof(EDCResources.StringResource))]
        public string RouteName { get; set; }

        [Display(Name = "PointQueryViewModel_RouteStepName", ResourceType = typeof(EDCResources.StringResource))]
        public string RouteStepName { get; set; }

        [Display(Name = "PointQueryViewModel_RouteOperationName", ResourceType = typeof(EDCResources.StringResource))]
        public string RouteOperationName { get; set; }

        [Display(Name = "PointQueryViewModel_ProductionLineCode", ResourceType = typeof(EDCResources.StringResource))]
        public string ProductionLineCode { get; set; }

        [Display(Name = "PointQueryViewModel_EquipmentCode", ResourceType = typeof(EDCResources.StringResource))]
        public string EquipmentCode { get; set; }

        [Display(Name = "PointQueryViewModel_CategoryName", ResourceType = typeof(EDCResources.StringResource))]
        public string CategoryName { get; set; }

        [Display(Name = "PointQueryViewModel_SamplingPlanName", ResourceType = typeof(EDCResources.StringResource))]
        public string SamplingPlanName { get; set; }

    }

    public class PointViewModel
    {
        public PointViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.Status = EnumObjectStatus.Available;
        }

        [Required]
        public string Key { get; set; }

        [Required]
        [Display(Name = "PointViewModel_GroupName", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string GroupName { get; set; }

        [Required]
        [Display(Name = "PointViewModel_ActionName", ResourceType = typeof(EDCResources.StringResource))]
        public EnumEDCAction ActionName { get; set; }
        [Required]
        [Display(Name = "PointViewModel_CategoryName", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string CategoryName { get; set; }
        [Required]
        [Display(Name = "PointViewModel_SamplingPlanName", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string SamplingPlanName { get; set; }

        [Display(Name = "PointViewModel_ProductionLineCode", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ProductionLineCode { get; set; }

        [Display(Name = "PointViewModel_MaterialType", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialType { get; set; }


        [Display(Name = "PointViewModel_MaterialCode", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }


        [Display(Name = "PointViewModel_RouteEnterpriseName", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteEnterpriseName { get; set; }

        [Display(Name = "PointViewModel_RouteName", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteName { get; set; }

        [Display(Name = "PointViewModel_RouteStepName", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteStepName { get; set; }

        [Display(Name = "PointViewModel_RouteOperationName", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteOperationName { get; set; }



        [Display(Name = "PointViewModel_EquipmentCode", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string EquipmentCode { get; set; }



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
            IDictionary<EnumEDCAction, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumEDCAction>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetSamplingPlanList()
        {
            using (SamplingPlanServiceClient client = new SamplingPlanServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<SamplingPlan>> result = client.Get(ref cfg);
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

        public IEnumerable<SelectListItem> GetCategoryList()
        {
            using (CategoryServiceClient client = new CategoryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<Category>> result = client.Get(ref cfg);
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