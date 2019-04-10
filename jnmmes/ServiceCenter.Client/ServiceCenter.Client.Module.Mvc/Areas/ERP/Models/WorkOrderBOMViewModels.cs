
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.PPM;
using PPMResources = ServiceCenter.Client.Mvc.Resources.PPM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.PPM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.ERP.Models
{
    public class WorkOrderBOMQueryViewModel
    {
        public WorkOrderBOMQueryViewModel()
        {

        }
        [Display(Name = "WorkOrderBOMQueryViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "WorkOrderBOMQueryViewModel_MaterialCode", ResourceType = typeof(PPMResources.StringResource))]
        public string MaterialCode { get; set; }
    }

    public class WorkOrderBOMViewModel
    {
        public WorkOrderBOMViewModel()
        {
            this.EditTime = DateTime.Now;
            this.CreateTime = DateTime.Now;
            this.Qty = 1;
        }

        [Required]
        [Display(Name = "WorkOrderBOMViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "WorkOrderBOMViewModel_ItemNo", ResourceType = typeof(PPMResources.StringResource))]
        [Range(1, 65536
            , ErrorMessageResourceName = "ValidateRange"
            , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int ItemNo { get; set; }

        [Required]
        [Display(Name = "WorkOrderBOMViewModel_RowNo", ResourceType = typeof(PPMResources.StringResource))]
        [Range(1, 65536
            , ErrorMessageResourceName = "ValidateRange"
            , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int RowNo { get; set; }

        [Required]
        [Display(Name = "WorkOrderBOMViewModel_MaterialCode", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        public string MaterialName { get; set; }

        [Required]
        [Display(Name = "WorkOrderBOMViewModel_Qty", ResourceType = typeof(PPMResources.StringResource))]
        [Range(0, 2147483648
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        public decimal Qty { get; set; }

        [Display(Name = "WorkOrderBOMViewModel_MaterialUnit", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(30, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialUnit { get; set; }

        [Display(Name = "WorkOrderBOMViewModel_WorkCenter", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 0, ErrorMessage = null
             , ErrorMessageResourceName = "ValidateMaxStringLength"
             , ErrorMessageResourceType = typeof(StringResource))]
        public string WorkCenter { get; set; }

        [Display(Name = "WorkOrderBOMViewModel_StoreLocation", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 0, ErrorMessage = null
             , ErrorMessageResourceName = "ValidateMaxStringLength"
             , ErrorMessageResourceType = typeof(StringResource))]
        public string StoreLocation { get; set; }

        /// <summary> 最小扣料单位 </summary>
        [Display(Name = "WorkOrderBOMViewModel_MinUnit", ResourceType = typeof(PPMResources.StringResource))]
        public decimal MinUnit { get; set; }

        /// <summary> 可替换物料 </summary>
        [Display(Name = "WorkOrderBOMViewModel_ReplaceMaterial", ResourceType = typeof(PPMResources.StringResource))]
        public string ReplaceMaterial { get; set; }

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