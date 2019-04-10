using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Areas.FMM.Models;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.Model;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using System.Drawing;
using ServiceCenter.Client.Mvc.Areas.EMS.Models;
using ServiceCenter.MES.Service.Client.EMS;
using ServiceCenter.MES.Model.EMS;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Controllers
{
    public class EquipmentRealtimeMonitorController : Controller
    {
        // GET: FMM/EquipmentRealtimeMonitor
        public ActionResult Index()
        {
            return View(new EquipmentLayoutDetailQueryViewModel());
        }
        public ActionResult GetEquipmentReasonCode(string ReasonCodeCategoryName)
        {
            //取得设备原因组明细
            IList<EquipmentReasonCodeCategoryDetail> lst = new List<EquipmentReasonCodeCategoryDetail>();
            using (EquipmentReasonCodeCategoryDetailServiceClient client = new EquipmentReasonCodeCategoryDetailServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.ReasonCodeCategoryName ='{0}'"
                                          , ReasonCodeCategoryName),
                    OrderBy = "ItemNo"
                };
                MethodReturnResult<IList<EquipmentReasonCodeCategoryDetail>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }
            return Json(from item in lst
                        select new
                        {
                            Text = item.Key.ReasonCodeName,
                            Value = item.Key.ReasonCodeName
                        }, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Query(EquipmentLayoutDetailQueryViewModel model)
        {
            using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
            {
                MethodReturnResult<EquipmentLayout> result = await client.GetAsync(model.LayoutName ?? string.Empty);
                ViewBag.EquipmentLayout = result.Data;
            }

            using (EquipmentLayoutDetailServiceClient client = new EquipmentLayoutDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(" Key.LayoutName = '{0}'"
                                              , model.LayoutName)
                    };
                    MethodReturnResult<IList<EquipmentLayoutDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false
                    };
                    MethodReturnResult<IList<EquipmentState>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.StateList = result.Data;
                    }
                });
            }
            return PartialView("_ListPartial", new EquipmentLayoutDetailViewModel() { LayoutName = model.LayoutName });
        }

        /// <summary> 修改设备状态记录及设备状态 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(EquipmentStateEventViewModel model)
        {
            MethodReturnResult rs = new MethodReturnResult();

            try
            {                
                DateTime now = DateTime.Now;
                
                //新增设备事件
                using (EquipmentStateEventServiceClient client = new EquipmentStateEventServiceClient())
                {
                    //设备事件
                    EquipmentStateEvent newEquipmentStateEvent = new EquipmentStateEvent()
                    {
                        Key = "",                                               //设备事件主键
                        EquipmentCode = model.EquipmentCode,                    //设备代码
                        EquipmentChangeStateName = model.ChangeStateName,       //设备状态切换名称                        
                        EquipmentFromStateName = model.FromStateName,           //来源状态
                        EquipmentToStateName = model.ToStateName,               //目标状态
                        ReasonCodeCategoryName = model.ReasonCodeCategoryName,  //原因类型
                        ReasonCodeName = model.ReasonCodeName,                  //原因代码
                        Description = model.Description,                        //描述
                        IsCurrent = true,                                       //当前状态
                        Creator = User.Identity.Name,                           //创建人
                        CreateTime = now,                                       //创建时间
                        Editor = User.Identity.Name,                            //编辑人
                        EditTime = now                                          //编辑时间  
                    };

                    rs = await client.AddAsync(newEquipmentStateEvent);

                    if (rs.Code > 0)
                    {
                        return Json(rs);
                    }

                    rs.Message = "设备状态设置成功！";

                    return Json(rs);
                }
            }
            catch (Exception ex)
            {
                rs.Code = 1000;
                rs.Message = ex.Message;
                rs.Detail = ex.ToString();

                return Json(rs);
            }
        }
    
        public void ShowBackgroudImage(string key)
        {
            HttpContext.Response.ContentType = "image/jpeg";
            try
            {
                using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
                {
                    MethodReturnResult<EquipmentLayout> result = client.Get(key);

                    if (result.Code == 0 && result.Data != null && result.Data.BackgroundImage != null)
                    {
                        HttpContext.Response.BinaryWrite(result.Data.BackgroundImage);
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

        public ActionResult GetEQPInfo(string LayoutName)
        {
            using (EquipmentLayoutDetailServiceClient client = new EquipmentLayoutDetailServiceClient())
            {
                MethodReturnResult<DataTable> result = client.GetEQPInfo(LayoutName);

                return Json(JsonConvert.SerializeObject(result.Data), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetEquipmentInfo(string EqpCode)
        {
            string strEqpInfo = "";
            EquipmentLayoutDetailEqpInfoViewModel model =null;
            using (EquipmentLayoutDetailServiceClient client = new EquipmentLayoutDetailServiceClient())
            {
                MethodReturnResult<DataTable> result = client.GetEquipmentInfo(EqpCode);
                if(result!=null && result.Code==0)
                {
                    if(result.Data!=null && result.Data.Rows.Count>0)
                    {
                        model = new EquipmentLayoutDetailEqpInfoViewModel()
                        {
                            EquipmentCode = result.Data.Rows[0]["EQUIPMENT_CODE"].ToString(),
                            EquipmentName = result.Data.Rows[0]["EQUIPMENT_NAME"].ToString(),
                            EquipmentStateName = result.Data.Rows[0]["EQUIPMENT_STATE_NAME"].ToString(),
                            EquipmentStateMinutes = result.Data.Rows[0]["MinutesQty"].ToString(),
                            Creator = result.Data.Rows[0]["CREATOR"].ToString(),
                            CREATE_TIME = result.Data.Rows[0]["Create_Time"].ToString(),
                            Description = result.Data.Rows[0]["Description"].ToString()
                        };
                        return PartialView("_InfoPartial", model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_InfoPartial");
        }

        
        public ActionResult GetParameByEquCode(string EqpCode)
        {
            using (EquipmentLayoutDetailServiceClient client = new EquipmentLayoutDetailServiceClient())
            {
                MethodReturnResult<DataTable> result = client.GetParameByEqpCode(EqpCode);
                var l = from t in result.Data.AsEnumerable()
                        group t by new
                        {
                            t1 = t.Field<string>("EQUIPMENT_CODE"),
                            t2 = t.Field<string>("EQUIPMENT_NAME"),
                            t3 = t.Field<string>("LINE_CODE"),
                            t4 = t.Field<string>("ROUTE_OPERATION_NAME")
                        } into m
                        select new
                        {
                            EQUIPMENT_CODE = m.First().Field<string>("EQUIPMENT_CODE"),
                            EQUIPMENT_NAME = m.First().Field<string>("EQUIPMENT_NAME"),
                            //LINE_CODE = m.First().Field<string>("LINE_CODE"),
                            //ROUTE_OPERATION_NAME = m.First().Field<string>("ROUTE_OPERATION_NAME"),
                        };
                return Json(l, JsonRequestBehavior.AllowGet);
            }
        }
    }
}