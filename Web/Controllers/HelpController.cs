using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralPurposeBot.Web.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeneralPurposeBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpController : ControllerBase
    {
        public CommandService CommandService { get; }

        public HelpController(CommandService commandService)
        {
            CommandService = commandService;
        }

        [HttpGet]
        public List<Module> GlobalHelp()
        {
            var modules = new List<Module>();
            foreach (var module in CommandService.Modules.Where(m => m.Parent == null))
                modules.Add(new Module(module));
            return modules;
        }
    }
}
