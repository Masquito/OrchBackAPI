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
        [Route("GetAllUserMessagess")]
        public ActionResult GetAllUserMessagess([FromBody] UsersComing userWithIdOnly)
        {
            var users = new List<Users>();
            var messages = _context.Messages.Where(eb => eb.DevlieryId == userWithIdOnly.Id).ToList().OrderByDescending(eb => eb.SendDate);
            List<Guid> authorIds = [];    
            foreach (var message in messages)
            {
                authorIds.Add(message.AuthorId);
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
    }
}
