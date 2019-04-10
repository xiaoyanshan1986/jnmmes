
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.FMM;
using FMMResources = ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.FMM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class CalibrationPlateLineQueryViewModel
    {
        public CalibrationPlateLineQueryViewModel()
        {

        }
        [Display(Name = "CalibrationPlateViewModel_CalibrationPlateID", ResourceType = typeof(FMMResources.StringResource))]
        public string CalibrationPlateID { get; set; }

        [Display(Name = "CalibrationPlateQueryViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }

        [Display(Name = "CalibrationPlateQueryViewModel_LineCode", ResourceType = typeof(FMMResources.StringResource))]
        public string LineCode { get; set; }
        public IEnumerable<SelectListItem> GetLineCodeList()
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
                       Text = item.Key,
                       Value = item.Key
                   };
        }
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

    }

    public class CalibrationPlateLineViewModel
    {
        public CalibrationPlateLineViewModel()
        {
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "CalibrationPlateLineViewModel_CalibrationPlateID", ResourceType = typeof(FMMResources.StringResource))]
        public string CalibrationPlateID { get; set; }

        //[Required]
        [Display(Name = "CalibrationPlateViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateLineViewModel_LineCode", ResourceType = typeof(FMMResources.StringResource))]
        public string LineCode { get; set; }

        [Display(Name = "CalibrationPlateLineViewModel_Explain", ResourceType = typeof(FMMResources.StringResource))]
        public string Explain { get; set; }

        [Display(Name = "Creator", ResourceType = typeof(FMMResources.StringResource))]
        public string Creator { get; set; }

        [Display(Name = "CreateTime", ResourceType = typeof(FMMResources.StringResource))]
        public DateTime? CreateTime { get; set; }
        [Required]
        [Display(Name = "Editor", ResourceType = typeof(FMMResources.StringResource))]
        public string Editor { get; set; }
        [Display(Name = "EditTime", ResourceType = typeof(FMMResources.StringResource))]
        public DateTime? EditTime { get; set; }


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

        public IEnumerable<SelectListItem> GetLineCodeList()
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
                       Text = item.Key,
                       Value = item.Key
                   };
        }


    }
}