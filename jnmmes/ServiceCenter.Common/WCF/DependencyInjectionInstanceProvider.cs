using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace ServiceCenter.WCF
{
    /// <summary>
    /// Dependency injection instance provider
    /// </summary>
    /// <remarks>
    /// This kind of behavior controls the lifecycle of a WCF service instance, 
    /// so it is the best place to inject the service dependencies.
    /// </remarks>
    public class DependencyInjectionInstanceProvider : IInstanceProvider
    {
        private Type serviceType;
        private string _configFile = string.Empty;
        private string _containerName = string.Empty;
        private static readonly object syncRoot = new object();
        private static Dictionary<string, IUnityContainer> containers = new Dictionary<string, IUnityContainer>();
        private IUnityContainer currentContainer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceType">Service implamantation type</param>
        /// <param name="configFile">Dependency config file</param>
        /// <param name="containerName">dependecy container name</param>
        public DependencyInjectionInstanceProvider(Type serviceType, string configFile, string containerName)
        {
            this.serviceType = serviceType;
            this._configFile = configFile;
            this._containerName = containerName;
            InitContainer(this._configFile, this._containerName);
        }

        /// <summary>
        /// Returns a unique dependency key
        /// </summary>
        /// <param name="dependencyFile">Dependency config file</param>
        /// <param name="containerName">Dependency container name</param>
        /// <returns>unique dependency key </returns>
        private string GetDependencyKey(string dependencyFile, string containerName)
        {
            return "[" + dependencyFile + "]" + "[" + containerName + "]";
        }

        /// <summary>
        /// init dependency container
        /// </summary>
        /// <param name="dependencyFile">Dependency config file</param>
        /// <param name="containerName">Dependency conainer name</param>
        private void InitContainer(string dependencyFile, string containerName)
        {
            string currentContainerKey = GetDependencyKey(dependencyFile, containerName);
            if (!containers.ContainsKey(currentContainerKey))
            {
                lock (syncRoot)
                {
                    if (!containers.ContainsKey(currentContainerKey))
                    {
                        containers[currentContainerKey] = currentContainer = new UnityContainer();
                        UnityConfigurationSection section=null;
                        if (!string.IsNullOrEmpty(dependencyFile))
                        {
                            string dependencyFilePath = dependencyFile;
                            if (!System.IO.Path.IsPathRooted(dependencyFile))
                            {
                                dependencyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dependencyFile);
                            }
                            if (File.Exists(dependencyFilePath))
                            {
                                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                                map.ExeConfigFilename = dependencyFilePath;
                                System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                                section = (UnityConfigurationSection)config.GetSection("unity");
                            }
                        }
                        else
                        {
                            section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                        }
                        if (section != null)
                        {
                            section.Configure(currentContainer, containerName);
                        }
                    }
                }
            }
            else
            {
                currentContainer = containers[currentContainerKey];
            }
        }

        /// <summary>
        /// Gets a fresh service instance
        /// </summary>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        /// <summary>
        /// Gets a fresh service instance
        /// </summary>
        /// <param name="instanceContext"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            if (currentContainer == null)
            {
                string currentContainerKey = GetDependencyKey(this._configFile, this._containerName);
                if (containers.ContainsKey(currentContainerKey))
                {
                    containers.Remove(currentContainerKey);
                }
                InitContainer(this._configFile, this._containerName);
            }
            return currentContainer.Resolve(serviceType);
        }

        /// <summary>
        /// Releases the specified service instance
        /// </summary>
        /// <param name="instanceContext"></param>
        /// <param name="instance"></param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }
    }
}
