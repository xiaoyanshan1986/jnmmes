using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace WinClient.VTestDataTransfer
{
    static class Program
    {
        public static System.Threading.Mutex Run;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool noRun = false;
            Run = new System.Threading.Mutex(true, "WinClient.VTestDataTransfer", out noRun);

            if (noRun)
            {
                Run.ReleaseMutex();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());

                Application.Run(new FrmIVDataUpload());
            }
            else
            {
                MessageBox.Show("程序已运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
        }
    }
}
