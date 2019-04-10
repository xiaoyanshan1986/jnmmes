using Newtonsoft.Json;
using ServiceCenter.Client.Mvc.Areas.ERP.Models;
using ServiceCenter.Client.Mvc.Resources.ERP;
using ServiceCenter.MES.Model.ERP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.ERP;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Contract.ERP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Web.Caching;

namespace ServiceCenter.Client.Mvc.Areas.ERP.Controllers
{
    public class ERPMaterialReceiptController : Controller
    {
        #region 注释

        //public ActionResult Detail(string ReceiptNo)
        //{
        //    MaterialReceiptViewModel model = null;
        //    using (LineStoreServiceClient client = new LineStoreServiceClient())
        //    {
        //        PagingConfig cfg = new PagingConfig()
        //        {
        //            OrderBy = "Key"
        //        };
        //        MethodReturnResult<IList<LineStore>> result = client.Get(ref cfg);
        //        List<SelectListItem> LineStoreList = new List<SelectListItem>();
        //        //LineStoreList.Add(new SelectListItem() { Text = "--Select--", Value = "select" });
        //        foreach (LineStore item in result.Data)
        //        {
        //            LineStoreList.Add(new SelectListItem() { Text = item.Key, Value = item.Key });
        //        }
        //        ViewBag.LineStore = LineStoreList;
        //    }

        //    using (ERPClient client = new ERPClient())
        //    {
        //        MethodReturnResult<DataSet> result = client.GetERPMaterialReceipt(ReceiptNo);
        //        if (result.Data.Tables[0].Rows.Count > 0)
        //        {
        //            model = new MaterialReceiptViewModel()
        //            {
        //                ReceiptNo = result.Data.Tables[0].Rows[0]["VBILLCODE"].ToString(),
        //                OrderNumber = result.Data.Tables[0].Rows[0]["VPRODUCTBATCH"].ToString(),
        //                ReceiptDate = DateTime.Parse(result.Data.Tables[0].Rows[0]["DBILLDATE"].ToString())
        //            };
        //        }
        //        ViewBag.MaterialReceipt = model;

        //        return View(model);
        //    }
        //}

        //public ActionResult GetMaterialReceiptDetail(string ReceiptNo)
        //{
        //    using (ERPClient client = new ERPClient())
        //    {
        //        PagingConfig cfg = new PagingConfig();
        //        //List<MaterialReceiptDetailViewModel> list = new List<MaterialReceiptDetailViewModel>();
        //        List<MaterialReceiptReplaceViewModel> list = new List<MaterialReceiptReplaceViewModel>();
        //        MethodReturnResult<DataSet> result = client.GetERPMaterialReceiptDetail(ReceiptNo);
        //        //     t1.battransrate 转换率
        //        //     t1.batcolor     颜色
        //        //     t1.batlvl       等级
        //        if (result.Code == 0 && result.Data.Tables[0].Rows.Count > 0)
        //        {
        //            //for (int i = 0; i < result.Data.Tables[0].Rows.Count; i++)
        //            //{
        //            MethodReturnResult<List<MaterialReceiptReplace>> lstMaterialReceiptReplace = client.GetMaterialReceiptReplaceDetail(result, cfg.PageNo, cfg.PageSize);
        //            //MaterialReceiptDetailViewModel model = new MaterialReceiptDetailViewModel()
        //            //{
        //            //    ItemNo = i + 1,
        //            //    MaterialCode = result.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString(),
        //            //    MaterialLot = result.Data.Tables[0].Rows[i]["VBATCHCODE"].ToString(),
        //            //    Qty = Convert.ToDouble(result.Data.Tables[0].Rows[i]["NNUM"].ToString()),
        //            //    SupplierCode = result.Data.Tables[0].Rows[i]["SUPPLIERCODE"].ToString(),
        //            //    SupplierName = result.Data.Tables[0].Rows[i]["SUPPLIERNAME"].ToString(),
        //            //    Attr1 = result.Data.Tables[0].Rows[i]["BATTRANSRATE"].ToString(),//效率档 t1.battransrate                                        
        //            //    Attr2 = result.Data.Tables[0].Rows[i]["BATCOLOR"].ToString(),//花色  t1.batcolor
        //            //    Attr3 = result.Data.Tables[0].Rows[i]["BATLVL"].ToString(),//等级  t1.batlvl
        //            //};
        //            if (lstMaterialReceiptReplace.Code == 0 && lstMaterialReceiptReplace.Data != null)
        //            {
        //                cfg.Records = result.Data.Tables[0].Rows.Count;
        //                //for (int i = Convert.ToInt32(cfg.PageNo * cfg.PageSize); i < (cfg.PageNo + 1) * cfg.PageSize; i++)
        //                //{
        //                //    MaterialReceiptReplaceViewModel model = new MaterialReceiptReplaceViewModel()
        //                //    {
        //                //        ItemNo = lstMaterialReceiptReplace.Data[i].Key,
        //                //        OrderNumber = lstMaterialReceiptReplace.Data[i].OrderNumber,
        //                //        OldMaterialCode = lstMaterialReceiptReplace.Data[i].OldMaterialCode,
        //                //        OldMaterialLot = lstMaterialReceiptReplace.Data[i].OldMaterialLot,
        //                //        MaterialCode = lstMaterialReceiptReplace.Data[i].MaterialCode,
        //                //        MaterialLot = lstMaterialReceiptReplace.Data[i].MaterialLot,
        //                //        Qty = lstMaterialReceiptReplace.Data[i].Qty,
        //                //        CellPower = lstMaterialReceiptReplace.Data[i].CellPower,
        //                //        CellGrade = lstMaterialReceiptReplace.Data[i].CellGrade,
        //                //        OldCellColor = lstMaterialReceiptReplace.Data[i].OldCellColor,
        //                //        CellColor = lstMaterialReceiptReplace.Data[i].CellColor,
        //                //        OldSupplierCode = lstMaterialReceiptReplace.Data[i].OldSupplierCode,
        //                //        OldSupplierName = lstMaterialReceiptReplace.Data[i].OldSupplierName,
        //                //        SupplierCode = lstMaterialReceiptReplace.Data[i].SupplierCode,
        //                //        SupplierName = lstMaterialReceiptReplace.Data[i].SupplierName,
        //                //        OldManufacturerCode = lstMaterialReceiptReplace.Data[i].OldManufacturerCode,
        //                //        OldManufacturerName = lstMaterialReceiptReplace.Data[i].OldManufacturerName,
        //                //        ManufacturerCode = lstMaterialReceiptReplace.Data[i].ManufacturerCode,
        //                //        ManufacturerName = lstMaterialReceiptReplace.Data[i].ManufacturerName,
        //                //        SupplierMaterialLot = lstMaterialReceiptReplace.Data[i].SupplierMaterialLot,
        //                //        Description = lstMaterialReceiptReplace.Data[i].Description
        //                //    };
        //                //    list.Add(model);
        //                //}

        //                foreach (MaterialReceiptReplace mrr in lstMaterialReceiptReplace.Data)
        //                {
        //                    MaterialReceiptReplaceViewModel model = new MaterialReceiptReplaceViewModel()
        //                    {
        //                        ItemNo = mrr.Key,
        //                        OrderNumber = mrr.OrderNumber,
        //                        OldMaterialCode = mrr.OldMaterialCode,
        //                        OldMaterialLot = mrr.OldMaterialLot,
        //                        MaterialCode = mrr.MaterialCode,
        //                        MaterialLot = mrr.MaterialLot,
        //                        Qty = mrr.Qty,
        //                        CellPower = mrr.CellPower,
        //                        CellGrade = mrr.CellGrade,
        //                        OldCellColor = mrr.OldCellColor,
        //                        CellColor = mrr.CellColor,
        //                        OldSupplierCode = mrr.OldSupplierCode,
        //                        OldSupplierName = mrr.OldSupplierName,
        //                        SupplierCode = mrr.SupplierCode,
        //                        SupplierName = mrr.SupplierName,
        //                        OldManufacturerCode = mrr.OldManufacturerCode,
        //                        OldManufacturerName = mrr.OldManufacturerName,
        //                        ManufacturerCode = mrr.ManufacturerCode,
        //                        ManufacturerName = mrr.ManufacturerName,
        //                        SupplierMaterialLot = mrr.SupplierMaterialLot,
        //                        Description = mrr.Description
        //                    };
        //                    list.Add(model);
        //                }
        //            }
        //            //list.Add(model);
        //            //}
        //            ViewBag.MaterialReceiptReplace = list;
        //            ViewBag.ReceiptNo = ReceiptNo;
        //            ViewBag.PagingConfig = cfg;
        //        }
        //        return PartialView("_ListPartial");
        //    }
        //}

        //public string REback(string MaterialLot)
        //{
        //    MethodReturnResult result = new MethodReturnResult();
        //    using (ERPClient client = new ERPClient())
        //    {
        //        REbackdataParameter param = new REbackdataParameter();
        //        param.PackageNo = MaterialLot;
        //        param.ErrorMsg = "";
        //        result = client.GetREbackdata(param);
        //        //if (result.Code == 1000)
        //        //{
        //        //    result.Message += string.Format(WIPResources.StringResource.REbackdata_Fail);
        //        //}
        //    }
        //    return result.Message;
        ////}

        //分页获取领料明细
        //public ActionResult PagingQueryReplace(string receiptNo, MethodReturnResult<List<MaterialReceiptReplace>> materialReceiptReplaceDetailAll, string orderBy, int? currentPageNo, int? currentPageSize)
        //{
        //    using (ERPClient client = new ERPClient())
        //    {
        //        int pageNo = currentPageNo ?? 0;
        //        int pageSize = currentPageSize ?? 20;
        //        if (Request["PageNo"] != null)
        //        {
        //            pageNo = Convert.ToInt32(Request["PageNo"]);
        //        }
        //        if (Request["PageSize"] != null)
        //        {
        //            pageSize = Convert.ToInt32(Request["PageSize"]);
        //        }
        //        List<MaterialReceiptReplaceViewModel> list = new List<MaterialReceiptReplaceViewModel>();
        //        MethodReturnResult<DataSet> result = client.GetERPMaterialReceiptDetail(receiptNo);
        //        if (result.Code == 0 && result.Data.Tables[0].Rows.Count > 0)
        //        {
        //            MethodReturnResult<List<MaterialReceiptReplace>> lstMaterialReceiptReplace = client.GetMaterialReceiptReplaceDetail(result, pageNo, pageSize);
        //            if (lstMaterialReceiptReplace.Code == 0 && lstMaterialReceiptReplace.Data != null)
        //            {
        //                //for (int i = Convert.ToInt32(pageNo * pageSize); i < (pageNo + 1) * pageSize; i++)
        //                //{
        //                //    MaterialReceiptReplaceViewModel model = new MaterialReceiptReplaceViewModel()
        //                //    {
        //                //        ItemNo = lstMaterialReceiptReplace.Data[i].Key,
        //                //        OrderNumber = lstMaterialReceiptReplace.Data[i].OrderNumber,
        //                //        OldMaterialCode = lstMaterialReceiptReplace.Data[i].OldMaterialCode,
        //                //        OldMaterialLot = lstMaterialReceiptReplace.Data[i].OldMaterialLot,
        //                //        MaterialCode = lstMaterialReceiptReplace.Data[i].MaterialCode,
        //                //        MaterialLot = lstMaterialReceiptReplace.Data[i].MaterialLot,
        //                //        Qty = lstMaterialReceiptReplace.Data[i].Qty,
        //                //        CellPower = lstMaterialReceiptReplace.Data[i].CellPower,
        //                //        CellGrade = lstMaterialReceiptReplace.Data[i].CellGrade,
        //                //        OldCellColor = lstMaterialReceiptReplace.Data[i].OldCellColor,
        //                //        CellColor = lstMaterialReceiptReplace.Data[i].CellColor,
        //                //        OldSupplierCode = lstMaterialReceiptReplace.Data[i].OldSupplierCode,
        //                //        OldSupplierName = lstMaterialReceiptReplace.Data[i].OldSupplierName,
        //                //        SupplierCode = lstMaterialReceiptReplace.Data[i].SupplierCode,
        //                //        SupplierName = lstMaterialReceiptReplace.Data[i].SupplierName,
        //                //        OldManufacturerCode = lstMaterialReceiptReplace.Data[i].OldManufacturerCode,
        //                //        OldManufacturerName = lstMaterialReceiptReplace.Data[i].OldManufacturerName,
        //                //        ManufacturerCode = lstMaterialReceiptReplace.Data[i].ManufacturerCode,
        //                //        ManufacturerName = lstMaterialReceiptReplace.Data[i].ManufacturerName,
        //                //        SupplierMaterialLot = lstMaterialReceiptReplace.Data[i].SupplierMaterialLot,
        //                //        Description = lstMaterialReceiptReplace.Data[i].Description
        //                //    };
        //                //    list.Add(model);
        //                //}

        //                foreach (MaterialReceiptReplace mrr in lstMaterialReceiptReplace.Data)
        //                {
        //                    MaterialReceiptReplaceViewModel model = new MaterialReceiptReplaceViewModel()
        //                    {
        //                        ItemNo = mrr.Key,
        //                        OrderNumber = mrr.OrderNumber,
        //                        OldMaterialCode = mrr.OldMaterialCode,
        //                        OldMaterialLot = mrr.OldMaterialLot,
        //                        MaterialCode = mrr.MaterialCode,
        //                        MaterialLot = mrr.MaterialLot,
        //                        Qty = mrr.Qty,
        //                        CellPower = mrr.CellPower,
        //                        CellGrade = mrr.CellGrade,
        //                        OldCellColor = mrr.OldCellColor,
        //                        CellColor = mrr.CellColor,
        //                        OldSupplierCode = mrr.OldSupplierCode,
        //                        OldSupplierName = mrr.OldSupplierName,
        //                        SupplierCode = mrr.SupplierCode,
        //                        SupplierName = mrr.SupplierName,
        //                        OldManufacturerCode = mrr.OldManufacturerCode,
        //                        OldManufacturerName = mrr.OldManufacturerName,
        //                        ManufacturerCode = mrr.ManufacturerCode,
        //                        ManufacturerName = mrr.ManufacturerName,
        //                        SupplierMaterialLot = mrr.SupplierMaterialLot,
        //                        Description = mrr.Description
        //                    };
        //                    list.Add(model);
        //                }
        //            }
        //            PagingConfig cfg = new PagingConfig()
        //            {
        //                PageNo = pageNo,
        //                PageSize = pageSize,
        //                Records = result.Data.Tables[0].Rows.Count
        //            };
        //            ViewBag.MaterialReceiptReplace = list;
        //            ViewBag.ReceiptNo = receiptNo;
        //            ViewBag.PagingConfig = cfg;
        //        }
        //        return PartialView("_ListPartial");
        //    }
        //}

        #endregion        

        public Cache cache = HttpRuntime.Cache;

        // GET: ERP/ERPMaterialReceipt
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Query(MaterialReceiptQueryViewModel model)
        {
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult ReturnResult = new MethodReturnResult();
                MethodReturnResult<DataSet> result = client.GetERPMaterialReceipt(model.ReceiptNo);

                if (result.Code == 0)
                {
                    if (result.Data.Tables[0].Rows.Count > 0)
                    {
                        WorkOrderServiceClient workClient = new WorkOrderServiceClient();
                        MethodReturnResult<WorkOrder> rst = await workClient.GetAsync(result.Data.Tables[0].Rows[0]["VPRODUCTBATCH"].ToString());
                        if (rst.Data == null)
                        {
                            ReturnResult.Code = 1001;
                            ReturnResult.Message = string.Format(StringResource.ERPWorkOrder_Error_Query, model.ReceiptNo, result.Data.Tables[0].Rows[0]["VPRODUCTBATCH"].ToString());
                        }
                    }
                    else
                    {
                        ReturnResult.Code = 1001;
                        ReturnResult.Message = string.Format(StringResource.ERPMaterialReceipt_Error_Query, model.ReceiptNo);
                    }
                }
                else
                {
                    ReturnResult.Code = result.Code;
                    ReturnResult.Message = result.Message;
                }
                return Json(ReturnResult);
            }
        }       

        //领料明细前20条
        public ActionResult GetMaterialReceiptDetail(string ReceiptNo)
        {            
            using (ERPClient client = new ERPClient())
            {
                PagingConfig cfg = new PagingConfig();
                List<MaterialReceiptReplaceViewModel> list = new List<MaterialReceiptReplaceViewModel>();
                MethodReturnResult<DataSet> result = client.GetERPMaterialReceiptDetail(ReceiptNo);
                if (result.Code == 0 && result.Data.Tables[0].Rows.Count > 0)
                {
                    MethodReturnResult<List<MaterialReceiptReplace>> lstMaterialReceiptReplace = new MethodReturnResult<List<MaterialReceiptReplace>>();
                    //获取全部领料明细
                    if (cache[ReceiptNo] != null)
                    {
                        lstMaterialReceiptReplace = (MethodReturnResult<List<MaterialReceiptReplace>>)cache[ReceiptNo];
                    }
                    else
                    {
                        lstMaterialReceiptReplace = client.GetMaterialReceiptReplaceDetail(result, 0, 20);
                    }                    

                    if (lstMaterialReceiptReplace.Code == 0 && lstMaterialReceiptReplace.Data != null)
                    {
                        //将全部领料明细写入缓存
                        //将数组添加到缓存中——使用Add方法
                        if (cache[ReceiptNo] == null)
                        {
                            cache.Add(ReceiptNo, lstMaterialReceiptReplace, null, DateTime.Now.AddSeconds(900), TimeSpan.Zero, CacheItemPriority.Normal, null);
                        }

                        cfg.Records = result.Data.Tables[0].Rows.Count;
                        int count = 0;
                        if ((cfg.PageNo + 1) * cfg.PageSize < lstMaterialReceiptReplace.Data.Count)
                        {
                            count = (cfg.PageNo + 1) * cfg.PageSize;
                        }
                        else
                        {
                            count = lstMaterialReceiptReplace.Data.Count;
                        }
                        //生成领料明细前20条
                        for (int i = Convert.ToInt32(cfg.PageNo * cfg.PageSize); i < count; i++)
                        {
                            MaterialReceiptReplaceViewModel model = new MaterialReceiptReplaceViewModel()
                            {
                                ItemNo = lstMaterialReceiptReplace.Data[i].Key,
                                OrderNumber = lstMaterialReceiptReplace.Data[i].OrderNumber,
                                OldMaterialCode = lstMaterialReceiptReplace.Data[i].OldMaterialCode,
                                OldMaterialLot = lstMaterialReceiptReplace.Data[i].OldMaterialLot,
                                MaterialCode = lstMaterialReceiptReplace.Data[i].MaterialCode,
                                MaterialLot = lstMaterialReceiptReplace.Data[i].MaterialLot,
                                Qty = lstMaterialReceiptReplace.Data[i].Qty,
                                CellPower = lstMaterialReceiptReplace.Data[i].CellPower,
                                CellGrade = lstMaterialReceiptReplace.Data[i].CellGrade,
                                OldCellColor = lstMaterialReceiptReplace.Data[i].OldCellColor,
                                CellColor = lstMaterialReceiptReplace.Data[i].CellColor,
                                OldSupplierCode = lstMaterialReceiptReplace.Data[i].OldSupplierCode,
                                OldSupplierName = lstMaterialReceiptReplace.Data[i].OldSupplierName,
                                SupplierCode = lstMaterialReceiptReplace.Data[i].SupplierCode,
                                SupplierName = lstMaterialReceiptReplace.Data[i].SupplierName,
                                OldManufacturerCode = lstMaterialReceiptReplace.Data[i].OldManufacturerCode,
                                OldManufacturerName = lstMaterialReceiptReplace.Data[i].OldManufacturerName,
                                ManufacturerCode = lstMaterialReceiptReplace.Data[i].ManufacturerCode,
                                ManufacturerName = lstMaterialReceiptReplace.Data[i].ManufacturerName,
                                SupplierMaterialLot = lstMaterialReceiptReplace.Data[i].SupplierMaterialLot,
                                Description = lstMaterialReceiptReplace.Data[i].Description
                            };
                            list.Add(model);
                        }                        
                    }
                    ViewBag.MaterialReceiptReplace = list;
                    //ViewBag.MaterialReceiptReplaceDetailAll = JsonConvert.SerializeObject(lstMaterialReceiptReplace);
                    ViewBag.ReceiptNo = ReceiptNo;
                    ViewBag.PagingConfig = cfg;
                }
                return PartialView("_ListPartial");
            }
        }

        //分页获取领料明细
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PagingQueryReplace(string receiptNo, string orderBy, int? currentPageNo, int? currentPageSize)
        {
            using (ERPClient client = new ERPClient())
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

                MethodReturnResult<List<MaterialReceiptReplace>> MaterialReceiptReplaceDetailAll = new MethodReturnResult<List<MaterialReceiptReplace>>();
                //获取全部领料明细
                if (cache[receiptNo] != null)
                {
                    MaterialReceiptReplaceDetailAll = (MethodReturnResult<List<MaterialReceiptReplace>>)cache[receiptNo];
                }
                else
                {
                    MethodReturnResult<DataSet> result = client.GetERPMaterialReceiptDetail(receiptNo);
                    MaterialReceiptReplaceDetailAll = client.GetMaterialReceiptReplaceDetail(result, 0, 20);
                }
                
                List<MaterialReceiptReplaceViewModel> list = new List<MaterialReceiptReplaceViewModel>();
                if (MaterialReceiptReplaceDetailAll.Code == 0 && MaterialReceiptReplaceDetailAll.Data != null)
                {
                    //将数组添加到缓存中——使用Add方法
                    if (cache[receiptNo] == null)
                    {
                        cache.Add(receiptNo, MaterialReceiptReplaceDetailAll, null, DateTime.Now.AddSeconds(900), TimeSpan.Zero, CacheItemPriority.Normal, null);
                    }  
                    int count = 0;
                    if ((pageNo + 1) * pageSize < MaterialReceiptReplaceDetailAll.Data.Count)
                    {
                        count = (pageNo + 1) * pageSize;
                    }
                    else
                    {
                        count = MaterialReceiptReplaceDetailAll.Data.Count;
                    }
                    for (int i = Convert.ToInt32(pageNo * pageSize); i < count; i++)
                    {
                        MaterialReceiptReplaceViewModel model = new MaterialReceiptReplaceViewModel()
                        {
                            ItemNo = MaterialReceiptReplaceDetailAll.Data[i].Key,
                            OrderNumber = MaterialReceiptReplaceDetailAll.Data[i].OrderNumber,
                            OldMaterialCode = MaterialReceiptReplaceDetailAll.Data[i].OldMaterialCode,
                            OldMaterialLot = MaterialReceiptReplaceDetailAll.Data[i].OldMaterialLot,
                            MaterialCode = MaterialReceiptReplaceDetailAll.Data[i].MaterialCode,
                            MaterialLot = MaterialReceiptReplaceDetailAll.Data[i].MaterialLot,
                            Qty = MaterialReceiptReplaceDetailAll.Data[i].Qty,
                            CellPower = MaterialReceiptReplaceDetailAll.Data[i].CellPower,
                            CellGrade = MaterialReceiptReplaceDetailAll.Data[i].CellGrade,
                            OldCellColor = MaterialReceiptReplaceDetailAll.Data[i].OldCellColor,
                            CellColor = MaterialReceiptReplaceDetailAll.Data[i].CellColor,
                            OldSupplierCode = MaterialReceiptReplaceDetailAll.Data[i].OldSupplierCode,
                            OldSupplierName = MaterialReceiptReplaceDetailAll.Data[i].OldSupplierName,
                            SupplierCode = MaterialReceiptReplaceDetailAll.Data[i].SupplierCode,
                            SupplierName = MaterialReceiptReplaceDetailAll.Data[i].SupplierName,
                            OldManufacturerCode = MaterialReceiptReplaceDetailAll.Data[i].OldManufacturerCode,
                            OldManufacturerName = MaterialReceiptReplaceDetailAll.Data[i].OldManufacturerName,
                            ManufacturerCode = MaterialReceiptReplaceDetailAll.Data[i].ManufacturerCode,
                            ManufacturerName = MaterialReceiptReplaceDetailAll.Data[i].ManufacturerName,
                            SupplierMaterialLot = MaterialReceiptReplaceDetailAll.Data[i].SupplierMaterialLot,
                            Description = MaterialReceiptReplaceDetailAll.Data[i].Description
                        };
                        list.Add(model);
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        PageNo = pageNo,
                        PageSize = pageSize,
                        Records = MaterialReceiptReplaceDetailAll.Data.Count
                    };
                    ViewBag.MaterialReceiptReplace = list;
                    //ViewBag.MaterialReceiptReplaceDetailAll = MaterialReceiptReplaceDetailAll;
                    ViewBag.ReceiptNo = receiptNo;
                    ViewBag.PagingConfig = cfg;
                }
                return PartialView("_ListPartial");
            }
        }

        public ActionResult Save(string ReceiptNo, string LineStore, string Description)
        {
           // MethodReturnResult res = new MethodReturnResult();
            //string error_mes = string.Empty;
            MethodReturnResult result = new MethodReturnResult();
            using (ERPClient client = new ERPClient())
            {
                ERPMaterialReceiptParameter param = new ERPMaterialReceiptParameter();
                param.ReceiptNo = ReceiptNo;
                param.LineStore = LineStore;
                param.Creator = User.Identity.Name;
                param.Description = Description;
                result = client.AddERPMaterialReceipt(param);
                if (result.Code == 0)
                {
                    result.Message += string.Format(StringResource.ERPMaterialReceipt_Add_Success);
                    //移除缓存
                    cache.Remove(ReceiptNo); 
                }
            }
            return Json(result);
        }               

        public ActionResult Detail(string ReceiptNo)
        {
            MaterialReceiptViewModel model = null;
            using (LineStoreServiceClient client = new LineStoreServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "Key"
                };
                MethodReturnResult<IList<LineStore>> result = client.Get(ref cfg);
                List<SelectListItem> LineStoreList = new List<SelectListItem>();
                //LineStoreList.Add(new SelectListItem() { Text = "--Select--", Value = "select" });
                foreach (LineStore item in result.Data)
                {
                    LineStoreList.Add(new SelectListItem() { Text = item.Key, Value = item.Key });
                }
                ViewBag.LineStore = LineStoreList;
            }

            using (ERPClient client = new ERPClient())
            {
                //根据材料出库单单据号查询ERP中是否存在该出库单
                MethodReturnResult<DataSet> result = client.GetERPMaterialReceiptStore(ReceiptNo);
                DataTable dtERP = new DataTable();
                DataTable dtMES = new DataTable();
                dtERP = result.Data.Tables["dtERP"];
                dtMES = result.Data.Tables["dtMES"];
                if (result.Data.Tables[0].Rows.Count > 0)
                {
                    model = new MaterialReceiptViewModel()
                    {
                        ReceiptNo = dtERP.Rows[0]["VBILLCODE"].ToString(),
                        OrderNumber = dtERP.Rows[0]["VPRODUCTBATCH"].ToString(),
                        ReceiptDate = DateTime.Parse(dtERP.Rows[0]["DBILLDATE"].ToString()),
                        LineStoreName = dtMES.Rows[0]["STORE_NAME"].ToString()
                    };
                }
                ViewBag.MaterialReceipt = model;

                return View(model);
            }
        }
       
    }
}