using NHibernate;
// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="RoleService.cs" company="">
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
    /// 实现角色管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RoleService : IRoleContract
    {
        /// <summary>
        /// 角色数据访问读写。
        /// </summary>
        /// <value>The role data engine.</value>
        public IRoleDataEngine RoleDataEngine { get; set; }
        /// <summary>
        /// Gets or sets the user in role data engine.
        /// </summary>
        /// <value>The user in role data engine.</value>
        public IUserInRoleDataEngine UserInRoleDataEngine { get; set; }
        /// <summary>
        /// Gets or sets the user own resource data engine.
        /// </summary>
        /// <value>The user own resource data engine.</value>
        public IRoleOwnResourceDataEngine RoleOwnResourceDataEngine { get; set; }

        /// <summary>
        /// 添加角色。
        /// </summary>
        /// <param name="obj">角色数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Role obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RoleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RoleService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RoleDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleService_OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改角色。
        /// </summary>
        /// <param name="obj">角色数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Role obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RoleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RoleService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RoleDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleService_OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除角色。
        /// </summary>
        /// <param name="key">角色标识符（角色名称）。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RoleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RoleService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RoleDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取角色数据。
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><see cref="MethodReturnResult&lt;Role&gt;" />,角色数据.</returns>
        public MethodReturnResult<Role> Get(string key)
        {
            MethodReturnResult<Role> result = new MethodReturnResult<Role>();
            if (!this.RoleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RoleService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RoleDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取角色数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Role&gt;" />,角色数据集合。</returns>
        public MethodReturnResult<IList<Role>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Role>> result = new MethodReturnResult<IList<Role>>();
            try
            {
                result.Data = this.RoleDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleService_OtherError, ex.Message);
            }
            return result;
        }


        public MethodReturnResult SetRoleUser(string roleName, IList<UserInRole> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //using (TransactionScope scope = new TransactionScope())
                ISession session = this.RoleDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.UserInRoleDataEngine.DeleteByRoleName(roleName);

                    foreach (UserInRole uir in lst)
                    {
                        if (uir.Key.RoleName != roleName) continue;
                        this.UserInRoleDataEngine.Insert(uir,session);
                    }
                    //scope.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleService_OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult SetRoleResource(string roleName, IList<RoleOwnResource> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //using (TransactionScope scope = new TransactionScope())
                ISession session = this.RoleDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.RoleOwnResourceDataEngine.DeleteByRoleName(roleName);

                    foreach (RoleOwnResource uir in lst)
                    {
                        if (uir.Key.RoleName != roleName) continue;
                        this.RoleOwnResourceDataEngine.Insert(uir,session);
                    }
                    //scope.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.RoleService_OtherError, ex.Message);
            }
            return result;
        }
    }
}
