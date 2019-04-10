
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.EDC;
using EDCResources = ServiceCenter.Client.Mvc.Resources.EDC;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.EDC;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Models
{
    public class CategoryDetailQueryViewModel
    {
        public CategoryDetailQueryViewModel()
        {

        }
        [Display(Name = "CategoryDetailQueryViewModel_CategoryName", ResourceType = typeof(EDCResources.StringResource))]
        public string CategoryName { get; set; }

        [Display(Name = "CategoryDetailQueryViewModel_ParameterName", ResourceType = typeof(EDCResources.StringResource))]
        public string ParameterName { get; set; }
    }

    public class CategoryDetailViewModel
    {
        public CategoryDetailViewModel()
        {
            this.EditTime = DateTime.Now;
            this.ItemNo = 1;
        }

        [Required]
        [Display(Name = "CategoryDetailViewModel_CategoryName", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string CategoryName { get; set; }

        [Required]
        [Display(Name = "CategoryDetailViewModel_ParameterName", ResourceType = typeof(EDCResources.StringResource))]
        public string ParameterName { get; set; }

        [Required]
        [Display(Name = "CategoryDetailViewModel_ItemNo", ResourceType = typeof(EDCResources.StringResource))]
        [Range(0, 65536
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int ItemNo { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        public IEnumerable<SelectListItem> GetParameterName()
        {
            using (ParameterServiceClient client = new ParameterServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Type='{0}'", Convert.ToInt32(EnumParameterType.EDC))
                };

                MethodReturnResult<IList<Parameter>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }
    }
}