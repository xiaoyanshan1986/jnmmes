using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Socket1
{

    public partial class Form1 : Form
    {
        Socket socket = null;
        public Form1()
        {
            InitializeComponent();
        }
        private void btn_start_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress Ip = IPAddress.Parse(ipBox.Text);//将IP地址转换为实例，否则报错提示无法从sting型换成long型的                  
                IPEndPoint ipe = new IPEndPoint(Ip, Int32.Parse(port.Text));
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//建立新的套字节                 
                try
                {
                    socket.Connect(ipe);//连接到指定的服务器                 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("客户端：" + "连接失败！" + ex.Message);
                    return;
                }
                string sendmessage = SendMessage.Text;
                byte[] send = Encoding.UTF8.GetBytes(sendmessage);
                socket.Send(send, send.Length, 0);//发送消息 


                sendmessage ="";
                send = Encoding.UTF8.GetBytes(sendmessage);
                socket.Send(send, send.Length, 0);//发送消息 

                //SendMessage.Text = "";
                //string recv = "";
                //byte[] recvbytes = new byte[1024];
                //int bytes = socket.Receive(recvbytes, recvbytes.Length, 0);
                ////接收消息                  
                //recv += Encoding.UTF8.GetString(recvbytes, 0, bytes);
                ////转换为字符串    
                //SendMessage.Text = recv;
                //port.Text = recv;
                socket.Close();
                
            }
            catch
            {
                MessageBox.Show("连接出错！");
                btn_start.Enabled = true;
            }
        }
        private void btn_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
            btn_start.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(socket.Connected)
            { 
                string sendmessage = SendMessage.Text;
                byte[] send = Encoding.UTF8.GetBytes(sendmessage);
                socket.Send(send, send.Length, 0);//发送消息    
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.Visible = true;
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Show();
            WindowState = FormWindowState.Normal;
            this.Focus();
        }
    }
}
