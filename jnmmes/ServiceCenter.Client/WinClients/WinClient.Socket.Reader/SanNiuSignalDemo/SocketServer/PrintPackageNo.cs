using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SanNiuSignal;
using SanNiuSignal.PublicTool;
namespace SocketServer
{
    class PrintPackageNo
    {
        private string _packageNo = "";
        private string  _TotalNo="";

        public event TxDelegate<string> PrintPackageEvent;
        public PrintPackageNo( string packageNo ,string totalNo)
        {
            _packageNo = packageNo;
            _TotalNo = totalNo;
        }

        public void printPackage()
        {
            if (PrintPackageEvent != null)
            {
                CommonMethod.eventInvoket(() => { PrintPackageEvent(_packageNo); });      
            }
        }
    }
}
