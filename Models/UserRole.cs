using System;

namespace Auth.Models
{
    public class UserRole : Model<Guid>
    {
        public User User { get; set; }

        public Guid UserId { get; set; }

        public Role Role { get; set; }

        public Guid RoleId { get; set; }
    }
}