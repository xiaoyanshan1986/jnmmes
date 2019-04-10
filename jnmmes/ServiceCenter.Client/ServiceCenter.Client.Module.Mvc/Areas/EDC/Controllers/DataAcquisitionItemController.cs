using ServiceCenter.Client.Mvc.Areas.EDC.Models;
using ServiceCenter.Client.Mvc.Resources.EDC;
using EDCResources = ServiceCenter.Client.Mvc.Resources.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Client.EDC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Controllers
{
    public class DataAcquisitionItemController : Controller
    {
        //
        // GET: /EDC/DataAcquisitionItem/
        public async Task<ActionResult> Index()
        {
            //using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
            //{
            //    await Task.Run(() =>
            //    {
            //        PagingConfig cfg = new PagingConfig()
            //        {
            //            OrderBy = "Key"
            //        };

            //        MethodReturnResult<IList<DataAcquisitionItem>> result = client.Get(ref cfg);

            //        if (result.Code == 0)
            //        {
            //            ViewBag.PagingConfig = cfg;
            //            ViewBag.List = result.Data;
            //        }
            //    });
            //}

            //return View(new DataAcquisitionItemQueryViewModel());

            return View(new DataAcquisitionItemQueryViewModel() { RouteStepName = ""});
        }

        //
        //POST: /EDC/DataAcquisitionItem/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(DataAcquisitionItemQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();

                        //项目代码
                        if (!string.IsNullOrEmpty(model.ItemCode))
                        {
                            where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.ItemCode);
                        }

                        //工序条件
                        if (model.RouteStepName != null && model.RouteStepName != "")
                        {
                            where.AppendFormat(" {0} RouteStepName = '{1}'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.RouteStepName);
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };

                        MethodReturnResult<IList<DataAcquisitionItem>> result = client.Get(ref cfg);

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
        /// 分页处理程序
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

                using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
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
                        MethodReturnResult<IList<DataAcquisitionItem>> result = client.Get(ref cfg);
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
        /// 保存项目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(DataAcquisitionItemViewModel model)
        {
            using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
            {
                DataAcquisitionItem obj = new DataAcquisitionItem()
                {
                    Key = model.ItemCode,                   //项目代码
                    ItemName = model.ItemName,              //项目名称
                    Description = model.Description,        //描述
                    RouteStepName = model.RouteStepName,    //工序名称                    
                    Creator = User.Identity.Name,           //创建人
                    CreateTime = DateTime.Now,              //创建日期
                    Editor = User.Identity.Name,            //编辑人
                    EditTime = DateTime.Now                 //编辑日期     
                };

                MethodReturnResult rst = await client.AddAsync(obj);

                if (rst.Code == 0)
                {
                    rst.Message = string.Format(StringResource.DataAcquisitionItem_Save_Success
                                                , model.ItemCode);
                }

                return Json(rst);
            }
        }

        /// <summary>
        /// 需改项目
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ActionResult> Modify(string itemCode)
        {
            using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
            {
                MethodReturnResult<DataAcquisitionItem> result = await client.GetAsync(itemCode);

                if (result.Code == 0)
                {
                    DataAcquisitionItemViewModel viewModel = new DataAcquisitionItemViewModel()
                    {
                        ItemCode = result.Data.Key,                     //项目代码
                        ItemName = result.Data.ItemName,                //项目名称
                        Description = result.Data.Description,          //描述
                        RouteStepName = result.Data.RouteStepName,      //工序名称
                        Creator = result.Data.Creator,                  //创建人
                        CreateTime = result.Data.CreateTime,            //创建日期
                        Editor = result.Data.Editor,                    //编辑人
                        EditTime = result.Data.EditTime                 //编辑日期
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

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(DataAcquisitionItemViewModel model)
        {
            using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
            {
                MethodReturnResult<DataAcquisitionItem> result = await client.GetAsync(model.ItemCode);

                if (result.Code == 0)
                {
                    result.Data.Key = model.ItemCode;                   //项目代码
                    result.Data.ItemName = model.ItemName;              //项目名称
                    result.Data.Description = model.Description;        //项目描述
                    result.Data.RouteStepName = model.RouteStepName;    //工序名称
                    result.Data.Editor = User.Identity.Name;            //编辑人
                    result.Data.EditTime = DateTime.Now;                //编辑日期

                    MethodReturnResult rst = await client.ModifyAsync(result.Data);

                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(StringResource.DataAcquisitionItem_SaveModify_Success
                                                    , model.ItemCode);
                    }

                    return Json(rst);
                }

                return Json(result);
            }
        }

        /// <summary>
        /// 确定明细信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ActionResult> Detail(string itemCode)
        {
            using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
            {
                MethodReturnResult<DataAcquisitionItem> result = await client.GetAsync(itemCode);
                if (result.Code == 0)
                {
                    DataAcquisitionItemViewModel viewModel = new DataAcquisitionItemViewModel()
                    {
                        ItemCode = result.Data.Key,             //项目代码
                        ItemName = result.Data.ItemName,        //项目名称
                        Description = result.Data.Description,  //描述
                        RouteStepName = result.Data.RouteStepName,      //工序名称
                        Creator = result.Data.Creator,          //创建人
                        CreateTime = result.Data.CreateTime,    //创建日期   
                        Editor = result.Data.Editor,            //编辑人
                        EditTime = result.Data.EditTime         //编辑日期
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
        /// 删除项目
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string itemCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
            {
                result = await client.DeleteAsync(itemCode);

                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.DataAcquisitionItem_Delete_Success
                                                    , itemCode);
                }

                return Json(result);
            }
        }

        ////录入数据操作设置
        public async Task<ActionResult> IndexInput()
        {
            return View(new DataAcquisitionItemQueryViewModel() { RouteStepName = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InputQuery(DataAcquisitionItemQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();

                        //项目代码
                        if (!string.IsNullOrEmpty(model.ItemCode))
                        {
                            where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.ItemCode);
                        }

                        //工序条件
                        if (model.RouteStepName != null && model.RouteStepName != "")
                        {
                            where.AppendFormat(" {0} RouteStepName = '{1}'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.RouteStepName);
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };

                        MethodReturnResult<IList<DataAcquisitionItem>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_InputListPartial");
        }
    }
}