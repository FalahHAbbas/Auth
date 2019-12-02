using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Auth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Auth.Configs {
    public class AuthHelper {
        public static List<Privilege> InitRoles() {
            var actions = Assembly.GetExecutingAssembly().GetTypes().SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(MyAuthorizeAttribute), false).Length > 0)
                .ToArray().Select(methodInfo => new {
                    CalssName = methodInfo.DeclaringType?.Name,
                    MethodName = methodInfo.Name,
                    HttpMethods =
                        methodInfo.GetCustomAttributes(typeof(HttpMethodAttribute)).ToList().Select(
                            attribute => new {
                                ((HttpMethodAttribute) attribute).Template,
                                Type = ((HttpMethodAttribute) attribute).HttpMethods.First()
                                    .ToString()
                            }).ToList(),
                    ContrllerPath = methodInfo.DeclaringType
                        ?.GetCustomAttributes(typeof(RouteAttribute)).ToList()
                        .Select(attribute => ((RouteAttribute) attribute).Template).ToList()
                }).ToList();
            var privileges = new List<Privilege>();
            actions.ForEach(action => {
                action.ContrllerPath.ForEach(path => {
                    privileges.AddRange(action.HttpMethods.Select(httpMethod => new Privilege {
                        Id = Guid.NewGuid(),
                        CalssName = action.CalssName,
                        MethodName = action.MethodName,
                        Template =
                            "/"+path.Replace("[Controller]",
                                action.CalssName.Replace("Controller", "")) + "/" +
                            httpMethod.Template,
                        MethodType = httpMethod.Type
                    }));
                });
            });

            return privileges;
        }
    }
}