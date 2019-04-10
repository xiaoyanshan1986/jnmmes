﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EDCResources = ServiceCenter.Client.Mvc.Resources.EDC;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Models
{
    public class DataAcquisitionTransQueryViewModel
    {
        public DataAcquisitionTransQueryViewModel()
        {
            PageNo = 0;
            PageSize = 20;
        }

        /// <summary>
        /// 采集时间
        /// </summary>
        [Display(Name = "DataAcquisitionDataQueryViewModel_EDCTime", ResourceType = typeof(EDCResources.StringResource))]
        public DateTime EDCTime { get; set; }

        /// <summary>
        /// 采集项目代码
        /// </summary>
        [Display(Name = "DataAcquisitionDataQueryViewModel_ItemCode", ResourceType = typeof(EDCResources.StringResource))]
        public string ItemCode { get; set; }

        /// <summary>
        /// 车间
        /// </summary>
        [Display(Name = "DataAcquisitionDataQueryViewModel_LocationName", ResourceType = typeof(EDCResources.StringResource))]
        public string LocationName { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        [Display(Name = "DataAcquisitionDataQueryViewModel_LineCode", ResourceType = typeof(EDCResources.StringResource))]
        public string LineCode { get; set; }

        /// <summary>
        /// 工序
        /// </summary>
        [Display(Name = "DataAcquisitionDataQueryViewModel_RouteOperationName", ResourceType = typeof(EDCResources.StringResource))]
        public string RouteOperationName { get; set; }

        /// <summary>
        /// 设备代码
        /// </summary>
        [Display(Name = "DataAcquisitionDataQueryViewModel_EquipmentCode", ResourceType = typeof(EDCResources.StringResource))]
        public string EquipmentCode { get; set; }

        /// <summary>
        /// 数据单页面大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int Records { get; set; }

        /// <summary>
        /// 取得线别代码
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetLineList()
        {
            //获取用户拥有权限的生产线。
            IList<Resource> lstResource = new List<Resource>();

            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.ProductionLine);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }

            IList<ProductionLine> lst = new List<ProductionLine>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false
            };
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }

            return from item in lst
                   where lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = string.Format("{0}[{1}]", item.Name, item.Key),
                       Value = item.Key
                   };
        }

        /// <summary>
        /// 取得工步清单
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRouteOperationList()
        {
            //获取用户拥有权限的工序。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.RouteOperation);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }

            IList<string> lstPackageOperation = new List<string>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = "Key.AttributeName='IsPackageOperation'"
            };
            using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
            {
                MethodReturnResult<IList<RouteOperationAttribute>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data != null)
                {
                    bool isPackageOperation = false;
                    lstPackageOperation = (from item in result.Data
                                           where bool.TryParse(item.Value, out isPackageOperation) == true
                                                 && isPackageOperation == true
                                           select item.Key.RouteOperationName).ToList();
                }
            }

            IList<RouteOperation> lst = new List<RouteOperation>();
            cfg.Where = "Status=1";
            cfg.OrderBy = "SortSeq";

            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }


            return from item in lst
                   where lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = item.Key,
                       Value = item.Key
                   };
        }
    }
}