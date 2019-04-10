
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
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class BinRuleQueryViewModel
    {
        public BinRuleQueryViewModel()
        {

        }
        [Display(Name = "BinRuleQueryViewModel_PackageLine", ResourceType = typeof(FMMResources.StringResource))]
        public string PackageLine { get; set; }
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

    public class BinRuleViewModel
    {
        public BinRuleViewModel()
        {
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "BinRuleViewModel_BinNo", ResourceType = typeof(FMMResources.StringResource))]
        public string BinNo { get; set; }

        [Required]
        [Display(Name = "BinRuleViewModel_PackageLine", ResourceType = typeof(FMMResources.StringResource))]
        public string PackageLine { get; set; }

        [Required]
        [Display(Name = "BinRuleViewModel_WorkOrderNumber", ResourceType = typeof(FMMResources.StringResource))]
        public string WorkOrderNumber { get; set; }
        [Required]
        [Display(Name = "BinRuleViewModel_Grade", ResourceType = typeof(FMMResources.StringResource))]
        public string Grade { get; set; }
        [Required]
        [Display(Name = "BinRuleViewModel_PsItemNo", ResourceType = typeof(FMMResources.StringResource))]
        public int PsItemNo { get; set; }
        [Required]
        [Display(Name = "BinRuleViewModel_PsSubCode", ResourceType = typeof(FMMResources.StringResource))]
        public string PsSubCode { get; set; }
        [Required]
        [Display(Name = "BinRuleViewModel_PsCode", ResourceType = typeof(FMMResources.StringResource))]
        public string PsCode { get; set; }
        [Required]
        [Display(Name = "BinRuleViewModel_Color", ResourceType = typeof(FMMResources.StringResource))]
        public string Color { get; set; }

        [Required]
        [Display(Name = "BinRuleViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public  string LocationName { get; set; }

        [Display(Name = "BinRuleViewModel_Creator", ResourceType = typeof(FMMResources.StringResource))]
        public string Creator { get; set; }

        [Display(Name = "BinRuleViewModel_CreateTime", ResourceType = typeof(FMMResources.StringResource))]
        public DateTime? CreateTime { get; set; }
        [Required]
        [Display(Name = "BinRuleViewModel_Editor", ResourceType = typeof(FMMResources.StringResource))]
        public string Editor { get; set; }
        [Display(Name = "EditTime", ResourceType = typeof(FMMResources.StringResource))]
        public DateTime? EditTime { get; set; }


        public IEnumerable<SelectListItem> GetLocationNameList()
        {
            IList<Location> lst = new List<Location>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format("Level='{0}'", Convert.ToInt32(LocationLevel.Room))
            };
            using (LocationServiceClient client = new LocationServiceClient())
            {
                MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
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

        public IEnumerable<SelectListItem> GetColorList()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
                using (BaseAttributeValueServiceClient client1 = new BaseAttributeValueServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.CategoryName='Color' AND Key.AttributeName='VALUE'"),
                        OrderBy = "Key.ItemOrder"
                    };

                    MethodReturnResult<IList<BaseAttributeValue>> result = client1.Get(ref cfg);
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

    }
}