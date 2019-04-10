
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using RPTResources = ServiceCenter.Client.Mvc.Resources.RPT;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.RPT;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Models
{
    /// <summary>
    /// 在制品Move数据查询视图模型类。
    /// </summary>
    public class OEEDailyDataViewModel
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public OEEDailyDataViewModel()
        {
        }
        /// <summary>
        /// 车间名称
        /// </summary>
        [Display(Name = "OEEDailyDataViewModel_LocationName", ResourceType = typeof(RPTResources.StringResource))]

        public string LocationName { get; set; }
        /// <summary>
        /// 开始日期。
        /// </summary>
        [Required]
        [Display(Name = "OEEDailyDataViewModel_StartDate", ResourceType = typeof(RPTResources.StringResource))]
        public string StartDate { get; set; }
        /// <summary>
        /// 结束日期。
        /// </summary>
        [Required]
        [Display(Name = "OEEDailyDataViewModel_EndDate", ResourceType = typeof(RPTResources.StringResource))]
        public string EndDate { get; set; }
        /// <summary>
        /// 设备代码组名称。
        /// </summary>
        [Display(Name = "OEEDailyDataViewModel_EqipmentName", ResourceType = typeof(RPTResources.StringResource))]
        public string EquipmentName { get; set; }

        public IEnumerable<SelectListItem> GetLocations()
        {
            using (LocationServiceClient client = new LocationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Level='{0}'", Convert.ToInt32(LocationLevel.Room))
                };

                MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }

        public IEnumerable<SelectListItem> GetShiftNames()
        {
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<Shift>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }
        /// <summary>
        /// 取得设备组分类信息
        /// </summary>
        public IEnumerable<SelectListItem> GetEquipmentGroupList()
        {
            IList<Equipment> lst = new List<Equipment>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false
            };
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }
            var query = from item in lst
                        group item by new { t1 = item.GroupName }
                            into m
                            select new
                            {
                                Text = m.First().GroupName,
                                Value = m.First().Key,
                            } into r
                            orderby r.Text, r.Value
                            select r;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var data in query)
            {
                dic.Add(data.Text, data.Value);

            }
            return from item in dic
                   select new SelectListItem()
                   {
                       Value = item.Key,
                       Text = item.Key
                       //Text = item.Key
                   };



        }
    }
}