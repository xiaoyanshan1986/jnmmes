// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.WIP
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="PackageQueryServiceClient.cs" company="">
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
using System.Data;

/// <summary>
/// The WIP namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.WIP
{
    /// <summary>
    /// 定义包装查询契约调用方式。
    /// </summary>
    public class PackageOemQueryServiceClient : ClientBase<IPackageOemQueryContract>, IPackageOemQueryContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageOemQueryServiceClient" /> class.
        /// </summary>
        public PackageOemQueryServiceClient()
        {
             
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryOemServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public PackageOemQueryServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageOemQueryServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageOemQueryServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageOemQueryServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public MethodReturnResult<PackageOemDetail> Get(PackageOemDetailKey key)
        {
            return base.Channel.Get(key);
        }

        public MethodReturnResult<IList<PackageOemDetail>> Get(ref PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }

        public MethodReturnResult<PackageOemDetail> GetDetail(PackageOemDetailKey key)
        {
            return base.Channel.GetDetail(key);
        }

        public MethodReturnResult<IList<PackageOemDetail>> GetDetail(ref PagingConfig cfg)
        {
            return base.Channel.GetDetail(ref cfg);
        }

        public async Task<MethodReturnResult<PackageOemDetail>> GetAsync(PackageOemDetailKey key)
        {
            return await Task.Run<MethodReturnResult<PackageOemDetail>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        public async Task<MethodReturnResult<PackageOemDetail>> GetDetailAsync(PackageOemDetailKey key)
        {
            return await Task.Run<MethodReturnResult<PackageOemDetail>>(() =>
            {
                return base.Channel.GetDetail(key);
            });
        }
        public MethodReturnResult<DataSet> GetPackageTransaction(string key)
        {
            return base.Channel.GetPackageTransaction(key);
        }
        public MethodReturnResult CleanBin(string lineCode, string binNo)
        {
            return base.Channel.CleanBin(lineCode, binNo);
        }
    }
}
