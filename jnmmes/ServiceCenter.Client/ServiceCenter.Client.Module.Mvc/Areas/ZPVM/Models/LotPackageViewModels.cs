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

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{

    public class ZPVMLotViewModel:LotViewModel
    {

        public IVTestData GetIVTestData(string lotNumber)
        {
            using (IVTestDataServiceClient client = new IVTestDataServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo=0,
                    PageSize=1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
                };
                MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data!=null & result.Data.Count>0)
                {
                    return result.Data[0];
                }
            }
            return null;
        }

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

        public List<string> GetCodeAndItemNo(OemData oemData)
        {
            List<string> dic = new List<string>();
            using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("PowerName='{0}' AND Key.OrderNumber='{1}'", oemData.PnName,oemData.OrderNumber.ToString().Trim().ToUpper())
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


        public string GetPowersetName(string code,int itemNo)
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
    }

    public class ZPVMLotPackageViewModel:LotPackageViewModel
    {
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
    }
}