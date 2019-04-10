
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
using ServiceCenter.Client.Mvc.Areas.FMM.Models;
using System.Text;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Model.ZPVM;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class ChestMonitorViewModel
    {
        public ChestMonitorViewModel()
        {

        }   
    }

    public class ChestMonitorQueryViewModel
    {
        public ChestMonitorQueryViewModel()
        {

        }        

        [Display(Name = "是否自动刷新")]
        public bool IsAutoRefresh { get; set; }
        
        [Display(Name = "工单号")]
        public string OrderNumber { get; set; }

        [Display(Name = "产品编码")]
        public string MaterialCode { get; set; }

        [Display(Name = "功率档")]
        public string PowerName { get; set; }

        [Display(Name = "等级")]
        public string Grade { get; set; }

        [Display(Name = "花色")]
        public string Color { get; set; }

        [Display(Name = "电流档")]
        public string PowerSubCode { get; set; }

        public IEnumerable<SelectListItem> GetOrderNumberList()
        {
            IList<WorkOrder> lst = new List<WorkOrder>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = " OrderState = 0"
            };
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
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
    }
}