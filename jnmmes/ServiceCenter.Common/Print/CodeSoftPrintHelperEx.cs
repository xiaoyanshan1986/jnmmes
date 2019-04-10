using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabelManager2;

namespace ServiceCenter.Common.Print
{

    class CodeSoftPrintHelperEx : IPrintHelperEx
    {
        private string strKey = "";
        private string strPrintTempaltePath = "";
        private IDocument objPrintDocument;
        private IApplication objPrintApplication;
        private int nPrintQuantity = 1;

        public CodeSoftPrintHelperEx(string key,IDocument printDocument)
        {
            strKey = key;
            objPrintDocument = printDocument;
        }

        public CodeSoftPrintHelperEx(string key, IDocument printDocument,IApplication printApplication)
        {
            strKey = key;
            objPrintDocument = printDocument;
            objPrintApplication = printApplication;
        }
        public string Key
        {
            get
            {
                return strKey;
            }
            set
            {
                strKey = value;
            }
        }

         public string PrintTemplatePath
        {
            get
            {
                return strPrintTempaltePath;
            }
            set
            {
                strPrintTempaltePath = value;
            }
        }

        public IApplication PrintApplication
        {
            get
            {
                return objPrintApplication;
            }
            set
            {
                objPrintApplication = value;
            }
        }

        public IDocument PrintDocument
        {
            get
            {
                return objPrintDocument;
            }
            set
            {
                objPrintDocument = value;
            }
        }

        public int PrintQuantity
        {
            get
            {
                return nPrintQuantity;
            }
            set
            {
                nPrintQuantity = value;
            }
        }


        public bool Print( ExpandoObject obj)
        {
            var printQty = this.nPrintQuantity;
            IDocument doc = this.PrintDocument;
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
            doc.PrintDocument(printQty);
            return true;
        }

        public void Dispose()
        {
            try {
              
                if(objPrintDocument!=null)
                {
                    objPrintDocument.FormFeed();
                    objPrintDocument.Close();
                    objPrintDocument = null;
                }
                if (objPrintApplication != null)
                {
                    objPrintApplication.Quit();
                    objPrintApplication = null;
                }
            }
            catch
            {

           
            }
            finally
            {
                if (objPrintApplication != null)
                {
                    objPrintApplication.Quit();
                    objPrintApplication = null;
                }
            }
        }
    }


    /// <summary>
    /// 打印帮助接口。
    /// </summary>
    public interface IPrintHelperEx : IDisposable
    {
        string Key
        {
            get;
            set;
        }

        int PrintQuantity
        {
            get;
            set;
        }

        string  PrintTemplatePath
        {
            get;
            set;
        }

        IDocument PrintDocument
        {
            get;
            set;
        }

        IApplication PrintApplication
        {
            get;
            set;
        }

        bool Print(ExpandoObject obj);
    }
}
