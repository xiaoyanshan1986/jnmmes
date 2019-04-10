
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
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Models
{
    public class PlanMonthQueryViewModel
    {
        public PlanMonthQueryViewModel()
        {

        }
        [Display(Name = "PlanMonthQueryViewModel_Year", ResourceType = typeof(PPMResources.StringResource))]
        public string Year { get; set; }

        [Display(Name = "PlanMonthQueryViewModel_Month", ResourceType = typeof(PPMResources.StringResource))]
        public string Month { get; set; }

        [Display(Name = "PlanMonthQueryViewModel_LocationName", ResourceType = typeof(PPMResources.StringResource))]
        public string LocationName { get; set; }
        
        public IEnumerable<SelectListItem> GetLocationName()
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

        public IEnumerable<SelectListItem> GetYear()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            int startYear = DateTime.Now.Year;
            for (int i = 0; i < 5; i++)
            {
                string value = Convert.ToString(startYear + i);
                lst.Add(new SelectListItem()
                {
                    Text = value,
                    Value = value
                });
            }
            return lst;
        }

        public IEnumerable<SelectListItem> GetMonth()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                string value = i.ToString("00");
                lst.Add(new SelectListItem()
                {
                    Text = value,
                    Value = value
                });
            }
            return lst;
        }
    }

    public class PlanMonthViewModel
    {
        public PlanMonthViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        /// <summary>
        /// 年
        /// </summary>
        [Required]
        [Display(Name = "PlanMonthViewModel_Year", ResourceType = typeof(PPMResources.StringResource))]
        public string Year { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        [Required]
        [Display(Name = "PlanMonthViewModel_Month", ResourceType = typeof(PPMResources.StringResource))]
        public string Month { get; set; }

        /// <summary>
        /// 车间
        /// </summary>
        [Required]
        [Display(Name = "PlanMonthViewModel_LocationName", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string LocationName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        //[Required]
        [Display(Name = "PlanMonthViewModel_PlanQty", ResourceType = typeof(PPMResources.StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string PlanQty { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }
        
        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }
        
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }
        
        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        public IEnumerable<SelectListItem> GetLocationName()
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

        public IEnumerable<SelectListItem> GetRouteStepName()
        {
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
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

        public IEnumerable<SelectListItem> GetScheduleName()
        {
            using (ScheduleServiceClient client = new ScheduleServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<Schedule>> result = client.Get(ref cfg);
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

        public IEnumerable<SelectListItem> GetYear()
        {
            List<SelectListItem> lst= new List<SelectListItem>();
            int startYear = DateTime.Now.Year;
            for(int i=0;i<5;i++)
            {
                string value = Convert.ToString(startYear + i);
                lst.Add(new SelectListItem()
                        {
                            Text = value,
                            Value = value
                        });
            }
            return lst;
        }

        public IEnumerable<SelectListItem> GetMonth()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                string value = i.ToString("00");
                lst.Add(new SelectListItem()
                {
                    Text = value,
                    Value = value
                });
            }
            return lst;
        }
    }
}