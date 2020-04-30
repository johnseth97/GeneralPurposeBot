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

            var resultStr = result.IsSuccess ? "Success" : (result.Error + ": " + result.ErrorReason);
            System.Console.WriteLine($"Command [{command.Value.Name}] executed for -> [{context.Message.Author}] (Result: {resultStr})");
            if (!result.IsSuccess)
            {
                // I'm not 100% sure that these are what some of these mean tbh.
                switch (result.Error)
                {
                    case CommandError.UnknownCommand:
                        await context.Channel.SendMessageAsync("Command not found. Type !help for list of commands").ConfigureAwait(false);
                        break;
                    case CommandError.ParseFailed:
                        await context.Channel.SendMessageAsync($"Could not parse the arguments for this command! {result.ErrorReason}").ConfigureAwait(false);
                        var logChannel = _client.GetChannel(ulong.Parse(_config["errorLogChannel"])) as ITextChannel;
                        await logChannel.SendMessageAsync($"Error while executing `{command.Value.Name}` for {context.Message.Author}: {resultStr}\n```{result.ErrorReason}```").ConfigureAwait(false);
                        break;
                    case CommandError.BadArgCount:
                        await context.Channel.SendMessageAsync("Not enough arguments for this command!").ConfigureAwait(false);
                        break;
                    case CommandError.ObjectNotFound:
                        await context.Channel.SendMessageAsync("Internal error while getting command arguments!").ConfigureAwait(false);
                        logChannel = _client.GetChannel(ulong.Parse(_config["errorLogChannel"])) as ITextChannel;
                        await logChannel.SendMessageAsync($"Error while executing `{command.Value.Name}` for {context.Message.Author}: {resultStr}\n```{result.ErrorReason}```").ConfigureAwait(false);
                        break;
                    case CommandError.MultipleMatches:
                        await context.Channel.SendMessageAsync("Multiple matches were found for one of the command's arguments!").ConfigureAwait(false);
                        break;
                    case CommandError.UnmetPrecondition:
                        await context.Channel.SendMessageAsync($"Unmet precondition for command: {result.ErrorReason}").ConfigureAwait(false);
                        break;
                    case CommandError.Exception:
                    case CommandError.Unsuccessful:
                        await context.Channel.SendMessageAsync("Internal error while executing command!").ConfigureAwait(false);
                        logChannel = _client.GetChannel(ulong.Parse(_config["errorLogChannel"])) as ITextChannel;
                        await logChannel.SendMessageAsync($"Error while executing `{command.Value.Name}` for {context.Message.Author}: {resultStr}\n```{result.ErrorReason}```").ConfigureAwait(false);
                        break;
                }
            }
        }
    }
}

