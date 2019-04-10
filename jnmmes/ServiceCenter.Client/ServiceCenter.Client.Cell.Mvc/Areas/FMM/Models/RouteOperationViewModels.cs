
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
    public class RouteOperationQueryViewModel
    {
        public RouteOperationQueryViewModel()
        {

        }

        [Display(Name = "RouteOperationQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }
    }

    public class RouteOperationViewModel
    {
        public RouteOperationViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.Status = EnumObjectStatus.Available;
            this.SortSeq = 1;
        }

        [Required]
        [Display(Name = "RouteOperationViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }
        /// <summary>
        /// 加工时长。
        /// </summary>
        [Display(Name = "RouteOperationViewModel_Duration", ResourceType = typeof(FMMResources.StringResource))]
        [Range(0, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        public double? Duration { get; set; }
        /// <summary>
        /// 排序序号。
        /// </summary>
        [Display(Name = "RouteOperationViewModel_SortSeq", ResourceType = typeof(FMMResources.StringResource))]
        [Range(1, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int SortSeq { get; set; }
        /// <summary>
        /// 报废原因代码分组名称。
        /// </summary>
        [Display(Name = "ScrapReasonCodeCategoryName", ResourceType = typeof(FMMResources.StringResource))]
        public  string ScrapReasonCodeCategoryName { get; set; }
        /// <summary>
        /// 不良原因代码分组名称。
        /// </summary>
        [Display(Name = "DefectReasonCodeCategoryName", ResourceType = typeof(FMMResources.StringResource))]
        public string DefectReasonCodeCategoryName { get; set; }

        [Display(Name = "Status", ResourceType = typeof(StringResource))]
        public EnumObjectStatus Status { get; set; }

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

        public IEnumerable<SelectListItem> GetObjectStatusList()
        {
            IDictionary<EnumObjectStatus, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumObjectStatus>();

            return from item in dic
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = Convert.ToString(item.Key)
                   }; ;
        }

        public IEnumerable<SelectListItem> GetReasonCodeCategoryName(EnumReasonCodeType type)
        {
            using (ReasonCodeCategoryServiceClient client = new ReasonCodeCategoryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Type='{0}'", Convert.ToInt32(type))
                };

                MethodReturnResult<IList<ReasonCodeCategory>> result = client.Get(ref cfg);
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

    }
}