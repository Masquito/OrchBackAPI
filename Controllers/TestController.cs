using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orch_back_API.Entities;

namespace Orch_back_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly MyJDBContext myJDBContext;

        public TestController()
        {
            this.myJDBContext = new MyJDBContext();
        }

        [HttpGet]
        public IActionResult get_data_test()
        {
            var user = myJDBContext.Users.First();
            return Ok(user);
        }
    }
}
