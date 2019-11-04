using System;

namespace Auth.Models
{
    public class UserPrivilege : Model<Guid>
    {
        public User User { get; set; }

        public Guid UserId { get; set; }

        public Privilege Privilege { get; set; }

        public Guid PrivilegeId { get; set; }
    }
}