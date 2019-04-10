using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.DataAccess.Interface.ZPVC;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.MES.Service.Contract.ZPVC;
using ServiceCenter.MES.Service.ZPVC.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.ZPVC
{
    /// <summary>
    /// 实现装箱管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class BoxService : IBoxContract
    {
        /// <summary>
        /// 包装数据数据访问读写。
        /// </summary>
        public IPackageDataEngine PackageDataEngine { get; set; }
        /// <summary>
        /// 包装明细数据数据访问读写。
        /// </summary>
        public IPackageDetailDataEngine PackageDetailDataEngine { get; set; }
        /// <summary>
        /// 包装信息数据数据访问读写。
        /// </summary>
        public IPackageInfoDataEngine PackageInfoDataEngine { get; set; }

        /// <summary>
        /// 装箱。
        /// </summary>
        /// <param name="p">装箱参数类。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        public MethodReturnResult Box(BoxParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(p.BoxNo))
                {
                    p.BoxNo = p.BoxNo.ToUpper();
                }
                //获取箱数据
                Package box = this.PackageDataEngine.Get(p.BoxNo??string.Empty);
                //获取包数据
                Package package = this.PackageDataEngine.Get(p.PackageNo ?? string.Empty);
                PackageInfo packageInfo = this.PackageInfoDataEngine.Get(p.PackageNo ?? string.Empty);
                if (package == null || packageInfo==null || package.PackageType!=EnumPackageType.Packet)
                {
                    result.Code = 2001;
                    result.Message = string.Format("电池小包 {0} 不存在。", p.PackageNo);
                    return result;
                }
                else if (package.PackageState == EnumPackageState.Packaged)
                {
                    result.Code = 2001;
                    result.Message = string.Format("电池小包 {0} 已装箱完成。", p.PackageNo);
                    return result;
                }

                //如果箱数据不为空
                if(box!=null)
                {
                    //获取箱的第一包数据
                    PagingConfig cfg=new PagingConfig(){
                        PageNo=0,
                        PageSize=1,
                        Where=string.Format("Key.PackageNo='{0}' AND Key.ObjectType='{1}'"
                                            ,box.Key
                                            ,Convert.ToInt32(EnumPackageObjectType.Packet)),
                        OrderBy = "ItemNo"
                    };
                    IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
                    if(lstPackageDetail.Count>0)
                    {
                        Package firstPackage = this.PackageDataEngine.Get(lstPackageDetail[0].Key.ObjectNumber);
                        PackageInfo firstPackageInfo = this.PackageInfoDataEngine.Get(firstPackage.Key);

                        #region //判断箱信息和包信息是否一致
                        if (packageInfo.ProductId != firstPackageInfo.ProductId)
                        {
                            result.Code = 2002;
                            result.Message = string.Format("电池小包 {0} 产品编号（{1}）同箱的产品编号（{2}）不一致。"
                                                            , p.PackageNo
                                                            , packageInfo.ProductId
                                                            , firstPackageInfo.ProductId);
                            return result;
                        }

                        if (packageInfo.EfficiencyName != firstPackageInfo.EfficiencyName)
                        {
                            result.Code = 2002;
                            result.Message = string.Format("电池小包 {0} 效率（{1}）同箱的效率（{2}）不一致。"
                                                            , p.PackageNo
                                                            , packageInfo.EfficiencyName
                                                            , firstPackageInfo.EfficiencyName);
                            return result;
                        }

                        if (packageInfo.Grade != firstPackageInfo.Grade)
                        {
                            result.Code = 2002;
                            result.Message = string.Format("电池小包 {0} 等级（{1}）同箱的等级（{2}）不一致。"
                                                            , p.PackageNo
                                                            , packageInfo.Grade
                                                            , firstPackageInfo.Grade);
                            return result;
                        }

                        if (packageInfo.Color != firstPackageInfo.Color)
                        {
                            result.Code = 2002;
                            result.Message = string.Format("电池小包 {0} 颜色（{1}）同箱的颜色（{2}）不一致。"
                                                            , p.PackageNo
                                                            , packageInfo.Color
                                                            , firstPackageInfo.Color);
                            return result;
                        }

                        if (packageInfo.PNType != firstPackageInfo.PNType)
                        {
                            result.Code = 2002;
                            result.Message = string.Format("电池小包 {0} 导电类型（{1}）同箱的导电类型（{2}）不一致。"
                                                            , p.PackageNo
                                                            , packageInfo.PNType
                                                            , firstPackageInfo.PNType);
                            return result;
                        }

                        if (packageInfo.ConfigCode != firstPackageInfo.ConfigCode)
                        {
                            result.Code = 2002;
                            result.Message = string.Format("电池小包 {0} 分类编号（{1}）同箱的分类编号（{2}）不一致。"
                                                            , p.PackageNo
                                                            , packageInfo.ConfigCode
                                                            , firstPackageInfo.ConfigCode);
                            return result;
                        }
                        #endregion
                    }
                }

                //using(TransactionScope ts=new TransactionScope())
                ISession session = this.PackageDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    int itemNo = 1;
                    //如果箱数据为空
                    if (box == null)
                    {
                        //新增箱数据
                        box = new Package()
                        {
                            CreateTime = DateTime.Now,
                            Creator = p.Creator,
                            Editor = p.Creator,
                            EditTime = DateTime.Now,
                            IsLastPackage = false,
                            Key = p.BoxNo,
                            Quantity = package.Quantity,
                            MaterialCode = package.MaterialCode,
                            OrderNumber = package.OrderNumber,
                            PackageState = EnumPackageState.Packaging,
                            PackageType = EnumPackageType.Box
                        };
                        this.PackageDataEngine.Insert(box,session);
                    }
                    else
                    {
                        Package boxUpdate = box.Clone() as Package;
                        boxUpdate.Quantity += package.Quantity;
                        boxUpdate.EditTime = DateTime.Now;
                        boxUpdate.Editor = p.Creator;
                        this.PackageDataEngine.Update(boxUpdate,session);

                        //获取箱中包的最大项目号。
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key.PackageNo='{0}' AND Key.ObjectType='{1}'"
                                                , box.Key
                                                , Convert.ToInt32(EnumPackageObjectType.Packet)),
                            OrderBy = "ItemNo Desc"
                        };
                        IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg,session);
                        if (lstPackageDetail != null && lstPackageDetail.Count > 0)
                        {
                            itemNo = lstPackageDetail[0].ItemNo + 1;
                        }
                    }
                    //关联箱和包数据。
                    PackageDetail pd = new PackageDetail()
                    {
                        CreateTime=DateTime.Now,
                        Creator=p.Creator,
                        OrderNumber=package.OrderNumber,
                        MaterialCode=package.MaterialCode,
                        Key = new PackageDetailKey()
                        {
                            ObjectNumber=package.Key,
                            ObjectType=EnumPackageObjectType.Packet,
                            PackageNo=box.Key
                        },
                        ItemNo=itemNo
                    };
                    this.PackageDetailDataEngine.Insert(pd,session);
                    //更新电池小包数据。
                    Package packageUpadte = package.Clone() as Package;
                    packageUpadte.Editor = p.Creator;
                    packageUpadte.EditTime = DateTime.Now;
                    packageUpadte.PackageState = EnumPackageState.Packaged;
                    this.PackageDataEngine.Update(packageUpadte,session);

                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 拆箱。
        /// </summary>
        /// <param name="p">拆箱参数类。</param>
        /// <returns>MethodReturnResult.</returns>
        public MethodReturnResult Unbox(UnboxParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                //获取箱数据
                Package box = this.PackageDataEngine.Get(p.BoxNo ?? string.Empty);
                if (box == null || box.PackageType != EnumPackageType.Box)
                {
                    result.Code = 2001;
                    result.Message = string.Format("箱 {0} 不存在。", p.BoxNo);
                    return result;
                }
                else if (box.PackageState == EnumPackageState.Packaged)
                {
                    result.Code = 2001;
                    result.Message = string.Format("箱 {0} 已完成。", p.BoxNo);
                    return result;
                }
                Package package = this.PackageDataEngine.Get(p.PackageNo);

                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.PackageDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    //删除箱包数据
                    PackageDetailKey key = new PackageDetailKey()
                    {
                        PackageNo = p.BoxNo,
                        ObjectType = EnumPackageObjectType.Packet,
                        ObjectNumber = p.PackageNo
                    };
                    this.PackageDetailDataEngine.Delete(key,session);
                    //更新剩余的箱包数据。
                    //获取箱中所有的包数据。
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.PackageNo='{0}' AND Key.ObjectType='{1}'"
                                            , box.Key
                                            , Convert.ToInt32(EnumPackageObjectType.Packet)),
                        OrderBy = "ItemNo"
                    };
                    IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg,session);
                    int itemNo = 1;
                    foreach (PackageDetail pd in lstPackageDetail)
                    {
                        if (pd.ItemNo != itemNo)
                        {
                            PackageDetail pdUpdate = pd.Clone() as PackageDetail;
                            pdUpdate.ItemNo = itemNo;
                            this.PackageDetailDataEngine.Update(pdUpdate,session);
                        }
                        itemNo++;
                    }
                    //更新箱数据
                    Package boxUpdate = box.Clone() as Package;
                    boxUpdate.EditTime = DateTime.Now;
                    boxUpdate.Editor = p.Creator;
                    boxUpdate.Quantity -= (package != null ? package.Quantity : 0);
                    this.PackageDataEngine.Update(boxUpdate,session);
                    //更新包数据
                    if (package != null)
                    {
                        Package packageUpdate = package.Clone() as Package;
                        packageUpdate.Editor = p.Creator;
                        packageUpdate.EditTime = DateTime.Now;
                        packageUpdate.PackageState = EnumPackageState.Packaging;
                        this.PackageDataEngine.Update(packageUpdate,session);
                    }

                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }
    }
}
