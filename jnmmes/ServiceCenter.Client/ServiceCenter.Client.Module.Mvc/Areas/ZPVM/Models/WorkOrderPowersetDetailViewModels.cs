
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.ZPVM;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.ZPVM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{
    public class WorkOrderPowersetDetailQueryViewModel
    {
        public WorkOrderPowersetDetailQueryViewModel()
        {

        }
        [Display(Name = "WorkOrderRuleQueryViewModel_OrderNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "WorkOrderRuleQueryViewModel_MaterialCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "PowersetDetailQueryViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Code { get; set; }

        [Display(Name = "PowersetDetailQueryViewModel_ItemNo", ResourceType = typeof(ZPVMResources.StringResource))]
        public int ItemNo { get; set; }
    }

    public class WorkOrderPowersetDetailViewModel
    {
        public WorkOrderPowersetDetailViewModel()
        {
            this.IsUsed = true;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "WorkOrderRuleViewModel_OrderNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "WorkOrderRuleViewModel_MaterialCode", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        [Required]
        [Display(Name = "PowersetDetailViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "PowersetDetailViewModel_ItemNo", ResourceType = typeof(ZPVMResources.StringResource))]
        public int ItemNo { get; set; }

        [Required]
        [Display(Name = "PowersetDetailViewModel_SubCode", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string SubCode { get; set; }

        [Required]
        [Display(Name = "PowersetDetailViewModel_SubName", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string SubName { get; set; }

        [Required]
        [Display(Name = "PowersetDetailViewModel_MinValue", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? MinValue { get; set; }

        [Required]
        [Display(Name = "PowersetDetailViewModel_MaxValue", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? MaxValue { get; set; }

        [Display(Name = "PowersetDetailViewModel_IsDeletePicture", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool IsDeletePicture { get; set; }

        [Display(Name = "PowersetDetailViewModel_Picture", ResourceType = typeof(ZPVMResources.StringResource))]
        [RegularExpression(".+(\\.(jpg|gif|jpeg|bmp|png))"
              , ErrorMessageResourceName = "ValidateImageFileFormat"
              , ErrorMessageResourceType = typeof(StringResource))]
        public virtual HttpPostedFileBase Picture { get; set; }

        /// <summary>
        /// 是否可用。
        /// </summary>
        [Display(Name = "PowersetDetailViewModel_IsUsed", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool IsUsed { get; set; }


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