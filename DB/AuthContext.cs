using System;
using System.Linq;
using Auth.Configs;
using Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth.DB {
    public class AuthContext : DbContext {
        public AuthContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Privilege> Privileges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            var privileges = AuthHelper.InitRoles();
            modelBuilder.Entity<Privilege>().HasData(privileges);

            var viewerRole = new Role {Id = Guid.NewGuid(), Name = "Viewer"};
            modelBuilder.Entity<Role>().HasData(viewerRole);

            var viewerRolePrivileges = privileges.Where(p => p.MethodType.Equals("GET"))
                .Select(p => new RolePrivilege {
                    Id = Guid.NewGuid(), PrivilegeId = p.Id, RoleId = viewerRole.Id
                }).ToList();
            modelBuilder.Entity<RolePrivilege>().HasData(viewerRolePrivileges);

            var adderRole = new Role {Id = Guid.NewGuid(), Name = "Adder"};
            modelBuilder.Entity<Role>().HasData(adderRole);
            var adderRolePrivileges = privileges.Where(p => p.MethodType.Equals("POST")).Select(p =>
                    new RolePrivilege {
                        Id = Guid.NewGuid(), PrivilegeId = p.Id, RoleId = adderRole.Id
                    })
                .ToList();
            modelBuilder.Entity<RolePrivilege>().HasData(adderRolePrivileges);
        }
    }
}