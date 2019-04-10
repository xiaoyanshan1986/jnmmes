
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
    public class ScheduleDayViewModel
    {
        public ScheduleDayViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "ScheduleDayViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }

        [Required]
        [Display(Name = "ScheduleDayViewModel_RouteOperationName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteOperationName { get; set; }

        [Required]
        [Display(Name = "ScheduleDayViewModel_Day", ResourceType = typeof(FMMResources.StringResource))]
        public DateTime Day { get; set; }
        [Required]
        [Display(Name = "ScheduleDayViewModel_ShiftName", ResourceType = typeof(FMMResources.StringResource))]
        public string ShiftName { get; set; }

        [Required]
        [Display(Name = "ScheduleDayViewModel_ShiftValue", ResourceType = typeof(FMMResources.StringResource))]
        public string ShiftValue { get; set; }

        [Required]
        [Display(Name = "ScheduleDayViewModel_SeqNo", ResourceType = typeof(FMMResources.StringResource))]
        public int SeqNo { get; set; }
        [Required]
        [Display(Name = "ScheduleDayViewModel_StartTime", ResourceType = typeof(FMMResources.StringResource))]
        public string StartTime { get; set; }
        [Required]
        [Display(Name = "ScheduleDayViewModel_EndTime", ResourceType = typeof(FMMResources.StringResource))]
        public string EndTime { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        public IList<SelectListItem> GetShiftValue()
        {
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.CategoryName='{0}'
                                           AND Key.AttributeName='{1}'"
                                            , "ShiftValue"
                                            , "Value"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Value,
                                                          Value = item.Value
                                                      };

                    return lst.ToList();
                }
            }
            return new List<SelectListItem>();
        }

        public Shift GetShift(string shiftName)
        {
            Shift obj=HttpContext.Current.Cache.Get(shiftName) as Shift;
            if(obj!=null)
            {
                return obj;
            }

            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                MethodReturnResult<Shift> result = client.Get(shiftName);
                if (result.Code <= 0)
                {
                    HttpContext.Current.Cache.Add(shiftName
                                                  , result.Data
                                                  , null
                                                  , DateTime.Now.AddSeconds(5)
                                                  , System.Web.Caching.Cache.NoSlidingExpiration
                                                  , System.Web.Caching.CacheItemPriority.Normal
                                                  , null);
                    return result.Data;
                }
            }
            return null;
        }

    }
}