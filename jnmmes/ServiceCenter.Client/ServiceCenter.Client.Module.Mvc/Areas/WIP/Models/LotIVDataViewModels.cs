
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.WIP;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.RBAC;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class LotIVDataViewModels
    {

        public LotIVDataViewModels()
        {
        }
        [Required]
        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        //[StringLength(50, MinimumLength = 1
        //        , ErrorMessageResourceName = "ValidateStringLength"
        //        , ErrorMessageResourceType = typeof(StringResource))]
        //[RegularExpression("[^,]+"
        //      , ErrorMessageResourceName = "ValidateString"
        //      , ErrorMessageResourceType = typeof(StringResource))]
        public string LotNumber { get; set; }

        [Display(Name = "读头编号")]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string ScanNo { get; set; }


        [Display(Name = "BinNo")]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string BinNo { get; set; }
    }
}