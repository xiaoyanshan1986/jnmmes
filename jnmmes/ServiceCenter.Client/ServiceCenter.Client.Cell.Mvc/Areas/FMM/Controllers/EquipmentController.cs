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
    public class EquipmentController : Controller
    {
        //
        // GET: /FMM/Equipment/
        public async Task<ActionResult> Index()
        {
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new EquipmentQueryViewModel());
        }

        //
        //POST: /FMM/Equipment/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(EquipmentQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (EquipmentServiceClient client = new EquipmentServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Code))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Code);
                            }
                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Name LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }
                            if (!string.IsNullOrEmpty(model.GroupName))
                            {
                                where.AppendFormat(" {0} GroupName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.GroupName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);

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
        //POST: /FMM/Equipment/PagingQuery
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

                using (EquipmentServiceClient client = new EquipmentServiceClient())
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
                        MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
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
        // POST: /FMM/Equipment/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(EquipmentViewModel model)
        {
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                Equipment obj = new Equipment()
                {
                    Key = model.Code,
                    AssetsNo=model.AssetsNo,
                    AvTime=model.AvTime,
                    GroupName=model.GroupName,
                    ChangeStateName=null,
                    IsBatch=model.IsBatch,
                    IsMultiChamber = model.IsMultiChamber,
                    IsChamber = model.IsMultiChamber?false:model.IsChamber,
                    LineCode=model.LineCode,
                    LocationName=model.LocationName,
                    MaxQuantity=model.MaxQuantity,
                    MinQuantity=model.MinQuantity,
                    Name=model.Name,
                    No=model.No,
                    RealEquipmentCode=model.RealEquipmentCode,
                    RunRate=model.RunRate,
                    StateName=model.StateName,
                    TactTime=model.TactTime,
                    TotalChamber=model.TotalChamber,
                    Type=model.Type,
                    WPH=model.WPH,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };

                obj.ChamberIndex = obj.IsChamber ? model.ChamberIndex : null;
                obj.ParentEquipmentCode = obj.IsChamber ? model.ParentEquipmentCode : null;

                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.Equipment_Save_Success
                                                , model.Code);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/Equipment/Modify
        public async Task<ActionResult> Modify(string key)
        {
            EquipmentViewModel viewModel = new EquipmentViewModel();
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                MethodReturnResult<Equipment> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new EquipmentViewModel()
                    {
                        AssetsNo=result.Data.AssetsNo,
                        AvTime = result.Data.AvTime,
                        ChamberIndex = result.Data.ChamberIndex,
                        GroupName = result.Data.GroupName,
                        IsBatch = result.Data.IsBatch,
                        IsChamber = result.Data.IsChamber,
                        IsMultiChamber = result.Data.IsMultiChamber,
                        LineCode = result.Data.LineCode,
                        LocationName = result.Data.LocationName,
                        MaxQuantity = result.Data.MaxQuantity,
                        MinQuantity = result.Data.MinQuantity,
                        Name = result.Data.Name,
                        No = result.Data.No,
                        ParentEquipmentCode = result.Data.ParentEquipmentCode,
                        RealEquipmentCode = result.Data.RealEquipmentCode,
                        RunRate = result.Data.RunRate,
                        StateName = result.Data.StateName,
                        TactTime = result.Data.TactTime,
                        TotalChamber = result.Data.TotalChamber,
                        Type = result.Data.Type,
                        WPH = result.Data.WPH,
                        Description = result.Data.Description,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime,
                        CreateTime=result.Data.CreateTime,
                        Creator=result.Data.Creator,
                        Code=result.Data.Key
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
        // POST: /FMM/Equipment/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(EquipmentViewModel model)
        {
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                MethodReturnResult<Equipment> result = await client.GetAsync(model.Code);

                if (result.Code == 0)
                {
                    result.Data.AssetsNo=model.AssetsNo;
                    result.Data.AvTime = model.AvTime;
                    
                    result.Data.GroupName = model.GroupName;
                    result.Data.IsBatch = model.IsBatch;
                    result.Data.IsMultiChamber = model.IsMultiChamber;
                    result.Data.IsChamber = model.IsMultiChamber?false:model.IsChamber;
                    result.Data.ChamberIndex = result.Data.IsChamber ? model.ChamberIndex : null;
                    result.Data.ParentEquipmentCode = result.Data.IsChamber ? model.ParentEquipmentCode:null;
                    result.Data.LineCode = model.LineCode;
                    result.Data.LocationName = model.LocationName;
                    result.Data.MaxQuantity = model.MaxQuantity;
                    result.Data.MinQuantity = model.MinQuantity;
                    result.Data.Name = model.Name;
                    result.Data.No = model.No;
                    result.Data.RealEquipmentCode = model.RealEquipmentCode;
                    result.Data.RunRate = model.RunRate;
                    result.Data.TactTime = model.TactTime;
                    result.Data.TotalChamber = model.TotalChamber;
                    result.Data.Type = model.Type;
                    result.Data.WPH = model.WPH;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.Equipment_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/Equipment/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                MethodReturnResult<Equipment> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    EquipmentViewModel viewModel = new EquipmentViewModel()
                    {
                        AssetsNo = result.Data.AssetsNo,
                        AvTime = result.Data.AvTime,
                        ChamberIndex = result.Data.ChamberIndex,
                        GroupName = result.Data.GroupName,
                        IsBatch = result.Data.IsBatch,
                        IsChamber = result.Data.IsChamber,
                        IsMultiChamber = result.Data.IsMultiChamber,
                        LineCode = result.Data.LineCode,
                        LocationName = result.Data.LocationName,
                        MaxQuantity = result.Data.MaxQuantity,
                        MinQuantity = result.Data.MinQuantity,
                        Name = result.Data.Name,
                        No = result.Data.No,
                        ParentEquipmentCode = result.Data.ParentEquipmentCode,
                        RealEquipmentCode = result.Data.RealEquipmentCode,
                        RunRate = result.Data.RunRate,
                        StateName = result.Data.StateName,
                        TactTime = result.Data.TactTime,
                        TotalChamber = result.Data.TotalChamber,
                        Type = result.Data.Type,
                        WPH = result.Data.WPH,
                        Description = result.Data.Description,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Code = result.Data.Key
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
        // POST: /FMM/Equipment/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.Equipment_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}