using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class JsonModel
    {
        public object Data { get; set; }
        public string Msg { get; set; }
        public string Statu { get; set; }
        public string BackUrl { get; set; }
    }
}