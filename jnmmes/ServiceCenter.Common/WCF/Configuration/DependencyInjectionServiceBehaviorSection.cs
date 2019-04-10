using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;

namespace ServiceCenter.WCF.Configuration
{
    /// <summary>
    /// Configuration section for the DIServiceBehavior. 
    /// </summary>
    public class DependencyInjectionServiceBehaviorSection : BehaviorExtensionElement
    {
        public DependencyInjectionServiceBehaviorSection()
            : base()
        {
        }

        /// <summary>
        /// Dependency config file local path
        /// </summary>
        [ConfigurationProperty("dependencyConfigFile")]
        public string DependencyConfigFile
        {
            get
            {
                return (string)base["dependencyConfigFile"];
            }
            set
            {
                base["dependencyConfigFile"] = value;
            }
        }

        /// <summary>
        /// Dependency container name
        /// </summary>
        [ConfigurationProperty("containerName")]
        public string ContainerName
        {
            get
            {
                return (string)base["containerName"];
            }
            set
            {
                base["containerName"] = value;
            }
        }

        /// <summary>
        /// Dependency injection service behavior
        /// </summary>
        public override Type BehaviorType
        {
            get { return typeof(DependencyInjectionServiceBehavior); }
        }

        /// <summary>
        /// Create new dependency injection service behavior
        /// </summary>
        /// <returns>new instance of dependency injection service behavior</returns>
        protected override object CreateBehavior()
        {
            return new DependencyInjectionServiceBehavior(this.DependencyConfigFile, this.ContainerName);
        }
    }
}
