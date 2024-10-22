using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBitcoin.Protocol;
using NBitcoin.Secp256k1;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Orch_back_API.Entities;

namespace Orch_back_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private static string APIKey = "b8af0de6aa8e4d8aae4902937a7386ff";
        private static string InfuraHTTPS = "https://mainnet.infura.io/v3/b8af0de6aa8e4d8aae4902937a7386ff";
        private static string localGanacheNetwork = "HTTP://127.0.0.1:7545";

        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _configuration;
        private MyJDBContext _dbContext;

        public PaymentsController(ILogger<PaymentsController> logger, IConfiguration configuration, MyJDBContext dbContext)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._dbContext = dbContext;
        }

        [HttpPost]
        [Route("paymentcheck")]
        public async Task<IActionResult> PaymentChecking([FromBody] UsersComing user)
        {
            //var web3 = new Web3(localGanacheNetwork);
            //var balance = await web3.Eth.GetBalance.SendRequestAsync("");
            //var etherAmount = Web3.Convert.FromWei(balance.Value);


            //First let's create an account with our private key for the account address 
            var privateKey = "";
            var chainId = 1337;
            var account = new Account(privateKey, chainId);
            Console.WriteLine("Our account: " + account.Address);
            //Now let's create an instance of Web3 using our account pointing to our local chain
            var web3 = new Web3(account, localGanacheNetwork);
            web3.TransactionManager.UseLegacyAsDefault = true; //Using legacy option instead of 1559



            // Check the balance of the account we are going to send the Ether
            var balance = await web3.Eth.GetBalance.SendRequestAsync("");
            Console.WriteLine("Receiver account balance before sending Ether: " + balance.Value + " Wei");
            Console.WriteLine("Receiver account balance before sending Ether: " + Web3.Convert.FromWei(balance.Value) +
                              " Ether");

            // Lets transfer 1.11 Ether
            var transaction = await web3.Eth.GetEtherTransferService()
                .TransferEtherAndWaitForReceiptAsync("", 1.11m);

            balance = await web3.Eth.GetBalance.SendRequestAsync("");
            Console.WriteLine("Receiver account balance after sending Ether: " + balance.Value);
            Console.WriteLine("Receiver account balance after sending Ether: " + Web3.Convert.FromWei(balance.Value) +
                              " Ether");
            var transferHandler = web3.Eth.GetEtherTransferService();
            var receipt = web3.Eth.Transactions.GetTransactionReceipt.CreateBatchItem("", 3);
            return Ok(new {balance});
        }

        [HttpPost]
        [Route("storetxhash")]
        public async Task<IActionResult> Storetxhash([FromBody] UsersComing userWithtxhash)
        {
            await _dbContext.Users.Where(eb => eb.Id == userWithtxhash.Id).ExecuteUpdateAsync(setters => setters
                .SetProperty(eb => eb.Role, userWithtxhash.City));
            if(userWithtxhash.City == "FUA")
            {
                Notifications notification = new Notifications();
                notification.Id = Guid.NewGuid();
                var user = await _dbContext.Users.Where(eb => eb.Id == userWithtxhash.Id).FirstOrDefaultAsync();
                notification.Author = user;
                notification.Content = "SYSTEM: Your transaction has been completed. Your are now a Full Access User.";
                notification.SendDate = DateTime.Now;
                notification.DeliveryId = user!.Id;
                notification.AuthorId = null;
                await _dbContext.AddAsync(notification);
                await _dbContext.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        [Route("gettxhash")]
        public async Task<IActionResult> Gettxhash([FromBody] UsersComing usercoming)
        {
            var userfull = await _dbContext.Users.Where(ev => ev.Id == usercoming.Id).FirstOrDefaultAsync();
            return Ok(new {userfull!.Role});
        }

    }
}
