
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
    public class MaterialChestParameterQueryViewModel
    {
        public MaterialChestParameterQueryViewModel()
        {
            
        }

        /// <summary>
        /// 产品编码
        /// </summary>
        [Display(Name = "MaterialChestParameterViewModel_ProductCode", ResourceType = typeof(FMMResources.StringResource))]
        public string ProductCode { get; set; }

        ///// <summary>
        ///// 颜色是否不可混
        ///// </summary>
        //[Display(Name = "MaterialChestParameterViewModel_ColorLimit", ResourceType = typeof(FMMResources.StringResource))]
        //public bool ColorLimit { get; set; }
        
        ///// <summary>
        ///// 等级是否不可混
        ///// </summary>
        //[Display(Name = "MaterialChestParameterViewModel_GradeLimit", ResourceType = typeof(FMMResources.StringResource))]
        //public string GradeLimit { get; set; }

        ///// <summary>
        ///// 功率是否不可混
        ///// </summary>
        //[Display(Name = "MaterialChestParameterViewModel_PowerLimit", ResourceType = typeof(FMMResources.StringResource))]
        //public string PowerLimit { get; set; }

        ///// <summary>
        ///// 电流档是否不可混
        ///// </summary>
        //[Display(Name = "MaterialChestParameterViewModel_IscLimit", ResourceType = typeof(FMMResources.StringResource))]
        //public string IscLimit { get; set; }      
    }

    public class MaterialChestParameterViewModel
    {
        public MaterialChestParameterViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        /// <summary>
        /// 产品编码
        /// </summary>
        [Required]
        [Display(Name = "MaterialChestParameterViewModel_ProductCode", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string ProductCode { get; set; }

        /// <summary>
        /// 颜色是否不可混
        /// </summary>
        [Required]
        [Display(Name = "MaterialChestParameterViewModel_ColorLimit", ResourceType = typeof(FMMResources.StringResource))]
        public bool ColorLimit { get; set; }

        /// <summary>
        /// 等级是否不可混
        /// </summary>
        [Required]
        [Display(Name = "MaterialChestParameterViewModel_GradeLimit", ResourceType = typeof(FMMResources.StringResource))]
        public bool GradeLimit { get; set; }

        /// <summary>
        /// 功率是否不可混
        /// </summary>
        [Required]
        [Display(Name = "MaterialChestParameterViewModel_PowerLimit", ResourceType = typeof(FMMResources.StringResource))]
        public bool PowerLimit { get; set; }

        /// <summary>
        /// 电流档是否不可混
        /// </summary>
        [Required]
        [Display(Name = "MaterialChestParameterViewModel_IscLimit", ResourceType = typeof(FMMResources.StringResource))]
        public bool IscLimit { get; set; }        

        /// <summary>
        /// 工单是否不可混
        /// </summary>
        [Required]
        [Display(Name = "MaterialChestParameterViewModel_OrderNumberLimit", ResourceType = typeof(FMMResources.StringResource))]
        public bool OrderNumberLimit { get; set; }

        /// <summary>
        /// 尾柜产品编码是否不可混
        /// </summary>
        [Required]
        [Display(Name = "MaterialChestParameterViewModel_LastChestMaterialLimit", ResourceType = typeof(FMMResources.StringResource))]
        public bool LastChestMaterialLimit { get; set; }

        /// <summary>
        /// 满柜数量。
        /// </summary>
        [Required]
        [Display(Name = "MaterialChestParameterViewModel_FullChestQty", ResourceType = typeof(FMMResources.StringResource))]
        public int FullChestQty { get; set; }

        /// <summary>
        /// 柜最大满包数量。
        /// </summary>
        [Required]
        [Display(Name = "柜最大满包数量")]
        public int InChestFullPackageQty { get; set; }

        /// <summary>
        /// 包装能否入柜。
        /// </summary>
        [Required]
        [Display(Name = "MaterialChestParameterViewModel_IsPackagedChest", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsPackagedChest { get; set; }

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