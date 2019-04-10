namespace ServiceCenter.Client.WinService.ImageDataTransfer
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
            this.siImageDataTransfer = new System.ServiceProcess.ServiceInstaller();
            // 
            // spiInstaller
            // 
            this.spiInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.spiInstaller.Password = null;
            this.spiInstaller.Username = null;
            // 
            // siImageDataTransfer
            // 
            this.siImageDataTransfer.Description = "MES 图片传输服务，用于将指定设备产生的图片上传到 MES 服务器。";
            this.siImageDataTransfer.DisplayName = "MES.ImageDataTransfer";
            this.siImageDataTransfer.ServiceName = "MES.ImageDataTransfer";
            this.siImageDataTransfer.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.spiInstaller,
            this.siImageDataTransfer});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller spiInstaller;
        private System.ServiceProcess.ServiceInstaller siImageDataTransfer;
    }
}