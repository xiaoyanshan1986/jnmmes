
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.PPM;
using PPMResources = ServiceCenter.Client.Mvc.Resources.PPM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.PPM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Models
{
    public class WorkOrderProductQueryViewModel
    {
        public WorkOrderProductQueryViewModel()
        {

        }
        [Display(Name = "WorkOrderProductQueryViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "WorkOrderProductQueryViewModel_MaterialCode", ResourceType = typeof(PPMResources.StringResource))]
        public string MaterialCode { get; set; }
    }

    public class WorkOrderProductViewModel
    {
        public WorkOrderProductViewModel()
        {
            this.EditTime = DateTime.Now;
            this.CreateTime = DateTime.Now;
            this.ItemNo = 1;
        }

        [Required]
        [Display(Name = "WorkOrderProductViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "WorkOrderProductQueryViewModel_ItemNo", ResourceType = typeof(PPMResources.StringResource))]
        [Range(0, 65536
            , ErrorMessageResourceName = "ValidateRange"
            , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int ItemNo { get; set; }

        [Required]
        [Display(Name = "WorkOrderProductViewModel_MaterialCode", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        [Required]
        [Display(Name = "WorkOrderProductQueryViewModel_IsMain", ResourceType = typeof(PPMResources.StringResource))]
        public bool IsMain { get; set; }
        
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