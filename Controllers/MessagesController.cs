using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orch_back_API.Entities;

namespace Orch_back_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly MyJDBContext _context;
        public MessagesController(IConfiguration config, MyJDBContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost]
        [Route("GetAllUserMessagessWithFilter")]
        public ActionResult GetAllUserMessagessWithFilter([FromForm] UsersComing userWithIdOnlyAndOnUsernameIsFilter)
        {
            var users = new List<Users>();
            var messages = _context.Messages.Where(eb => eb.DeliveryId == userWithIdOnlyAndOnUsernameIsFilter.Id).ToList().OrderByDescending(eb => eb.SendDate);
            List<Guid> authorIds = [];
            foreach (var message in messages)
            {
                authorIds.Add((Guid)message.AuthorId);
            }

            if(userWithIdOnlyAndOnUsernameIsFilter.Username == null)
            {
                users = [.. _context.Users.Where(eb => authorIds.Contains(eb.Id))];
            }
            else
            {
                users = [.. _context.Users.Where(eb => authorIds.Contains(eb.Id)).Where(x => x.Username.Contains(userWithIdOnlyAndOnUsernameIsFilter.Username))];
            }

            if (messages.Count() == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(new { messages, users });
            }
        }

        [HttpPost]
        [Route("GetAllUserMessagess")]
        public ActionResult GetAllUserMessagess([FromBody] UsersComing userWithIdOnly)
        {
            var users = new List<Users>();
            var messages = _context.Messages.Where(eb => eb.DeliveryId == userWithIdOnly.Id).ToList().OrderByDescending(eb => eb.SendDate);
            List<Guid> authorIds = [];    
            foreach (var message in messages)
            {
                authorIds.Add((Guid)message.AuthorId);
            }

            users = [.. _context.Users.Where(eb => authorIds.Contains(eb.Id))];

            if(messages.Count() == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(new { messages, users } );
            }
        }

        [HttpPost]
        [Route("reciveMessageSendByUserToUser")]
        public ActionResult ReciveMessageSendByUsertoUser([FromForm] Messages message)
        {
            //Notification to user
            Notifications notification = new Notifications();
            notification.Id = Guid.NewGuid();
            notification.Author = _context.Users.Where(eb => eb.Id == message.AuthorId).FirstOrDefault();
            notification.Content = "User " + notification.Author.Username + " sent you a message";
            notification.SendDate = DateTime.Now;
            notification.DeliveryId = message.DeliveryId;
            notification.AuthorId = message.AuthorId;

            //Message
            Messages messageToWrite = new Messages();
            messageToWrite.Id = Guid.NewGuid();
            messageToWrite.Content = message.Content;
            messageToWrite.SendDate = DateTime.UtcNow;
            messageToWrite.DeliveryId = message.DeliveryId;
            messageToWrite.AuthorId = message.AuthorId;
            messageToWrite.Author = _context.Users.Where(eb => eb.Id == message.AuthorId).FirstOrDefault();

            //Database operations
            _context.Messages.Add(messageToWrite);
            _context.Notifications.Add(notification);
            _context.SaveChanges();

            return Ok("wiadomosc dodana poprawnie");
        }
    }
}
