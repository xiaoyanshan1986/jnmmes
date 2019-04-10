using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Client.RPT
{
   public class WIPIVTestServiceClient: ClientBase<IWIPIVTestContract>, IWIPIVTestContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        public WIPIVTestServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public WIPIVTestServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WIPIVTestServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WIPIVTestServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LotMaterialListServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WIPIVTestServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public MethodReturnResult<DataSet> GetBaseDataForIVTest(string type)
        {
            return base.Channel.GetBaseDataForIVTest(type);
        }
        public MethodReturnResult<DataSet> Get(WIPIVTestGetParameter p)
        {
            return base.Channel.Get( p);
        }

        public MethodReturnResult<DataTable> GetIVDataForJZ(WIPIVTestGetParameter p)
        {
            return base.Channel.GetIVDataForJZ(p);
        }

        public MethodReturnResult<DataSet> GetIVDataForCTM(WIPIVTestGetParameter p)
        {
            return base.Channel.GetIVDataForCTM(p);
        }

    }
}
