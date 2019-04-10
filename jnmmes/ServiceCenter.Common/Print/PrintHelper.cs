using ServiceCenter.Common.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiceCenter.Common.Print
{
    /// <summary>
    /// 条形码帮助类。
    /// </summary>
    public sealed class BarCodeHelper
    {
        /// <summary>
        /// 获取CODE39规则的字符串。
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static string GetCode39String(string inputData)
        {
            return string.Format("*{0}*", inputData);
        }
        /// <summary>
        /// 获取CODE128规则的字符串。
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static string GetCode128String(string inputData)
        {
            //Ì CODE 128B 起始位
            //Î CODE 128B 结束位
            //计算校验位
            string result;
            int checksum = 104;
            for (int ii = 0; ii < inputData.Length; ii++)
            {
                if (inputData[ii] >= 32)
                {
                    checksum += (inputData[ii] - 32) * (ii + 1);
                }
                else
                {
                    checksum += (inputData[ii] + 64) * (ii + 1);
                }
            }
            checksum = checksum % 103;
            if (checksum < 95)
            {
                checksum += 32;
            }
            else
            {
                checksum += 100;
            }

            result = Convert.ToChar(204) + inputData.ToString() + Convert.ToChar(checksum) + Convert.ToChar(206);
            return result;
        }
    }

    /// <summary>
    /// 打印帮助工厂类。
    /// </summary>
    public sealed class PrintHelperFactory
    {
        /// <summary>
        /// 创建打印帮助对象。
        /// </summary>
        /// <param name="content">打印内容。codesoft:// 开头表示为codesoft文件，使用Codesoft方式打印</param>
        /// <returns>打印帮助对象。</returns>
        public static IPrintHelper CreatePrintHelper(string content)
        {
            if (content.StartsWith("codesoft://"))
            {
                return new CodesoftPrintHelper();
            }
            else {
                return new PrintHelper();
            }
        }
    }
    /// <summary>
    /// 打印帮助接口。
    /// </summary>
    public interface IPrintHelper:IDisposable
    {
        /// <summary>
        /// 本地打印。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="template"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
       bool RAWPrint(string name, string template, ExpandoObject obj);
        /// <summary>
        /// 网络打印。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="port"></param>
        /// <param name="template"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
       bool NetworkPrint(string name, string port, string template, ExpandoObject obj);
    }

    /// <summary>
    /// 打印帮助类。
    /// </summary>
    public sealed class PrintHelper:IPrintHelper
    {
        /// <summary>
        /// 将待打印数据发送到网络打印机进行打印。
        /// </summary>
        /// <param name="ip">打印机名称/IP地址。</param>
        /// <param name="port">打印机端口。</param>
        /// <param name="template">打印内容字符串模版。</param>
        /// <param name="obj">替换模版内容的对象。 {#propertyName}。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public bool NetworkPrint(string name, string port, string template, ExpandoObject obj)
        {
            string content = GetContent(template, obj);
            var printQty = 1;
            if (obj != null)
            {
                KeyValuePair<string, object> printQtyPair = obj.FirstOrDefault(item => item.Key == "PrintQty");
                if (printQtyPair.Value != null)
                {
                    int.TryParse(Convert.ToString(printQtyPair.Value), out printQty);
                }
            }
            using (Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                IPAddress ipa = IPAddress.Parse(name);
                IPEndPoint ipe = new IPEndPoint(ipa, int.Parse(port));
                int count = 0;
                do
                {
                    try
                    {
                        soc.Connect(ipe);
                    }
                    catch (Exception ex)
                    {
                        count++;
                        if (count > 2 && soc.Connected == false)
                        {
                            throw ex;
                        }
                    }
                } while (soc.Connected == false);

                if (soc.Connected == true)
                {
                    byte[] b = System.Text.Encoding.Default.GetBytes(content);
                    while (printQty>0)
                    {
                        soc.Send(b);
                        printQty--;
                    }
                    soc.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 将待打印数据发送到网络打印机进行打印。
        /// </summary>
        /// <param name="ip">打印机IP地址。</param>
        /// <param name="port">打印机端口。</param>
        /// <param name="content">打印内容字符串。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public bool NetworkPrint(string name, string port, string content)
        {
            return NetworkPrint(name, port, content, null);
        }

        /// <summary>
        /// 将待打印数据发送到串口打印机进行打印。
        /// </summary>
        /// <param name="port">打印机端口。</param>
        /// <param name="template">打印内容字符串模版。</param>
        /// <param name="obj">替换模版内容的对象。 {#propertyName}。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public bool SerialPrint(string port, string template, ExpandoObject obj)
        {
            string content = GetContent(template, obj);
            var printQty = 1;
            if (obj != null)
            {
                KeyValuePair<string, object> printQtyPair = obj.FirstOrDefault(item => item.Key == "PrintQty");
                if (printQtyPair.Value != null)
                {
                    int.TryParse(Convert.ToString(printQtyPair.Value), out printQty);
                }
            }
            bool bSuccess = true;
            using (SerialPort comPort = new SerialPort(port))
            {
                if (!comPort.IsOpen)
                {
                    comPort.Open();
                }
                try
                {
                    while (printQty > 0)
                    {
                        comPort.WriteLine(content);
                        printQty--;
                    }
                }
                finally
                {
                    comPort.Close();
                }
            }
            return bSuccess;
        }

        /// <summary>
        /// 将待打印数据发送到串口打印机进行打印。
        /// </summary>
        /// <param name="port">打印机端口。</param>
        /// <param name="content">打印内容字符串。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public bool SerialPrint(string port, string content)
        {
            return SerialPrint(port, content);
        }

        /// <summary>
        /// 将待打印数据发送到并口打印机进行打印。
        /// </summary>
        /// <param name="port">打印机端口。</param>
        /// <param name="template">打印内容字符串模版。</param>
        /// <param name="obj">替换模版内容的对象。 {#propertyName}。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public bool ParallelPrint(string port, string template, ExpandoObject obj)
        {
            string content = GetContent(template, obj);
            var printQty = 1;
            if (obj != null)
            {
                KeyValuePair<string, object> printQtyPair = obj.FirstOrDefault(item => item.Key == "PrintQty");
                if (printQtyPair.Value != null)
                {
                    int.TryParse(Convert.ToString(printQtyPair.Value), out printQty);
                }
            }
            bool bSuccess = true;
            while (printQty > 0)
            {
                bSuccess = ParallelPrinterHelper.SendStringToPrinter(port, content);
                printQty--;
            }
            return bSuccess;
        }

        /// <summary>
        /// 将待打印数据发送到并口打印机进行打印。
        /// </summary>
        /// <param name="port">打印机端口。</param>
        /// <param name="content">打印内容字符串。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public bool ParallelPrint(string port, string content)
        {
            return ParallelPrint(port, content, null);
        }

        /// <summary>
        ///  将待打印数据发送到本地打印机进行打印。
        /// </summary>
        /// <param name="name">打印机名称。</param>
        /// <param name="template">打印内容字符串模版。</param>
        /// <param name="obj">替换模版内容的对象。 {#propertyName}。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public bool RAWPrint(string name, string template, ExpandoObject obj)
        {
            string content = GetContent(template, obj);
            var printQty = 1;
            if (obj != null)
            {
                KeyValuePair<string, object> printQtyPair = obj.FirstOrDefault(item => item.Key == "PrintQty");
                if (printQtyPair.Value != null)
                {
                    int.TryParse(Convert.ToString(printQtyPair.Value), out printQty);
                }
            }
            bool bSuccess = true;
            while (printQty > 0)
            {
                bSuccess = RAWPrinterHelper.SendStringToPrinter(name, content);
                printQty--;
            }
            return bSuccess;
        }
        /// <summary>
        ///  将待打印数据发送到本地打印机进行打印。
        /// </summary>
        /// <param name="name">打印机名称。</param>
        /// <param name="content">字符串。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public bool RAWPrint(string name, string content)
        {
            return RAWPrint(name, content,null);
        }

        public string GetContent(string template, ExpandoObject obj)
        {
            string content = template;
            if (obj == null)
            {
                return content;
            }
            Match m = Regex.Match(content, "({#([a-zA-Z0-9_]+)})");
            while (m.Success)
            {
                string name = m.Result("$2");
                string replaceName = m.Result("$1");
                KeyValuePair<string, object> pair = obj.FirstOrDefault(item => item.Key.ToUpper() == name.ToUpper());
                if (pair.Value != null)
                {
                    content = content.Replace(replaceName, Convert.ToString(pair.Value));
                }
                m = m.NextMatch();
            }
            return content;
        }
        
        void IDisposable.Dispose()
        {
        }
    }


    /// <summary>
    /// Codesoft模板打印帮助类。
    /// </summary>
    public sealed class CodesoftPrintHelper : IPrintHelper
    {
        public CodesoftPrintHelper()
        {
        }

        /// <summary>
        /// Codesoft模板打印。
        /// </summary>
        /// <param name="name">打印机名称。</param>
        /// <param name="templatePath">codesoft模板绝对路径。</param>
        /// <param name="obj">替换模版文件中的变量。</param>
        /// <returns></returns>
        public bool RAWPrint(string name, string templatePath, ExpandoObject obj)
        {
            return Print(name, string.Empty, false, templatePath, obj);
        }

        /// <summary>
        /// Codesoft模板打印。
        /// </summary>
        /// <param name="name">打印机名称/IP地址。。</param>
        /// <param name="port">打印机端口。</param>
        /// <param name="templatePath">codesoft模板绝对路径。</param>
        /// <param name="obj">替换模版文件中的变量。</param>
        /// <returns></returns>
        public bool NetworkPrint(string name, string port, string templatePath, ExpandoObject obj)
        {
            //string printerName = string.Format("{0}", name);
            //string printerPort = string.Format("->{0}:{1}", name, port);
            return Print(name, string.Empty, false, templatePath, obj);
        }
        /// <summary>
        /// Codesoft模板打印。
        /// </summary>
        /// <param name="name">打印机名称/IP地址。</param>
        /// <param name="port">打印机端口。</param>
        /// <param name="directAccess">直接访问打印机。</param>
        /// <param name="templatePath">codesoft模板绝对路径。</param>
        /// <param name="obj">替换模版文件中的变量。</param>
        /// <returns></returns>
        public bool Print(string name, string port, bool directAccess, string templatePath, ExpandoObject obj)
        {
            LabelManager2.IApplication lbl = null;
            LabelManager2.IDocument doc = null;
            try
            {

                lbl = new LabelManager2.ApplicationClass();
                string path = templatePath.Replace("codesoft://", string.Empty);
                if (doc == null)
                {
                    if (Path.IsPathRooted(path) == false)
                    {
                        string directoryName = System.AppDomain.CurrentDomain.BaseDirectory;
                        path = Path.GetFullPath(Path.Combine(directoryName, path));
                    }
                    lbl.Documents.Open(path);
                    if (lbl == null)
                    {
                        throw new Exception(string.Format("打开模板文件({0})失败。", path));
                    }
                    doc = lbl.ActiveDocument;
                }

                if (doc == null)
                {
                    throw new Exception(string.Format("打开模板文件({0})失败。", path));
                }

                var printQty = 1;
                if (obj != null)
                {
                    for (int i = 1; i <= doc.Variables.FormVariables.Count; i++)
                    {
                        var variableItem = doc.Variables.FormVariables.Item(i);
                        KeyValuePair<string, object> pair = obj.FirstOrDefault(item => item.Key.ToUpper() == variableItem.Name.ToUpper());
                        if (pair.Value != null)
                        {
                            variableItem.Value = Convert.ToString(pair.Value);
                        }
                    }
                    KeyValuePair<string, object> printQtyPair = obj.FirstOrDefault(item => item.Key.ToUpper() == "PrintQty".ToUpper());

                    if (printQtyPair.Value != null)
                    {
                        int.TryParse(Convert.ToString(printQtyPair.Value), out printQty);
                    }
                }

                string defaultPrinterName = doc.Printer.Name;

                if (defaultPrinterName != name && doc.Printer.SwitchTo(name, port, directAccess) == false)
                {
                    LabelManager2.Strings vars = lbl.PrinterSystem().Printers(LabelManager2.enumKindOfPrinters.lppxAllPrinters);
                    string printerNames = string.Empty;
                    for (int i = 1; i <= vars.Count; i++)
                    {
                        printerNames += vars.Item(i) + ";";
                    }
                    throw new Exception(string.Format("打印机({0})不存在。系统中存在的打印机列表 {1}。", name, printerNames));
                }

                doc.PrintDocument(printQty);
            }
            finally
            {
                //释放非托管资源
                if (doc != null)
                {
                    doc.FormFeed();
                    doc.Close();
                    doc = null;
                }

                if (lbl != null)
                {
                    lbl.Quit();
                    lbl = null;
                }
            }
            return true;
        }

        private bool isDispose = false;

        void IDisposable.Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected void Dispose(bool isDisposing)
        {
            if (!isDispose)
            {
                if (isDisposing)
                {
                    //释放托管资源
                }
            }
            isDispose = true;
        }

        ~CodesoftPrintHelper()
        {
            Dispose(false);
        }
    }

    /// <summary>
    /// 将将待打印数据发送到本地打印机的帮助类。
    /// </summary>
    internal sealed class RAWPrinterHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private class DOCINFO
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;

            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFO di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        /// <summary>
        /// 将字节发送给指定的打印机进行打印。
        /// </summary>
        /// <param name="szPrinterName">打印机名称。</param>
        /// <param name="pBytes">指向字节的指针。</param>
        /// <param name="dwCount">字节数量。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFO di = new DOCINFO();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }
        /// <summary>
        /// 将文件发送给指定的打印机进行打印。
        /// </summary>
        /// <param name="szPrinterName">打印机名称。</param>
        /// <param name="szFileName">文件名称。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }
        /// <summary>
        /// 将字符串发送给指定的打印机进行打印。
        /// </summary>
        /// <param name="szPrinterName">打印机名称。</param>
        /// <param name="szString">字符串。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            bool bSuccess = false;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return bSuccess;
        }
    }

    /// <summary>
    /// 将待打印数据发送到并口打印机的帮助类。
    /// </summary>
    internal sealed class ParallelPrinterHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct OVERLAPPED
        {
            int Internal;
            int InternalHigh;
            int Offset;
            int OffSetHigh;
            int hEvent;
        }

        [DllImport("kernel32.dll")]
        private static extern int CreateFile(string lpFileName,
                                             uint dwDesiredAccess,
                                             int dwShareMode,
                                             int lpSecurityAttributes,
                                             int dwCreationDisposition,
                                             int dwFlagsAndAttributes,
                                             int hTemplateFile);

        [DllImport("kernel32.dll")]
        private static extern bool WriteFile(int hFile,
                                             byte[] lpBuffer,
                                             int nNumberOfBytesToWriter,
                                             out int lpNumberOfBytesWriten,
                                             out OVERLAPPED lpOverLapped);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(int hObject);

        private static int iHandle;
        /// <summary>
        /// Open LPT port
        /// </summary>
        /// <returns>True or false</returns>
        /// <summary>
        /// Open LPT port
        /// </summary>
        /// <param name="printerPort">Printer port</param>
        /// <returns>Boolean value indication open port succeeded or failed</returns>
        private static bool LPTOpen(string printerPort)
        {
            iHandle = CreateFile(printerPort, 0x40000000, 0, 0, 3, 0, 0);
            if (iHandle != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Close LPT port
        /// </summary>
        /// <returns>True or false</returns>
        private static bool LPTClose()
        {
            return CloseHandle(iHandle);
        }

        /// <summary>
        /// 将字符串发送给指定的打印机进行打印。
        /// </summary>
        /// <param name="printerPort">打印机端口。</param>
        /// <param name="content">打印内容字符串。</param>
        /// <returns>是否打印成功，true：成功。false：失败。</returns>
        public static bool SendStringToPrinter(string printerPort, string content)
        {
            bool bSuccess = false;
            try
            {
                if (!LPTOpen(printerPort)) { return false; }
                if (iHandle != 1)
                {
                    int i;
                    OVERLAPPED x;
                    byte[] mybyte = Encoding.Default.GetBytes(content);
                    if (!WriteFile(iHandle, mybyte, mybyte.Length, out i, out x))
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                LPTClose();
            }
            return bSuccess;
        }
    }
}
