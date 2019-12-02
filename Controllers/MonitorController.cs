using System.Linq;
using Auth.Configs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Auth.Controllers {
    [Route("api/My")]
    [Route("api/[Controller]")]
    public class MonitorController : Controller {
        private readonly IActionDescriptorCollectionProvider _provider;

        public MonitorController(IActionDescriptorCollectionProvider provider) {
            _provider = provider;
        }

        [MyAuthorize]
        [HttpGet("routes/{ass}")]
        public IActionResult GetRoutes(string ass) {
            var routes = _provider.ActionDescriptors.Items.Select(x => new {
                Action = x.RouteValues["Action"],
                Controller = x.RouteValues["Controller"],
                Name = x.AttributeRouteInfo.Name,
                Template = x.AttributeRouteInfo.Template
            }).ToList();
            return Ok(routes);
        }

        [MyAuthorize]
        [HttpGet("routes")]
        public IActionResult GetRoutes2() {
            var routes = _provider.ActionDescriptors.Items.Select(x => new {
                Action = x.RouteValues["Action"],
                Controller = x.RouteValues["Controller"],
                Name = x.AttributeRouteInfo.Name,
                Template = x.AttributeRouteInfo.Template
            }).ToList();
            return Ok(routes);
        }
    }
}