// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Contract.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="IResourceContract.cs" company="">
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
    /// 定义资源管理契约接口。
    /// </summary>
    [ServiceContract]
    public interface IResourceContract
    {
        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="obj">资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Add(Resource obj);
        /// <summary>
        /// 修改资源。
        /// </summary>
        /// <param name="obj">资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Modify(Resource obj);
        /// <summary>
        /// 删除资源。
        /// </summary>
        /// <param name="key">资源标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Delete(ResourceKey key);
        /// <summary>
        /// 获取资源数据。
        /// </summary>
        /// <param name="key">资源标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;Resource&gt;" />,资源数据.</returns>
        [OperationContract]
        MethodReturnResult<Resource> Get(ResourceKey key);
        /// <summary>
        /// 获取资源数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Resource&gt;&gt;，资源数据集合.</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<Resource>> Get(ref PagingConfig cfg);
    }
}
