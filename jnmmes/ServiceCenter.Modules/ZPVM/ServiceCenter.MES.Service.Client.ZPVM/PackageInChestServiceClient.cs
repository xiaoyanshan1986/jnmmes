// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.ZPVM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="PackageInChestServiceClient.cs" company="">
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
using System.Data;
using ServiceCenter.MES.Service.Contract.ERP;

/// <summary>
/// The ZPVM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.ZPVM
{
    #region 注释Generate和FinishPackageInChest
    //public MethodReturnResult<string> Generate(string chestNo,string packageNo)
    //{
    //    return base.Channel.Generate(chestNo,packageNo);
    //}

    //public async Task<MethodReturnResult<string>> GenerateAsync(string chestNo, string packageNo)
    //{
    //    return await Task.Run<MethodReturnResult<string>>(() =>
    //    {
    //        return base.Channel.Generate(chestNo,packageNo);
    //    });
    //}

    //public MethodReturnResult FinishPackageInChest(ChestParameter p)
    //{
    //    return base.Channel.FinishPackageInChest(p);
    //}
    #endregion

    /// <summary>
    /// 定义批次包装操作契约调用方式。
    /// </summary>
    public class PackageInChestServiceClient : ClientBase<IPackageInChestContract>, IPackageInChestContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageInChestServiceClient" /> class.
        /// </summary>
        public PackageInChestServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageInChestServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public PackageInChestServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageInChestServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageInChestServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageInChestServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageInChestServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageInChestServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public PackageInChestServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult Chest(ChestParameter p)
        {
            return base.Channel.Chest(p);
        }

        public MethodReturnResult<DataSet> GetChestDetail(ref ChestParameter p)
        {
            return base.Channel.GetChestDetail(ref p);
        }

        public MethodReturnResult<DataSet> GetChestDetailByDB(ref ChestParameter p)
        {
            return base.Channel.GetChestDetailByDB(ref p);
        }

        public MethodReturnResult<DataSet> GetRefreshChestDetailByDB(ref ChestParameter p)
        {
            return base.Channel.GetRefreshChestDetailByDB(ref p);
        }

        public MethodReturnResult<DataSet> GetCheckedChestDetailByDB(ref ChestParameter p)
        {
            return base.Channel.GetCheckedChestDetailByDB(ref p);
        }

        public MethodReturnResult ChangeChest(string chestNo, string userName)
        {
            return base.Channel.ChangeChest(chestNo,userName);
        }

        public MethodReturnResult<IList<ChestDetail>> GetDetail(ref PagingConfig cfg)
        {
            return base.Channel.GetDetail(ref cfg);
        }

        public MethodReturnResult<ChestDetail> GetDetail(ChestDetailKey key)
        {
            return base.Channel.GetDetail(key);
        }

        public MethodReturnResult<Chest> Get(string key)
        {
            return base.Channel.Get(key);
        }

        public MethodReturnResult<IList<Chest>> Get(ref PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }        

        public async Task<MethodReturnResult> ChestAsync(ChestParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Chest(p);
            });
        }

        public MethodReturnResult UnPackageInChest(ChestParameter p)
        {
            return base.Channel.UnPackageInChest(p);
        }

        public async Task<MethodReturnResult> UnPackageInChestAsync(ChestParameter p)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.UnPackageInChest(p);
            });
        }       

        public MethodReturnResult<string> GetChestNo(string packageNo, string chestNo, bool isLastChest, bool isManual)
        {
            return base.Channel.GetChestNo(packageNo, chestNo, isLastChest, isManual);
        }

        public MethodReturnResult CheckPackageInChest(string packageNo, string chestNo, string userName)
        {
            return base.Channel.CheckPackageInChest(packageNo, chestNo, userName);
        }

        public MethodReturnResult UnCheckPackageInChest(string packageNo, string chestNo, string userName)
        {
            return base.Channel.UnCheckPackageInChest(packageNo, chestNo, userName);
        }

        public MethodReturnResult FinishChest(ChestParameter p)
        {
            return base.Channel.FinishChest(p);
        }

        public MethodReturnResult GetREbackdata(REbackdataParameter p)
        {
            return base.Channel.GetREbackdata(p);
        }
    }
}
