using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
            Usage = "";
            foreach (var param in command.Parameters)
            {
                Usage += param.IsOptional ? "[" : "<";
                Usage += param.Type.Name;
                Usage += " ";
                Usage += param.Name;
                Usage += param.IsOptional ? (" = " + param.DefaultValue != null ? param.DefaultValue : "null") : "";
                Usage += param.IsOptional ? "] " : "> ";
            }
            Usage = Usage.Trim();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Aliases { get; set; }
        public string Usage { get; set; }
    }
}
