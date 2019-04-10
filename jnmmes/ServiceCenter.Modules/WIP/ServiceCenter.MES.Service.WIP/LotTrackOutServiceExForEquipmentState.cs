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
using ServiceCenter.MES.DataAccess.Interface.EMS;

namespace ServiceCenter.MES.Service.WIP
{
    
    public partial class LotTrackOutService
    {
        /// <summary>
        /// 设备数据访问类。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 设备状态数据访问类。
        /// </summary>
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
        
       


    }
}
