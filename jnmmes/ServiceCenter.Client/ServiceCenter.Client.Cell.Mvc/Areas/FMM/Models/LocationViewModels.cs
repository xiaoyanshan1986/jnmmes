
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
    public class LocationQueryViewModel
    {
        public LocationQueryViewModel()
        {
            this.Level = null;
        }

        [Display(Name = "LocationQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

        [Display(Name = "LocationQueryViewModel_Level", ResourceType = typeof(FMMResources.StringResource))]
        public LocationLevel? Level { get; set; }

        [Display(Name = "LocationViewModel_ParentLocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string ParentLocationName { get; set; }

        public IEnumerable<SelectListItem> GetLocationLevelList()
        {
            IList<string> nullValues = new List<string>();
            nullValues.Add("");

            IDictionary<LocationLevel, string> dic = EnumExtensions.GetDisplayNameDictionary<LocationLevel>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return (from item in nullValues
                    select new SelectListItem()
                    {
                        Text = item,
                        Value = null,
                        Selected = true
                    }).Union(lst);
        }
    }

    public class LocationViewModel
    {
        public LocationViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            
        }

        [Required]
        [Display(Name = "LocationViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(20, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }


        [Required]
        [Display(Name = "LocationViewModel_Level", ResourceType = typeof(FMMResources.StringResource))]
        public LocationLevel Level { get; set; }




        [Display(Name = "LocationViewModel_ParentLocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string ParentLocationName { get; set; }

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

        public IEnumerable<SelectListItem> GetLocations(LocationLevel level)
        {
            using(LocationServiceClient client=new LocationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging=false,
                    Where = string.Format("Level='{0}'",Convert.ToInt32(level))
                };

                MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
                if(result.Code<=0)
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

        public IEnumerable<SelectListItem> GetLocationLevelList()
        {
            IDictionary<LocationLevel, string> dic = EnumExtensions.GetDisplayNameDictionary<LocationLevel>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }
    }
}