using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetAllUserMessagessWithFilter([FromForm] UsersComing userWithIdOnlyAndOnUsernameIsFilter)
        {
            var users = new List<Users>();
            var messagesPreOrdered = await _context.Messages.Where(eb => eb.DeliveryId == userWithIdOnlyAndOnUsernameIsFilter.Id).ToListAsync();
            var messages = messagesPreOrdered.OrderByDescending(eb => eb.SendDate);
            List<Guid> authorIds = [];
            foreach (var message in messages)
            {
                authorIds.Add((Guid)message.AuthorId);
            }

            if(userWithIdOnlyAndOnUsernameIsFilter.Username == null)
            {
                users = await _context.Users.Where(eb => authorIds.Contains(eb.Id)).ToListAsync();
            }
            else
            {
                users = await _context.Users.Where(eb => authorIds.Contains(eb.Id)).Where(x => x.Username.Contains(userWithIdOnlyAndOnUsernameIsFilter.Username)).ToListAsync();
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
        public async Task<IActionResult> GetAllUserMessagess([FromBody] UsersComing userWithIdOnly)
        {
            var users = new List<Users>();
            var messagesPreOrdered = await _context.Messages.Where(eb => eb.DeliveryId == userWithIdOnly.Id).ToListAsync();
            var messages = messagesPreOrdered.OrderByDescending(eb => eb.SendDate);
            List<Guid> authorIds = [];    
            foreach (var message in messages)
            {
                authorIds.Add((Guid)message.AuthorId);
            }

            users = await _context.Users.Where(eb => authorIds.Contains(eb.Id)).ToListAsync();

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
        [Route("GetLast5UserMessages")]
        public async Task<IActionResult> GetLast5UserMessages([FromBody] UsersComing userWithIdOnly)
        {
            var users = new List<Users>();
            var messagesPreOrder = await _context.Messages.Where(eb => eb.DeliveryId == userWithIdOnly.Id).ToListAsync();
            var messages = messagesPreOrder.OrderByDescending(eb => eb.SendDate).Take(5);
            List<Guid> authorIds = [];
            foreach (var message in messages)
            {
                authorIds.Add((Guid)message.AuthorId);
            }

            users = await _context.Users.Where(eb => authorIds.Contains(eb.Id)).ToListAsync();

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
        [Route("reciveMessageSendByUserToUser")]
        public async Task<IActionResult> ReciveMessageSendByUsertoUser([FromForm] Messages message)
        {
            //Notification to user
            Notifications notification = new Notifications();
            notification.Id = Guid.NewGuid();
            notification.Author = await _context.Users.Where(eb => eb.Id == message.AuthorId).FirstOrDefaultAsync();
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
            messageToWrite.Author = await _context.Users.Where(eb => eb.Id == message.AuthorId).FirstOrDefaultAsync();

            //Database operations
            await _context.Messages.AddAsync(messageToWrite);
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            return Ok("wiadomosc dodana poprawnie");
        }
    }
}
