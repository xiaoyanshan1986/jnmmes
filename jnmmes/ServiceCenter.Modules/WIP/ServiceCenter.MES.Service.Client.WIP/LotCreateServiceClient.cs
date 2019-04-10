// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.WIP
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="LotCreateServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;

/// <summary>
/// The WIP namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.WIP
{
    /// <summary>
    /// 定义批次创建管理契约调用方式。
    /// </summary>
    public class LotCreateServiceClient : ClientBase<ILotCreateContract>, ILotCreateContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LotCreateServiceClient" /> class.
        /// </summary>
        public LotCreateServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotCreateServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public LotCreateServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotCreateServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotCreateServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotCreateServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotCreateServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotCreateServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotCreateServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult Create(CreateParameter p)
        {
            return base.Channel.Create(p);
        }

        public async Task<MethodReturnResult> CreateAsync(CreateParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Create(p);
            });
        }
        public MethodReturnResult UpdateLotSEModules(Lot lot)
        {
            return base.Channel.UpdateLotSEModules(lot);
        }
        public MethodReturnResult<IList<string>> Generate(EnumLotType lotType, string orderNumber, int i, string prefix)
        {
            return base.Channel.Generate(lotType, orderNumber, i, prefix);
        }

        public async Task<MethodReturnResult<IList<string>>> GenerateAsync(EnumLotType lotType, string orderNumber, int i, string prefix)
        {
            return await Task.Run<MethodReturnResult<IList<string>>>(() =>
            {
                return base.Channel.Generate(lotType, orderNumber, i, prefix);
            });
        }
    }
}
