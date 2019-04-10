
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

namespace ServiceCenter.Client.Mvc.Areas.LSM.Models
{
    public class MaterialReturnQueryViewModel
    {
        public MaterialReturnQueryViewModel()
        {

        }
        [Display(Name = "MaterialReturnQueryViewModel_ReturnNo", ResourceType = typeof(LSMResources.StringResource))]
        public string ReturnNo { get; set; }

        [Display(Name = "MaterialReturnQueryViewModel_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "MaterialReturnQueryViewModel_StartReturnDate", ResourceType = typeof(LSMResources.StringResource))]
        public DateTime? StartReturnDate { get; set; }

        [Display(Name = "MaterialReturnQueryViewModel_EndReturnDate", ResourceType = typeof(LSMResources.StringResource))]
        public DateTime? EndReturnDate { get; set; }
    }

    public class MaterialReturnViewModel
    {
        public MaterialReturnViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.ReturnDate = DateTime.Now.Date;
            this.Type = EnumReturnType.Normal;
            this.ReturnNo = "TMK";
        }

        [Required]
        [Display(Name = "MaterialReturnViewModel_ReturnNo", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        //[RegularExpression("TMK[0-9]{2}(0[1-9]|1[0-2])[0-9]{4}"
        //                   , ErrorMessage = "格式为：TMK+YYMM(年月)+4位流水号")]
        public string ReturnNo { get; set; }

        [Required]
        [Display(Name = "MaterialReturnViewModel_Type", ResourceType = typeof(LSMResources.StringResource))]
        public EnumReturnType Type { get; set; }

        [Required]
        [Display(Name = "MaterialReturnViewModel_ReturnDate", ResourceType = typeof(LSMResources.StringResource))]
        public DateTime ReturnDate { get; set; }
        [Required]
        [Display(Name = "MaterialReturnViewModel_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
               , ErrorMessageResourceName = "ValidateStringLength"
               , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

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

    public class MaterialReturnDetailQueryViewModel
    {
        public MaterialReturnDetailQueryViewModel()
        {
            //this.ReturnDate = DateTime.Now.ToString("yyyy-MM-dd");
        }

        [Display(Name = "MaterialReturnDetailQueryViewModel_ReturnNo", ResourceType = typeof(LSMResources.StringResource))]
        public string ReturnNo { get; set; }

        [Display(Name = "MaterialReturnDetailQueryViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        public string LineStoreName { get; set; }

        [Display(Name = "MaterialReturnViewModel_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "MaterialReturnDetailQueryViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "MaterialReturnDetailQueryViewModel_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialLot { get; set; }


        [Display(Name = "MaterialReturnViewModel_ReturnDate", ResourceType = typeof(LSMResources.StringResource))]
        public DateTime ReturnDate { get; set; }

        [Display(Name = "MaterialReturnViewModel_Store", ResourceType = typeof(LSMResources.StringResource))]
        public string Store { get; set; }

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

        public MaterialReturn GetMaterialReturn(string key)
        {
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                MethodReturnResult<MaterialReturn> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }
    }

    public class MaterialReturnDetailViewModel
    {
        public MaterialReturnDetailViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Display(Name = "MaterialReturnDetailViewModel_ReturnNo", ResourceType = typeof(LSMResources.StringResource))]
        public string ReturnNo { get; set; }

        [Display(Name = "MaterialReturnDetailViewModel_ItemNo", ResourceType = typeof(LSMResources.StringResource))]
        public int ItemNo { get; set; }
        [Required]
        [Display(Name = "MaterialReturnDetailViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string LineStoreName { get; set; }
        [Required]
        [Display(Name = "MaterialReturnDetailViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }
        [Required]
        [Display(Name = "MaterialReturnDetailViewModel_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
              , ErrorMessageResourceName = "ValidateStringLength"
              , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
               , ErrorMessageResourceName = "ValidateString"
               , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialLot { get; set; }

        [Required]
        [Display(Name = "MaterialReturnDetailViewModel_SupplierCode", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
              , ErrorMessageResourceName = "ValidateStringLength"
              , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
               , ErrorMessageResourceName = "ValidateString"
               , ErrorMessageResourceType = typeof(StringResource))]
        public string SupplierCode { get; set; }
        [Required]
        [Display(Name = "MaterialReturnDetailViewModel_Qty", ResourceType = typeof(LSMResources.StringResource))]
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

        public MaterialReturn GetMaterialReturn(string key)
        {
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                MethodReturnResult<MaterialReturn> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }
    }

}