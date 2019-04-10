
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.ZPVC;
using ZPVCResources = ServiceCenter.Client.Mvc.Resources.ZPVC;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.ZPVC;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.ZPVC.Models
{
    public class EfficiencyConfigurationQueryViewModel
    {
        public EfficiencyConfigurationQueryViewModel()
        {

        }
        [Display(Name = "EfficiencyConfigurationQueryViewModel_Group", ResourceType = typeof(ZPVCResources.StringResource))]
        public string Group { get; set; }
        [Display(Name = "EfficiencyConfigurationQueryViewModel_Code", ResourceType = typeof(ZPVCResources.StringResource))]
        public string Code { get; set; }

    }

    public class EfficiencyConfigurationViewModel
    {
        public EfficiencyConfigurationViewModel()
        {
            this.IsUsed = true;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "EfficiencyConfigurationViewModel_Group", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Group { get; set; }

        [Required]
        [Display(Name = "EfficiencyConfigurationViewModel_Code", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "EfficiencyConfigurationViewModel_EffiCode", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                    , ErrorMessageResourceName = "ValidateStringLength"
                    , ErrorMessageResourceType = typeof(StringResource))]
        public string EffiCode { get; set; }

        [Required]
        [Display(Name = "EfficiencyConfigurationViewModel_EffiName", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                    , ErrorMessageResourceName = "ValidateStringLength"
                    , ErrorMessageResourceType = typeof(StringResource))]
        public string EffiName { get; set; }


        [Required]
        [Display(Name = "EfficiencyConfigurationViewModel_Lower", ResourceType = typeof(ZPVCResources.StringResource))]
        public double? Lower { get; set; }


        [Display(Name = "EfficiencyConfigurationViewModel_Upper", ResourceType = typeof(ZPVCResources.StringResource))]
        public double? Upper { get; set; }

        
        [Display(Name = "EfficiencyConfigurationViewModel_Grade", ResourceType = typeof(ZPVCResources.StringResource))]
        public string Grade { get; set; }

        [Display(Name = "EfficiencyConfigurationViewModel_Color", ResourceType = typeof(ZPVCResources.StringResource))]
        public string Color { get; set; }

        [Display(Name = "EfficiencyConfigurationViewModel_MaterialCode", ResourceType = typeof(ZPVCResources.StringResource))]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 是否可用。
        /// </summary>
        [Display(Name = "EfficiencyConfigurationViewModel_IsUsed", ResourceType = typeof(ZPVCResources.StringResource))]
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


        public IEnumerable<SelectListItem> GetGradeList()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='Grade' AND Key.AttributeName='VALUE'"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstValues = result.Data;
                }
            }
            return from item in lstValues
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Value
                   };
        }

        public IEnumerable<SelectListItem> GetColorList()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='Color' AND Key.AttributeName='VALUE'"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstValues = result.Data;
                }
            }
            return from item in lstValues
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Value
                   };
        }
    }
}