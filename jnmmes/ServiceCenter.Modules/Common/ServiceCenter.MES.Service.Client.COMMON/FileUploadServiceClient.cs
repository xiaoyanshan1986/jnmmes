// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.WIP
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="FileUploadServiceClient.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using ServiceCenter.Model;
using ServiceCenter.MES.Service.Contract.COMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;


namespace ServiceCenter.Service.Client
{
    /// <summary>
    /// 定义文件上传操作契约调用方式。
    /// </summary>
    public class FileUploadServiceClient : ClientBase<IFileUploadContract>, IFileUploadContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadServiceClient" /> class.
        /// </summary>
        public FileUploadServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public FileUploadServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public FileUploadServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public FileUploadServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public FileUploadServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult<string> Upload(string fileName, Byte[] fileContent)
        {
            return base.Channel.Upload(fileName,fileContent);
        }

        public async Task<MethodReturnResult> UploadAsync(string fileName, Byte[] fileContent)
        {
            return await Task.Run<MethodReturnResult<string>>(() =>
            {
                return base.Channel.Upload(fileName,fileContent);
            });
        }
    }
}
