using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.WIP;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{

    public class IVTestDataQueryViewModel
    {
        public IVTestDataQueryViewModel()
        {
            this.PageNo = 0;
            this.PageSize = 20;
            this.TotalRecords = 0;
            this.IsDefault = true;
            this.StartTestTime = DateTime.Now.ToString("yyyy-MM-dd");
            this.EndTestTime = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");
        }

        [Display(Name = "LotNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        public string LotNumber { get; set; }

        [Display(Name = "IVTestDataQueryViewModel_StartTestTime", ResourceType = typeof(ZPVMResources.StringResource))]
        public string StartTestTime { get; set; }

        [Display(Name = "IVTestDataQueryViewModel_EndTestTime", ResourceType = typeof(ZPVMResources.StringResource))]
        public string EndTestTime { get; set; }

        [Display(Name = "IVTestDataQueryViewModel_EquipmentCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string EquipmentCode { get; set; }

        [Display(Name = "IVTestDataQueryViewModel_IsDefault", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool? IsDefault { get; set; }

        [Display(Name = "IVTestDataQueryViewModel_IsPrint", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool? IsPrint { get; set; }
        [Display(Name = "IVTestDataQueryViewModel_IsJZ", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool? IsJZ { get; set; }
        [Display(Name = "IVTestDataQueryViewModel_lineCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string lineCode { get; set; }

        public int PageSize { get; set; }

        public int PageNo { get; set; }

        public int TotalRecords { get; set; }

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

        public string GetEfficiency(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                MethodReturnResult<Lot> result = client.Get(lotNumber);
                if (result.Code == 0)
                {
                    return result.Data.Attr1;
                }
            }
            return string.Empty;
        }

        /// <summary> 获取层压机信息 </summary>
        /// <param name="lotNumber">批次号</param>
        /// <returns></returns>
        public LotAttribute GetLaminator(string lotNumber)
        {
            LotAttribute LotAttribute = null;       //批次属性
            Equipment Equipment = null;             //批次设备

            //获取批次属性
            using (LotAttributeServiceClient client = new LotAttributeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND Key.AttributeName ='LayerEquipmentNo'", lotNumber)
                };
                MethodReturnResult<IList<LotAttribute>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    LotAttribute = result.Data[0];

                    //获取批次设备
                    Equipment = GetEquipment(LotAttribute.AttributeValue);

                    if (Equipment != null)
                    {
                        LotAttribute.AttributeValue = Equipment.Name;
                    }
                    else
                    {
                        LotAttribute.AttributeValue = LotAttribute.AttributeValue;
                    }

                }

            }
            return LotAttribute;

        }

        /// <summary> 获取设备信息 </summary>
        /// <param name="lotNumber">批次号</param>
        /// <returns></returns>
        public Equipment GetEquipment(string equipment)
        {
            Equipment Equipment = null;
           
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key='{0}'  ", equipment)
                };
                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data.Count > 0)
                {
                    Equipment = result.Data[0];
                }

                return Equipment;
            }

        }    


    }

    public class IVTestDataViewModel
    {
        public string GetPowersetName(string lotNumber, string powersetCode, int itemNo)
        {
            string powerName = string.Empty;
            string cacheKey = string.Format("{0}_{1}", powersetCode, itemNo);
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
                        Code = powersetCode,
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
        public LotBOM GetLotCellMaterial(string lotNumber)
        {
            LotBOM lotBOMObj = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND  MaterialCode like'110%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMObj = result.Data[0];
                }
            }
            return lotBOMObj;
        }

        #region 获得批次电池片用料BOM信息
        public LotBOM GetLotCellBom(string lotNumber)
        {
            LotBOM lotBOMCell = null;
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
                    lotBOMCell = result.Data[0];
                }
            }
            return lotBOMCell;
        }
        #endregion

        #region 获得电池片供应商
        public Supplier GetLotCellMaterialSupplier(string lotNumber)
        {
            LotBOM lotBOMGlass = null;
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
            }

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

            Supplier sCell = null;
            using (SupplierServiceClient client = new SupplierServiceClient())
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
                                            , lotBOMGlass.Key.MaterialLot
                                            , lotBOMGlass.MaterialCode
                                            , lotBOMGlass.LineStoreName
                                            , Lot.OrderNumber != null ? Lot.OrderNumber : string.Empty
                                            )
                };
                MethodReturnResult<IList<Supplier>> rst = client.Get(ref cfg);
                if (rst.Code <= 0 && rst.Data.Count > 0)
                {
                    sCell = rst.Data[0];
                }
                return sCell;
            }
        }
        #endregion
        public LotBOM GetLotGlassMaterial(string lotNumber)
        {
            LotBOM lotBOMGlass = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130303%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMGlass = result.Data[0];
                }
            }
            return lotBOMGlass;
        }

        #region 获得批次玻璃用料BOM信息
        public LotBOM GetLotGlassBom(string lotNumber)
        {
            LotBOM lotBOMGlass = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130303%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMGlass = result.Data[0];
                }
            }
            return lotBOMGlass;
        }
        #endregion

        #region 获得玻璃供应商
        public Supplier GetLotGlassMaterialSupplier(string lotNumber)
        {
            LotBOM lotBOMGlass = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130303%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMGlass = result.Data[0];
                }
            }

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

            Supplier sGlass = null;
            using (SupplierServiceClient client = new SupplierServiceClient())
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
                                            , lotBOMGlass.Key.MaterialLot
                                            , lotBOMGlass.MaterialCode
                                            , lotBOMGlass.LineStoreName
                                            , Lot.OrderNumber != null ? Lot.OrderNumber : string.Empty
                                            )
                };
                MethodReturnResult<IList<Supplier>> rst = client.Get(ref cfg);
                if (rst.Code <= 0 && rst.Data.Count > 0)
                {
                    sGlass = rst.Data[0];
                }
                return sGlass;
            }
        }
        #endregion
        public LotBOM GetLotEvaMaterial(string lotNumber)
        {
            LotBOM lotBOMEva = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130304%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMEva = result.Data[0];
                }
            }
            return lotBOMEva;
        }

        #region 获得批次Eva用料BOM信息
        public LotBOM GetLotEvaBom(string lotNumber)
        {
            LotBOM lotBOMEva = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130304%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMEva = result.Data[0];
                }
            }
            return lotBOMEva;
        }
        #endregion

        #region 获得Eva供应商
        public Supplier GetLotEvaMaterialSupplier(string lotNumber)
        {
            LotBOM lotBOMGlass = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130304%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMGlass = result.Data[0];
                }
            }

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

            Supplier sEva = null;
            using (SupplierServiceClient client = new SupplierServiceClient())
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
                                            , lotBOMGlass.Key.MaterialLot
                                            , lotBOMGlass.MaterialCode
                                            , lotBOMGlass.LineStoreName
                                            , Lot.OrderNumber != null ? Lot.OrderNumber : string.Empty
                                            )
                };
                MethodReturnResult<IList<Supplier>> rst = client.Get(ref cfg);
                if (rst.Code <= 0 && rst.Data.Count > 0)
                {
                    sEva = rst.Data[0];
                }
                return sEva;
            }
        }
        #endregion
        public LotBOM GetLotHltMaterial(string lotNumber)
        {
            LotBOM lotBOMHlt = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130301%'", lotNumber)//130308
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMHlt = result.Data[0];
                }
            }
            return lotBOMHlt;
        }

        #region 获得批次互联条用料BOM信息
        public LotBOM GetLotHltBom(string lotNumber)
        {
            LotBOM lotBOMHlt = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130301%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMHlt = result.Data[0];
                }
            }
            return lotBOMHlt;
        }
        #endregion

        #region 获得互联条供应商
        public Supplier GetLotHltMaterialSupplier(string lotNumber)
        {
            LotBOM lotBOMGlass = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130301%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMGlass = result.Data[0];
                }
            }

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

            Supplier sHlt = null;
            using (SupplierServiceClient client = new SupplierServiceClient())
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
                                            , lotBOMGlass.Key.MaterialLot
                                            , lotBOMGlass.MaterialCode
                                            , lotBOMGlass.LineStoreName
                                            , Lot.OrderNumber != null ? Lot.OrderNumber : string.Empty
                                            )
                };
                MethodReturnResult<IList<Supplier>> rst = client.Get(ref cfg);
                if (rst.Code <= 0 && rst.Data.Count > 0)
                {
                    sHlt = rst.Data[0];
                }
                return sHlt;
            }
        }
        #endregion

        public LotBOM GetLotBBMaterial(string lotNumber)
        {
            LotBOM lotBOMBB = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130308%'", lotNumber)//130308
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMBB = result.Data[0];
                }
            }
            return lotBOMBB;
        }

        #region 获得批次背板用料BOM信息
        public LotBOM GetLotBBBom(string lotNumber)
        {
            LotBOM lotBOMBB = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130308%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMBB = result.Data[0];
                }
            }
            return lotBOMBB;
        }
        #endregion

        #region 获得背板供应商
        public Supplier GetLotBBMaterialSupplier(string lotNumber)
        {
            LotBOM lotBOMGlass = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND MaterialCode like'130308%'", lotNumber)
                };
                MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lotBOMGlass = result.Data[0];
                }
            }

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

            Supplier sBB = null;
            using (SupplierServiceClient client = new SupplierServiceClient())
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
                                            , lotBOMGlass.Key.MaterialLot
                                            , lotBOMGlass.MaterialCode
                                            , lotBOMGlass.LineStoreName
                                            , Lot.OrderNumber != null ? Lot.OrderNumber : string.Empty
                                            )
                };
                MethodReturnResult<IList<Supplier>> rst = client.Get(ref cfg);
                if (rst.Code <= 0 && rst.Data.Count > 0)
                {
                    sBB = rst.Data[0];
                }
                return sBB;
            }
        }
        #endregion
        public Lot GetLot(string lotNumber)
        {
            Lot lot = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key='{0}'", lotNumber)
                };
                MethodReturnResult<IList<Lot>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lot = result.Data[0];
                }
            }
            return lot;
        }

        public string GetEfficiency(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                MethodReturnResult<Lot> result = client.Get(lotNumber);
                if (result.Code == 0)
                {
                    return result.Data.Attr1;
                }
            }
            return string.Empty;
        }

        /// <summary> 获取层压机信息 </summary>
        /// <param name="lotNumber">批次号</param>
        /// <returns></returns>
        public LotAttribute GetLaminator(string lotNumber)
        {
            LotAttribute LotAttribute = null;
            using (LotAttributeServiceClient client = new LotAttributeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND Key.AttributeName ='LayerEquipmentNo'", lotNumber)
                };
                MethodReturnResult<IList<LotAttribute>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    LotAttribute = result.Data[0];

                    if (LotAttribute.AttributeValue == "EMCY2001")
                    {
                        LotAttribute.AttributeValue = "102B-1#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY2002")
                    {
                        LotAttribute.AttributeValue = "102B-2#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY2003")
                    {
                        LotAttribute.AttributeValue = "102B-3#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY2004")
                    {
                        LotAttribute.AttributeValue = "102B-4#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY2005")
                    {
                        LotAttribute.AttributeValue = "102B-5#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY2006")
                    {
                        LotAttribute.AttributeValue = "102B-6#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY2007")
                    {
                        LotAttribute.AttributeValue = "102B-7#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY2008")
                    {
                        LotAttribute.AttributeValue = "102B-8#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY2009")
                    {
                        LotAttribute.AttributeValue = "102B-9#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY2010")
                    {
                        LotAttribute.AttributeValue = "102B-10#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY1001")
                    {
                        LotAttribute.AttributeValue = "102A-1#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY1002")
                    {
                        LotAttribute.AttributeValue = "102A-2#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY1003")
                    {
                        LotAttribute.AttributeValue = "102A-3#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY1004")
                    {
                        LotAttribute.AttributeValue = "102A-4#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY1005")
                    {
                        LotAttribute.AttributeValue = "102A-5#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY1006")
                    {
                        LotAttribute.AttributeValue = "102A-6#";
                    }
                    else if (LotAttribute.AttributeValue == "EMCY1007")
                    {
                        LotAttribute.AttributeValue = "102A-7#";
                    }
                    else if (LotAttribute.AttributeValue == "")
                    {
                        LotAttribute.AttributeValue = "null";
                    }
                }
                else 
                {
                    result.Data=null;
                 
                }
                return LotAttribute;
            }

        }

        /// <summary> 获取设备信息 </summary>
        /// <param name="lotNumber">批次号</param>
        /// <returns></returns>
        public Equipment GetEquipment(string equipment)
        {
            Equipment Equipment = null;
            
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key='{0}'  ", equipment)
                };
                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data.Count > 0)
                {
                    Equipment = result.Data[0];
                }
            
                return Equipment;
            }

        }  
    }

}