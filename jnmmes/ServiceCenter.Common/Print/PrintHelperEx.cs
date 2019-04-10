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
using LabelManager2;

namespace ServiceCenter.Common.Print
{
  
    /// <summary>
    /// 打印帮助工厂类。
    /// </summary>
    public sealed class PrintHelperFactoryEx
    {
        public static Dictionary<string, IPrintHelperEx> DicPrintDocuments = new Dictionary<string, IPrintHelperEx>();
        private static  Object thisLock = new Object();
        public static IPrintHelperEx CreatePrintHelper(string printName,string printPort,string content)
        {
            IPrintHelperEx printHelper = null;
            //string strKey = printName + content;
            string strKey = printName;
            string strTemplatePath = content.Replace("codesoft://", string.Empty);
            string path = content.Replace("codesoft://", string.Empty);
            if (DicPrintDocuments.ContainsKey(strKey)==false)
            {
                LabelManager2.IApplication lbl = null;
                LabelManager2.IDocument doc = null;
                try
                {
                    lbl = new LabelManager2.ApplicationClass();                    
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

                    string defaultPrinterName = doc.Printer.Name;
                    //defaultPrinterName = printName;

                    if (defaultPrinterName != printName && doc.Printer.SwitchTo(printName, printPort, false) == false)
                    {
                        LabelManager2.Strings vars = lbl.PrinterSystem().Printers(LabelManager2.enumKindOfPrinters.lppxAllPrinters);
                        string printerNames = string.Empty;
                        for (int i = 1; i <= vars.Count; i++)
                        {
                            printerNames += vars.Item(i) + ";";
                        }
                        throw new Exception(string.Format("打印机({0})不存在。系统中存在的打印机列表 {1}。", printName, printerNames));
                    }

                    printHelper = new CodeSoftPrintHelperEx(strKey, doc,lbl);
                    printHelper.PrintTemplatePath = strTemplatePath;
                    if (doc != null)
                    {
                        lock (thisLock)
                        {
                            DicPrintDocuments.Add(strKey, printHelper);
                        }
                    }
                }
                finally
                {

                }
            }
            else
            {
                printHelper = DicPrintDocuments[strKey];
                if (printHelper.PrintTemplatePath != strTemplatePath)
                {
                    IDocument doc = printHelper.PrintApplication.Documents.Open(path);
                    printHelper.PrintDocument = doc;
                    printHelper.PrintTemplatePath = strTemplatePath;
                    lock (thisLock)
                    {
                        DicPrintDocuments[strKey] = printHelper;
                    }
                }
            }
            return printHelper;
        }

        public static void RemovePrintHelper(string key)
        {
            if(DicPrintDocuments.ContainsKey(key))
            {
                IPrintHelperEx printHelper = null;
                printHelper = DicPrintDocuments[key];
                lock (thisLock)
                {
                    DicPrintDocuments.Remove(key);
                }
                printHelper.Dispose();
            }

        }

        public static IPrintHelper CreatePrintHelper(string content)
        {
            if (content.StartsWith("codesoft://"))
            {
                return new CodesoftPrintHelper();
            }
            else
            {
                return new PrintHelper();
            }
        }
    }
    
}
