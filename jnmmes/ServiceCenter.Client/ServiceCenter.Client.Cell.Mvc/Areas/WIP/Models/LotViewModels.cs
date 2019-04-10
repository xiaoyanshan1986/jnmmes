
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
    public class LotQueryViewModel
    {
        public LotQueryViewModel()
        {
            this.DeletedFlag = false;
            this.HoldFlag = false;
            this.StartCreateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.EndCreateTime = DateTime.Now.AddDays(1);
        }

        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string LotNumber { get; set; }

        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string LotNumber1 { get; set; }

        [Display(Name = "LotViewModel_OrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string OrderNumber { get; set; }
        [Display(Name = "LotViewModel_MaterialCode", ResourceType = typeof(WIPResources.StringResource))]
        public string MaterialCode { get; set; }
        [Display(Name = "LotViewModel_LocationName", ResourceType = typeof(WIPResources.StringResource))]
        public string LocationName { get; set; }
        [Display(Name = "LotViewModel_LineCode", ResourceType = typeof(WIPResources.StringResource))]
        public string LineCode { get; set; }
        [Display(Name = "LotViewModel_RouteStepName", ResourceType = typeof(WIPResources.StringResource))]
        public string RouteStepName { get; set; }
        [Display(Name = "LotViewModel_PackageNo", ResourceType = typeof(WIPResources.StringResource))]
        public string PackageNo { get; set; }
        [Display(Name = "LotViewModel_StateFlag", ResourceType = typeof(WIPResources.StringResource))]
        public EnumLotState? StateFlag { get; set; }
        [Display(Name = "LotViewModel_HoldFlag", ResourceType = typeof(WIPResources.StringResource))]
        public bool? HoldFlag { get; set; }
        [Display(Name = "LotViewModel_DeletedFlag", ResourceType = typeof(WIPResources.StringResource))]
        public bool? DeletedFlag { get; set; }
        [Display(Name = "LotViewModel_StartCreateTime", ResourceType = typeof(WIPResources.StringResource))]
        public DateTime? StartCreateTime { get; set; }
        [Display(Name = "LotViewModel_EndCreateTime", ResourceType = typeof(WIPResources.StringResource))]
        public DateTime? EndCreateTime { get; set; }


        public IEnumerable<SelectListItem> GetLocationNameList()
        {
            IList<Location> lst = new List<Location>();
            PagingConfig cfg=new PagingConfig(){
                IsPaging=false,
                Where=string.Format("Level='{0}'",Convert.ToInt32(LocationLevel.Room))
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

        public IEnumerable<SelectListItem> GetRouteOperationNameList()
        {
            IList<RouteOperation> lst = new List<RouteOperation>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available)),
                OrderBy = "SortSeq"
            };
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
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

        public IEnumerable<SelectListItem> GetStateFlagList()
        {
            IDictionary<EnumLotState, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumLotState>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetBoolList()
        {
            Dictionary<bool, string> dic = new Dictionary<bool, string>();
            dic.Add(true, StringResource.Yes);
            dic.Add(false, StringResource.No);

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }
    }
    public class LotViewModel
    {
        public LotViewModel()
        {
        }
        /// <summary>
        /// 批次号
        /// </summary>
        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string LotNumber { get; set; }
        /// <summary>
        /// 容器号。
        /// </summary>
        [Display(Name = "LotViewModel_ContainerNo", ResourceType = typeof(WIPResources.StringResource))]
        public string ContainerNo { get; set; }
        /// <summary>
        /// 批次类型。
        /// </summary>
        [Display(Name = "LotViewModel_LotType", ResourceType = typeof(WIPResources.StringResource))]
        public EnumLotType LotType { get; set; }
        /// <summary>
        /// 原始工单号。
        /// </summary>
        [Display(Name = "LotViewModel_OriginalOrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string OriginalOrderNumber { get; set; }
        /// <summary>
        /// 当前工单号。
        /// </summary>
        [Display(Name = "LotViewModel_OrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string OrderNumber { get; set; }
        /// <summary>
        /// 物料编码。
        /// </summary>
        [Display(Name = "LotViewModel_MaterialCode", ResourceType = typeof(WIPResources.StringResource))]
        public string MaterialCode { get; set; }
        /// <summary>
        /// 等级。
        /// </summary>
        [Display(Name = "LotViewModel_Grade", ResourceType = typeof(WIPResources.StringResource))]
        public string Grade { get; set; }
        /// <summary>
        /// 花色。
        /// </summary>
        [Display(Name = "LotViewModel_Color", ResourceType = typeof(WIPResources.StringResource))]
        public string Color { get; set; }
        /// <summary>
        /// 优先级。
        /// </summary>
        [Display(Name = "LotViewModel_Priority", ResourceType = typeof(WIPResources.StringResource))]
        public EnumLotPriority Priority { get; set; }
        /// <summary>
        /// 初始数量。
        /// </summary>
        [Display(Name = "LotViewModel_InitialQuantity", ResourceType = typeof(WIPResources.StringResource))]
        public double InitialQuantity { get; set; }
        /// <summary>
        /// 当前数量。
        /// </summary>
        [Display(Name = "LotViewModel_Quantity", ResourceType = typeof(WIPResources.StringResource))]
        public double Quantity { get; set; }
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
        [Display(Name = "LotViewModel_RouteEnterpriseName", ResourceType = typeof(WIPResources.StringResource))]
        public string RouteEnterpriseName { get; set; }
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        [Display(Name = "LotViewModel_RouteName", ResourceType = typeof(WIPResources.StringResource))]
        public string RouteName { get; set; }
        /// <summary>
        /// 工步名称。
        /// </summary>
        [Display(Name = "LotViewModel_RouteStepName", ResourceType = typeof(WIPResources.StringResource))]
        public string RouteStepName { get; set; }
        /// <summary>
        /// 生产线代码。
        /// </summary>
        [Display(Name = "LotViewModel_LineCode", ResourceType = typeof(WIPResources.StringResource))]
        public string LineCode { get; set; }
        /// <summary>
        /// 设备代码。
        /// </summary>
        [Display(Name = "LotViewModel_EquipmentCode", ResourceType = typeof(WIPResources.StringResource))]
        public string EquipmentCode { get; set; }
        /// <summary>
        /// 开始等待时间。
        /// </summary>
        [Display(Name = "LotViewModel_StartWaitTime", ResourceType = typeof(WIPResources.StringResource))]
        public DateTime? StartWaitTime { get; set; }
        /// <summary>
        /// 开始处理时间。
        /// </summary>
        [Display(Name = "LotViewModel_StartProcessTime", ResourceType = typeof(WIPResources.StringResource))]
        public DateTime? StartProcessTime { get; set; }
        /// <summary>
        /// 是否主批次。
        /// </summary>
        [Display(Name = "LotViewModel_IsMainLot", ResourceType = typeof(WIPResources.StringResource))]
        public bool IsMainLot { get; set; }
        /// <summary>
        /// 拆分状态。
        /// </summary>
        [Display(Name = "LotViewModel_SplitFlag", ResourceType = typeof(WIPResources.StringResource))]
        public bool SplitFlag { get; set; }
        /// <summary>
        /// 返修标志。 0:未返修 >0：返修次数
        /// </summary>
        [Display(Name = "LotViewModel_RepairFlag", ResourceType = typeof(WIPResources.StringResource))]
        public int RepairFlag { get; set; }
        /// <summary>
        /// 返工标志。0:未返工 >0：返工次数
        /// </summary>
        [Display(Name = "LotViewModel_ReworkFlag", ResourceType = typeof(WIPResources.StringResource))]
        public int ReworkFlag { get; set; }
        /// <summary>
        /// 暂停状态。
        /// </summary>
        [Display(Name = "LotViewModel_HoldFlag", ResourceType = typeof(WIPResources.StringResource))]
        public bool HoldFlag { get; set; }
        /// <summary>
        /// 出货标志。
        /// </summary>
        [Display(Name = "LotViewModel_ShippedFlag", ResourceType = typeof(WIPResources.StringResource))]
        public bool ShippedFlag { get; set; }
        /// <summary>
        /// 包装标记。
        /// </summary>
        [Display(Name = "LotViewModel_PackageFlag", ResourceType = typeof(WIPResources.StringResource))]
        public bool PackageFlag { get; set; }
        /// <summary>
        /// 包装号。
        /// </summary>
        [Display(Name = "LotViewModel_PackageNo", ResourceType = typeof(WIPResources.StringResource))]
        public string PackageNo { get; set; }
        /// <summary>
        /// 结束删除标志。
        /// </summary>
        [Display(Name = "LotViewModel_DeletedFlag", ResourceType = typeof(WIPResources.StringResource))]
        public bool DeletedFlag { get; set; }
        /// <summary>
        /// 批次操作状态。
        /// </summary>
        [Display(Name = "LotViewModel_StateFlag", ResourceType = typeof(WIPResources.StringResource))]
        public EnumLotState StateFlag { get; set; }
        /// <summary>
        /// 状态。
        /// </summary>
        [Display(Name = "LotViewModel_Status", ResourceType = typeof(WIPResources.StringResource))]
        public EnumObjectStatus Status { get; set; }
        /// <summary>
        /// 操作计算机名称。
        /// </summary>
        [Display(Name = "LotViewModel_OperateComputer", ResourceType = typeof(WIPResources.StringResource))]
        public string OperateComputer { get; set; }
        /// <summary>
        /// 上一线别代码。
        /// </summary>
        [Display(Name = "LotViewModel_PreLineCode", ResourceType = typeof(WIPResources.StringResource))]
        public string PreLineCode { get; set; }
        /// <summary>
        /// 返工操作人。
        /// </summary>
        [Display(Name = "LotViewModel_Reworker", ResourceType = typeof(WIPResources.StringResource))]
        public string Reworker { get; set; }
        /// <summary>
        /// 返工时间。
        /// </summary>
        [Display(Name = "LotViewModel_ReworkTime", ResourceType = typeof(WIPResources.StringResource))]
        public DateTime? ReworkTime { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        public string Description { get; set; }
        /// <summary>
        /// 区域名称。
        /// </summary>
        [Display(Name = "LotViewModel_LocationName", ResourceType = typeof(WIPResources.StringResource))]
        public string LocationName { get; set; }
        /// <summary>
        /// 创建人。
        /// </summary>
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 编辑人。
        /// </summary>
        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        public IList<LotAttribute> GetLotAttributes(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging=false,
                    Where=string.Format("Key.LotNumber='{0}'",lotNumber)
                };
                MethodReturnResult<IList<LotAttribute>> result = client.GetAttribute(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data;
                }
            }
            return null;
        }

        public Lot GetLot(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                MethodReturnResult<Lot> result = client.Get(lotNumber);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data;
                }
            }
            return null;
        }
    }

    public class LotAttributeViewModel
    {
        /// <summary>
        /// 批次号
        /// </summary>
        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string LotNumber { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        [Display(Name = "AttributeName", ResourceType = typeof(WIPResources.StringResource))]
        public string AttributeName { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        [Display(Name = "AttributeValue", ResourceType = typeof(WIPResources.StringResource))]
        public string AttributeValue { get; set; }
        /// <summary>
        /// 编辑人。
        /// </summary>
        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }
    }
}