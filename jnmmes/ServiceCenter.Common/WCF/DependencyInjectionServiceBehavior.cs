using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace ServiceCenter.WCF
{
    /// <summary>
    /// Dependency injection service behavior
    /// </summary>
    /// <remarks>
    /// This behavior is only used to hook up the instance provider in the WCF dispatcher at runtime.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DependencyInjectionServiceBehavior : Attribute, IServiceBehavior
    {
        private string configFile;
        private string containerName;

        /// <summary>
        /// 
        /// </summary>
        public DependencyInjectionServiceBehavior()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <param name="containerName"></param>
        public DependencyInjectionServiceBehavior(string configFile, string containerName)
            : base()
        {
            this.configFile = configFile;
            this.containerName = containerName;
        }

        /// <summary>
        /// Dependency instance provider applied all end points
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher cd = cdb as ChannelDispatcher;
                if (cd != null)
                {
                    foreach (EndpointDispatcher ed in cd.Endpoints)
                    {
                        ed.DispatchRuntime.InstanceProvider =
                            new DependencyInjectionInstanceProvider(serviceDescription.ServiceType, configFile, containerName);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        /// <param name="endpoints"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}
