using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Auth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Auth.Configs {

    public class AuthHelper{

        public static  List<Privilege> InitRoles() {
            var actions = Assembly.GetExecutingAssembly().GetTypes().SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(MyAuthorizeAttribute), false).Length > 0)
                .ToArray().Select(methodInfo => new {
                    CalssName = methodInfo.DeclaringType?.Name,
                    MethodName = methodInfo.Name,
                    Controller = ((RouteAttribute) methodInfo.DeclaringType.GetCustomAttribute(typeof(RouteAttribute))).Template,
                    HttpMethods = methodInfo.GetCustomAttributes(typeof(HttpMethodAttribute))
                        .ToList().Select(attribute => new {
                            ((HttpMethodAttribute) attribute).Template,
                            Type = ((HttpMethodAttribute) attribute).HttpMethods.First().ToString()
                        }).ToList()
                }).ToList();
            var privileges = new List<Privilege>();
            actions.ForEach(action => {
                privileges.AddRange(action.HttpMethods.Select(httpMethod => new Privilege {
                    Id = Guid.NewGuid(),
                    CalssName = action.CalssName,
                    MethodName = action.MethodName,
                    Template = httpMethod.Template,
                    MethodType = httpMethod.Type,
                }));
            });

         return   privileges/*.ForEach(async privilege =>
                await _repositoryWrapper.PrivilegeRepository.Add(privilege))*/;
        }
    }
}