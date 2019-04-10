using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.BaseData
{
    public class BaseDataAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BaseData";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            
            context.MapRoute(
                "BaseData_default",
                "BaseData/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}