// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Contract.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="IUserInRoleContract.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Contract.RBAC
{
    /// <summary>
    /// 定义用户角色管理契约接口。
    /// </summary>
    [ServiceContract]
    public interface IUserInRoleContract
    {
        /// <summary>
        /// 添加用户角色。
        /// </summary>
        /// <param name="obj">用户角色数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Add(UserInRole obj);
        /// <summary>
        /// 修改用户角色。
        /// </summary>
        /// <param name="obj">用户角色数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Modify(UserInRole obj);
        /// <summary>
        /// 删除用户角色。
        /// </summary>
        /// <param name="key">用户角色标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Delete(UserInRoleKey key);
        /// <summary>
        /// 获取用户角色数据。
        /// </summary>
        /// <param name="key">用户角色标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;UserInRole&gt;" />,用户角色数据.</returns>
        [OperationContract]
        MethodReturnResult<UserInRole> Get(UserInRoleKey key);
        /// <summary>
        /// 获取用户角色数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;UserInRole&gt;&gt;，用户角色数据集合.</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<UserInRole>> Get(ref PagingConfig cfg);
    }
}
