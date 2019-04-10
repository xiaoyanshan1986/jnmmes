// ***********************************************************************
// Assembly         : ServiceCenter.Client.Mvc
// Author           : peter
// Created          : 08-06-2014
//
// Last Modified By : peter
// Last Modified On : 08-06-2014
// ***********************************************************************
// <copyright file="RoleController.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
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
    /// Class RoleController.
    /// </summary>
    [Authorize]
    public class RoleController : Controller
    {
        //
        // GET: /RBAC/Role/
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        public async Task<ActionResult> Index()
        {
            using (RoleServiceClient client = new RoleServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig();
                    MethodReturnResult<IList<Role>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.RoleList = result.Data;
                    }
                });
            }
            return View();
        }
        //
        //POST: /RBAC/Role/Query
        /// <summary>
        /// Queries the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RoleQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RoleServiceClient client = new RoleServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.RoleName))
                            {
                                where.AppendFormat("Key LIKE '{0}%'", model.RoleName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Role>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.RoleList = result.Data;
                        }
                    });
                }
            }
            return PartialView("_RoleListPartial");
        }
        //
        //POST: /RBAC/Role/PagingQuery
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
                int pageNo = currentPageNo ?? 0;
                int pageSize = currentPageSize ?? 20;
                if (Request["PageNo"] != null)
                {
                    pageNo = Convert.ToInt32(Request["PageNo"]);
                }
                if (Request["PageSize"] != null)
                {
                    pageSize = Convert.ToInt32(Request["PageSize"]);
                }

                using (RoleServiceClient client = new RoleServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = pageNo,
                            PageSize = pageSize,
                            Where = where ?? string.Empty,
                            OrderBy = orderBy ?? string.Empty
                        };
                        MethodReturnResult<IList<Role>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.RoleList = result.Data;
                        }
                    });
                }
            }
            return PartialView("_RoleListPartial");
        }
        //
        // POST: /RBAC/Role/SaveRole
        /// <summary>
        /// Saves the role.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveRole(RoleViewModel model)
        {
            using (RoleServiceClient client = new RoleServiceClient())
            {
                Role u = new Role()
                {
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name,
                    Key = model.RoleName
                };
                MethodReturnResult rst = await client.AddAsync(u);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(StringResource.Role_SaveRole_Success, u.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /RBAC/Role/Modify
        /// <summary>
        /// Modifies the specified role name.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        public async Task<ActionResult> Modify(string roleName)
        {
            using (RoleServiceClient client = new RoleServiceClient())
            {
                MethodReturnResult<Role> result = await client.GetAsync(roleName ?? string.Empty);
                if (result.Code == 0)
                {
                    RoleViewModel viewModel = new RoleViewModel()
                    {
                        RoleName = result.Data.Key,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    return PartialView("_ModifyRolePartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_ModifyRolePartial");
        }
        //
        // POST: /RBAC/Role/SaveModify
        /// <summary>
        /// Saves the modify.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RoleViewModel model
                                        ,SetRoleUserViewModel roleUser
                                        ,SetRoleResourceViewModel roleResource)
        {
            MethodReturnResult rst = new MethodReturnResult();
            try {
                using (RoleServiceClient client = new RoleServiceClient())
                {
                    MethodReturnResult<Role> result = await client.GetAsync(model.RoleName);
                    if (result.Code > 0)
                    {
                        return Json(result);
                    }
                    //修改角色基本信息。
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    rst = await client.ModifyAsync(result.Data);
                    if (rst.Code > 0)
                    {
                        return Json(rst);
                    }
                    //设置角色用户
                    IList<UserInRole> lstUserInRole = new List<UserInRole>();
                    if (roleUser.Users != null)
                    {
                        foreach (string loginName in roleUser.Users)
                        {
                            lstUserInRole.Add(new UserInRole()
                            {
                                Key = new UserInRoleKey() { LoginName = loginName, RoleName = model.RoleName },
                                Editor = User.Identity.Name,
                                EditTime = DateTime.Now
                            });
                        }
                    }
                    rst = await client.SetRoleUserAsync(model.RoleName, lstUserInRole);
                    if (rst.Code > 0)
                    {
                        return Json(rst);
                    }
                    //设置角色资源
                    IList<RoleOwnResource> lstRoleOwnResource = new List<RoleOwnResource>();
                    if (roleResource.Resources != null)
                    {
                        foreach (string val in roleResource.Resources)
                        {
                            string[] vals = val.Split('$');
                            ResourceType type = (ResourceType)Enum.Parse(typeof(ResourceType), vals[0]);
                            string code = string.Concat(vals.Skip(1));

                            lstRoleOwnResource.Add(new RoleOwnResource()
                            {
                                Key = new RoleOwnResourceKey() { RoleName =model.RoleName , ResourceCode = code, ResourceType = type },
                                Editor = User.Identity.Name,
                                EditTime = DateTime.Now
                            });
                        }
                    }
                    rst = await client.SetRoleResourceAsync(model.RoleName, lstRoleOwnResource);
                    if (rst.Code > 0)
                    {
                        return Json(rst);
                    }

                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(StringResource.Role_SaveModify_Success, model.RoleName);
                    }
                    return Json(rst);
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
        // GET: /RBAC/Role/Detail
        /// <summary>
        /// Details the specified role name.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        public async Task<ActionResult> Detail(string roleName)
        {
            using (RoleServiceClient client = new RoleServiceClient())
            {
                MethodReturnResult<Role> result = await client.GetAsync(roleName ?? string.Empty);
                if (result.Code == 0)
                {
                    RoleViewModel viewModel = new RoleViewModel()
                    {
                        RoleName = result.Data.Key,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    return PartialView("_RoleInfoPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_RoleInfoPartial");
        }
        //
        // POST: /RBAC/Role/Delete
        /// <summary>
        /// Deletes the specified role name.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string roleName)
        {
            using (RoleServiceClient client = new RoleServiceClient())
            {
                MethodReturnResult result = await client.DeleteAsync(roleName ?? string.Empty);
                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.Role_Delete_Success, roleName);
                }
                return Json(result);
            }
        }
	}
}