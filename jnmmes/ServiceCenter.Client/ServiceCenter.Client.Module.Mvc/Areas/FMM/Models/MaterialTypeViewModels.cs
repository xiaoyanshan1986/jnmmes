
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
    public class MaterialTypeQueryViewModel
    {
        public MaterialTypeQueryViewModel()
        {

        }

        [Display(Name = "MaterialTypeQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

    }

    public class MaterialTypeViewModel
    {
        public MaterialTypeViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;

        }

        [Required]
        [Display(Name = "MaterialTypeViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(20, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }


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