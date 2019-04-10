
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.ZPVM;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.ZPVM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{
    public class SupplierToManufacturerViewModel 
    {
        public SupplierToManufacturerViewModel()
        {

        }

        [Required]
        [Display(Name = "物料编码")]
        public string MaterialCode { get; set; }

        [Required]
        [Display(Name = "生产工单")]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "供应商代码")]
        public string SupplierCode { get; set; }

        [Required]
        [Display(Name = "生产厂商代码")]
        public string ManufacturerCode { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }
          
    }
    public class SupplierToManufacturerQueryViewModel
    {
        public SupplierToManufacturerQueryViewModel()
        {

        }

        [Display(Name = "物料编码")]
        public string MaterialCode { get; set; }

        [Display(Name = "生产工单")]
        public string OrderNumber { get; set; }

        [Display(Name = "供应商代码")]
        public string SupplierCode { get; set; }

        [Display(Name = "生产厂商代码")]
        public string ManufacturerCode { get; set; }
    }

}