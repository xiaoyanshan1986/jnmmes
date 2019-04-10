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
using ServiceCenter.MES.DataAccess.Interface.LSM;

namespace ServiceCenter.MES.Service.WIP
{
    
    public partial class LotTrackOutService
    {

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
        /// 工单BOM数据访问类。
        /// </summary>
        public IWorkOrderBOMDataEngine WorkOrderBOMDataEngine
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
    }
}
