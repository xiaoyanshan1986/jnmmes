using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.Model;
using System;
using System.Data;
using System.ServiceModel;

namespace ServiceCenter.MES.Service.Client.RPT
{
    public class QMSemiProductionServiceClient : ClientBase<IQMSemiProductionContract>, IQMSemiProductionContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        public QMSemiProductionServiceClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public QMSemiProductionServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public QMSemiProductionServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public QMSemiProductionServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public QMSemiProductionServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public MethodReturnResult<DataSet> GetBaseDataForIVTest(string type)
        {
            return base.Channel.GetBaseDataForIVTest(type);
        }
        public MethodReturnResult<DataSet> GetSemiProdQtyForLine(QMSemiProductionGetParameter p)
        {
            return base.Channel.GetSemiProdQtyForLine(p);
        }

        public MethodReturnResult<DataSet> GetSemiProdQtyForLocation(QMSemiProductionGetParameter p)
        {
            return base.Channel.GetSemiProdQtyForLocation(p);
        }

        public MethodReturnResult<DataSet> GetQtyForDefective(QMSemiProductionGetParameter p)
        {
            return base.Channel.GetQtyForDefective(p);
        }

        public MethodReturnResult<DataSet> GetQtyForDefectPOS(DefectPOSGetParameter p)
        {
            return base.Channel.GetQtyForDefectPOS(p);
        }
        public MethodReturnResult<DataSet> GetQtyForDefectReason(DefectPOSGetParameter p)
        {
            return base.Channel.GetQtyForDefectReason(p);
        }

        public MethodReturnResult<DataSet> GetEquipmentDailyMoveForOEE(string EquipmentNo, string curDate)
        {
            return base.Channel.GetEquipmentDailyMoveForOEE(EquipmentNo, curDate);
        }
    }
}
