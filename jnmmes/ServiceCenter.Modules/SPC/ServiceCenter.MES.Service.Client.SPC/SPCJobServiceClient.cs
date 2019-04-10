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
    public class SPCJobServiceClient : ClientBase<ISPCJobContract>, ISPCJobContract, IDisposable
    {
        public SPCJobServiceClient()
        { 
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public SPCJobServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public SPCJobServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public SPCJobServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDisplayServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public SPCJobServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }
        public MethodReturnResult Add(SPCJob obj)
        {
            return base.Channel.Add(obj);
        }
        public MethodReturnResult Modify(SPCJob obj)
        {
            return base.Channel.Modify(obj);
        }

        public MethodReturnResult Delete(string key)
        {
            return base.Channel.Delete(key);
        }
        public MethodReturnResult<SPCJob> Get(string key)
        {
            return base.Channel.Get(key);
        }
        public MethodReturnResult<IList<SPCJob>> Get(ref PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }

        public MethodReturnResult<IList<SPCJobParam>> GetJobParams(string JobId)
        {
            return base.Channel.GetJobParams(JobId);
        }


        public MethodReturnResult AddJobParams(SPCJobParam obj)
        {
            return base.Channel.AddJobParams(obj);
        }
        public MethodReturnResult ModifyJobParams(SPCJobParam obj)
        {
            return base.Channel.ModifyJobParams(obj);
        }

        public MethodReturnResult DeleteJobParams(SPCJobParamKey key)
        {
            return base.Channel.DeleteJobParams(key);
        }

        public MethodReturnResult<SPCJobParam> GetSPCJobParam(SPCJobParamKey key)
        {
            return base.Channel.GetSPCJobParam(key);
        }
    }

}
