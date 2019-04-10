// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.LSM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="LineStoreMaterialServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.LSM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.Model;

/// <summary>
/// The LSM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.LSM
{
    /// <summary>
    /// 定义线边仓物料管理契约调用方式。
    /// </summary>
    public class LineStoreMaterialServiceClient : ClientBase<ILineStoreMaterialContract>, ILineStoreMaterialContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineStoreMaterialServiceClient" /> class.
        /// </summary>
        public LineStoreMaterialServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStoreMaterialServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public LineStoreMaterialServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStoreMaterialServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LineStoreMaterialServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStoreMaterialServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LineStoreMaterialServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStoreMaterialServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LineStoreMaterialServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        
        /// <summary>
        /// 获取线边仓物料数据。
        /// </summary>
        /// <param name="key">线边仓物料标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LineStoreMaterial&gt;" />,线边仓物料数据.</returns>
        public MethodReturnResult<LineStoreMaterial> Get(LineStoreMaterialKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;LineStoreMaterial&gt;&gt;.</returns>
        public async Task<MethodReturnResult<LineStoreMaterial>> GetAsync(LineStoreMaterialKey key)
        {
            return await Task.Run<MethodReturnResult<LineStoreMaterial>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取线边仓物料数据集合。
        /// </summary>
        /// <param name="cfg">查询线边仓物料.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;LineStoreMaterial&gt;&gt;，线边仓物料数据集合.</returns>
        public MethodReturnResult<IList<LineStoreMaterial>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }


        /// <summary>
        /// 获取线边仓物料明细数据。
        /// </summary>
        /// <param name="key">线边仓物料明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LineStoreMaterialDetail&gt;" />,线边仓物料明细数据.</returns>
        public MethodReturnResult<LineStoreMaterialDetail> GetDetail(LineStoreMaterialDetailKey key)
        {
            return base.Channel.GetDetail(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;LineStoreMaterialDetail&gt;&gt;.</returns>
        public async Task<MethodReturnResult<LineStoreMaterialDetail>> GetDetailAsync(LineStoreMaterialDetailKey key)
        {
            return await Task.Run<MethodReturnResult<LineStoreMaterialDetail>>(() =>
            {
                return base.Channel.GetDetail(key);
            });
        }

        /// <summary>
        /// 获取线边仓物料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询线边仓物料明细.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;LineStoreMaterialDetail&gt;&gt;，线边仓物料明细数据集合.</returns>
        public MethodReturnResult<IList<LineStoreMaterialDetail>> GetDetail(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.GetDetail(ref cfg);
        }
        /// <summary>
        /// 物料拆批
        /// </summary>
        /// <param name="sparam"></param>
        /// <returns></returns>
        public MethodReturnResult SplitMaterialLot(SplitMaterialLotParameter sparam)
        {
            return base.Channel.SplitMaterialLot(sparam);
        }
    }
}
