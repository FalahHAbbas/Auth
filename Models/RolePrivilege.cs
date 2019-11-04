using System;

namespace Auth.Models {
    public class RolePrivilege : Model<Guid> {
        public Role Role { get; set; }

        public Guid RoleId { get; set; }

        public Privilege Privilege { get; set; }

        public Guid PrivilegeId { get; set; }
    }
}