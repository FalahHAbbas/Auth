using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Models
{
    public class User : Model<Guid>
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public List<UserPrivilege> Privileges { get; set; }
        public List<UserRole> Roles { get; set; }

        [NotMapped] public string Token { get; set; }
    }
}