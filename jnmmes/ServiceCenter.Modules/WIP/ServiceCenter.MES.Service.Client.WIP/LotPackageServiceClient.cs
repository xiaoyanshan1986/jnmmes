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
    public class LotPackageServiceClient : ClientBase<ILotPackageContract>, ILotPackageContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LotPackageServiceClient" /> class.
        /// </summary>
        public LotPackageServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotPackageServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public LotPackageServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotPackageServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotPackageServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotPackageServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotPackageServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotPackageServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotPackageServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult Package(PackageParameter p)
        {
            return base.Channel.Package(p);
        }

        public MethodReturnResult FinishPackage(PackageParameter p)
        {
            return base.Channel.FinishPackage(p);
        }

        public async Task<MethodReturnResult> PackageAsync(PackageParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Package(p);
            });
        }



        public MethodReturnResult UnPackage(PackageParameter p)
        {
            return base.Channel.UnPackage(p);
        }

        public async Task<MethodReturnResult> UnPackageAsync(PackageParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.UnPackage(p);
            });
        }

        public MethodReturnResult<string> Generate(string lotNumber, bool isLastestPackage)
        {
            return base.Channel.Generate(lotNumber, isLastestPackage);
        }

        public async Task<MethodReturnResult<string>> GenerateAsync(string lotNumber, bool isLastestPackage)
        {
            return await Task.Run<MethodReturnResult<string>>(() =>
            {
                return base.Channel.Generate(lotNumber, isLastestPackage);
            });
        }


        public MethodReturnResult TrackOutPackage(PackageParameter p)
        {
            return base.Channel.TrackOutPackage(p);
        }

        public async Task<MethodReturnResult> TrackOutPackageAsync(PackageParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.TrackOutPackage(p);
            });
        }

    }
}
