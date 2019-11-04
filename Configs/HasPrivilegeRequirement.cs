using System.Linq;
using System.Security.Claims;
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
                    Template =
                        resource.HttpContext.Request.Path.Value
                            .Replace("/api/" + resource.RouteData.Values["controller"] + "/", "")
                            .Split("/")[0],
                    MethodType = resource.HttpContext.Request.Method,
                    CalssName = resource.RouteData.Values["controller"] + "Controller",
                    MethodName = resource.RouteData.Values["action"].ToString()
                };


                var userPrivileges = context.User.FindFirst(c => c.Type == "Privileges").Value
                    .Split(";").ToList();
                if (userPrivileges.Contains(privilege.Name)) {
                    context.Succeed(requirement);
                    return;
                }

                var userRoles = context.User.FindFirstValue("Roles").Split(";").ToList();
                var role = await _repositoryWrapper.RoleRepository
                    .GetRaw(p => userRoles.Contains(p.Name))
                    .Where(p => p.Privileges.Any(pr => pr.Privilege.Name == privilege.Name))
                    .FirstOrDefaultAsync();

                if (role != null) {
                    context.Succeed(requirement);
                }
            }
        }
    }
}