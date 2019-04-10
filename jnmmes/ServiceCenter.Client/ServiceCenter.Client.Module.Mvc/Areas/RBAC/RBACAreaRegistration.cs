using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.RBAC
{
    public class RBACAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RBAC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RBAC_default",
                "RBAC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}