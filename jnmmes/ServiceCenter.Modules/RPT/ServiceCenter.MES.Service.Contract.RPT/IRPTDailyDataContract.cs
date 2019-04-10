// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RPT
// Author           : 方军
// Created          : 2016-01-12 13:43:52.250
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
    public class RPTDailyDataGetParameter
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
        /// 班别
        /// </summary>
        [DataMember]
        public string SchedulingCode { get; set; }

        /// <summary>
        /// 月数据显示数量（当前月向前）
        /// </summary>
        [DataMember]
        public int MonthDataNumber { get; set; }

        /// <summary>
        /// 年数据显示数量（当前年向前）
        /// </summary>
        [DataMember]
        public int YearDataNumber { get; set; }

        /// <summary>
        /// 页号。
        /// </summary>
        [DataMember]
        public int PageNo { get; set; }

        /// <summary>
        /// 每页大小。
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// 总记录数。
        /// </summary>
        [DataMember]
        public int Records { get; set; }
    }

    /// <summary>
    /// 组件日运营报表契约接口。
    /// </summary>
    [ServiceContract]
    public interface IRPTDailyDataContract
    {
        /// <summary>
        /// 组件日运营数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult<DataSet> Get(ref RPTDailyDataGetParameter p);
    }
}
