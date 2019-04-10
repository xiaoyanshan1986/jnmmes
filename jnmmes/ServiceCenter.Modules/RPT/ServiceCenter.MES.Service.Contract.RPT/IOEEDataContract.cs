// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RPT
// Author           : 李娇
// Created          : 2016-9-18 15:03:51
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="RPTDailyDataService.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.Data;

namespace ServiceCenter.MES.Service.Contract.RPT
{
    /// <summary>
    /// 组件日运营数据获取参数类。
    /// </summary>
    [DataContract]
    public class OEEDataGetParameter
    {
        /// <summary>
        /// 报表代码
        /// </summary>
        [DataMember]
        public string ReportCode { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        [DataMember]
        public string EndDate { get; set; }
        /// <summary>
        /// 车间
        /// </summary>
        [DataMember]
        public string LocationName { get; set; }
        /// <summary>
        /// 线别
        /// </summary>
        [DataMember]
        public string LineCode { get; set; }
        /// <summary>
        /// 工单号
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }
        /// <summary>
        /// 产品型号
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }
        /// <summary>
        /// 工步
        /// </summary>
        [DataMember]
        public string StepName { get; set; }
    }

    /// <summary>
    /// 组件日运营报表契约接口。
    /// </summary>
    [ServiceContract]
    public interface IOEEDataContract
    {
        /// <summary>
        /// 组件日运营数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult<DataSet> Get(OEEDataGetParameter p);
        [OperationContract]
        MethodReturnResult<DataSet> GetOEEDailyData(OEEDataGetParameter p);
    }
}
