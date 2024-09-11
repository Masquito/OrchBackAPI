using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Orch_back_API.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;

namespace Orch_back_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly MyJDBContext _dbcontext;
        private readonly IConfiguration _configuration;
        public RegisterController(MyJDBContext myJDBContext, IConfiguration configuration)
        {
            this._dbcontext = myJDBContext;
            this._configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Users userCame)
        {
            PasswordHasher<Users> passwordHasher = new();
            Users userToAdd = userCame;

            var users = await _dbcontext.Users.ToListAsync();
            if (users.Contains(userToAdd))
            {
                return Forbid("User already exists");
            }

            userToAdd.Role = "NFUA";
            userToAdd.Id = new Guid();
            userToAdd.Password = passwordHasher.HashPassword(userToAdd, userToAdd.Password);
            userToAdd.ProfilePhotoPath = Shared.ImgagesFolderPath + "\\defaultProfilePicture.png";
            await _dbcontext.Users.AddAsync(userToAdd);
            await _dbcontext.SaveChangesAsync();
            bool finished = true;
            return Ok(new { finished });
        }

    }
}
