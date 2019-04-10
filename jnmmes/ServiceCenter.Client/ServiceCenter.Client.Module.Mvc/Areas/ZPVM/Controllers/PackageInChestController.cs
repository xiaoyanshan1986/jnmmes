using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using System.Threading.Tasks;
using System.Data;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class PackageInChestController : Controller
    {
        // GET: /ZPVM/PackageInChest/
        // 显示入柜作业界面
        public ActionResult Index()
        {
            return View(new ChestViewModel());
        }        

        public ActionResult Check()
        {
            return View(new ChestViewModel());
        }

        //POST: /ZPVM/PackageInChest/Query
        //查询刷新入柜作业界面
        [HttpPost]
        public ActionResult Query(ChestViewModel model)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                if (!string.IsNullOrEmpty(model.ChestNo))
                {
                    model.ChestNo = model.ChestNo.ToUpper().Trim();
                    ChestParameter param = new ChestParameter();
                    param.ChestNo = model.ChestNo;
                    using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                    {
                        MethodReturnResult<DataSet> ds = client.GetRefreshChestDetailByDB(ref param);
                        if (ds.Code > 0)
                        {                            
                            result.Code = ds.Code;
                            result.Message = ds.Message;
                            result.Detail = ds.Detail;

                            return Json(result);
                        }

                        ViewBag.ListData = ds.Data.Tables[0];

                        //MethodReturnResult<Chest> result1 = client.Get(model.ChestNo);
                        //if (result1.Code == 0)
                        //{
                        //    using (MaterialChestParameterServiceClient client1 = new MaterialChestParameterServiceClient())
                        //    {
                        //        MethodReturnResult<MaterialChestParameter> rst3 = client1.Get(result1.Data.MaterialCode);
                        //        if (rst3.Data != null)
                        //        {
                        //            model.FullQuantity = rst3.Data.FullChestQty;
                        //        }
                        //        else
                        //        {
                        //            return Json(rst3);
                        //        }
                        //    }
                        //    ViewBag.Chest = result1.Data;
                        //    model.StoreLocation = result1.Data.StoreLocation;
                        //    model.CurrentQuantity = result1.Data.Quantity;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", model);
            }
            else
            {
                return View("Index", model);
            }            
        }                

        [HttpPost]
        public ActionResult QueryChecked(ChestViewModel model)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                if (!string.IsNullOrEmpty(model.ChestNo))
                {
                    model.ChestNo = model.ChestNo.ToUpper().Trim();
                    ChestParameter param = new ChestParameter();
                    param.ChestNo = model.ChestNo;
                    using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                    {
                        MethodReturnResult<DataSet> ds = client.GetCheckedChestDetailByDB(ref param);
                        if (ds.Code > 0)
                        {
                            result.Code = ds.Code;
                            result.Message = ds.Message;
                            result.Detail = ds.Detail;

                            return Json(result);
                        }

                        ViewBag.ListData = ds.Data.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartialCheck", model);
            }
            else
            {
                return View("check", model);
            }
        }

        //托号入柜
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ChestViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            //Package obj = null;
            if (model.PackageNo == null || model.PackageNo == "")
            {
                result.Code = 1001;
                result.Message = string.Format("托号不可为空。");
                return Json(result);
            }
            else
            {
                model.PackageNo = model.PackageNo.ToUpper().Trim();
            }
            try
            {               
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    //取得最佳柜号
                    MethodReturnResult<string> rst1 = client.GetChestNo(model.PackageNo.ToUpper().Trim(), model.ChestNo, model.IsLastestPackageInChest, model.IsManual);
                    if (rst1.Code > 0)
                    {
                        return Json(rst1);
                    }
                    else
                    {
                        model.ChestNo = rst1.Data;
                    }
                }
                MethodReturnResult<Chest> rst2 = null;
                //重新获取当前数量。
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    rst2 = client.Get(model.ChestNo);
                    if (rst2.Code == 1000)
                    {
                        return Json(rst2);
                    }
                    //检查柜状态
                    if (rst2.Data != null && rst2.Data.ChestState != EnumChestState.Packaging && rst2.Data.ChestState != EnumChestState.InFabStore)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("柜 {0} 非 [{1}或{2}] 状态，不能入柜。"
                                                        , model.ChestNo.ToUpper()
                                                        , EnumChestState.Packaging.GetDisplayName()
                                                        , EnumChestState.InFabStore.GetDisplayName());
                        return Json(result);
                    }
                    //设置满柜数量。
                    if (rst2.Code <= 0 && rst2.Data != null)
                    {
                        model.CurrentQuantity = rst2.Data.Quantity;                       
                    }
                    MethodReturnResult<Package> rstOfPackage = null;

                    using (PackageQueryServiceClient clientOfPackage = new PackageQueryServiceClient())
                    {
                        rstOfPackage = clientOfPackage.Get(model.PackageNo.ToUpper().Trim());
                    }
                    using (MaterialChestParameterServiceClient client1 = new MaterialChestParameterServiceClient())
                    {
                        MethodReturnResult<MaterialChestParameter> rst3 = client1.Get(rstOfPackage.Data.MaterialCode);
                        if (rst3.Data != null)
                        {
                            model.FullQuantity = rst3.Data.FullChestQty;
                        }
                        else
                        {
                            return Json(rst3);
                        }
                    }
                }
                
                //如果满柜数量为空，提示
                if (model.FullQuantity == 0)
                {                   
                    result.Code = 1001;
                    result.Message = string.Format("托号内产品编码【{0}】设置的满柜数量为0，请联系成柜规则设定人员修改。", rst2.Data.MaterialCode);
                    return Json(result);
                }

                double newCurrentQuantity = model.CurrentQuantity + 1;
                //当前数量超过满柜数量，不能继续入柜。
                if (newCurrentQuantity > model.FullQuantity)
                {
                    result.Code = 1;
                    result.Message = string.Format("柜（{0}) 当前数量({1})加上该托号（{2}）数量（{3}），超过满柜数量。"
                                                    , model.ChestNo.ToUpper()
                                                    , model.CurrentQuantity
                                                    , model.PackageNo.ToUpper().Trim()
                                                    , 1);
                    return Json(result);
                }
                model.CurrentQuantity = newCurrentQuantity;                               
                result = Chest(model);               
                //返回成柜结果。
                if (result.Code <= 0)
                {
                    model.StoreLocation = result.Detail.Split('-')[1];
                    MethodReturnResult<ChestViewModel> rstFinal = new MethodReturnResult<ChestViewModel>()
                    {
                        Code = result.Code,
                        Data = model,
                        Detail = result.Detail,
                        HelpLink = result.HelpLink,
                        Message = result.Message
                    };
                    return Json(rstFinal);
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

        [HttpPost]
        public ActionResult Finish(ChestViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //如果柜号为空。
                if (string.IsNullOrEmpty(model.ChestNo))
                {
                    result.Code = 1001;
                    result.Message = string.Format("柜号不能为空。");
                    return Json(result);
                }
                else
                {
                    model.ChestNo = model.ChestNo.ToUpper().Trim();
                }
                Chest obj = null;
                //如果当前数量为空，获取当前数量
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    MethodReturnResult<Chest> rst2 = client.Get(model.ChestNo);
                    if (rst2.Code > 0)
                    {
                        return Json(rst2);
                    }
                    if (rst2.Code <= 0 && rst2.Data != null)
                    {
                        //检查柜状态
                        if (rst2.Data.ChestState != EnumChestState.Packaging)
                        {
                            result.Code = 1001;
                            result.Message = string.Format("柜 {0} 非{1}状态，不能执行手动完成入柜。"
                                                            , model.ChestNo.ToUpper()
                                                            , EnumChestState.Packaging.GetDisplayName());
                            return Json(result);
                        }
                        //设置当前数量。
                        obj = rst2.Data;
                        model.CurrentQuantity = rst2.Data.Quantity;
                        if (model.CurrentQuantity == 0)
                        {
                            result.Code = 1001;
                            result.Message = string.Format("柜 {0} 数量为0，不能执行手动完成入柜。", model.ChestNo.ToUpper());
                            return Json(result);
                        }
                    }
                }
                //如果满柜数量为空，获取满柜数量
                if (model.FullQuantity == 0)
                {
                    using (MaterialChestParameterServiceClient client1 = new MaterialChestParameterServiceClient())
                    {
                        MethodReturnResult<MaterialChestParameter> rst3 = client1.Get(obj.MaterialCode);
                        if (rst3.Data != null)
                        {
                            model.FullQuantity = rst3.Data.FullChestQty;
                        }
                        else
                        {
                            return Json(rst3);
                        }
                    }
                }
                //非尾柜，不能完成入柜
                if (model.IsLastestPackageInChest == false && obj.IsLastPackage == false)
                {
                    result.Code = 1;
                    result.Message = string.Format("柜({0})非尾柜，不能手动完成入柜。", model.ChestNo);
                    return Json(result);
                }
                //判断柜号所在库位和当前界面所选库位是否匹配。
                if (!string.IsNullOrEmpty(obj.StoreLocation))
                {
                    if (obj.StoreLocation != model.StoreLocation)
                    {
                        model.StoreLocation = obj.StoreLocation;
                    }
                }
                else
                {
                    ////获取柜内第一块组件
                    //ChestDetail chestDetail = new ChestDetail();
                    //PagingConfig cfg = new PagingConfig()
                    //{
                    //    IsPaging = false,
                    //    OrderBy = " ItemNo ",
                    //    Where = string.Format(@" Key = '{0}' and ItemNo = 1 ", obj.Key)
                    //};
                    //PackageInChestServiceClient client = new PackageInChestServiceClient();
                    //MethodReturnResult<IList<ChestDetail>> lstChestDetail = client.GetDetail(ref cfg);
                    //if (lstChestDetail.Data != null && lstChestDetail.Data.Count > 0)
                    //{
                    //    chestDetail = lstChestDetail.Data[0];
                    //    result.Code = 1;
                    //    result.Message = string.Format("柜({0})未设置库位，请使用包装清单打印界面选择库位并输入托号[{1}]查询按钮设置。"
                    //                                    , model.ChestNo,chestDetail.Key.ObjectNumber);
                    //    return Json(result);
                    //}
                    //else
                    //{
                    //    result.Code = 1;
                    //    result.Message = string.Format("柜({0})内无明细。", model.ChestNo);
                    //    return Json(result);
                    //}
                    if (model.StoreLocation == null || model.StoreLocation == "")
                    {
                        result.Code = 1;
                        result.Message = string.Format("柜({0})未设置库位，请在界面选择所需的库位后再点击手动完成入柜按钮。"
                                                        , model.ChestNo);
                        return Json(result);
                    }
                }
                model.IsFinishPackage = true;
                result = FinishChest(model);
                //result = Package(model);

                //返回包装结果。
                if (result.Code <= 0)
                {
                    MethodReturnResult<ChestViewModel> rstFinal = new MethodReturnResult<ChestViewModel>()
                    {
                        Code = result.Code,
                        Data = model,
                        Detail = result.Detail,
                        HelpLink = result.HelpLink,
                        Message = result.Message
                    };
                    return Json(rstFinal);
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

        //托号检验
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Check(ChestViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (model.PackageNo == null || model.PackageNo == "")
            {
                result.Code = 1001;
                result.Message = string.Format("托号不可为空。");
                return Json(result);
            }
            else
            {
                model.PackageNo = model.PackageNo.ToUpper().Trim();
            }
            try
            {
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    //判断托号是否已检验
                    MethodReturnResult<Package> rst1 = client.Get(model.PackageNo);
                    if (rst1.Code > 0)
                    {
                        rst1.Message += string.Format(@"或托号{0}已归档",model.PackageNo);
                        return Json(rst1);
                    }
                    else
                    {
                        if (rst1.Data != null)
                        {                            
                            #region 明细检验
                            if (rst1.Data.PackageState != EnumPackageState.Checked)
                            {
                                using (PackageInChestServiceClient clientOfChest = new PackageInChestServiceClient())
                                {
                                    result = clientOfChest.CheckPackageInChest(model.PackageNo, model.ChestNo, User.Identity.Name);
                                    if (result.Code <= 0)
                                    {
                                        result.Message = string.Format(@"托号{0}检验成功", model.PackageNo);
                                        model.ChestNo = result.Detail;
                                    }
                                }
                            }
                            else
                            {
                                using (PackageInChestServiceClient clientOfChest = new PackageInChestServiceClient())
                                {
                                    MethodReturnResult<Chest> rst2 = clientOfChest.Get(rst1.Data.ContainerNo);
                                    if (rst2.Code > 0)
                                    {
                                        return Json(rst2);
                                    }
                                    if (rst2.Code <= 0 && rst2.Data != null)
                                    {
                                        model.ChestNo = rst2.Data.Key;
                                        result.Message = string.Format(@"托号{0}已检验过，并检验成功", model.PackageNo);
                                        result.Detail = rst2.Data.Key;
                                        result.ObjectNo = Convert.ToInt32(rst2.Data.ChestState).ToString();
                                    }
                                }
                            }
                            #endregion
                        }                        
                    }
                }                                
                //返回包装结果。
                if (result.Code <= 0)
                {                    
                    MethodReturnResult<ChestViewModel> rstFinal = new MethodReturnResult<ChestViewModel>()
                    {
                        Code = result.Code,
                        Data = model,
                        Detail = result.Detail,
                        HelpLink = result.HelpLink,
                        Message = result.Message,
                        ObjectNo = ((EnumChestState)Convert.ToInt32(result.ObjectNo)).GetDisplayName()
                    };
                    return Json(rstFinal);
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

        //托号取消检验
        [HttpPost]
        public ActionResult UnCheck(string chestNo,string packageNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    //判断托号是否已检验
                    MethodReturnResult<Package> rst1 = client.Get(packageNo);
                    if (rst1.Code > 0)
                    {
                        return Json(rst1);
                    }
                    else
                    {
                        if (rst1.Data.PackageState != EnumPackageState.Checked)
                        {
                            result.Code = 2000;
                            result.Message = string.Format(@"托号{0}未检验", packageNo);
                        }
                        else
                        {
                            using (PackageInChestServiceClient clientOfChest = new PackageInChestServiceClient())
                            {
                                result = clientOfChest.UnCheckPackageInChest(packageNo, chestNo, User.Identity.Name);
                                if (result.Code > 0)
                                {
                                    return Json(result);
                                }
                            }
                        }
                    }
                }
                ChestViewModel model = new ChestViewModel()
                {
                    PackageNo = packageNo,
                    ChestNo = chestNo
                };

                //返回包装结果。
                if (result.Code <= 0)
                {
                    MethodReturnResult<ChestViewModel> rstFinal = new MethodReturnResult<ChestViewModel>()
                    {
                        Code = result.Code,
                        Data = model,
                        Detail = result.Detail,
                        HelpLink = result.HelpLink,
                        Message = result.Message,
                        ObjectNo = ((EnumChestState)Convert.ToInt32(result.ObjectNo)).GetDisplayName()
                    };
                    return Json(rstFinal);
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

        //托号出柜
        [HttpPost]
        public ActionResult UnPackageInChest(string chestNo, int itemNo, string packageNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (packageNo == null || packageNo == "")
            {
                result.Code = 1001;
                result.Message = string.Format("托号不可为空。");
                return Json(result);
            }
            
            //进行批次包装作业。
            ChestParameter p = new ChestParameter()
            {
                Editor = User.Identity.Name,
                ChestNo = chestNo.ToUpper(),
                PackageNo = packageNo.ToUpper(),
                ModelType = 0
            };

            try
            {
                //获取包装记录
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    MethodReturnResult<Chest> rst1 = client.Get(chestNo);
                    if (rst1.Code > 0 || rst1.Data == null)
                    {
                        return Json(rst1);
                    }
                    else
                    {
                        #region 注释--出柜条件
                        //if (rst1.Data.ChestState != EnumChestState.Packaging)
                        //{
                        //    result.Code = 1001;
                        //    result.Message = string.Format("柜[{0}]已完成入柜，不可出柜。", chestNo);
                        //    return Json(result);
                        //}
                        #endregion
                    }
                }
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    result = client.UnPackageInChest(p);

                    if (result.Code == 0)
                    {
                        result.Message = string.Format("托号[{0}]出柜（{1}）成功。"
                                                       ,p.PackageNo, p.ChestNo);
                    }

                }
                return Json(result);
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

        // 托号入柜作业---包装模型对象model
        private MethodReturnResult Chest(ChestViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            //进行包装入柜作业。
            ChestParameter p = new ChestParameter()
            {
                Editor = User.Identity.Name,
                ChestNo = model.ChestNo,
                IsFinishPackageInChest = model.IsFinishPackage,
                IsLastestPackageInChest = model.IsLastestPackageInChest,
                ChestFullQty = model.FullQuantity,
                StoreLocation=model.StoreLocation,
                PackageNo = model.PackageNo.ToUpper().Trim(),
                isManual = model.IsManual,
                ModelType = 0
            };

            if (model.CurrentQuantity == model.FullQuantity)
            {
                p.IsFinishPackageInChest = true;
                model.IsFinishPackage = true;
            }

            using (PackageInChestServiceClient client = new PackageInChestServiceClient())
            {
                result = client.Chest(p);
                string detailInfo = result.Detail;
                if (result.Code == 0 && model.IsFinishPackage == false)
                {
                    result.Message = string.Format("托号 {0} 成功入柜到（{1}）。"
                                                   , model.PackageNo.ToUpper().Trim()
                                                   , model.ChestNo);
                }
                else if (result.Code == 0 && model.IsFinishPackage == true)
                {
                    result = client.ChangeChest(model.ChestNo, User.Identity.Name);
                    if (result.Code <= 0)
                    {
                        result.Detail = detailInfo;
                        result.Message = string.Format("托号 {0} 成功入柜到（{1}）,柜号{1}入柜完成。"
                                                    , model.PackageNo.ToUpper().Trim(), model.ChestNo);
                    }
                    
                }
            }
            return result;
        }

        private MethodReturnResult FinishChest(ChestViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            //进行批次包装作业。
            ChestParameter p = new ChestParameter()
            {
                Editor = User.Identity.Name,
                ChestNo = model.ChestNo,
                IsFinishPackageInChest = model.IsFinishPackage,
                IsLastestPackageInChest = model.IsLastestPackageInChest,
                ChestFullQty = model.FullQuantity,
                StoreLocation = model.StoreLocation,
                PackageNo = model.PackageNo.ToUpper().Trim(),
                isManual = model.IsManual,
                ModelType = 0
            };

            using (PackageInChestServiceClient client = new PackageInChestServiceClient())
            {
                result = client.FinishChest(p);

                if (result.Code == 0 && model.IsFinishPackage == true)
                {
                    result.Message = string.Format("尾柜 {0} 已手动完成入柜。", model.ChestNo);
                }
            }
            return result;
        }

        // 获取托号信息
        private MethodReturnResult GetPackage(string packageNo)
        {            
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<Package> rst = null;
            Package obj = null;
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                rst = client.Get(packageNo);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    obj = rst.Data;
                }
                else
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.Detail = rst.Detail;
                    return result;
                }
            }
            if (obj == null)
            {
                result.Code = 2001;
                result.Message = string.Format(@"包装号[{0}]不存在。",packageNo);
                return result;
            }
            else if (obj.ContainerNo != null && obj.ContainerNo != "")
            {
                result.Code = 2002;
                result.Message = string.Format("包装号[{0}]已入柜[{1}]。", packageNo,obj.ContainerNo);
                return result;
            }
            //else if (obj.PackageState != EnumPackageState.ToWarehouse && obj.PackageState != EnumPackageState.Shipped)
            //{
            //    result.Code = 2003;
            //    result.Message = string.Format("包装号[{0}]未入库。", packageNo);
            //    return result;
            //}
            else if (obj.PackageState == EnumPackageState.Packaging 
                || obj.PackageState == EnumPackageState.InFabStore
                || obj.PackageState == EnumPackageState.Shipped)
            {
                result.Code = 2003;
                result.Message = string.Format("包装号[{0}]当前状态[{1}],不可入柜！", packageNo,obj.PackageState.GetDisplayName());
                return result;
            }
            return rst;
        }

        // 获取柜信息--当前数量/是否尾包/库位
        public ActionResult GetChestInfo(string chestNo)
        {
            double currentQuantity = 0;
            bool isLastestPackage = false;
            string storeLocation = "";
            double fullQuantity = 0;
            int code = 0;

            if (!string.IsNullOrEmpty(chestNo))
            {
                //获取当前数量
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    MethodReturnResult<Chest> rst2 = client.Get(chestNo);
                    if (rst2.Code == 1000)
                    {
                        return Json(rst2);
                    }                   
                    if (rst2.Code <= 0 && rst2.Data != null)
                    {
                        using (MaterialChestParameterServiceClient client1 = new MaterialChestParameterServiceClient())
                        {
                            MethodReturnResult<MaterialChestParameter> rst3 = client1.Get(rst2.Data.MaterialCode);
                            if (rst3.Data != null)
                            {
                                fullQuantity = rst3.Data.FullChestQty;
                            }
                            else
                            {
                                return Json(rst3, JsonRequestBehavior.AllowGet);
                            }
                        }
                        currentQuantity = rst2.Data.Quantity;
                        isLastestPackage = rst2.Data.IsLastPackage;
                        storeLocation = rst2.Data.StoreLocation;
                    }
                }
            }

            return Json(new
            {
                Code = code,
                CurrentQuantity = currentQuantity,
                IsLastestPackage = isLastestPackage,
                StoreLocation = storeLocation,
                FullQuantity = fullQuantity
            }, JsonRequestBehavior.AllowGet);
        }       
    }
}