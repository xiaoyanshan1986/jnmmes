
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
    public class MaterialBOMQueryViewModel
    {
        public MaterialBOMQueryViewModel()
        {

        }
        [Display(Name = "MaterialBOMQueryViewModel_MaterialCode", ResourceType = typeof(FMMResources.StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "MaterialBOMQueryViewModel_RawMaterialCode", ResourceType = typeof(FMMResources.StringResource))]
        public string RawMaterialCode { get; set; }
    }

    public class MaterialBOMViewModel
    {
        public MaterialBOMViewModel()
        {
            this.EditTime = DateTime.Now;
            this.CreateTime = DateTime.Now;
            this.Qty = 1;
        }

        [Required]
        [Display(Name = "MaterialBOMViewModel_MaterialCode", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        [Required]
        [Display(Name = "MaterialBOMViewModel_ItemNo", ResourceType = typeof(FMMResources.StringResource))]
        [Range(1, 65536
            , ErrorMessageResourceName = "ValidateRange"
            , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int ItemNo { get; set; }

        [Required]
        [Display(Name = "MaterialBOMViewModel_RawMaterialCode", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RawMaterialCode { get; set; }

        [Required]
        [Display(Name = "MaterialBOMViewModel_Qty", ResourceType = typeof(FMMResources.StringResource))]
        [Range(0, 2147483648
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        public double Qty { get; set; }

        [Display(Name = "MaterialBOMViewModel_MaterialUnit", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(30, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialUnit { get; set; }

        [Display(Name = "MaterialBOMViewModel_WorkCenter", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 0, ErrorMessage = null
             , ErrorMessageResourceName = "ValidateMaxStringLength"
             , ErrorMessageResourceType = typeof(StringResource))]
        public string WorkCenter { get; set; }

        [Display(Name = "MaterialBOMViewModel_StoreLocation", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 0, ErrorMessage = null
             , ErrorMessageResourceName = "ValidateMaxStringLength"
             , ErrorMessageResourceType = typeof(StringResource))]
        public string StoreLocation { get; set; }

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


    }
}