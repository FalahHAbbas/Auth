using Microsoft.AspNetCore.Authorization;

namespace Auth.Configs {
    public class MyAuthorizeAttribute : AuthorizeAttribute, IAuthorizeData {
        public MyAuthorizeAttribute() : base("Main") { }
    }
}