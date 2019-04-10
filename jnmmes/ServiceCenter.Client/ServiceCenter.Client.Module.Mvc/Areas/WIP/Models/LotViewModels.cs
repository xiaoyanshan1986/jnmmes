
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
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Service.Client.LSM;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class LotQueryViewModel
    {
        public LotQueryViewModel()
        {
            //this.DeletedFlag = false;
            //this.HoldFlag = false;
            //this.StartCreateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //this.EndCreateTime = DateTime.Now.AddDays(1);
            this.StartCreateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 000);
            //this.StartCreateTime = DateTime.Now.AddHours(-1);
            //this.EndCreateTime = DateTime.Now.AddDays(1);

            this.DeletedFlag = false;
            this.HoldFlag = false;
            //this.StartCreateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
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

        public List<string> RouteOperationList()
        {
            List<string> lst = new List<string>();
            //获取用户拥有权限的工序。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.RouteOperation);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;

                    for (int i = 0; i < lstResource.Count; i++)
                    {
                        lst.Add(result.Data.ToList<Resource>()[i].Name);
                    }
                }
            }
            return lst;

        }
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


        public LotBOM GetLotCellMaterial(string lotNumber)
        {
            LotBOM lotBOMObj = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo=0,
                    PageSize=1,
                    //Where = string.Format("Key.LotNumber='{0}' AND Key.ItemNo=1", lotNumber)
                    Where = string.Format("Key.LotNumber='{0}' AND (MaterialCode Like '11%'  OR MaterialCode LIKE '1803%'  OR MaterialCode LIKE '2511%') ", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data !=null && result.Data.Count > 0)
                {
                    lotBOMObj = result.Data[0];
                }
            }
            return lotBOMObj;
        }

        #region 获得电池片供应商
        public Supplier GetLotCellMaterialSupplier(string lotNumber)
        {
            Lot Lot = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}'", lotNumber)
                };
                MethodReturnResult<Lot> result = client.Get(lotNumber);
                if (result.Code <= 0 && result.Data != null)
                {
                    Lot = result.Data;
                }
            }

            LotBOM lotBOMGlass = null;
            string key = "";
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'110%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMGlass = result.Data[0];
                }
                else
                {
                    lotBOMGlass = null;

                }
            }
            Supplier sCell = null;
            using (SupplierServiceClient client = new SupplierServiceClient())
            {
                if (lotBOMGlass!= null)
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format(@"EXISTS (FROM LineStoreMaterialDetail as p
                                                      WHERE p.SupplierCode=self.Key
                                                      AND p.Key.MaterialLot='{0}'
                                                      AND p.Key.MaterialCode='{1}'
                                                      AND p.Key.LineStoreName='{2}'
                                                      AND p.Key.OrderNumber='{3}')"
                                                , lotBOMGlass.Key.MaterialLot != null ? lotBOMGlass.Key.MaterialLot : string.Empty
                                                , lotBOMGlass.MaterialCode != null ? lotBOMGlass.MaterialCode : string.Empty
                                                , lotBOMGlass.LineStoreName != null ? lotBOMGlass.LineStoreName : string.Empty
                                                , Lot.OrderNumber != null ? Lot.OrderNumber : string.Empty
                                                )
                    };
                    MethodReturnResult<IList<Supplier>> rst1 = client.Get(ref cfg);
                    if (rst1.Code <= 0 && rst1.Data.Count > 0)
                    {
                        sCell = rst1.Data[0];
                    }
                }
                else
                {
                    sCell = null;
                }
                return sCell;
            }
        }
        #endregion

        #region 获得电池片名称
        public Material GetLotCellMaterialName(string MaterialCode)
        {
            Material Material = null;
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                //PagingConfig cfg = new PagingConfig()
                //{
                //    PageNo = 0,
                //    PageSize = 1,
                //    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'110%'", lotNumber)
                //};
                MethodReturnResult<Material> result = client.Get(MaterialCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    Material = result.Data;
                }
            }
            return Material;
        }
        #endregion

        public Supplier GetSupplier(string supplierCode)
        {
            Supplier s = null;
            using (SupplierServiceClient client = new SupplierServiceClient())
            {
                MethodReturnResult<Supplier> result = client.Get(supplierCode);
                if (result.Code <= 0 && result.Data!=null)
                {
                    s = result.Data;
                }
            }
            return s;
        }

        public LineStoreMaterialDetail GetLineStoreMaterialDetail(LineStoreMaterialDetailKey key)
        {
            LineStoreMaterialDetail obj = null;
            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                MethodReturnResult<LineStoreMaterialDetail> result = client.GetDetail(key);
                if (result.Code <= 0 && result.Data != null)
                {
                    obj = result.Data;
                }
            }
            return obj;
        }
        public IEnumerable<SelectListItem> GetProductionLineList()
        {
            //获取用户拥有权限的生产线。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.ProductionLine);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }
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
                   where lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = string.Format("{0}[{1}]", item.Name, item.Key),
                       Value = item.Key
                   };
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