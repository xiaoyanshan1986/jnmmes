using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinClient.ELImageTransfer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string useName = txtName.Text.Trim();
            List<string> listName = new List<string>()
            {
                "k0100177",
                "k0100100",
                "k0102955",
                "k0102645",
                "k0100252",
                "k0100269"
            };
            bool login = false;
            if (listName.Contains(useName))
	        {
		          login = true;
	        };
            if (!login)
            {
                MessageBox.Show("非法用户，请更换用户名！");
                return;
            }
            if (txtPwd.Text.Trim().Length<=0)
            {
                MessageBox.Show("请输入密码！");
                return;
            }
            try
            {
                string strSql = string.Format("   SELECT USER_NAME FROM dbo.RBAC_USER WHERE LOGIN_NAME='{0}' AND PASSWORD='{1}'"
                                                    , txtName.Text.Trim()
                                                    , txtPwd.Text.Trim());
                DataTable dt = SqlHelper.ExecuteDataTable(strSql, CommandType.Text);
                if (dt.Rows.Count > 0)
                {

                    FrmELImageUpload mainform = new FrmELImageUpload();
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
    }
}
