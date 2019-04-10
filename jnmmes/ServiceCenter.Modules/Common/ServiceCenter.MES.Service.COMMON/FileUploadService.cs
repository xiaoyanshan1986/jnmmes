using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Service.Contract.COMMON;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Service.COMMON
{
    /// <summary>
    /// 文件上传类。
    /// </summary>
    public class FileUploadService : IFileUploadContract
    {
        /// <summary>
        /// 上传文件存放的相对文件路径。
        /// </summary>
        public string RelativeFileUploadPath { get; set; }

        #region IFileUpload 成员
        MethodReturnResult<string> IFileUploadContract.Upload(string fileName, byte[] fileContent)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>();
            if (string.IsNullOrEmpty(fileName))
            {
                result.Code = 1001;
                result.Message = "文件名为空。";
                return result;
            }

            try
            {
                string relativeFilePath = this.RelativeFileUploadPath;
                if (string.IsNullOrEmpty(relativeFilePath))
                {
                    relativeFilePath = "upload";
                }
                string guidPath = Guid.NewGuid().ToString();
                relativeFilePath = Path.Combine(relativeFilePath, guidPath);

                string baseFilePath = AppDomain.CurrentDomain.BaseDirectory;
                string destFilePath = Path.Combine(baseFilePath, relativeFilePath);
                if (!Directory.Exists(destFilePath))
                {
                    Directory.CreateDirectory(destFilePath);
                }
                string destFileName = Path.Combine(destFilePath, fileName);
                using (FileStream fs = new FileStream(destFileName, FileMode.CreateNew))
                {
                    if (fileContent != null && fileContent.Length > 0)
                    {
                        fs.Write(fileContent, 0, fileContent.Length);
                    }
                }

                result.Data = Path.Combine(relativeFilePath, fileName);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }
        #endregion
    }
}
