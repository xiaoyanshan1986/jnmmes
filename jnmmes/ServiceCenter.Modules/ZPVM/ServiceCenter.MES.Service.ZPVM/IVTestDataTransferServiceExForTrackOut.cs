using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using ServiceCenter.Common;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Contract.ZPVM;

namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 扩展IV测试数据，进行批次JOB数据记录，以自动出站。
    /// </summary>
    public partial class IVTestDataTransferService
    {
        
        /// <summary>
        /// 批次作业数据访问类。
        /// </summary>
        public ILotJobDataEngine LotJobDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 设备数据访问类。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工序设备数据访问类。
        /// </summary>
        public IRouteOperationEquipmentDataEngine RouteOperationEquipmentDataEngine 
        {
            get;
            set;
        }
        /// <summary>
        /// 区域数据访问类。
        /// </summary>
        public ILocationDataEngine LocationDataEngine
        {
            get;
            set;
        }
        
    }
}
