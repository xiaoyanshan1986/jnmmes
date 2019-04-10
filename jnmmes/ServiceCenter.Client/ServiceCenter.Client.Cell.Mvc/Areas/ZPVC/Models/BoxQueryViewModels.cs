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
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.MES.Service.Client.ZPVC;
using ZPVCResources = ServiceCenter.Client.Mvc.Resources.ZPVC;

namespace ServiceCenter.Client.Mvc.Areas.ZPVC.Models
{

    public class BoxQueryViewModel 
    {
        public BoxQueryViewModel()
        {
            this.StartCreateTime = DateTime.Now.ToString("yyyy/MM/dd");
            this.EndCreateTime = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd");
        }
        [Display(Name = "BoxViewModel_BoxNo", ResourceType = typeof(ZPVCResources.StringResource))]
        public string BoxNo { get; set; }

        [Display(Name = "BoxViewModel_BoxNo", ResourceType = typeof(ZPVCResources.StringResource))]
        public string BoxNo1 { get; set; }

        [Display(Name = "PackageNo", ResourceType = typeof(ZPVCResources.StringResource))]
        public string PackageNo { get; set; }

        [Display(Name = "PackageNo", ResourceType = typeof(ZPVCResources.StringResource))]
        public string PackageNo1 { get; set; }

        [Display(Name = "BoxQueryViewModel_StartCreateTime", ResourceType = typeof(ZPVCResources.StringResource))]
        public string StartCreateTime { get; set; }

        [Display(Name = "BoxQueryViewModel_EndCreateTime", ResourceType = typeof(ZPVCResources.StringResource))]
        public string EndCreateTime { get; set; }
    }


}