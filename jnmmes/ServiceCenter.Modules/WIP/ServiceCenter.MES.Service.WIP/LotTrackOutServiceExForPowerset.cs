using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.RBAC;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System.ServiceModel.Activation;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;

namespace ServiceCenter.MES.Service.WIP
{
    
    public partial class LotTrackOutService
    {
        public IIVTestDataDataEngine IVTestDataDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单规则数据访问类。
        /// </summary>
        public IWorkOrderRuleDataEngine WorkOrderRuleDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单产品数据访问类。
        /// </summary>
        public IWorkOrderProductDataEngine WorkOrderProductDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单分档规则数据访问类。
        /// </summary>
        public IWorkOrderPowersetDataEngine WorkOrderPowersetDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单分档明细规则数据访问类。
        /// </summary>
        public IWorkOrderPowersetDetailDataEngine WorkOrderPowersetDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工单衰减规则数据访问类。
        /// </summary>
        public IWorkOrderDecayDataEngine WorkOrderDecayDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 衰减数据访问类。
        /// </summary>
        public IDecayDataEngine DecayDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工单控制对象规则数据访问类。
        /// </summary>
        public IWorkOrderControlObjectDataEngine WorkOrderControlObjectDataEngine
        {
            get;
            set;
        }


        public bool CheckControlObject(string type, double value, double controlValue)
        {
            switch (type)
            {
                case ">":
                    return value > controlValue;
                case "<":
                    return value < controlValue;
                case "=":
                case "==":
                    return value == controlValue;
                case ">=":
                    return value >= controlValue;
                case "<=":
                    return value <= controlValue;
                case "<>":
                case "!=":
                    return value != controlValue;
                default:
                    break;
            }
            return false;
        }
    }
}
