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
            AuthHelper.InitRoles().ForEach(privilege => modelBuilder.Entity<Privilege>().HasData(privilege));
        }
    }
}