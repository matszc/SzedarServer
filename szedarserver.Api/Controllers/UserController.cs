using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterModel user)
        {
            await _userService.RegisterAsync(user);
            return Ok();
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginData)
        {
            var res = await _userService.LoginAsync(loginData);
            if(res == null)
            {
                return NotFound("User does not exists");
            }
            return Ok(res);
        }

        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok();
        }
    }
}