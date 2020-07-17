using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using GeneralPurposeBot.Web.Models.Auth;

namespace GeneralPurposeBot.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet]
        public IActionResult SignIn() =>
            Challenge(new AuthenticationProperties { RedirectUri = "/",  }, "Discord");

        [HttpGet("whoami")]
        public async Task<Whoami> WhoamiAsync()
        {
            return new Whoami(await HttpContext.AuthenticateAsync().ConfigureAwait(false));
        }
    }
}
