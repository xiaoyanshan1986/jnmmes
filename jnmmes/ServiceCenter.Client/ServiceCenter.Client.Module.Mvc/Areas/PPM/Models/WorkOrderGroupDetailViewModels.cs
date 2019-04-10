
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.PPM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.PPM;
using System.Text;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Models
{
    public class WorkOrderGroupDetailViewModel 
    {
        public WorkOrderGroupDetailViewModel()
        {
            
        }

        [Required]
        [Display(Name = "产品编码")]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ProductCode { get; set; }

        [Required]
        [Display(Name = "工单号")]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "混工单组")]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string WorkOrderGroupNo { get; set; }
  

        [Display(Name = "项目号")]
        public int ItemNo { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        //获取混工单组号
        public IEnumerable<SelectListItem> GetWorkOrderGroupNoList()
        {
            using (WorkOrderGroupDetailServiceClient client = new WorkOrderGroupDetailServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<WorkOrderGroupDetail>> result = client.Gets(ref cfg);
                if (result.Code <= 0)
                {
                    List<string> ii = new List<string>();
                    for (int i = 0; i < result.Data.Count; i++)
                    {
                        ii.Add(result.Data[i].Key.WorkOrderGroupNo.ToString());
                    }
                        //result.Data.Distinct();
                        return from item in ii.Distinct()
                               select new SelectListItem()
                               {
                                   Text = item,
                                   Value = item
                                   //,Selected = false
                               }; 

                }
            }
            return new List<SelectListItem>();
        }

        //获取工单明细
        public WorkOrder GetWorkOrder(string orderNumber)
        {
            WorkOrder workOrder = new WorkOrder();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = client.Get(orderNumber);
                if (result.Code <= 0)
                {
                    workOrder = result.Data;
                }
            }
            return workOrder;
        }

        //初始化混工单组号的值
        public string SetWorkOrderGroupNo()
        {
            string WorkOrderGroupNo = "";
            using (WorkOrderGroupDetailServiceClient client = new WorkOrderGroupDetailServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                    //,OrderBy = "Key.WorkOrderGroupNo Desc"
                };

                MethodReturnResult<IList<WorkOrderGroupDetail>> result = client.Gets(ref cfg);
                if (result.Code <= 0 && result.Data.Count > 0)
                {
                    List<int> ii = new List<int>();
                    for (int h = 0; h < result.Data.Count; h++)
                    {
                        string ss = result.Data[h].Key.WorkOrderGroupNo.ToString();
                        ii.Add(Convert.ToInt32(result.Data[h].Key.WorkOrderGroupNo.ToString().Substring(5, ss.Length - 5)));
                    }
                    //获取最大序列号
                    int temp = 0;
                    for (int x = 0; x < ii.Count; x++)
                    {
                        if (temp < ii[x])
                        {
                            temp = ii[x];
                        }
                    }

                    //string n = result.Data[0].Key.WorkOrderGroupNo.ToString();
                    //string j = result.Data[0].Key.WorkOrderGroupNo.ToString().Substring(5, n.Length - 5);
                    //int i = Convert.ToInt32(j) + 1;
                    int i = temp + 1;

                    WorkOrderGroupNo = "GROUP" + Convert.ToString(i);
                }
                else
                {
                    WorkOrderGroupNo = "GROUP1";
                }
            }
            return WorkOrderGroupNo;
        }

        
    }
    public class WorkOrderGroupQueryDetailViewModel
    {
        public WorkOrderGroupQueryDetailViewModel()
        {

        }

        [Display(Name = "产品编码")]
        public string ProductCode { get; set; }

        [Display(Name = "工单号")]
        public string OrderNumber { get; set; }

        [Display(Name = "混工单组")]
        public string WorkOrderGroupNo { get; set; }
        
    }

}