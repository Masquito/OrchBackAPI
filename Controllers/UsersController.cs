using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Orch_back_API.Entities;
using System.Drawing;
using System.Drawing.Imaging;
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
        public ActionResult UpdateUserData([FromForm] UsersComing user)
        {
            PasswordHasher<Users> passwordHasher = new();
            var coPrzyszlo = user;
            Users userConverted = new Users
            {
                Id = coPrzyszlo.Id,
                Username = coPrzyszlo.Username,    //W TEJ WERSJI DZIAŁA PRZESYLANIE PLIKU
                Password = coPrzyszlo.Password,
                Email = coPrzyszlo.Email,
                Role = coPrzyszlo.Role,
                Region = coPrzyszlo.Region,
                Age = coPrzyszlo.Age,
                City = coPrzyszlo.City,
                ProfilePhotoPath = Shared.ImgagesFolderPath + "\\" + coPrzyszlo.Id.ToString() + "ProfilePhoto.jpg",
                Notifications = coPrzyszlo.Notifications,
                Messes = coPrzyszlo.Messes
            };
            userConverted.Password = passwordHasher.HashPassword(userConverted, userConverted.Password);
            var ProfilePhoto = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(coPrzyszlo.ProfilePhoto));
            using (var ms = new System.IO.MemoryStream(ProfilePhoto))
            {
                using (var img = Image.FromStream(ms))
                {
                    img.Save(userConverted.ProfilePhotoPath, ImageFormat.Jpeg);
                }
            }
            _context.Users.Update(userConverted);
            _context.SaveChanges();
            return Ok();
        }

    }
}

