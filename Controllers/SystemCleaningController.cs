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
    public class SystemCleaningController : ControllerBase
    {
        private readonly MyJDBContext _context;
        public SystemCleaningController(MyJDBContext context)
        {
            this._context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CleanOldNotifications()
        {
            await _context.Notifications.Where(eb => eb.SendDate < DateTime.UtcNow.AddDays(-3)).ExecuteDeleteAsync();
            return Ok();
        }
    }
}
