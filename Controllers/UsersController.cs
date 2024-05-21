using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Orch_back_API.Entities;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Orch_back_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyJDBContext _context;
        public UsersController(MyJDBContext context)
        {
            this._context = context;
        }

        [HttpPost]
        [Route("updatedata")]
        public ActionResult UpdateUserData([FromBody] UsersComing user)
        {
            PasswordHasher<Users> passwordHasher = new();
            var coPrzyszlo = user;
            Users userConverted = new Users
            {
                Id = coPrzyszlo.Id,
                Username = coPrzyszlo.Username,
                Password = coPrzyszlo.Password,
                Email = coPrzyszlo.Email,
                Role = coPrzyszlo.Role,
                Region = coPrzyszlo.Region,
                Age = coPrzyszlo.Age,
                City = coPrzyszlo.City,
                ProfilePhoto = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(coPrzyszlo.ProfilePhoto)),
                Notifications = coPrzyszlo.Notifications,
                Messes = coPrzyszlo.Messes
            };
            userConverted.Password = passwordHasher.HashPassword(userConverted, userConverted.Password);
            _context.Users.Update(userConverted);
            _context.SaveChanges();
            return Ok();
        }

    }
}

