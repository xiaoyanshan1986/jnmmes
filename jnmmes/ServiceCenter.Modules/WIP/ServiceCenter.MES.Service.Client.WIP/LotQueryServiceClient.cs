// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.WIP
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="LotQueryServiceClient.cs" company="">
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
    /// 定义批次查询契约调用方式。
    /// </summary>
    public class LotQueryServiceClient : ClientBase<ILotQueryContract>, ILotQueryContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LotQueryServiceClient" /> class.
        /// </summary>
        public LotQueryServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotQueryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public LotQueryServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotQueryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotQueryServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotQueryServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotQueryServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotQueryServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public LotQueryServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult<Lot> Get(string key)
        {
            return base.Channel.Get(key);
        }

        public MethodReturnResult<IList<Lot>> Get(ref PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }

        public MethodReturnResult<DataSet> GetEx(RPTLotQueryDetailParameter p)
        {
            return base.Channel.GetEx(p);
        }

        public MethodReturnResult<LotAttribute> GetAttribute(LotAttributeKey key)
        {
            return base.Channel.GetAttribute(key);
        }

        public MethodReturnResult<IList<LotAttribute>> GetAttribute(ref PagingConfig cfg)
        {
            return base.Channel.GetAttribute(ref cfg);
        }

        public MethodReturnResult<DataSet> GetAttributeEx(RPTLotQueryDetailParameter p)
        {
            return base.Channel.GetAttributeEx(p);
        }

        public MethodReturnResult<DataSet> GetLotList(RPTLotQueryDetailParameter p)
        {
            return base.Channel.GetLotList(p);
        }

        public async Task<MethodReturnResult<Lot>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<Lot>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        public async Task<MethodReturnResult<LotAttribute>> GetAttributeAsync(LotAttributeKey key)
        {
            return await Task.Run<MethodReturnResult<LotAttribute>>(() =>
            {
                return base.Channel.GetAttribute(key);
            });
        }


        public MethodReturnResult<LotTransactionHistory> GetLotTransactionHistory(string key)
        {
            return base.Channel.GetLotTransactionHistory(key);
        }

        public async Task<MethodReturnResult<LotTransactionHistory>> GetLotTransactionHistoryAsync(string key)
        {
            return await Task.Run<MethodReturnResult<LotTransactionHistory>>(() =>
            {
                return base.Channel.GetLotTransactionHistory(key);
            });
        }

        public MethodReturnResult<IList<LotTransactionHistory>> GetLotTransactionHistory(ref PagingConfig cfg)
        {
            return base.Channel.GetLotTransactionHistory(ref cfg);
        }

        public MethodReturnResult<LotTransaction> GetTransaction(string key)
        {
            return base.Channel.GetTransaction(key);
        }

        public async Task<MethodReturnResult<LotTransaction>> GetTransactionAsync(string key)
        {
            return await Task.Run<MethodReturnResult<LotTransaction>>(() =>
            {
                return base.Channel.GetTransaction(key);
            });
        }

        public MethodReturnResult<IList<LotTransaction>> GetTransaction(ref PagingConfig cfg)
        {
            return base.Channel.GetTransaction(ref cfg);
        }

        public MethodReturnResult<LotTransactionParameter> GetTransactionParameter(LotTransactionParameterKey key)
        {
            return base.Channel.GetTransactionParameter(key);
        }
        public async Task<MethodReturnResult<LotTransactionParameter>> GetTransactionParameterAsync(LotTransactionParameterKey key)
        {
            return await Task.Run<MethodReturnResult<LotTransactionParameter>>(() =>
            {
                return base.Channel.GetTransactionParameter(key);
            });
        }
        public MethodReturnResult<IList<LotTransactionParameter>> GetTransactionParameter(ref PagingConfig cfg)
        {
            return base.Channel.GetTransactionParameter(ref cfg);
        }

        public MethodReturnResult<LotTransactionDefect> GetLotTransactionDefect(LotTransactionDefectKey key)
        {
            return base.Channel.GetLotTransactionDefect(key);
        }
        public async Task<MethodReturnResult<LotTransactionDefect>> GetLotTransactionDefectAsync(LotTransactionDefectKey key)
        {
            return await Task.Run<MethodReturnResult<LotTransactionDefect>>(() =>
            {
                return base.Channel.GetLotTransactionDefect(key);
            });
        }
        public MethodReturnResult<IList<LotTransactionDefect>> GetLotTransactionDefect(ref PagingConfig cfg)
        {
            return base.Channel.GetLotTransactionDefect(ref cfg);
        }

        public MethodReturnResult<LotTransactionScrap> GetLotTransactionScrap(LotTransactionScrapKey key)
        {
            return base.Channel.GetLotTransactionScrap(key);
        }
        public async Task<MethodReturnResult<LotTransactionScrap>> GetLotTransactionScrapAsync(LotTransactionScrapKey key)
        {
            return await Task.Run<MethodReturnResult<LotTransactionScrap>>(() =>
            {
                return base.Channel.GetLotTransactionScrap(key);
            });
        }
        public MethodReturnResult<IList<LotTransactionScrap>> GetLotTransactionScrap(ref PagingConfig cfg)
        {
            return base.Channel.GetLotTransactionScrap(ref cfg);
        }

        public MethodReturnResult<LotTransactionPatch> GetLotTransactionPatch(LotTransactionPatchKey key)
        {
            return base.Channel.GetLotTransactionPatch(key);
        }
        public async Task<MethodReturnResult<LotTransactionPatch>> GetLotTransactionPatchAsync(LotTransactionPatchKey key)
        {
            return await Task.Run<MethodReturnResult<LotTransactionPatch>>(() =>
            {
                return base.Channel.GetLotTransactionPatch(key);
            });
        }
        public MethodReturnResult<IList<LotTransactionPatch>> GetLotTransactionPatch(ref PagingConfig cfg)
        {
            return base.Channel.GetLotTransactionPatch(ref cfg);
        }

        public MethodReturnResult<LotBOM> GetLotBOM(LotBOMKey key)
        {
            return base.Channel.GetLotBOM(key);
        }
        public async Task<MethodReturnResult<LotBOM>> GetLotBOMAsync(LotBOMKey key)
        {
            return await Task.Run<MethodReturnResult<LotBOM>>(() =>
            {
                return base.Channel.GetLotBOM(key);
            });
        }
        public MethodReturnResult<IList<LotBOM>> GetLotBOM(ref PagingConfig cfg)
        {
            return base.Channel.GetLotBOM(ref cfg);
        }

        public MethodReturnResult<LotTransactionEquipment> GetLotTransactionEquipment(string key)
        {
            return base.Channel.GetLotTransactionEquipment(key);
        }

        public async Task<MethodReturnResult<LotTransactionEquipment>> GetLotTransactionEquipmentAsync(string key)
        {
            return await Task.Run<MethodReturnResult<LotTransactionEquipment>>(() =>
            {
                return base.Channel.GetLotTransactionEquipment(key);
            });
        }

        public MethodReturnResult<IList<LotTransactionEquipment>> GetLotTransactionEquipment(ref PagingConfig cfg)
        {
            return base.Channel.GetLotTransactionEquipment(ref cfg);
        }

        public MethodReturnResult<LotJob> GetLotJob(string key)
        {
            return base.Channel.GetLotJob(key);
        }

        public async Task<MethodReturnResult<LotJob>> GetLotJobAsync(string key)
        {
            return await Task.Run<MethodReturnResult<LotJob>>(() =>
            {
                return base.Channel.GetLotJob(key);
            });
        }

        public MethodReturnResult<IList<LotJob>> GetLotJob(ref PagingConfig cfg)
        {
            return base.Channel.GetLotJob(ref cfg);
        }


        public MethodReturnResult<LotTransactionCheck> GetLotTransactionCheck(string key)
        {
            return base.Channel.GetLotTransactionCheck(key);
        }

        public async Task<MethodReturnResult<LotTransactionCheck>> GetLotTransactionCheckAsync(string key)
        {
            return await Task.Run<MethodReturnResult<LotTransactionCheck>>(() =>
            {
                return base.Channel.GetLotTransactionCheck(key);
            });
        }

        public MethodReturnResult<IList<LotTransactionCheck>> GetLotTransactionCheck(ref PagingConfig cfg)
        {
            return base.Channel.GetLotTransactionCheck(ref cfg);
        }

        public MethodReturnResult<DataSet> GetLotCount(string where)
        {
            return base.Channel.GetLotCount(where);
        }

        public MethodReturnResult<DataSet> GetLotColor(string lot)
        {
            return base.Channel.GetLotColor(lot);
        }
        public MethodReturnResult<DataSet> GetRPTLotMaterialList(ref RPTLotMateriallistParameter p)
        {
            return base.Channel.GetRPTLotMaterialList(ref p);
        }

        public MethodReturnResult<DataSet> GetRPTLotProcessingHistory( ref RPTLotMateriallistParameter p)
        {
            return base.Channel.GetRPTLotProcessingHistory( ref p);
        }

        public MethodReturnResult<DataSet> GetMapDataQueryDb(ref RPTLotQueryDetailParameter p)
        {
            return base.Channel.GetMapDataQueryDb(ref p);
        }
    }
}
