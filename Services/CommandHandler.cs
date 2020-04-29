using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;
using System;

namespace GeneralPurposeBot.Services
{
    public class CommandHandler
    {
        // setup fields to be set later in the constructor
        private readonly IConfiguration _config;
        public readonly CommandService Commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;


        public CommandHandler(IServiceProvider services)
        {
            // juice up the fields with these services
            // since we passed the services in, we can use GetRequiredService to pass them into the fields set earlier
            _config = services.GetRequiredService<IConfiguration>();
            Commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            // take action when we execute a command
            Commands.CommandExecuted += CommandExecutedAsync;

            // take action when we receive a message (so we can process it, and see if it is a valid command)
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        // this class is where the magic starts, and takes actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // ensures we don't process system/other bot messages
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            // sets the argument position away from the prefix we set
            var argPos = 0;

            // get prefix from the configuration file
            char prefix = Char.Parse(_config["Prefix"]);

            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos)))
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);

            // execute command if one is found that matches
            await Commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {

            // log success to the console and exit this method
            if (result.IsSuccess)
            {
                System.Console.WriteLine($"Command [{command.Value.Name}] executed for -> [{context.Message.Author}]");
                return;
            }

            if (!command.IsSpecified)
            {
                await context.Channel.SendMessageAsync("Command not found. Type !help for list of commands");
                return;
            }

            if (result.Error == CommandError.BadArgCount)
            {
                await context.Channel.SendMessageAsync("Too few arguments. Type !help <command name> for arguments!");
                return;
            }

            if (!result.IsSuccess && command.IsSpecified)
            {
                System.Console.WriteLine($"[{context.Message.Timestamp}]: The message [{context.Message}] from [{context.Message.Author}] triggered error [{result.Error}]");
                await context.Channel.SendMessageAsync("Congratulations! You broke something!");
                return;
            }
        }
    }
}

