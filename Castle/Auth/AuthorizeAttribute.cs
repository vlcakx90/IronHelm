﻿using Castle.Models.User;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Castle.Interfaces;
using Castle.Services;

namespace Castle.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    //public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    //{
    //    private readonly IList<Role> _roles;

    //    public AuthorizeAttribute(params Role[] roles)
    //    {
    //        _roles = roles ?? new Role[] { };
    //    }
    //    public void OnAuthorization(AuthorizationFilterContext context)
    //    {
    //        // skip authorization if action is decorated with [AllowAnonymous] attribute
    //        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
    //        if (allowAnonymous)
    //            return;

    //        // authorization
    //        var user = (User?)context.HttpContext.Items["User"];
    //        if (user == null || (_roles.Any() && !_roles.Contains(user.Role))) // Check Role
    //        {
    //            // not logged in or role not authorized
    //            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
    //        }
    //    }
    //}

    public class AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IList<Role> _roles;

        public AuthorizeAttribute(params Role[] roles)
        {
            _roles = roles ?? new Role[] { };
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;


            // authorization
            var user = (User?)context.HttpContext.Items["User"]; // see JwtMiddleware

            if (user == null || (_roles.Any() && !_roles.Contains(user.Role))) // Check Role
            {
                // not logged in or role not authorized
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
