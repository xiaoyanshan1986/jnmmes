using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.COMMON
{
    /// <summary>
    /// 文件上传服务类。
    /// </summary>
    [ServiceContract]
    public interface IFileUploadContract
    {
        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <returns>相对文件路径。</returns>
        [OperationContract]
        MethodReturnResult<string> Upload(string fileName, Byte[] fileContent);
    }
}
