
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.EMS;
using EMSResources = ServiceCenter.Client.Mvc.Resources.EMS;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.EMS;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.EMS.Models
{
    public class EquipmentStateEventQueryViewModel
    {
        public EquipmentStateEventQueryViewModel()
        {
            this.StartCreateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.EndCreateTime = DateTime.Now.AddDays(1);
        }

        [Display(Name = "EquipmentStateEventQueryViewModel_EquipmentCode", ResourceType = typeof(EMSResources.StringResource))]
        public string EquipmentCode { get; set; }

        [Display(Name = "EquipmentStateEventQueryViewModel_ChangeStateName", ResourceType = typeof(EMSResources.StringResource))]
        public string ChangeStateName { get; set; }

        [Display(Name = "EquipmentStateEventQueryViewModel_FromStateName", ResourceType = typeof(EMSResources.StringResource))]
        public string FromStateName { get; set; }

        [Display(Name = "EquipmentStateEventQueryViewModel_ToStateName", ResourceType = typeof(EMSResources.StringResource))]
        public string ToStateName { get; set; }

        [Required]
        [Display(Name = "EquipmentStateEventQueryViewModel_StartCreateTime", ResourceType = typeof(EMSResources.StringResource))]
        public DateTime? StartCreateTime { get; set; }

        [Required]
        [Display(Name = "EquipmentStateEventQueryViewModel_EndCreateTime", ResourceType = typeof(EMSResources.StringResource))]
        public DateTime? EndCreateTime { get; set; }
    }

    public class EquipmentStateEventViewModel
    {
        public EquipmentStateEventViewModel()
        {
            this.CreateTime = DateTime.Now;
        }
        [Required]
        [Display(Name = "EquipmentStateEventViewModel_RouteOperationName", ResourceType = typeof(EMSResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteOperationName { get; set; }

        [Required]
        [Display(Name = "EquipmentStateEventViewModel_ProductionLine", ResourceType = typeof(EMSResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ProductionLine { get; set; }

        [Required]
        [Display(Name = "EquipmentStateEventViewModel_EquipmentCode", ResourceType = typeof(EMSResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string EquipmentCode { get; set; }
        [Display(Name = "EquipmentStateEventViewModel_EquipmentName", ResourceType = typeof(EMSResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                    , ErrorMessageResourceName = "ValidateStringLength"
                    , ErrorMessageResourceType = typeof(StringResource))]
        public string EquipmentName { get; set; }

        [Required]
        [Display(Name = "EquipmentStateEventViewModel_ChangeStateName", ResourceType = typeof(EMSResources.StringResource))]
        public string ChangeStateName { get; set; }

        [Required]
        [Display(Name = "EquipmentStateEventViewModel_FromStateName", ResourceType = typeof(EMSResources.StringResource))]
        public string FromStateName { get; set; }

        [Required]
        [Display(Name = "EquipmentStateEventViewModel_ToStateName", ResourceType = typeof(EMSResources.StringResource))]
        public string ToStateName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        public IEnumerable<SelectListItem> GetRouteOperaionNameList()
        {
            //获取用户拥有权限的工序名称。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.RouteOperation);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }
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

            List<SelectListItem> lst = (from item in lstRouteOperation
                                        where lstResource.Any(m => m.Data == item.Key)
                                        orderby item.SortSeq
                                        select new SelectListItem()
                                        {
                                            Text = item.Key,
                                            Value = item.Key
                                        }).ToList();
            if (lst.Count > 0)
            {
                lst[0].Selected = true;
            }
            return lst;
        }

        public IEnumerable<SelectListItem> GetProductionLineList()
        {
            //获取用户拥有权限的生产线代码。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.ProductionLine);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }
            IList<ProductionLine> lstProductionLine = new List<ProductionLine>();
            //获取生产线。
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lstProductionLine = result.Data;
                }
            }

            List<SelectListItem> lst = (from item in lstProductionLine
                                        where lstResource.Any(m => m.Data == item.Key)
                                        orderby item.LocationName
                                        select new SelectListItem()
                                        {
                                            Text = item.Key,
                                            Value = item.Key
                                        }).ToList();

            if (lst.Count > 0)
            {
                lst[0].Selected = true;
            }
            return lst;
        }

    }
}