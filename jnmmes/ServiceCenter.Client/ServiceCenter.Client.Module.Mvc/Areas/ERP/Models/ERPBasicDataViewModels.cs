
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ERPResources = ServiceCenter.Client.Mvc.Resources.ERP;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.ERP;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.ERP;

namespace ServiceCenter.Client.Mvc.Areas.ERP.Models
{

    public class ERPBasicDataViewModels
    {
        public ERPBasicDataViewModels()
        {
           
        }      
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

    }
    
}