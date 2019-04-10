
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.LSM;
using LSMResources = ServiceCenter.Client.Mvc.Resources.LSM;
using ServiceCenter.Client.Mvc.Resources;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.LSM;

namespace ServiceCenter.Client.Mvc.Areas.LSM.Models
{
    public class MaterialLoadingQueryViewModel
    {
        public MaterialLoadingQueryViewModel()
        {
            this.StartLoadingTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.EndLoadingTime = DateTime.Now;
        }
        [Display(Name = "MaterialLoadingQueryViewModel_LoadingNo", ResourceType = typeof(LSMResources.StringResource))]
        public string LoadingNo { get; set; }
        [Display(Name = "MaterialLoadingQueryViewModel_RouteOperationName", ResourceType = typeof(LSMResources.StringResource))]
        public string RouteOperationName { get; set; }
        [Display(Name = "MaterialLoadingQueryViewModel_ProductionLineCode", ResourceType = typeof(LSMResources.StringResource))]
        public string ProductionLineCode { get; set; }
        [Display(Name = "MaterialLoadingQueryViewModel_EquipmentCode", ResourceType = typeof(LSMResources.StringResource))]
        public string EquipmentCode { get; set; }
        [Display(Name = "MaterialLoadingQueryViewModel_StartLoadingTime", ResourceType = typeof(LSMResources.StringResource))]
        public DateTime? StartLoadingTime { get; set; }

        [Display(Name = "MaterialLoadingQueryViewModel_EndLoadingTime", ResourceType = typeof(LSMResources.StringResource))]
        public DateTime? EndLoadingTime { get; set; }
    }

    public class MaterialLoadingViewModel
    {
        public MaterialLoadingViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.LoadingTime = DateTime.Now;
        }
        [Required]
        [Display(Name = "MaterialLoadingViewModel_LoadingNo", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        //[RegularExpression("MLM[0-9]{2}(0[1-9]|1[0-2])[0-9]{6}"
        //                , ErrorMessage = "格式为：MLM+YYMM(年月)+6位流水号")]
        public string LoadingNo { get; set; }

        [Required]
        [Display(Name = "MaterialLoadingViewModel_RouteOperationName", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteOperationName { get; set; }


        [Required]
        [Display(Name = "MaterialLoadingViewModel_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "MaterialLoadingViewModel_ProductionLineCode", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                      , ErrorMessageResourceName = "ValidateStringLength"
                      , ErrorMessageResourceType = typeof(StringResource))]
        public string ProductionLineCode { get; set; }
        [Required]
        [Display(Name = "MaterialLoadingViewModel_EquipmentCode", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                      , ErrorMessageResourceName = "ValidateStringLength"
                      , ErrorMessageResourceType = typeof(StringResource))]
        public string EquipmentCode { get; set; }

        
        [Display(Name = "MaterialLoadingViewModel_LoadingTime", ResourceType = typeof(LSMResources.StringResource))]
        public DateTime LoadingTime { get; set; }

        [Display(Name = "MaterialLoadingViewModel_Operator", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                      , ErrorMessageResourceName = "ValidateStringLength"
                      , ErrorMessageResourceType = typeof(StringResource))]
        public string Operator { get; set; }

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

        public IEnumerable<SelectListItem> GetRouteOperationNameList()
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
                    where lstResource.Any(m=>m.Data==item.Key)
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

        public IEnumerable<SelectListItem> GetProductionLineCodeList()
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

        public Material GetMaterial(string key)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }
    }

    public class MaterialLoadingDetailQueryViewModel : MaterialLoadingQueryViewModel
    {
        public MaterialLoadingDetailQueryViewModel()
        {
            this.IsShowZeroData = true;
        }

        [Display(Name = "MaterialLoadingViewModel_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "MaterialLoadingDetailViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "MaterialLoadingDetailViewModel_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialLot { get; set; }

        [Display(Name = "MaterialLoadingDetailViewModel_IsShowZeroData", ResourceType = typeof(LSMResources.StringResource))]
        public bool IsShowZeroData { get; set; }

        public MaterialLoading GetMaterialLoading(string loadingNo)
        {
            MaterialLoading obj = new MaterialLoading();
            using (MaterialLoadingServiceClient client = new MaterialLoadingServiceClient())
            {
                MethodReturnResult<MaterialLoading> result = client.Get(loadingNo);
                if (result.Code <= 0 && result.Data != null)
                {
                    obj = result.Data;
                }
            }
            return obj;
        }

        public Material GetMaterial(string key)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }
    }

    public class MaterialLoadingDetailViewModel
    {
        public MaterialLoadingDetailViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Display(Name = "MaterialLoadingDetailViewModel_LoadingNo", ResourceType = typeof(LSMResources.StringResource))]
        public string LoadingNo { get; set; }
        
        [Display(Name = "MaterialLoadingDetailViewModel_ItemNo", ResourceType = typeof(LSMResources.StringResource))]
        public int ItemNo { get; set; }

        [Required]
        [Display(Name = "MaterialLoadingDetailViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
               , ErrorMessageResourceName = "ValidateStringLength"
               , ErrorMessageResourceType = typeof(StringResource))]
        public string LineStoreName { get; set; }

        [Required]
        [Display(Name = "MaterialLoadingDetailViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
               , ErrorMessageResourceName = "ValidateStringLength"
               , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        [Required]
        [Display(Name = "MaterialLoadingDetailViewModel_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
             , ErrorMessageResourceName = "ValidateStringLength"
             , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
               , ErrorMessageResourceName = "ValidateString"
               , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialLot { get; set; }

        [Required]
        [Display(Name = "MaterialLoadingDetailViewModel_LoadingQty", ResourceType = typeof(LSMResources.StringResource))]
        [Range(0, 2147483648
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        public double? LoadingQty { get; set; }

        [Display(Name = "MaterialLoadingDetailViewModel_UnloadingQty", ResourceType = typeof(LSMResources.StringResource))]
        [Range(0, 2147483648
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        public double? UnloadingQty { get; set; }

        [Required]
        [Display(Name = "MaterialLoadingDetailViewModel_CurrentQty", ResourceType = typeof(LSMResources.StringResource))]
        [Range(0, 2147483648
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        public double CurrentQty { get; set; }
        
        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }


        public MaterialLoading GetMaterialLoading(string loadingNo)
        {
            MaterialLoading obj = new MaterialLoading();
            using(MaterialLoadingServiceClient client=new MaterialLoadingServiceClient())
            {
                MethodReturnResult<MaterialLoading> result = client.Get(loadingNo);
                if(result.Code<=0 && result.Data!=null)
                {
                    obj = result.Data;
                }
            }
            return obj;
        }

        public Material GetMaterial(string key)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }
    }

}