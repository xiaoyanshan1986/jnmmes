using ServiceCenter.Client.Mvc.Areas.FMM.Models;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{
    public class IVTestDataForPackageQueryViewModel
    {
        public IVTestDataForPackageQueryViewModel()
        {
            this.PageNo = 0;
            this.PageSize = 20;
            this.TotalRecords = 0;
        }
        [Display(Name = "包装号")]
        //[Required]
        public String PackageNo { get; set; }
        [Display(Name = "批次号")]
        public String LotNumber { get; set; }
     
        //[Display(Name = "入库单号")]
        //public String DeliveryOrderNo { get; set; }
        public int PageSize { get; set; }

        public int PageNo { get; set; }

        public int TotalRecords { get; set; }
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

    public class IVTestDataForPackageViewModel
    {
        public IVTestDataForPackageViewModel()
        {
            this.PageNo = 0;
            this.PageSize = 20;
            this.TotalRecords = 0;
        }
        [Display(Name = "包装号")]
        //[Required]
        public String PackageNo { get; set; }
        [Display(Name = "批次号")]
        public String LotNumber { get; set; }

        public int PageSize { get; set; }

        public int PageNo { get; set; }

        public int TotalRecords { get; set; }

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

        public String GetType(String materialCode)
        {
            MaterialQueryViewModel model = new MaterialQueryViewModel();
            model.Code = materialCode;
            StringBuilder where = new StringBuilder();
            using (MaterialServiceClient client = new MaterialServiceClient())
            {

                where.AppendFormat("  Key = '{0}'", model.Code);
                PagingConfig cfg = new PagingConfig()
                         {
                             OrderBy = "Key",
                             Where = where.ToString()
                         };
                MethodReturnResult<IList<Material>> result = client.Get(ref cfg);

                //List<Material>  MaterialList =result.Data.ToList<Material>();
                String ProductType = null;
                if (result.Code==0)
                {
                                    string barcode = result.Data[0].Key;
                string qty = result.Data[0].MainRawQtyPerLot.ToString();
                ProductType = string.Format("JNM{0}{1}"
                                    , barcode.StartsWith("1201") ? "M" : "P"
                                    , qty);
                }

                return ProductType;
            }
        }
    }

}