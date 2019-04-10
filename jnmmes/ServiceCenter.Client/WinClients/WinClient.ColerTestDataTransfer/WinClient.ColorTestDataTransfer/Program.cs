using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Practices.EnterpriseLibrary.Data;
using log4net;

namespace WinClient.ColorTestDataTransfer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());
            LogHelper helper = new LogHelper();
            LogHelper.WriteLogInfo("START>Application");
            Application.Run(new FrmColorDataUpload());
           
        }
    }
}
