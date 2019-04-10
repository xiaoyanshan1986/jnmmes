
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
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Model.ZPVM;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class LotBinViewModel
    {
        string localName = System.Configuration.ConfigurationSettings.AppSettings["LocalName"];
        public LotBinViewModel()
        {
        }
        //[Required]
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
            if (localName == "K01")
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
                        IsPaging = false,
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
                           Text = item.LineCode
                       };
            }
            else if (localName == "G01")
            {
                MethodReturnResult<IList<ProductionLine>> result = new MethodReturnResult<IList<ProductionLine>>();
                IList<ProductionLine> lst = new List<ProductionLine>();
                using (ProductionLineServiceClient client = new ProductionLineServiceClient())
                {
                    StringBuilder where = new StringBuilder();
                    //where.AppendFormat(" {0} Name LIKE 'Reader%'"
                    //                            , where.Length > 0 ? "AND" : string.Empty);
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "Key"
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
                           Text = item.Key
                       };
            }
            else
            {
                return null; 
            }                    
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

        public BinRule GetBinRule(string binNo,string packageline)
        {
            using (BinRuleServiceClient client = new BinRuleServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.BinNo='{0}' AND Key.PackageLine='{1}'", binNo, packageline),
                  
                };
                MethodReturnResult<IList<BinRule>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null & result.Data.Count > 0)
                {
                    return result.Data[0];
                }
            }
            return null;
        }
        public Powerset GetPsName(string PsCode, int PsItemNo)
        {
            using (PowersetServiceClient client = new PowersetServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.Code='{0}' AND Key.ItemNo={1}", PsCode, PsItemNo),

                };
                MethodReturnResult<IList<Powerset>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null & result.Data.Count > 0)
                {
                    return result.Data[0];
                }
            }
            return null;
        }

    }
}