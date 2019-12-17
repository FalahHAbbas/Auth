using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Auth.Models;
using Auth.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Auth.Configs {
    public class HasPrivilegeRequirement : IAuthorizationRequirement {
        public static void Of(AuthorizationPolicyBuilder policy) =>
            policy.Requirements.Add(new HasPrivilegeRequirement());
    }

    public class HasPrivilegeHandler : AuthorizationHandler<HasPrivilegeRequirement> {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public HasPrivilegeHandler(IRepositoryWrapper repositoryWrapper) {
            _repositoryWrapper = repositoryWrapper;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            HasPrivilegeRequirement requirement) {
            if (!context.User.HasClaim(c => c.Type == "Privileges" || c.Type == "Roles")) {
                return;
            }

            if (context.Resource is AuthorizationFilterContext resource) {
                var privilege = new Privilege {
                    Template = resource.HttpContext.Request.Path.Value
                    // .Replace("/api/" + resource.RouteData.Values["controller"] + "/", "")
                    // .Split("/")[0]
                    ,
                    MethodType = resource.HttpContext.Request.Method,
                    ClassName = resource.RouteData.Values["controller"] + "Controller",
                    MethodName = resource.RouteData.Values["action"].ToString()
                };


                var userPrivileges = context.User.FindFirst(c => c.Type == "Privileges").Value
                    .Split(";").Select(p => Regex.Replace(p, @"{.*}", "")).ToList();
                if (userPrivileges.Any(up => privilege.Template.StartsWith(up))) {
                    context.Succeed(requirement);
                    return;
                }

                var userRoles = context.User.FindFirstValue("Roles").Split(";").ToList();
                if (await _repositoryWrapper.RoleRepository.GetRaw(p => userRoles.Contains(p.Name))
                        .Where(p => p.Privileges.Any(pr => pr.Privilege.Name == privilege.Name))
                        .FirstOrDefaultAsync() != null) {
                    context.Succeed(requirement);
                }
            }
        }
    }
}