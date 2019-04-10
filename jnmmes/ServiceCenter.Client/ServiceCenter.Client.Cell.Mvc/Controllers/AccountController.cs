using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Models;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.RBAC;
using System.Web.Security;
using ServiceCenter.Client.Mvc.Resources;

namespace ServiceCenter.Client.Mvc.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
                {
                    MethodReturnResult<User> result = await client.AuthenticateAsync(model.UserName, model.Password);
                    if (result.Code == 0 && result.Data!=null)
                    {
                        result.Data.LastLoginTime = DateTime.Now;
                        result.Data.LastLoginIP = HttpContext.Request.UserHostAddress;
                        using (UserServiceClient usclient = new UserServiceClient())
                        {
                            MethodReturnResult rstl = await usclient.ModifyAsync(result.Data);
                            if (rstl.Code > 0)
                            {
                                AddErrors(result);
                            }
                        }

                        SignIn(result.Data, model.RememberMe);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        private void SignIn(User user, bool isPersistent)
        {
            //创建一个FormsAuthenticationTicket，它包含登录名以及额外的用户数据。
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                user.Key,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                isPersistent,
                user.UserName??string.Empty,
                FormsAuthentication.FormsCookiePath);
            //加密Ticket，变成一个加密的字符串。
            string cookieValue = FormsAuthentication.Encrypt(ticket);
            //根据加密结果创建登录Cookie
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue);
            cookie.HttpOnly = true;
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Domain = FormsAuthentication.CookieDomain;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (isPersistent)
            {
                cookie.Expires = DateTime.Now.AddDays(1);
            }
            //写登录Cookie
            HttpContext.Response.Cookies.Remove(cookie.Name);
            HttpContext.Response.Cookies.Add(cookie);
            HttpContext.Cache.Remove(User.Identity.Name);
            
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User u = new User() { 
                    Key=model.LoginName,
                    Password=model.Password,
                    UserName=model.UserName
                };

                using(UserServiceClient client=new UserServiceClient())
                {
                    MethodReturnResult result=await client.AddAsync(u);
                    if (result.Code == 0)
                    {
                        SignIn(u, isPersistent:false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }
        //
        // GET: /Account/ModifyInfo
        public ActionResult ModifyInfo(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangeInfoSuccess ? "修改信息成功。"
                : message == ManageMessageId.Error ? "出现错误。"
                : "";
            ViewBag.ReturnUrl = Url.Action("ModifyInfo");

            using (UserServiceClient client = new UserServiceClient())
            {
                MethodReturnResult<User> result = client.Get(User.Identity.Name);
                if (result.Code == 0)
                {
                    ModifyInfoViewModel model = new ModifyInfoViewModel()
                    {
                        Email=result.Data.Email,
                        MobilePhone=result.Data.MobilePhone,
                        OfficePhone=result.Data.OfficePhone
                    };
                    return View(model);
                }
            }
            return View();
        }
        //
        // POST: /Account/ModifyInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ModifyInfo(ModifyInfoViewModel model)
        {
            ViewBag.ReturnUrl = Url.Action("ModifyInfo");
            if (ModelState.IsValid)
            {
                using (UserServiceClient client = new UserServiceClient())
                {
                    MethodReturnResult<User> result = await client.GetAsync(User.Identity.Name);
                    //获取到用户数据。
                    if (result.Code == 0)
                    {
                        result.Data.Email = model.Email;
                        result.Data.MobilePhone = model.MobilePhone;
                        result.Data.OfficePhone = model.OfficePhone;
                        result.Data.Editor = HttpContext.User.Identity.Name;
                        result.Data.EditTime = DateTime.Now;

                        MethodReturnResult rst = await client.ModifyAsync(result.Data);
                        //修改成功。
                        if (rst.Code == 0)
                        {
                            return RedirectToAction("ModifyInfo", new { Message = ManageMessageId.ChangeInfoSuccess });
                        }
                        else
                        {
                            AddErrors(rst);
                        }
                    }
                    else
                    {//出现错误。
                        AddErrors(result);
                    }
                }
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }
        //
        // GET: /Account/ModifyPassword
        public ActionResult ModifyPassword(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                     message == ManageMessageId.ChangePasswordSuccess ? "你的密码已更改。"
                     : message == ManageMessageId.Error ? "出现错误。"
                     : "";
            ViewBag.ReturnUrl = Url.Action("ModifyPassword");
            return View();
        }
        //
        // POST: /Account/ModifyPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ModifyPassword(ModifyPasswordViewModel model)
        {
            ViewBag.ReturnUrl = Url.Action("ModifyPassword");
            if (ModelState.IsValid)
            {
                using(UserServiceClient client=new UserServiceClient())
                {
                    MethodReturnResult<User>  result = await client.GetAsync(User.Identity.Name);
                    //获取到用户数据。
                    if (result.Code==0)
                    {
                        //旧密码输入正确。
                        if (result.Data.Password == model.OldPassword)
                        {
                            //设置新密码。
                            result.Data.Password = model.NewPassword;
                            result.Data.Editor = HttpContext.User.Identity.Name;
                            result.Data.EditTime = DateTime.Now;

                            MethodReturnResult rst = await client.ModifyAsync(result.Data);
                            //修改成功。
                            if (rst.Code == 0)
                            {
                                return RedirectToAction("ModifyPassword", new { Message = ManageMessageId.ChangePasswordSuccess });
                            }
                            else
                            {
                                AddErrors(rst);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("C1000", StringResource.AccountController_OldPasswordError);
                        }
                    }
                    else
                    {//出现错误。
                        AddErrors(result);
                    }
                }
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }


        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.Cache.Remove(User.Identity.Name);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #region 帮助程序
        private void AddErrors(MethodReturnResult result)
        {
            ModelState.AddModelError("", result.Message);
        }


        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            ChangeInfoSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion
    }
}