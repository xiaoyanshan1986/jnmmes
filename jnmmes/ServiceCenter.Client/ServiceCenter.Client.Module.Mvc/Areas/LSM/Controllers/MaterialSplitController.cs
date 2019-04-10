using ServiceCenter.Client.Mvc.Areas.FMM.Models;
using ServiceCenter.Client.Mvc.Areas.LSM.Models;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Contract.LSM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.LSM.Controllers
{
    public class MaterialSplitController : Controller
    {
        //
        //
        // POST: /WIP/MaterialSplit/Index
        public async Task<ActionResult> Index()
        {
            return View(new MaterialSplitQueryViewModel());
        }

        // POST: /WIP/MaterialSplit/QueryQuantity
        [HttpPost]
        public async Task<ActionResult> QueryQuantity(String MaterialNumber)
        {
            double Quantity = 0;
            MaterialQueryViewModel model = new MaterialQueryViewModel();
            if (ModelState.IsValid)
            {
                using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            Where = string.Format("  Key.MaterialLot Like  '{0}'"
                                        , MaterialNumber)
                        };
                        MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);

                        if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                        {
                            Quantity = result.Data[0].CurrentQty;
                        }
                        else
                        {

                            return;
                        }
                    });
                }
            }
            return Json(Quantity);
        }



        //POST: /WIP/MaterialSplit/Query
        [HttpPost]

        public async Task<ActionResult> Query(MaterialSplitQueryViewModel MaterialSplitQueryViewModel)
        {
            MethodReturnResult<IList<LineStoreMaterialDetail>> result = new MethodReturnResult<IList<LineStoreMaterialDetail>>();
            MaterialSplitViewModel model = new MaterialSplitViewModel();
            LineStoreMaterialDetail lsmd = new LineStoreMaterialDetail();
            model.count = MaterialSplitQueryViewModel.count;
            model.ParentMaterialLotNumber = MaterialSplitQueryViewModel.MaterialLotNumber;
            model.OrderNumber = MaterialSplitQueryViewModel.OrderNumber;
            if (ModelState.IsValid)
            {
                using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            Where = string.Format("  Key.MaterialLot LIKE  '{0}' AND Key.OrderNumber='{1}'"
                                        , MaterialSplitQueryViewModel.MaterialLotNumber, MaterialSplitQueryViewModel.OrderNumber)
                        };
                        result = client.GetDetail(ref cfg);
                        if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                        {
                            ViewBag.ParentMaterial = result.Data[0];
                            lsmd = result.Data[0];
                            ViewBag.ParentCurrentQty = lsmd.CurrentQty;
                            ViewBag.CurrentQty = lsmd.CurrentQty / MaterialSplitQueryViewModel.count;

                            for (int i = 0; i < model.count; i++)
                            {
                                model.ChildViewModel.Add(new ChildViewModel()
                                {
                                    Quantity = lsmd.CurrentQty / MaterialSplitQueryViewModel.count
                                });
                            }
                        }
                        else
                        {
                            ViewBag.flag = 0;
                            ViewBag.Message = "批次不存在";
                        }
                    });
                }
            }

            return View("_MaterialSplit", model);
        }
        //POST: /WIP/MaterialSplit/Query
        [HttpPost]

        public async Task<ActionResult> ExecuteSplitMaterial(MaterialSplitViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (ModelState.IsValid)
            {
                using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
                {
                    await Task.Run(() =>
                    {

                        SplitMaterialLotParameter sparam = new SplitMaterialLotParameter();
                        List<ChildMaterialLotParameter> lstChildMaterials = new List<ChildMaterialLotParameter>();
                        ChildMaterialLotParameter child = new ChildMaterialLotParameter();
                        sparam.ParentMaterialLotNumber = model.ParentMaterialLotNumber;
                        sparam.OrderNumber = model.OrderNumber;
                        sparam.count = model.count;
                        sparam.Creator = User.Identity.Name;
                        sparam.Operator = User.Identity.Name;
                        for (int i = 0; i < model.count; i++)
                        {
                            child = new ChildMaterialLotParameter();
                            child.MaterialLotNumber = model.ChildViewModel[i].ChildMaterialLotNumber;
                            child.Quantity = model.ChildViewModel[i].Quantity;
                            lstChildMaterials.Add(child);

                        }
                        sparam.ChildMaterialLot = lstChildMaterials;
                        result = client.SplitMaterialLot(sparam);
                        if (result.Code == 0)
                        {
                            result.Message = "保存成功";
                        }
                    });

                };
            }

            return Json(result);

        }

        public ActionResult GetOrderNumber(string MaterialLotNumber)
        {
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                if (!string.IsNullOrEmpty(MaterialLotNumber))
                {
                    MethodReturnResult<DataSet> rst = client.GetOrderNumberByMaterialLot(MaterialLotNumber);
                    if (rst.Code <= 0)
                    {
                        {
                            var data = from item in rst.Data.Tables[0].AsEnumerable()
                                     select item["ORDER_NUMBER"].ToString();
                                                                


                            return Json(data, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return null;
            }

        }

    }
}