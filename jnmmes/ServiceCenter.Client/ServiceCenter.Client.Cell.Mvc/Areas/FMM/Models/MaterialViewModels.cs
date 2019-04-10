
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

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class MaterialQueryViewModel
    {
        public MaterialQueryViewModel()
        {
        }

        [Display(Name = "MaterialQueryViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public string Type { get; set; }

        [Display(Name = "MaterialQueryViewModel_Code", ResourceType = typeof(FMMResources.StringResource))]
        public string Code { get; set; }

        [Display(Name = "MaterialQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

    }

    public class MaterialViewModel
    {
        public MaterialViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.Status = EnumObjectStatus.Available;
        }

        [Required]
        [Display(Name = "MaterialViewModel_Code", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "MaterialViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }


        [Display(Name = "MaterialViewModel_Spec", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                   , ErrorMessageResourceName = "ValidateMaxStringLength"
                   , ErrorMessageResourceType = typeof(StringResource))]
        public string Spec { get; set; }

        [Display(Name = "MaterialViewModel_Unit", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(10, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Unit { get; set; }

        [Display(Name = "MaterialViewModel_BarCode", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string BarCode { get; set; }

        [Display(Name = "MaterialViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public string Type { get; set; }

        [Display(Name = "MaterialViewModel_Class", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Class { get; set; }

        [Display(Name = "MaterialViewModel_IsRaw", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsRaw { get; set; }

        [Display(Name = "MaterialViewModel_IsProduct", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsProduct { get; set; }

        /// <summary>
        /// 每批主材料数量。
        /// </summary>
        [Required]
        [Display(Name = "MaterialTypeViewModel_MainRawQtyPerLot", ResourceType = typeof(FMMResources.StringResource))]
        public double MainRawQtyPerLot { get; set; }
        /// <summary>
        /// 每批产品数量。
        /// </summary>
        [Required]
        [Display(Name = "MaterialTypeViewModel_MainProductQtyPerLot", ResourceType = typeof(FMMResources.StringResource))]
        public double MainProductQtyPerLot { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }


        [Display(Name = "Status", ResourceType = typeof(StringResource))]
        public EnumObjectStatus Status { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }


        public IEnumerable<SelectListItem> GetMaterialTypeList()
        {
            using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<MaterialType>> result = client.Get(ref cfg);
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
        public IEnumerable<SelectListItem> GetObjectStatusList()
        {
            IDictionary<EnumObjectStatus, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumObjectStatus>();

            return  from item in dic
                    select new SelectListItem()
                    {
                        Text = item.Value,
                        Value = Convert.ToString(item.Key)
                    };;
        }


    }
}