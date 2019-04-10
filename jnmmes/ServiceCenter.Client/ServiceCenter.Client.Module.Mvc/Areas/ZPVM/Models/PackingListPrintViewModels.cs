
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

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{
    public class PackingListPrintQueryViewModel
    {
        public PackingListPrintQueryViewModel()
        {
            this.IsAutoPackageNo = false;
        }

        [Display(Name = "PackingListPrintQueryViewModel_IsLotNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool IsLotNumber { get; set; }


        [Display(Name = "是否是自动包装号")]
        public bool IsAutoPackageNo { get; set; }

        [Display(Name = "批次号1")]
        public string LotNumber1 { get; set; }


        [Display(Name = "批次号2")]
        public string LotNumber2 { get; set; }


        [Display(Name = "PackingListPrintQueryViewModel_PackageNo", ResourceType = typeof(ZPVMResources.StringResource))]
        public string PackageNo { get; set; }


        [Display(Name = "PackingListPrintQueryViewModel_PackageListType", ResourceType = typeof(ZPVMResources.StringResource))]
        public EnumPackageListType PackageListType { get; set; }

        public IEnumerable<SelectListItem> GetPackageListTypeList()
        {
            IDictionary<EnumPackageListType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumPackageListType>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }
    }
    /// <summary>
    /// 包装清单类型。
    /// </summary>
    public enum EnumPackageListType
    {
        [Display(Name = "EnumPackageListType_Normal", ResourceType = typeof(ZPVMResources.StringResource))]
        Normal=0,
        [Display(Name = "EnumPackageListType_Other", ResourceType = typeof(ZPVMResources.StringResource))]
        Other = 100
    }
}