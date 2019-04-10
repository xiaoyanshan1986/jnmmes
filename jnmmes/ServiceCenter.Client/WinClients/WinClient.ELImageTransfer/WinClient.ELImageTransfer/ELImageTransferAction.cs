using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinClient.ELImageTransfer.Configuration;

namespace WinClient.ELImageTransfer
{
    class ELImageTransferAction
    {
        private string strImageFullPath = "";
        public void Execute(ELImageDeviceElement element, String strLot)
        {
            strImageFullPath = CommonFun.GetFullPath(element.SourceImagePathRoot, element.SourceImagePathFormat);
            string strFileName = strImageFullPath + strLot + "." + element.ImageExtensionName;
            FileInfo fInfo = new FileInfo(strFileName);
            FtpManager.UploadFile(fInfo, element.FtpTargetFolder, element.FtpServer, element.FtpUser, element.FtpPassword);

        }
    }
}
