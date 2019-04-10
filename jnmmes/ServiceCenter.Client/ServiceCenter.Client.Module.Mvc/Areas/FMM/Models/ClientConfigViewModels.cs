
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

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class ClientConfigQueryViewModel
    {
        public ClientConfigQueryViewModel()
        {

        }

        [Display(Name = "ClientConfigQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

        [Display(Name = "ClientConfigViewModel_ClientType", ResourceType = typeof(FMMResources.StringResource))]
        public EnumClientType? ClientType { get; set; }

        [Display(Name = "ClientConfigQueryViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }

        [Display(Name = "ClientConfigQueryViewModel_IPAddress", ResourceType = typeof(FMMResources.StringResource))]
        public string IPAddress { get; set; }

        public IEnumerable<SelectListItem> GetClientTypeList()
        {
            IDictionary<EnumClientType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumClientType>();

            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Key.ToString()
                   };
        }
    }

    public class ClientConfigViewModel
    {
        public ClientConfigViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;

        }

        [Required]
        [Display(Name = "ClientConfigViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "ClientConfigViewModel_ClientType", ResourceType = typeof(FMMResources.StringResource))]
        public EnumClientType ClientType { get; set; }

        [Required]
        [Display(Name = "ClientConfigViewModel_IPAddress", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(255, MinimumLength = 1, ErrorMessage = null
                         , ErrorMessageResourceName = "ValidateMaxStringLength"
                         , ErrorMessageResourceType = typeof(StringResource))]
        public string IPAddress { get; set; }

        [Required]
        [Display(Name = "ClientConfigViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        public IEnumerable<SelectListItem> GetLocation()
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

        public IEnumerable<SelectListItem> GetClientTypeList()
        {
            IDictionary<EnumClientType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumClientType>();

            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Key.ToString()
                   };
        }
    }
}