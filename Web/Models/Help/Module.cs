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
        public Module(ModuleInfo module)
        {
            Name = module.Name;
            Description = module.Summary;
            if (module.Submodules.Count > 0)
            {
                Submodules = new List<Module>();
                foreach (var submodule in module.Submodules)
                    Submodules.Add(new Module(submodule));
            }
            Commands = new List<Command>();
            foreach (var command in module.Commands)
            {
                Commands.Add(new Command(command));
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<Module> Submodules { get; set; }
        public List<Command> Commands { get; set; }
    }
}
