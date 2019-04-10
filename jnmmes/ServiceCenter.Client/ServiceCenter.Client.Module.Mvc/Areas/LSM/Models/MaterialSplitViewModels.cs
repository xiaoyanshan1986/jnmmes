
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
using ServiceCenter.MES.Service.Client.LSM;
using System.Data;

namespace ServiceCenter.Client.Mvc.Areas.LSM.Models
{
    public class MaterialSplitQueryViewModel
    {
        [Display(Name = "物料批号")]
        public string MaterialLotNumber { get; set; }

        [Display(Name = "工单号")]
        public string OrderNumber { get; set; }
        [Required]
        [Display(Name = "拆批数量")]
        [Range(1, 100
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int count { get; set; }
    }

    public class MaterialSplitViewModel
    {

        public MaterialSplitViewModel()
        {
            ChildViewModel = new List<ChildViewModel>();
            ChildViewModel childViewModel;
            for (int i = 0; i < this.count; i++)
            {
                childViewModel = new ChildViewModel();
                this.ChildViewModel.Add(childViewModel);
            }
        }


        [Required]
        [Display(Name = "拆批数量")]
        public int count { get; set; }

        [Required]
        [Display(Name = "父级批次号")]
        public string ParentMaterialLotNumber { get; set; }


        [Display(Name = "工单号")]
        public string OrderNumber { get; set; }


        [Display(Name = "子批次号信息")]
        public IList<ChildViewModel> ChildViewModel { get; set; }


        //public MaterialSplitViewModel()
        //{
        //    this.CreateTime = DateTime.Now;
        //    this.EditTime = DateTime.Now;
        //}
        //[Display(Name = "LineStoreMaterialDetailQueryViewModel_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        //public string OrderNumber { get; set; }
        //[Display(Name = "LineStoreMaterialDetailViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        //public string LineStoreName { get; set; }

        //[Display(Name = "LineStoreMaterialDetailViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        //public string MaterialCode { get; set; }

        //[Display(Name = "LineStoreMaterialDetailViewModel_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        //public string MaterialLot { get; set; }




        //[Display(Name = "LineStoreMaterialDetailViewModel_ReceiveQty", ResourceType = typeof(LSMResources.StringResource))]
        //public double ReceiveQty { get; set; }
        //[Display(Name = "LineStoreMaterialDetailViewModel_ReturnQty", ResourceType = typeof(LSMResources.StringResource))]
        //public double ReturnQty { get; set; }
        //[Display(Name = "LineStoreMaterialDetailViewModel_ScrapQty", ResourceType = typeof(LSMResources.StringResource))]
        //public double ScrapQty { get; set; }
        //[Display(Name = "LineStoreMaterialDetailViewModel_LoadingQty", ResourceType = typeof(LSMResources.StringResource))]
        //public double LoadingQty { get; set; }
        //[Display(Name = "LineStoreMaterialDetailViewModel_UnloadingQty", ResourceType = typeof(LSMResources.StringResource))]
        //public double UnloadingQty { get; set; }
        //[Display(Name = "LineStoreMaterialDetailViewModel_CurrentQty", ResourceType = typeof(LSMResources.StringResource))]
        //public double CurrentQty { get; set; }

        //[Display(Name = "LineStoreMaterialDetailViewModel_SupplierCode", ResourceType = typeof(LSMResources.StringResource))]
        //public string SupplierCode { get; set; }

        //[Display(Name = "LineStoreMaterialDetailViewModel_SupplierMaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        //public string SupplierMaterialLot { get; set; }

        //[Display(Name = "Description", ResourceType = typeof(StringResource))]
        //public string Description { get; set; }

        //[Display(Name = "Editor", ResourceType = typeof(StringResource))]
        //public string Editor { get; set; }


        //[Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        //public DateTime? EditTime { get; set; }


        //[Display(Name = "Creator", ResourceType = typeof(StringResource))]
        //public string Creator { get; set; }


        //[Display(Name = "CreateTime", ResourceType = typeof(StringResource))]

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
    }
      

    public class ChildViewModel
    {
        public ChildViewModel()
        {
        }
        [Required]
        [Display(Name = "子批次号")]
        public String ChildMaterialLotNumber { get; set; }
        [Required]
        [Display(Name = "数量")]
        public double Quantity { get; set; }
    }

    public class MaterialSplitDetailQueryViewModel
    {
        public MaterialSplitDetailQueryViewModel()
        {
            this.ReturnDate = DateTime.Now.ToString("yyyy-MM-dd");
        }

        [Display(Name = "MaterialSplitDetailQueryViewModel_ReturnNo", ResourceType = typeof(LSMResources.StringResource))]
        public string ReturnNo { get; set; }

        [Display(Name = "MaterialSplitDetailQueryViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        public string LineStoreName { get; set; }

        [Display(Name = "MaterialSplitViewModel_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "MaterialSplitDetailQueryViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "MaterialSplitDetailQueryViewModel_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialLot { get; set; }


        [Display(Name = "MaterialSplitViewModel_ReturnDate", ResourceType = typeof(LSMResources.StringResource))]
        public string ReturnDate { get; set; }

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

        //public MaterialSplit GetMaterialSplit(string key)
        //{
        //    using (MaterialSplitServiceClient client = new MaterialSplitServiceClient())
        //    {
        //        MethodReturnResult<MaterialSplit> rst = client.Get(key);
        //        if (rst.Code <= 0)
        //        {
        //            return rst.Data;
        //        }
        //    }
        //    return null;
        //}
    }

    public class MaterialSplitDetailViewModel
    {
        public MaterialSplitDetailViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Display(Name = "MaterialSplitDetailViewModel_ReturnNo", ResourceType = typeof(LSMResources.StringResource))]
        public string ReturnNo { get; set; }

        [Display(Name = "MaterialSplitDetailViewModel_ItemNo", ResourceType = typeof(LSMResources.StringResource))]
        public int ItemNo { get; set; }
        [Required]
        [Display(Name = "MaterialSplitDetailViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string LineStoreName { get; set; }
        [Required]
        [Display(Name = "MaterialSplitDetailViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }
        [Required]
        [Display(Name = "MaterialSplitDetailViewModel_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
              , ErrorMessageResourceName = "ValidateStringLength"
              , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
               , ErrorMessageResourceName = "ValidateString"
               , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialLot { get; set; }
        [Required]
        [Display(Name = "MaterialSplitDetailViewModel_Qty", ResourceType = typeof(LSMResources.StringResource))]
        [Range(0, 2147483648
                  , ErrorMessageResourceName = "ValidateRange"
                  , ErrorMessageResourceType = typeof(StringResource))]
        public double? Qty { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                    , ErrorMessageResourceName = "ValidateString"
                    , ErrorMessageResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                , ErrorMessageResourceName = "ValidateMaxStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string DetailDescription { get; set; }

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

        //public MaterialSplit GetMaterialSplit(string key)
        //{
        //    using (MaterialSplitServiceClient client = new MaterialSplitServiceClient())
        //    {
        //        MethodReturnResult<MaterialSplit> rst = client.Get(key);
        //        if (rst.Code <= 0)
        //        {
        //            return rst.Data;
        //        }
        //    }
        //    return null;
        //}
    }

}