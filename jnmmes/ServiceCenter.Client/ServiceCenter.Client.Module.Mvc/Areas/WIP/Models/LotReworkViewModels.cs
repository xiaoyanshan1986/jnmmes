
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
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class LotReworkViewModel
    {
        public LotReworkViewModel()
        {
            this.IsPackageNo = false;
        }
        [Required]
        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        //[StringLength(50, MinimumLength = 1
        //        , ErrorMessageResourceName = "ValidateStringLength"
        //        , ErrorMessageResourceType = typeof(StringResource))]
        //[RegularExpression("[^,]+"
        //      , ErrorMessageResourceName = "ValidateString"
        //      , ErrorMessageResourceType = typeof(StringResource))]
        public string LotNumber { get; set; }

        [Display(Name = "LotReworkViewModel_IsPackageNo", ResourceType = typeof(WIPResources.StringResource))]
        public bool IsPackageNo { get; set; }

        [Required]
        [Display(Name = "LotReworkViewModel_NewOrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string NewOrderNumber { get; set; }

        [Required]
        [Display(Name = "LotReworkViewModel_NewMaterialCode", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
             , ErrorMessageResourceName = "ValidateString"
             , ErrorMessageResourceType = typeof(StringResource))]
        public string NewMaterialCode { get; set; }

        [Required]
        [Display(Name = "LotReworkViewModel_RouteEnterpriseName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteEnterpriseName { get; set; }
        [Required]
        [Display(Name = "LotReworkViewModel_RouteName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteName { get; set; }
        [Required]
        [Display(Name = "LotReworkViewModel_RouteStepName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteStepName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }




        public  bool AuthenticateResourceFunction(string functionCode)
        {
            bool isAuthorize = true;
            try
            {
                using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
                {
                    MethodReturnResult result = client.AuthenticateResource(HttpContext.Current.User.Identity.Name, ResourceType.MenuItemFunction, functionCode);
                    if (result.Code > 0)
                    {
                        isAuthorize = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isAuthorize = false;
            }
          
            return isAuthorize;
        }
    }

    /// <summary>
    /// 托返工模型
    /// </summary>
    public class PackageReworkViewModel
    {
        public PackageReworkViewModel()
        {
            this.IsLot = true;
        }

        /// <summary>
        /// 托号
        /// </summary>
        [Required]
        [Display(Name = "PackageNo", ResourceType = typeof(WIPResources.StringResource))]
        public string PackageNo { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        //[Required]
        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string LotNumber { get; set; }

        /// <summary>
        /// 车间
        /// </summary>
        [Display(Name = "LocationName", ResourceType = typeof(WIPResources.StringResource))]
        public string LocationName { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        [Display(Name = "LineCode", ResourceType = typeof(WIPResources.StringResource))]
        public string LineCode { get; set; }

        /// <summary>
        /// 工艺流程
        /// </summary>
        [Display(Name = "PackageReworkViewModel_RouteName", ResourceType = typeof(WIPResources.StringResource))]
        public string RouteName { get; set; }

        /// <summary>
        /// 工步
        /// </summary>
        [Display(Name = "PackageReworkViewModel_RouteStepName", ResourceType = typeof(WIPResources.StringResource))]
        public string RouteStepName { get; set; }

        /// <summary>
        /// 工单
        /// </summary>
        [Display(Name = "OrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 是否保留托号
        /// </summary>
        [Display(Name = "RetainPackageNo", ResourceType = typeof(WIPResources.StringResource))]
        public bool RetainPackageNo { get; set; }

        /// <summary>
        /// 是否按批次号投料
        /// </summary>
        [Display(Name = "IsLot", ResourceType = typeof(WIPResources.StringResource))]
        public bool IsLot { get; set; }

        /// <summary>
        /// 取得车间列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetLocationName()
        {
            using (LocationServiceClient client = new LocationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Level='{0}'", Convert.ToInt32(LocationLevel.Room))
                };

                MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
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

        /// <summary>
        /// 获取生产线列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetLineList()
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

            IList<ProductionLine> lst = new List<ProductionLine>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false
            };
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }

            return from item in lst
                   where lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = string.Format("{0}[{1}]", item.Name, item.Key),
                       Value = item.Key
                   };
        }
    }
}