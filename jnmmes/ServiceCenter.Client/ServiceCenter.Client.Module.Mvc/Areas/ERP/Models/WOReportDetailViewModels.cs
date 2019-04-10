
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ERPResources = ServiceCenter.Client.Mvc.Resources.ERP;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.ERP;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.ERP;


namespace ServiceCenter.Client.Mvc.Areas.ERP.Models
{
    public class WOReportDetailQueryViewModel
    {
        public WOReportDetailQueryViewModel()
        {
            //this.BillCheckState = EnumBillCheckState.NoCheck;
        }

        /// <summary>
        /// 单据号
        /// </summary>
        [Display(Name = "WOReportDetailViewModels_BillCode", ResourceType = typeof(ERPResources.StringResource))]
        public string BillCode { get; set; }

        /// <summary>
        /// 托号
        /// </summary>
        [Required]
        [Display(Name = "WOReportDetailViewModels_ObjectNumber", ResourceType = typeof(ERPResources.StringResource))]
        public string ObjectNumber { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        [Display(Name = "WOReportViewModels_BillDate", ResourceType = typeof(ERPResources.StringResource))]
        public DateTime? BillDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "WOReportViewModels_Note", ResourceType = typeof(ERPResources.StringResource))]
        public string Note { get; set; }

        /// <summary>
        /// 单据接收核对状态
        /// </summary>
        [Display(Name = "单据接收核对状态")]
        public EnumBillCheckState BillCheckState { get; set; } 

        /// <summary>
        /// 入库单状态
        /// </summary>
        [Display(Name = "WOReportViewModels_BillState", ResourceType = typeof(ERPResources.StringResource))]
        public EnumBillState BillState { get; set; }      
        
        [Display(Name = "WOReportViewModels_BillMaker", ResourceType = typeof(ERPResources.StringResource))]
        public string BillMaker { get; set; }

        [Display(Name = "WOReportViewModels_BillMakedDate", ResourceType = typeof(ERPResources.StringResource))]
        public DateTime? BillMakedDate { get; set; }

        [Display(Name = "WOReportViewModels_MixType", ResourceType = typeof(ERPResources.StringResource))]
        public EnumMixType MixType { get; set; }
        [Display(Name = "WOReportViewModels_ScrapType", ResourceType = typeof(ERPResources.StringResource))]
        public EnumScrapType ScrapType { get; set; }

        [Display(Name = "WOReportViewModels_OrderNumber", ResourceType = typeof(ERPResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "WOReportViewModels_MaterialCode", ResourceType = typeof(ERPResources.StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "WOReportViewModels_WRCode", ResourceType = typeof(ERPResources.StringResource))]
        public string WRCode { get; set; }
        public string INCode { get; set; }

        [Display(Name = "WOReportViewModels_Store", ResourceType = typeof(ERPResources.StringResource))]
        public string Store { get; set; }

        /// <summary>
        /// 入库类型
        /// </summary>
        [Display(Name = "WOReportViewModels_StockInType", ResourceType = typeof(ERPResources.StringResource))]
        public EnumStockInType BillType { get; set; }

        public IEnumerable<SelectListItem> GetMixtype()
        {
            IDictionary<EnumMixType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumMixType>();

            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = Convert.ToString(item.Key)
                   };
        }

        public IEnumerable<SelectListItem> GetScraptype()
        {
            IDictionary<EnumScrapType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumScrapType>();

            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = Convert.ToString(item.Key)
                   };
        }

        /// <summary>
        /// 取得入库类型列表
        /// </summary>
        /// <param name="iScrap">报废标识 0 - 非报废 1 - 报废</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetBillType(int iScrap)
        {
            IList<SelectListItem> lst = new List<SelectListItem>();

            IDictionary<EnumStockInType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumStockInType>();

            foreach (KeyValuePair<EnumStockInType, string> item in dic)
            {

                if (iScrap == 0)
                {
                    if (item.Key == EnumStockInType.Scrap)
                    {
                        continue;
                    }
                }
                else
                {
                    if (item.Key != EnumStockInType.Scrap)
                    {
                        continue;
                    }
                }

                SelectListItem NewItem = new SelectListItem()
                {
                    Text = item.Value,
                    Value = Convert.ToString(item.Key)
                };

                lst.Add(NewItem);
            }

            return lst.ToList();
        }
    }

    public class WOReportDetailViewModel
    {
        public WOReportDetailViewModel()
        {
            
        }
        [Required]
        [Display(Name = "WOReportDetailViewModels_BillCode", ResourceType = typeof(ERPResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string BillCode { get; set; }

        [Required]
        [Display(Name = "WOReportDetailViewModels_ItemNo", ResourceType = typeof(ERPResources.StringResource))]
        public int ItemNo { get; set; }

        [Display(Name = "WOReportDetailViewModels_ObjectNumber", ResourceType = typeof(ERPResources.StringResource))]
        public string ObjectNumber { get; set; }

        [Display(Name = "WOReportDetailViewModels_MaterialCode", ResourceType = typeof(ERPResources.StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "WOReportDetailViewModels_Grade", ResourceType = typeof(ERPResources.StringResource))]
        public string Grade { get; set; }

        [Display(Name = "WOReportDetailViewModels_Color", ResourceType = typeof(ERPResources.StringResource))]
        public string Color { get; set; }

        [Display(Name = "WOReportDetailViewModels_EffiName", ResourceType = typeof(ERPResources.StringResource))]
        public string EffiName { get; set; }

        [Display(Name = "WOReportDetailViewModels_EffiCode", ResourceType = typeof(ERPResources.StringResource))]
        public string EffiCode { get; set; }

        [Display(Name = "WOReportDetailViewModels_Qty", ResourceType = typeof(ERPResources.StringResource))]
        public decimal Qty { get; set; }
        
        [Display(Name = "WOReportDetailViewModels_SumCOEF_PMAX", ResourceType = typeof(ERPResources.StringResource))]
        public decimal SumCoefPMax { get; set; }

        //[Display(Name = "WOReportDetailViewModels_MaterialName", ResourceType = typeof(ERPResources.StringResource))]
        //public decimal MaterialName { get; set; }

        [Display(Name = "WOReportViewModels_OrderNumber", ResourceType = typeof(ERPResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "WOReportViewModels_PsSubcode", ResourceType = typeof(ERPResources.StringResource))]
        public string PsSubcode { get; set; }

        /// <summary>
        /// ERP入库单号
        /// </summary>
        [Display(Name = "WOReportViewModels_ERPStockInCode", ResourceType = typeof(ERPResources.StringResource))]
        public string ERPStockInCode { get; set; }

        /// <summary>
        /// ERP入库单主键
        /// </summary>
        [Display(Name = "WOReportViewModels_ERPStockInKey", ResourceType = typeof(ERPResources.StringResource))]
        public string ERPStockInKey { get; set; }        

        /// <summary>
        /// 托核对状态
        /// </summary>
        [Display(Name = "托核对状态")]
        public string PackageCheckState { get; set; }

    }
}