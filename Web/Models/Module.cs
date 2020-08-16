using Discord.Commands;
using GeneralPurposeBot.Web.Models.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Web.Models.Auth
{
    public class Module
    {
        public Module()
        {
        }

        public Module(ModuleInfo module)
        {
            Name = module.Name;
            Summary = module.Summary;
            Remarks = module.Remarks;
            if (module.Submodules.Count > 0)
            {
                var submodules = new List<Module>();
                foreach (var submodule in module.Submodules)
                    submodules.Add(new Module(submodule));
                Submodules = submodules;
            }
            var commands = new List<Command>();
            foreach (var command in module.Commands)
            {
                commands.Add(new Command(command));
            }
            Commands = commands;
        }

        public string Name { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public IEnumerable<Module> Submodules { get; set; }
        public IEnumerable<Command> Commands { get; set; }
    }
}
