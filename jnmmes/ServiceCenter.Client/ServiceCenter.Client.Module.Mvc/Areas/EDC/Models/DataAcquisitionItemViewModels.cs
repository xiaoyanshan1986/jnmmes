
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.EDC;
using EDCResources = ServiceCenter.Client.Mvc.Resources.EDC;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.EDC;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Models
{
    public class DataAcquisitionItemQueryViewModel
    {
        public DataAcquisitionItemQueryViewModel()
        {
        }

        /// <summary>
        /// 工序名称
        /// </summary>
        [Display(Name = "DataAcquisitionItemQueryViewModel_ItemCode", ResourceType = typeof(EDCResources.StringResource))]
        public string ItemCode { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        [Display(Name = "DataAcquisitionItemViewModel_RouteStepName", ResourceType = typeof(EDCResources.StringResource))]

        public string RouteStepName { get; set; }

        public IEnumerable<SelectListItem> GetRouteStepNameList()
        {
            //获取用户拥有权限的工序名称。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.RouteOperation);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }

            IList<RouteOperation> lstRouteOperation = new List<RouteOperation>();

            //获取工序名称。
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lstRouteOperation = result.Data;
                }
            }

            List<SelectListItem> lst = (from item in lstRouteOperation
                                        where lstResource.Any(m => m.Data == item.Key)
                                        orderby item.SortSeq
                                        select new SelectListItem()
                                        {
                                            Text = item.Key,
                                            Value = item.Key
                                        }).ToList();
            if (lst.Count > 0)
            {
                lst[0].Selected = true;
            }
            return lst;
        }
    }

    public class DataAcquisitionItemViewModel
    {
        public DataAcquisitionItemViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;

        }

        /// <summary>
        /// 采集项目代码
        /// </summary>
        [Required]
        [Display(Name = "DataAcquisitionItemQueryViewModel_ItemCode", ResourceType = typeof(EDCResources.StringResource))]
        [Editable(false)]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "DataAcquisitionItemViewModel_ValidateStringLength"
                        , ErrorMessageResourceType = typeof(EDCResources.StringResource))]
        public string ItemCode { get; set; }

        /// <summary>
        /// 采集项目名称
        /// </summary>
        [Required]
        [Display(Name = "DataAcquisitionItemViewModel_ItemName", ResourceType = typeof(EDCResources.StringResource))]
        [Editable(false)]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "DataAcquisitionItemViewModel_ValidateStringLength"
                        , ErrorMessageResourceType = typeof(EDCResources.StringResource))]
        public string ItemName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "DataAcquisitionItemViewModel_Description", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "DataAcquisitionItemViewModel_MaxValidateStringLength"
                     , ErrorMessageResourceType = typeof(EDCResources.StringResource))]
        public string Description { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        [Display(Name = "DataAcquisitionItemViewModel_RouteStepName", ResourceType = typeof(EDCResources.StringResource))]

        public string RouteStepName { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public string Creator { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public string Editor { get; set; }

        /// <summary>
        /// 编辑日期
        /// </summary>
        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public DateTime? EditTime { get; set; }


        public IEnumerable<SelectListItem> GetObjectStatusList()
        {
            IDictionary<EnumObjectStatus, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumObjectStatus>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetRouteStepNameList()
        {
            //获取用户拥有权限的工序名称。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.RouteOperation);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }

            IList<RouteOperation> lstRouteOperation = new List<RouteOperation>();

            //获取工序名称。
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Status='{0}'", Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lstRouteOperation = result.Data;
                }
            }

            List<SelectListItem> lst = (from item in lstRouteOperation
                                        where lstResource.Any(m => m.Data == item.Key)
                                        orderby item.SortSeq
                                        select new SelectListItem()
                                        {
                                            Text = item.Key,
                                            Value = item.Key
                                        }).ToList();
            if (lst.Count > 0)
            {
                lst[0].Selected = true;
            }
            return lst;
        }
    }
}