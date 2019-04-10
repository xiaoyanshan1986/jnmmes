using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.RPT;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QMResources = ServiceCenter.Client.Mvc.Resources.RPT;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Models
{
    public class SemiProductionViewModels
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public SemiProductionViewModels()
        {
            this.StartDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.EndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 车间
        /// </summary>
        [Display(Name = "SemiProductionViewModel_LocationName", ResourceType = typeof(QMResources.StringResource))]
        public string LocationName { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [Display(Name = "SemiProductionViewModel_Grade", ResourceType = typeof(QMResources.StringResource))]
        public string Grade { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        [Display(Name = "SemiProductionViewModel_IsProdReport", ResourceType = typeof(QMResources.StringResource))]
        public string IsProdReport { get; set; }

        /// <summary>
        /// 开始日期。
        /// </summary>
        [Required]
        [Display(Name = "SemiProductionViewModel_StartDate", ResourceType = typeof(QMResources.StringResource))]
        public string StartDate { get; set; }
        /// <summary>
        /// 结束日期。
        /// </summary>
        [Required]
        [Display(Name = "SemiProductionViewModel_EndDate", ResourceType = typeof(QMResources.StringResource))]
        public string EndDate { get; set; }
        public string CurDay { get; set; }
        public string LineCode { get; set; }

        public string LotNumber { get; set; }
        public string PosX { get; set; }
        public string PosY { get; set; }
        public string StepName { get; set; }

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

        public IEnumerable<SelectListItem> Gettype()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("0", "半成品");
            dic.Add("1", "成品");
            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Key
                   };
        }

        public IEnumerable<SelectListItem> GetGradeList()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='Grade' AND Key.AttributeName='VALUE'"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstValues = result.Data;
                }
            }
            return from item in lstValues
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Value
                   };
        }
        public IEnumerable<SelectListItem> GetProductionLineList()
        {
            IList<ProductionLine> lst = new List<ProductionLine>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false
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
                   select new SelectListItem()
                   {
                       Text = item.Name,
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
        public IEnumerable<SelectListItem> GetPosX()
        {
            IList<string> lis = new List<string>();
            lis.Add("01");
            lis.Add("02");
            lis.Add("03");
            lis.Add("04");
            lis.Add("05");
            lis.Add("06");
            lis.Add("07");
            lis.Add("08");
            lis.Add("09");
            lis.Add("10");
            lis.Add("11");
            lis.Add("12");
            return from item in lis
                   select new SelectListItem()
                   {
                       Text = item,
                       Value = item
                   };
        }
        public IEnumerable<SelectListItem> GetPosY()
        {
            IList<string> lis = new List<string>();
            lis.Add("1");
            lis.Add("2");
            lis.Add("3");
            lis.Add("4");
            lis.Add("5");
            lis.Add("6");          
            return from item in lis
                   select new SelectListItem()
                   {
                       Text = item,
                       Value = item
                   };
        }
    }
}