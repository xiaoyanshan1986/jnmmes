
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
    public class RuleGradeQueryViewModel
    {
        public RuleGradeQueryViewModel()
        {

        }

        [Display(Name = "RuleGradeQueryViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Code { get; set; }

    }

    public class RuleGradeViewModel
    {
        public RuleGradeViewModel()
        {
            this.IsUsed = true;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "RuleGradeViewModel_Code", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "RuleGradeViewModel_Grade", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Grade { get; set; }

        [Required]
        [Display(Name = "RuleGradeViewModel_ItemNo", ResourceType = typeof(ZPVMResources.StringResource))]
        [Range(1, 65536
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int? ItemNo { get; set; }

        [Required]
        [Display(Name = "RuleGradeViewModel_MixPowerset", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool MixPowerset { get; set; }

        [Required]
        [Display(Name = "RuleGradeViewModel_MixSubPowerset", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool MixSubPowerset { get; set; }

        [Required]
        [Display(Name = "RuleGradeViewModel_MixColor", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool MixColor { get; set; }

        [Required]
        [Display(Name = "RuleGradeViewModel_PackageGroup", ResourceType = typeof(ZPVMResources.StringResource))]
        public string PackageGroup { get; set; }

        /// <summary>
        /// 是否可用。
        /// </summary>
        [Display(Name = "RuleGradeViewModel_IsUsed", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool IsUsed { get; set; }


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
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = "Key.CategoryName='Grade' AND Key.AttributeName='VALUE'",
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Value,
                                                          Value = item.Value
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }

    }
}