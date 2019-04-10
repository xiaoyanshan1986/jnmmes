
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
    public class RouteOperationEquipmentQueryViewModel
    {
        public RouteOperationEquipmentQueryViewModel()
        {

        }
        [Display(Name = "RouteOperationEquipmentQueryViewModel_RouteOperationName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteOperationName { get; set; }

        [Display(Name = "RouteOperationEquipmentQueryViewModel_EquipmentCode", ResourceType = typeof(FMMResources.StringResource))]
        public string EquipmentCode { get; set; }
    }

    public class RouteOperationEquipmentViewModel
    {
        public RouteOperationEquipmentViewModel()
        {
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "RouteOperationEquipmentViewModel_RouteOperationName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteOperationName { get; set; }

        [Required]
        [Display(Name = "RouteOperationEquipmentViewModel_EquipmentCode", ResourceType = typeof(FMMResources.StringResource))]
        public string EquipmentCode { get; set; }

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

        public Equipment GetEquipment(string key)
        {
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                MethodReturnResult<Equipment> result = client.Get(key);
                if (result.Code <= 0)
                {
                    return result.Data;
                }
            }
            return null;
        }
    }
}