using System;
using System.Threading.Tasks;
using Auth.Configs;
using Auth.Forms;
using Auth.Models;
using Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers {
    [Route("api/[Controller]")]
    public class UserController : BaseController {
        private readonly IUserService _userService;

        public UserController(IUserService userService) { _userService = userService; }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(SingleResponse<User>), 200)]
        [ProducesResponseType(typeof(SingleResponse<>), 400)]
        public async Task<IActionResult> Login([FromBody] LoginForm loginForm) {
            var result = await _userService.Login(loginForm);
            return result == null
                ? (IActionResult) BadRequest(new SingleResponse<string> {
                    Status = false, Message = "UserName or Password is not correct", Data = null
                })
                : Ok(new SingleResponse<User> {
                    Status = true, Message = "Successfully", Data = result
                });
        }

        [MyAuthorize]
        [HttpPost("Register")]
        [HttpGet("LogOut")]
        [ProducesResponseType(typeof(SingleResponse<User>), 200)]
        [ProducesResponseType(typeof(SingleResponse<>), 400)]
        public async Task<IActionResult> Register([FromBody] User user) {
            var result = await _userService.Register(user);
            return result == null
                ? (IActionResult) BadRequest(new SingleResponse<string> {
                    Status = false, Message = "Missing information is required", Data = null
                })
                : Ok(new SingleResponse<User> {
                    Status = true, Message = "Successfully", Data = result
                });
        }

        [Authorize]
        [HttpPost("Update/{id}")]
        [ProducesResponseType(typeof(SingleResponse<User>), 200)]
        [ProducesResponseType(typeof(SingleResponse<>), 400)]
        public async Task<IActionResult> Update(Guid id, [FromBody] User user) {
            var result = await _userService.Update(user, id);
            return result == null
                ? (IActionResult) BadRequest(new SingleResponse<string> {
                    Status = false, Message = "User can not be updated", Data = null
                })
                : Ok(new SingleResponse<User> {
                    Status = true, Message = "Successfully", Data = result
                });
        }

        [Authorize]
        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(SingleResponse<User>), 200)]
        [ProducesResponseType(typeof(SingleResponse<>), 400)]
        public async Task<IActionResult> GetById(Guid id) {
            var user = await _userService.GetUser(id);
            if (user == null)
                return BadRequest(new SingleResponse<string> {
                    Status = false, Message = "User does not exist", Data = null
                });
            user.Password = null;
            return Ok(new SingleResponse<User> {
                Status = true, Message = "Successfully", Data = user
            });
        }


        [Authorize]
        [HttpDelete("Remove/{id}")]
        [ProducesResponseType(typeof(SingleResponse<User>), 200)]
        [ProducesResponseType(typeof(SingleResponse<>), 400)]
        public async Task<IActionResult> Update(Guid id) {
            var result = await _userService.Remove(id);
            return result == null
                ? (IActionResult) BadRequest(new SingleResponse<string> {
                    Status = false, Message = "User can not be removed", Data = null
                })
                : Ok(new SingleResponse<User> {
                    Status = true, Message = "Successfully", Data = result
                });
        }
    }
}