using NHibernate;
// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="UserService.cs" company="">
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
    /// 实现用户管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UserService : IUserContract
    {
        /// <summary>
        /// 用户数据访问读写。
        /// </summary>
        /// <value>The user data engine.</value>
        public IUserDataEngine UserDataEngine { get; set; }

        /// <summary>
        /// Gets or sets the user in role data engine.
        /// </summary>
        /// <value>The user in role data engine.</value>
        public IUserInRoleDataEngine UserInRoleDataEngine { get; set; }
        /// <summary>
        /// Gets or sets the user own resource data engine.
        /// </summary>
        /// <value>The user own resource data engine.</value>
        public IUserOwnResourceDataEngine UserOwnResourceDataEngine { get; set; }

        /// <summary>
        /// 添加用户。
        /// </summary>
        /// <param name="obj">用户数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(User obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.UserDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.UserService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.UserDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserService_OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改用户。
        /// </summary>
        /// <param name="obj">用户数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(User obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.UserDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.UserService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.UserDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 设置用户角色数据。
        /// </summary>
        /// <param name="loginName">用户登录名。</param>
        /// <param name="lst">角色数据集合。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult SetUserRole(string loginName, IList<UserInRole> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //using (TransactionScope scope = new TransactionScope()) 
                ISession session = this.UserDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                { 
                    this.UserInRoleDataEngine.DeleteByLoginName(loginName);
                   
                    foreach (UserInRole uir in lst) 
                    {
                        if (uir.Key.LoginName != loginName) continue;
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
                result.Message = String.Format(StringResource.UserService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 设置用户资源数据。
        /// </summary>
        /// <param name="loginName">用户登录名。</param>
        /// <param name="lst">用户资源数据集合。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult SetUserResource(string loginName, IList<UserOwnResource> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //using (TransactionScope scope = new TransactionScope())
                ISession session = this.UserDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.UserOwnResourceDataEngine.DeleteByLoginName(loginName);

                    foreach (UserOwnResource uir in lst)
                    {
                        if (uir.Key.LoginName != loginName) continue;
                        this.UserOwnResourceDataEngine.Insert(uir);
                    }
                    //scope.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserService_OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="key">用户标识符（用户登录名）。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.UserDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.UserService_IsNotExists, key);
                return result;
            }
            try
            {
                this.UserDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取用户数据。
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><see cref="MethodReturnResult&lt;User&gt;" />,用户数据.</returns>
        public MethodReturnResult<User> Get(string key)
        {
            MethodReturnResult<User> result = new MethodReturnResult<User>();
            if (!this.UserDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.UserService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.UserDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取用户数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;User&gt;" />,用户数据集合。</returns>
        public MethodReturnResult<IList<User>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<User>> result = new MethodReturnResult<IList<User>>();
            try
            {
                result.Data = this.UserDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.UserService_OtherError, ex.Message);
            }
            return result;
        }


    }
}
