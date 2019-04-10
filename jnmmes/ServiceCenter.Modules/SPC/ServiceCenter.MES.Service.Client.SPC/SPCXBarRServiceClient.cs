using ServiceCenter.MES.Service.Contract.SPC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Client.SPC
{
    public class SPCXBarRServiceClient : ClientBase<ISPCXBarRContract>, ISPCXBarRContract, IDisposable
    {

        public SPCXBarRServiceClient()
        { }
         /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public SPCXBarRServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public SPCXBarRServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public SPCXBarRServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public SPCXBarRServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }


        public MethodReturnResult<DataSet> Get(SPCXBarRDataGetParameter p)
        {
            return base.Channel.Get(p);
        }
        public MethodReturnResult<DataSet> GetEquipment(string stepname)
        {
            return base.Channel.GetEquipment(stepname);
        }

        public MethodReturnResult<DataSet> GetJobDataCode(string codeType, string jobId, string stepName)
        {
            return base.Channel.GetJobDataCode(codeType,jobId,stepName);
        }
        public MethodReturnResult<DataSet> GetXBarData(SPCXBarRDataGetParameter p)
        {
            return base.Channel.GetXBarData(p);
        }

        public MethodReturnResult<DataSet> GetChartMonitorList(SPCChartMonitorQuery p)
        {
            return base.Channel.GetChartMonitorList(p);
        }
        public MethodReturnResult<DataSet> GetOriginalDataForExport(string testtime, string linecode, string eqpcode, string ParamterName)
        {
            return base.Channel.GetOriginalDataForExport(testtime, linecode, eqpcode, ParamterName);
        }

        public int UpdateDealNote(string testtime, string linecode, string eqpcode, string ParamterName, string Note)
        {
            return base.Channel.UpdateDealNote( testtime, linecode, eqpcode, ParamterName, Note);
        }
    }
}
