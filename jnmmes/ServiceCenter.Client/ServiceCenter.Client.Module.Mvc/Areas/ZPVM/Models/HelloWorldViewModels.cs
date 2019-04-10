using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.WIP;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{

    public class HelloWorldViewModel 
    {
        public HelloWorldViewModel()
        {
        }


        [Display(Name = "HelloWorldViewModel_Name", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Name { get; set; }
    }

}