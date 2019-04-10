using System;
using System.Threading;
using WinClient.Socket.ReaderServer.Configuration;
using WinClient.Socket.ReaderServer;

namespace WinClient.Socket.ReaderServer.ThreadWrapper
{
    public class LotReaderThreadWrapper : IDisposable
    {
        /// <summary>
        /// 获取线程执行的循环标志。
        /// </summary>
        public bool Loop { get; private set; }
        /// <summary>
        /// 获取线程执行异步事件。
        /// </summary>
        public AutoResetEvent AutoResetEvent { get; private set; }
        /// <summary>
        /// 获取线程对象。
        /// </summary>
        public Thread Thread { get; private set; }
        /// <summary>
        /// 获取EL/IV图片设备对象。
        /// </summary>
        public LotReaderDeviceElement Lotinfo { get; private set; }
        /// <summary>
        /// 构造函数。
        /// </summary>
        public LotReaderThreadWrapper(LotReaderDeviceElement lotinfo, ParameterizedThreadStart threadStart)
        {
            this.Loop = true;
            this.Lotinfo = lotinfo;
            this.AutoResetEvent = new AutoResetEvent(false);
            this.Thread = new Thread(threadStart);
        }
        /// <summary>
        /// 启动线程。
        /// </summary>
        public void Start()
        {
            this.Thread.Start(this);
        }
        /// <summary>
        /// 停止线程。
        /// </summary>
        public void Stop()
        {
            this.Loop = false;
            if (!this.AutoResetEvent.WaitOne(100))
            {
                this.Thread.Abort();
            }
            this.Thread = null;
        }
        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            this.AutoResetEvent.Close();
            this.AutoResetEvent = null;
            this.Thread = null;
        }
    }
}
