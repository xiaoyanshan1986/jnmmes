using ServiceCenter.MES.Model.SPC;
using ServiceCenter.MES.Service.Contract.SPC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Client.SPC
{
    public class SPCJobRulesServiceClient : ClientBase<ISPCJobRulesContract>, ISPCJobRulesContract, IDisposable
    {
        public SPCJobRulesServiceClient()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public SPCJobRulesServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public SPCJobRulesServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public SPCJobRulesServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public SPCJobRulesServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }
        public MethodReturnResult Add(SPCJobRules obj)
        {
            return base.Channel.Add(obj);
        }
        public MethodReturnResult Modify(SPCJobRules obj)
        {
            return base.Channel.Modify(obj);
        }

        public MethodReturnResult Delete(SPCJobRulesKey key)
        {
            return base.Channel.Delete(key);
        }
        public MethodReturnResult<SPCJobRules> Get(SPCJobRulesKey key)
        {
            return base.Channel.Get(key);
        }
        public MethodReturnResult<IList<SPCJobRules>> Get(ref PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
