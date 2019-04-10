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

    public class LotOemPackageQueryViewModel
    {
        public LotOemPackageQueryViewModel()
        {
        //    this.Time = DateTime.Now.ToString("yyyy/MM/dd");
        }
        [Display(Name = "LotOemPackageQueryViewModel_Type", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Type { get; set; }

        [Display(Name = "LotOemPackageQueryViewModel_SN", ResourceType = typeof(ZPVMResources.StringResource))]
        public string SN { get; set; }

        [Display(Name = "LotOemPackageQueryViewModel_PNOM", ResourceType = typeof(ZPVMResources.StringResource))]
        public string PNOM { get; set; }

        [Display(Name = "LotOemPackageQueryViewModel_PackageNo", ResourceType = typeof(ZPVMResources.StringResource))]
        public string PackageNo { get; set; }
        //public IEnumerable<SelectListItem> GetTypeList()
        //{
        //    IList<PackageOemDetail> lst = new List<PackageOemDetail>();
        //    PagingConfig cfg = new PagingConfig()
        //    {
        //        IsPaging = false
        //    };
        //    using (PackageOemQueryServiceClient client = new PackageOemQueryServiceClient())
        //    {
        //        MethodReturnResult<IList<PackageOemDetail>> result = client.Get(ref cfg);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            lst = result.Data;
        //        }
        //    }
        //    return from item in lst
        //           select new SelectListItem()
        //           {
        //               Text = item.Key,
        //               Value = item.Key
        //           };
        }
        //public IEnumerable<SelectListItem> GetSNList()
        //{
        //    IList<SN> lst = new List<SN>();
        //    PagingConfig cfg = new PagingConfig()
        //    {
        //        IsPaging = false,

        //    };
        //    using (PackageOemQueryServiceClient client = new PackageOemQueryServiceClient())
        //    {
        //        MethodReturnResult<IList<SN>> result = client.Get(ref cfg);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            lst = result.Data;
        //        }
        //    }
        //    return from item in lst
        //           select new SelectListItem()
        //           {
        //               Text = item.Key,
        //               Value = item.Key
        //           };
        //}
        //public IEnumerable<SelectListItem> GetPNOMList()
        //{

        //    IList<PNOM> lst = new List<PNOM>();
        //    PagingConfig cfg = new PagingConfig()
        //    {
        //        IsPaging = false,

        //    };
        //    using (PackageOemQueryServiceClient client = new PackageOemQueryServiceClient())
        //    {
        //        MethodReturnResult<IList<PNOM>> result = client.Get(ref cfg);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            lst = result.Data;
        //        }
        //    }
        //    return from item in lst
        //           select new SelectListItem()
        //           {
        //               Text = item.Key,
        //               Value = item.Key
        //           };
    //    }



    //}
    public class LotOemPackageViewModel
    {
        public LotOemPackageViewModel()
        {
            //this.Time = string.Format("yy-MM-dd", DateTime.Now);
        }
        [Required]
        [Display(Name = " LotOemPackageViewModel_No", ResourceType = typeof(ZPVMResources.StringResource))]
        public int No { get; set; }

        [Required]
        [Display(Name = " LotOemPackageViewModel_PackageNo", ResourceType = typeof(ZPVMResources.StringResource))]
        public string PALLET_NO { get; set; }

        [Required]
        [Display(Name = " LotOemPackageViewModel_Type", ResourceType = typeof(ZPVMResources.StringResource))]
        public String  Type { get; set; }

        [Required]
        [Display(Name = " LotOemPackageViewModel_SN", ResourceType = typeof(ZPVMResources.StringResource))]
        public String  SN { get; set; }

        [Required]
        [Display(Name = " LotOemPackageViewModel_PMP", ResourceType = typeof(ZPVMResources.StringResource))]
        public double PMP { get; set; }

        [Required]
        [Display(Name = " LotOemPackageViewModel_ISC", ResourceType = typeof(ZPVMResources.StringResource))]
        public double ISC { get; set; }

        [Required]
        [Display(Name = " LotOemPackageViewModel_VOC", ResourceType = typeof(ZPVMResources.StringResource))]
        public double VOC { get; set; }

        [Required]
        [Display(Name = " LotOemPackageViewModel_IMP", ResourceType = typeof(ZPVMResources.StringResource))]
        public double IMP { get; set; }

        [Required]
        [Display(Name = " LotOemPackageViewModel_VMP", ResourceType = typeof(ZPVMResources.StringResource))]
        public double VMP { get; set; }

        [Required]
        [Display(Name = " LotOemPackageViewModel_FF", ResourceType = typeof(ZPVMResources.StringResource))]
        public double FF { get; set; }

        [Required]
        [Display(Name = " LotOemPackageViewModel_PNOM", ResourceType = typeof(ZPVMResources.StringResource))]
        public double PNOM { get; set; }

        [Required]
        [Display(Name = " LotOemPackageViewModel_DL", ResourceType = typeof(ZPVMResources.StringResource))]
        public double CURRENT { get; set; }

        [Display(Name = " LotOemPackageViewModel_Time", ResourceType = typeof(ZPVMResources.StringResource))]
         public DateTime? CreateTime { get; set; }




        //public IEnumerable<SelectListItem> GetTypeList()
        //{
        //    IList<Type> lst = new List<Type>();
        //    PagingConfig cfg = new PagingConfig()
        //    {
        //        IsPaging = false,

        //    };
        //    using (PackageOemQueryServiceClient client = new PackageOemQueryServiceClient())
        //    {
        //        MethodReturnResult<IList<Type>> result = client.Get(ref cfg);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            lst = result.Data;
        //        }
        //    }
        //    return from item in lst
        //           select new SelectListItem()
        //           {
        //               Text = item.Key,
        //               Value = item.Key
        //           };
        //}

        //public IEnumerable<SelectListItem> GetSNList()
        //{
        //    IList<SNLine> lst = new List<SNLine>();
        //    PagingConfig cfg = new PagingConfig()
        //    {
        //        IsPaging = false
        //    };
        //    using (PackageOemQueryServiceClient client = new PackageOemQueryServiceClient())
        //    {
        //        MethodReturnResult<IList<SNLine>> result = client.Get(ref cfg);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            lst = result.Data;
        //        }
        //    }
        //    return from item in lst
        //           select new SelectListItem()
        //           {
        //               Text = item.Key,
        //               Value = item.Key
        //           };
        //}

        //public IEnumerable<SelectListItem> GetPNOMList()
        //{

        //    IList<PNOM> lst = new List<PNOM>();
        //    PagingConfig cfg = new PagingConfig()
        //    {
        //        IsPaging = false,

        //    };
        //    using (PackageOemQueryServiceClient client = new PackageOemQueryServiceClient())
        //    {
        //        MethodReturnResult<IList<PNOM>> result = client.Get(ref cfg);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            lst = result.Data;
        //        }
        //    }
        //    return from item in lst
        //           select new SelectListItem()
        //           {
        //               Text = item.Key,
        //               Value = item.Key
        //           };
        //}

    }
}