using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using Orch_back_API.Entities;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Orch_back_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly MyJDBContext _context;
        public LoginController(IConfiguration config, MyJDBContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Users users)
        {
            PasswordHasher<Users> passwordHasher = new();
            var Shared = new Shared(this._context, this._config);
            var user = await Shared.Authenticate(users);
            if (user != null)
            {
                var token = Shared.GenerateToken(user);
                return Ok(new { token, user });
            }

            return NotFound("user not found");
        }
    }
}
