using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.ZPVM;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.ZPVM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Model.ERP;
using ServiceCenter.MES.Service.Client.ERP;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{
    public class ChestViewModel
    {
        public ChestViewModel()
        {
            //this.FullQuantity = GetFullChestQty();
        }

        #region 声明变量
        [Display(Name = "ChestNo", ResourceType = typeof(ZPVMResources.StringResource))]
        public string ChestNo { get; set; }

        /// <summary>
        /// 满包数量
        /// </summary>
        [Required]
        [Display(Name = "ChestViewModel_FullQuantity", ResourceType = typeof(ZPVMResources.StringResource))]
        public double FullQuantity { get; set; }

        /// <summary>
        /// 当前数量
        /// </summary>
        [Required]
        [Display(Name = "ChestViewModel_CurrentQuantity", ResourceType = typeof(ZPVMResources.StringResource))]
        public double CurrentQuantity { get; set; }

        /// <summary>
        /// 柜当前状态
        /// </summary>
        [Display(Name = "柜当前状态")]
        public EnumChestState ChestState { get; set; }

        /// <summary>
        /// 是否完成入柜？
        /// </summary>
        public bool IsFinishPackage { get; set; }

        /// <summary>
        /// 是否尾柜？
        /// </summary>
        [Required]
        [Display(Name = "ChestViewModel_IsLastestPackageInChest", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool IsLastestPackageInChest { get; set; }

        [Display(Name = "PackageNo", ResourceType = typeof(ZPVMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string PackageNo { get; set; }

        /// <summary>
        /// 物料编码。
        /// </summary>
        [Display(Name = "LotViewModel_MaterialCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 等级。
        /// </summary>
        [Display(Name = "LotViewModel_Grade", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Grade { get; set; }

        /// <summary>
        /// 花色。
        /// </summary>
        [Display(Name = "LotViewModel_Color", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Color { get; set; }

        /// <summary>
        /// 库位。
        /// </summary>
        //[Required]
        [Display(Name = "库位")]
        public string StoreLocation { get; set; }

        /// <summary>
        /// 是否手动模式
        /// </summary>
        [Required]
        [Display(Name = "是否手动")]
        public bool IsManual { get; set; }
        #endregion

        #region 声明方法
        //获取基础数据库位表
        public IEnumerable<SelectListItem> GetStoreLocationList()
        {
            IList<BaseAttributeValue> lstResource = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='StoreLocation' AND Key.AttributeName='StoreLocationName'")
                };
                MethodReturnResult<IList<BaseAttributeValue>> lstStoreLocationName= client.Get(ref cfg);
                if (lstStoreLocationName.Data != null && lstStoreLocationName.Data.Count > 0)
                {
                    lstResource = lstStoreLocationName.Data;
                }
            }
            return from item in lstResource
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Value
                   };
        }

        //获取OEM组件信息
        public OemData GetOemData(string lotNumber)
        {
            using (OemDataServiceClient client = new OemDataServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key='{0}'", lotNumber)
                };
                MethodReturnResult<IList<OemData>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null & result.Data.Count > 0)
                {
                    return result.Data[0];
                }
            }
            return null;
        }

        //获取OEM组件的分档代码和分档项目号
        public List<string> GetCodeAndItemNo(OemData oemData)
        {
            List<string> dic = new List<string>();
            using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("PowerName='{0}' AND Key.OrderNumber='{1}'", oemData.PnName, oemData.OrderNumber.ToString().Trim().ToUpper())
                };
                MethodReturnResult<IList<WorkOrderPowerset>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null & result.Data.Count > 0)
                {
                    dic.Add(result.Data[0].Key.Code);
                    dic.Add(result.Data[0].Key.ItemNo.ToString());
                    //string iii = dic[0];
                    return dic;
                }
            }
            return null;
        }

        //获取入柜控制参数中满柜数量
        public double GetFullChestQty()
        {
            double qty = 0;
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='ChestInParameters' AND Key.AttributeName='FullChestQty'")
                };
                MethodReturnResult<IList<BaseAttributeValue>> fullChestQty = client.Get(ref cfg);
                if (fullChestQty.Data != null && fullChestQty.Data.Count > 0)
                {
                    qty = Convert.ToDouble(fullChestQty.Data[0].Value);
                }
            }            
            return qty;
        }

        //获取自制组件IV数据
        public IVTestData GetIVTestData(string lotNumber)
        {
            using (IVTestDataServiceClient client = new IVTestDataServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
                };
                MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null & result.Data.Count > 0)
                {
                    return result.Data[0];
                }
            }
            return null;
        }

        //获取自制组件功率名称
        public string GetPowersetName(string code, int itemNo)
        {
            string powerName = string.Empty;
            string cacheKey = string.Format("{0}_{1}", code, itemNo);
            if (HttpContext.Current.Cache[cacheKey] != null)
            {
                powerName = HttpContext.Current.Cache[cacheKey] as string;
            }
            else
            {
                using (PowersetServiceClient client = new PowersetServiceClient())
                {
                    MethodReturnResult<Powerset> result = client.Get(new PowersetKey()
                    {
                        Code = code,
                        ItemNo = itemNo
                    });
                    if (result.Code == 0)
                    {
                        powerName = result.Data.PowerName;
                        HttpContext.Current.Cache[cacheKey] = powerName;
                    }
                }
            }
            return powerName;
        }

        //获取自制组件批次信息
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

        //获取批次属性数据
        public IList<LotAttribute> GetLotAttributes(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.LotNumber='{0}'", lotNumber)
                };
                MethodReturnResult<IList<LotAttribute>> result = client.GetAttribute(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data;
                }
            }
            return null;
        }

        //获取托第一块明细数据
        public PackageDetail GetPackageDetail(string PackageNo)
        {
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.PackageNo ='{0}' and ItemNo=1", PackageNo)
                };
                MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count>0)
                {
                    return result.Data[0];
                }
            }
            return null;
        }

        //获取托数据
        public Package GetPackage(string PackageNo)
        {
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                MethodReturnResult<Package> result = client.Get(PackageNo);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data;
                }
            }
            return null;
        }
        #endregion
    }

    public class ChestDetailQueryViewModel
    {
        public ChestDetailQueryViewModel()
        {
            this.ChestDate = DateTime.Now.ToString("yyyy-MM-dd");
            this.PageNo = 0;
            this.PageSize = 20;
        }

        #region 声明变量
        [Display(Name = "柜号")]
        public string ChestNo { get; set; }

        [Display(Name = "包装号")]
        public string PackageNo { get; set; }

        [Display(Name = "工单号")]
        public string OrderNumber { get; set; }

        [Display(Name = "批次号")]
        public string LotNumber { get; set; }

        [Display(Name = "物料编码")]
        public string MaterialCode { get; set; }

        [Display(Name = "入柜日期")]
        public string ChestDate { get; set; }

        public int PageSize { get; set; }

        public int PageNo { get; set; }

        public int TotalRecords { get; set; }
        #endregion

        #region 声明方法

        //获取托数据
        public Package GetPackage(string PackageNo)
        {
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                MethodReturnResult<Package> result = client.Get(PackageNo);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data;
                }
            }
            return null;
        }

        //获取柜信息
        public Chest GetChest(string key)
        {
            using (PackageInChestServiceClient client = new PackageInChestServiceClient())
            {
                MethodReturnResult<Chest> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }

        //获取柜明细
        public ChestDetail GetChestDetail(ChestDetailKey key)
        {
            using (PackageInChestServiceClient client = new PackageInChestServiceClient())
            {
                MethodReturnResult<ChestDetail> rst = client.GetDetail(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }

        //获取自制组件IV数据
        public IVTestData GetIVTestData(string lotNumber)
        {
            using (IVTestDataServiceClient client = new IVTestDataServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
                };
                MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null & result.Data.Count > 0)
                {
                    return result.Data[0];
                }
            }
            return null;
        }

        //获取自制组件批次信息
        public Lot GetLotData(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                MethodReturnResult<Lot> result = client.Get(lotNumber);

                if (result.Code == 0 && result.Data != null)
                {
                    return result.Data;
                }
            }
            return null;
        }

        //获取功率名称
        public string GetPowersetName(string code, int itemNo)
        {
            string powerName = string.Empty;
            string cacheKey = string.Format("{0}_{1}", code, itemNo);
            if (HttpContext.Current.Cache[cacheKey] != null)
            {
                powerName = HttpContext.Current.Cache[cacheKey] as string;
            }
            else
            {
                using (PowersetServiceClient client = new PowersetServiceClient())
                {
                    MethodReturnResult<Powerset> result = client.Get(new PowersetKey()
                    {
                        Code = code,
                        ItemNo = itemNo
                    });
                    if (result.Code == 0)
                    {
                        powerName = result.Data.PowerName;
                        HttpContext.Current.Cache[cacheKey] = powerName;
                    }
                }
            }
            return powerName;
        }

        ////获取自制组件批次信息
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

        //获取批次属性数据
        public IList<LotAttribute> GetLotAttributes(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.LotNumber='{0}'", lotNumber)
                };
                MethodReturnResult<IList<LotAttribute>> result = client.GetAttribute(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data;
                }
            }
            return null;
        }

        //获取托明细
        public PackageDetail GetPackageDetail(string PackageNo)
        {
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.PackageNo ='{0}' and ItemNo=1", PackageNo)
                };
                MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data[0];
                }
            }
            return null;
        }

        //获取托号所在入库单明细
        public WOReportDetail GetWOReportDetail(string PackageNo)
        {
            WOReportClient client=new WOReportClient();
            MethodReturnResult<IList<WOReportDetail>> lstWOReportDetail = null;
            WOReportDetail woReportDetail = null;
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(" ObjectNumber = '{0}'",PackageNo)
            };
            lstWOReportDetail = client.GetWOReportDetail(ref cfg);
            if(lstWOReportDetail.Data!=null && lstWOReportDetail.Data.Count>0)
            {
                woReportDetail = lstWOReportDetail.Data[0];
            }
            return woReportDetail;
        }

        //获取托号所在入库单
        public WOReport GetWOReport(string BillCode)
        {
            WOReportClient client = new WOReportClient();
            WOReport woReport = null;
            MethodReturnResult<WOReport> mtWOReport = null;
            mtWOReport = client.GetWOReport(BillCode);
            if (mtWOReport != null)
            {
                woReport = mtWOReport.Data;
            }
            return woReport;
        }

        #endregion
    }
}