using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Astronergy.ServiceCenter.Service.Contract
{
    /// <summary>
    /// 文件上传服务类。
    /// </summary>
    [ServiceContract]
    public interface IFileUpload
    {
        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <returns>相对文件路径。</returns>
        [OperationContract]
        string Upload(string fileName, Byte[] fileContent);
    }
}
