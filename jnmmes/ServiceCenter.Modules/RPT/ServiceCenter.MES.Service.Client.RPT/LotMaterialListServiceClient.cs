// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.RPT
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="LotMaterialListServiceClient.cs" company="">
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
    /// 定义批次物料数据查询的契约调用方式。
    /// </summary>
    public class LotMaterialListServiceClient : ClientBase<ILotMaterialListContract>, ILotMaterialListContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        public LotMaterialListServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public LotMaterialListServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotMaterialListServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotMaterialListServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotMaterialListServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult<DataSet> Get(ref LotMaterialListQueryParameter p)
        {
            return base.Channel.Get(ref p);
        }

        /// <summary>
        /// 查询物料消耗数据。
        /// </summary>
        /// <param name="p">参数</param>
        /// <returns>返回结果集</returns>
        public MethodReturnResult<DataSet> GetMaterialConsume(RPTDailyDataGetParameter p)
        {
            return base.Channel.GetMaterialConsume(p);
        }

        /// <summary>
        /// 获取物料出货数据
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetRPTMaterialData(ref MaterialDataParameter p)
        {
            return base.Channel.GetRPTMaterialData(ref p);
        }
    }
}
