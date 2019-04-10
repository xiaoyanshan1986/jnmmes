
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
    public class ScheduleDetailQueryViewModel
    {
        public ScheduleDetailQueryViewModel()
        {

        }
        [Display(Name = "ScheduleDetailQueryViewModel_ScheduleName", ResourceType = typeof(FMMResources.StringResource))]
        public string ScheduleName { get; set; }

        [Display(Name = "ScheduleDetailQueryViewModel_ShiftName", ResourceType = typeof(FMMResources.StringResource))]
        public string ShiftName { get; set; }
    }

    public class ScheduleDetailViewModel
    {
        public ScheduleDetailViewModel()
        {
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "ScheduleDetailViewModel_ScheduleName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ScheduleName { get; set; }

        [Required]
        [Display(Name = "ScheduleDetailViewModel_ShiftName", ResourceType = typeof(FMMResources.StringResource))]
        public string ShiftName { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        public IEnumerable<SelectListItem> GetShiftName()
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
    }
}