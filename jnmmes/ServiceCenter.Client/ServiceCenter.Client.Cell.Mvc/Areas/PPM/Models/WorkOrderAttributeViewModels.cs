
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

namespace ServiceCenter.Client.Mvc.Areas.PPM.Models
{
    public class WorkOrderAttributeQueryViewModel
    {
        public WorkOrderAttributeQueryViewModel()
        {

        }
        [Display(Name = "WorkOrderAttributeQueryViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "WorkOrderAttributeQueryViewModel_AttributeName", ResourceType = typeof(PPMResources.StringResource))]
        public string AttributeName { get; set; }
    }

    public class WorkOrderAttributeViewModel
    {
        public WorkOrderAttributeViewModel()
        {
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "WorkOrderAttributeViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "WorkOrderAttributeViewModel_AttributeName", ResourceType = typeof(PPMResources.StringResource))]
        public string AttributeName { get; set; }

        [Required]
        [Display(Name = "WorkOrderAttributeViewModel_Value", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Value { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        public IEnumerable<SelectListItem> GetAttributeName()
        {
            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='{0}'", "WorkOrderAttribute")
                };

                MethodReturnResult<IList<BaseAttribute>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = string.Format("{0}({1})",item.Key.AttributeName,item.Description),
                                                          Value = item.Key.AttributeName
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }
    }
}