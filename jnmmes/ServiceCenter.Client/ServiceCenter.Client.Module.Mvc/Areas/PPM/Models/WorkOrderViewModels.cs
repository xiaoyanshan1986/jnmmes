
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
            OrderState = EnumWorkOrderState.Open.GetHashCode().ToString();
        }

        /// <summary>
        /// 工单号
        /// </summary>
        [Display(Name = "WorkOrderQueryViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 工单状态
        /// </summary>
        [Display(Name = "WorkOrderViewModel_OrderState", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderState { get; set; }

        /// <summary>
        /// 取得工单状态列表，包含空状态用于表示不选择
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> GetOrderStateList()
        {
            IEnumerable<SelectListItem> Enumlst = new List<SelectListItem>();
            IList<SelectListItem> lst = new List<SelectListItem>();

            IDictionary<EnumObjectStatus, string> dic1 = EnumExtensions.GetDisplayNameDictionary<EnumObjectStatus>();

            IDictionary<EnumWorkOrderState, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumWorkOrderState>();

            Enumlst = from item in dic
                      select new SelectListItem()
                      {
                          Text = item.Value,
                          Value = Convert.ToString(item.Key.GetHashCode())
                      };

            SelectListItem NewItem = new SelectListItem()
            {
                Text = "",
                Value = ""
            };

            lst.Add(NewItem);

            foreach (SelectListItem item in Enumlst)
            {
                NewItem = new SelectListItem()
                {
                    Text = item.Text,
                    Value = item.Value
                };

                lst.Add(NewItem);
            }

            return lst;
        }
    }

    public class WorkOrderViewModel
    {
        public WorkOrderViewModel()
        {
            this.OrderNumber = "1MO";                               //工单号初始化
            this.PlanStartTime = DateTime.Now.Date;                 //计划开工日期
            this.PlanFinishTime = DateTime.Now.Date.AddDays(7);     //计划完工日期            
            this.OrderQuantity = 100;                               //工单初始化数量
            this.Priority = EnumWorkOrderPriority.Normal;           //优先级
            this.CloseType = EnumWorkOrderCloseType.None;           //关闭类型
            this.CreateType = EnumWorkOrderCreateType.MES;          //数据创建类型 0 - MES系统创建
            this.OrderState = EnumWorkOrderState.Open;              //工单状态
            this.CreateTime = DateTime.Now;                         //创建时间
            this.EditTime = DateTime.Now;                           //编辑时间
        }

        /// <summary>
        /// 工单号
        /// </summary>
        [Required]
        [Display(Name = "WorkOrderViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[1-3]MO-[0-9]{2}(0[1-9]|1[0-2])[0-9]{4}"
                           ,ErrorMessage="格式为：xMO-YYMM(年月)+4位流水号")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 物料代码
        /// </summary>
        [Required]
        [Display(Name = "WorkOrderViewModel_MaterialCode", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                      , ErrorMessageResourceName = "ValidateStringLength"
                      , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 工单状态
        /// </summary>
        [Display(Name = "WorkOrderViewModel_OrderState", ResourceType = typeof(PPMResources.StringResource))]
        public EnumWorkOrderState OrderState { get; set; }

        /// <summary>
        /// 工单类型
        /// </summary>
        [Display(Name = "WorkOrderViewModel_OrderType", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderType { get; set; }

        /// <summary>
        /// 工单优先级
        /// </summary>
        [Display(Name = "WorkOrderViewModel_Priority", ResourceType = typeof(PPMResources.StringResource))]
        public EnumWorkOrderPriority Priority { get; set; }

        /// <summary>
        /// 工单数量
        /// </summary>
        [Required]
        [Display(Name = "WorkOrderViewModel_OrderQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double OrderQuantity { get; set; }

        /// <summary>
        /// 再制品数量
        /// </summary>
        [Display(Name = "WorkOrderViewModel_WIPQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double WIPQuantity { get; set; }

        /// <summary>
        /// 报废数量
        /// </summary>
        [Display(Name = "WorkOrderViewModel_ScrapQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double ScrapQuantity { get; set; }

        /// <summary>
        /// 完工数量
        /// </summary>
        [Display(Name = "WorkOrderViewModel_FinishQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double FinishQuantity { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        [Display(Name = "WorkOrderViewModel_LeftQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double LeftQuantity { get; set; }

        /// <summary>
        /// 返工数量
        /// </summary>
        [Display(Name = "WorkOrderViewModel_ReworkQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double ReworkQuantity { get; set; }

        /// <summary>
        /// 返修数量
        /// </summary>
        [Display(Name = "WorkOrderViewModel_RepairQuantity", ResourceType = typeof(PPMResources.StringResource))]
        public double RepairQuantity { get; set; }

        /// <summary>
        /// 计划开工时间
        /// </summary>
        [Required]
        [Display(Name = "WorkOrderViewModel_PlanStartTime", ResourceType = typeof(PPMResources.StringResource))]
        public DateTime PlanStartTime { get; set; }

        /// <summary>
        /// 计划完工时间
        /// </summary>
        [Required]
        [Display(Name = "WorkOrderViewModel_PlanFinishTime", ResourceType = typeof(PPMResources.StringResource))]
        public DateTime PlanFinishTime { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [Required]
        [Display(Name = "WorkOrderViewModel_StartTime", ResourceType = typeof(PPMResources.StringResource))]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [Required]
        [Display(Name = "WorkOrderViewModel_FinishTime", ResourceType = typeof(PPMResources.StringResource))]
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// 工单关闭状态
        /// </summary>
        [Display(Name = "WorkOrderViewModel_CloseType", ResourceType = typeof(PPMResources.StringResource))]
        public EnumWorkOrderCloseType CloseType { get; set; }

        /// <summary>
        /// 保税类型
        /// </summary>
        [Display(Name = "WorkOrderViewModel_RevenueType", ResourceType = typeof(PPMResources.StringResource))]
        public string RevenueType { get; set; }

        /// <summary>
        /// 生产车间
        /// </summary>
        [Display(Name = "WorkOrderViewModel_LocationName", ResourceType = typeof(PPMResources.StringResource))]
        public string LocationName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        /// <summary>
        /// 工单创建类型
        /// </summary>
        [Display(Name = "WorkOrderViewModel_CreateType", ResourceType = typeof(PPMResources.StringResource))]
        public EnumWorkOrderCreateType CreateType { get; set; }

        /// <summary>
        /// ERP工单主键
        /// </summary>
        [Display(Name = "WorkOrderViewModel_ERPWorkOrderKey", ResourceType = typeof(PPMResources.StringResource))]
        public string ERPWorkOrderKey { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        /// <summary>
        /// 编辑日期
        /// </summary>
        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }
             

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

//        public IEnumerable<SelectListItem> GetOrderStateList()
//        {
//            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"Key.CategoryName='{0}'
//                                           AND Key.AttributeName='{1}'"
//                                            , "OrderState"
//                                            , "Value"),
//                    OrderBy = "Key.ItemOrder"
//                };

//                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
//                if (result.Code <= 0)
//                {
//                    IEnumerable<SelectListItem> lst = from item in result.Data
//                                                      select new SelectListItem()
//                                                      {
//                                                          Text = item.Value,
//                                                          Value = item.Value
//                                                      };

//                    return lst.ToList();
//                }
//            }
//            return new List<SelectListItem>();
//        }
        /// <summary>
        /// 取得工单状态列表，包含空状态用于表示不选择
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> GetOrderStateList()
        {
            IEnumerable<SelectListItem> Enumlst = new List<SelectListItem>();
            IList<SelectListItem> lst = new List<SelectListItem>();

            IDictionary<EnumObjectStatus, string> dic1 = EnumExtensions.GetDisplayNameDictionary<EnumObjectStatus>();

            IDictionary<EnumWorkOrderState, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumWorkOrderState>();

            Enumlst = from item in dic
                      select new SelectListItem()
                      {
                          Text = item.Value,
                          Value = Convert.ToString(item.Key.GetHashCode())
                      };

            //SelectListItem NewItem = new SelectListItem()
            //{
            //    Text = "",
            //    Value = ""
            //};

            //lst.Add(NewItem);

            foreach (SelectListItem item in Enumlst)
            {
                SelectListItem NewItem = new SelectListItem()
                {
                    Text = item.Text,
                    Value = item.Value
                };

                lst.Add(NewItem);
            }

            return lst;
        }


        /// <summary> 取得工单类型数组（类型值、类型名称） </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetOrderTypeList()
        {
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.CategoryName = '{0}'
                                           AND (Key.AttributeName = '{1}' or Key.AttributeName = '{2}')"
                                            , "OrderType"
                                            , "VALUE"
                                            , "Name"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);

                if (result.Code == 0)
                {
                    IList<SelectListItem> lst = new List<SelectListItem>();
                    int currItemOrder = 0;                                      //记录当前项目序号

                    //循环创建列表对象
                    foreach(BaseAttributeValue baseAtrValue in result.Data)
                    {
                        if (baseAtrValue.Key.AttributeName == "VALUE" || baseAtrValue.Key.AttributeName == "Name")
                        {
                            if (lst.Count == 0)
                            {
                                SelectListItem NewItem = new SelectListItem()
                                {
                                    Text = baseAtrValue.Value,
                                    Value = baseAtrValue.Value
                                };

                                lst.Add(NewItem);

                                currItemOrder = baseAtrValue.Key.ItemOrder;
                            }
                            else
                            {
                                //判断与当前列表节点主键是否一致
                                if (baseAtrValue.Key.ItemOrder == currItemOrder)
                                {
                                    if (baseAtrValue.Key.AttributeName == "VALUE")
                                    {
                                        lst[lst.Count - 1].Value = baseAtrValue.Value;
                                    }
                                    else
                                    {
                                        lst[lst.Count - 1].Text = baseAtrValue.Value;
                                    }
                                }
                                else
                                {
                                    SelectListItem NewItem = new SelectListItem()
                                    {
                                        Text = baseAtrValue.Value,
                                        Value = baseAtrValue.Value
                                    };

                                    lst.Add(NewItem);

                                    currItemOrder = baseAtrValue.Key.ItemOrder;
                                }
                            }      
                        }
                    }

                    return lst.ToList();
                }
            }

            return new List<SelectListItem>();
        }

        /// <summary> 取得工单类型名称 </summary>
        /// <param name="orderType">工单类型代码</param>
        /// <returns></returns>
        public string GetOrderTypeName(string orderType)
        {
            string orderTypeName = "";

            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                //取得对应的ID号
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.CategoryName = '{0}'
                                           AND (Key.AttributeName = '{1}' AND Value = '{2}')"
                                            , "OrderType"
                                            , "VALUE"
                                            , orderType),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> itemid = client.Get(ref cfg);

                if (itemid.Code == 0)
                {
                    //取得对应的ID号
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.CategoryName = '{0}'
                                           AND (Key.AttributeName = '{1}' AND Key.ItemOrder = {2})"
                                                , "OrderType"
                                                , "Name"
                                                , itemid.Data[0].Key.ItemOrder),
                        OrderBy = "Key.ItemOrder"
                    };

                    MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        orderTypeName = result.Data[0].Value;
                    }
                }
            }

            return orderTypeName;
        }
    }
}