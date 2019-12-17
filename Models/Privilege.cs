using System;
using System.Collections.Generic;

namespace Auth.Models
{
    public class Privilege : Model<Guid>
    {
        public string Name => ClassName + "@" + MethodName + "@" + Template + "@" + MethodType;
        public string UserDefinedName { get; set; } = "";
        public List<UserPrivilege> Users { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string Template { get; set; }
        public string MethodType { get; set; }
    }
}