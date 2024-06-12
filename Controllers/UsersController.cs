using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Orch_back_API.Entities;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
                ProfilePhotoPath = Shared.ImgagesFolderPath + "\\" + coPrzyszlo.Id.ToString() + "ProfilePhoto" + coPrzyszlo.ProfilePhoto.FileName.ToString(),
                Notifications = coPrzyszlo.Notifications,
                Messes = coPrzyszlo.Messes
            };
            userConverted.Password = passwordHasher.HashPassword(userConverted, userConverted.Password);
            using (Stream fileStream = new FileStream(userConverted.ProfilePhotoPath, FileMode.Create))
            {
                coPrzyszlo.ProfilePhoto.CopyTo(fileStream);
                fileStream.Dispose();
            }
            _context.Users.Update(userConverted);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Route("getuserphoto")]
        public ActionResult GetUserImage([FromBody] Users userWithIdOnly)
        {
            var user = _context.Users.Where(aw => aw.Id == userWithIdOnly.Id).First();
            var filePath = user.ProfilePhotoPath;

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var image = System.IO.File.OpenRead(filePath);
            string extension = Path.GetExtension(image.Name);
            return File(image, "image/" + extension.ToString().Substring(1));
        }
    }
}

