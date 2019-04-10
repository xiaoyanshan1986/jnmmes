using ServiceCenter.MES.Model.FMM;
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
    public class LotMaterialListViewModel
    {
        
        public LotMaterialListViewModel()
        {
            //this.PageNo = 0;
            //this.PageSize = 20;
            this.StartCreateTime = DateTime.Now.AddDays(-7).Date;
            this.EndCreateTime = DateTime.Now.AddDays(1).Date;
            //this.TotalRecords = 0;
        }
       
               
        [Display(Name = "LotMaterialListViewModel_LocationName", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string LocationName { get; set; }

        [Display(Name = "LotMaterialListViewModel_OrderNumber", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "LotMaterialListViewModel_StartCreateTime", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public DateTime? StartCreateTime { get; set; }

        [Display(Name = "LotMaterialListViewModel_EndCreateTime", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public DateTime? EndCreateTime { get; set; }

        [Display(Name = "LotMaterialListViewModel_LotNumber",ResourceType=typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string LotNumber { get; set; }

        [Display(Name = "LotMaterialListViewModel_LotNumber",ResourceType=typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string LotNumber1 { get; set; }
        [Display(Name = "LotMaterialListViewModel_Activity", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string Activity { get; set; }
        [Display(Name = "LotMaterialListViewModel_RouteEnterpriseName", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string RouteEnterpriseName { get; set; }
   
        [Display(Name = "LotMaterialListViewModel_RouteName", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string RouteName { get; set; }
        [Display(Name = "LotMaterialListViewModel_RouteStepName", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string RouteStepName { get; set; }
   
        [Display(Name = "LotMaterialListViewModel_Computer", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string Computer { get; set; }
   
        [Display(Name = "LotMaterialListViewModel_Description", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string Description { get; set; }
        [Display(Name = "LotMaterialListViewModel_Creator", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string Creator { get; set; }
   
        [Display(Name = "LotMaterialListViewModel_CreateTime", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public DateTime? CreateTime { get; set; }
      
        [Display(Name = "LotMaterialListViewModel_Editor", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string Editor { get; set; }
   
        [Display(Name = "LotMaterialListViewModel_EditTime", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public DateTime? EditTime { get; set; }

        [Display(Name = "LotMaterialListViewModel_LineCode", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string LineCode { get; set; }

        [Display(Name = "LotMaterialListViewModel_StartTime", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public DateTime? StartTime { get; set; }

        [Display(Name = "LotMaterialListViewModel_EndTime", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public DateTime? EndTime { get; set; }
   
   
   


        public int PageSize { get; set; }

        public int PageNo { get; set; }

        public int TotalRecords { get; set; }

        public IEnumerable<SelectListItem> GetLocationNameList()
        {
            IList<Location> lst = new List<Location>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format("Level='{0}'", Convert.ToInt32(LocationLevel.Room))
            };
            using (LocationServiceClient client = new LocationServiceClient())
            {
                MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
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

        /// <summary>
        /// 取得线别代码
        /// </summary>
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
                       Text = item.LocationName,
                       Value = item.Key
                   };
        }

    }
    public class LotMaterialList1ViewModel : LotMaterialListViewModel
    {
        public LotMaterialList1ViewModel()
        {
            this.PageNo = 0;
            this.PageSize = 20;
            this.TotalRecords = 0;
        }
        public string LotNumber { get; set; }
        public string OrderNumber { get; set; }
        public string Activity { get; set; }
        public string RouteEnterpriseName { get; set; }

        public int RouteName { get; set; }

        public int RouteStepName { get; set; }

        public int Computer { get; set; }
        public int Creator{ get; set; }

        public int Editor { get; set; }
        public int PageSize { get; set; }

        public int PageNo { get; set; }

        public int TotalRecords { get; set; }
    }


    /// <summary>
    /// 批次物料出货信息界面Model
    /// </summary>
    public class LotMaterialListOutViewModel
    {
        public LotMaterialListOutViewModel()
        {
            this.PageNo = 0;
            this.PageSize = 20;
            this.OutEndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            this.OutStartTime = Convert.ToDateTime(DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss"));
        }

        //产品编码
        [Display(Name = "LotMaterialListViewModel_ProductMaterialCode", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string ProductMaterialCode { get; set; }

        //出库开始时间
        [Display(Name = "LotMaterialListViewModel_OutStartTime", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public DateTime? OutStartTime { get; set; }

        //出库结束时间
        [Display(Name = "LotMaterialListViewModel_OutEndTime", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public DateTime? OutEndTime { get; set; }

        //出库包装号
        [Display(Name = "LotMaterialListViewModel_OutPackageNo", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string OutPackageNo { get; set; }

        //物料编码
        [Display(Name = "LotMaterialListViewModel_BomMaterialCode", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string BomMaterialCode { get; set; }

        //物料名称
        [Display(Name = "LotMaterialListViewModel_BomMaterialName", ResourceType = typeof(ServiceCenter.Client.Mvc.Resources.RPT.StringResource))]
        public string BomMaterialName { get; set; }

        //每页记录数
        public int PageSize { get; set; }

        //页号
        public int PageNo { get; set; }

        //页数
        public int Pages { get; set; }

        //总记录数
        public int Records { get; set; }

    }

}