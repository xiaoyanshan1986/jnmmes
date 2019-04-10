using ServiceCenter.Client.Mvc.Areas.PPM.Models;
using PPMResources = ServiceCenter.Client.Mvc.Resources.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.MES.Service.Client.ERP;
using System.Data;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Controllers
{
    public class WorkOrderGroupDetailController : Controller
    {
        //
        // GET: /PPM/WorkOrderGroupDetail/
        public async Task<ActionResult> Index()
        {
            using (WorkOrderGroupDetailServiceClient client = new WorkOrderGroupDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Nums Desc,ItemNo Asc"
                    };
                    MethodReturnResult<IList<WorkOrderGroupDetail>> result = client.Gets(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new WorkOrderGroupQueryDetailViewModel());
        }

        public ActionResult Show()
        {
            //Response.StatusDescription = "Partial";
            return PartialView("_AddPartialEverNo", new WorkOrderGroupDetailViewModel());
        }

        public ActionResult Show1()
        {
            //Response.StatusDescription = "Partial";
            return PartialView("_AddPartial", new WorkOrderGroupDetailViewModel());
        }

        //
        //POST: /PPM/WorkOrderGroupDetail/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderGroupQueryDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderGroupDetailServiceClient client = new WorkOrderGroupDetailServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.ProductCode))
                            {
                                where.AppendFormat(" {0} Key.ProductCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ProductCode.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.OrderNumber))
                            {
                                where.AppendFormat(" {0} Key.OrderNumber LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.WorkOrderGroupNo))
                            {
                                where.AppendFormat(" {0} Key.WorkOrderGroupNo LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.WorkOrderGroupNo.ToString().Trim().ToUpper());
                            }

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Nums Desc,ItemNo Asc",                           
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrderGroupDetail>> result = client.Gets(ref cfg);

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
        //POST: /PPM/WorkOrderGroupDetail/PagingQuery
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

                using (WorkOrderGroupDetailServiceClient client = new WorkOrderGroupDetailServiceClient())
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
                        MethodReturnResult<IList<WorkOrderGroupDetail>> result = client.Gets(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                            //ViewBag.GroupNoList = GetWorkOrderGroupNoList();
                            //ViewBag.GroupNo = SetWorkOrderGroupNo();
                        }
                    });
                }
            }
            return PartialView("_ListPartial");
        }

        //获取工单号
        public ActionResult GetOrderNumber(string q)
        {
            IList<WorkOrder> lstDetail = new List<WorkOrder>();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key LIKE '{0}%' AND OrderState='0' AND CloseType=0"
                                           , q),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            var lnq = from item in lstDetail
                      select item.Key;

            return Json(from item in lstDetail
                        select new
                        {
                            @label = item.Key + "-" + item.MaterialCode,
                            @value = item.Key,
                            @ProductCode = item.MaterialCode
                        }, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /PPM/WorkOrderGroupDetail/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderGroupDetailViewModel model)
        {            
            DateTime now = DateTime.Now;
            MethodReturnResult rst = new MethodReturnResult();
            if (model.WorkOrderGroupNo == "" ||model.WorkOrderGroupNo ==null)
            {
                rst.Code = 1000;
                rst.Message = "混工单组号不可为空!";
                return Json(rst);
            }
            if (model.OrderNumber == ""||model.OrderNumber ==null)
            {
                rst.Code = 1000;
                rst.Message = "工单不可为空!";
                return Json(rst);
            }
            if (model.ProductCode == ""||model.ProductCode ==null)
            {
                rst.Code = 1000;
                rst.Message = "产品编码不可为空!";
                return Json(rst);
            }

            using (WorkOrderServiceClient client0 = new WorkOrderServiceClient())
            {
                PagingConfig cfg0 = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key LIKE '{0}%' AND OrderState='0' AND CloseType=0"
                                           , model.OrderNumber.Trim().ToUpper()),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<WorkOrder>> result0 = client0.Get(ref cfg0);
                if (result0.Code <= 0 && result0.Data != null)
                {
                    if (model.ProductCode.Trim().ToUpper() == result0.Data[0].MaterialCode.ToString())
                    {
                        #region 新增规则
                        using (WorkOrderGroupDetailServiceClient client = new WorkOrderGroupDetailServiceClient())
                        {
                            //获取混工单组规则中该组的最大序列号
                            StringBuilder where = new StringBuilder();
                            if (model != null)
                            {
                                if (!string.IsNullOrEmpty(model.WorkOrderGroupNo))
                                {
                                    where.AppendFormat(" {0} Key.WorkOrderGroupNo LIKE '{1}%'"
                                                        , where.Length > 0 ? "AND" : string.Empty
                                                        , model.WorkOrderGroupNo.ToString().Trim().ToUpper());
                                }

                            }
                            PagingConfig cfg = new PagingConfig()
                            {
                                OrderBy = "ItemNo Desc",
                                Where = where.ToString()
                            };
                            MethodReturnResult<IList<WorkOrderGroupDetail>> result = client.Gets(ref cfg);
                            if (result != null && result.Data.Count > 0)
                            {
                                //判断新增工单是否已存在至混工单组
                                StringBuilder where1 = new StringBuilder();
                                if (model != null)
                                {
                                    if (!string.IsNullOrEmpty(model.WorkOrderGroupNo))
                                    {
                                        where1.AppendFormat(" {0} Key.OrderNumber = '{1}'"
                                                            , where1.Length > 0 ? "AND" : string.Empty
                                                            , model.OrderNumber.ToString().Trim().ToUpper());
                                    }

                                }
                                PagingConfig cfg1 = new PagingConfig()
                                {
                                    //OrderBy = "ItemNo Desc",
                                    Where = where1.ToString()
                                };
                                MethodReturnResult<IList<WorkOrderGroupDetail>> result1 = client.Gets(ref cfg1);
                                if (result1 != null && result1.Data.Count > 0)
                                {
                                    rst.Code = 1000;
                                    rst.Message = string.Format("新增工单{0}已存在至混工单组{1}中!"
                                                                , model.OrderNumber.Trim().ToUpper()
                                                                , result1.Data[0].Key.WorkOrderGroupNo.ToString());
                                    return Json(rst);
                                }

                                if (model.ProductCode.Trim().ToUpper() == result.Data[0].Key.ProductCode.ToString())
                                {
                                    int i = 1;
                                    if (result.Code == 0 && result.Data.Count > 0)
                                    {
                                        i = Convert.ToInt32(result.Data[0].ItemNo.ToString()) + 1;
                                    }

                                    string n = model.WorkOrderGroupNo.ToString().Trim().ToUpper();
                                    WorkOrderGroupDetail obj = new WorkOrderGroupDetail()
                                    {
                                        Key = new WorkOrderGroupDetailKey()
                                        {
                                            WorkOrderGroupNo = model.WorkOrderGroupNo.ToString().Trim().ToUpper(),
                                            OrderNumber = model.OrderNumber.ToString().Trim().ToUpper(),
                                            ProductCode = model.ProductCode.ToString().Trim().ToUpper()
                                        },
                                        Nums = Convert.ToInt32(n.Substring(5, n.Length - 5)),
                                        ItemNo = i,
                                        CreateTime = DateTime.Now,
                                        Creator = User.Identity.Name,
                                        Editor = User.Identity.Name,
                                        EditTime = DateTime.Now,
                                        Description = result.Data[0].Description
                                    };
                                    rst = await client.AddAsync(obj);
                                    if (rst.Code == 0)
                                    {
                                        rst.Message = string.Format(PPMResources.StringResource.WorkOrderGroupDetail_Save_Success, obj.Key.ToString());
                                    }
                                    return Json(rst);
                                }
                                else
                                {
                                    rst.Code = 1000;
                                    rst.Message = string.Format("新增工单{0}的产品编码{1}与混工单组的产品编码{2}不一致!"
                                                                , model.OrderNumber.Trim().ToUpper()
                                                                , model.ProductCode.Trim().ToUpper()
                                                                , result.Data[0].Key.ProductCode.ToString());
                                    return Json(rst);
                                } 
                            }
                            else
                            {
                                //获取混工单组规则中该组的最大序列号
                                StringBuilder where1 = new StringBuilder();
                                if (model != null)
                                {
                                    if (!string.IsNullOrEmpty(model.WorkOrderGroupNo))
                                    {
                                        where1.AppendFormat(" {0} Key.OrderNumber = '{1}'"
                                                            , where1.Length > 0 ? "AND" : string.Empty
                                                            , model.OrderNumber.ToString().Trim().ToUpper());
                                    }

                                }
                                PagingConfig cfg1 = new PagingConfig()
                                {
                                    //OrderBy = "ItemNo Desc",
                                    Where = where1.ToString()
                                };
                                MethodReturnResult<IList<WorkOrderGroupDetail>> result1 = client.Gets(ref cfg1);
                                if (result1 != null && result1.Data.Count > 0)
                                {
                                    rst.Code = 1000;
                                    rst.Message = string.Format("新增工单{0}已存在至混工单组{1}中!"
                                                                , model.OrderNumber.Trim().ToUpper()
                                                                , result1.Data[0].Key.WorkOrderGroupNo.ToString());
                                    return Json(rst);
                                }
                                else
                                {
                                    int i = 1;                                    
                                    string n = model.WorkOrderGroupNo.ToString().Trim().ToUpper();
                                    WorkOrderGroupDetail obj = new WorkOrderGroupDetail()
                                    {
                                        Key = new WorkOrderGroupDetailKey()
                                        {
                                            WorkOrderGroupNo = model.WorkOrderGroupNo.ToString().Trim().ToUpper(),
                                            OrderNumber = model.OrderNumber.ToString().Trim().ToUpper(),
                                            ProductCode = model.ProductCode.ToString().Trim().ToUpper()
                                        },
                                        Nums = Convert.ToInt32(n.Substring(5, n.Length - 5)),
                                        ItemNo = i,
                                        CreateTime = DateTime.Now,
                                        Creator = User.Identity.Name,
                                        Editor = User.Identity.Name,
                                        EditTime = DateTime.Now,
                                        Description = model.Description,
                                    };
                                    rst = await client.AddAsync(obj);
                                    if (rst.Code == 0)
                                    {
                                        rst.Message = string.Format(PPMResources.StringResource.WorkOrderGroupDetail_Save_Success, obj.Key.ToString());
                                    }
                                    return Json(rst);
                                }
                            }

                                                      
                        }
                        #endregion
                    }
                    else
                    {
                        rst.Code = 1000;
                        rst.Message = string.Format("工单号{0}对应的产品编码{1}与界面上的产品编码{2}不一致!"
                                                    ,model.OrderNumber.Trim().ToUpper()
                                                    ,result0.Data[0].MaterialCode.ToString()
                                                    ,model.ProductCode.Trim().ToUpper());
                        return Json(rst);
                    }
                }
                else
                {
                    rst.Code = 1000;
                    rst.Message = string.Format("工单号{0}不存在或已关闭!",model.OrderNumber.Trim().ToUpper());
                    return Json(rst);
                }
            }

                      
        }
        //

        public async Task<ActionResult> Modify(string workOrderGroupNo, string orderNumber, string productCode)
        {
            WorkOrderGroupDetailViewModel viewModel = new WorkOrderGroupDetailViewModel();
            using (WorkOrderGroupDetailServiceClient client = new WorkOrderGroupDetailServiceClient())
            {
                MethodReturnResult<WorkOrderGroupDetail> result = await client.GetAsync(new WorkOrderGroupDetailKey()
                {
                    WorkOrderGroupNo = workOrderGroupNo,
                    OrderNumber = orderNumber,
                    ProductCode = productCode
                });
                if (result.Code == 0)
                {
                    viewModel = new WorkOrderGroupDetailViewModel()
                    {
                        WorkOrderGroupNo = result.Data.Key.WorkOrderGroupNo,
                        ProductCode = result.Data.Key.ProductCode,
                        OrderNumber = result.Data.Key.OrderNumber,
                        Description = result.Data.Description
                    };
                    return PartialView("_ModifyPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_ModifyPartial", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderGroupDetailViewModel model)
        {
            using (WorkOrderGroupDetailServiceClient client = new WorkOrderGroupDetailServiceClient())
            {
                WorkOrderGroupDetailKey key = new WorkOrderGroupDetailKey()
                {
                    WorkOrderGroupNo = model.WorkOrderGroupNo.ToString().Trim().ToUpper(),
                    ProductCode = model.ProductCode.ToString().Trim().ToUpper(),
                    OrderNumber = model.OrderNumber.ToString().Trim().ToUpper()
                };
                MethodReturnResult<WorkOrderGroupDetail> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format("混工单组{0}规则修改成功！",model.WorkOrderGroupNo);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }

        // GET: /PPM/WorkOrderGroupDetail/Detail
        public async Task<ActionResult> Detail(string WorkOrderGroupNo, string orderNumber, string productCode)
        {
            using (WorkOrderGroupDetailServiceClient client = new WorkOrderGroupDetailServiceClient())
            {
                WorkOrderGroupDetailKey key = new WorkOrderGroupDetailKey()
                {
                    WorkOrderGroupNo = WorkOrderGroupNo,
                    OrderNumber = orderNumber,
                    ProductCode = productCode
                };
                MethodReturnResult<WorkOrderGroupDetail> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    WorkOrderGroupDetailViewModel viewModel = new WorkOrderGroupDetailViewModel()
                    {
                        WorkOrderGroupNo = result.Data.Key.WorkOrderGroupNo,
                        OrderNumber = result.Data.Key.OrderNumber,
                        ProductCode = result.Data.Key.ProductCode,    
                        ItemNo = result.Data.ItemNo,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime,
                        Description = result.Data.Description
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
        //
        // POST: /PPM/WorkOrderGroupDetail/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string WorkOrderGroupNo, string orderNumber, string productCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            WorkOrderGroupDetailKey key = new WorkOrderGroupDetailKey()
            {
                WorkOrderGroupNo = WorkOrderGroupNo,
                OrderNumber = orderNumber,
                ProductCode = productCode
            };
            using (WorkOrderGroupDetailServiceClient client = new WorkOrderGroupDetailServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format("删除{0}成功",key);
                }
                return Json(result);
            }
        }
    }
}