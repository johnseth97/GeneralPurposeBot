using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Web.Models.Auth
{
    public class Whoami
    {
        public Whoami()
        {
        }

        public Whoami(AuthenticateResult authResult)
        {
            Authenticated = authResult.Succeeded;
            FailureReason = authResult.Failure?.Message;
            if (Authenticated)
            {
                AvatarUrl = authResult.Principal.FindFirstValue("urn:discord:avatar:url");
                Username = authResult.Principal.Identity.Name;
                Discriminator = int.Parse(authResult.Principal.FindFirstValue("urn:discord:user:discriminator"));
            }
        }

        public bool Authenticated { get; set; }
        public string FailureReason { get; set; }
        public string AvatarUrl { get; set; }
        public string Username { get; set; }
        public int Discriminator { get; set; }
    }
}
