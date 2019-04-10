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
using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.Model.EMS;


namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 扩展批次包装，进行批次进站操作。
    /// </summary>
    public partial class LotPackageService
    {

        /// <summary>
        /// 设备数据访问对象。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine 
        { 
            get; set; 
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
        /// 在批次包装时，进行批次进站操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public void ExecuteTrackIn(PackageParameter p)
        {            
            foreach(string lotNumber in p.LotNumbers)
            {
                
            }
            
        }

    }
}
