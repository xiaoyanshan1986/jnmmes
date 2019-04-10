
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
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVC.Models
{
    public class BoxPrintViewModel
    {
        public BoxPrintViewModel()
        {
            this.PrintQty = 1;
        }

        [Required]
        [Display(Name = "BoxViewModel_BoxNo", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string BoxNo { get; set; }

        [Display(Name = "BoxViewModel_BoxNo", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string BoxNo1 { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_PrinterName", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(255, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string PrinterName { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_PrintLabelCode", ResourceType = typeof(ZPVCResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string PrintLabelCode { get; set; }

        [Required]
        [Display(Name = "PackageViewModel_PrintQty", ResourceType = typeof(ZPVCResources.StringResource))]
        [Range(1, 10
            , ErrorMessageResourceName = "ValidateRange"
            , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int PrintQty { get; set; }

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
                    Where = string.Format("Type='{0}' AND IsUsed=1", Convert.ToInt32(EnumPrintLabelType.Box))
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
    }
}