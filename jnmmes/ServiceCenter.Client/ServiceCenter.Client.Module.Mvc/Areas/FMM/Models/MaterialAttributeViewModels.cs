
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
    public class MaterialAttributeQueryViewModel
    {
        public MaterialAttributeQueryViewModel()
        {

        }
        [Display(Name = "MaterialAttributeQueryViewModel_MaterialCode", ResourceType = typeof(FMMResources.StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "MaterialAttributeQueryViewModel_AttributeName", ResourceType = typeof(FMMResources.StringResource))]
        public string AttributeName { get; set; }
    }

    public class MaterialAttributeViewModel
    {
        public MaterialAttributeViewModel()
        {
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "MaterialAttributeViewModel_MaterialCode", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        [Required]
        [Display(Name = "MaterialAttributeViewModel_AttributeName", ResourceType = typeof(FMMResources.StringResource))]
        public string AttributeName { get; set; }

        [Required]
        [Display(Name = "MaterialAttributeViewModel_Value", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Value { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        public IEnumerable<SelectListItem> GetAttributeName()
        {
            //using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            //{
            //    PagingConfig cfg = new PagingConfig()
            //    {
            //        IsPaging = false,
            //        Where = string.Format("Key.CategoryName='{0}'", "MaterialAttribute")
            //    };

            //    MethodReturnResult<IList<BaseAttribute>> result = client.Get(ref cfg);
            //    if (result.Code <= 0)
            //    {
            //        IEnumerable<SelectListItem> lst = from item in result.Data
            //                                          select new SelectListItem()
            //                                          {
            //                                              Text = item.Key.AttributeName,
            //                                              Value = item.Key.AttributeName
            //                                          };
            //        return lst;
            //    }
            //}
            //return new List<SelectListItem>();

            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='{0}'", "WorkOrderAttribute")
                };

                MethodReturnResult<IList<BaseAttribute>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = string.Format("{0}({1})", item.Key.AttributeName, item.Description),
                                                          Value = item.Key.AttributeName
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }
    }
}