// ***********************************************************************
// Assembly         : ServiceCenter.Client.Mvc
// Author           : peter
// Created          : 08-01-2014
//
// Last Modified By : peter
// Last Modified On : 08-06-2014
// ***********************************************************************
// <copyright file="UserController.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json;
using ServiceCenter.Client.Mvc.Areas.RBAC.Models;
using ServiceCenter.Client.Mvc.Resources.RBAC;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

/// <summary>
/// The Controllers namespace.
/// </summary>
namespace ServiceCenter.Client.Mvc.Areas.RBAC.Controllers
{
    /// <summary>
    /// Class UserController.
    /// </summary>
    [Authorize]
    public class UserController : Controller
    {
        //
        // GET: /RBAC/User/
        /// <summary>
        /// 首页。
        /// </summary>
        /// <returns>ActionResult.</returns>
        public async Task<ActionResult> Index()
        {
            using (UserServiceClient client = new UserServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig();
                    MethodReturnResult<IList<User>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.UserList = result.Data;
                    }
                });
            }
            return View();
        }
        //
        // POST: /RBAC/User/Query
        /// <summary>
        /// 查询。
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="currentPageNo">The current page no.</param>
        /// <param name="currentPageSize">Size of the current page.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(UserQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (UserServiceClient client = new UserServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.LoginName))
                            {
                                where.AppendFormat(" Key LIKE '{0}%'", model.LoginName);
                            }
                            if (!string.IsNullOrEmpty(model.UserName))
                            {
                                where.AppendFormat(" {0} UserName LIKE '{1}%'", 
                                                   where.Length>0?"AND":string.Empty,
                                                   model.UserName);
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            Where=where.ToString()
                        };
                        MethodReturnResult<IList<User>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.UserList = result.Data;
                        }
                    });
                }
            } 
            return PartialView("_UserListPartial");
        }
        //
        // POST: /RBAC/User/PagingQuery
        /// <summary>
        /// Pagings the query.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="currentPageNo">The current page no.</param>
        /// <param name="currentPageSize">Size of the current page.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
        {
            if (ModelState.IsValid)
            {
                int pageNo = currentPageNo??0;
                int pageSize = currentPageSize ?? 20;
                if (Request["PageNo"] != null)
                {
                    pageNo = Convert.ToInt32(Request["PageNo"]);
                }
                if (Request["PageSize"] != null)
                {
                    pageSize = Convert.ToInt32(Request["PageSize"]);
                }

                using (UserServiceClient client = new UserServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = pageNo,
                            PageSize = pageSize,
                            Where = where??string.Empty,
                            OrderBy = orderBy ?? string.Empty
                        };
                        MethodReturnResult<IList<User>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.UserList = result.Data;
                        }
                    });
                }
            }
            return PartialView("_UserListPartial");
        }
        //
        // POST: /RBAC/User/SaveUser
        /// <summary>
        /// Saves the user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveUser(UserViewModel model)
        {
            using (UserServiceClient client = new UserServiceClient())
            {
                User u=new User(){
                    UserName = model.UserName,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    IsActive = model.IsActive,
                    IsApproved = model.IsApproved,
                    IsLocked = model.IsLocked,
                    Email = model.Email,
                    MobilePhone = model.MobilePhone,
                    OfficePhone = model.OfficePhone,
                    Password = model.Password,
                    CreateTime=DateTime.Now,
                    Creator=User.Identity.Name,
                    Key=model.LoginName
                };
                MethodReturnResult rst = await client.AddAsync(u);
                if (rst.Code == 0)
                {
                    rst.Message=string.Format(StringResource.User_SaveUser_Success,u.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /RBAC/User/Modify
        /// <summary>
        /// Modifies the specified login name.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <returns>ActionResult.</returns>
        public async Task<ActionResult> Modify(string loginName)
        {
            using (UserServiceClient client = new UserServiceClient())
            {
                MethodReturnResult<User> result =await client.GetAsync(loginName??string.Empty);
                if (result.Code == 0)
                {
                    UserViewModel viewModel = new UserViewModel()
                    {
                         LoginName=result.Data.Key,
                         UserName=result.Data.UserName,
                         CreateTime=result.Data.CreateTime,
                         Creator=result.Data.Creator,
                         Description=result.Data.Description,
                         Editor=result.Data.Editor,
                         EditTime=result.Data.EditTime,
                         IsActive=result.Data.IsActive,
                         IsApproved=result.Data.IsApproved,
                         IsLocked=result.Data.IsLocked,
                         Email=result.Data.Email,
                         LastLoginIP=result.Data.LastLoginIP,
                         LastLoginTime=result.Data.LastLoginTime,
                         MobilePhone=result.Data.MobilePhone,
                         OfficePhone=result.Data.OfficePhone,
                         Password=result.Data.Password
                    };
                    return PartialView("_ModifyUserPartial",viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_ModifyUserPartial");
        }
        //
        // POST: /RBAC/User/SaveModify
        /// <summary>
        /// Saves the modify.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(UserViewModel model
            ,SetUserRoleViewModel userInRole
            ,SetUserResourceViewModel userOwnResource)
        {
            MethodReturnResult rst = new MethodReturnResult();
            try
            {
                using (UserServiceClient client = new UserServiceClient())
                {
                    MethodReturnResult<User> result = await client.GetAsync(model.LoginName);
                    if (result.Code > 0)
                    {
                        return Json(result);
                    }
                    //修改用户
                    result.Data.UserName = model.UserName;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    result.Data.IsActive = model.IsActive;
                    result.Data.IsApproved = model.IsApproved;
                    result.Data.IsLocked = model.IsLocked;
                    result.Data.Email = model.Email;
                    result.Data.MobilePhone = model.MobilePhone;
                    result.Data.OfficePhone = model.OfficePhone;
                    result.Data.Password = model.Password;

                    rst = await client.ModifyAsync(result.Data);
                    if (rst.Code > 0)
                    {
                        return Json(rst);
                    }

                    //设置用户角色
                    IList<UserInRole> lstUserInRole = new List<UserInRole>();
                    if (userInRole.Roles != null)
                    {
                        foreach (string roleName in userInRole.Roles)
                        {
                            lstUserInRole.Add(new UserInRole()
                            {
                                Key = new UserInRoleKey() { LoginName = model.LoginName, RoleName = roleName },
                                Editor = User.Identity.Name,
                                EditTime = DateTime.Now
                            });
                        }
                    }
                    rst = await client.SetUserRoleAsync(model.LoginName, lstUserInRole);
                    if (rst.Code > 0)
                    {
                        return Json(rst);
                    }
                    //设置用户资源
                    IList<UserOwnResource> lstUserOwnResource = new List<UserOwnResource>();
                    if (userOwnResource.Resources != null)
                    {
                        foreach (string val in userOwnResource.Resources)
                        {
                            string[] vals = val.Split('$');
                            ResourceType type = (ResourceType)Enum.Parse(typeof(ResourceType), vals[0]);
                            string code = string.Concat(vals.Skip(1));

                            lstUserOwnResource.Add(new UserOwnResource()
                            {
                                Key = new UserOwnResourceKey() { LoginName = model.LoginName, ResourceCode = code, ResourceType = type },
                                Editor = User.Identity.Name,
                                EditTime = DateTime.Now
                            });
                        }
                    }
                    rst = await client.SetUserResourceAsync(model.LoginName, lstUserOwnResource);
                    if (rst.Code > 0)
                    {
                        return Json(rst);
                    }
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(StringResource.User_SaveModify_Success, model.LoginName);
                    }
                }
            }
            catch (Exception ex)
            {
                rst.Code = 1000;
                rst.Message = ex.Message;
            }
            return Json(rst);
        }
        //
        // GET: /RBAC/User/Detail
        /// <summary>
        /// Details the specified login name.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <returns>ActionResult.</returns>
        public async Task<ActionResult> Detail(string loginName)
        {
            using (UserServiceClient client = new UserServiceClient())
            {
                MethodReturnResult<User> result = await client.GetAsync(loginName ?? string.Empty);
                if (result.Code == 0)
                {
                    UserViewModel viewModel = new UserViewModel()
                    {
                        LoginName = result.Data.Key,
                        UserName = result.Data.UserName,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime,
                        IsActive = result.Data.IsActive,
                        IsApproved = result.Data.IsApproved,
                        IsLocked = result.Data.IsLocked,
                        Email = result.Data.Email,
                        LastLoginIP = result.Data.LastLoginIP,
                        LastLoginTime = result.Data.LastLoginTime,
                        MobilePhone = result.Data.MobilePhone,
                        OfficePhone = result.Data.OfficePhone,
                        Password = result.Data.Password
                    };
                    return PartialView("_UserInfoPartial",viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_UserInfoPartial");
        }
        //
        // POST: /RBAC/User/Delete
        /// <summary>
        /// Deletes the specified login name.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string loginName)
        {
            using (UserServiceClient client = new UserServiceClient())
            {
                MethodReturnResult result = new MethodReturnResult();
                //超级管理员不能删除
                if (loginName != "admin") { 
                    result = await client.DeleteAsync(loginName ?? string.Empty);
                    if (result.Code == 0)
                    {
                        result.Message = string.Format(StringResource.User_Delete_Success, loginName);
                    }
                }
                return Json(result);
            }
        }
	}
}