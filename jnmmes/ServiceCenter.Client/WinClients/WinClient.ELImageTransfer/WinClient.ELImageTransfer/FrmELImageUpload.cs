using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinClient.ELImageTransfer.Configuration;

namespace WinClient.ELImageTransfer
{

    public partial class FrmELImageUpload : Form
    {
        public string username;
        ELImageConfigurationSection _section = null;
        ELImageDeviceElement _deviceElement = null;

        string strIP = "";
        string strAccessConnection = "";
        private ELImageTransferAction dataTransferAction = null;
        public FrmELImageUpload()
        {
            InitializeComponent();
        }

        private void FrmELImageUpload_Load(object sender, EventArgs e)
        {
            label4.Text = username;
            try
            {
                bool blFound = false;
                string hostInfo = Dns.GetHostName();
                System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                //IPAddress ipaddress = addressList[0];
                //string ips = ipaddress.ToString();
                //MessageBox.Show(ips);


                //获取配置节信息
                this._section = (ELImageConfigurationSection)ConfigurationManager.GetSection("mes.ELImage");
                //增加线程个数。
                foreach (ELImageDeviceElement element in this._section.Devices)
                {
                    foreach (IPAddress ipAddress in addressList)
                    {
                        strIP = ipAddress.ToString();
                        if (strIP == element.Name)
                        {
                            blFound = true;
                            _deviceElement = element;
                            break;
                        }
                    }
                }
                if (blFound == false)
                {
                    MessageBox.Show("此电脑的IP地址没有在配置文件中找到对应的配置,请联系IT");
                    Application.Exit();
                }
                else
                {
                    txtSourceImagePath.Text = _deviceElement.SourceImagePathRoot;
                }
            }
            catch (Exception ex)
            {
                
                //throw;
            }

        }

        private void btnImageUpload_Click(object sender, EventArgs e)
        {
            string strLotNumber = txtLotNumber.Text.Trim();
            if (!string.IsNullOrEmpty(strLotNumber))
            {
                string strImageFullPath = CommonFun.GetFullPath(txtSourceImagePath.Text.Trim(), _deviceElement.SourceImagePathFormat);
                string strFileName = strImageFullPath + strLotNumber + "." + _deviceElement.ImageExtensionName;
                string strResult = CommonFun.UploadFile(strFileName, _deviceElement);
                txtLotNumber.Text = "";
                richTextBox1.Text = strResult + "\n" + richTextBox1.Text;
            }
            else
            {
                MessageBox.Show("批次号不能为空！");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //OpenFileDialog fileDialog = new OpenFileDialog();
            //fileDialog.Multiselect = true;
            //fileDialog.Title = "请选择文件";
            //fileDialog.Filter = "所有文件(*.*)|*.*";
            //if (fileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    string file = fileDialog.FileName;
            //    txtSourceImagePath.Text = file;
            //    MessageBox.Show("已选择文件:" + file, "选择文件提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                //MessageBox.Show("已选择文件夹:" + foldPath, "选择文件夹提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSourceImagePath.Text = foldPath + "\\";
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

        }


        private void FrmELImageUpload_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            if (MessageBox.Show("您确定要退出登录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
