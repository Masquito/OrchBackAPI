using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orch_back_API.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Orch_back_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly MyJDBContext _context;
        public NotificationsController( MyJDBContext context)
        {
            this._context = context;
        }

        [HttpPost]
        public ActionResult GetAllNotificationsFrom3Months([FromBody] Users loggedUserWithIdOnly)
        {
            var coPrzyszlo = loggedUserWithIdOnly;
            List<Notifications> loggedUserNotificationsFrom3Months = [.. _context.Notifications.Where(x => x.DeliveryId == loggedUserWithIdOnly.Id).Where(x => x.SendDate > DateTime.UtcNow.AddMonths(-3))];
            if(loggedUserNotificationsFrom3Months.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(loggedUserNotificationsFrom3Months);
            }
        }

        [HttpPost]
        [Route("DeleteNotification")]
        public ActionResult DeleteNotification([FromBody] Notifications notificationWithIdOnly)
        {
            var coPrzyszl = notificationWithIdOnly;
            _context.Notifications.Remove(coPrzyszl);
            _context.SaveChanges();
            return Ok();
        }
    }
}
