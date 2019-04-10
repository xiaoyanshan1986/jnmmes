using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.EDC
{
    public class EDCAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "EDC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "EDC_default",
                "EDC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}