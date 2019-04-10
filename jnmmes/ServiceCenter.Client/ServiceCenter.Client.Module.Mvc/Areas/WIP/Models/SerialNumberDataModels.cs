
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
    public class SerialNumberDataViewModel
    {
        public SerialNumberDataViewModel()
        {
            this.PageNo = 0;
            this.PageSize = 20;
            this.TotalRecords = 0;
        }

        [Display(Name = "包装号")]
        //[Required]
        public string PackageNo { get; set; }
        [Display(Name = "批次号")]
        public string LotNumber { get; set; }

        /// <summary>
        /// 工步名称。
        /// </summary>
        [Display(Name = "LotViewModel_RouteStepName", ResourceType = typeof(WIPResources.StringResource))]
        public string RouteStepName { get; set; }

        [Display(Name = "工单号")]
        public string OrderNumber { get; set; }

        [Display(Name = "标签条码格式")]
        public string MapType { get; set; }

        [Display(Name = "产品编码")]
        public string MaterialCode { get; set; }

        public int PageSize { get; set; }

        public int PageNo { get; set; }

        public int TotalRecords { get; set; }

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

        public IEnumerable<SelectListItem> GetMapTypeList()
        {
            IList<string> lst = new List<string>();
            lst.Add("正确");
            lst.Add("错误");
            return from item in lst
                   select new SelectListItem()
                   {
                       Text = item,
                       Value = item
                   };
        }
    }      
}