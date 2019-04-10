// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.WIP
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="LotTrackOutServiceClient.cs" company="">
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
    /// 定义批次出站操作契约调用方式。
    /// </summary>
    public class LotTrackOutServiceClient : ClientBase<ILotTrackOutContract>, ILotTrackOutContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LotTrackOutServiceClient" /> class.
        /// </summary>
        public LotTrackOutServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotTrackOutServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public LotTrackOutServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotTrackOutServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotTrackOutServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotTrackOutServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotTrackOutServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotTrackOutServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotTrackOutServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult TrackOut(TrackOutParameter p)
        {
            return base.Channel.TrackOut(p);
        }

        public MethodReturnResult ModifyIVDataForLot(LotIVDataParameter lotIVDataParameter)
        {
            return base.Channel.ModifyIVDataForLot(lotIVDataParameter);
        }

        public async Task<MethodReturnResult> ModifyIVDataForLottAsync(LotIVDataParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.ModifyIVDataForLot(p);
            });
        }

        public async Task<MethodReturnResult> TrackOutAsync(TrackOutParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.TrackOut(p);
            });
        }
    }
}
