// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="RoleOwnResourceService.cs" company="">
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
    /// 实现角色资源管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RoleOwnResourceService : IRoleOwnResourceContract
    {
        /// <summary>
        /// 角色资源数据访问读写。
        /// </summary>
        /// <value>The role own resource data engine.</value>
        public IRoleOwnResourceDataEngine RoleOwnResourceDataEngine { get; set; }


        /// <summary>
        /// 添加角色资源。
        /// </summary>
        /// <param name="obj">角色资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RoleOwnResource obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RoleOwnResourceDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RoleOwnResourceService_IsExists, obj.Key.RoleName,obj.Key.ResourceType,obj.Key.ResourceCode);
                return result;
            }
            try
            {
                this.RoleOwnResourceDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleOwnResourceService_OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改角色资源。
        /// </summary>
        /// <param name="obj">角色资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RoleOwnResource obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RoleOwnResourceDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RoleOwnResourceService_IsNotExists, obj.Key.RoleName,obj.Key.ResourceType,obj.Key.ResourceCode);
                return result;
            }
            try
            {
                this.RoleOwnResourceDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleOwnResourceService_OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除角色资源。
        /// </summary>
        /// <param name="key">角色资源标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RoleOwnResourceKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RoleOwnResourceDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RoleOwnResourceService_IsNotExists, key.RoleName,key.ResourceType,key.ResourceCode);
                return result;
            }
            try
            {
                this.RoleOwnResourceDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleOwnResourceService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取角色资源数据。
        /// </summary>
        /// <param name="key">角色资源标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;RoleOwnResource&gt;" />,角色资源数据.</returns>
        public MethodReturnResult<RoleOwnResource> Get(RoleOwnResourceKey key)
        {
            MethodReturnResult<RoleOwnResource> result = new MethodReturnResult<RoleOwnResource>();
            if (!this.RoleOwnResourceDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RoleOwnResourceService_IsNotExists, key.RoleName, key.ResourceType, key.ResourceCode);
                return result;
            }
            try
            {
                result.Data = this.RoleOwnResourceDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleOwnResourceService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取角色资源数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RoleOwnResource&gt;" />,角色资源数据集合。</returns>
        public MethodReturnResult<IList<RoleOwnResource>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RoleOwnResource>> result = new MethodReturnResult<IList<RoleOwnResource>>();
            try
            {
                result.Data = this.RoleOwnResourceDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleOwnResourceService_OtherError, ex.Message);
            }
            return result;
        }
    }
}
