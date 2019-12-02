using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Auth.Forms;
using Auth.Models;
using Auth.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Services
{
    public interface IUserService
    {
        Task<User> Register(User user);
        Task<User> Login(LoginForm loginForm);
        Task<User> Update(User user, Guid id);
        Task<User> Remove(Guid id);
        Task<User> GetUser(Guid id);
        Task<User> GetUser(string name);
        User GenerateToken(User user);
    }

    public class UserService : IUserService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public UserService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<User> Register(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _repositoryWrapper.UserRepository.Add(user);
            return GenerateToken(user);
        }

        public async Task<User> Login(LoginForm loginForm)
        {
            var user = new User {Username = loginForm.Name, Password = loginForm.Password};
            var result =
                await _repositoryWrapper.UserRepository.GetRaw(u => u.Username == user.Username)
                    .Include(u => u.Privileges)
                    .ThenInclude(p => p.Privilege)
                    .Include(u => u.Roles)
                    .ThenInclude(p => p.Role)
                    .FirstOrDefaultAsync();
            if (result == null) return null;
            var valid = Verify(user, result);
            return valid ? GenerateToken(result) : null;
        }

        private static bool Verify(User user, User result) =>
            BCrypt.Net.BCrypt.Verify(user.Password, result.Password);


        public User GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim("Username", user.Username),
                new Claim("Name", user.Name),
                new Claim("id", user.Id.ToString()),
                new Claim("Privileges", string.Join(";", user.Privileges.Select(p => p.Privilege.Template))),
                new Claim("Roles", string.Join(";", user.Roles.Select(p => p.Role.Name))),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddDays(30),
                notBefore: DateTime.UtcNow, audience: "Audience", issuer: "Issuer",
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("Hlkjds0-324mf34pojf-34r34fwlknef0943")),
                    SecurityAlgorithms.HmacSha256));
            user.Token = new JwtSecurityTokenHandler().WriteToken(token);
            return user;
        }

        public async Task<User> Update(User user, Guid id) =>
            await _repositoryWrapper.UserRepository.Update(user, id);


        public async Task<User> Remove(Guid id) =>
            await _repositoryWrapper.UserRepository.Remove(id);

        public async Task<User> GetUser(Guid id) =>
            await _repositoryWrapper.UserRepository.GetById(id);

        public async Task<User> GetUser(string name) =>
            await _repositoryWrapper.UserRepository.Get(x => x.Username == name);
    }
}