using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RPTResources = ServiceCenter.Client.Mvc.Resources.RPT;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Models
{
    public class RTPEquipmentViewModels
    {
         /// <summary>
        /// 构造函数。
        /// </summary>
        public RTPEquipmentViewModels()
        {
            this.curDate = DateTime.Now.ToString("yyyy-MM-dd");
            
        }
        /// <summary>
        /// 设备
        /// </summary>
        [Display(Name = "RTPEquipmentViewModel_EquipmentCode", ResourceType = typeof(RPTResources.StringResource))]
        public string EquipmentCode { get; set; }
        /// <summary>
        /// 日期。
        /// </summary>
        [Required]
        [Display(Name = "RTPEquipmentViewModel_curDate", ResourceType = typeof(RPTResources.StringResource))]
        public string curDate { get; set; }
    }
}