// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="ResourceService.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ServiceCenter.MES.DataAccess.Interface.RBAC;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Contract.RBAC;
using ServiceCenter.MES.Service.RBAC.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.Service.RBAC
{
    /// <summary>
    /// 实现资源管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ResourceService : IResourceContract
    {
        /// <summary>
        /// 资源数据访问读写。
        /// </summary>
        /// <value>The resource data engine.</value>
        public IResourceDataEngine ResourceDataEngine { get; set; }


        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="obj">资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Resource obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ResourceDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ResourceService_IsExists, obj.Key.Type,obj.Key.Code);
                return result;
            }
            try
            {
                this.ResourceDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.ResourceService_OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改资源。
        /// </summary>
        /// <param name="obj">资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Resource obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ResourceDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ResourceService_IsNotExists, obj.Key.Type, obj.Key.Code);
                return result;
            }
            try
            {
                this.ResourceDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.ResourceService_OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除资源。
        /// </summary>
        /// <param name="key">资源标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(ResourceKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ResourceDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ResourceService_IsNotExists, key.Type,key.Code);
                return result;
            }
            try
            {
                this.ResourceDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.ResourceService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取资源数据。
        /// </summary>
        /// <param name="key">资源标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;Resource&gt;" />,资源数据.</returns>
        public MethodReturnResult<Resource> Get(ResourceKey key)
        {
            MethodReturnResult<Resource> result = new MethodReturnResult<Resource>();
            if (!this.ResourceDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ResourceService_IsNotExists, key.Type,key.Code);
                return result;
            }
            try
            {
                result.Data = this.ResourceDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.ResourceService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取资源数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Resource&gt;" />,资源数据集合。</returns>
        public MethodReturnResult<IList<Resource>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Resource>> result = new MethodReturnResult<IList<Resource>>();
            try
            {
                result.Data = this.ResourceDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.ResourceService_OtherError, ex.Message);
            }
            return result;
        }
    }
}
