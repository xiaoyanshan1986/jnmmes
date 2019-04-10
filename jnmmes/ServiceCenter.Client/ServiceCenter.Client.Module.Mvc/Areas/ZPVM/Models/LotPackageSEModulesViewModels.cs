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
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{

    public class LotPackageSEModulesViewModel
    {
        public LotPackageSEModulesViewModel()
        {
            this.StartCreateTime = DateTime.Now.ToString("yyyy/MM/dd");
            this.EndCreateTime = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd");
        }

        [Display(Name = "PackageNo", ResourceType = typeof(ZPVMResources.StringResource))]
        public string PackageNo { get; set; }

        [Display(Name = "PackageNo", ResourceType = typeof(ZPVMResources.StringResource))]
        public string PackageNo1 { get; set; }

        [Display(Name = "LotPackageQueryViewModel_OrderNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "LotPackageQueryViewModel_StartCreateTime", ResourceType = typeof(ZPVMResources.StringResource))]
        public string StartCreateTime { get; set; }

        [Display(Name = "LotPackageQueryViewModel_EndCreateTime", ResourceType = typeof(ZPVMResources.StringResource))]
        public string EndCreateTime { get; set; }

        public Lot GetLotData(string packageNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    Where = string.Format("PackageNo='{0}'", packageNumber),
                    IsPaging=false
                };
                MethodReturnResult<IList<Lot>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null && result.Data.Count>0)
                {
                    return result.Data.FirstOrDefault();
                }
            }
            return null;
        }
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
        public string GetProductTypes(string materialCode, string orderNumber, string powersetCode, int itemNo,Lot lot)
        {
            string productType = string.Empty;
            Material material = null;
            string wop = "";

            //WorkOrderPowerset wop = null;           
            //using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            //{
            //    MethodReturnResult<WorkOrderPowerset> result = client.Get(new WorkOrderPowersetKey()
            //    {
            //        ItemNo = itemNo,
            //        OrderNumber = orderNumber,
            //        MaterialCode = materialCode,
            //        Code = powersetCode
            //    });

            //    if (result.Code == 0 && result.Data != null)
            //    {
            //        wop = result.Data;
            //    }               
            //}
            wop = GetPowersetName(lot.Key, powersetCode, itemNo);

            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(materialCode);
                MethodReturnResult<MaterialAttribute> resultOfMaterialAttr = new MethodReturnResult<MaterialAttribute>();
                MaterialAttributeServiceClient clientOfMattr = new MaterialAttributeServiceClient();
                if (result.Code == 0)
                {
                    material = result.Data;
                    if (wop != "")
                    {
                        int indexOfType = material.Name.IndexOf('-');
                        if (indexOfType >= 0)
                        {
                            productType = material.Name.Substring(0, indexOfType) + "-" + wop.Substring(0, 3);
                        }
                        else
                        {
                            productType = material.Name.Substring(0, 6) + "-" + wop.Substring(0, 3);
                        }
                        if (material.Name.Contains("PV"))
                        {
                            MaterialAttributeKey materialAttributeKey = new MaterialAttributeKey()
                            {
                                MaterialCode = material.Key,
                                AttributeName = "ProductType"
                            };
                            resultOfMaterialAttr = clientOfMattr.Get(materialAttributeKey);
                            if (resultOfMaterialAttr.Data != null)
                            {
                                string valueOf = resultOfMaterialAttr.Data.Value.Trim();
                                int indexOfType1 = valueOf.IndexOf('*');
                                if (lot.Attr3 != null && lot.Attr3 != "")
                                {
                                    productType = string.Format("S{0}{1}-{2}{3}-1WA"
                                            , valueOf.Substring(0, indexOfType1)
                                            , wop.Substring(0, 3)
                                            , material.MainRawQtyPerLot
                                            , valueOf.Substring(valueOf.Length - 3)
                                        );
                                }
                                else
                                {
                                    productType = string.Format("{0}{1}-{2}{3}-1WA"
                                            , valueOf.Substring(0, indexOfType1)
                                            , wop.Substring(0, 3)
                                            , material.MainRawQtyPerLot
                                            , valueOf.Substring(valueOf.Length - 3)
                                        );
                                }
                            }
                            else
                            {
                                productType = "";
                            }
                        }
                    }                   
                }
            }
            return productType;
        }

        public string GetProductType(string materialCode, string orderNumber, string powersetCode, int itemNo, Lot lot)
        {
            string productType = string.Empty;
            Material material = null;
            string wop = "";

            wop = GetPowersetName(lot.Key, powersetCode, itemNo);

            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(materialCode);
                MethodReturnResult<MaterialAttribute> resultOfMaterialAttr = new MethodReturnResult<MaterialAttribute>();
                MaterialAttributeServiceClient clientOfMattr = new MaterialAttributeServiceClient();
                if (result.Code == 0)
                {
                    material = result.Data;
                    if (wop != "")
                    {
                        int indexOfType = material.Name.IndexOf('-');
                        if (indexOfType >= 0)
                        {
                            productType = material.Name.Substring(0, indexOfType) + "-" + wop.Substring(0, 3);
                        }
                        else
                        {
                            productType = material.Name.Substring(0, 6) + "-" + wop.Substring(0, 3);
                        }
                        if (material.Name.Contains("PV"))
                        {
                            MaterialAttributeKey materialAttributeKey = new MaterialAttributeKey()
                            {
                                MaterialCode = material.Key,
                                AttributeName = "ProductType"
                            };
                            resultOfMaterialAttr = clientOfMattr.Get(materialAttributeKey);
                            if (resultOfMaterialAttr.Data != null)
                            {
                                string valueOf = resultOfMaterialAttr.Data.Value.Trim();
                                int indexOfType1 = valueOf.IndexOf('*');
                                productType = string.Format("{0}{1}-{2}{3}"
                                           , valueOf.Substring(0, indexOfType1)
                                           , wop.Substring(0, 3)
                                           , material.MainRawQtyPerLot
                                           , valueOf.Substring(valueOf.Length - 3));
                            }
                            else
                            {
                                productType = "";
                            }
                        }
                    }
                }
            }
            return productType;
        }
    }
}