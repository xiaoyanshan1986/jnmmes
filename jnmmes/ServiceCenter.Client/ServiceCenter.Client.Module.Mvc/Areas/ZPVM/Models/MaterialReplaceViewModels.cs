
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
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{
    public class MaterialReplaceViewModel 
    {
        public Supplier GetSupplier(string key)
        {
            using (SupplierServiceClient client = new SupplierServiceClient())
            {
                MethodReturnResult<Supplier> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }

        public MaterialReplaceViewModel()
        {

        }

        [Required]
        [Display(Name = "产品编码")]
        public string ProductCode { get; set; }

        [Required]
        [Display(Name = "工单号")]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "原物料编码")]
        public string OldMaterialCode { get; set; }

        [Required]
        [Display(Name = "原物料供应商")]
        public string OldMaterialSupplier { get; set; }

        [Required]
        [Display(Name = "现物料编码")]
        public string NewMaterialCode { get; set; }

        [Required]
        [Display(Name = "现物料供应商")]
        public string NewMaterialSupplier { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }
        
    }
    public class MaterialQueryReplaceViewModel
    {
        public MaterialQueryReplaceViewModel()
        {

        }

        [Display(Name = "产品编码")]
        public string ProductCode { get; set; }

        [Display(Name = "工单号")]
        public string OrderNumber { get; set; }

        [Display(Name = "原物料编码")]
        public string OldMaterialCode { get; set; }

        [Display(Name = "原物料供应商")]
        public string OldMaterialSupplier { get; set; }

        [Display(Name = "现物料编码")]
        public string NewMaterialCode { get; set; }

        [Display(Name = "现物料供应商")]
        public string NewMaterialSupplier { get; set; }
    }

}