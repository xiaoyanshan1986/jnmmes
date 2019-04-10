using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Astronergy.ServiceCenter.Service.Contract;

namespace Astronergy.ServiceCenter.Service
{
    /// <summary>
    /// 文件上传类。
    /// </summary>
    public class FileUpload:IFileUpload
    {
        /// <summary>
        /// 上传文件存放的相对文件路径。
        /// </summary>
        public string RelativeFileUploadPath { get; set; }

        #region IFileUpload 成员
        string IFileUpload.Upload(string fileName, byte[] fileContent)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            string relativeFilePath = this.RelativeFileUploadPath;
            if (string.IsNullOrEmpty(relativeFilePath))
            {
                relativeFilePath = "upload";
            }
            string guidPath = Guid.NewGuid().ToString();
            relativeFilePath = Path.Combine(relativeFilePath,guidPath);

            string baseFilePath = AppDomain.CurrentDomain.BaseDirectory;
            string destFilePath = Path.Combine(baseFilePath, relativeFilePath);
            if (!Directory.Exists(destFilePath))
            {
                Directory.CreateDirectory(destFilePath);
            }
            string destFileName = Path.Combine(destFilePath, fileName);
            using (FileStream fs = new FileStream(destFileName, FileMode.CreateNew))
            {
                if (fileContent != null && fileContent.Length>0)
                {
                    fs.Write(fileContent, 0, fileContent.Length);
                }
            }
            return Path.Combine(relativeFilePath, fileName);
        }
        #endregion
    }
}
