using Discord;
using Discord.Commands;
using GeneralPurposeBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [Name("Help"), Group("help"), Summary("Gets help on commands.")]
    [CoreModule]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        public CommandHandler CommandHandler { get; set; }

        [Command, Summary("List modules")]
        public async Task ModuleList()
        {
            var eb = new EmbedBuilder()
                .WithTitle("Module List")
                .WithDescription("To get help on a module, do `help module <modulename>`\n")
                .WithColor(0, 0, 255);
            foreach (var module in CommandHandler.Commands.Modules)
            {
                eb.WithDescription(eb.Description + "\n**" + module.Name + "** - " + (module.Summary ?? "No summary for module."));
            }
            await Context.Channel.SendMessageAsync("", false, eb.Build());
        }

        [Command("module"), Summary("List commands in a module")]
        public async Task ModuleCommandList(string moduleName = "help")
        {
            var matchingModules = CommandHandler.Commands.Modules.Where(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (!matchingModules.Any())
            {
                await Context.Channel.SendMessageAsync("No modules with that name found!");
                return;
            }
            var eb = new EmbedBuilder()
                .WithTitle("Commands in module: " + moduleName)
                .WithDescription("To get help on a command, do `help command <commandname>`\n")
                .WithColor(0, 0, 255);
            var module = matchingModules.First();
            if (!string.IsNullOrEmpty(module.Remarks))
            {
                eb.WithDescription(module.Remarks + "\n\n" + eb.Description);
            }
            foreach (var command in module.Commands)
            {
                eb.WithDescription(eb.Description + "\n**" + command.Aliases[0] + "** - " + (command.Summary ?? "No summary for command."));
            }
            await Context.Channel.SendMessageAsync("", false, eb.Build());
        }

        [Command("command"), Summary("Get help on a specific command.")]
        public async Task CommandInfo(params string[] commandName)
        {
            var joinedCommandName = string.Join(' ', commandName);
            var matchingCommands = CommandHandler.Commands.Commands.Where(c => c.Aliases[0] == joinedCommandName);
            if (!matchingCommands.Any())
            {
                await Context.Channel.SendMessageAsync("No commands with that name were found in that module!");
                return;
            }
            if (matchingCommands.Count() > 1)
            {
                await Context.Channel.SendMessageAsync("Somehow, multiple commands matched that! Poke a bot developer.");
                return;
            }
            var command = matchingCommands.First();
            var eb = new EmbedBuilder()
                .WithTitle("Command: " + joinedCommandName)
                .WithDescription("Command summary: " + command.Summary + "\n")
                .WithColor(0, 0, 255);
            var usage = "**" + joinedCommandName + "** ";
            foreach (var parameter in command.Parameters)
            {
                usage += parameter.IsOptional ? "[" : "<";
                usage += parameter.Type.Name;
                usage += " ";
                usage += parameter.Name;
                usage += parameter.IsOptional ? (" = " + parameter.DefaultValue) : "";
                usage += parameter.IsOptional ? "] " : "> ";
            }
            eb.AddField("Usage", usage);
            await Context.Channel.SendMessageAsync("", false, eb.Build());
        }
    }
}
