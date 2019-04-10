using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.WIP;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Models
{

    public class VIRTestDataQueryViewModel
    {
        public VIRTestDataQueryViewModel()
        {
            this.PageNo = 0;
            this.PageSize = 20;
            this.TotalRecords = 0;
            this.StartTestTime = DateTime.Now.ToString("yyyy-MM-dd");
            this.EndTestTime = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");
        }

        [Display(Name = "VIRTestDataQueryViewModel_LotNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        public string LotNumber { get; set; }

        [Display(Name = "VIRTestDataQueryViewModel_StartTestTime", ResourceType = typeof(ZPVMResources.StringResource))]
        public string StartTestTime { get; set; }

        [Display(Name = "VIRTestDataQueryViewModel_EndTestTime", ResourceType = typeof(ZPVMResources.StringResource))]
        public string EndTestTime { get; set; }

        [Display(Name = "VIRTestDataQueryViewModel_EquipmentCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string EquipmentCode { get; set; }

        [Display(Name = "VIRTestDataQueryViewModel_TestResult", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool? TestResult { get; set; }
        public int PageSize { get; set; }

        public int PageNo { get; set; }

        public int TotalRecords { get; set; }

        public IEnumerable<SelectListItem> GetBoolList()
        {
            Dictionary<bool, string> dic = new Dictionary<bool, string>();
            dic.Add(true, StringResource.Yes);
            dic.Add(false, StringResource.No);

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

      

 
    }

    public class VIRTestDataViewModel
    {
        public VIRTestDataViewModel()
        {
            this.PageNo = 0;
            this.PageSize = 20;
            this.TotalRecords = 0;
        }
        [Required]
        [Display(Name = " VIRTestDataViewModel_EquipmentCode", ResourceType = typeof(ZPVMResources.StringResource))]
        public string EquipmentCode { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_TestTime", ResourceType = typeof(ZPVMResources.StringResource))]
        public DateTime? TestTime { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_LotNumber", ResourceType = typeof(ZPVMResources.StringResource))]
        public string  LotNumber { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_TestResult", ResourceType = typeof(ZPVMResources.StringResource))]
        public string  TestResult { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_TestFlag", ResourceType = typeof(ZPVMResources.StringResource))]
        public bool TestFlag { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_TestStepSeq", ResourceType = typeof(ZPVMResources.StringResource))]
        public int TestStepSeq { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_TestStepResult", ResourceType = typeof(ZPVMResources.StringResource))]
        public string TestStepResult { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_TestParam1", ResourceType = typeof(ZPVMResources.StringResource))]
        public string  TestParam1 { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_TestParam2", ResourceType = typeof(ZPVMResources.StringResource))]
        public string  TestParam2 { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Voltage", ResourceType = typeof(ZPVMResources.StringResource))]
        public string  Voltage { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Frequency", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Frequency { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Ecurren", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Ecurren { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Hilimit", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Hilimit { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Lolimit", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Lolimit { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Rampup", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Rampup { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Dwelltime", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Dwelltime { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Delaytimet", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Delaytime { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Ramphit", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Ramphit { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Chargelo", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Chargelo { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Offset", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Offset { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Arcsense", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Arcsense { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Arcfail", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Arcfail { get; set; }

        [Required]
        [Display(Name = " VIRTestDataViewModel_Scanner", ResourceType = typeof(ZPVMResources.StringResource))]
        public string Scanner { get; set; }

        public int PageSize { get; set; }

        public int PageNo { get; set; }

        public int TotalRecords { get; set; }
    }

}