
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.WIP;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.RBAC;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class LotDefectQueryViewModel
    {
        public LotDefectQueryViewModel()
        {
            this.StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.EndTime = DateTime.Now.AddDays(1);
        }

        [Display(Name = "LotPatchViewModel_OrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }
        
        [Display(Name = "ReasonCodeName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string ReasonCodeName { get; set; }

        [Display(Name = "LotDefectViewModel_RouteStepName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteStepName { get; set; }

        [Display(Name = "LotDefectViewModel_RouteOperationName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteOperationName { get; set; }

        [Display(Name = "LotDefectViewModel_ResponsiblePerson", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string ResponsiblePerson { get; set; }

        [Display(Name = "LotDefectViewModel_StartTime", ResourceType = typeof(WIPResources.StringResource))]
        public DateTime? StartTime { get; set; }

        [Display(Name = "LotDefectViewModel_EndTime", ResourceType = typeof(WIPResources.StringResource))]
        public DateTime? EndTime { get; set; }

        public IEnumerable<SelectListItem> GetRouteOperationNameList()
        {
            IList<RouteOperation> lst = new List<RouteOperation>();
            //获取工序信息。
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available))
                };
                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lst = result.Data;
                }
            }
            return from item in lst
                   orderby item.SortSeq
                   select new SelectListItem
                   {
                       Text = item.Key,
                       Value = item.Key
                   };
        }
    }

    public class LotDefectViewModel
    {
        public LotDefectViewModel()
        {
            this.DefectQuantity = 1;
        }
        [Required]
        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string LotNumber { get; set; }

        [Required]
        [Display(Name = "ReasonCodeCategoryName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string ReasonCodeCategoryName { get; set; }

        [Required]
        [Display(Name = "ReasonCodeName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string ReasonCodeName { get; set; }

        [Display(Name = "ReasonDescription", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string ReasonDescription { get; set; }

        [Display(Name = "LotDefectViewModel_RouteOperationName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteOperationName { get; set; }

        [Display(Name = "LotDefectViewModel_ResponsiblePerson", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string ResponsiblePerson { get; set; }

        [Required]
        [Range(1, 65536
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        [Display(Name = "LotDefectViewModel_DefectQuantity", ResourceType = typeof(WIPResources.StringResource))]
        public double DefectQuantity { get; set; }


        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> GetRouteOperationNameList()
        {
            IList<RouteOperation> lst = new List<RouteOperation>();
            //获取工序信息。
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available))
                };
                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lst = result.Data;
                }
            }
            return from item in lst
                   orderby item.SortSeq
                   select new SelectListItem
                   {
                       Text = item.Key,
                       Value = item.Key
                   };
        }

    }
}