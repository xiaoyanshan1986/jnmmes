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
using ServiceCenter.MES.Model.EMS;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.DataAccess.Interface.LSM;

namespace ServiceCenter.MES.Service.WIP
{
    public partial class WipEngineerService
    {
        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次自定义属性数据访问类。
        /// </summary>
        public ILotAttributeDataEngine LotAttributeDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 批次操作数据访问类。
        /// </summary>
        public ILotTransactionDataEngine LotTransactionDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次历史数据访问类。
        /// </summary>
        public ILotTransactionHistoryDataEngine LotTransactionHistoryDataEngine
        {
            get;
            set;
        }

        /// <summary>
        ///  批次附加参数数据访问类。
        /// </summary>
        public ILotTransactionParameterDataEngine LotTransactionParameterDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 生产线数据访问对象。
        /// </summary>
        public IProductionLineDataEngine ProductionLineDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 区域数据访问对象。
        /// </summary>
        public ILocationDataEngine LocationDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 批次加工设备数据访问对象。
        /// </summary>
        public ILotTransactionEquipmentDataEngine LotTransactionEquipmentDataEngine
        {
            get;
            set;
        }


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
        /// <summary>
        /// 产品控制对象规则数据访问类。
        /// </summary>
        public IProductControlObjectDataEngine ProductControlObjectDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工步参数数据访问类。
        /// </summary>
        public IRouteStepParameterDataEngine RouteStepParameterDataEngine
        {
            get;
            set;
        }

        public IMaterialReceiptDetailDataEngine MaterialReceiptDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 线上仓物料明细数据访问类。
        /// </summary>
        public ILineStoreMaterialDetailDataEngine LineStoreMaterialDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工单上料数据访问类。
        /// </summary>
        public IMaterialLoadingDetailDataEngine MaterialLoadingDetailDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单上料数据访问类。
        /// </summary>
        public IMaterialLoadingDataEngine MaterialLoadingDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单BOM数据访问类。
        /// </summary>
        public IWorkOrderBOMDataEngine WorkOrderBOMDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 产品BOM数据访问类。
        /// </summary>
        public IMaterialBOMDataEngine MaterialBOMDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次BOM数据访问类。
        /// </summary>
        public ILotBOMDataEngine LotBOMDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 供应商数据访问类。
        /// </summary>
        public ISupplierDataEngine SupplierDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 物料数据访问类。
        /// </summary>
        public IMaterialDataEngine MaterialDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工艺流程组明细数据访问对象。
        /// </summary>
        public IRouteEnterpriseDetailDataEngine RouteEnterpriseDetailDataEngine
        {
            get;
            set;
        }
     

        /// <summary>
        ///  批次下一工艺工步数据访问类。
        /// </summary>
        public ILotTransactionStepDataEngine LotTransactionStepDataEngine
        {
            get;
            set;
        }
       

        /// <summary>
        /// 批次检验数据访问对象。
        /// </summary>
        public ILotTransactionCheckDataEngine LotTransactionCheckDataEngine
        {
            get;
            set;
        }

        public IIVTestDataDataEngine IVTestDataDataEngine
        {
            get;
            set;
        }


        public ILotTransactionDefectDataEngine LotTransactionDefectDataEngine
        {
            get;
            set;
        }

        public ILotTransactionScrapDataEngine LotTransactionScrapDataEngine
        {
            get;
            set;
        }



        //校准板线别类
        public ICalibrationPlateLineDataEngine CalibrationPlateLineDataEngine
        {
            get;
            set;
        }

        /// <summary> 安规测试数据类 </summary>
        public IVIRTestDataDataEngine VIRTestDataDataEngine
        {
            get;
            set;
        }
    }
}
