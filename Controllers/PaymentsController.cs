using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orch_back_API.Entities;

namespace Orch_back_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        //Fire programing music: https://www.youtube.com/watch?v=AF8LSurfct4&list=PLEM4vOSCprStzppPemEYAF6ZEUrQYj5N5&ab_channel=FilFar

        //Can be looked up on Infura account
        private static string APIKey = "b8af0de6aa8e4d8aae4902937a7386ff";
        private static string InfuraHTTPS = "https://mainnet.infura.io/v3/b8af0de6aa8e4d8aae4902937a7386ff";

        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _configuration;
        private MyJDBContext _dbContext;

        public PaymentsController(ILogger<PaymentsController> logger, IConfiguration configuration, MyJDBContext dbContext)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._dbContext = dbContext;
        }



    }
}
