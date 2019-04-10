// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="ReasonCodeCategoryDetailServiceClient.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Service.Contract.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.Model;

/// <summary>
/// The FMM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.FMM
{
    /// <summary>
    /// 定义代码分组明细管理契约调用方式。
    /// </summary>
    public class EquipmentReasonCodeCategoryDetailServiceClient : ClientBase<IEquipmentReasonCodeCategoryDetailContract>, IEquipmentReasonCodeCategoryDetailContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonCodeCategoryDetailServiceClient" /> class.
        /// </summary>
        public EquipmentReasonCodeCategoryDetailServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonCodeCategoryDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public EquipmentReasonCodeCategoryDetailServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonCodeCategoryDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentReasonCodeCategoryDetailServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonCodeCategoryDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentReasonCodeCategoryDetailServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonCodeCategoryDetailServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentReasonCodeCategoryDetailServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加代码分组明细。
        /// </summary>
        /// <param name="obj">代码分组明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentReasonCodeCategoryDetail obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(EquipmentReasonCodeCategoryDetail obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改代码分组明细。
        /// </summary>
        /// <param name="obj">代码分组明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(EquipmentReasonCodeCategoryDetail obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(EquipmentReasonCodeCategoryDetail obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除代码分组明细。
        /// </summary>
        /// <param name="key">代码分组明细标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(EquipmentReasonCodeCategoryDetailKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">代码分组明细标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(EquipmentReasonCodeCategoryDetailKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }
        /// <summary>
        /// 获取代码分组明细数据。
        /// </summary>
        /// <param name="key">代码分组明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;ReasonCodeCategoryDetail&gt;" />,代码分组明细数据.</returns>
        public MethodReturnResult<EquipmentReasonCodeCategoryDetail> Get(EquipmentReasonCodeCategoryDetailKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;ReasonCodeCategoryDetail&gt;&gt;.</returns>
        public async Task<MethodReturnResult<EquipmentReasonCodeCategoryDetail>> GetAsync(EquipmentReasonCodeCategoryDetailKey key)
        {
            return await Task.Run<MethodReturnResult<EquipmentReasonCodeCategoryDetail>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取代码分组明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;ReasonCodeCategoryDetail&gt;&gt;，代码分组明细数据集合.</returns>
        public MethodReturnResult<IList<EquipmentReasonCodeCategoryDetail>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
