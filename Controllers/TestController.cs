using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.Configs;
using Auth.Models;
using Auth.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auth.Controllers
{
    [Route("api/[Controller]")]
    public class TestController : BaseController
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public TestController(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        [MyAuthorize]
        [HttpPost("Temp")]
        public async Task<ActionResult<User>> Test()
        {
            var users = await _repositoryWrapper.UserRepository.GetRaw().Include(c => c.Privileges)
                .ThenInclude(a => a.Privilege).ToListAsync();
            return Ok(users);
        }

        [HttpPost("InitUser")]
        public async Task<ActionResult<User>> InitUser()
        {
            var user = await _repositoryWrapper.UserRepository.Add(new User
            {
                Name = "Falah",
                Password = "Falah",
            });
            var all = await _repositoryWrapper.PrivilegeRepository.GetAll();
            var privileges = new List<UserPrivilege>();
            all.Data.ForEach(d => privileges.Add(new UserPrivilege
            {
                UserId = user.Id,
                Privilege = d
            }));
            user.Privileges = privileges;
            await _repositoryWrapper.UserRepository.Update(user, user.Id);
            return Ok(user);
        }

        [MyAuthorize]
        [HttpGet("GetUsers")]
        public async Task<ActionResult<User>> GetAll()
        {
            var users = await _repositoryWrapper.UserRepository.GetRaw().Include(c => c.Privileges)
                .ThenInclude(a => a.Privilege).ToListAsync();
            return Ok(users);
        }

        [MyAuthorize]
        [HttpPost("Temp1")]
        public async Task<ActionResult<User>> Test1()
        {
            var roles = await _repositoryWrapper.RoleRepository.GetRaw().Include(c => c.Privileges)
                .ThenInclude(a => a.Privilege).ToListAsync();
            return Ok(roles);
        }
    }
}