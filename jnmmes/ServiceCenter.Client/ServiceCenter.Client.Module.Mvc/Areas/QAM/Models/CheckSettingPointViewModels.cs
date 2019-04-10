
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.QAM;
using QAMResources = ServiceCenter.Client.Mvc.Resources.QAM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.QAM;
using System.Web.Mvc;
using ServiceCenter.Common;

namespace ServiceCenter.Client.Mvc.Areas.QAM.Models
{
    public class CheckSettingPointQueryViewModel
    {
        public CheckSettingPointQueryViewModel()
        {

        }
        [Display(Name = "CheckSettingPointQueryViewModel_CheckSettingKey", ResourceType = typeof(QAMResources.StringResource))]
        public string CheckSettingKey { get; set; }

        [Display(Name = "CheckSettingPointQueryViewModel_GroupName", ResourceType = typeof(QAMResources.StringResource))]
        public string GroupName { get; set; }

        [Display(Name = "CheckSettingPointQueryViewModel_CategoryName", ResourceType = typeof(QAMResources.StringResource))]
        public string CategoryName { get; set; }

        [Display(Name = "CheckSettingPointQueryViewModel_CheckPlanName", ResourceType = typeof(QAMResources.StringResource))]
        public string CheckPlanName { get; set; }
    }

    public class CheckSettingPointViewModel
    {
        public CheckSettingPointViewModel()
        {
            this.EditTime = DateTime.Now;
            this.CreateTime = DateTime.Now;
            this.Status = EnumObjectStatus.Available;
        }

        [Required]
        [Display(Name = "CheckSettingPointViewModel_CheckSettingKey", ResourceType = typeof(QAMResources.StringResource))]
        public string CheckSettingKey { get; set; }


        [Required]
        [Display(Name = "CheckSettingPointViewModel_ItemNo", ResourceType = typeof(QAMResources.StringResource))]
        [Range(0, 65536
              , ErrorMessageResourceName = "ValidateRange"
              , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int ItemNo { get; set; }


        [Required]
        [Display(Name = "CheckSettingPointViewModel_CategoryName", ResourceType = typeof(QAMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string CategoryName { get; set; }

        [Required]
        [Display(Name = "CheckSettingPointViewModel_CheckPlanName", ResourceType = typeof(QAMResources.StringResource))]
        public string CheckPlanName { get; set; }


        [Display(Name = "Status", ResourceType = typeof(StringResource))]
        public EnumObjectStatus Status { get; set; }

        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        public IEnumerable<SelectListItem> GetObjectStatusList()
        {
            IDictionary<EnumObjectStatus, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumObjectStatus>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }
        public IEnumerable<SelectListItem> GetCheckPlanList()
        {
            using (CheckPlanServiceClient client = new CheckPlanServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<CheckPlan>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return from item in result.Data
                           select new SelectListItem()
                           {
                               Text = item.Key,
                               Value = item.Key
                           };
                }
            }
            return new List<SelectListItem>();
        }

        public IEnumerable<SelectListItem> GetCategoryList()
        {
            using (CheckCategoryServiceClient client = new CheckCategoryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<CheckCategory>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return from item in result.Data
                           select new SelectListItem()
                           {
                               Text = item.Key,
                               Value = item.Key
                           };
                }
            }
            return new List<SelectListItem>();
        }
    }
}