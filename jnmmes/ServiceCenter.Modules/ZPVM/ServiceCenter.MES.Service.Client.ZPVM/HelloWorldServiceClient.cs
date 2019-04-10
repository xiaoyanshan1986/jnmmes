// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.ZPVM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="HelloWorldServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.Model;

/// <summary>
/// The ZPVM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.ZPVM
{
    /// <summary>
    /// 定义Hello World契约调用方式。
    /// </summary>
    public class HelloWorldServiceClient : ClientBase<IHelloWorldContract>, IHelloWorldContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelloWorldServiceClient" /> class.
        /// </summary>
        public HelloWorldServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelloWorldServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public HelloWorldServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelloWorldServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public HelloWorldServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelloWorldServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public HelloWorldServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelloWorldServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public HelloWorldServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }


        public string Hello(string name)
        {
            return base.Channel.Hello(name);
        }
    }
}
