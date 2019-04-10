using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{
    public class ContrastColorViewModel
    {
        public ContrastColorViewModel()
        {
            this.StartTestTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.EndTestTime = DateTime.Now.AddDays(1);
        }
        [Required]
        [Display(Name = "测试时间（起）")]
        public DateTime? StartTestTime { get; set; }
        [Required]
        [Display(Name = "测试时间（止）")]
        public DateTime? EndTestTime { get; set; }

    }
}