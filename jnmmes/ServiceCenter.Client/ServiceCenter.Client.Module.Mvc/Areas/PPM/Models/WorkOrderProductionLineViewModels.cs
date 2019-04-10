
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.PPM;
using PPMResources = ServiceCenter.Client.Mvc.Resources.PPM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.PPM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Models
{
    public class WorkOrderProductionLineQueryViewModel
    {
        public WorkOrderProductionLineQueryViewModel()
        {

        }
        [Display(Name = "WorkOrderProductionLineQueryViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "WorkOrderProductionLineQueryViewModel_LineCode", ResourceType = typeof(PPMResources.StringResource))]
        public string LineCode { get; set; }
    }

    public class WorkOrderProductionLineViewModel
    {
        public WorkOrderProductionLineViewModel()
        {
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "WorkOrderProductionLineViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "WorkOrderProductionLineViewModel_LineCode", ResourceType = typeof(PPMResources.StringResource))]
        public string LineCode { get; set; }


        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        public IEnumerable<SelectListItem> GetLineCodeList()
        {
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key+"-"+item.Name,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }

        public ProductionLine GetProductionLine(string lineCode)
        {
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<ProductionLine> result = client.Get(lineCode);
                if (result.Code <= 0)
                {
                    return result.Data;
                }
            }
            return new ProductionLine();
        }
    }
}