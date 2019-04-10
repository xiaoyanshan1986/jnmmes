using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SanNiuSignal;
using System.Net;
using System.Threading;
using System.Collections;
using WinClient.Socket.ReaderServer.ThreadWrapper;
using System.Configuration;
using WinClient.Socket.ReaderServer.Configuration;
using WinClient.Socket.ReaderServer;
using ServiceCenter.Client.WinService.ImageDataTransfer;
namespace WinClient.Socket.ReaderServer
{
    public partial class Server : Form
    {
        LotReaderDeviceElement _deviceElement = null;
        LotReaderConfigurationSection _section = null;
        IList<LotReaderThreadWrapper> lstWrapper = new List<LotReaderThreadWrapper>();
        private LotReaderAction lotReaderAction = new LotReaderAction();
        public Server()
        {
            InitializeComponent();
        }
        private void Server_Load(object sender, EventArgs e)
        {
            this._section = (LotReaderConfigurationSection)ConfigurationManager.GetSection("mes.reader");
            if (_section.Devices.Count > 0)
            {
                this._deviceElement = _section.Devices[0];
            }
        }
        #region TCPServer服务器
        private ITxServer server = null;

        private Thread threadHandlePackageNo;
        Queue<LotReaderInfo> queue = new Queue<LotReaderInfo>();

        /// <summary>
        /// 当接收到来之客户端的文本信息的时候
        /// </summary>
        /// <param name="state"></param>
        /// <param name="str"></param>
        private void acceptString(IPEndPoint ipEndPoint, string str)
        {
            string[] sp1 = str.Split('\\');//\r\n
            string lotNumber = str.Substring(1, str.Length - 3);
            ListViewItem item = new ListViewItem(new string[] { DateTime.Now.ToString(), ipEndPoint.ToString(),lotNumber});
            if (this.listInfo.Items.Count>100)
            {
                this.listInfo.Items.Clear();
            }
            this.listInfo.Items.Insert(0, item);

            ReceiveLotNumber receive = new WinClient.Socket.ReaderServer.ReceiveLotNumber(lotNumber, ipEndPoint.ToString().Split(':')[0], rTextBox.Text);
            receive.ReceiveLotNumberEvent += new TxDelegate<string, string>(ReceiveLotsInfo);

            threadHandlePackageNo = new Thread(receive.receiveLotNumber);
            threadHandlePackageNo.Start();
            threadHandlePackageNo.IsBackground = true;
        }

        private void ReceiveLotsInfo(string LotNumber, string ip)
        {
            LotReaderInfo item = new LotReaderInfo();
            foreach (LotReaderDeviceElement element in this._section.Devices)
            {
                if (ip == element.ReaderIP)
                {
                    item.ReaderIP = ip;
                    item.LotNumber = LotNumber;
                    item.LineCode = element.LineCode;
                    item.FirstStepCode = element.FirstStepCode;
                    item.SecondEquipmentCode = element.SecondEquipmentCode;
                    item.SecondStepCode = element.SecondStepCode;
                    item.FirstEquipmentCode = element.FirstEquipmentCode;
                    item.WorkShop = element.WorkShop;
                    item.WorkShopId = element.WorkShopId;
                    item.FlowId = element.FlowId;
                    item.FlowSubId = element.FlowSubId;
                }
            }
            queue.Enqueue(item);
            if (rTextBox.TextLength>200)
            {
                rTextBox.Text = "";
            }
            rTextBox.Text = ip + "---" + item.LotNumber + "\n" + rTextBox.Text;

        }

        /// <summary>
        /// 启动按钮Tcp服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBegin_Click(object sender, EventArgs e)
        {
            try
            {
                server = TxStart.startServer(int.Parse(textBox_port.Text));
                server.AcceptString += new TxDelegate<IPEndPoint, string>(acceptString);
                server.StartEngine();
                this.btnBegin.Enabled = false;
            }
            catch (Exception Ex) { MessageBox.Show(Ex.Message); }
        }
        private void btnDeal_Click(object sender, EventArgs e)
        {
            btnDeal.Enabled = false;
            if (lstWrapper == null)
            {
                lstWrapper = new List<LotReaderThreadWrapper>();
            }
            lotReaderAction.OnLotReaderFinished += new EventHandler<LotReaderFinishedArgs>(DataTransferFinished);
            //获取配置节信息
            this._section = (LotReaderConfigurationSection)ConfigurationManager.GetSection("mes.reader");
            //增加线程个数。

            foreach (LotReaderDeviceElement element in this._section.Devices)
            {
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(DealData);
                LotReaderThreadWrapper wrapper = new LotReaderThreadWrapper(element, threadStart);
                lstWrapper.Add(wrapper);
            }

            //启动线程。
            foreach (LotReaderThreadWrapper wrapper0 in lstWrapper)
            {
                //StartTransfer();
                Thread.Sleep(100);
                wrapper0.Start();
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }
        private void DealData(object obj)
        {
            try
            {
                LotReaderThreadWrapper wrapper = obj as LotReaderThreadWrapper;
                if (wrapper == null)
                {
                    return;
                }
                DealData(wrapper);
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 数据转置
        /// </summary>
        private void DealData(LotReaderThreadWrapper wrapper)
        {
            while (wrapper.Loop)
            {
                try
                {
                    if (queue != null && queue.Count > 0)
                    {
                        LotReaderDeviceElement lotinfo = new LotReaderDeviceElement();
                        lock (queue)
                        {
                            //获取读头配置信息
                            lotinfo.ReaderIP = queue.Peek().ReaderIP;
                            lotinfo.LineCode = queue.Peek().LineCode;
                            lotinfo.LotNumber = queue.Peek().LotNumber;
                            lotinfo.FirstStepCode = queue.Peek().FirstStepCode;
                            lotinfo.FirstEquipmentCode = queue.Peek().FirstEquipmentCode;
                            lotinfo.SecondStepCode = queue.Peek().SecondStepCode;
                            lotinfo.SecondEquipmentCode = queue.Peek().SecondEquipmentCode;
                            lotinfo.WorkShop = queue.Peek().WorkShop;
                            lotinfo.WorkShopId = queue.Peek().WorkShopId;
                            lotinfo.FlowId = queue.Peek().FlowId;
                            lotinfo.FlowSubId = queue.Peek().FlowSubId;
                            DateTime dtStartTime = DateTime.Now;

                            //执行过站操作
                            lotReaderAction.Execute(lotinfo);
                            //请除队列中执行的批次信息
                            queue.Dequeue();
                        }                        
                    }
                }
                catch (Exception ex)
                {
                }
                if (wrapper.Loop)
                {
                    Thread.Sleep(1000);
                }
            }
            wrapper.AutoResetEvent.Set();
        }
        void DataTransferFinished(object sender, LotReaderFinishedArgs e)
        {
            if (queue.Count > 0)
            {
                if (richTextBox1.TextLength > 2000)
                {
                    richTextBox1.Text = "";
                }
                richTextBox1.Text = queue.Peek().ReaderIP + "—'批号：" + queue.Peek().LotNumber + "'\r\n" + e.TransferMsg + "\r\n" + richTextBox1.Text;
            }
        }
        private void CloseForm()
        {
            if (MessageBox.Show("是否退出系统", "ReaderServer", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                CommonFun.eventInvoket(() =>
                {
                    try
                    {
                        foreach (LotReaderThreadWrapper wrapper in lstWrapper)
                        {
                            wrapper.Stop();
                            wrapper.Dispose();
                        }
                        lstWrapper.Clear();
                        lstWrapper = null;

                    }
                    catch (Exception ex)
                    {
                    }
                    lotReaderAction.OnLotReaderFinished -= new EventHandler<LotReaderFinishedArgs>(DataTransferFinished);
                });
                this.Close();
            }
        }
        protected override void WndProc(ref Message msg)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;

            if (msg.Msg == WM_SYSCOMMAND && ((int)msg.WParam == SC_CLOSE))
            {
                if (MessageBox.Show("是否退出", "ReaderServer", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CommonFun.eventInvoket(() =>
                    {
                        try
                        {
                            foreach (LotReaderThreadWrapper wrapper in lstWrapper)
                            {
                                wrapper.Stop();
                                wrapper.Dispose();
                            }
                            lstWrapper.Clear();
                            lstWrapper = null;

                        }
                        catch (Exception ex)
                        {
                        }
                        lotReaderAction.OnLotReaderFinished -= new EventHandler<LotReaderFinishedArgs>(DataTransferFinished);
                    });
                    this.Close();
                }
                return;
            }
            base.WndProc(ref msg);
        }
        #endregion
    }
}
