using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SanNiuSignal;
using System.Net;
using System.IO;
using SanNiuSignal.FileCenter;
using SocketClient;
using System.Threading;
namespace SocketServer
{
   

    public partial class Server : Form,IFileReceiveMust
    {
        #region TCPServer服务器
        private ITxServer server = null;

        private Thread threadHandlePackageNo;
            

        /// <summary>
        /// 当接收到来之客户端的文本信息的时候
        /// </summary>
        /// <param name="state"></param>
        /// <param name="str"></param>
        private void acceptString(IPEndPoint ipEndPoint, string str)
        {
            ListViewItem item = new ListViewItem(new string[] { DateTime.Now.ToString(), ipEndPoint.ToString(), str });
            this.listView1.Items.Insert(0, item);

            PrintPackageNo package = new SocketServer.PrintPackageNo(str, rTextBox.Text);
            package.PrintPackageEvent += new TxDelegate<string>(printPackage);

            threadHandlePackageNo = new Thread(package.printPackage);
            threadHandlePackageNo.Start();
            threadHandlePackageNo.IsBackground = true;
        }

        private void printPackage(string packageNo)
        {
            rTextBox.Text = packageNo + "\n" + rTextBox.Text ;
        }

        /// <summary>
        /// 当接收到来之客户端的图片信息的时候
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="bytes"></param>
        private void acceptBytes(IPEndPoint ipEndPoint, byte[] bytes)
        {
            MessageBox.Show(bytes.Length.ToString());
            this.pictureBox1.Image = objectByte.ReadImage(bytes);
        }
        /// <summary>
        /// 当有客户端连接上来的时候
        /// </summary>
        /// <param name="state"></param>
        private void connect(IPEndPoint ipEndPoint)
        {
            show(ipEndPoint, "上线");
        }
        /// <summary>
        /// 当对方已收到我方发送数据的时候
        /// </summary>
        /// <param name="state"></param>
        private void dateSuccess(IPEndPoint ipEndPoint)
        {
            textBox_msg.Text = "已向" + ipEndPoint.ToString() + "发送成功";
        }
        /// <summary>
        /// 当有客户端掉线的时候
        /// </summary>
        /// <param name="state"></param>
        /// <param name="str"></param>
        private void disconnection(IPEndPoint ipEndPoint, string str)
        {
            show(ipEndPoint, "下线");
        }
        /// <summary>
        /// 当服务器完全关闭的时候
        /// </summary>
        private void engineClose()
        {
            MessageBox.Show("服务器已关闭");
        }
        /// <summary>
        /// 当服务器非正常原因断开的时候
        /// </summary>
        /// <param name="str"></param>
        private void engineLost(string str)
        { MessageBox.Show(str); }
        private void button3_Click(object sender, EventArgs e)
        {
            // server.clientClose(server.StateAll[0]);
            MessageBox.Show(server.ClientAll[0].ToString());
        }
        /// <summary>
        /// 启动按钮Tcp服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                server = TxStart.startServer(int.Parse(textBox_port.Text));
                server.AcceptString += new TxDelegate<IPEndPoint, string>(acceptString);
                server.AcceptByte += new TxDelegate<IPEndPoint, byte[]>(acceptBytes);
                server.Connect += new TxDelegate<IPEndPoint>(connect);
                server.dateSuccess += new TxDelegate<IPEndPoint>(dateSuccess);
                server.Disconnection += new TxDelegate<IPEndPoint, string>(disconnection);
                server.EngineClose += new TxDelegate(engineClose);
                server.EngineLost += new TxDelegate<string>(engineLost);
                //server.BufferSize=12048;
                //server.FileLog = "C:\\test.txt";
                server.StartEngine();
                this.button1.Enabled = false;
                this.button2.Enabled = true;
                this.button3.Enabled = true;
            }
            catch (Exception Ex) { MessageBox.Show(Ex.Message); }
             
        }
        /// <summary>
        /// 发送按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                IPEndPoint client = (IPEndPoint)this.comboBox1.SelectedItem;
                if (client == null)
                {
                    MessageBox.Show("没有选中任何在线客户端！");
                    return;
                }

                if (!this.server.clientCheck(client))
                {
                    MessageBox.Show("目标客户端不在线！");
                    return;
                }
                server.sendMessage(client, textBox_msg.Text);
            }
            catch (Exception Ex) { MessageBox.Show(Ex.Message); }
        }
        /// <summary>
        /// 下面显示的
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="str"></param>
        private void show(IPEndPoint ipEndPoint, string str)
        {
            label_zt.Text = ipEndPoint.ToString() + ":" + str;
            label_all.Text = "当前在线人数:" + this.server.ClientNumber.ToString();
        }
        /// <summary>
        /// 当组合框按下的时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_DropDown_1(object sender, EventArgs e)
        {
            try
            {
                List<IPEndPoint> list = this.server.ClientAll;
                this.comboBox1.DataSource = list;
            }
            catch { }
        }
        /// <summary>
        /// 关闭客户端的按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click_1(object sender, EventArgs e)
        {
            IPEndPoint client = (IPEndPoint)this.comboBox1.SelectedItem;
            if (client == null)
            {
                MessageBox.Show("没有选中任何在线客户端！");
                return;
            }

            if (!this.server.clientCheck(client))
            {
                MessageBox.Show("目标客户端不在线！");
                return;
            }
            server.clientClose(client);
        }
        #endregion

        #region Udp引擎区
        private IUdpTx udptx = null;
        IPEndPoint UdpIPEndPoint = null;
        /// <summary>
        /// UDP启动按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            udptx = TxStart.startUdp();
            udptx.Port = int.Parse(textBox1.Text);
            udptx.AcceptString += new TxDelegate<IPEndPoint, string>(UdpAcceptString);
            udptx.dateSuccess += new TxDelegate<IPEndPoint>(UdpDateSuccess);
            udptx.EngineClose += new TxDelegate(engineClose);
            udptx.EngineLost += new TxDelegate<string>(engineLost);
            udptx.StartEngine();
            button6.Enabled = false;
        }
        /// <summary>
        /// 当接收到来之客户端的信息的时候
        /// </summary>
        /// <param name="state"></param>
        /// <param name="str"></param>
        private void UdpAcceptString(IPEndPoint ipEndPoint, string str)
        {
            UdpIPEndPoint = ipEndPoint;//udp用用的
            ListViewItem item = new ListViewItem(new string[] { DateTime.Now.ToString(), ipEndPoint.ToString(), str });
            this.listView1.Items.Insert(0, item);

            
        }


        /// <summary>
        /// 对方收到我的信息
        /// </summary>
        /// <param name="ipEndPoint"></param>
        private void UdpDateSuccess(IPEndPoint ipEndPoint)
        {
            textBox2.Text = "已向" + ipEndPoint.ToString() + "发送成功";
        }
        /// <summary>
        /// 发送UDP信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (UdpIPEndPoint == null)
            {
                MessageBox.Show("对方还没有发送UDP数据信息过来；找不到对方IPEndPoint"); return; 
            }
            udptx.sendMessage(UdpIPEndPoint, textBox2.Text);
        }
        #endregion

        #region 文件接收区
        private IFileReceive FileReceive = null;
        #region IFileReceiveMust 成员

        /// <summary>
        /// 文件已中断；状态已自动改为暂停状态；等待对方上线的时候；进行续传；
        /// </summary>
        /// <param name="FileLabel">文件标签</param>
        /// <param name="Reason">中断原因</param>
        public void FileBreak(int FileLabel, string Reason)
        {
            MessageBox.Show(Reason);
            button5.Text = "文件续传";
        }
        /// <summary>
        /// 对方已经取消这个文件的传输；我方已经关掉这个传输
        /// </summary>
        /// <param name="FileLabel">文件标签</param>
        public void FileCancel(int FileLabel)
        {
            button5.Text = "等待文件";
        }
        /// <summary>
        /// 文件开始续传；这时不会触发开始传输的方法
        /// </summary>
        /// <param name="FileLabel">文件标签</param>
        public void FileContinue(int FileLabel)
        {
            button5.Text = "暂停接收";
        }
        /// <summary>
        /// 文件传输失败
        /// </summary>
        /// <param name="FileLabel">文件标签</param>
        public void FileFailure(int FileLabel)
        {
            button5.Text = "等待文件";
        }
        /// <summary>
        /// 对方拒绝续传;文件又处于暂停状态；
        /// </summary>
        /// <param name="FileLabel">文件标签</param>
        public void FileNoContinue(int FileLabel)
        {
            button5.Text = "文件续传";
        }
        /// <summary>
        /// 对方发过来的续传确认信息；你是否同意续传；
        /// </summary>
        /// <param name="FileLabel">文件标签</param>
        /// <returns>同意或不同意</returns>
        public bool FileOrNotContingue(int FileLabel)
        {
            MessageBox.Show("是否续传这个文件");
            button5.Text = "暂停接收";
            return true;
        }
        /// <summary>
        /// 对方暂停；我方也已经暂停；等待着对方的再一次请求传输；会在FileOrNotContingue这里触发
        /// </summary>
        /// <param name="FileLabel">文件标签</param>
        public void FileStop(int FileLabel)
        {
            button5.Text = "文件续传";
        }
        /// <summary>
        /// 得到文件的进度;每次缓冲区为单位折算成百分比输出进度；这样可以提高效率；
        /// </summary>
        /// <param name="FileLabel">文件标签</param>
        /// <param name="Progress">进度</param>
        public void FileProgress(int FileLabel, int Progress)
        {
            label7.Text = Progress.ToString()+"%";
            progressBar1.Value = Progress;
        }
        /// <summary>
        /// 发送方发来个文件传输信息；你是否同意接收；如同意请直接回复需要保存的文件地址；如果不同意回复空文本
        /// </summary>
        /// <param name="FileLabel">文件标签</param>
        /// <param name="FileName">文件名称</param>
        /// <param name="FileLenth">文件长度</param>
        /// <returns>保存地址</returns>
        public string ReceiveOrNo(int FileLabel, string FileName, long FileLenth)
        {
            MessageBox.Show("有一个文件传输过来,文件名:" + FileName + "文件长度:" + FileLenth.ToString() + "个字节");
            string str = "C:\\" + "Receive" + FileName;
            textBox4.Text = str;
            label6.Text = FileLabel.ToString();
            button5.Text = "暂停接收";
            return str;
        }
        /// <summary>
        /// 文件接收成功
        /// </summary>
        /// <param name="FileLabel">文件标签</param>
        public void ReceiveSuccess(int FileLabel)
        {
            MessageBox.Show("接收完成");
            button5.Text = "等待文件";
        }
        #endregion
        /// <summary>
        /// 文件操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            switch (button5.Text)
            {
                case "暂停接收":
                    try
                    {
                        FileReceive.FileStop(int.Parse(label6.Text));
                        button5.Text = "文件续传";
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    break;
                case "文件续传":
                    try
                    {
                        if (checkBox1.Checked)
                            udptx.ContinueFile(UdpIPEndPoint, int.Parse(label6.Text));
                        else
                        {
                            IPEndPoint client = (IPEndPoint)this.comboBox1.SelectedItem;
                            if (client == null)
                            {
                                MessageBox.Show("没有选中任何在线客户端！");
                                return;
                            }

                            if (!this.server.clientCheck(client))
                            {
                                MessageBox.Show("目标客户端不在线！");
                                return;
                            }
                            server.ContinueFile(client, int.Parse(label6.Text));
                        }
                    }
                    catch (Exception Ex) { MessageBox.Show(Ex.Message); }
                    break;
            }
        }
        /// <summary>
        /// 取消接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                FileReceive.FileCancel(int.Parse(label6.Text));
                label7.Text = "0%";
                progressBar1.Value = 0;
                button5.Text = "等待文件";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        /// <summary>
        /// 启动文件接收系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                FileReceive = FileStart.StartFileReceive(this);
            }
            catch (Exception Ex) { MessageBox.Show(Ex.Message); return; }
            FileReceive.BufferSize = 191230;
            button8.Enabled = false;
        }
        #endregion

        public Server()
        { InitializeComponent(); }
    }
}
