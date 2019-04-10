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
    public class PackageQueryServiceClient : ClientBase<IPackageQueryContract>, IPackageQueryContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        public PackageQueryServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public PackageQueryServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageQueryServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageQueryServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageQueryServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult<Package> Get(string key)
        {
            return base.Channel.Get(key);
        }

        public MethodReturnResult<IList<Package>> Get(ref PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }

        public MethodReturnResult<PackageDetail> GetDetail(PackageDetailKey key)
        {
            return base.Channel.GetDetail(key);
        }

        public MethodReturnResult<IList<PackageDetail>> GetDetail(ref PagingConfig cfg)
        {
            return base.Channel.GetDetail(ref cfg);
        }

        public async Task<MethodReturnResult<Package>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<Package>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        public async Task<MethodReturnResult<PackageDetail>> GetDetailAsync(PackageDetailKey key)
        {
            return await Task.Run<MethodReturnResult<PackageDetail>>(() =>
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
            return base.Channel.CleanBin(lineCode,binNo);
        }
        public MethodReturnResult<DataSet> GetRPTpackagelist(RPTpackagelistParameter p)
        {
            return base.Channel.GetRPTpackagelist(p);
        }

        public MethodReturnResult<DataSet> GetOEMpackagelist(RPTpackagelistParameter p)
        {
            return base.Channel.GetOEMpackagelist(p);
        }

        /// <summary> 存储过程获取包装历史记录数据查询 </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetRPTpackagelistQueryDb(ref RPTpackagelistParameter p)
        {
            return base.Channel.GetRPTpackagelistQueryDb(ref p);
        }
        /// <summary> 存储过程获取包装历史记录数据查询 </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetPackageTransactionQueryDb(ref RPTpackagelistParameter p)
        {
            return base.Channel.GetPackageTransactionQueryDb(ref p);
        }


        public MethodReturnResult<DataSet> GetRPTPackageNoInfo(RPTpackagelistParameter p)
        {
            return base.Channel.GetRPTPackageNoInfo(p);
        }

        public MethodReturnResult UpdateAdd(Package p, string action)
        {
            return base.Channel.UpdateAdd(p, action);
        }
    }
}
