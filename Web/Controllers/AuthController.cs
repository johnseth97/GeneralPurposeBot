using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using GeneralPurposeBot.Web.Models.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GeneralPurposeBot.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("SignIn")]
        public IActionResult SignIn() =>
            Challenge(new AuthenticationProperties { RedirectUri = "/",  }, "Discord");

        [HttpGet("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            return Redirect("/");
        }

        [HttpGet("whoami")]
        public async Task<Whoami> WhoamiAsync()
        {
            return new Whoami(await HttpContext.AuthenticateAsync().ConfigureAwait(false));
        }
    }
}
