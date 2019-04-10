using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Resources;
using System.Drawing;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class WorkOrderPowersetDetailController : Controller
    {
        //
        
        // GET: /ZPVM/WorkOrderPowersetDetail/
        public async Task<ActionResult> Index(string OrderNumber, string MaterialCode,string code,int? itemNo)
        {
            if (string.IsNullOrEmpty(code) || itemNo == null)
            {
                return RedirectToAction("Index", "WorkOrderPowerset");
            }

            using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            {
                WorkOrderPowersetKey key = new WorkOrderPowersetKey()
                {
                    MaterialCode=MaterialCode,
                    OrderNumber=OrderNumber,
                    Code = code,
                    ItemNo = itemNo.Value
                };
                MethodReturnResult<WorkOrderPowerset> result = await client.GetAsync(key);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "WorkOrderPowerset");
                }
                ViewBag.WorkOrderPowerset = result.Data;
            }

            using (WorkOrderPowersetDetailServiceClient client = new WorkOrderPowersetDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.Code = '{0}' AND Key.ItemNo='{1}' AND Key.MaterialCode='{2}' AND Key.OrderNumber='{3}'"
                                              , code
                                              , itemNo
                                              , MaterialCode
                                              , OrderNumber),
                        OrderBy = "Key.ItemNo"
                    };
                    MethodReturnResult<IList<WorkOrderPowersetDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new WorkOrderPowersetDetailQueryViewModel() { OrderNumber = OrderNumber, MaterialCode = MaterialCode, Code = code, ItemNo = itemNo.Value });
        }

        //
        //POST: /ZPVM/WorkOrderPowersetDetail/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderPowersetDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderPowersetDetailServiceClient client = new WorkOrderPowersetDetailServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.OrderNumber = '{1}'"
                                                   , where.Length > 0 ? "AND" : string.Empty
                                                   , model.OrderNumber);

                            where.AppendFormat(" {0} Key.MaterialCode = '{1}'"
                                                   , where.Length > 0 ? "AND" : string.Empty
                                                   , model.MaterialCode);

                            where.AppendFormat(" {0} Key.Code = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Code);

                            where.AppendFormat(" {0} Key.ItemNo = '{1}'"
                                                   , where.Length > 0 ? "AND" : string.Empty
                                                   , model.ItemNo);

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrderPowersetDetail>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial");
        }
        //
        //POST: /ZPVM/WorkOrderPowersetDetail/PagingQuery
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

                using (WorkOrderPowersetDetailServiceClient client = new WorkOrderPowersetDetailServiceClient())
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
                        MethodReturnResult<IList<WorkOrderPowersetDetail>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial");
        }
        //
        // POST: /ZPVM/WorkOrderPowersetDetail/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderPowersetDetailViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            if (model.Picture != null
              && !model.Picture.ContentType.Contains("image"))
            {
                rst.Code = 1000;
                rst.Message = string.Format(StringResource.ValidateImageFileFormat, ZPVMResources.StringResource.PowersetDetailViewModel_Picture);
                return Json(rst);
            }
            try 
            {
                using (WorkOrderPowersetDetailServiceClient client = new WorkOrderPowersetDetailServiceClient())
                {
                    WorkOrderPowersetDetail obj = new WorkOrderPowersetDetail()
                    {
                        Key = new WorkOrderPowersetDetailKey(){
                            OrderNumber=model.OrderNumber,
                            MaterialCode=model.MaterialCode,
                            Code = model.Code.ToUpper(),
                            ItemNo=model.ItemNo,
                            SubCode = model.SubCode
                        },
                        MaxValue = model.MaxValue,
                        MinValue = model.MinValue,
                        SubName = model.SubName,
                        IsUsed=model.IsUsed,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        Creator = User.Identity.Name
                    };
                    if (model.Picture != null
                                && model.Picture.ContentLength > 10)
                    {
                        int length = (int)model.Picture.InputStream.Length;
                        obj.Picture = new byte[length];
                        model.Picture.InputStream.Read(obj.Picture, 0, length);
                    }
                    rst = await client.AddAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderPowersetDetail_Save_Success
                                                    , obj.Key);
                    }
                    return Json(rst);
                }
            }
            catch (Exception ex)
            {
                rst.Code = 1000;
                rst.Message = ex.Message;
                rst.Detail = ex.ToString();
            }
            return Json(rst);
        }
        //
        // GET: /ZPVM/WorkOrderPowersetDetail/Modify
        public async Task<ActionResult> Modify(string OrderNumber, string MaterialCode,string code,int itemNo,string subCode)
        {
            WorkOrderPowersetDetailViewModel viewModel = new WorkOrderPowersetDetailViewModel();
            using (WorkOrderPowersetDetailServiceClient client = new WorkOrderPowersetDetailServiceClient())
            {
                MethodReturnResult<WorkOrderPowersetDetail> result = await client.GetAsync(new WorkOrderPowersetDetailKey()
                {
                    OrderNumber=OrderNumber,
                    MaterialCode=MaterialCode,
                    Code=code,
                    ItemNo = itemNo,
                    SubCode=subCode
                });
                if (result.Code == 0)
                {
                    viewModel = new WorkOrderPowersetDetailViewModel()
                    {
                        MaterialCode=result.Data.Key.MaterialCode,
                        OrderNumber=result.Data.Key.OrderNumber,
                        Code=result.Data.Key.Code,
                        ItemNo=result.Data.Key.ItemNo,
                        SubCode=result.Data.Key.SubCode, 
                        SubName=result.Data.SubName,
                        MaxValue=result.Data.MaxValue,
                        MinValue=result.Data.MinValue,
                        IsUsed=result.Data.IsUsed,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    return PartialView("_ModifyPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_ModifyPartial");
        }

        //
        // POST: /ZPVM/WorkOrderPowersetDetail/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderPowersetDetailViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            if (model.Picture != null
               && !model.Picture.ContentType.Contains("image"))
            {
                rst.Code = 1000;
                rst.Message = string.Format(StringResource.ValidateImageFileFormat, ZPVMResources.StringResource.PowersetDetailViewModel_Picture);
                return Json(rst);
            }
            try 
            {
                using (WorkOrderPowersetDetailServiceClient client = new WorkOrderPowersetDetailServiceClient())
                {
                    WorkOrderPowersetDetailKey key = new WorkOrderPowersetDetailKey()
                    {
                        MaterialCode=model.MaterialCode,
                        OrderNumber=model.OrderNumber,
                        Code = model.Code,
                        ItemNo=model.ItemNo,
                        SubCode=model.SubCode
                    };
                    MethodReturnResult<WorkOrderPowersetDetail> result = await client.GetAsync(key);

                    if (result.Code == 0)
                    {
                        result.Data.MaxValue = model.MaxValue;
                        result.Data.MinValue = model.MinValue;
                        result.Data.SubName = model.SubName;
                        result.Data.IsUsed = model.IsUsed;
                        result.Data.Editor = User.Identity.Name;
                        result.Data.EditTime = DateTime.Now;

                        if (model.IsDeletePicture)
                        {
                            result.Data.Picture = null;
                        }
                        else if (model.Picture != null && model.Picture.ContentLength > 10)
                        {
                            int length = (int)model.Picture.InputStream.Length;
                            result.Data.Picture = new byte[length];
                            model.Picture.InputStream.Read(result.Data.Picture, 0, length);
                        }

                        rst = await client.ModifyAsync(result.Data);
                        if (rst.Code == 0)
                        {
                            rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderPowersetDetail_SaveModify_Success
                                                        , model.Code);
                        }
                        return Json(rst);
                    }
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                rst.Code = 1000;
                rst.Message = ex.Message;
                rst.Detail = ex.ToString();
            }
            return Json(rst);
        }
        //
        // GET: /ZPVM/WorkOrderPowersetDetail/Detail
        public async Task<ActionResult> Detail(string OrderNumber, string MaterialCode, string code, int itemNo, string subCode)
        {
            using (WorkOrderPowersetDetailServiceClient client = new WorkOrderPowersetDetailServiceClient())
            {
                WorkOrderPowersetDetailKey key = new WorkOrderPowersetDetailKey()
                {
                    OrderNumber=OrderNumber,
                    MaterialCode=MaterialCode,
                    Code = code,
                    ItemNo = itemNo,
                    SubCode=subCode
                };
                MethodReturnResult<WorkOrderPowersetDetail> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    WorkOrderPowersetDetailViewModel viewModel = new WorkOrderPowersetDetailViewModel()
                    {
                        OrderNumber=OrderNumber,
                        MaterialCode=MaterialCode,
                        Code = result.Data.Key.Code,
                        ItemNo = result.Data.Key.ItemNo,
                        SubCode = result.Data.Key.SubCode,
                        SubName = result.Data.SubName,
                        MaxValue = result.Data.MaxValue,
                        MinValue = result.Data.MinValue,
                        IsUsed = result.Data.IsUsed,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    return PartialView("_InfoPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_InfoPartial");
        }
        /// <summary>
        /// //铭牌显示子分档(图片)
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="materialCode"></param>
        /// <param name="code"></param>
        /// <param name="itemNo"></param>
        /// <param name="subCode"></param>
        public void ShowPicture(string orderNumber,string materialCode,string code, int itemNo, string subCode)
        {
            HttpContext.Response.ContentType = "image/jpeg";
            try
            {
                using (WorkOrderPowersetDetailServiceClient client = new WorkOrderPowersetDetailServiceClient())
                {
                    MethodReturnResult<WorkOrderPowersetDetail> result = client.Get(new WorkOrderPowersetDetailKey()
                    {
                        OrderNumber=orderNumber,
                        MaterialCode=materialCode,
                        Code = code,
                        ItemNo = itemNo,
                        SubCode = subCode
                    });

                    if (result.Code == 0 && result.Data != null && result.Data.Picture != null)
                    {
                        HttpContext.Response.BinaryWrite(result.Data.Picture);
                    }
                    else
                    {
                        System.Drawing.Bitmap backgroudImage = new Bitmap(10, 10);

                        Graphics g = Graphics.FromImage(backgroudImage);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(25, 221, 237, 247)), new Rectangle(0, 0, 10, 10));
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        backgroudImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        HttpContext.Response.BinaryWrite(ms.ToArray());
                    }
                }
            }
            catch
            {
                System.Drawing.Bitmap backgroudImage = new Bitmap(10, 10);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                backgroudImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                HttpContext.Response.BinaryWrite(ms.ToArray());
            }
            HttpContext.Response.End();
        }

        //
        // POST: /ZPVM/WorkOrderPowersetDetail/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string OrderNumber, string MaterialCode, string code, int itemNo, string subCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            WorkOrderPowersetDetailKey key = new WorkOrderPowersetDetailKey()
            {
                MaterialCode=MaterialCode,
                OrderNumber=OrderNumber,
                Code = code,
                ItemNo = itemNo,
                SubCode=subCode
            };
            using (WorkOrderPowersetDetailServiceClient client = new WorkOrderPowersetDetailServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.WorkOrderPowersetDetail_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}