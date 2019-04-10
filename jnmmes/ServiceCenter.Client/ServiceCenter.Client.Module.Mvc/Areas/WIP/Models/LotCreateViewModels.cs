
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

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class LotCreateMainViewModel
    {
        public LotCreateMainViewModel()
        {
            this.Count = 1;
            this.PrintQty = 2;
        }

        /// <summary>
        /// 工单号
        /// </summary>
        [Required]
        [Display(Name = "LotCreateMainViewModel_OrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        //[StringLength(50, MinimumLength = 1
        //        , ErrorMessageResourceName = "ValidateStringLength"
        //        , ErrorMessageResourceType = typeof(StringResource))]        
        public string OrderNumber { get; set; }

        /// <summary>
        /// 单批次产品数量
        /// </summary>
        [Required]
        [Display(Name = "LotCreateMainViewModel_LotQuantity", ResourceType = typeof(WIPResources.StringResource))]
        //[Range(1, 65536
        //        , ErrorMessageResourceName = "ValidateRange"
        //        , ErrorMessageResourceType = typeof(StringResource))]
        public double LotQuantity { get; set; }

        /// <summary>
        /// 产品物料代码
        /// </summary>
        [Required]
        [Display(Name = "LotCreateMainViewModel_MaterialCode", ResourceType = typeof(WIPResources.StringResource))]        
        public string MaterialCode { get; set; }

        /// <summary>
        /// 创建数量
        /// </summary>
        [Required]
        [Display(Name = "LotCreateMainViewModel_Count", ResourceType = typeof(WIPResources.StringResource))]
        //[Range(1, 100
        //        , ErrorMessageResourceName = "ValidateRange"
        //        , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int Count { get; set; }

        /// <summary>
        /// 车间代码
        /// </summary>
        [Required]
        [Display(Name = "LotCreateMainViewModel_LocationName", ResourceType = typeof(WIPResources.StringResource))]
        //[StringLength(50, MinimumLength = 1
        //        , ErrorMessageResourceName = "ValidateStringLength"
        //        , ErrorMessageResourceType = typeof(StringResource))]
        public string LocationName { get; set; }

        /// <summary>
        /// 线别代码
        /// </summary>
        [Required]
        [Display(Name = "LotCreateMainViewModel_LineCode", ResourceType = typeof(WIPResources.StringResource))]
        //[StringLength(50, MinimumLength = 1
        //        , ErrorMessageResourceName = "ValidateStringLength"
        //        , ErrorMessageResourceType = typeof(StringResource))]
        public string LineCode { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        [Display(Name = "LotPrintViewModel_PrinterName", ResourceType = typeof(WIPResources.StringResource))]
        //[StringLength(255, MinimumLength = 1
        //        , ErrorMessageResourceName = "ValidateStringLength"
        //        , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string PrinterName { get; set; }

        /// <summary>
        /// 打印标签类型
        /// </summary>
        [Display(Name = "LotPrintViewModel_PrintLabelCode", ResourceType = typeof(WIPResources.StringResource))]
        //[StringLength(50, MinimumLength = 1
        //        , ErrorMessageResourceName = "ValidateStringLength"
        //        , ErrorMessageResourceType = typeof(StringResource))]
        public string PrintLabelCode { get; set; }

        /// <summary>
        /// 打印数量
        /// </summary>
        [Display(Name = "LotPrintViewModel_PrintQty", ResourceType = typeof(WIPResources.StringResource))]
        [Range(0, 10
            , ErrorMessageResourceName = "ValidateRange"
            , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int PrintQty { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        //批次号列表
        [Required]
        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        //[StringLength(50, MinimumLength = 1
        //                , ErrorMessageResourceName = "ValidateStringLength"
        //                , ErrorMessageResourceType = typeof(StringResource))]
        public string LotNumber { get; set; }
                
        /// <summary> 取得打印机清单 </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 取得标签清单
        /// </summary>
        /// <param name="materialCode">产品代码</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetLabelCodeList(string materialCode)
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

            //获取产品打印标签数据
            IList<PrintLabel> printLabellst = new List<PrintLabel>();

            using (PrintLabelServiceClient client = new PrintLabelServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Type = '{0}' AND IsUsed = 1
                                          AND EXISTS( FROM MaterialPrintSet as p 
                                                      WHERE p.Key.LabelCode = self.Key 
                                                        AND p.Key.MaterialCode = '{1}' )",
                                            Convert.ToInt32(EnumPrintLabelType.Lot),
                                            materialCode)
                };

                //MaterialPrintSet f = new MaterialPrintSet();
                //f.Key.LabelCode = "";

                MethodReturnResult<IList<PrintLabel>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data != null)
                {
                    printLabellst = result.Data;
                }
            }

            return from item in printLabellst
                   select new SelectListItem
                   {
                       Text = string.Format("{0}[{1}]", item.Key, item.Name),
                       Value = item.Key,
                       Selected = item.Key == defaultLabel
                   };





        //    IList<MaterialPrintSet> lst = new List<MaterialPrintSet>();

        //    using (MaterialPrintSetServiceClient client = new MaterialPrintSetServiceClient())
        //    {
        //        PagingConfig cfg = new PagingConfig()
        //        {
        //            IsPaging = false,
        //            Where = string.Format("Type='{0}'", Convert.ToInt32(EnumPrintLabelType.Lot))
        //        };

        //        MethodReturnResult<IList<MaterialPrintSet>> result = client.Get(ref cfg);

        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            lst = result.Data;
        //        }
        //    }

        //    return from item in lst
        //           select new SelectListItem
        //           {
        //               Text = string.Format("{0}[{1}]", item.Key, item.Name),
        //               Value = item.Key.LabelCode,
        //               Selected = item.Key.LabelCode == defaultLabel
        //           };
        }

        //public IEnumerable<SelectListItem> GetLabelCodeList()
        //{
        //    string hostName = HttpContext.Current.Request.UserHostName;
        //    string attributeName = "PrintLabelCode";
        //    string defaultLabel = string.Empty;

        //    //获取设置的默认值。
        //    using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
        //    {
        //        MethodReturnResult<ClientConfigAttribute> result = client.Get(new ClientConfigAttributeKey()
        //        {
        //            ClientName = hostName,
        //            AttributeName = attributeName
        //        });
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            defaultLabel = result.Data.Value;
        //        }
        //    }

        //    //获取打印标签数据。
        //    IList<PrintLabel> lst = new List<PrintLabel>();

        //    using (PrintLabelServiceClient client = new PrintLabelServiceClient())
        //    {
        //        PagingConfig cfg = new PagingConfig()
        //        {
        //            IsPaging = false,
        //            Where = string.Format("Type='{0}' AND IsUsed=1", Convert.ToInt32(EnumPrintLabelType.Lot))
        //        };
        //        MethodReturnResult<IList<PrintLabel>> result = client.Get(ref cfg);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            lst = result.Data;
        //        }
        //    }

        //    return from item in lst
        //           select new SelectListItem
        //           {
        //               Text = string.Format("{0}[{1}]", item.Key, item.Name),
        //               Value = item.Key,
        //               Selected = item.Key == defaultLabel
        //           };
        //}

        /// <summary> 取得车间代码 </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetLocations()
        {
            using (LocationServiceClient client = new LocationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Level='{0}'", Convert.ToInt32(LocationLevel.Room))
                };

                MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }
    }
    
    public class LotCreateDetailViewModel : LotCreateMainViewModel
    {
        public LotCreateDetailViewModel()
        {
        }

        [Required]
        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string LotNumber { get; set; }

        [Display(Name = "LotCreateDetailViewModel_ProductCode", ResourceType = typeof(WIPResources.StringResource))]
        public string ProductCode { get; set; }

        [Display(Name = "LotCreateDetailViewModel_MaterialQty", ResourceType = typeof(WIPResources.StringResource))]
        public double MaterialQty { get; set; }

        [Display(Name = "LotCreateDetailViewModel_SupplierCode", ResourceType = typeof(WIPResources.StringResource))]
        public string SupplierCode { get; set; }

        [Required]
        [Display(Name = "LotCreateDetailViewModel_RouteEnterpriseName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteEnterpriseName { get; set; }
        [Required]
        [Display(Name = "LotCreateDetailViewModel_RouteName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteName { get; set; }
        [Required]
        [Display(Name = "LotCreateDetailViewModel_RouteStepName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteStepName { get; set; }

        [Required]
        [Display(Name = "LotCreateDetailViewModel_RawQuantity", ResourceType = typeof(WIPResources.StringResource))]
        [Range(1, 65536
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        public double RawQuantity { get; set; }

        [Required]
        [Display(Name = "LotCreateDetailViewModel_Quantity", ResourceType = typeof(WIPResources.StringResource))]
        [Range(1, 65536
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        public double Quantity { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> GetRouteEnterpriseNameList(string orderNumber,EnumLotType lotType)
        {
            IList<WorkOrderRoute> lstWorkOrderRoute = new List<WorkOrderRoute>();
            
            //获取工单工艺信息。
            using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.OrderNumber='{0}'", orderNumber),
                    OrderBy = "Key.ItemNo"
                };
                if (lotType == EnumLotType.Rework)
                {
                    cfg.Where += " AND IsRework=1";
                }
                else
                {
                    cfg.Where += " AND IsRework=0";
                }

                MethodReturnResult<IList<WorkOrderRoute>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lstWorkOrderRoute = result.Data;
                }
            }
            var lnq=from item in lstWorkOrderRoute 
                    select item.RouteEnterpriseName;

            return from item in lnq.Distinct()
                   select new SelectListItem
                   {
                       Text = item,
                       Value = item
                   };
        }

        /// <summary>
        /// 取得车间代码
        /// </summary>
        public IEnumerable<SelectListItem> GetLocations()
        {
            using (LocationServiceClient client = new LocationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Level='{0}'", Convert.ToInt32(LocationLevel.Room))
                };

                MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }
    }
}
