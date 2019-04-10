
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
    public class ParameterDerivationQueryViewModel
    {
        public ParameterDerivationQueryViewModel()
        {

        }
        [Display(Name = "ParameterDerivationQueryViewModel_DerivedParameterName", ResourceType = typeof(FMMResources.StringResource))]
        public string DerivedParameterName { get; set; }

        [Display(Name = "ParameterDerivationQueryViewModel_ParameterName", ResourceType = typeof(FMMResources.StringResource))]
        public string ParameterName { get; set; }
    }

    public class ParameterDerivationViewModel
    {
        public ParameterDerivationViewModel()
        {
            this.EditTime = DateTime.Now;
            this.ItemNo = 1;
        }

        [Required]
        [Display(Name = "ParameterDerivationViewModel_DerivedParameterName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string DerivedParameterName { get; set; }

        [Required]
        [Display(Name = "ParameterDerivationViewModel_ParameterName", ResourceType = typeof(FMMResources.StringResource))]
        public string ParameterName { get; set; }

        [Required]
        [Display(Name = "ParameterDerivationViewModel_ItemNo", ResourceType = typeof(FMMResources.StringResource))]
        public int ItemNo { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }
        public IEnumerable<SelectListItem> GetParameterName(string derivedParameterName)
        {
            using (ParameterServiceClient client = new ParameterServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key!='{0}' AND DataType!='{1}'"
                                         , derivedParameterName
                                         , Convert.ToInt32(EnumDataType.String))
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