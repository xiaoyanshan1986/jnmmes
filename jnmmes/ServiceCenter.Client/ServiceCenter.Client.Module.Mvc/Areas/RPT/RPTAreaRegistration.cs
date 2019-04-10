using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.RPT
{
    public class RPTAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RPT";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RPT_default",
                "RPT/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}