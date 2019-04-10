
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
    public class CalibrationPlateQueryViewModel
    {
        public CalibrationPlateQueryViewModel()
        {

        }
        [Display(Name = "CalibrationPlateQueryViewModel_LineCode", ResourceType = typeof(FMMResources.StringResource))]
        public string LineCode { get; set; }

        [Display(Name = "CalibrationPlateQueryViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }
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
        public IEnumerable<SelectListItem> GetTypeList()
        {

             List<SelectListItem> selectList = new List<SelectListItem>();


            foreach (object e in Enum.GetValues(typeof(EnumPlateType)))

            {
                selectList.Add(new SelectListItem { Text = e.ToString(), Value = ((int)e).ToString() });
            }
            return selectList;
        }
    }

    public class CalibrationPlateViewModel
    {
        public CalibrationPlateViewModel()
        {
            this.EditTime = DateTime.Now;
        }
        [Required]
        [Display(Name = "CalibrationPlateViewModel_CalibrationPlateID", ResourceType = typeof(FMMResources.StringResource))]
        public string CalibrationPlateID { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_CalibrationPlateName", ResourceType = typeof(FMMResources.StringResource))]
        public string CalibrationPlateName { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_CalibrationPlateType", ResourceType = typeof(FMMResources.StringResource))]
        public int CalibrationPlateType { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_PM", ResourceType = typeof(FMMResources.StringResource))]
        public double PM { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_ISC", ResourceType = typeof(FMMResources.StringResource))]
        public double ISC { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_VOC", ResourceType = typeof(FMMResources.StringResource))]
        public double VOC { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_MaxPM", ResourceType = typeof(FMMResources.StringResource))]
        public double MaxPM { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_MinPM", ResourceType = typeof(FMMResources.StringResource))]
        public double MinPM { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_MaxISC", ResourceType = typeof(FMMResources.StringResource))]
        public double MaxISC { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_MinISC", ResourceType = typeof(FMMResources.StringResource))]
        public double MinISC { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_MaxVOC", ResourceType = typeof(FMMResources.StringResource))]
        public double MaxVOC { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_MinVOC", ResourceType = typeof(FMMResources.StringResource))]
        public double MinVOC { get; set; }

        [Display(Name = "CalibrationPlateViewModel_Explain", ResourceType = typeof(FMMResources.StringResource))]
        public string Explain { get; set; }

        [Display(Name = "CalibrationPlateViewModel_Creator", ResourceType = typeof(FMMResources.StringResource))]
        public string Creator { get; set; }

        [Display(Name = "CalibrationPlateViewModel_CreateTime", ResourceType = typeof(FMMResources.StringResource))]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "CalibrationPlateViewModel_Editor", ResourceType = typeof(FMMResources.StringResource))]
        public string Editor { get; set; }

        [Display(Name = "CalibrationPlateViewModel_EditTime", ResourceType = typeof(FMMResources.StringResource))]
        public DateTime? EditTime { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_StdIsc1", ResourceType = typeof(FMMResources.StringResource))]
        public double StdIsc1 { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_StdIsc2", ResourceType = typeof(FMMResources.StringResource))]
        public double StdIsc2 { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_Stdsun1", ResourceType = typeof(FMMResources.StringResource))]
        public double Stdsun1 { get; set; }

        [Required]
        [Display(Name = "CalibrationPlateViewModel_Stdsun2", ResourceType = typeof(FMMResources.StringResource))]
        public double Stdsun2 { get; set; }


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

        public IEnumerable<SelectListItem> GetTypeList()
        {

            List<SelectListItem> selectList = new List<SelectListItem>();


            foreach (EnumPlateType e in Enum.GetValues(typeof(EnumPlateType)))
            {
                selectList.Add(new SelectListItem { Text = e.GetDisplayName().ToString(), Value = ((int)e).ToString() });
            }
            return selectList;
        }
    }
}