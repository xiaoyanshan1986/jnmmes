
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.ZPVC;
using ZPVCResources = ServiceCenter.Client.Mvc.Resources.ZPVC;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.ZPVC;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;

namespace ServiceCenter.Client.Mvc.Areas.ZPVC.Models
{
    public class PackageViewModel
    {
        public PackageViewModel()
        {
            this.PrintQty = 1;
            this.Qty = 100;
        }

        [Required]
        [Display(Name = "PackageViewModel_LineCode", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string LineCode { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_OrderNumber", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_ProductNumber", ResourceType = typeof(ZPVCResources.StringResource))]
        public string ProductNumber { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_Group", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Group { get; set; }


        [Display(Name = "PackageViewModel_PrinterName", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(255, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string PrinterName { get; set; }

        [Display(Name = "PackageViewModel_PrintLabelCode", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string PrintLabelCode { get; set; }

        [Display(Name = "PackageViewModel_PrintQty", ResourceType = typeof(ZPVCResources.StringResource))]
        [Range(0, 10
            , ErrorMessageResourceName = "ValidateRange"
            , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int PrintQty { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_Qty", ResourceType = typeof(ZPVCResources.StringResource))]
        [Range(1, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                            , ErrorMessageResourceName = "ValidateInt"
                            , ErrorMessageResourceType = typeof(StringResource))]
        public double? Qty { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_PackageNo", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string PackageNo { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_Code", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_Name", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                    , ErrorMessageResourceName = "ValidateStringLength"
                    , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_Lower", ResourceType = typeof(ZPVCResources.StringResource))]
        public double? Lower { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_Upper", ResourceType = typeof(ZPVCResources.StringResource))]
        public double? Upper { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_Grade", ResourceType = typeof(ZPVCResources.StringResource))]
        public string Grade { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_Color", ResourceType = typeof(ZPVCResources.StringResource))]
        public string Color { get; set; }


        [Required]
        [Display(Name = "PackageViewModel_PNType", ResourceType = typeof(ZPVCResources.StringResource))]
        public string PNType { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_MaterialCode", ResourceType = typeof(ZPVCResources.StringResource))]
        public string MaterialCode { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_Style", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Style { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_Technology", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Technology { get; set; }

        public IEnumerable<SelectListItem> GetGradeList()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='Grade' AND Key.AttributeName='VALUE'"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstValues = result.Data;
                }
            }
            return from item in lstValues
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Value
                   };
        }

        public IEnumerable<SelectListItem> GetColorList()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='Color' AND Key.AttributeName='VALUE'"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstValues = result.Data;
                }
            }
            return from item in lstValues
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Value
                   };
        }

        public IEnumerable<SelectListItem> GetProductionLineList()
        {
            //获取用户拥有权限的生产线。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.ProductionLine);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }
            IList<ProductionLine> lst = new List<ProductionLine>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = "Key LIKE '%06CS%'"
            };
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }

            return from item in lst
                   where lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = string.Format("{0}[{1}]", item.Name, item.Key),
                       Value = item.Key
                   };
        }


        public IEnumerable<SelectListItem> GetOrderNumberList()
        {
            IList<WorkOrder> lstValues = new List<WorkOrder>();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("CloseType='{0}'",Convert.ToInt32(EnumWorkOrderCloseType.None)),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstValues = result.Data;
                }
            }
            return from item in lstValues
                   select new SelectListItem()
                   {
                       Text = item.Key,
                       Value = item.Key
                   };
        }

        public IEnumerable<SelectListItem> GetGroupList()
        {
            IList<EfficiencyConfiguration> lstValues = new List<EfficiencyConfiguration>();
            using (EfficiencyConfigurationServiceClient client = new EfficiencyConfigurationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("IsUsed=1"),
                    OrderBy = "Key.Group"
                };

                MethodReturnResult<IList<EfficiencyConfiguration>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstValues = result.Data;
                }
            }

            var lnq = (from item in lstValues
                       select item.Key.Group).Distinct();
            return from item in lnq
                   select new SelectListItem()
                   {
                       Text = item,
                       Value = item
                   };
        }

        public IEnumerable<SelectListItem> GetPrinterNameList()
        {
            IList<ClientConfigAttribute> lst = new List<ClientConfigAttribute>();
            string hostName = HttpContext.Current.Request.UserHostName;
            string attributeName = "PrinterName";

            using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.ClientName='{0}' AND Key.AttributeName LIKE '{1}%'"
                                          , hostName
                                          , attributeName),
                    OrderBy = "Key.AttributeName"
                };
                MethodReturnResult<IList<ClientConfigAttribute>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lst = result.Data;
                }
            }
            return from item in lst
                   select new SelectListItem
                   {
                       Text = item.Value,
                       Value = item.Value
                   };
        }
        public IEnumerable<SelectListItem> GetLabelCodeList()
        {
            string hostName = HttpContext.Current.Request.UserHostName;
            string attributeName = "PrintLabelCode";
            string defaultLabel = string.Empty;
            //获取设置的默认值。
            using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
            {
                MethodReturnResult<ClientConfigAttribute> result = client.Get(new ClientConfigAttributeKey()
                {
                    ClientName = hostName,
                    AttributeName = attributeName
                });
                if (result.Code <= 0 && result.Data != null)
                {
                    defaultLabel = result.Data.Value;
                }
            }
            //获取打印标签数据。
            IList<PrintLabel> lst = new List<PrintLabel>();
            using (PrintLabelServiceClient client = new PrintLabelServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Type='{0}' AND IsUsed=1", Convert.ToInt32(EnumPrintLabelType.Package))
                };
                MethodReturnResult<IList<PrintLabel>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }

            return from item in lst
                   select new SelectListItem
                   {
                       Text = string.Format("{0}[{1}]", item.Key, item.Name),
                       Value = item.Key,
                       Selected = item.Key == defaultLabel
                   };
        }


        public IEnumerable<SelectListItem> GetStyleList()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='Style'"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstValues = result.Data;
                }
            }

            var lnqValue=from item in lstValues
                    where item.Key.AttributeName=="VALUE"
                    select item;

            var lnqName = from item in lstValues
                           where item.Key.AttributeName == "NAME"
                           select item;

            return from item in lnqValue
                   join itemName in lnqName on item.Key.ItemOrder equals itemName.Key.ItemOrder
                   select new SelectListItem()
                   {
                       Text = string.Format("{0}-{1}",item.Value, itemName.Value),
                       Value = item.Value
                   };
        }

        public IEnumerable<SelectListItem> GetTechnologyList()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='Technology'"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstValues = result.Data;
                }
            }

            var lnqValue = from item in lstValues
                           where item.Key.AttributeName == "VALUE"
                           select item;

            var lnqName = from item in lstValues
                          where item.Key.AttributeName == "NAME"
                          select item;

            return from item in lnqValue
                   join itemName in lnqName on item.Key.ItemOrder equals itemName.Key.ItemOrder
                   select new SelectListItem()
                   {
                       Text = string.Format("{0}-{1}", item.Value, itemName.Value),
                       Value = item.Value
                   };
        }
        

        public IEnumerable<SelectListItem> GetPNTypeList()
        {
            IList<string> lstValues = new List<string>();
            lstValues.Add("P");
            lstValues.Add("N");
            return from item in lstValues
                   select new SelectListItem()
                   {
                       Text = item,
                       Value = item
                   };
        }

        public Package GetPackage(string packageNo)
        {
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                MethodReturnResult<Package> result = client.Get(packageNo);
                return result.Data;
            }
        }

        public PackageInfo GetPackageInfo(string packageNo)
        {
            using (PackageInfoServiceClient client = new PackageInfoServiceClient())
            {
                MethodReturnResult<PackageInfo> result = client.Get(packageNo);
                return result.Data;
            }
        }

        public ProductionLine GetProductionLine(string lineCode)
        {
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<ProductionLine> result = client.Get(lineCode);
                return result.Data;
            }
        }
    } 
}