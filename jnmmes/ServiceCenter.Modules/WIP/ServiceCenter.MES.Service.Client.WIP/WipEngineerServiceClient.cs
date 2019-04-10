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
    /// 定义WipEngineer契约调用方法
    /// </summary>
    public class WipEngineerServiceClient : ClientBase<IWipEngineerContract>, IWipEngineerContract, IDisposable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        public WipEngineerServiceClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public WipEngineerServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WipEngineerServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WipEngineerServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageQueryServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WipEngineerServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public ServiceCenter.Model.MethodReturnResult TrackOutLot(TrackOutParameter p)
        {
            return base.Channel.TrackOutLot(p);
        }

        public ServiceCenter.Model.MethodReturnResult TrackInLot(TrackInParameter p)
        {
            return base.Channel.TrackInLot(p);
        }

        public MethodReturnResult ExecuteInPackageDetail(Lot lot,string packageLine)
        {
            return base.Channel.ExecuteInPackageDetail(lot, packageLine);
        }

        public MethodReturnResult<IList<PrintLabelLog>> Get(ref PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }

        public MethodReturnResult UpdatePrintLabelLog(PrintLabelLog printLabelLog)
        {
            return base.Channel.UpdatePrintLabelLog(printLabelLog);
        }

        public MethodReturnResult UnDoPackageCorner(string lotNumber)
        {
            return base.Channel.UnDoPackageCorner(lotNumber);
        }
        public MethodReturnResult ExecuteInAbnormalBIN(Lot lot, string PackageLine)
        {
            return base.Channel.ExecuteInAbnormalBIN( lot,  PackageLine);
        }
    }
    
}
