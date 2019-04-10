using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.FMM;
using FMMResources = ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.FMM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class MaterialPrintSetQueryViewModel
    {
        public MaterialPrintSetQueryViewModel()
        {

        }

        /// <summary>
        /// 产品代码
        /// </summary>
        [Display(Name = "MaterialPrintSetViewModel_MaterialCode", ResourceType = typeof(FMMResources.StringResource))]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [Display(Name = "MaterialPrintSetViewModel_MaterialName", ResourceType = typeof(FMMResources.StringResource))]
        public string MaterialName { get; set; }
    }

    public class MaterialPrintSetViewModel
    {
        public MaterialPrintSetViewModel()
        {
            this.Qty = 1;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        /// <summary>
        /// 产品代码
        /// </summary>
        [Required]
        [Display(Name = "MaterialPrintSetViewModel_MaterialCode", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 标签代码
        /// </summary>
        [Required]
        [Display(Name = "MaterialPrintSetViewModel_LabelCode", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string LabelCode { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        [Required]
        [Display(Name = "MaterialPrintSetViewModel_LabelName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string LabelName { get; set; }

        /// <summary>
        /// 打印数量
        /// </summary>
        [Required]
        [Display(Name = "MaterialPrintSetViewModel_Qty", ResourceType = typeof(FMMResources.StringResource))]
        [Range(1, 65536
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int Qty { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        /// <summary>
        /// 编辑日期
        /// </summary>
        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        /// <summary>
        /// 取得打印标签列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetLabelCodeList()
        {
            using (PrintLabelServiceClient client = new PrintLabelServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = "IsUsed='1'",
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<PrintLabel>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key + "-" + item.Name,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }

            return new List<SelectListItem>();
        }

        /// <summary>
        /// 根据标签代码取得标签对象
        /// </summary>
        /// <param name="labelCode"></param>
        /// <returns></returns>
        public PrintLabel GetPrintLabel(string labelCode)
        {
            using (PrintLabelServiceClient client = new PrintLabelServiceClient())
            {
                MethodReturnResult<PrintLabel> result = client.Get(labelCode);

                if (result.Code <= 0)
                {
                    return result.Data;
                }
            }

            return new PrintLabel();
        }
    }
}