using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SanNiuSignal;
using SanNiuSignal.PublicTool;
namespace WinClient.Socket.ReaderServer
{
    class ReceiveLotNumber
    {
        private string _packageNo = "";
        private string _TotalNo = "";
        private string _ip = "";

        public event TxDelegate<string, string> ReceiveLotNumberEvent;
        public ReceiveLotNumber(string packageNo, string ip, string totalNo)
        {
            _packageNo = packageNo;
            _ip = ip;
            _TotalNo = totalNo;
        }

        public void receiveLotNumber()
        {
            if (ReceiveLotNumberEvent != null)
            {
                CommonMethod.eventInvoket(() => { ReceiveLotNumberEvent(_packageNo, _ip); });
            }
        }
    }
}
