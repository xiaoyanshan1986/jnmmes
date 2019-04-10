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
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{

    public class OemDataViewModel
    {
        public WorkOrder GetWorkOrder(string workOrder)
        {
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key='{0}'", workOrder)
                };
                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null & result.Data.Count > 0)
                {
                    return result.Data[0];
                }
            }
            return null;
        }

        public OemDataViewModel()
        {

        }
        [Display(Name = "OemDataViewModel_LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string LotNumber { get; set; }

        [Display(Name = "OemDataViewModel_OrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "OemDataViewModel_PnName", ResourceType = typeof(WIPResources.StringResource))]
        public string PnName { get; set; }

        [Display(Name = "OemDataViewModel_PsSubCode", ResourceType = typeof(WIPResources.StringResource))]
        public string PsSubCode { get; set; }

        [Required]
        [Display(Name = " OemDataViewModel_Grade", ResourceType = typeof(WIPResources.StringResource))]
        public string Grade { get; set; }

        [Required]
        [Display(Name = " OemDataViewModel_Color", ResourceType = typeof(WIPResources.StringResource))]
        public string Color { get; set; }

        [Required]
        [Display(Name = " OemDataViewModel_PackageNo", ResourceType = typeof(WIPResources.StringResource))]
        public String PackageNo { get; set; }

        [Required]
        [Display(Name = " OemDataViewModel_Status", ResourceType = typeof(WIPResources.StringResource))]
        public String Status { get; set; }
    }           
}