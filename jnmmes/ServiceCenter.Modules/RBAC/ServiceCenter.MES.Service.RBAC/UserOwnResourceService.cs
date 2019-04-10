// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="UserOwnResourceService.cs" company="">
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
    /// 实现用户资源管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UserOwnResourceService : IUserOwnResourceContract
    {
        /// <summary>
        /// 用户资源数据访问读写。
        /// </summary>
        /// <value>The user own resource data engine.</value>
        public IUserOwnResourceDataEngine UserOwnResourceDataEngine { get; set; }


        /// <summary>
        /// 添加用户资源。
        /// </summary>
        /// <param name="obj">用户资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(UserOwnResource obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.UserOwnResourceDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.UserOwnResourceService_IsExists, obj.Key.LoginName,obj.Key.ResourceType,obj.Key.ResourceCode);
                return result;
            }
            try
            {
                this.UserOwnResourceDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserOwnResourceService_OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改用户资源。
        /// </summary>
        /// <param name="obj">用户资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(UserOwnResource obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.UserOwnResourceDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.UserOwnResourceService_IsNotExists, obj.Key.LoginName,obj.Key.ResourceType,obj.Key.ResourceCode);
                return result;
            }
            try
            {
                this.UserOwnResourceDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserOwnResourceService_OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除用户资源。
        /// </summary>
        /// <param name="key">用户资源标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(UserOwnResourceKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.UserOwnResourceDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.UserOwnResourceService_IsNotExists, key.LoginName,key.ResourceType,key.ResourceCode);
                return result;
            }
            try
            {
                this.UserOwnResourceDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserOwnResourceService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取用户资源数据。
        /// </summary>
        /// <param name="key">用户资源标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;UserOwnResource&gt;" />,用户资源数据.</returns>
        public MethodReturnResult<UserOwnResource> Get(UserOwnResourceKey key)
        {
            MethodReturnResult<UserOwnResource> result = new MethodReturnResult<UserOwnResource>();
            if (!this.UserOwnResourceDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.UserOwnResourceService_IsNotExists, key.LoginName, key.ResourceType, key.ResourceCode);
                return result;
            }
            try
            {
                result.Data = this.UserOwnResourceDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserOwnResourceService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取用户资源数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;UserOwnResource&gt;" />,用户资源数据集合。</returns>
        public MethodReturnResult<IList<UserOwnResource>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<UserOwnResource>> result = new MethodReturnResult<IList<UserOwnResource>>();
            try
            {
                result.Data = this.UserOwnResourceDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserOwnResourceService_OtherError, ex.Message);
            }
            return result;
        }
    }
}
