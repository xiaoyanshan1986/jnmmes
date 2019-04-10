using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RBACMenuAuthorizeAttribute());
        }
    }

    public class  RBACMenuAuthorizeAttribute: AuthorizeAttribute
    {
        protected MethodReturnResult AuthenticateResult{get;set;}

        private string _actionPath = string.Empty;
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuthorize = base.AuthorizeCore(httpContext);
            if (isAuthorize)
            {
                try
                {
                    string loginName = httpContext.User.Identity.Name;
                    TimeSpan SessTimeOut = new TimeSpan(0, 0, System.Web.HttpContext.Current.Session.Timeout, 0, 0);
                    if (httpContext.Cache.Get(loginName) == null)
                    {
                        httpContext.Cache.Insert(loginName, httpContext.Session.SessionID, null, DateTime.MaxValue, SessTimeOut);
                    }

                    string curPath = this._actionPath;
                    if (string.IsNullOrEmpty(curPath))
                    {
                        curPath = httpContext.Request.CurrentExecutionFilePath;
                    }
                    using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
                    {
                        this.AuthenticateResult = client.AuthenticateResource(loginName, ResourceType.Menu, curPath);
                        if (this.AuthenticateResult.Code > 0)
                        {
                            isAuthorize = false;
                        }
                    }
                }
                catch(Exception ex)
                {
                    this.AuthenticateResult = new MethodReturnResult();
                    this.AuthenticateResult.Code = 1000;
                    this.AuthenticateResult.Message = ex.Message;
                    this.AuthenticateResult.Detail = ex.ToString();
                    isAuthorize = false;
                }
            }
            return isAuthorize;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            StringBuilder sbActionPath = new StringBuilder();
            if (filterContext.RouteData.DataTokens.ContainsKey("area"))
            {
                sbActionPath.AppendFormat("/{0}",filterContext.RouteData.DataTokens["area"]);
            }
            foreach(KeyValuePair<string, object> val in filterContext.RouteData.Values)
            {
                sbActionPath.AppendFormat("/{0}",val.Value);
            }
            this._actionPath = sbActionPath.ToString();
            base.OnAuthorization(filterContext);
        }

        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            return base.OnCacheAuthorization(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult() { Data = this.AuthenticateResult };
                }
                else
                {
                    ViewResult view = new ViewResult() { ViewName = "Unauthorized" };
                    view.ViewBag.AuthenticateResult = this.AuthenticateResult;
                    filterContext.Result = view;
                }
            }
        }
    }


}
