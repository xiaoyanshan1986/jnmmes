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

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{

    public class CheckDataQueryViewModel 
    {
        public CheckDataQueryViewModel()
        {
            this.StartCheckTime = DateTime.Now.ToString("yyyy/MM/dd");
            this.EndCheckTime = DateTime.Now.AddDays(1).Date.ToString("yyyy/MM/dd");
        }


        [Display(Name = "CheckDataQueryViewModel_LineCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string LineCode { get; set; }

        [Display(Name = "LotNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        public string LotNumber { get; set; }

        [Display(Name = "CheckDataQueryViewModel_StartCheckTime", ResourceType = typeof(ZPVMResources.StringResource))]
        public string StartCheckTime { get; set; }

        [Display(Name = "CheckDataQueryViewModel_EndCheckTime", ResourceType = typeof(ZPVMResources.StringResource))]
        public string EndCheckTime { get; set; }

        
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
    }

    public class CheckDataViewModel
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

        public IVTestData GetIVTestData(string lotNumber)
        {
            using (IVTestDataServiceClient client = new IVTestDataServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo=0,
                    PageSize=1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1",lotNumber)
                };
                MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data.Count>0)
                {
                    return result.Data[0];
                }
            }
            return null;
        }

        public LotTransactionHistory GetLotTransactionHistory(string key)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                MethodReturnResult<LotTransactionHistory> result = client.GetLotTransactionHistory(key);
                if (result.Code <= 0)
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
                MethodReturnResult<Lot> result = client.Get(lotNumber??string.Empty);
                if (result.Code <= 0)
                {
                    return result.Data;
                }
            }
            return null;
        }
    }

}