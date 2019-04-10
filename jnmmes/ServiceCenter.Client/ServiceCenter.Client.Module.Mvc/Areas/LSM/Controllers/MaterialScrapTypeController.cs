using ServiceCenter.Client.Mvc.Areas.LSM.Models;
using ServiceCenter.Client.Mvc.Resources.LSM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.LSM.Controllers
{
    public class MaterialScrapTypeController : Controller
    {
        // GET: LSM/MaterialScrapType
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Query(MaterialScrapTypeQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                MethodReturnResult<IList<MaterialScrap>> result1 = new MethodReturnResult<IList<MaterialScrap>>();
                MethodReturnResult<IList<MaterialScrapDetail>> result2 = new MethodReturnResult<IList<MaterialScrapDetail>>();
                MethodReturnResult<IList<MaterialScrapTypeViewModels>> result3 = new MethodReturnResult<IList<MaterialScrapTypeViewModels>>();
                List<MaterialScrapTypeViewModels> list = new List<MaterialScrapTypeViewModels>();
                using (MaterialScrapServiceClient client = new MaterialScrapServiceClient())
                {
                    StringBuilder where = new StringBuilder();

                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.BillCode))
                        {
                            where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.BillCode);
                        }
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "CreateTime desc",
                        Where = where.ToString()
                    };

                    result1 = client.Get(ref cfg);
                }
                using (MaterialScrapServiceClient client = new MaterialScrapServiceClient())
                {
                    StringBuilder where = new StringBuilder();

                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.BillCode))
                        {
                            where.AppendFormat(" {0} Key.ScrapNo LIKE '{1}%'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.BillCode);
                        }
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "CreateTime desc",
                        Where = where.ToString()
                    };

                    result2 = client.GetDetail(ref cfg);
                    if (result2.Code == 0)
                    {
                        foreach (var data in result1.Data)
                        {
                            MaterialScrapTypeViewModels mst = new MaterialScrapTypeViewModels();
                            mst.BillCode = data.Key;
                            mst.OrderNumber = data.OrderNumber;
                            mst.ScrapDate = data.ScrapDate;
                            mst.ScrapType = data.Type;
                            mst.Description = data.Description;
                            foreach (var item in result2.Data)
                            {
                                if(mst.BillCode==item.Key.ScrapNo)
                                {
                                    mst.MaterialCode = item.MaterialCode;
                                    mst.MaterialLot = item.MaterialLot;
                                    mst.Qty = item.Qty;
                                    mst.LineStoreName = item.LineStoreName;
                                    break;
                                }                            
                            }
                            list.Add(mst);
                        }
                        if (list.Count > 0)
                        {
                            result3.Data = list;
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result3.Data;
                        }
                    }
                }

            }
            return PartialView("_ListPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(MaterialScrapTypeViewModels model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (string.IsNullOrWhiteSpace(model.BillCode))
                {
                    string BillCode = string.Format("SC{0:yyMMdd}", DateTime.Now);
                    int itemNo = 0;
                    using (MaterialScrapServiceClient client = new MaterialScrapServiceClient())
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key LIKE '{0}%'"
                                                    , BillCode),
                            OrderBy = "Key Desc"
                        };
                        MethodReturnResult<IList<MaterialScrap>> rst = client.Get(ref cfg);
                        if (rst.Code <= 0 && rst.Data.Count > 0)
                        {
                            string maxBillNo = rst.Data[0].Key.Replace(BillCode, "");
                            int.TryParse(maxBillNo, out itemNo);
                        }
                        itemNo++;

                        model.BillCode = BillCode + itemNo.ToString("000");
                    }
                    model.BillCode = model.BillCode.ToUpper();
                }

                using (MaterialScrapServiceClient client = new MaterialScrapServiceClient())
                {
                    IList<MaterialScrapDetail> lstMS = new List<MaterialScrapDetail>();
                    lstMS.Clear();
                    MaterialScrap woReport = new MaterialScrap()
                    {
                        Key = model.BillCode,
                        ScrapDate = model.ScrapDate,

                        Type = EnumScrapType.Normal,
                        OrderNumber = model.OrderNumber,
                        Editor = User.Identity.Name,
                        Creator = User.Identity.Name,
                        Description = model.Description,
                    };
                    MaterialScrapDetailKey msdKey = new MaterialScrapDetailKey()
                    {
                        ScrapNo = model.BillCode,
                        ItemNo = 1
                    };
                    MaterialScrapDetail MSDetail = new MaterialScrapDetail()
                    {
                        Key = msdKey,
                        MaterialCode = model.MaterialCode,
                        MaterialLot = model.MaterialLot,
                        OrderNumber = model.OrderNumber,
                        Qty = model.Qty,
                        LineStoreName = model.LineStoreName,
                        Description = model.Description,
                        Editor = User.Identity.Name,
                        Creator = User.Identity.Name,
                    };
                    lstMS.Add(MSDetail);
                    result = client.Add(woReport, lstMS);
                    if (result.Code == 0)
                    {
                        result.Message = string.Format(StringResource.MaterialScrap_Save_Success, model.BillCode);
                        result.Detail = woReport.Key;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return Json(result);
        }

        public ActionResult Modify(string key)
        {
            MaterialScrapTypeViewModels model = new MaterialScrapTypeViewModels();

            using (MaterialScrapServiceClient client1 = new MaterialScrapServiceClient())
            {
                MaterialScrapDetailKey msdKey = new MaterialScrapDetailKey()
                {
                    ScrapNo = key,
                    ItemNo = 1
                };
                MethodReturnResult<MaterialScrapDetail> result1 = client1.GetDetail(msdKey);
                model.MaterialCode = result1.Data.MaterialCode;
                model.MaterialLot = result1.Data.MaterialLot;
                model.Qty = result1.Data.Qty;
                model.LineStoreName = result1.Data.LineStoreName;
            }
            using (MaterialScrapServiceClient client = new MaterialScrapServiceClient())
            {
                MethodReturnResult<MaterialScrap> result = client.Get(key);
                if (result.Code == 0)
                {
                    model.BillCode = result.Data.Key;
                    model.ScrapDate = result.Data.ScrapDate;
                    model.ScrapType = result.Data.Type;
                    model.OrderNumber = result.Data.OrderNumber;
                    model.Creator = result.Data.Creator;
                    model.Editor = User.Identity.Name;
                    model.Description = result.Data.Description;

                    return PartialView("_ModifyPartial", model);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            return PartialView("_ModifyPartial");
        }

        [HttpPost]
        public ActionResult SaveModify(MaterialScrapTypeViewModels model)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialScrapServiceClient client = new MaterialScrapServiceClient())
            {
                MaterialScrap woReport = new MaterialScrap()
                {
                    Key = model.BillCode,
                    ScrapDate = model.ScrapDate,
                    Type = model.ScrapType,
                    OrderNumber = model.OrderNumber,
                    Creator = model.Creator,
                    Editor = User.Identity.Name,
                    Description = model.Description,
                };

                //result = client.EditWOReport(woReport);

                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.MaterialScrap_Edit_Success, model.BillCode);
                }

            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialScrapServiceClient client = new MaterialScrapServiceClient())
            {
                result = client.Delete(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.MaterialScrap_Delete_Success, key);
                }
            }

            return Json(result);
        }

        public ActionResult GetOrderNumbers()
        {
            IList<WorkOrder> lstWorkOrder = new List<WorkOrder>();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"CloseType='{0}' "
                                           , Convert.ToInt32(EnumCloseType.None))
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lstWorkOrder = result.Data;
                }
            }
            return Json(from item in lstWorkOrder
                        select new
                        {
                            Text = item.Key,
                            Value = item.Key
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialCodes(string orderNumber)
        {
            //根据物料类型获取物料。
            //IList<WorkOrder> lst = new List<WorkOrder>();
            IList<LineStoreMaterialDetail> lstMaterial = null;

            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.OrderNumber='{0}' "
                                           , orderNumber)
                };
                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstMaterial = result.Data;
                }
            }
            var lnq = from item in lstMaterial
                      select item.Key.MaterialCode;

            return Json(lnq.Distinct(), JsonRequestBehavior.AllowGet);
        }
    }
}