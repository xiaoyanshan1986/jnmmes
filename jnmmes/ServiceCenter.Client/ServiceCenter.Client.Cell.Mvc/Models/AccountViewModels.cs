using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ServiceCenter.Client.Mvc.Models
{
    public class ModifyInfoViewModel
    {
        [Display(Name = "电子邮件")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "{0} 格式不正确。")]
        public string Email { get; set; }

        [Display(Name = "办公电话")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "{0} 格式不正确。")]
        public string OfficePhone { get; set; }

        [Display(Name = "移动电话")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "{0} 格式不正确。")]
        public string MobilePhone { get; set; }
    }


    public class ModifyPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel:Areas.RBAC.Models.UserViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }


    public class MenuViewModel
    {
        public IList<Resource> GetMenuResource(string loginName)
        {
            string cacheName = loginName+"_Menu";

            IList<Resource> lstMenu = HttpContext.Current.Cache[cacheName] as IList<Resource>;
            if (lstMenu == null)
            {
                using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
                {

                    MethodReturnResult<IList<Resource>> result = client.GetResourceList(loginName, ResourceType.Menu);
                    if (result.Code == 0)
                    {
                        TimeSpan SessTimeOut = new TimeSpan(0, 0, System.Web.HttpContext.Current.Session.Timeout, 0, 0); 
                        HttpContext.Current.Cache.Add(cacheName
                                                     , result.Data
                                                     , new System.Web.Caching.CacheDependency(null, new string[] { HttpContext.Current.User.Identity.Name })
                                                     , System.Web.Caching.Cache.NoAbsoluteExpiration
                                                     , SessTimeOut
                                                     , System.Web.Caching.CacheItemPriority.Default
                                                     , null);
                        return result.Data;
                    }
                }
            }
            return lstMenu;
        }

        public IList<Resource> GetMenuItemResource(string loginName)
        {
            string cacheName = loginName + "_MenuItem";

            IList<Resource> lstMenu = HttpContext.Current.Cache[cacheName] as IList<Resource>;
            if (lstMenu == null)
            {
                using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
                {
                    MethodReturnResult<IList<Resource>> result = client.GetResourceList(loginName, ResourceType.MenuItem);
                    if (result.Code == 0)
                    {
                        TimeSpan SessTimeOut = new TimeSpan(0, 0, System.Web.HttpContext.Current.Session.Timeout, 0, 0); 

                        HttpContext.Current.Cache.Add(cacheName
                                                         , result.Data
                                                         , new System.Web.Caching.CacheDependency(null, new string[] { HttpContext.Current.User.Identity.Name })
                                                         , System.Web.Caching.Cache.NoAbsoluteExpiration
                                                         , SessTimeOut
                                                         , System.Web.Caching.CacheItemPriority.Default
                                                         , null);
                        return result.Data;
                    }
                }
            }
            return lstMenu;
        }

    }
}
