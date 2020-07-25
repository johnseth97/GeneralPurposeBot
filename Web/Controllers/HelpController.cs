using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralPurposeBot.Services;
using GeneralPurposeBot.Web.Models.Auth;
using GeneralPurposeBot.Web.Models.Help;
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
        public ServerPropertiesService SpService { get; }

        public HelpController(CommandService commandService, ServerPropertiesService spService)
        {
            CommandService = commandService;
            SpService = spService;
        }


        [HttpGet]
        public List<Module> GlobalHelp()
        {
            var modules = new List<Module>();
            foreach (var module in CommandService.Modules.Where(m => m.Parent == null))
                modules.Add(new Module(module));
            return modules;
        }

        private IEnumerable<Module> GetServerModules(IEnumerable<ModuleInfo> modules, ulong serverId)
        {
            // if we get passed an empty submodule list, return null for convenience
            if (modules == null) return null;

            // we only want top-level modules in the base list
            return modules.Where(m => m.Parent == null)
                // only keep enabled top-level modules
                .Where(m => SpService.IsModuleEnabled(m.GetFullName(), serverId))
                // convert to API module objects
                .Select(m => new Module()
                {
                    Name = m.Name,
                    Description = m.Summary,
                    Commands = m.Commands.Select(c => new Command(c)),
                    Submodules = GetServerModules(m.Submodules, serverId)
                });
        }

        [HttpGet("{server}")]
        public IEnumerable<Module> ServerHelp(ulong server)
        {
            return GetServerModules(CommandService.Modules, server);
        }
    }
}
