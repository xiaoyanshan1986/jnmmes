using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.Model.EMS;
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.MES.Service.WIP.ServiceExtensions
{
    /// <summary>
    /// 扩展批次包装检查，进行包装特性检查操作。
    /// </summary>
    class LotPackageForCheckPackageAttribute : ILotPackageCheck
    {
        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 批次属性数据访问类。
        /// </summary>
        public ILotAttributeDataEngine LotAttributeDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public IBaseAttributeDataEngine BaseAttributeDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 包装数据访问类。
        /// </summary>
        public IPackageDataEngine PackageDataEngine { get; set; }
        /// <summary>
        /// 包装明细数据访问类。
        /// </summary>
        public IPackageDetailDataEngine PackageDetailDataEngine { get; set; }

        /// <summary>
        /// 在批次包装时，进行批次检查操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Check(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (p.LotNumbers==null)
            {
                return result;
            }

            //获取包装记录。
            Package packageObj = this.PackageDataEngine.Get(p.PackageNo);
            if (packageObj == null || packageObj.Quantity==0)
            {
                return result;
            }

            //获取包装特性。
            PagingConfig cfg = new PagingConfig()
            {
               IsPaging=false,
               Where = string.Format("Key.CategoryName='{0}'","LotPackageAttribute")
            };
            IList<BaseAttribute> lstAttribute=this.BaseAttributeDataEngine.Get(cfg);

            //获取包装明细中的第一条记录。
            cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format("Key.PackageNo='{0}' AND ItemNo=1",p.PackageNo)
            };

            IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
            if (lstPackageDetail.Count == 0)
            {
                return result;
            }
            PackageDetail packageDetailObj = lstPackageDetail[0];
            //获取批次特性记录。
            cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format("Key.LotNumber='{0}'", packageDetailObj.Key.ObjectNumber)
            };
            IList<LotAttribute> lstLotAttribute = this.LotAttributeDataEngine.Get(cfg);
            Dictionary<string, string> dicAttribute = new Dictionary<string, string>();

            foreach(BaseAttribute item in lstAttribute)
            {
                var lnq = from litem in lstLotAttribute
                          where litem.Key.AttributeName == item.Key.AttributeName
                          select litem.AttributeValue;
                dicAttribute.Add(item.Key.AttributeName, lnq.FirstOrDefault());
            }
            //获取包等级。
            Lot lot = this.LotDataEngine.Get(packageDetailObj.Key.ObjectNumber);
            string packageGrade = lot.Grade;


            foreach(string lotNumber in p.LotNumbers)
            {
                //检查等级
                lot = this.LotDataEngine.Get(lotNumber);
                string grade = lot.Grade;
                if (grade != packageGrade)
                {
                    result.Code = 3000;
                    result.Message = string.Format("批次等级值（{0}）和包等级值（{1}）不匹配，无法进行包装。"
                                                    , grade
                                                    , packageGrade);
                    return result;
                }
                //检查批次特性。
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.LotNumber='{0}'", lotNumber)
                };
                lstLotAttribute = this.LotAttributeDataEngine.Get(cfg);

                foreach (BaseAttribute item in lstAttribute)
                {
                    var lnq = from litem in lstLotAttribute
                              where litem.Key.AttributeName == item.Key.AttributeName
                              select litem.AttributeValue;
                    //检查必需特性值是否有值。
                    if (item.IsPrimaryKey && lnq.Count() == 0)
                    {
                        result.Code = 3001;
                        result.Message = string.Format("批次特性（{0}）没有设置值，无法进行包装。", item.Key.AttributeName);
                        return result;
                    }
                    string val=lnq.SingleOrDefault();
                    string packageVal=dicAttribute[item.Key.AttributeName];
                    //检查包特性和批次特性是否一致。
                    if (packageVal!=val)
                    {
                        result.Code = 3002;
                        result.Message = string.Format("批次特性（{0}）值（{1}）和包（{2}）特性值（{3}）不匹配，无法进行包装。"
                                                        , item.Key.AttributeName
                                                        , val
                                                        , p.PackageNo
                                                        , packageVal);
                        return result;
                    }
                }
            }
            return result;
        }

    }
}
