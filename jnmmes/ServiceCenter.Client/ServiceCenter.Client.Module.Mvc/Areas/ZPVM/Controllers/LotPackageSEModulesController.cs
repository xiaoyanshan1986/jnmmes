using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Resources;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using System.IO;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Model.WIP;
using System.Xml;
using System.Net;
using ServiceCenter.MES.Service.Contract.WIP;
using System.Data;
using ServiceCenter.MES.Service.Contract.ERP;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class LotPackageSEModulesController : Controller
    {
        //
        // GET: /WIP/LotPackageQuery/
        public async Task<ActionResult> Index()
        {
            return await Query(new LotPackageSEModulesViewModel());
        }
        //
        //POST: /WIP/LotPackageQuery/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(LotPackageSEModulesViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = GetQueryCondition(model)
                        };
                        MethodReturnResult<IList<Package>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", new LotPackageSEModulesViewModel());
            }
            else
            {
                return View("Index", model);
            }
        }
        //
        //POST: /WIP/LotPackageQuery/PagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
        {
            if (ModelState.IsValid)
            {
                int pageNo = currentPageNo ?? 0;
                int pageSize = currentPageSize ?? 20;
                if (Request["PageNo"] != null)
                {
                    pageNo = Convert.ToInt32(Request["PageNo"]);
                }
                if (Request["PageSize"] != null)
                {
                    pageSize = Convert.ToInt32(Request["PageSize"]);
                }

                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = pageNo,
                            PageSize = pageSize,
                            Where = where ?? string.Empty,
                            OrderBy = orderBy ?? string.Empty
                        };
                        MethodReturnResult<IList<Package>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new LotPackageSEModulesViewModel());
        }
        public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

         /// <summary>
        /// 从ftp服务器上获得文件列表
        /// </summary>
        /// <param name="RequedstPath">服务器下的相对路径</param>
        /// <returns></returns>
        public bool  GetFile(string ftpServerIP, string ftpUserID, string ftpPassword, string fileDate)
        {
            FtpWebRequest reqFTP;
            string uri = "ftp://" + ftpServerIP + "/" + fileDate ;   //目标路径 path为服务器地址
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            FtpWebResponse response = null;
            try
            {
                // ftp用户名和密码
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                response = (FtpWebResponse)reqFTP.GetResponse();
                FtpStatusCode code = response.StatusCode;//OpeningData
                response.Close();
                return true;
            
            }
            catch 
            {
                if (response != null)
                {
                    response.Close();
                }
                return false;
            }
        }
    

        /// <summary>
        /// 新建目录 上一级必须先存在
        /// </summary>
        /// <param name="dirName">服务器下的相对路径</param>
        public static bool MakeDir(string ftpServerIP, string ftpUserID, string ftpPassword, string fileDate)
        {
            FtpWebRequest reqFTP;
            string uri = "ftp://" + ftpServerIP + "/" + fileDate;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            try
            {
                // 指定数据传输类型
                reqFTP.UseBinary = true;
                // ftp用户名和密码
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                return true;
            }
            catch
            {
                reqFTP.Abort();
                return false;
            }
        }
        
        /// <summary>
        /// FTP上传
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="filename">文件名称</param>
        /// <param name="ftpServerIP">FTP地址</param>
        /// <param name="ftpUserID">用户名</param>
        /// <param name="ftpPassword">密码</param>
        /// <returns></returns>
        public static bool UploadFtp(string filePath,string fileDate, string filename, string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            FileInfo fileInf = new FileInfo(filePath + "\\" + filename);
            string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
            FtpWebRequest reqFTP;
            // Create FtpWebRequest object from the Uri provided 
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" +fileDate+"/"+ fileInf.Name));
            try
            {
                // Provide the WebPermission Credintials 
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                // By default KeepAlive is true, where the control connection is not closed 
                // after a command is executed. 
                reqFTP.KeepAlive = false;
                // Specify the command to be executed. 
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                // Specify the data transfer type. 
                reqFTP.UseBinary = true;
                // Notify the server about the size of the uploaded file 
                reqFTP.ContentLength = fileInf.Length;
                // The buffer size is set to 2kb 
                int buffLength = 10240;
                byte[] buff = new byte[buffLength];
                int contentLen;

                // Opens a file stream (System.IO.FileStream) to read the file to be uploaded 
                //FileStream fs = fileInf.OpenRead(); 
                FileStream fs = fileInf.OpenRead();

                // Stream to which the file to be upload is written 
                Stream strm = reqFTP.GetRequestStream();

                // Read from the file stream 2kb at a time 
                contentLen = fs.Read(buff, 0, buffLength);

                // Till Stream content ends 
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the FTP Upload Stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

                // Close the file stream and the Request Stream 
                strm.Close();
                fs.Close();
                return true;
            }
            catch
            {
                reqFTP.Abort();
                //  Logging.WriteError(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 下载XML文件
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Path"></param>
        /// <param name="FileName"></param>

        public static bool DownLoad(string Url, string Path,string FileName)
        {
            HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            try
            {
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                Stream stream = new FileStream(Path + FileName, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                stream.Close();
                responseStream.Close();
                return true;
            }
            catch
            {
                response.Close();
                request.Abort();
                //  Logging.WriteError(ex.Message + ex.StackTrace);
                return false;
            }
          
        }

        //
        //POST: /WIP/LotPackageQuery/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToXML(LotPackageSEModulesViewModel model)
        {
            IList<Package> lstLotPackage = new List<Package>();
            string ftpServerIP = System.Configuration.ConfigurationManager.AppSettings["ftpServerIP"];
            string ftpUserID = System.Configuration.ConfigurationManager.AppSettings["ftpUserID"];
            string ftpPassword = System.Configuration.ConfigurationManager.AppSettings["ftpPassword"];
            string fileDate = DateTime.Now.ToString("yyyyMMdd");
            if (!GetFile(ftpServerIP, ftpUserID, ftpPassword, fileDate))//
            {
                MakeDir(ftpServerIP, ftpUserID, ftpPassword, fileDate);
            }
            LotPackageSEModulesViewModel SEModulesModel = new LotPackageSEModulesViewModel();
            try
            {
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            OrderBy = "Key",
                            Where = GetQueryCondition(model)
                        };
                        MethodReturnResult<IList<Package>> resultPackage = client.Get(ref cfg);
                        if (resultPackage.Code == 0)
                        {
                            lstLotPackage = resultPackage.Data;
                        }
                    });
                }
          
                string successMessage =string.Empty;//记录导出成功批次
                string failMessage = string.Empty; ;//记录导出失败批次
                for (int i = 0; i < lstLotPackage.Count; i++)
                {
                    string packageNo = lstLotPackage[i].Key;//包装号
                    MethodReturnResult<IList<Lot>> lotDetail = null;

                    using (LotQueryServiceClient client = new LotQueryServiceClient())//取得包装号对应的批次信息
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            Where = string.Format("PackageNo='{0}'", packageNo),
                            IsPaging = false
                        };
                        lotDetail = client.Get(ref cfg);
                    }
                    //string productType = SEModulesModel.GetProductType();
                    //创建XML文件
                    #region 1.创建类型声明节点
                    XmlDocument xmlDoc = new XmlDocument();     //XML对象
                    XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "ISO-8859-1", "no");
                    xmlDoc.AppendChild(node);
                    #endregion

                    #region 2.创建根节点
                    XmlElement root = xmlDoc.CreateElement("Parts");
                    root.SetAttribute("xmlns:xsi", @"http://www.w3.org/2001/XMLSchema-instance");
                    xmlDoc.AppendChild(root);
                    #endregion
                  
                    #region 3批次数据节点
                    for (int j = 0; j < lotDetail.Data.Count; j++)
                    {
                        string lotNumber = lotDetail.Data[j].Key;//批次号
                        //string packageNo = lotDetail.Data[j].PackageNo;//托号
                        string seModulesNo = string.Empty;
                        //if (lotDetail.Data[j].Attr3 != "" && lotDetail.Data[j].Attr3.Split('-').Length > 1)
                        //{
                        //    seModulesNo = lotDetail.Data[j].Attr3.Split('-')[1];
                        //}
                        //else
                        //{
                        //    seModulesNo = lotDetail.Data[j].Attr3;
                        //}
                        seModulesNo = lotDetail.Data[j].Attr3;
                       
                        string orderNumber = lotDetail.Data[j].OrderNumber;
                        string materialCode = lotDetail.Data[j].MaterialCode;

                        string pmp = "";
                        string isc = "";
                        string imp = "";
                        string voc = "";
                        string vmp = "";
                        string ff = "";
                        string pnom = "";
                        string current = "";
                        string colour = "";

                        if (lotDetail.Data[j].Color == "深蓝")
                        {
                            colour = "Dark Blue";
                        }
                        if (lotDetail.Data[j].Color == "浅蓝")
                        {
                            colour = "Light Blue";
                        }
                        if (lotDetail.Data[j].Color == "正蓝")
                        {
                            colour = "Blue";
                        } 

                        IVTestData ivtest = SEModulesModel.GetIVTestData(lotNumber);
                        RPTpackagelistParameter param = new RPTpackagelistParameter();
                        param.PackageNo = packageNo;
                        param.LotNumber = lotNumber;
                        param.PageSize = 20;
                        using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                        {
                            MethodReturnResult<DataSet> ds = client.GetRPTpackagelistQueryDb(ref param);
                            if (ds.Code > 0)
                            {
                                //return Content("批次{0}五大参数异常", lotNumber);
                            }
                            else
                            {
                                DataTable dtOfIV = ds.Data.Tables[0];
                                pmp = Convert.ToDecimal(dtOfIV.Rows[0]["COEF_PMAX"]).ToString("F3");
                                isc = Convert.ToDecimal(dtOfIV.Rows[0]["COEF_ISC"]).ToString("F3");
                                imp = Convert.ToDecimal(dtOfIV.Rows[0]["COEF_IMAX"]).ToString("F3");
                                voc = Convert.ToDecimal(dtOfIV.Rows[0]["COEF_VOC"]).ToString("F3");
                                vmp = Convert.ToDecimal(dtOfIV.Rows[0]["COEF_VMAX"]).ToString("F3");
                                if (Convert.ToDecimal(dtOfIV.Rows[0]["COEF_FF"]) < 1)
                                {
                                    ff = (Convert.ToDecimal(dtOfIV.Rows[0]["COEF_FF"])*100).ToString("F3");
                                }
                                else
                                {
                                    ff = Convert.ToDecimal(dtOfIV.Rows[0]["COEF_FF"]).ToString("F3");
                                }                               
                                pnom = dtOfIV.Rows[0]["PM_NAME"].ToString();                                
                                current = dtOfIV.Rows[0]["PS_SUBCODE"].ToString();
                                if (current == "α")
                                {
                                    current = "A";
                                }
                                if (current == "β")
                                {
                                    current = "B";
                                }
                                if (current == "γ")
                                {
                                    current = "C";
                                }
                            }
                        }
                        string productType = SEModulesModel.GetProductType(materialCode, orderNumber, ivtest.PowersetCode, ivtest.PowersetItemNo.Value, lotDetail.Data[j]);//对应主铭牌上的产品型号
                        string productTypes = SEModulesModel.GetProductTypes(materialCode, orderNumber, ivtest.PowersetCode, ivtest.PowersetItemNo.Value, lotDetail.Data[j]); //对应副标签上的产品型号
                        if (productType == "" || productTypes=="")
                        {
                            return Content("产片编码：{0}未设置产品对应属性", materialCode);
                        }
                        XmlNode PartDataNode = xmlDoc.CreateNode(XmlNodeType.Element, "PartData", null);
                        root.AppendChild(PartDataNode);
                        CreateNode(xmlDoc, PartDataNode, "ModuleSerialNumber", "JN"+lotNumber);
                        CreateNode(xmlDoc, PartDataNode, "SESerialNumber", seModulesNo);
                        CreateNode(xmlDoc, PartDataNode, "PartNumber", productTypes);
                        CreateNode(xmlDoc, PartDataNode, "PalletNumber", packageNo);

                        //CreateNode(xmlDoc, PartDataNode, "Pmp-W", pmp);
                        //CreateNode(xmlDoc, PartDataNode, "Isc-A", isc);
                        //CreateNode(xmlDoc, PartDataNode, "Imp-A", imp);
                        //CreateNode(xmlDoc, PartDataNode, "Voc-V", voc);
                        //CreateNode(xmlDoc, PartDataNode, "Vmp-V", vmp);
                        //CreateNode(xmlDoc, PartDataNode, "FF-%", ff);
                        //CreateNode(xmlDoc, PartDataNode, "Pnom-W", pnom);
                        //CreateNode(xmlDoc, PartDataNode, "Current-A", current);

                        CreateNode(xmlDoc, PartDataNode, "Pmp", pmp+"W");
                        CreateNode(xmlDoc, PartDataNode, "Isc", isc+"A");
                        CreateNode(xmlDoc, PartDataNode, "Imp", imp+"A");
                        CreateNode(xmlDoc, PartDataNode, "Voc", voc+"V");
                        CreateNode(xmlDoc, PartDataNode, "Vmp", vmp+"V");
                        CreateNode(xmlDoc, PartDataNode, "FF", ff+"%");
                        CreateNode(xmlDoc, PartDataNode, "Pnom", pnom);
                        CreateNode(xmlDoc, PartDataNode, "Current", current);
                        CreateNode(xmlDoc, PartDataNode, "Colour", colour);
                        CreateNode(xmlDoc, PartDataNode, "LabelPN", productType);
                    }
                    #endregion

                    //XML路径
                    string path = Server.MapPath("~/SEModules/");
                    if (Directory.Exists(path) == false)
                    {
                        Directory.CreateDirectory(path);
                    }
                    string fileName = "SEModules" + DateTime.Now.ToString("yyyyMMddHHmm") + packageNo + ".xml";
                    string  pathFile = path + fileName ;
                    xmlDoc.Save(pathFile);
               
                    //string ftpServerIP = "seftp.solaredge.com";
                    //string ftpUserID = "jinergy";
                    //string ftpPassword = "26t&%l*H!#ZI";
                    if (UploadFtp(path, fileDate,fileName, ftpServerIP, ftpUserID, ftpPassword))
                    {
                        successMessage = successMessage + packageNo+";" ;
                    }
                    else
                    {
                        failMessage = failMessage + packageNo + ";";
                    }
                }
                if (successMessage.Length > 0)
                {
                    successMessage = successMessage + "导出成功";
                }
                if (failMessage.Length > 0)
                {
                    failMessage = failMessage + "导出失败";
                }
                return Content("批次号："+successMessage+failMessage);
            }
            catch (Exception err)
            {
                return Content(string.Format("导出XML报错：{0}", err.Message));
            }
          
        }

        public string GetQueryCondition(LotPackageSEModulesViewModel model)
        {
            StringBuilder where = new StringBuilder();
            where.AppendFormat(" Quantity > 0");
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.PackageNo) && !string.IsNullOrEmpty(model.PackageNo1))
                {
                    where.AppendFormat(" {0} Key >= '{1}' AND Key<='{2}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PackageNo
                                        , model.PackageNo1);
                }
                else if (!string.IsNullOrEmpty(model.PackageNo))
                {
                    char [] splitChars=new char[] { ',', '$' };
                    string[] packageNos = model.PackageNo.TrimEnd(splitChars).Split(splitChars);

                    #region 界面托号包含归档，则归档托号提取
                    if (packageNos.Length >= 1)
                    {
                        //若托号不存在当前库，提取归档托号数据
                        foreach (string item in packageNos)
                        {
                            Package package = null;
                            using (PackageQueryServiceClient clientOfPackage = new PackageQueryServiceClient())
                            {
                                if (clientOfPackage.Get(item.Trim().ToUpper()) != null && clientOfPackage.Get(item.Trim().ToUpper()).Data != null)
                                {
                                    package = clientOfPackage.Get(item.Trim().ToUpper()).Data;
                                }
                            }
                            if (package == null)
                            {
                                MethodReturnResult resultOfRePackage = new MethodReturnResult();
                                //若存在归档，则提取
                                using (PackageInChestServiceClient packageInChestServiceClient = new PackageInChestServiceClient())
                                {
                                    //返回已归档的(WIP_PACKAGE表)数据
                                    REbackdataParameter pre = new REbackdataParameter();
                                    pre.PackageNo = item;
                                    pre.ErrorMsg = "";
                                    pre.ReType = 1;
                                    pre.IsDelete = 0;
                                    resultOfRePackage = packageInChestServiceClient.GetREbackdata(pre);

                                    if (resultOfRePackage.Code > 0)
                                    {
                                        
                                    }
                                    else
                                    {
                                        //提取其他归档表数据到当前库，并删除从归档库
                                        pre = new REbackdataParameter();
                                        pre.PackageNo = item;
                                        pre.ReType = 2;
                                        pre.IsDelete = 1;
                                        resultOfRePackage = packageInChestServiceClient.GetREbackdata(pre);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    if (packageNos.Length <= 1)
                    {
                        where.AppendFormat(" {0} Key = '{1}'"
                                            , where.Length > 0 ? "AND" : string.Empty
                                            , packageNos[0]);
                    }
                    else
                    {
                        

                        where.AppendFormat(" {0} Key IN ("
                                            , where.Length > 0 ? "AND" : string.Empty);

                        foreach (string package in packageNos)
                        {
                            where.AppendFormat("'{0}',", package);
                        }
                        where.Remove(where.Length - 1, 1);
                        where.Append(")");
                    }
                }


                if (!string.IsNullOrEmpty(model.OrderNumber))
                {
                    where.AppendFormat(" {0} OrderNumber LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.OrderNumber);
                }

                if (model.StartCreateTime != null)
                {
                    where.AppendFormat(" {0} CreateTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.StartCreateTime);
                }

                if (model.EndCreateTime != null)
                {
                    where.AppendFormat(" {0} CreateTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EndCreateTime);
                }
            }
            return where.ToString();
        }
	}
}