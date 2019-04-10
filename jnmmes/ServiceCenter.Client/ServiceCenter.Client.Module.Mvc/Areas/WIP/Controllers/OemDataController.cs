using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using System.Threading.Tasks;
using ServiceCenter.MES.Service.Client.WIP;
using System.Text;
using ServiceCenter.Model;
using System.Web;
using System.IO;
using NPOI.HSSF.UserModel;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class OemDataController :  Controller
    {
        public ActionResult Index()
        {
            return View(new OemDataViewModel());
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(OemDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (OemDataServiceClient client = new OemDataServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.LotNumber))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.LotNumber.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.OrderNumber))
                            {
                                where.AppendFormat(" {0} OrderNumber LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.PackageNo))
                            {
                                where.AppendFormat(" {0} PackageNo LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.PackageNo.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.PnName))
                            {
                                where.AppendFormat(" {0} PnName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.PnName.ToString().Trim().ToUpper());
                            }
                            if (!string.IsNullOrEmpty(model.Grade))
                            {
                                where.AppendFormat(" {0} Grade LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Grade.ToString().Trim().ToUpper());
                            }
                            if (!string.IsNullOrEmpty(model.Color))
                            {
                                where.AppendFormat(" {0} Color LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Color.ToString().Trim().ToUpper());
                            }
                            if (!string.IsNullOrEmpty(model.PsSubCode))
                            {
                                where.AppendFormat(" {0} PsSubCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.PsSubCode.ToString().Trim().ToUpper());
                            }
                            if (!string.IsNullOrEmpty(model.Status))
                            {
                                where.AppendFormat(" {0} Status LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Status.ToString().Trim().ToUpper());
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<OemData>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.OemDataList = result.Data;
                        }
                    });
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", model);
            }
            else
            {
                return View(model);
            }
        }

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

                using (OemDataServiceClient client = new OemDataServiceClient())
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
                        MethodReturnResult<IList<OemData>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.OemDataList = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IuputFromExcel(OemDataViewModel model)
        {
            DataTable data = new DataTable();
            await Task.Run(() =>
                {
                    string strExcel = "";

                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
                    openFileDialog.Filter = "文本文件|*.*|C#文件|*.cs|所有文件|*.*";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.FilterIndex = 1;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        strExcel = openFileDialog.FileName;
                    }
                    data = ExcelToDataTable(strExcel);                    
                });
            return Json(data);       
        }

        public static DataTable ExcelToDataTable(string strExcelFileName)
        {
            FileStream fs = System.IO.File.OpenRead(strExcelFileName);
            //源的定义            
            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + strExcelFileName + ";" + "Extended Properties='Excel 12.0;HDR=YES;IMEX=1';";

            //定义存放的数据表
            DataSet ds = new DataSet();

            //连接数据源
            OleDbConnection conn = new OleDbConnection(connString);
            conn.Open();
            HSSFWorkbook workbook = new HSSFWorkbook(fs);
            HSSFSheet sheet = (HSSFSheet)workbook.GetSheetAt(0); // 获取此文件第一个Sheet页
            string strExcel = "select * from" + "[" + sheet.SheetName + "$]";
            OleDbDataAdapter myCmd = null; ;
            myCmd = new OleDbDataAdapter(strExcel, conn);
            myCmd.Fill(ds, "table");
            DataTable data = ds.Tables["table"];

            conn.Close();

            return data;
        }

    }
}