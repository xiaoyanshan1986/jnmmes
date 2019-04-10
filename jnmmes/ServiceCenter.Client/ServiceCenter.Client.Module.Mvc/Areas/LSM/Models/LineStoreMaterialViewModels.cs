
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
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Model.ZPVM;

namespace ServiceCenter.Client.Mvc.Areas.LSM.Models
{
    public class LineStoreMaterialQueryViewModel
    {
        public LineStoreMaterialQueryViewModel()
        {

        }

        [Display(Name = "LineStoreMaterialQueryViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        public string LineStoreName { get; set; }
        [Display(Name = "LineStoreMaterialQueryViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialCode { get; set; }

    }

    public class LineStoreMaterialViewModel
    {
        public LineStoreMaterialViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }
        [Required]
        [Display(Name = "LineStoreMaterialViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string LineStoreName { get; set; }

        [Required]
        [Display(Name = "LineStoreMaterialViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                      , ErrorMessageResourceName = "ValidateStringLength"
                      , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }


        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

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

    public class LineStoreMaterialDetailQueryViewModel
    {
        public LineStoreMaterialDetailQueryViewModel()
        {

        }

        [Display(Name = "LineStoreMaterialDetailQueryViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        public string LineStoreName { get; set; }
        [Display(Name = "LineStoreMaterialDetailQueryViewModel_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "LineStoreMaterialDetailQueryViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialCode { get; set; }
        [Display(Name = "LineStoreMaterialDetailQueryViewModel_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialLot { get; set; }

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

        public Supplier GetSupplier(string key)
        {
            using (SupplierServiceClient client = new SupplierServiceClient())
            {
                MethodReturnResult<Supplier> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }
    }

    public class LineStoreMaterialDetailViewModel
    {
        public LineStoreMaterialDetailViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Display(Name = "LineStoreMaterialDetailViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        public string LineStoreName { get; set; }


        [Display(Name = "LineStoreMaterialDetailViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialCode { get; set; }


        [Display(Name = "LineStoreMaterialDetailViewModel_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialLot { get; set; }

        [Display(Name = "LineStoreMaterialDetailViewModel_ReceiveQty", ResourceType = typeof(LSMResources.StringResource))]
        public double ReceiveQty { get; set; }
        [Display(Name = "LineStoreMaterialDetailViewModel_ReturnQty", ResourceType = typeof(LSMResources.StringResource))]
        public double ReturnQty { get; set; }
        [Display(Name = "LineStoreMaterialDetailViewModel_ScrapQty", ResourceType = typeof(LSMResources.StringResource))]
        public double ScrapQty { get; set; }
        [Display(Name = "LineStoreMaterialDetailViewModel_LoadingQty", ResourceType = typeof(LSMResources.StringResource))]
        public double LoadingQty { get; set; }
        [Display(Name = "LineStoreMaterialDetailViewModel_UnloadingQty", ResourceType = typeof(LSMResources.StringResource))]
        public double UnloadingQty { get; set; }
        [Display(Name = "LineStoreMaterialDetailViewModel_CurrentQty", ResourceType = typeof(LSMResources.StringResource))]
        public double CurrentQty { get; set; }

        [Display(Name = "LineStoreMaterialDetailViewModel_SupplierCode", ResourceType = typeof(LSMResources.StringResource))]
        public string SupplierCode { get; set; }

        [Display(Name = "LineStoreMaterialDetailViewModel_SupplierMaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        public string SupplierMaterialLot { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        public string Description { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }
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

        public Supplier GetSupplier(string key)
        {
            using (SupplierServiceClient client = new SupplierServiceClient())
            {
                MethodReturnResult<Supplier> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }

        //获取经供应商转换所得生产厂商信息
        public Manufacturer GetSupplierToManufacturer(string materialCode, string orderNumber, Supplier s)
        {
            Manufacturer mf = null;
            if (s != null)
            {
                using (SupplierToManufacturerServiceClient clients = new SupplierToManufacturerServiceClient())
                {
                    PagingConfig cfg0 = new PagingConfig()
                    {
                        Where = string.Format(@"Key.MaterialCode='{0}' AND Key.SupplierCode='{1}'"
                                                , materialCode
                                                , s.Key)
                    };
                    MethodReturnResult<IList<SupplierToManufacturer>> result = clients.Gets(ref cfg0);
                    if (result.Code <= 0 && result.Data.Count > 0)
                    {
                        if (result.Data[0].Key.OrderNumber == "*")
                        {
                            using (ManufacturerServiceClient clientss = new ManufacturerServiceClient())
                            {
                                MethodReturnResult<Manufacturer> rsts = clientss.Get(result.Data[0].ManufacturerCode);
                                if (rsts.Data != null)
                                {
                                    mf = rsts.Data;
                                }
                            }
                        }
                        else
                        {
                            PagingConfig cfg1 = new PagingConfig()
                            {
                                Where = string.Format(@"Key.MaterialCode='{0}' AND Key.OrderNumber='{1}' AND Key.SupplierCode='{2}'"
                                                        , materialCode
                                                        , orderNumber
                                                        , s.Key)
                            };
                            MethodReturnResult<IList<SupplierToManufacturer>> results = clients.Gets(ref cfg1);
                            if (results.Code <= 0 && results.Data.Count > 0)
                            {
                                using (ManufacturerServiceClient clientss = new ManufacturerServiceClient())
                                {
                                    MethodReturnResult<Manufacturer> rsts = clientss.Get(results.Data[0].ManufacturerCode);
                                    if (rsts.Data != null)
                                    {
                                        mf = rsts.Data;
                                    }
                                }
                            }
                        }
                    }
                }
            }          
            return mf;
        }
    }

}