namespace ServiceCenter.Client.WinService.IVTestDataTransfer
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.spiInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.siIVTestDataTransfer = new System.ServiceProcess.ServiceInstaller();
            // 
            // spiInstaller
            // 
            this.spiInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.spiInstaller.Password = null;
            this.spiInstaller.Username = null;
            // 
            // siIVTestDataTransfer
            // 
            this.siIVTestDataTransfer.Description = "MES IV测试数据传输服务，用于将指定设备产生的IV测试数据上传到 MES 服务器。";
            this.siIVTestDataTransfer.DisplayName = "MES.IVTestDataTransfer";
            this.siIVTestDataTransfer.ServiceName = "MES.IVTestDataTransfer";
            this.siIVTestDataTransfer.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.spiInstaller,
            this.siIVTestDataTransfer});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller spiInstaller;
        private System.ServiceProcess.ServiceInstaller siIVTestDataTransfer;
    }
}