using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using System.Text;
using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotBinController : Controller
    {
        string localName = System.Configuration.ConfigurationSettings.AppSettings["LocalName"];
        //
        // GET: /WIP/LotTerminal/
        public ActionResult Index()
        {
            return View(new LotBinViewModel());
        }


        public ActionResult Save(LotBinViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                string IPAddress = model.ScanIP;
                string packalgeLine = string.Empty;
                if (localName == "K01")
                {
                    MethodReturnResult<Equipment> resultEquipment = new MethodReturnResult<Equipment>();
                    using (EquipmentServiceClient client = new EquipmentServiceClient())
                    {
                        resultEquipment = client.Get(IPAddress);
                        if (resultEquipment.Code > 0)
                        {
                            result.Code = resultEquipment.Code;
                            result.Message = resultEquipment.Message;
                            return Json(result);
                        }
                        else
                        {
                            packalgeLine = resultEquipment.Data.LineCode;
                        }

                    }
                }
                if (localName == "G01")
                {
                    packalgeLine = model.ScanIP;
                }
                                
                InBinParameter p = new InBinParameter()
                {
                    Creator = User.Identity.Name,
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    BinNo = model.BinNo,
                    //ReasonCodeCategoryName = model.ReasonCodeCategoryName,
                    //ReasonCodeName = model.ReasonCodeName,
                    //Remark = model.Description,
                    PackageLine = packalgeLine,
                    ScanLotNumber = model.LotNumber,
                    ScanIP = model.ScanIP,
                    ScanNo = model.ScanNo,//
                    LotNumbers = new List<string>()
                };

                //char splitChar = ',';
                ////获取批次号值。
                //string[] lotNumbers = Request["LotNumber"].ToUpper().Split(splitChar);
                //p.LotNumbers = lotNumbers.ToList();


                //for (int i = 0; i < p.LotNumbers.Count; i++)
                //{
                //    string lotNumber = p.LotNumbers[i];
                //    //result = GetLot(lotNumber);
                //    if (result.Code > 0)
                //    {
                //        return Json(result);
                //    }
                //}

                using (LotBinServiceClient client = new LotBinServiceClient())
                {
                    result = client.InBin(p);
                }
                if (result.Code == 0)
                {
                    result.Message = string.Format("批次{0}入Bin成功.{1}", model.LotNumber, result.Message);
                }

            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return Json(result);
        }


        public ActionResult ChkBin()
        {
            LotBinViewModel model = new LotBinViewModel();
            return ChkBin(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChkBin(LotBinViewModel lotBinViewModel)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                string IPAddress = lotBinViewModel.ScanIP;
                string packalgeLine = string.Empty;
                if (localName == "K01")
                {
                    MethodReturnResult<Equipment> resultEquipment = new MethodReturnResult<Equipment>();
                    using (EquipmentServiceClient client = new EquipmentServiceClient())
                    {
                        resultEquipment = client.Get(IPAddress);
                        if (resultEquipment.Code > 0)
                        {
                            result.Code = resultEquipment.Code;
                            result.Message = resultEquipment.Message;
                            return Json(result);
                        }
                        else
                        {
                            packalgeLine = resultEquipment.Data.LineCode;
                        }

                    }
                }
                if (localName == "G01")
                {
                    packalgeLine = lotBinViewModel.ScanIP;
                }
                InBinParameter p = new InBinParameter()
                {
                    Creator = User.Identity.Name, 
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    ScanLotNumber = lotBinViewModel.LotNumber,
                    ScanIP = lotBinViewModel.ScanIP,
                    PackageLine=packalgeLine,
                    ScanNo = lotBinViewModel.ScanNo,//
                    LotNumbers = new List<string>()
                };

                //char splitChar = ',';
                ////获取批次号值。
                //string[] lotNumbers = lotBinViewModel.LotNumber.ToUpper().Split(splitChar);
                //p.LotNumbers = lotNumbers.ToList();


                //for (int i = 0; i < p.LotNumbers.Count; i++)
                //{
                //    string lotNumber = p.LotNumbers[i];
                //    //result = GetLot(lotNumber);
                //    if (result.Code > 0)
                //    {
                //        return Json(result);
                //    }
                //}

                using (LotBinServiceClient client = new LotBinServiceClient())
                {
                    result = client.ChkBin(p);
                }
                result.Message = string.Format("批次{0}将入Bin{1}.原因{2}", lotBinViewModel.LotNumber, result.Detail,result.Message);


            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return Json(result);
        }

        public ActionResult Query()
        {
            LotBinQueryViewModel model = new LotBinQueryViewModel();
            return Query(model);
        }


        //POST: /WIP/LotDefect/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(LotBinQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (LotBinServiceClient client = new LotBinServiceClient())
                {
                    MethodReturnResult<IList<PackageBin>> result = client.QueryBinListFromPackageLine(model.PackageLine);

                    if (result.Code == 0)
                    {
                        ViewBag.List = result.Data;
                    }
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial",model);
            }
            else
            {
                return View(model);
            }
        }
        public ActionResult QueryPackageNo(String packageNo)
        {

            if (!string.IsNullOrEmpty(packageNo))
            {
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    MethodReturnResult<Package> result2 = client.Get(packageNo.Trim().ToUpper());
                    if (result2.Code == 0)
                    {
                        ViewBag.Package = result2.Data;
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.PackageNo='{0}'", packageNo.Trim().ToUpper()),
                        OrderBy = "ItemNo"
                    };
                    MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PackageDetailList = result.Data;
                    }
                }
            }
            return PartialView("_ListPackageQuery", new ZPVMLotPackageViewModel());
        }



        //POST: /WIP/LotDefect/Query
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult RefreshBinList(string packageLine, string binNo)
        {
            if (ModelState.IsValid)
            {
                using (LotBinServiceClient client = new LotBinServiceClient())
                {
                    MethodReturnResult<IList<PackageBin>> result = client.QueryBinListFromPackageLine(packageLine);

                    if (result.Code == 0)
                    {
                        ViewBag.List = result.Data;

                    }
                }
            }
            return PartialView("_ListPartial", new LotBinQueryViewModel());
        }


        public string GetQueryCondition(LotBinQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            /*
            if (model != null)
            {
                where.Append(@" EXISTS ( From LotTransaction as p 
                                         WHERE p.Key=self.Key.TransactionKey 
                                         AND p.UndoFlag='0')");

                if (!string.IsNullOrEmpty(model.OrderNumber))
                {
                    where.AppendFormat(@" {0} EXISTS( From LotTransaction as p 
                                                      WHERE p.Key=self.Key.TransactionKey 
                                                      AND p.OrderNumber='{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.OrderNumber);
                }

                if (!string.IsNullOrEmpty(model.RouteStepName))
                {
                    where.AppendFormat(@" {0} EXISTS( From LotTransaction as p 
                                                      WHERE p.Key=self.Key.TransactionKey 
                                                      AND p.RouteStepName='{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.RouteStepName);
                }

                if (!string.IsNullOrEmpty(model.ReasonCodeName))
                {
                    where.AppendFormat(" {0} Key.ReasonCodeName LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ReasonCodeName);
                }

                if (!string.IsNullOrEmpty(model.RouteOperationName))
                {
                    where.AppendFormat(" {0} RouteOperationName LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.RouteOperationName);
                }

                if (!string.IsNullOrEmpty(model.ResponsiblePerson))
                {
                    where.AppendFormat(" {0} ResponsiblePerson LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ResponsiblePerson);
                }

                if (model.StartTime != null)
                {
                    where.AppendFormat(" {0} EditTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.StartTime);
                }

                if (model.EndTime != null)
                {
                    where.AppendFormat(" {0} EditTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EndTime);
                }

            }*/
            return where.ToString();
        }
    }
}