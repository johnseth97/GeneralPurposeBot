using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using GeneralPurposeBot.Web.Models;

namespace GeneralPurposeBot.Services
{
    public class CommandHandler
    {
        // setup fields to be set later in the constructor
        private readonly IConfiguration _config;
        public readonly CommandService Commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly ServerPropertiesService _spService;

        public CommandHandler(IServiceProvider services, IConfiguration config, CommandService commandService, DiscordSocketClient client, ServerPropertiesService spService)
        {
            // didn't grab these from the service provider because it's usually considered better convention to have them defined in the constructor
            _config = config;
            Commands = commandService;
            _client = client;
            _spService = spService;
            _services = services;

            // take action when we execute a command
            Commands.CommandExecuted += CommandExecutedAsync;

            // take action when we receive a message (so we can process it, and see if it is a valid command)
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services).ConfigureAwait(false);
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

            // get prefix from the database (if we can)
            string prefix;
            if (message.Channel is IGuildChannel guildChannel)
            {
                var props = _spService.GetProperties(guildChannel.GuildId);
                prefix = props.Prefix ?? _config["Prefix"];
            }
            else
            {
                prefix = _config["Prefix"];
            }

            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasStringPrefix(prefix, ref argPos)))
            {
                return;
            }
            var context = new SocketCommandContext(_client, message);

            // figure out what module the command is in (if any)
            // command search code based on code from CommandService.ExecuteAsync
            var searchResult = await CommandSearch(context, argPos).ConfigureAwait(false);

            bool enabled = true;
            if (context.Guild != null)
                enabled = _spService.IsModuleEnabled(searchResult.Module, context.Guild.Id);
            // execute command if one is found that matches and it's in an enabled module
            if (enabled)
                await Commands.ExecuteAsync(context, argPos, _services).ConfigureAwait(false);
        }

        // Basically the Discord.Net CommandService's ExecuteAsync method, but without actually running the command.
        public async Task<CommandInfo> CommandSearch(ICommandContext context, int argPos)
        {
            var searchResult = Commands.Search(context, argPos);
            if (!searchResult.IsSuccess)
            {
                return null;
            }

            var commands = searchResult.Commands;
            var preconditionResults = new Dictionary<CommandMatch, PreconditionResult>();

            foreach (var match in commands)
            {
                preconditionResults[match] = await match.Command.CheckPreconditionsAsync(context, _services).ConfigureAwait(false);
            }

            var successfulPreconditions = preconditionResults
                .Where(x => x.Value.IsSuccess)
                .ToArray();

            if (successfulPreconditions.Length == 0)
            {
                //All preconditions failed, return the one from the highest priority command
                var bestCandidate = preconditionResults
                    .OrderByDescending(x => x.Key.Command.Priority)
                    .FirstOrDefault(x => !x.Value.IsSuccess);
                return bestCandidate.Key.Command;
            }

            //If we get this far, at least one precondition was successful.

            var parseResultsDict = new Dictionary<CommandMatch, ParseResult>();
            foreach (var pair in successfulPreconditions)
            {
                var parseResult = await pair.Key.ParseAsync(context, searchResult, pair.Value, _services).ConfigureAwait(false);

                if (parseResult.Error == CommandError.MultipleMatches)
                {
                    IReadOnlyList<TypeReaderValue> argList, paramList;
                    argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                    paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                    parseResult = ParseResult.FromSuccess(argList, paramList);
                    break;
                }

                parseResultsDict[pair.Key] = parseResult;
            }

            // Calculates the 'score' of a command given a parse result
            float CalculateScore(CommandMatch match, ParseResult parseResult)
            {
                float argValuesScore = 0, paramValuesScore = 0;

                if (match.Command.Parameters.Count > 0)
                {
                    var argValuesSum = parseResult.ArgValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;
                    var paramValuesSum = parseResult.ParamValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;

                    argValuesScore = argValuesSum / match.Command.Parameters.Count;
                    paramValuesScore = paramValuesSum / match.Command.Parameters.Count;
                }

                var totalArgsScore = (argValuesScore + paramValuesScore) / 2;
                return match.Command.Priority + totalArgsScore * 0.99f;
            }

            //Order the parse results by their score so that we choose the most likely result to execute
            var parseResults = parseResultsDict
                .OrderByDescending(x => CalculateScore(x.Key, x.Value));

            var successfulParses = parseResults
                .Where(x => x.Value.IsSuccess)
                .ToArray();

            if (successfulParses.Length == 0)
            {
                //All parses failed, return the one from the highest priority command, using score as a tie breaker
                var bestMatch = parseResults
                    .FirstOrDefault(x => !x.Value.IsSuccess);

                return bestMatch.Key.Command;
            }

            //If we get this far, at least one parse was successful. Return the most likely overload.
            return successfulParses[0].Key.Command;
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
                        await context.Channel.SendMessageAsync($"Internal error while getting command arguments - {result.ErrorReason}").ConfigureAwait(false);
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

