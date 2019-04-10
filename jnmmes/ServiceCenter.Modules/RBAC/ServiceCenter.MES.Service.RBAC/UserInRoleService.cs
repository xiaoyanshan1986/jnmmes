// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="UserInRoleService.cs" company="">
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
using System.Transactions;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.Service.RBAC
{
    /// <summary>
    /// 实现角色资源管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UserInRoleService : IUserInRoleContract
    {
        /// <summary>
        /// 角色资源数据访问读写。
        /// </summary>
        /// <value>The user in role data engine.</value>
        public IUserInRoleDataEngine UserInRoleDataEngine { get; set; }


        /// <summary>
        /// 添加角色资源。
        /// </summary>
        /// <param name="obj">角色资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(UserInRole obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.UserInRoleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.UserInRoleService_IsExists, obj.Key.LoginName,obj.Key.RoleName);
                return result;
            }
            try
            {
                this.UserInRoleDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserInRoleService_OtherError,ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 修改角色资源。
        /// </summary>
        /// <param name="obj">角色资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(UserInRole obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.UserInRoleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.UserInRoleService_IsNotExists, obj.Key.LoginName, obj.Key.RoleName);
                return result;
            }
            try
            {
                this.UserInRoleDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserInRoleService_OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除角色资源。
        /// </summary>
        /// <param name="key">角色资源标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(UserInRoleKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.UserInRoleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.UserInRoleService_IsNotExists, key.LoginName, key.RoleName);
                return result;
            }
            try
            {
                this.UserInRoleDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserInRoleService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取角色资源数据。
        /// </summary>
        /// <param name="key">用户角色标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;UserInRole&gt;" />,角色资源数据.</returns>
        public MethodReturnResult<UserInRole> Get(UserInRoleKey key)
        {
            MethodReturnResult<UserInRole> result = new MethodReturnResult<UserInRole>();
            if (!this.UserInRoleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.UserInRoleService_IsNotExists, key.LoginName, key.RoleName);
                return result;
            }
            try
            {
                result.Data = this.UserInRoleDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserInRoleService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取角色资源数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;UserInRole&gt;" />,角色资源数据集合。</returns>
        public MethodReturnResult<IList<UserInRole>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<UserInRole>> result = new MethodReturnResult<IList<UserInRole>>();
            try
            {
                result.Data = this.UserInRoleDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserInRoleService_OtherError, ex.Message);
            }
            return result;
        }
    }
}
