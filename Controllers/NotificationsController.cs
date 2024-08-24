using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orch_back_API.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Orch_back_API.Controllers
{
    public class objToSend
    {
        public Notifications notification { get; set; }
        public String dateS { get; set; }
        public String dateL { get; set; }
    }
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
        public ActionResult GetAllNotificationsFrom3Days([FromBody] Users loggedUserWithIdOnly)
        {
            List<objToSend> toSend = new List<objToSend>();
            List<Notifications> loggedUserNotificationsFrom3Months = [.. _context.Notifications.Where(x => x.DeliveryId == loggedUserWithIdOnly.Id).Where(x => x.SendDate > DateTime.UtcNow.AddDays(-3))];

            foreach(var notification in loggedUserNotificationsFrom3Months)
            {
                DateTime data = (DateTime)notification.SendDate;
                objToSend not = new objToSend();
                not.notification = notification;
                not.dateS = data.Hour.ToString() + ":" + checkIfAdd0(data.Minute);
                not.dateL = data.Year.ToString() + "." + checkIfAdd0(data.Month) + "." + checkIfAdd0(data.Day);
                toSend.Add(not);
            }

            if(loggedUserNotificationsFrom3Months.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(toSend);
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

        public string checkIfAdd0(int input)
        {
            if(input < 10)
            {
                return "0" + input.ToString();
            }
            else
            {
                return input.ToString();
            }
        }
    }
}
