// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.EDC
// Author           : fangjun
// Created          : 06-04-2017
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="DataAcquisitionFieldServiceClient.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ServiceCenter.MES.Service.Contract.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Service.Client.EDC
{
    /// <summary>
    /// 定义采集字段管理契约调用方式。
    /// </summary>
    public class DataAcquisitionFieldServiceClient : ClientBase<IDataAcquisitionFieldContract>, IDataAcquisitionFieldContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcquisitionFieldServiceClient" /> class.
        /// </summary>
        public DataAcquisitionFieldServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcquisitionFieldServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public DataAcquisitionFieldServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcquisitionFieldServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public DataAcquisitionFieldServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcquisitionFieldServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public DataAcquisitionFieldServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcquisitionFieldServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public DataAcquisitionFieldServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加基础数据。
        /// </summary>
        /// <param name="obj">基础数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(DataAcquisitionField obj)
        {
            return base.Channel.Add(obj);
        }

        public async Task<MethodReturnResult> AddAsync(DataAcquisitionField obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改基础数据。
        /// </summary>
        /// <param name="obj">基础数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(DataAcquisitionField obj)
        {
            return base.Channel.Modify(obj);
        }

        public async Task<MethodReturnResult> ModifyAsync(DataAcquisitionField obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }

        /// <summary>
        /// 删除基础数据。
        /// </summary>
        /// <param name="key">基础数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(DataAcquisitionFieldKey key)
        {
            return base.Channel.Delete(key);
        }


        public async Task<MethodReturnResult> DeleteAsync(DataAcquisitionFieldKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }
        /// <summary>
        /// 获取基础数据数据。
        /// </summary>
        /// <param name="key">基础数据标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionField&gt;" />,基础数据数据.</returns>
        public MethodReturnResult<DataAcquisitionField> Get(DataAcquisitionFieldKey key)
        {
            return base.Channel.Get(key);
        }

        public async Task<MethodReturnResult<DataAcquisitionField>> GetAsync(DataAcquisitionFieldKey key)
        {
            return await Task.Run<MethodReturnResult<DataAcquisitionField>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取基础数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;DataAcquisitionField&gt;&gt;，基础数据数据集合.</returns>
        public ServiceCenter.Model.MethodReturnResult<IList<DataAcquisitionField>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
