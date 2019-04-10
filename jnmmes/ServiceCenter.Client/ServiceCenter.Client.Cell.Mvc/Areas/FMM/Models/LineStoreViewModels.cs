
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
    public class LineStoreQueryViewModel
    {
        public LineStoreQueryViewModel()
        {
            this.Type = null;
        }

        [Display(Name = "LineStoreQueryViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public EnumLineStoreType? Type { get; set; }

        [Display(Name = "LineStoreQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

        [Display(Name = "LineStoreQueryViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }

        [Display(Name = "LineStoreQueryViewModel_RouteOperationName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteOperationName { get; set; }

        public IEnumerable<SelectListItem> GetLineStoreTypeList()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem() { Text = "", Value = null, Selected = true });

            IDictionary<EnumLineStoreType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumLineStoreType>();

            lst.AddRange(from item in dic
                        select new SelectListItem()
                        {
                            Text = item.Value,
                            Value = item.Key.ToString()
                        });
            return lst;
        }
    }

    public class LineStoreViewModel
    {
        public LineStoreViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;

        }

        [Required]
        [Display(Name = "LineStoreViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(20, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "LineStoreViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public EnumLineStoreType Type { get; set; }

        [Required]
        [Display(Name = "LineStoreViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public string LocationName { get; set; }

        [Display(Name = "LineStoreViewModel_RouteOperationName", ResourceType = typeof(FMMResources.StringResource))]
        public string RouteOperationName { get; set; }

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


        public IEnumerable<SelectListItem> GetLineStoreTypeList()
        {
            IDictionary<EnumLineStoreType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumLineStoreType>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

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

        public IEnumerable<SelectListItem> GetRouteOperationName()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem() { Text = "", Value = null, Selected = true });

            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available)),
                    OrderBy = "SortSeq"
                };

                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lst.AddRange(from item in result.Data
                                 select new SelectListItem()
                                 {
                                     Text = item.Key,
                                     Value = item.Key
                                 });
                }
            }
            return lst;
        }

    }
}