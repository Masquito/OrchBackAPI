using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
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
    public class NotificationObjectFromApi
    {
        public string VisitorId { get; set; }
        public string HostId { get; set;}
    }
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
            Users userConverted = new Users();
            if (coPrzyszlo.ProfilePhoto == null)
            {
                userConverted = new Users
                {
                    Id = coPrzyszlo.Id,
                    Username = coPrzyszlo.Username,
                    Password = coPrzyszlo.Password,
                    Email = coPrzyszlo.Email,
                    Role = coPrzyszlo.Role,
                    Region = coPrzyszlo.Region,
                    Age = coPrzyszlo.Age,
                    City = coPrzyszlo.City,
                    ProfilePhotoPath = _context.Users.Where(eb => eb.Id == user.Id).FirstOrDefault().ProfilePhotoPath,
                    Notifications = coPrzyszlo.Notifications,
                    Messes = coPrzyszlo.Messes
                };
            }
            else
            {
                userConverted = new Users
                {
                    Id = coPrzyszlo.Id,
                    Username = coPrzyszlo.Username,
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
            }
            userConverted.Password = passwordHasher.HashPassword(userConverted, userConverted.Password);
            if(coPrzyszlo.ProfilePhoto != null)
            {
                using (Stream fileStream = new FileStream(userConverted.ProfilePhotoPath, FileMode.Create))
                {
                    coPrzyszlo.ProfilePhoto.CopyTo(fileStream);  
                    fileStream.Dispose();
                }
            }
            _context.ChangeTracker.Clear();
            _context.Users.Update(userConverted);
            _context.SaveChanges();
            return Ok(userConverted);
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

        [HttpPost]
        [Route("getuserbyid")]
        public ActionResult GetUserById([FromBody] UsersComing user)
        {
            var id = user.Id;
            var userToReturn = _context.Users.Where(eb => eb.Id.Equals(id)).FirstOrDefault();
            if(userToReturn != null)
            {
                return Ok(new { userToReturn });
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("addnotificationwhenprofilevisited")]
        public ActionResult AddNotificationWhenProfileVisited([FromBody] NotificationObjectFromApi fromApi)
        {
            Notifications notificationToBeAddedToDatabase = new Notifications();
            Users authorOf = _context.Users.Where(eb => eb.Id.ToString() == fromApi.VisitorId).FirstOrDefault();
            notificationToBeAddedToDatabase.Id = Guid.NewGuid();
            notificationToBeAddedToDatabase.Author = authorOf;
            notificationToBeAddedToDatabase.AuthorId = authorOf.Id;
            notificationToBeAddedToDatabase.Content = "User " + authorOf.Username + " has visited your profile.";
            notificationToBeAddedToDatabase.SendDate = DateTime.Now;
            notificationToBeAddedToDatabase.DeliveryId = new Guid(fromApi.HostId);
            _context.Notifications.Add(notificationToBeAddedToDatabase);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Route("getuserssearchedforwithfilters")]
        public ActionResult GetUsersSearchedForWithFilters([FromForm] UsersComing user)
        {
            var ta = user;

            if (!IsRegion(ta) && !IsAge(ta) && !IsCity(ta))
            {
                return NotFound();
            }

            var query = _context.Users.AsQueryable();

            if (IsRegion(ta))
            {
                query = query.Where(eb => eb.Region == ta.Region);
            }

            if (IsAge(ta))
            {
                query = query.Where(eb => eb.Age == ta.Age);
            }

            if (IsCity(ta))
            {
                query = query.Where(eb => eb.City == ta.City);
            }

            var users = query.ToList();
            users.Remove(_context.Users.Where(eb => eb.Id == ta.Id).FirstOrDefault()!);
            return Ok(new {users});
        }

        [HttpPost]
        [Route("checkifusernameexists")]
        public ActionResult CheckIfUsernameExists([FromBody] UsersComing user)
        {
            foreach(var usera in _context.Users)
            {
                if(usera.Username == user.Username)
                {
                    return Ok(true);
                }
            }
            return Ok(false);
        }

        [HttpPost]
        [Route("checkifemailexists")]
        public ActionResult CheckIfEmailExists([FromBody] UsersComing user)
        {
            foreach (var usera in _context.Users)
            {
                if (usera.Email == user.Email)
                {
                    return Ok(true);
                }
            }
            return Ok(false);
        }

        private bool IsRegion(UsersComing user)
        {
            if(user.Region == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool IsAge(UsersComing user)
        {
            if (user.Age == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool IsCity(UsersComing user)
        {
            if (user.City == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

