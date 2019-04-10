
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.PPM;
using PPMResources = ServiceCenter.Client.Mvc.Resources.PPM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.PPM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Models
{
    public class WorkOrderQueryViewModel
    {
        public WorkOrderQueryViewModel()
        {

        }

        [Display(Name = "WorkOrderQueryViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderNumber { get; set; }
    }

    public class WorkOrderViewModel
    {
        public WorkOrderViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.StartTime = DateTime.Now.Date;
            this.FinishTime = DateTime.Now.Date.AddDays(7);
            this.OrderQuantity = 100;
            this.Priority = EnumWorkOrderPriority.Normal;
            this.CloseType = EnumWorkOrderCloseType.None;
            this.OrderNumber = "1MO";
        }
        [Required]
        [Display(Name = "WorkOrderViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[1-3]CO-[0-9]{2}(0[1-9]|1[0-2])[0-9]{4}"
                           ,ErrorMessage="格式为：xCO-YYMM(年月)+4位流水号")]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "WorkOrderViewModel_MaterialCode", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                      , ErrorMessageResourceName = "ValidateStringLength"
                      , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "WorkOrderViewModel_OrderState", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderState { get; set; }

        [Display(Name = "WorkOrderViewModel_OrderType", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderType { get; set; }

        [Display(Name = "WorkOrderViewModel_Priority", ResourceType = typeof(PPMResources.StringResource))]
        public EnumWorkOrderPriority Priority { get; set; }

        [Required]
        [Display(Name = "WorkOrderViewModel_OrderQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double OrderQuantity { get; set; }

        [Display(Name = "WorkOrderViewModel_WIPQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double WIPQuantity { get; set; }
        [Display(Name = "WorkOrderViewModel_ScrapQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double ScrapQuantity { get; set; }
        [Display(Name = "WorkOrderViewModel_FinishQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double FinishQuantity { get; set; }
        [Display(Name = "WorkOrderViewModel_LeftQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double LeftQuantity { get; set; }
        [Display(Name = "WorkOrderViewModel_ReworkQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double ReworkQuantity { get; set; }
        [Display(Name = "WorkOrderViewModel_RepairQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double RepairQuantity { get; set; }
        [Required]
        [Display(Name = "WorkOrderViewModel_StartTime", ResourceType = typeof(PPMResources.StringResource))]
        public DateTime StartTime { get; set; }
        [Required]
        [Display(Name = "WorkOrderViewModel_FinishTime", ResourceType = typeof(PPMResources.StringResource))]
        public DateTime FinishTime { get; set; }

        [Display(Name = "WorkOrderViewModel_CloseType", ResourceType = typeof(PPMResources.StringResource))]
        public EnumWorkOrderCloseType CloseType { get; set; }

        [Display(Name = "WorkOrderViewModel_RevenueType", ResourceType = typeof(PPMResources.StringResource))]
        public string RevenueType { get; set; }

        [Display(Name = "WorkOrderViewModel_LocationName", ResourceType = typeof(PPMResources.StringResource))]
        public string LocationName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        public IEnumerable<SelectListItem> GetWorkOrderPriorityList()
        {
            IDictionary<EnumWorkOrderPriority, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumWorkOrderPriority>();

            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = Convert.ToString(item.Key)
                   };
        }
        public IEnumerable<SelectListItem> GetWorkOrderCloseTypeList()
        {
            IDictionary<EnumWorkOrderCloseType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumWorkOrderCloseType>();

            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = Convert.ToString(item.Key)
                   };
        }

        public IEnumerable<SelectListItem> GetLocationNameList()
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
                    return from item in result.Data
                           select new SelectListItem()
                           {
                               Text = item.Key,
                               Value = item.Key
                           };
                }
            }
            return new List<SelectListItem>();
        }

        public IEnumerable<SelectListItem> GetOrderStateList()
        {
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.CategoryName='{0}'
                                           AND Key.AttributeName='{1}'"
                                            , "OrderState"
                                            , "Value"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Value,
                                                          Value = item.Value
                                                      };

                    return lst.ToList();
                }
            }
            return new List<SelectListItem>();
        }

        public IEnumerable<SelectListItem> GetOrderTypeList()
        {
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.CategoryName='{0}'
                                           AND Key.AttributeName='{1}'"
                                            , "OrderType"
                                            , "Value"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Value,
                                                          Value = item.Value
                                                      };

                    return lst.ToList();
                }
            }
            return new List<SelectListItem>();
        }
    }
}