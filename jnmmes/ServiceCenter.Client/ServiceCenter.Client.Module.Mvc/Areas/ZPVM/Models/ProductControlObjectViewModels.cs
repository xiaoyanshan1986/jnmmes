
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
    public class ProductControlObjectQueryViewModel
    {
        public ProductControlObjectQueryViewModel()
        {

        }

        [Display(Name = "ProductControlObjectQueryViewModel_ProductCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string ProductCode { get; set; }

        [Display(Name = "ProductControlObjectQueryViewModel_CellEff", ResourceType = typeof(ZPVMResources.StringResource))]
        public string CellEff { get; set; }

        [Display(Name = "ProductControlObjectQueryViewModel_SupplierCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string SupplierCode { get; set; }

        [Display(Name = "ProductControlObjectViewModel_SupplierName", ResourceType = typeof(ZPVMResources.StringResource))]
        public string SupplierName { get; set; }

        [Display(Name = "ProductControlObjectViewModel_ProductName", ResourceType = typeof(ZPVMResources.StringResource))]
        public string ProductName { get; set; }

    }

    public class ProductControlObjectViewModel
    {
        public ProductControlObjectViewModel()
        {
            this.IsUsed = true;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "ProductControlObjectViewModel_ProductCode", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ProductCode { get; set; }

        [Required]
        [Display(Name = "ProductControlObjectViewModel_ProductName", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ProductName { get; set; }

        [Required]
        [Display(Name = "ProductControlObjectViewModel_CellEff", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string CellEff { get; set; }

        [Required]
        [Display(Name = "ProductControlObjectViewModel_SupplierCode", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string SupplierCode { get; set; }

        [Required]
        [Display(Name = "ProductControlObjectViewModel_SupplierName", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string SupplierName { get; set; }

        [Required]
        [Display(Name = "ProductControlObjectViewModel_Object", ResourceType = typeof(ZPVMResources.StringResource))]
        public EnumPVMTestDataType Object { get; set; }

        [Required]
        [Display(Name = "ProductControlObjectViewModel_Type", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Type { get; set; }

        [Required]
        [Display(Name = "ProductControlObjectViewModel_Value", ResourceType = typeof(ZPVMResources.StringResource))]
        public double Value { get; set; }

        /// <summary>
        /// 是否可用。
        /// </summary>
        [Display(Name = "ProductControlObjectViewModel_IsUsed", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool IsUsed { get; set; }


        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }


        public IEnumerable<SelectListItem> GetControlObjectList()
        {
            IDictionary<EnumPVMTestDataType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumPVMTestDataType>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetControlTypeList()
        {
            string[] types = new string[]{ 
                ">",
                ">=",
                "=",
                "<",
                "<=",
                "<>"
            };
            IEnumerable<SelectListItem> lst = from item in types
                                              select new SelectListItem()
                                              {
                                                  Text = item,
                                                  Value = item
                                              };
            return lst;
        }
    }
}