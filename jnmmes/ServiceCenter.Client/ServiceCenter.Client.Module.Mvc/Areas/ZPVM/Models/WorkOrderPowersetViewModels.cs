
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
    public class WorkOrderPowersetQueryViewModel
    {
        public WorkOrderPowersetQueryViewModel()
        {

        }

        [Display(Name = "WorkOrderRuleQueryViewModel_OrderNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "WorkOrderRuleQueryViewModel_MaterialCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string MaterialCode { get; set; }
    }

    public class WorkOrderPowersetViewModel
    {
        public WorkOrderPowersetViewModel()
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
        [Display(Name = "PowersetViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Display(Name = "分档")]
        public string KeyCode { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_ItemNo", ResourceType = typeof(ZPVMResources.StringResource))]
        public int? ItemNo { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_Name", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_MinValue", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? MinValue { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_MaxValue", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? MaxValue { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_PowerName", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                       , ErrorMessageResourceName = "ValidateStringLength"
                       , ErrorMessageResourceType = typeof(StringResource))]
        public string PowerName { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_StandardPower", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? StandardPower { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_StandardIsc", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? StandardIsc { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_StandardVoc", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? StandardVoc { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_StandardIPM", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? StandardIPM { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_StandardVPM", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? StandardVPM { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_StandardFuse", ResourceType = typeof(ZPVMResources.StringResource))]
        public double? StandardFuse { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_PowerDifference", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                       , ErrorMessageResourceName = "ValidateStringLength"
                       , ErrorMessageResourceType = typeof(StringResource))]
        public string PowerDifference { get; set; }

        [Required]
        [Display(Name = "PowersetViewModel_SubWay", ResourceType = typeof(ZPVMResources.StringResource))]
        public EnumPowersetSubWay SubWay { get; set; }
        
        [Display(Name = "PowersetViewModel_ArticleNo", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                       , ErrorMessageResourceName = "ValidateStringLength"
                       , ErrorMessageResourceType = typeof(StringResource))]
        public string ArticleNo { get; set; }


        /// <summary>
        /// 是否混花色。
        /// </summary>
        [Display(Name = "PowersetViewModel_MixColor", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool MixColor { get; set; }

        /// <summary>
        /// 是否可用。
        /// </summary>
        [Display(Name = "PowersetViewModel_IsUsed", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool IsUsed { get; set; }


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

        public IEnumerable<SelectListItem> GetPowersetSubWayList()
        {
            IDictionary<EnumPowersetSubWay, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumPowersetSubWay>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }


    }
}