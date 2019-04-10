using ServiceCenter.MES.Model.FMM;
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
using RPTResources = ServiceCenter.Client.Mvc.Resources.RPT;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Models
{
    public class WIPIVTestDataViewModel
    {
         /// <summary>
        /// 构造函数。
        /// </summary>
        public WIPIVTestDataViewModel()
        {
            this.StartDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.EndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 批号
        /// </summary>
        [Display(Name = "WIPIVTestQueryViewModel_Lot_Number", ResourceType = typeof(RPTResources.StringResource))]
        public string Lot_Number { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        [Display(Name = "WIPIVTestQueryViewModel_EquipmentCode", ResourceType = typeof(RPTResources.StringResource))]
        public string EquipmentCode { get; set; }
        /// <summary>
        /// 分档规则
        /// </summary>
        [Display(Name = "WIPIVTestQueryViewModel_Attr_1", ResourceType = typeof(RPTResources.StringResource))]
        public string Attr_1 { get; set; }     
        /// <summary>
        /// 开始日期。
        /// </summary>
        [Required]
        [Display(Name = "WIPIVTestQueryViewModel_StartDate", ResourceType = typeof(RPTResources.StringResource))]
        public string StartDate { get; set; }
        /// <summary>
        /// 结束日期。
        /// </summary>
        [Required]
        [Display(Name = "WIPIVTestQueryViewModel_EndDate", ResourceType = typeof(RPTResources.StringResource))]
        public string EndDate { get; set; }

        /// <summary>
        /// 校准版编号
        /// </summary>
        [Display(Name = "WIPIVTestQueryViewModel_CalibrationPlateID", ResourceType = typeof(RPTResources.StringResource))]
        public string CalibrationPlateID { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        [Display(Name = "WIPIVTestQueryViewModel_LineCode", ResourceType = typeof(RPTResources.StringResource))]
        public string LineCode { get; set; }


        public IEnumerable<SelectListItem> GetBaseDataList(string type)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            MethodReturnResult<DataSet> rst = null;
            using (WIPIVTestServiceClient client = new WIPIVTestServiceClient())
            {
                rst = client.GetBaseDataForIVTest(type);
                if (rst.Code > 0 || rst.Data == null)
                {
                    return null;
                }
            }
            var query = from t in rst.Data.Tables[0].AsEnumerable()
                        group t by new { t1 = t.Field<string>("DATA") } into m
                        select new
                        {
                            EquipmentCode = m.First().Field<string>("DATA")
                        };
            return from item in query
                   select new SelectListItem()
                   {
                       Text = string.Format("{0}", item.EquipmentCode),
                       Value = item.EquipmentCode
                   };
        }

        /// <summary> 获取校准板列表 </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetCalibrationPlateIDList()
        {
            IList<CalibrationPlate> lst = new List<CalibrationPlate>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
            };
            using (CalibrationPlateServiceClient client = new CalibrationPlateServiceClient())
            {
                MethodReturnResult<IList<CalibrationPlate>> result = client.Get(ref cfg);
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

        /// <summary>获取线别列表 </summary>
        /// <returns></returns>
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