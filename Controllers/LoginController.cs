using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using Orch_back_API.Entities;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Orch_back_API.Controllers
{
    /// <summary>
    ///   <p>Atrybuty do kontrollera:</p>
    ///   <p>Authorize -> Dostęp do zasobów kontrollera wymaga autoryzacji</p>
    ///   <p>Route -> Określa trasę, pod którą będzie dostępny kontroller w aplikacji</p>
    ///   <p>ApiController -> Wskazuje, że dany kontroller jest kontrollerem API<br /></p>
    /// </summary>
    /// 
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        /// <summary>
        ///     <p>Tutaj widzimy referencje do interfejsu IConfiguration oraz do Klasy będącej kontekstem bazy danych MyJDBContext a pod tym konstruktor</p>
        ///     <p>Ogólnie ten kawałek aż do [AllowAnonymous] odpowiada za wstrzykiwanie zależności</p>
        /// </summary>
        private readonly IConfiguration _config;
        private readonly MyJDBContext _context;
        public LoginController(IConfiguration config, MyJDBContext context)
        {
            _config = config;
            _context = context;
        }

        /// <summary>Uwierzytelnia użytkownika</summary>
        /// <param name="users">The users.</param>
        ///     <p>[AllowAnonymous] -> zezwala na dostęp do akcji nieuwierzytelnionemu użytkownikowi</p>
        ///     <p>[HttpPost] -> Ta metoda powinna być wykonywana w przypadku żądań HTTP typu POST</p>
        /// <returns>Kod odpowiedzi HTTP wraz z wiadomością "user not found" lub tokenem JWT oraz danymi faktycznego użytkownika</returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([FromBody] Users users)
        {
            PasswordHasher<Users> passwordHasher = new();
            var Shared = new Shared(this._context, this._config);
            var user = Shared.Authenticate(users);
            if (user != null)
            {
                var token = Shared.GenerateToken(user);
                return Ok(new { token, user });
            }

            return NotFound("user not found");
        }
    }
}
