
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.WIP;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.Client.Mvc.Areas.FMM.Models;
using System.Text;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class LotBinViewModel
    {
        public LotBinViewModel()
        {
        }
        [Required]
        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string LotNumber { get; set; }

        [Display(Name = "读头编号")]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string ScanIP { get; set; }


        [Display(Name = "BinNo")]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string BinNo { get; set; }

        [Display(Name = "ScanNo")]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string ScanNo { get; set; }


        public IEnumerable<SelectListItem> GetScanCodeList()
        {
            MethodReturnResult<IList<Equipment>> result = new MethodReturnResult<IList<Equipment>>();
             IList<Equipment> lst = new List<Equipment>();
                using (EquipmentServiceClient client = new EquipmentServiceClient())
                {
                        StringBuilder where = new StringBuilder();
                        where.AppendFormat(" {0} Name LIKE 'Reader%'"
                                                    , where.Length > 0 ? "AND" : string.Empty);
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging=false,
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        result = client.Get(ref cfg);
                }
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
                 return from item in lst
                   select new SelectListItem()
                   {
                       Value = item.Key,
                       Text= item.LineCode
                   }; 

        }
    }


    public class LotBinQueryViewModel
    {
        public LotBinQueryViewModel()
        {
        }
        [Required]
        [Display(Name = "线别")]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string PackageLine { get; set; }

        [Display(Name = "Bin号")]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string BinNo { get; set; }


        [Display(Name = "是否自动刷新")]
        public bool IsAutoRefresh { get; set; }


        public IEnumerable<SelectListItem> GetLineCodeList()
        {
            IList<ProductionLine> lst = new List<ProductionLine>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false
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
                   select new SelectListItem()
                   {
                       Text = item.Key,
                       Value = item.Key
                   };
        }



    }
}