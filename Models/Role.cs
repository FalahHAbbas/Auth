using System;
using System.Collections.Generic;

namespace Auth.Models
{
    public class Role : Model<Guid>
    {
        public string Name { get; set; }
        public List<RolePrivilege> Privileges { get; set; }
        public List<UserRole> Users { get; set; }
    }
}