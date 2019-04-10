
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
    public class EquipmentLayoutDetailQueryViewModel
    {
        public EquipmentLayoutDetailQueryViewModel()
        {

        }
        [Display(Name = "EquipmentLayoutDetailQueryViewModel_LayoutName", ResourceType = typeof(FMMResources.StringResource))]
        public string LayoutName { get; set; }

        [Display(Name = "EquipmentLayoutDetailQueryViewModel_EquipmentCode", ResourceType = typeof(FMMResources.StringResource))]
        public string EquipmentCode { get; set; }  

        public IEnumerable<SelectListItem> GetLayoutList()
        {
            using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<EquipmentLayout>> result = client.Get(ref cfg);
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

        /// <summary> 获取设备原因代码组名称 </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetEquipmentReasonCodeCategoryName()
        {
            IList<EquipmentReasonCodeCategory> lst = null;
            using (EquipmentReasonCodeCategoryServiceClient client = new EquipmentReasonCodeCategoryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = "Key.Type='0'"
                };
                MethodReturnResult<IList<EquipmentReasonCodeCategory>> result = client.Get(ref cfg);
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

    public class EquipmentLayoutDetailViewModel
    {
        public EquipmentLayoutDetailViewModel()
        {
            this.EditTime = DateTime.Now;
            this.Left = 10;
            this.Top = 10;
            this.Width = 200;
            this.Height = 40;
        }

        [Required]
        [Display(Name = "EquipmentLayoutDetailViewModel_LayoutName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string LayoutName { get; set; }

        [Required]
        [Display(Name = "EquipmentLayoutDetailViewModel_EquipmentCode", ResourceType = typeof(FMMResources.StringResource))]
        public string EquipmentCode { get; set; }

        /// <summary>
        /// 位置Left。
        /// </summary>
        [Required]
        [Display(Name = "EquipmentLayoutDetailViewModel_Left", ResourceType = typeof(FMMResources.StringResource))]
        public  int Left { get; set; }
        /// <summary>
        /// 位置Top。
        /// </summary>
        [Required]
        [Display(Name = "EquipmentLayoutDetailViewModel_Top", ResourceType = typeof(FMMResources.StringResource))]
        public  int Top { get; set; }
        /// <summary>
        /// 宽度。
        /// </summary>
        [Required]
        [Display(Name = "EquipmentLayoutDetailViewModel_Width", ResourceType = typeof(FMMResources.StringResource))]
        public  int Width { get; set; }
        /// <summary>
        /// 高度。
        /// </summary>
        [Required]
        [Display(Name = "EquipmentLayoutDetailViewModel_Height", ResourceType = typeof(FMMResources.StringResource))]
        public  int Height { get; set; }
        [Required]
        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        public IEnumerable<SelectListItem> GetEquipmentCodeList()
        {
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = string.Format("{0}-{1}",item.Key,item.Name),
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }

    }

    public class EquipmentLayoutDetailEqpInfoViewModel
    {
        [Display(Name = "设备编号")]
        public string EquipmentCode { get; set; }

        [Display(Name = "设备名称")]
        public string EquipmentName { get; set; }


        [Display(Name = "设备状态")]
        public string EquipmentStateName { get; set; }

        [Display(Name = "切换人")]
        public string Creator { get; set; }

        [Display(Name = "切换原因")]
        public string Description { get; set; }     

        [Display(Name = "设备切换时间")]
        public string CREATE_TIME { get; set; }

        [Display(Name = "设备等待时间")]
        public string EquipmentStateMinutes { get; set; }

        [Display(Name = "设备原因代码组名称")]
        public string ReasonCodeCategoryName { get; set; }

        [Display(Name = "设备原因代码名称")]
        public string ReasonCodeName { get; set; }


        [Display(Name = "责任人")]
        public string ResponsiblePerson { get; set; }


    }
}