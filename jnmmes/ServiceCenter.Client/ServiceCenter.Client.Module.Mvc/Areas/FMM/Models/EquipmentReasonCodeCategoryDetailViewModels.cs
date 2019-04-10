
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
    public class EquipmentReasonCodeCategoryDetailQueryViewModel
    {
        public EquipmentReasonCodeCategoryDetailQueryViewModel()
        {

        }
        [Display(Name = "EquipmentReasonCodeCategoryDetailQueryViewModel_ReasonCodeCategoryName", ResourceType = typeof(FMMResources.StringResource))]
        public string ReasonCodeCategoryName { get; set; }

        [Display(Name = "EquipmentReasonCodeCategoryDetailQueryViewModel_ReasonCodeName", ResourceType = typeof(FMMResources.StringResource))]
        public string ReasonCodeName { get; set; }
    }

    public class EquipmentReasonCodeCategoryDetailViewModel
    {
        public EquipmentReasonCodeCategoryDetailViewModel()
        {
            this.EditTime = DateTime.Now;
            this.ItemNo = 1;
        }

        [Required]
        [Display(Name = "EquipmentReasonCodeCategoryDetailViewModel_ReasonCodeCategoryName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ReasonCodeCategoryName { get; set; }

        [Required]
        [Display(Name = "EquipmentReasonCodeCategoryDetailViewModel_ReasonCodeName", ResourceType = typeof(FMMResources.StringResource))]
        public string ReasonCodeName { get; set; }

        [Required]
        [Display(Name = "EquipmentReasonCodeCategoryDetailViewModel_ItemNo", ResourceType = typeof(FMMResources.StringResource))]
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

        public IEnumerable<SelectListItem> GetEquipmentReasonCodeName(EnumEquipmentReasonCodeType type)
        {
            using (EquipmentReasonCodeServiceClient client = new EquipmentReasonCodeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Type='{0}'", Convert.ToInt32(type))
                };

                MethodReturnResult<IList<EquipmentReasonCode>> result = client.Get(ref cfg);
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