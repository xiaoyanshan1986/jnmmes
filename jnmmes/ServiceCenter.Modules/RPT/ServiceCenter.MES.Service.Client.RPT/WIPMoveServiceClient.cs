// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.RPT
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="WIPMoveServiceClient.cs" company="">
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
    /// 定义在制品Move数据获取操作契约调用方式。
    /// </summary>
    public class WIPMoveServiceClient : ClientBase<IWIPMoveContract>, IWIPMoveContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WIPMoveServiceClient" /> class.
        /// </summary>
        public WIPMoveServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPMoveServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public WIPMoveServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPMoveServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WIPMoveServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPMoveServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WIPMoveServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPMoveServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WIPMoveServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult<DataSet> Get(WIPMoveGetParameter p)
        {
            return base.Channel.Get(p);
        }
        public MethodReturnResult<DataSet> GetWipMoveForStep(WIPMoveGetParameter p)
        {
            return base.Channel.GetWipMoveForStep(p);
        }

        public async Task<MethodReturnResult<DataSet>> GetAsync(WIPMoveGetParameter p)
        {
            return await Task.Run<MethodReturnResult<DataSet>>(() =>
            {
                return base.Channel.Get(p);
            });
        }


        public MethodReturnResult<DataSet> GetDetail(ref WIPMoveDetailGetParameter p)
        {
            return base.Channel.GetDetail(ref p);
        }

        public MethodReturnResult<DataSet> GetLotInformation(string lot)
        {
            return base.Channel.GetLotInformation(lot);
        }
        public MethodReturnResult<DataSet> GetDailyQuantityOfWIP(QMSemiProductionGetParameter p)
        {
            return base.Channel.GetDailyQuantityOfWIP(p);
        }
    }
}
