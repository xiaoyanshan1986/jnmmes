
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.FMM;
using FMMResources = ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.FMM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.Common.Print;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class PrintLabelQueryViewModel
    {
        public PrintLabelQueryViewModel()
        {

        }

        [Display(Name = "PrintLabelQueryViewModel_Code", ResourceType = typeof(FMMResources.StringResource))]
        public string Code { get; set; }

        [Display(Name = "PrintLabelQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

    }

    public class PrintLabelViewModel
    {
        public PrintLabelViewModel()
        {
            this.IsUsed = true;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;

        }

        [Required]
        [Display(Name = "PrintLabelViewModel_Code", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "PrintLabelViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }


        [Required]
        [Display(Name = "PrintLabelViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public EnumPrintLabelType Type { get; set; }

        [Required]
        [Display(Name = "PrintLabelViewModel_Content", ResourceType = typeof(FMMResources.StringResource))]
        public string Content { get; set; }

        /// <summary>
        /// 是否可用。
        /// </summary>
       [Display(Name = "PrintLabelViewModel_IsUsed", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsUsed { get; set; }


        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        public IEnumerable<SelectListItem> GetPrinterTypeList()
        {
            IDictionary<EnumPrintLabelType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumPrintLabelType>();

            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = Convert.ToString(item.Key)
                   };
        }
    }

    public class PrintTestViewModel
    {

        [Required]
        [Display(Name = "PrintTestViewModel_PrinterType", ResourceType = typeof(FMMResources.StringResource))]
        public EnumPrinterType PrinterType { get; set; }

        [Required]
        [Display(Name = "PrintTestViewModel_PrinterName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(255, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string PrinterName { get; set; }

        [Required]
        [Display(Name = "PrintTestViewModel_PrintContent", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(65536, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string PrintContent { get; set; }

        public IEnumerable<SelectListItem> GetPrinterTypeList()
        {
            IDictionary<EnumPrinterType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumPrinterType>();

            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = Convert.ToString(item.Key)
                   };
        }

        public IEnumerable<SelectListItem> GetPrinterList()
        {
            IList<ClientConfig> lst = new List<ClientConfig>();
            using (ClientConfigServiceClient client = new ClientConfigServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging=false,
                    Where = string.Format("ClientType='{0}' OR ClientType='{1}'"
                                            , Convert.ToInt32(EnumClientType.NetworkPrinter)
                                            , Convert.ToInt32(EnumClientType.RawPrinter))
                };
                MethodReturnResult<IList<ClientConfig>> result = client.Get(ref cfg);
                if(result.Code==0)
                {
                    lst = result.Data;
                }
            }
            return from item in lst
                   select new SelectListItem()
                   {
                       Text = item.Key,
                       Value = Convert.ToString(item.IPAddress)
                   };
        }

    }
}