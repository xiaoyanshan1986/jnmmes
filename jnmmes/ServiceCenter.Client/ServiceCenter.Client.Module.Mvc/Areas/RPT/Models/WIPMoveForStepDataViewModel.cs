using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Models
{
    public class WIPMoveForStepDataViewModel
    {
        public WIPMoveForStepDataViewModel()
        {
            this.StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.EndTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            this.StartDate = DateTime.Now.ToString("yyyy-MM-dd");
            this.EndDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        }

        [Required]
        [Display(Name = "开始日期")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "结束日期")]
        public string EndDate { get; set; }
        [Required]
        [Display(Name = "开始时间")]
        public string StartTime { get; set; }

        [Required]
        [Display(Name = "结束时间")]
        public string EndTime { get; set; }
        [Required]
        [Display(Name = "物料编码")]
        public string MaterialCode { get; set; }
        [Required]
        [Display(Name = "工序名称")]
        public string StepName { get; set; }
        [Display(Name = "车间名称")]
        public string LocationName { get; set; }
        [Display(Name = "班别名称")]
        public string ShiftName { get; set; }
        [Display(Name = "工单号")]
        public string OrderNumber { get; set; }

        public IEnumerable<SelectListItem> GetProductionLineList(string RouteStepName)
        {
            //获取生产线。
            IList<Resource> lstResource = new List<Resource>();
            IList<ProductionLine> lst = new List<ProductionLine>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"LocationName='{0}' ",
                                            RouteStepName)
            };
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }

            return from item in lst
                   where lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = string.Format("{0}[{1}]", item.Name, item.Key),
                       Value = item.Key
                   };
        }

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

        public IEnumerable<SelectListItem> GetLocation()
        {
            IList<Location> lst = new List<Location>();
            //获取车间名称
            using (LocationServiceClient client = new LocationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = " Level='2'"
                };

                MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
                if (result.Code <= 0)
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