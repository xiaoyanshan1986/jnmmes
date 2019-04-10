// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.RPT
// Author           : 方军
// Created          : 2016-01-12 13:43:52.250
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="WIPDisplayServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.Model;
using System.Data;

/// <summary>
/// The RPT namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.RPT
{
    /// <summary>
    /// 组件日运营数据获取操作契约调用方式。
    /// </summary>
    public class RPTDailyDataServiceClient : ClientBase<IRPTDailyDataContract>, IRPTDailyDataContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RPTDailyDataServiceClient" /> class.
        /// </summary>
        public RPTDailyDataServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RPTDailyDataServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public RPTDailyDataServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RPTDailyDataServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RPTDailyDataServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RPTDailyDataServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RPTDailyDataServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RPTDailyDataServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RPTDailyDataServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult<DataSet> Get(ref RPTDailyDataGetParameter p)
        {
            return base.Channel.Get(ref p);
        }

        public async Task<MethodReturnResult<DataSet>> GetAsync(RPTDailyDataGetParameter p)
        {
            return await Task.Run<MethodReturnResult<DataSet>>(() =>
            {
                return base.Channel.Get(ref p);
            });
        }
    }
}
