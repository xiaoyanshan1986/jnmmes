// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.WIP
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="LotPackageServiceClient.cs" company="">
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
    /// 定义批次包装操作契约调用方式。
    /// </summary>
    public class LotBinServiceClient : ClientBase<ILotBinContract>, ILotBinContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LotPackageServiceClient" /> class.
        /// </summary>
        public LotBinServiceClient() {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotPackageServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public LotBinServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotPackageServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotBinServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotPackageServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotBinServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotPackageServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotBinServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult InBin(InBinParameter p)
        {
            return base.Channel.InBin(p);
        }

        public MethodReturnResult ChkBin(InBinParameter p)
        {
            return base.Channel.ChkBin(p);
        }

        /// <summary>
        /// 组件路径显示
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult PathCheck(InBinParameter p)
        {
            return base.Channel.PathCheck(p);
        }


        public MethodReturnResult<IList<PackageBin>> QueryBinListFromPackageLine(string packageLine)
        {
            return base.Channel.QueryBinListFromPackageLine(packageLine);
        }


        public async Task<MethodReturnResult> InBinAsync(InBinParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.InBin(p);
            });
        }
    }
}
