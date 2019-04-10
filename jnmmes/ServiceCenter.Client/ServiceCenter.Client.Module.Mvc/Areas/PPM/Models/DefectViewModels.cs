
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
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Models
{
    public class DefectViewModel
    {
        public DefectViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        /// <summary>
        /// 年度
        /// </summary>
        [Required]
        [Display(Name = "DefectViewModel_Year", ResourceType = typeof(PPMResources.StringResource))]
        public virtual string Year { get; set; }

        /// <summary>
        /// 月份
        /// </summary>
        [Required]
        [Display(Name = "DefectViewModel_Month", ResourceType = typeof(PPMResources.StringResource))]
        public string Month { get; set; }

        [Required]
        [Display(Name = "DefectViewModel_Day", ResourceType = typeof(PPMResources.StringResource))]
        public string Day { get; set; }

        /// <summary>
        /// 车间代码
        /// </summary>
        [Required]
        [Display(Name = "DefectViewModel_LocationName", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string LocationName { get; set; }

        /// <summary>
        /// 班别
        /// </summary>
        [Required]
        [Display(Name = "DefectViewModel_ShiftName", ResourceType = typeof(PPMResources.StringResource))]
        public string ShiftName { get; set; }

        /// <summary>
        /// 不良组
        /// </summary>
        [Required]
        [Display(Name = "DefectViewModel_ReasonCodeCategoryName", ResourceType = typeof(PPMResources.StringResource))]
        public string ReasonCodeCategoryName { get; set; }

        /// <summary>
        /// 不良代码
        /// </summary>
        [Required]
        [Display(Name = "DefectViewModel_ReasonCodeName", ResourceType = typeof(PPMResources.StringResource))]
        public string ReasonCodeName { get; set; }

        /// <summary>
        /// 不良数量
        /// </summary>
        [Required]
        [Display(Name = "DefectViewModel_Qty", ResourceType = typeof(PPMResources.StringResource))]
        public double Qty { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }
        
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }
                
        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 取得车间代码
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 取得班别列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetShiftName()
        {
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = ""
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
        /// 取得产品代码
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetProductName()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName = 'RPTPerCapitaEfficiencyRatio' AND Key.AttributeName = 'ProcuctCode'"),
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

        /// <summary>
        /// 不良原因组
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetReasonCodeCategoryName()
        {
            IList<ReasonCodeCategory> lst = null;
            using (ReasonCodeCategoryServiceClient client = new ReasonCodeCategoryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = "Key.Type='0'"
                };
                MethodReturnResult<IList<ReasonCodeCategory>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }
            return from item in lst
                   select new SelectListItem
                   {
                       Text = item.Key,
                       Value = item.Key
                   };
        }

    }

    public class DefectQueryViewModel
    {
        public DefectQueryViewModel()
        {
            
        }

        /// <summary>
        /// 年度
        /// </summary>
        [Display(Name = "DefectViewModel_Year", ResourceType = typeof(PPMResources.StringResource))]
        public string qYear { get; set; }

        /// <summary>
        /// 月份
        /// </summary>
        [Display(Name = "DefectViewModel_Month", ResourceType = typeof(PPMResources.StringResource))]
        public string qMonth { get; set; }

        /// <summary>
        /// 日
        /// </summary>
        [Display(Name = "DefectViewModel_Day", ResourceType = typeof(PPMResources.StringResource))]
        public string qDay { get; set; }

        /// <summary>
        /// 车间代码
        /// </summary>
        [Display(Name = "DefectViewModel_LocationName", ResourceType = typeof(PPMResources.StringResource))]
        
        public string LocationName { get; set; }

        /// <summary>
        /// 班别
        /// </summary>
        [Display(Name = "DefectViewModel_ShiftName", ResourceType = typeof(PPMResources.StringResource))]
        public string ShiftName { get; set; }

        /// <summary>
        /// 取得车间代码
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 取得班别列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetShiftName()
        {
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = ""
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
        /// 取得产品代码
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetProductName()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName = 'RPTPerCapitaEfficiencyRatio' AND Key.AttributeName = 'ProcuctCode'"),
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
}