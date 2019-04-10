using ServiceCenter.Client.Mvc.Areas.FMM.Models;
using FMMResources = ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;
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

namespace ServiceCenter.Client.Mvc.Areas.FMM.Controllers
{
    public class EquipmentLayoutController : Controller
    {

        //
        // GET: /FMM/EquipmentLayout/
        public async Task<ActionResult> Index()
        {
            using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<EquipmentLayout>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new EquipmentLayoutQueryViewModel());
        }

        //
        //POST: /FMM/EquipmentLayout/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(EquipmentLayoutQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<EquipmentLayout>> result = client.Get(ref cfg);

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
        //POST: /FMM/EquipmentLayout/PagingQuery
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

                using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
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
                        MethodReturnResult<IList<EquipmentLayout>> result = client.Get(ref cfg);
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
        // POST: /FMM/EquipmentLayout/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(EquipmentLayoutViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            if (model.BackgroundImage != null
              && !model.BackgroundImage.ContentType.Contains("image"))
            {
                rst.Code = 1000;
                rst.Message = string.Format(StringResource.ValidateImageFileFormat, FMMResources.StringResource.EquipmentLayoutViewModel_BackgroundImage);
                return Json(rst);
            }
            try 
            {
                using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
                {
                    EquipmentLayout obj = new EquipmentLayout()
                    {
                        Key = model.Name,
                        Height = model.Height,
                        Width = model.Width,
                        Status = model.Status,
                        Description = model.Description,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        Creator = User.Identity.Name
                    };
                    if (model.BackgroundImage != null
                        && model.BackgroundImage.ContentLength > 10)
                    {
                        int length = (int)model.BackgroundImage.InputStream.Length;
                        obj.BackgroundImage = new byte[length];
                        model.BackgroundImage.InputStream.Read(obj.BackgroundImage, 0, length);
                    }
                    rst = await client.AddAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.EquipmentLayout_Save_Success
                                                    , model.Name);
                    }
                }
            }
            catch(Exception ex)
            {
                rst.Code = 1000;
                rst.Message = ex.Message;
                rst.Detail = ex.ToString();
            }
            return Json(rst);
            
        }
        //
        // GET: /FMM/EquipmentLayout/Modify
        public async Task<ActionResult> Modify(string key)
        {
            EquipmentLayoutViewModel viewModel = new EquipmentLayoutViewModel();
            using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
            {
                MethodReturnResult<EquipmentLayout> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new EquipmentLayoutViewModel()
                    {
                        Name = result.Data.Key,
                        Width=result.Data.Width,
                        Height=result.Data.Height,
                        Status=result.Data.Status,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
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
        // POST: /FMM/EquipmentLayout/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(EquipmentLayoutViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            if(model.BackgroundImage!=null
               &&!model.BackgroundImage.ContentType.Contains("image"))
            {
                rst.Code = 1000;
                rst.Message = string.Format(StringResource.ValidateImageFileFormat, FMMResources.StringResource.EquipmentLayoutViewModel_BackgroundImage);
                return Json(rst);
            }
            try 
            {
                using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
                {
                    MethodReturnResult<EquipmentLayout> result = await client.GetAsync(model.Name);

                    if (result.Code == 0)
                    {
                        result.Data.Height = model.Height;
                        result.Data.Width = model.Width;
                        result.Data.Status = model.Status;
                        result.Data.Description = model.Description;
                        result.Data.Editor = User.Identity.Name;
                        result.Data.EditTime = DateTime.Now;
                        if(model.IsDeleteBackgroudImage)
                        {
                            result.Data.BackgroundImage = null;
                        }
                        else if (model.BackgroundImage!=null && model.BackgroundImage.ContentLength > 10)
                        {
                            int length = (int)model.BackgroundImage.InputStream.Length;
                            result.Data.BackgroundImage = new byte[length];
                            model.BackgroundImage.InputStream.Read(result.Data.BackgroundImage, 0, length);
                        }
                        rst = await client.ModifyAsync(result.Data);
                        if (rst.Code == 0)
                        {
                            rst.Message = string.Format(FMMResources.StringResource.EquipmentLayout_SaveModify_Success
                                                        , model.Name);
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
        // GET: /FMM/EquipmentLayout/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
            {
                MethodReturnResult<EquipmentLayout> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    EquipmentLayoutViewModel viewModel = new EquipmentLayoutViewModel()
                    {
                        Name = result.Data.Key,
                        Width=result.Data.Width,
                        Height=result.Data.Height,
                        Status=result.Data.Status,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
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

        public void ShowBackgroudImage(string key)
        {
            HttpContext.Response.ContentType = "image/jpeg";
            try
            {
                using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
                {
                    MethodReturnResult<EquipmentLayout> result = client.Get(key);

                    if (result.Code == 0 && result.Data!=null && result.Data.BackgroundImage!=null)
                    {
                        HttpContext.Response.BinaryWrite(result.Data.BackgroundImage);
                    }
                    else
                    {
                        System.Drawing.Bitmap backgroudImage = new Bitmap(10,10);
                        
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
                System.IO.MemoryStream ms=new System.IO.MemoryStream();
                backgroudImage.Save(ms,System.Drawing.Imaging.ImageFormat.Jpeg);
                HttpContext.Response.BinaryWrite(ms.ToArray());
            }
            HttpContext.Response.End();
        }

        //
        // POST: /FMM/EquipmentLayout/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.EquipmentLayout_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}