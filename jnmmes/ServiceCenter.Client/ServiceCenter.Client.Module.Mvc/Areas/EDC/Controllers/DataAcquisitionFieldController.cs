using ServiceCenter.Client.Mvc.Areas.EDC.Models;
using ServiceCenter.Client.Mvc.Resources.EDC;
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
    public class DataAcquisitionFieldController : Controller
    {
        /// <summary>
        /// 根据采集项目初始化采集字段窗体
        /// </summary>
        /// <param name="itemCode">采集项目代码</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string itemCode)
        {
            //取得采集项目对象
            using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
            {
                MethodReturnResult<DataAcquisitionItem> result = await client.GetAsync(itemCode ?? string.Empty);

                if(result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "DataAcquisitionItem");
                }

                ViewBag.DataAcquisitionItem = result.Data;
            }

            //提取采集字段数据
            using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
            {
                await Task.Run(() =>
                {
                    //string where = string.Format("Key.ItemCode = '{0}'", itemCode);

                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format("Key.ItemCode = '{0}'", itemCode),
                        OrderBy = "Key.ItemCode,SerialNumber"                        
                    };

                    //提取数据
                    MethodReturnResult<IList<DataAcquisitionField>> result = client.Get(ref cfg);

                    if (result.Code > 0 || result.Data == null )
                    {
                        //错误信息处理
                    }
                    else
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }

            return View(new DataAcquisitionFieldQueryViewModel() { ItemCode = itemCode });
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(DataAcquisitionFieldQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();

                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.ItemCode))
                            {
                                where.AppendFormat(" Key.ItemCode = '{0}'", model.ItemCode);
                            }

                            if (!string.IsNullOrEmpty(model.FieldCode))
                            {
                                where.AppendFormat("{0} Key.FieldCode LIKE '{1}%'"
                                                   , where.Length > 0 ? "AND" : string.Empty
                                                   , model.FieldCode);
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {                            
                            Where = where.ToString(),
                            OrderBy = "Key.ItemCode,SerialNumber"
                        };

                        MethodReturnResult<IList<DataAcquisitionField>> result = client.Get(ref cfg);

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

                using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
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
                        MethodReturnResult<IList<DataAcquisitionField>> result = client.Get(ref cfg);
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
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(DataAcquisitionFieldViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
                {
                    DataAcquisitionField obj = new DataAcquisitionField()
                    {
                        Key = new DataAcquisitionFieldKey()
                        {
                            ItemCode = model.ItemCode,          //采集项目
                            FieldCode = model.FieldCode         //采集字段
                        },
                        FieldName = model.FieldName,            //字段说明
                        SerialNumber = model.SerialNumber,      //序号
                        DataType = model.DataType,              //数据类型
                        IsKEY = model.IsKEY,                    //主键
                        IsControl = model.IsControl,            //控制
                        MaxLine = model.MaxLine,                //控制上限
                        MinLine = model.MinLine,                //控制下限
                        Creator = User.Identity.Name,           //创建人
                        CreateTime = DateTime.Now,              //创建时间
                        Editor = User.Identity.Name,            //编辑人
                        EditTime = DateTime.Now                 //编辑时间
                    };

                    //增加对象
                    result = await client.AddAsync(obj);

                    if (result.Code == 0)
                    {
                        result.Message = string.Format(StringResource.DataAcquisitionField_Save_Success
                                                    , obj.Key);
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

        /// <summary>
        /// 修改数据提取
        /// </summary>
        /// <param name="itemCode">项目代码</param>
        /// <param name="fieldCode">字段代码</param>
        /// <returns></returns>
        public async Task<ActionResult> Modify(string itemCode, string fieldCode)
        {
            MethodReturnResult<DataAcquisitionField> result = new MethodReturnResult<DataAcquisitionField>();

            try
            {
                using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
                {                    
                    result = await client.GetAsync(new DataAcquisitionFieldKey()
                    {
                        ItemCode = itemCode,
                        FieldCode = fieldCode
                    });

                    if (result.Code == 0)
                    {
                        DataAcquisitionFieldViewModel viewModel = new DataAcquisitionFieldViewModel()
                        {
                            ItemCode = result.Data.Key.ItemCode,        //项目代码
                            FieldCode = result.Data.Key.FieldCode,      //字段代码
                            FieldName = result.Data.FieldName,          //字段说明
                            SerialNumber = result.Data.SerialNumber,    //序号
                            DataType = result.Data.DataType,            //数据类型
                            IsKEY = result.Data.IsKEY,                  //主键
                            IsControl = result.Data.IsControl,          //控制
                            MaxLine = result.Data.MaxLine,              //控制上限
                            MinLine = result.Data.MinLine,              //控制下限
                            Creator = result.Data.Creator,              //创建人
                            CreateTime = result.Data.CreateTime,        //创建时间
                            Editor = result.Data.Editor,                //编辑人
                            EditTime = result.Data.EditTime             //编辑时间
                        };

                        return PartialView("_ModifyPartial", viewModel);
                    }
                    else
                    {
                        ModelState.AddModelError("", result.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return PartialView("_ModifyPartial");                
        }

        /// <summary>
        /// 保存修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(DataAcquisitionFieldViewModel model)
        {
            using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
            {
                MethodReturnResult<DataAcquisitionField> result = await client.GetAsync(new DataAcquisitionFieldKey()
                {
                    ItemCode = model.ItemCode,
                    FieldCode = model.FieldCode
                });
                
                if (result.Code == 0)
                {
                    result.Data.FieldName = model.FieldName;            //字段说明
                    result.Data.SerialNumber = model.SerialNumber;      //序号
                    result.Data.DataType = model.DataType;              //数据类型
                    result.Data.IsKEY = model.IsKEY;                    //主键
                    result.Data.IsControl = model.IsControl;            //控制
                    result.Data.MaxLine = model.MaxLine;                //控制上限
                    result.Data.MinLine = model.MinLine;                //控制下限
                    result.Data.Creator = model.Creator;                //创建人
                    result.Data.CreateTime = model.CreateTime;          //创建时间
                    result.Data.Editor = User.Identity.Name; ;          //编辑人
                    result.Data.EditTime = DateTime.Now;                //编辑时间

                    MethodReturnResult rst = await client.ModifyAsync(result.Data);

                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(StringResource.DataAcquisitionField_SaveModify_Success
                                                    , result.Data.Key);
                    }

                    return Json(rst);
                }
                return Json(result);
            }
        }

        /// <summary>
        /// 取得采集项目对应的字段数据
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="fieldCode"></param>
        /// <returns></returns>
        public async Task<ActionResult> Detail(string itemCode, string fieldCode)
        {
            using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
            {
                //取得数据
                MethodReturnResult<DataAcquisitionField> result = await client.GetAsync(new DataAcquisitionFieldKey()
                {
                    ItemCode = itemCode,
                    FieldCode = fieldCode
                });

                if (result.Code == 0)
                {
                    DataAcquisitionFieldViewModel viewModel = new DataAcquisitionFieldViewModel()
                    {
                        ItemCode = result.Data.Key.ItemCode,        //项目代码
                        FieldCode = result.Data.Key.FieldCode,      //字段代码
                        FieldName = result.Data.FieldName,          //字段说明
                        SerialNumber = result.Data.SerialNumber,    //序号
                        DataType = result.Data.DataType,            //数据类型
                        IsKEY = result.Data.IsKEY,                  //主键
                        IsControl = result.Data.IsControl,          //控制
                        MaxLine = result.Data.MaxLine,              //控制上限
                        MinLine = result.Data.MinLine,              //控制下限
                        Creator = result.Data.Creator,              //创建人
                        CreateTime = result.Data.CreateTime,        //创建时间
                        Editor = result.Data.Editor,                //编辑人
                        EditTime = result.Data.EditTime             //编辑时间
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
        /// 删除字段
        /// </summary>
        /// <param name="itemCode">项目代码</param>
        /// <param name="fieldCode">字段代码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string itemCode, string fieldCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
            {
                var key = new DataAcquisitionFieldKey()
                {
                    ItemCode = itemCode,
                    FieldCode = fieldCode
                };

                result = await client.DeleteAsync(key);

                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.DataAcquisitionField_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
	}
}