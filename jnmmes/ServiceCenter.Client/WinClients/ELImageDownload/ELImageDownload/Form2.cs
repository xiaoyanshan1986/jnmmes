using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ELImageDownload
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string strSql = string.Format("   SELECT USER_NAME FROM dbo.RBAC_USER WHERE LOGIN_NAME='{0}' AND PASSWORD='{1}'"
                                                    , txt_Name.Text.Trim()
                                                    , txt_Pwd.Text.Trim());
                DataTable dt = SqlHelper.ExecuteDataTable(strSql, CommandType.Text);
                if (dt.Rows.Count > 0)
                {

                    Form1 mainform = new Form1();
                    //UserInfo user = new UserInfo();
                    string name = dt.Rows[0][0].ToString();
                    mainform.username = name;
                    this.DialogResult = DialogResult.OK;
                    this.Hide();
                    this.Visible = false;
                     mainform.Show();
                }
                else
                {
                    MessageBox.Show("用户名或密码错误!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("点击按钮事件异常：/n" + ex.ToString());
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
