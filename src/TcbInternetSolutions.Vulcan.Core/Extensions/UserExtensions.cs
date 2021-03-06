﻿using EPiServer.Security;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace TcbInternetSolutions.Vulcan.Core.Extensions
{
    /// <summary>
    /// User extensions
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// IVirtualRoleRepository dependency
        /// </summary>
        public static Injected<IVirtualRoleRepository> VirtualRoleRepository { get; set; }

        /// <summary>
        /// IUserImpersonation dependency
        /// </summary>
        public static Injected<IUserImpersonation> UserImpersonation { get; set; }

        /// <summary>
        /// IPrincipalAccessor
        /// </summary>
        public static Injected<IPrincipalAccessor> PrincipalAccessor { get; set; }

        /// <summary>
        /// Gets current principal
        /// </summary>
        /// <returns></returns>
        public static IPrincipal GetUser() => PrincipalAccessor.Service.Principal;

        /// <summary>
        /// Gets principal from username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static IPrincipal GetUser(string username) =>  UserImpersonation.Service.CreatePrincipal(username);
            
        /// <summary>
        /// Gets roles for given principle
        /// </summary>
        /// <param name="principle"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetRoles(this IPrincipal principle)
        {
            if (principle == null)
                throw new ArgumentNullException(nameof(principle));

            // todo: is PrincipalInfo needed for roleslist
            //var userPrinciple = new PrincipalInfo(principle);
            var list = new List<string>();

            foreach (var name in VirtualRoleRepository.Service.GetAllRoles())
            {
                if (VirtualRoleRepository.Service.TryGetRole(name, out var virtualRoleProvider) && virtualRoleProvider.IsInVirtualRole(principle, null))
                {
                    list.Add(name);
                }
            }
            //todo: figure out netstandard equivalent
#if NET461
            if (System.Web.Security.Roles.Enabled)
            {
                list.AddRange(System.Web.Security.Roles.GetRolesForUser(principle.Identity.Name));
            }
#endif
            return list.Distinct();
        }
    }
}