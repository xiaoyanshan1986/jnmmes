
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
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.RBAC;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class  EquipmentConsumingViewModel
    {
        public  EquipmentConsumingViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        /// <summary>
        /// 年度
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_Year", ResourceType = typeof(FMMResources.StringResource))]
        public virtual string Year { get; set; }

        /// <summary>
        /// 月份
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_Month", ResourceType = typeof(FMMResources.StringResource))]
        public string Month { get; set; }

        /// <summary>
        /// 日
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_Day", ResourceType = typeof(FMMResources.StringResource))]
        public string Day { get; set; }

        /// <summary>
        /// 车间代码
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string LocationName { get; set; }

        /// <summary>
        /// 班别
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_ShiftName", ResourceType = typeof(FMMResources.StringResource))]
        public string ShiftName { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_LineCode", ResourceType = typeof(FMMResources.StringResource))]
        public string LineCode { get; set; }

        /// <summary>
        /// 工序
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_RouteStepName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteStepName { get; set; }

        /// <summary>
        /// 设备代码
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_EquipmentCode", ResourceType = typeof(FMMResources.StringResource))]
        public string EquipmentCode { get; set; }

        /// <summary>
        /// 设备代码-设备名称
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_Value", ResourceType = typeof(FMMResources.StringResource))]
        public string Value { get; set; }

        /// <summary>
        /// 原因代码
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_ReasonCodeName", ResourceType = typeof(FMMResources.StringResource))]
        public string ReasonCodeName { get; set; }

        /// <summary>
        /// 耗时
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_Consuming", ResourceType = typeof(FMMResources.StringResource))]
        public int  Consuming { get; set; }

        /// <summary>
        /// 耗时
        /// </summary>
        [Required]
        [Display(Name = "EquipmentConsumingViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

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
        /// 获取工序列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRouteOperationList()
        {
            //获取用户拥有权限的工序。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.RouteOperation);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }

            IList<string> lstPackageOperation = new List<string>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = "Key.AttributeName='IsPackageOperation'"
            };
            using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
            {
                MethodReturnResult<IList<RouteOperationAttribute>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data != null)
                {
                    bool isPackageOperation = false;
                    lstPackageOperation = (from item in result.Data
                                           where bool.TryParse(item.Value, out isPackageOperation) == true
                                                 && isPackageOperation == true
                                           select item.Key.RouteOperationName).ToList();
                }
            }

            IList<RouteOperation> lst = new List<RouteOperation>();
            cfg.Where = "Status=1";
            cfg.OrderBy = "SortSeq";

            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }


            return from item in lst
                   where lstPackageOperation.Contains(item.Key.ToUpper()) == false
                         && lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = item.Key,
                       Value = item.Key
                   };
        }

        /// <summary>
        /// 获取生产线列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetProductionLineList()
        {
            //获取用户拥有权限的生产线。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.ProductionLine);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }
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
                   where lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = string.Format("{0}[{1}]", item.Name, item.Key),
                       Value = item.Key
                   };
        }

        /// <summary>
        /// 获取设备原因代码
        /// </summary>
        /// <returns></returns>

        public IEnumerable<SelectListItem> GetEquipmentReasonCodeName()
        {
            using (EquipmentReasonCodeServiceClient client = new EquipmentReasonCodeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = ""
                };

                MethodReturnResult<IList<EquipmentReasonCode>> result = client.Get(ref cfg);
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

        public IEnumerable<SelectListItem> GetEquipmentCode()
        {

            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = ""
                };

                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                        select new SelectListItem()
                                                        {
                                                            Text = item.Key + "-" + item.Name,
                                                            Value = item.Key  
                                                            
                                                        };
                    return lst;
                   
                }
            }
            return new List<SelectListItem>();
        }

        /// <summary> 获取设备信息 </summary>
        /// <param name="lotNumber">批次号</param>
        /// <returns></returns>
        public Equipment GetEquipment(string equipment)
        {
            Equipment Equipment = null;
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key='{0}'  ", equipment)
                };
                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data.Count > 0)
                {
                    Equipment = result.Data[0];
                }

                return Equipment;
            }

        }  
        
        
    }

    public class  EquipmentConsumingQueryViewModel
    {
        public  EquipmentConsumingQueryViewModel()
        {
            
        }

        /// <summary>
        /// 年度
        /// </summary>
        [Display(Name = "EquipmentConsumingQueryViewModel_Year", ResourceType = typeof(FMMResources.StringResource))]
        public string Year { get; set; }

        /// <summary>
        /// 月份
        /// </summary>
        [Display(Name = "EquipmentConsumingQueryViewModel_Month", ResourceType = typeof(FMMResources.StringResource))]
        public string Month { get; set; }

        /// <summary>
        /// 日
        /// </summary>
        [Display(Name = "EquipmentConsumingQueryViewModel_Day", ResourceType = typeof(FMMResources.StringResource))]
        public string Day { get; set; }

        /// <summary>
        /// 车间代码
        /// </summary>
        [Display(Name = "EquipmentConsumingQueryViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        
        public string LocationName { get; set; }

        /// <summary>
        /// 班别
        /// </summary>
        [Display(Name = "EquipmentConsumingQueryViewModel_ShiftName", ResourceType = typeof(FMMResources.StringResource))]
        public string ShiftName { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        [Display(Name = "EquipmentConsumingQueryViewModel_LineCode", ResourceType = typeof(FMMResources.StringResource))]
        public string LineCode { get; set; }

        /// <summary>
        /// 工序
        /// </summary>
        [Display(Name = "EquipmentConsumingQueryViewModel_RouteStepName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteStepName { get; set; }

        /// <summary>
        /// 设备代码
        /// </summary>
        [Display(Name = "EquipmentConsumingQueryViewModel_EquipmentCode", ResourceType = typeof(FMMResources.StringResource))]
        public string EquipmentCode { get; set; }

        /// <summary>
        /// 原因代码
        /// </summary>
        [Display(Name = "EquipmentConsumingQueryViewModel_ReasonCodeName", ResourceType = typeof(FMMResources.StringResource))]
        public string ReasonCodeName { get; set; }

        /// <summary>
        /// 原因代码
        /// </summary>
        [Display(Name = "EquipmentConsumingQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

        /// <summary>
        /// 设备原因代码-名称
        /// </summary>
        [Display(Name = "EquipmentConsumingQueryViewModel_Value", ResourceType = typeof(FMMResources.StringResource))]
        public string Value { get; set; }


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
        /// 获取工序列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRouteOperationList()
        {
            //获取用户拥有权限的工序。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.RouteOperation);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }

            IList<string> lstPackageOperation = new List<string>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = "Key.AttributeName='IsPackageOperation'"
            };
            using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
            {
                MethodReturnResult<IList<RouteOperationAttribute>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data != null)
                {
                    bool isPackageOperation = false;
                    lstPackageOperation = (from item in result.Data
                                           where bool.TryParse(item.Value, out isPackageOperation) == true
                                                 && isPackageOperation == true
                                           select item.Key.RouteOperationName).ToList();
                }
            }

            IList<RouteOperation> lst = new List<RouteOperation>();
            cfg.Where = "Status=1";
            cfg.OrderBy = "SortSeq";

            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }


            return from item in lst
                   where lstPackageOperation.Contains(item.Key.ToUpper()) == false
                         && lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = item.Key,
                       Value = item.Key
                   };
        }

        /// <summary>
        /// 获取生产线列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetProductionLineList()
        {
            //获取用户拥有权限的生产线。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.ProductionLine);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }
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
                   where lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = string.Format("{0}[{1}]", item.Name, item.Key),
                       Value = item.Key
                   };
        }

        /// <summary>
        /// 获取设备原因代码
        /// </summary>
        /// <returns></returns>

        public IEnumerable<SelectListItem> GetEquipmentReasonCodeName()
        {
            using (EquipmentReasonCodeServiceClient client = new EquipmentReasonCodeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = ""
                };

                MethodReturnResult<IList<EquipmentReasonCode>> result = client.Get(ref cfg);
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


        public IEnumerable<SelectListItem> GetDay(int year, int month)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            string sDate = year.ToString() + month.ToString("00") + "01";
            DateTime dtData = DateTime.ParseExact(sDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture); ;
            
            int days = DateTime.DaysInMonth(dtData.Year, dtData.Month);

            for (int i = 1; i <= days; i++)
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


        public IEnumerable<SelectListItem> GetEquipmentCode()
        {

            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = ""
                };

                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key + "-" + item.Name,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }


        /// <summary> 获取设备信息 </summary>
        /// <param name="lotNumber">批次号</param>
        /// <returns></returns>
        public Equipment GetEquipment(string equipment)
        {
            Equipment Equipment = null;
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key='{0}'  ", equipment)
                };
                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data.Count > 0)
                {
                    Equipment = result.Data[0];
                }

                return Equipment;
            }

        }  
        
    }
}