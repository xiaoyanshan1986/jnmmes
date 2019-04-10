using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.ZPVM
{
    [ServiceContract]
    public interface IHelloWorldContract
    {
        [OperationContract]
        string Hello(string name);
    }
}
