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

namespace ServiceCenter.Client.Mvc.Areas.FMM.Controllers
{
    public class MaterialPrintSetController : Controller
    {
        /// <summary>
        /// 初始化产品标签主页面
        /// </summary>
        /// <param name="materialCode">物料代码</param>
        /// <param name="matetalName">物料名称</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string materialCode, string materialName)
        {
            if (string.IsNullOrEmpty(materialCode))
            {
                return RedirectToAction("Index", "Material");
            }

            using (MaterialPrintSetServiceClient client = new MaterialPrintSetServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.MaterialCode='{0}'"
                                              , materialCode),
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<MaterialPrintSet>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }

            return View(new MaterialPrintSetQueryViewModel() { MaterialCode = materialCode, MaterialName = materialName });
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialPrintSetQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialPrintSetServiceClient client = new MaterialPrintSetServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" Key.MaterialCode='{0}'"
                                              , model.MaterialCode);

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<MaterialPrintSet>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }

            return PartialView("_ListPartial", new MaterialPrintSetQueryViewModel());
        }
        
        /// <summary>
        /// 分页处理
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="currentPageNo"></param>
        /// <param name="currentPageSize"></param>
        /// <returns></returns>
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

                using (MaterialPrintSetServiceClient client = new MaterialPrintSetServiceClient())
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
                        MethodReturnResult<IList<MaterialPrintSet>> result = client.Get(ref cfg);
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
        
        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialPrintSetViewModel model)
        {
            using (MaterialPrintSetServiceClient client = new MaterialPrintSetServiceClient())
            {
                MaterialPrintSet obj = new MaterialPrintSet()
                {
                    Key = new MaterialPrintSetKey()
                    {
                        MaterialCode = model.MaterialCode,
                        LabelCode = model.LabelCode
                    },                    
                    Qty = model.Qty,                    
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };

                MethodReturnResult rst = await client.AddAsync(obj);

                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.MaterialPrintSet_Save_Success
                                                , obj.Key);
                }

                return Json(rst);
            }
        }
        
        /// <summary>
        /// 修改标签设置
        /// </summary>
        /// <param name="MaterialCode"></param>
        /// <param name="labelCode"></param>
        /// <returns></returns>
        public async Task<ActionResult> Modify(string MaterialCode, string labelCode)
        {
            MaterialPrintSetViewModel viewModel = new MaterialPrintSetViewModel();

            using (MaterialPrintSetServiceClient client = new MaterialPrintSetServiceClient())
            {
                MethodReturnResult<MaterialPrintSet> result = await client.GetAsync(new MaterialPrintSetKey()
                {
                    MaterialCode = MaterialCode,
                    LabelCode = labelCode
                });

                if (result.Code == 0)
                {
                    viewModel = new MaterialPrintSetViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        LabelCode = result.Data.Key.LabelCode,
                        Qty = result.Data.Qty,
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

            return PartialView("_ModifyPartial", new MaterialPrintSetViewModel());
        }

        /// <summary>
        /// 保存修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(MaterialPrintSetViewModel model)
        {
            using (MaterialPrintSetServiceClient client = new MaterialPrintSetServiceClient())
            {
                MaterialPrintSet obj = new MaterialPrintSet()
                {
                    Key = new MaterialPrintSetKey()
                    {
                        MaterialCode = model.MaterialCode,
                        LabelCode = model.LabelCode
                    },
                    Qty = model.Qty,
                    CreateTime = model.CreateTime,
                    Creator = model.Creator,                    
                    EditTime = DateTime.Now,
                    Editor = User.Identity.Name,
                };

                MethodReturnResult result = await client.ModifyAsync(obj);

                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.MaterialPrintSet_SaveModify_Success
                                                , obj.Key);
                }

                return Json(result);
            }
        }
        
        /// <summary>
        /// 显示明细
        /// </summary>
        /// <param name="MaterialCode"></param>
        /// <param name="labelCode"></param>
        /// <returns></returns>
        public async Task<ActionResult> Detail(string MaterialCode, string labelCode)
        {
            using (MaterialPrintSetServiceClient client = new MaterialPrintSetServiceClient())
            {
                MaterialPrintSetKey key = new MaterialPrintSetKey()
                {
                    MaterialCode = MaterialCode,
                    LabelCode = labelCode
                };

                MethodReturnResult<MaterialPrintSet> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    MaterialPrintSetViewModel viewModel = new MaterialPrintSetViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        LabelCode = result.Data.Key.LabelCode,
                        Qty = result.Data.Qty,
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
        /// 删除标签打印设置
        /// </summary>
        /// <param name="MaterialCode"></param>
        /// <param name="labelCode"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string MaterialCode, string labelCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            MaterialPrintSetKey key = new MaterialPrintSetKey()
            {
                MaterialCode = MaterialCode,
                LabelCode = labelCode
            };
            using (MaterialPrintSetServiceClient client = new MaterialPrintSetServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.MaterialPrintSet_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }

        /// <summary>
        /// 取得产品打印标签数量
        /// </summary>
        /// <param name="materialCode">产品物料代码</param>
        /// <param name="labelCode">标签代码</param>
        /// <returns></returns>
        public ActionResult GetPrintLabelQty(string materialCode, string labelCode)        
        {
            MaterialPrintSetViewModel viewModel = new MaterialPrintSetViewModel();
            int qty = 0;

            using (MaterialPrintSetServiceClient client = new MaterialPrintSetServiceClient())
            {
                MethodReturnResult<MaterialPrintSet> result = client.Get(new MaterialPrintSetKey()
                {
                    MaterialCode = materialCode,
                    LabelCode = labelCode
                });

                if (result.Code == 0)
                {
                    qty = result.Data.Qty;                    
                }    
            }

            return Json(qty, JsonRequestBehavior.AllowGet);
        }
    }
}