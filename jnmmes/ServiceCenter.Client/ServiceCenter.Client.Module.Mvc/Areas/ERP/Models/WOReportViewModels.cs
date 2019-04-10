
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
    public class WOReportQueryViewModel
    {
        public WOReportQueryViewModel() { }

        [Display(Name = "WOReportViewModels_BillCode", ResourceType = typeof(ERPResources.StringResource))]
        public string BillCode { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        [Display(Name = "WOReportViewModels_BillState", ResourceType = typeof(ERPResources.StringResource))]
        public string BillState { get; set; }

        /// <summary>
        /// 入库类型
        /// </summary>
        [Display(Name = "WOReportViewModels_StockInType", ResourceType = typeof(ERPResources.StringResource))]
        public string BillType { get; set; }

        /// <summary>
        /// 取得入库类型列表
        /// </summary>
        /// <param name="iScrap">报废标识 【0 - 非报废（产成品/在制品）】; 【1 - 报废】</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetStockInType(int iScrap)
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
                    Value = Convert.ToString(item.Key.GetHashCode())
                };

                lst.Add(NewItem);
            }

            return lst.ToList();
        }

        /// <summary>
        /// 取得入库单状态
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetStockInState()
        {
            IDictionary<EnumBillState, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumBillState>();

            IList<SelectListItem> lst = new List<SelectListItem>();

            foreach (KeyValuePair<EnumBillState, string> item in dic)
            {
                SelectListItem NewItem = new SelectListItem()
                {
                    Text = item.Value,
                    Value = Convert.ToString(item.Key.GetHashCode())
                };

                lst.Add(NewItem);
            }

            return lst.ToList();
        }
    }

    public class WOReportViewModel
    {
        public WOReportViewModel()
        {
            BillState = EnumBillState.Create;   //入库单状态 0 - 新增 1 - 入库申请完成 2 - 入库接收完成    
            BillDate = DateTime.Now;            //入库日期初始值
            BillMakedDate = DateTime.Now;       //制单日期初始值
            MixType = EnumMixType.True;         //是否混工单（已经废止）
            ScrapType = EnumScrapType.False;    //是否是报废品入库
        }

        /// <summary>
        /// 入库单号
        /// </summary>
        [Display(Name = "WOReportViewModels_BillCode", ResourceType = typeof(ERPResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string BillCode { get; set; }

        /// <summary>
        /// 入库类型
        /// </summary>
        [Display(Name = "WOReportViewModels_StockInType", ResourceType = typeof(ERPResources.StringResource))]
        public EnumStockInType BillType { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        [Display(Name = "WOReportViewModels_BillState", ResourceType = typeof(ERPResources.StringResource))]
        public EnumBillState BillState { get; set; }

        /// <summary>
        /// 单据接收核对状态
        /// </summary>
        [Display(Name = "单据接收核对状态")]
        public EnumBillCheckState BillCheckState { get; set; } 

        /// <summary>
        /// 入库日期
        /// </summary>
        [Required]
        [Display(Name = "WOReportViewModels_BillDate", ResourceType = typeof(ERPResources.StringResource))]
        public DateTime? BillDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "WOReportViewModels_Note", ResourceType = typeof(ERPResources.StringResource))]
        public string Note { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        [Display(Name = "WOReportViewModels_BillMaker", ResourceType = typeof(ERPResources.StringResource))]
        public string BillMaker { get; set; }

        /// <summary>
        /// 制单日期
        /// </summary>
        [Display(Name = "WOReportViewModels_BillMakedDate", ResourceType = typeof(ERPResources.StringResource))]
        public DateTime? BillMakedDate { get; set; }

        /// <summary>
        /// 是否混工单
        /// </summary>
        [Display(Name = "WOReportViewModels_MixType", ResourceType = typeof(ERPResources.StringResource))]
        public EnumMixType MixType { get; set; }

        /// <summary>
        /// 报废入库
        /// </summary>
        [Display(Name = "WOReportViewModels_ScrapType", ResourceType = typeof(ERPResources.StringResource))]
        public EnumScrapType ScrapType { get; set; }

        /// <summary>
        /// 工单号
        /// </summary>
        [Display(Name = "WOReportViewModels_OrderNumber", ResourceType = typeof(ERPResources.StringResource))]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        [Display(Name = "WOReportViewModels_MaterialCode", ResourceType = typeof(ERPResources.StringResource))]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        [Display(Name = "WOReportViewModels_TotalQty", ResourceType = typeof(ERPResources.StringResource))]
        public decimal TotalQty { get; set; }

        /// <summary>
        /// ERP报工单号
        /// </summary>
        [Display(Name = "WOReportViewModels_WRCode", ResourceType = typeof(ERPResources.StringResource))]
        public string WRCode { get; set; }

        /// <summary>
        /// ERP报工单主键
        /// </summary>
        [Display(Name = "WOReportViewModels_WRKey", ResourceType = typeof(ERPResources.StringResource))]
        public string ERPWRKey { get; set; }

        /// <summary>
        /// 原ERP入库单主键（弃用）
        /// </summary>
        [Display(Name = "WOReportViewModels_INCode", ResourceType = typeof(ERPResources.StringResource))]
        public string INCode { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 获取是否混工单列表
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 获取报废类型列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetScraptype()
        {
            IDictionary<EnumScrapType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumScrapType>();

            return from item in dic
                   orderby item.Value descending
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = Convert.ToString(item.Key)
                   };
        }

        /// <summary>
        /// 取得入库类型列表
        /// </summary>
        /// <param name="iScrap">报废标识 【0 - 非报废】 【1 - 报废】</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetStockInType(int iScrap)
        {
            IList<SelectListItem> lst = new List<SelectListItem>();

            IDictionary<EnumStockInType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumStockInType>();

            foreach(KeyValuePair<EnumStockInType, string> item in dic)
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

        /// <summary>
        /// 取得入库单状态
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetStockInState()
        {
            IDictionary<EnumStockInType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumStockInType>();

            return from item in dic
                   orderby item.Value descending
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = Convert.ToString(item.Key)
                   };
        }
    }
    
}