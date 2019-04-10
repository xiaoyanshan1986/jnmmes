// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.RBAC
// Author           : peter
// Created          : 07-25-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="UserAuthenticateService.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ServiceCenter.MES.DataAccess.Interface.RBAC;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Contract.RBAC;
using ServiceCenter.MES.Service.RBAC.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.Service.RBAC
{
    /// <summary>
    /// 实现用户验证契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UserAuthenticateService : IUserAuthenticateContract
    {
        /// <summary>
        /// 用户数据访问对象。
        /// </summary>
        /// <value>The user data engine.</value>
        public IUserDataEngine UserDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 资源数据访问对象。
        /// </summary>
        /// <value>The user data engine.</value>
        public IResourceDataEngine ResourceDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 用户资源数据访问对象。
        /// </summary>
        /// <value>The user own resource data engine.</value>
        public IUserOwnResourceDataEngine UserOwnResourceDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 用户角色数据访问对象。
        /// </summary>
        /// <value>The user in role data engine.</value>
        public IUserInRoleDataEngine UserInRoleDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 角色资源数据访问对象。
        /// </summary>
        /// <value>The role own resource data engine.</value>
        public IRoleOwnResourceDataEngine RoleOwnResourceDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 用户身份验证。
        /// </summary>
        /// <param name="loginName">用户登录名。</param>
        /// <param name="password">用户登录密码。</param>
        /// <returns>验证反馈信息。</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        MethodReturnResult<User> IUserAuthenticateContract.Authenticate(string loginName, string password)
        {
            MethodReturnResult<User> result = new MethodReturnResult<User>();
            try
            {
                if (this.UserDataEngine == null)
                {
                    result.Code = 1003;
                    result.Message = StringResource.UserAuthenticateService_UserDataEngineIsNull;
                    return result;
                }
                //获取用户。
                result.Data = this.UserDataEngine.Get(loginName);
                if (result.Data == null)
                {
                    result.Code = 1001;
                    result.Message = string.Format(StringResource.UserAuthenticateService_UserIsNotExists, loginName);
                    return result;
                }
                ////密码HASH。
                //SHA1CryptoServiceProvider cryptoObject = new SHA1CryptoServiceProvider();
                //byte[] bs = System.Text.Encoding.UTF8.GetBytes(password);
                //bs = cryptoObject.ComputeHash(bs);
                //System.Text.StringBuilder s = new System.Text.StringBuilder();
                //foreach (byte b in bs)
                //{
                //    s.Append(b.ToString("x2").ToUpper());
                //}
                //string hashPassword = s.ToString();
                //密码匹配。
                if (result.Data.Password != password)
                {
                    result.Code = 1002;
                    result.Message = string.Format(StringResource.UserAuthenticateService_PasswordIsError, loginName);
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.UserAuthenticateService_OtherError,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 表示资源数据比对类型。
        /// </summary>
        class ResourceEqualityComparer : IEqualityComparer<Resource>
        {
            /// <summary>
            /// Equalses the specified x.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public bool Equals(Resource x, Resource y)
            {
                return x.Key.Code == y.Key.Code && x.Key.Type == y.Key.Type;
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public int GetHashCode(Resource obj)
            {
                return obj.Key.Type.GetHashCode() ^ obj.Key.Code.GetHashCode();
            }
        }
        /// <summary>
        /// 用户资源权限验证。
        /// </summary>
        /// <param name="loginName">用户登录名。</param>
        /// <param name="resourceType">资源类型。</param>
        /// <param name="resourceData">资源数据。</param>
        /// <returns>
        /// MethodReturnResult&lt;Resource&gt;.
        /// 0:表示验证成功。如无特别含义，使用0代表验证成功,如果需要特殊代码标识，请使用小于0的自定义操作。
        /// >0:表示验证失败。
        /// </returns>
        MethodReturnResult IUserAuthenticateContract.AuthenticateResource(string loginName, ResourceType resourceType, string resourceData)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //超级用户
                if (loginName == "admin")
                {
                    return result;
                }

                result = GetResourceAuthenticate();
                if (result.Code > 0)
                {
                    return result;
                }
                int nResourceType=Convert.ToInt32(resourceType);
                IList<Resource> lstResource = this.ResourceDataEngine.Get(new PagingConfig()
                {
                    IsPaging=false,
                    Where = string.Format(" Key.Type>='{0}' AND Key.Type<'{1}' AND Data='{2}'"
                                            , nResourceType
                                            , nResourceType % 10 == 0 ? nResourceType + 10 : nResourceType+1
                                            , resourceData)
                });
                //资源不存在，表示没有加入到权限管控中不需要进行验证。
                if (lstResource.Count==0)
                {
                    return result;
                }
                bool bFound = false;

                foreach(Resource r in lstResource)
                {
                    //获取用户资源数据。
                    UserOwnResource uor = this.UserOwnResourceDataEngine.Get(new UserOwnResourceKey()
                    {
                        LoginName = loginName,
                        ResourceCode = r.Key.Code,
                        ResourceType = r.Key.Type
                    });
                    if (uor != null)
                    {
                        return result;
                    }
                    //用户没有设置该资源，进一步判断用户所属的角色是否有设置该资源。
                    IList<UserInRole> lstUIR = this.UserInRoleDataEngine.Get(new PagingConfig()
                    {
                        Where = string.Format(" Key.LoginName='{0}' ", loginName)
                    });
                    //遍历用户角色清单。
                    foreach (UserInRole uir in lstUIR)
                    {
                        //获取角色资源数据。
                        RoleOwnResource ror = this.RoleOwnResourceDataEngine.Get(new RoleOwnResourceKey()
                        {
                            ResourceCode = r.Key.Code,
                            ResourceType = r.Key.Type,
                            RoleName = uir.Key.RoleName
                        });
                        //有找到该资源。
                        if (ror != null)
                        {
                            return result;
                        }
                    }
                }
                //用户（包含用户所属角色）没有设置该资源。
                if (!bFound)
                {
                    result.Code = 1004;
                    result.Message = string.Format(StringResource.UserAuthenticateService_UserResourceIsNotExists
                                                    , loginName
                                                    , resourceType
                                                    , resourceData);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.UserAuthenticateService_OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// Gets the resource list.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="resourceType">Type of the resource.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Resource&gt;&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        MethodReturnResult<IList<Resource>> IUserAuthenticateContract.GetResourceList(string loginName, ResourceType resourceType)
        {
            MethodReturnResult<IList<Resource>> result = new MethodReturnResult<IList<Resource>>();
            try
            {
                MethodReturnResult rst = GetResourceAuthenticate();
                if (rst.Code > 0)
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.HelpLink = rst.HelpLink;
                    result.Detail = rst.Detail;
                    return result;
                }
                //超级用户
                if (loginName == "admin")
                {
                    result.Data=this.ResourceDataEngine.Get(new PagingConfig()
                    {
                        IsPaging=false,
                        Where=string.Format("Key.Type='{0}'",Convert.ToInt32(resourceType))
                    });
                    return result;
                }
                result.Data = new List<Resource>();
                //获取用户资源数据。
                IList<UserOwnResource> lstUOR = this.UserOwnResourceDataEngine.Get(new PagingConfig()
                {
                    IsPaging=false,
                    Where = string.Format(" Key.LoginName='{0}' AND Key.ResourceType='{1}'"
                                            , loginName
                                            , Convert.ToInt32(resourceType))
                });
                ResourceEqualityComparer resourceComparer = new ResourceEqualityComparer();
                //添加资源数据到集合中。
                foreach (UserOwnResource uor in lstUOR)
                {
                    Resource r=this.ResourceDataEngine.Get(new ResourceKey()
                    {
                        Code = uor.Key.ResourceCode,
                        Type = uor.Key.ResourceType
                    });
                    if (r != null && !result.Data.Contains(r, resourceComparer))
                    {
                        result.Data.Add(r);
                    }
                }
                //获取用户角色数据。
                IList<UserInRole> lstUIR = this.UserInRoleDataEngine.Get(new PagingConfig()
                {
                    IsPaging=false,
                    Where = string.Format(" Key.LoginName='{0}' ", loginName)
                });

                //遍历用户角色清单。
                foreach (UserInRole uir in lstUIR)
                {
                    //获取角色资源数据。
                    IList<RoleOwnResource> lstROR = this.RoleOwnResourceDataEngine.Get(new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(" Key.RoleName='{0}' AND Key.ResourceType='{1}'"
                                            , uir.Key.RoleName
                                            , Convert.ToInt32(resourceType))
                    });
                    //添加资源数据到集合中。
                    foreach (RoleOwnResource ror in lstROR)
                    {
                        Resource r = this.ResourceDataEngine.Get(new ResourceKey()
                        {
                            Code = ror.Key.ResourceCode,
                            Type = ror.Key.ResourceType
                        });
                        if (r != null && !result.Data.Contains(r, resourceComparer))
                        {
                            result.Data.Add(r);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.UserAuthenticateService_OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="resourceCode">The resource code.</param>
        /// <returns>MethodReturnResult&lt;Resource&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        MethodReturnResult<Resource> IUserAuthenticateContract.GetResource(string loginName, ResourceType resourceType, string resourceCode)
        {
            MethodReturnResult<Resource> result = new MethodReturnResult<Resource>();
            try
            {
                MethodReturnResult rst = GetResourceAuthenticate();
                if (rst.Code > 0)
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.HelpLink = rst.HelpLink;
                    result.Detail = rst.Detail;
                    return result;
                }
                result.Data = this.ResourceDataEngine.Get(new ResourceKey()
                {
                    Code = resourceCode,
                    Type = resourceType
                });
                //资源不存在，表示没有加入到权限管控中不需要进行验证。
                if (result.Data == null)
                {
                    result.Code = 1005;
                    result.Message = string.Format(StringResource.UserAuthenticateService_ResourceIsNotExists, resourceType, resourceCode);
                    return result;
                }
                //超级用户
                if (loginName == "admin")
                {
                    return result;
                }

                bool bFound = false;
                //获取用户资源数据。
                UserOwnResource uor = this.UserOwnResourceDataEngine.Get(new UserOwnResourceKey()
                {
                    LoginName = loginName,
                    ResourceCode = resourceCode,
                    ResourceType = resourceType
                });
                //用户没有设置该资源，进一步判断用户所属的角色是否有设置该资源。
                if (uor == null)
                {
                    IList<UserInRole> lstUIR = this.UserInRoleDataEngine.Get(new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(" Key.LoginName='{0}' ", loginName)
                    });
                    //遍历用户角色清单。
                    foreach (UserInRole uir in lstUIR)
                    {
                        //获取角色资源数据。
                        RoleOwnResource ror = this.RoleOwnResourceDataEngine.Get(new RoleOwnResourceKey()
                        {
                           ResourceCode=resourceCode,
                           ResourceType=resourceType,
                           RoleName=uir.Key.RoleName
                        });
                        //有找到该资源。
                        if (ror != null)
                        {
                            bFound = true;
                            break;
                        }
                    }
                }
                else
                {
                    bFound = true;
                }

                //用户（包含用户所属角色）有设置该资源。
                if (bFound)
                {
                    return result;
                }
                else
                {
                    result.Code = 1004;
                    result.Message = string.Format(StringResource.UserAuthenticateService_UserResourceIsNotExists,loginName,resourceType,resourceCode);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.UserAuthenticateService_OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
       
        MethodReturnResult GetResourceAuthenticate()
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ResourceDataEngine == null)
            {
                result.Code = 1006;
                result.Message = StringResource.UserAuthenticateService_ResourceDataEngineIsNull;
                return result;
            }

            if (this.UserOwnResourceDataEngine == null)
            {
                result.Code = 1009;
                result.Message = StringResource.UserAuthenticateService_UserOwnResourceDataEngineIsNull;
                return result;
            }

            if (this.UserInRoleDataEngine == null)
            {
                result.Code = 1008;
                result.Message = StringResource.UserAuthenticateService_UserInRoleDataEngineIsNull;
                return result;
            }

            if (this.RoleOwnResourceDataEngine == null)
            {
                result.Code = 1007;
                result.Message = StringResource.UserAuthenticateService_RoleOwnResourceDataEngineIsNull;
                return result;
            }
            return result;
        }
    }
}
