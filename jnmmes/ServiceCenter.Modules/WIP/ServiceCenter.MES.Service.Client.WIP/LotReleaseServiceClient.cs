// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.WIP
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="LotReleaseServiceClient.cs" company="">
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
    /// 定义批次释放契约调用方式。
    /// </summary>
    public class LotReleaseServiceClient : ClientBase<ILotReleaseContract>, ILotReleaseContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LotReleaseServiceClient" /> class.
        /// </summary>
        public LotReleaseServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotReleaseServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public LotReleaseServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotReleaseServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotReleaseServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotReleaseServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotReleaseServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotReleaseServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotReleaseServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult Release(ReleaseParameter p)
        {
            return base.Channel.Release(p);
        }

        public async Task<MethodReturnResult> ReleaseAsync(ReleaseParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Release(p);
            });
        }
    }
}
