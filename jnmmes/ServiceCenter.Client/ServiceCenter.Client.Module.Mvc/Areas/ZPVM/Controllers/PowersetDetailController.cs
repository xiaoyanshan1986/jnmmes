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
    public class PowersetDetailController : Controller
    {
        //
        // GET: /ZPVM/PowersetDetail/
        public async Task<ActionResult> Index(string code,int? itemNo)
        {
            if (string.IsNullOrEmpty(code) || itemNo == null)
            {
                return RedirectToAction("Index", "Powerset");
            }

            using (PowersetServiceClient client = new PowersetServiceClient())
            {
                PowersetKey key = new PowersetKey()
                {
                    Code = code,
                    ItemNo = itemNo.Value
                };
                MethodReturnResult<Powerset> result = await client.GetAsync(key);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "Powerset");
                }
                ViewBag.Powerset = result.Data;
            }

            using (PowersetDetailServiceClient client = new PowersetDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.Code = '{0}' AND Key.ItemNo='{1}'"
                                              , code
                                              , itemNo),
                        OrderBy = "Key.ItemNo"
                    };
                    MethodReturnResult<IList<PowersetDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new PowersetDetailQueryViewModel() { Code = code,ItemNo=itemNo.Value });
        }

        //
        //POST: /ZPVM/PowersetDetail/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(PowersetDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (PowersetDetailServiceClient client = new PowersetDetailServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.Code = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Code);

                            where.AppendFormat(" {0} Key.ItemNo = '{1}'"
                                                   , where.Length > 0 ? "AND" : string.Empty
                                                   , model.ItemNo);

                            if (!string.IsNullOrEmpty(model.SubCode))
                            {
                                where.AppendFormat(" {0} Key.SubCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.SubCode);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<PowersetDetail>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/PowersetDetail/PagingQuery
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

                using (PowersetDetailServiceClient client = new PowersetDetailServiceClient())
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
                        MethodReturnResult<IList<PowersetDetail>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/PowersetDetail/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(PowersetDetailViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            if (model.Picture != null
              && !model.Picture.ContentType.Contains("image"))
            {
                rst.Code = 1000;
                rst.Message = string.Format(StringResource.ValidateImageFileFormat, ZPVMResources.StringResource.PowersetDetailViewModel_Picture);
                return Json(rst);
            }
            if (model.Picture.ContentLength > 6000)
            {
                rst.Code = 1000;
                rst.Message = "上传图片较大，请重新确认图片大小！";
                return Json(rst);
            }
            try 
            {
                using (PowersetDetailServiceClient client = new PowersetDetailServiceClient())
                {
                    PowersetDetail obj = new PowersetDetail()
                    {
                        Key = new PowersetDetailKey(){
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

                    if (model.Picture != null && model.Picture.ContentLength > 10 && model.Picture.ContentLength < 6000)
                    {
                        int length = (int)model.Picture.InputStream.Length;
                        obj.Picture = new byte[length];
                        model.Picture.InputStream.Read(obj.Picture, 0, length);
                    }

                    rst = await client.AddAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.PowersetDetail_Save_Success
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
        // GET: /ZPVM/PowersetDetail/Modify
        public async Task<ActionResult> Modify(string code,int itemNo,string subCode)
        {
            PowersetDetailViewModel viewModel = new PowersetDetailViewModel();
            using (PowersetDetailServiceClient client = new PowersetDetailServiceClient())
            {
                MethodReturnResult<PowersetDetail> result = await client.GetAsync(new PowersetDetailKey()
                {
                    Code=code,
                    ItemNo = itemNo,
                    SubCode=subCode
                });
                if (result.Code == 0)
                {
                    viewModel = new PowersetDetailViewModel()
                    {
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
        // POST: /ZPVM/PowersetDetail/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(PowersetDetailViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            if (model.Picture != null && !model.Picture.ContentType.Contains("image"))
            {
                rst.Code = 1000;
                rst.Message = string.Format(StringResource.ValidateImageFileFormat, ZPVMResources.StringResource.PowersetDetailViewModel_Picture);
                return Json(rst);
                
            }
            if (model.Picture.ContentLength > 6000)
            {
                rst.Code = 1000;
                rst.Message = "上传图片较大，请重新确认图片大小！";
                return Json(rst);
            }
            try 
            {
                using (PowersetDetailServiceClient client = new PowersetDetailServiceClient())
                {
                   
                    PowersetDetailKey key = new PowersetDetailKey()
                    {
                        Code = model.Code,
                        ItemNo=model.ItemNo,
                        SubCode=model.SubCode
                    };
                    MethodReturnResult<PowersetDetail> result = await client.GetAsync(key);

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
                        else if (model.Picture != null && model.Picture.ContentLength > 10 )
                        {
                            int length = (int)model.Picture.InputStream.Length;
                            result.Data.Picture = new byte[length];
                            model.Picture.InputStream.Read(result.Data.Picture, 0, length);
                        }

                        rst = await client.ModifyAsync(result.Data);
                        if (rst.Code == 0)
                        {
                            rst.Message += string.Format(ZPVMResources.StringResource.PowersetDetail_SaveModify_Success
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
        // GET: /ZPVM/PowersetDetail/Detail
        public async Task<ActionResult> Detail(string code, int itemNo, string subCode)
        {
            using (PowersetDetailServiceClient client = new PowersetDetailServiceClient())
            {
                PowersetDetailKey key = new PowersetDetailKey()
                {
                    Code = code,
                    ItemNo = itemNo,
                    SubCode=subCode
                };
                MethodReturnResult<PowersetDetail> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    PowersetDetailViewModel viewModel = new PowersetDetailViewModel()
                    {
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

        public void ShowPicture(string code,int itemNo,string subCode)
        {
            HttpContext.Response.ContentType = "image/jpeg";
            try
            {
                using (PowersetDetailServiceClient client = new PowersetDetailServiceClient())
                {
                    MethodReturnResult<PowersetDetail> result = client.Get(new PowersetDetailKey()
                    {
                        Code=code,
                        ItemNo=itemNo,
                        SubCode=subCode
                    });

                    if (result.Code == 0 && result.Data != null && result.Data.Picture != null )
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
        // POST: /ZPVM/PowersetDetail/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string code, int itemNo, string subCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            PowersetDetailKey key = new PowersetDetailKey()
            {
                Code = code,
                ItemNo = itemNo,
                SubCode=subCode
            };
            using (PowersetDetailServiceClient client = new PowersetDetailServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.PowersetDetail_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}