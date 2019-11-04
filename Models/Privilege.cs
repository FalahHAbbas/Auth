using System;
using System.Collections.Generic;

namespace Auth.Models
{
    public class Privilege : Model<Guid>
    {
        public string Name => CalssName + "@" + MethodName + "@" + Template + "@" + MethodType;
        public string UserDefinedName { get; set; } = "";
        public List<UserPrivilege> Users { get; set; }

        public string CalssName { get; set; }
        public string MethodName { get; set; }
        public string Template { get; set; }
        public string MethodType { get; set; }
    }
}