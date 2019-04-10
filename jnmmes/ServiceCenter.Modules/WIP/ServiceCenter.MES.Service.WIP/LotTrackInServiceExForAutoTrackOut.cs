using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using ServiceCenter.Common;
using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.Model.EMS;

namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 扩展批次进站，批次进站后自动出站。
    /// </summary>
    public partial class LotTrackInService
    {
        /// <summary>
        /// 批次定时作业数据访问对象。
        /// </summary>
        public ILotJobDataEngine LotJobDataEngine { get; set; }
        /// <summary>
        /// 批次数据访问对象。
        /// </summary>
        
        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 设备数据访问对象。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine { get; set; }

        /// <summary>
        /// 工步数据访问对象。
        /// </summary>
        public IRouteStepDataEngine RouteStepDataEngine { get; set; }

       
    }
}
