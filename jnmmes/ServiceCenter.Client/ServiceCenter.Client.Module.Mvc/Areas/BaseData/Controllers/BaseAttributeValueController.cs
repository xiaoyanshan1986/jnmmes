using ServiceCenter.Client.Mvc.Resources.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.BaseData.Controllers
{
    public class BaseAttributeValueController : Controller
    {
        //
        // GET: /BaseData/BaseAttributeValue/
        public async Task<ActionResult> Index(string categoryName)
        {
            using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
            {
                MethodReturnResult<BaseAttributeCategory> result = await client.GetAsync(categoryName ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "BaseAttributeCategory");
                }
                ViewBag.BaseAttributeCategory = result.Data;
            }

            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                await Task.Run(() =>
                {
                    string where = string.Format("Key.CategoryName='{0}'", categoryName);
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        OrderBy = "Key.CategoryName,Order",
                        Where = where
                    };
                    MethodReturnResult<IList<BaseAttribute>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.BaseAttributeList = result.Data;
                    }
                });
            }
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                await Task.Run(() =>
                {
                    string where = string.Format("Key.CategoryName='{0}'", categoryName);
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "Key.CategoryName,Key.ItemOrder",
                        Where = where
                    };
                    MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.List = result.Data;
                    }
                });
            }
            if(Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial");
            }
            return View();
        }
        //
        // POST: /BaseData/BaseAttributeValue/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save()
        {
            MethodReturnResult rst = new MethodReturnResult();

            int itemNo = Convert.ToInt32(Request["ItemNo"]);
            string categoryName = Request["CategoryName"];
            IList<BaseAttributeValue> lstVal=new List<BaseAttributeValue>();
            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                MethodReturnResult<IList<BaseAttribute>> result = await Task.Run<MethodReturnResult<IList<BaseAttribute>>>(() =>
                {
                    string where = string.Format("Key.CategoryName='{0}'", categoryName);
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        OrderBy = "Key.CategoryName,Order",
                        Where = where
                    };
                    return  client.Get(ref cfg);
                });
                
                
                if (result.Code == 0)
                {
                    foreach(BaseAttribute attr in result.Data)
                    {
                        string attrValue = Request[attr.Key.AttributeName]??string.Empty;

                        BaseAttributeValue val = new BaseAttributeValue()
                        {
                            Key = new BaseAttributeValueKey()
                            {
                                CategoryName=attr.Key.CategoryName,
                                AttributeName=attr.Key.AttributeName,
                                ItemOrder=itemNo
                            },
                            Value=attrValue.Split(',')[0],
                            Editor=User.Identity.Name,
                            EditTime=DateTime.Now
                        };
                        lstVal.Add(val);
                    }
                }
            }

            using(BaseAttributeValueServiceClient client=new BaseAttributeValueServiceClient())
            {
                rst =await client.AddAsync(lstVal);
                if(rst.Code==0)
                {
                    rst.Message = string.Format(StringResource.BaseAttributeValue_Save_Success);
                }
            }
            return Json(rst);
        }

         //
        // GET: /BaseData/BaseAttributeValue/Modify
        public async Task<ActionResult> Modify(string categoryName, int? itemOrder)
        {
            await SetBaseAttributeValue(categoryName, itemOrder);
            return PartialView("_ModifyPartial");
        }
        //
        // POST: /BaseData/BaseAttributeValue/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify()
        {
            MethodReturnResult rst = new MethodReturnResult();

            int itemNo = Convert.ToInt32(Request["ItemNo"]);
            string categoryName = Request["CategoryName"];
            IList<BaseAttributeValue> lstVal = new List<BaseAttributeValue>();
            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                MethodReturnResult<IList<BaseAttribute>> result = await Task.Run<MethodReturnResult<IList<BaseAttribute>>>(() =>
                {
                    string where = string.Format("Key.CategoryName='{0}'", categoryName);
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "Key.CategoryName,Order",
                        Where = where
                    };
                    return client.Get(ref cfg);
                });


                if (result.Code == 0)
                {
                    foreach (BaseAttribute attr in result.Data)
                    {
                        string attrValue = Request[attr.Key.AttributeName]??string.Empty;

                        BaseAttributeValue val = new BaseAttributeValue()
                        {
                            Key = new BaseAttributeValueKey()
                            {
                                CategoryName = attr.Key.CategoryName,
                                AttributeName = attr.Key.AttributeName,
                                ItemOrder = itemNo
                            },
                            Value = attrValue.Split(',')[0],
                            Editor = User.Identity.Name,
                            EditTime = DateTime.Now
                        };
                        lstVal.Add(val);
                    }
                }
            }

            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                rst = await client.ModifyAsync(lstVal);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(StringResource.BaseAttributeValue_Save_Success);
                }
            }
            return Json(rst);
        }
        //
        // GET: /BaseData/BaseAttributeValue/Detail
        public async Task<ActionResult> Detail(string categoryName, int? itemOrder)
        {
            await SetBaseAttributeValue(categoryName, itemOrder);
            return PartialView("_InfoPartial");
        }
        private async Task SetBaseAttributeValue(string categoryName, int? itemOrder)
        {
            ViewBag.ItemOrder = itemOrder;
            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                await Task.Run(() =>
                {
                    string where = string.Format("Key.CategoryName='{0}'", categoryName);
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "Key.CategoryName,Order",
                        Where = where
                    };
                    MethodReturnResult<IList<BaseAttribute>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.BaseAttributeList = result.Data;
                    }
                });
            }

            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                await Task.Run(() =>
                {
                    string where = string.Format("Key.CategoryName='{0}' AND Key.ItemOrder='{1}'"
                                                , categoryName
                                                , itemOrder ?? 0);
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "Key.CategoryName,Key.ItemOrder",
                        Where = where
                    };
                    MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.List = result.Data;
                    }
                });
            }
        }
        //
        // POST: /BaseData/BaseAttributeValue/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string categoryName, int? itemOrder)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (itemOrder == null)
            {
                result.Code = 1000;
                result.Message = StringResource.BaseAttributeValue_ParamterError;
                return Json(result);
            }

            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                result = await client.DeleteAsync(categoryName,itemOrder.Value);
                if (result.Code == 0)
                {
                    result.Message = StringResource.BaseAttributeValue_Delete_Success;
                }
                return Json(result);
            }
        }
	}
}