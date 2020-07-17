using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Web.Models.Help
{
    public class Command
    {
        public Command(CommandInfo command)
        {
            Name = command.Name;
            Description = command.Summary;
            Aliases = command.Aliases;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Aliases { get; set; }
        public List<string> Usages { get; set; }
    }
}
