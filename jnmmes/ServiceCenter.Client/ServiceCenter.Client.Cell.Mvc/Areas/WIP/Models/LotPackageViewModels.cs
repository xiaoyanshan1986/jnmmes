
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.WIP;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.RBAC;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class LotPackageViewModel
    {
        public LotPackageViewModel()
        {
        }


        [Display(Name = "PackageNo", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string PackageNo { get; set; }

        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string LotNumber { get; set; }

        [Required]
        [Display(Name = "LotPackageViewModel_RouteOperationName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteOperationName { get; set; }

        [Required]
        [Display(Name = "LotPackageViewModel_LineCode", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string LineCode { get; set; }

        [Required]
        [Display(Name = "LotPackageViewModel_EquipmentCode", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string EquipmentCode { get; set; }

        [Display(Name = "LotPackageViewModel_EquipmentState", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string EquipmentState { get; set; }


        /// <summary>
        /// 是否尾包？
        /// </summary>
        [Required]
        [Display(Name = "LotPackageViewModel_IsLastestPackage", ResourceType = typeof(WIPResources.StringResource))]
        public bool IsLastestPackage { get; set; }

        /// <summary>
        /// 是否完成包装。
        /// </summary>
        public bool IsFinishPackage { get; set; }

        /// <summary>
        /// 满包数量
        /// </summary>
        [Required]
        [Display(Name = "LotPackageViewModel_FullQuantity", ResourceType = typeof(WIPResources.StringResource))]
        public double FullQuantity { get; set; }

        /// <summary>
        /// 当前数量
        /// </summary>
        [Required]
        [Display(Name = "LotPackageViewModel_CurrentQuantity", ResourceType = typeof(WIPResources.StringResource))]
        public double CurrentQuantity { get; set; }


        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> GetRouteOperationList()
        {
            //获取用户拥有权限的工序。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.RouteOperation);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }

            IList<string> lstPackageOperation = new List<string>();
            using(RouteOperationAttributeServiceClient client=new RouteOperationAttributeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = "Key.AttributeName='IsPackageOperation'"
                };
                MethodReturnResult<IList<RouteOperationAttribute>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data != null)
                {
                    bool isPackageOperation=false;
                    lstPackageOperation = (from item in result.Data
                                          where bool.TryParse(item.Value, out isPackageOperation) == true
                                                && isPackageOperation==true
                                          select item.Key.RouteOperationName).ToList();
                }
            }

            return from item in lstResource
                   where lstPackageOperation.Any(m=>m==item.Data)
                   select new SelectListItem()
                   {
                       Text = item.Data,
                       Value = item.Data
                   };
        }

        public IEnumerable<SelectListItem> GetProductionLineList()
        {
            //获取用户拥有权限的生产线。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.ProductionLine);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }
            return from item in lstResource
                   select new SelectListItem()
                   {
                       Text = item.Data,
                       Value = item.Data
                   };
        }
    }
}