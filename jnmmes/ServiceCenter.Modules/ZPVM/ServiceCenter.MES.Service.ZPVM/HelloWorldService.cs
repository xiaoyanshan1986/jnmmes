using ServiceCenter.MES.Service.Contract.ZPVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.ZPVM
{
    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Allowed)]
    class HelloWorldService:IHelloWorldContract
    {
        public string Hello(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "Hello,World";
            }
            return "Hello," + name;
        }
    }
}
