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
using ServiceCenter.Common.DataAccess.NHibernate;
using System.ServiceModel.Activation;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.EMS;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Model.PPM;
using System.Data;
using ServiceCenter.MES.DataAccess.Interface.BaseData;

namespace ServiceCenter.MES.Service.WIP
{
    
    public partial class LotBinServiceEx 
    {
        /// <summary>
        /// 设备数据访问对象。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine
        {
            get;
            set;
        }


        public IEquipmentStateDataEngine EquipmentStateDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 设备状态切换数据访问类。
        /// </summary>
        public IEquipmentChangeStateDataEngine EquipmentChangeStateDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 设备状态事件数据访问类。
        /// </summary>
        public IEquipmentStateEventDataEngine EquipmentStateEventDataEngine
        {
            get;
            set;
        }


        /// <summary>
        /// 工艺工步数据访问对象。
        /// </summary>
        public IRouteStepDataEngine RouteStepDataEngine
        {
            get;
            set;
        }

        public IRouteEnterpriseDetailDataEngine RouteEnterpriseDetailDataEngine
        {
            get;
            set;
        }

        public ILotTransactionEquipmentDataEngine LotTransactionEquipmentDataEngine
        {
            get;
            set;
        }

        public ILotTransactionStepDataEngine LotTransactionStepDataEngine
        {
            get;
            set;
        }

        public ILotTransactionCheckDataEngine LotTransactionCheckDataEngine
        {
            get;
            set;
        }

        public IBaseAttributeValueDataEngine BaseAttributeValueDataEngine
        {

            get;
            set;
        }

        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 批次IV测试数据访问类。
        /// </summary>
        public IIVTestDataDataEngine IVTestDataDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 包装数据访问类。
        /// </summary>
        public IPackageDataEngine PackageDataEngine { get; set; }
        /// <summary>
        /// 包装明细数据访问类。
        /// </summary>
        public IPackageDetailDataEngine PackageDetailDataEngine { get; set; }

        /// <summary>
        /// 工单等级包装规则数据访问类。
        /// </summary>
        public IWorkOrderGradeDataEngine WorkOrderGradeDataEngine
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
    }
}
