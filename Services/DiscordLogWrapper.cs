using Discord;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services
{
    public class DiscordLogWrapper
    {
        private ILogger<DiscordLogWrapper> Logger { get; }
        public DiscordLogWrapper(ILogger<DiscordLogWrapper> logger)
        {
            Logger = logger;
        }
        public Task Log(LogMessage msg)
        {
            var logMsg = $"{msg.Source} - {msg.Message}";
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    Logger.LogCritical(logMsg);
                    break;
                case LogSeverity.Error:
                    Logger.LogError(logMsg);
                    break;
                case LogSeverity.Warning:
                    Logger.LogWarning(logMsg);
                    break;
                case LogSeverity.Info:
                    Logger.LogInformation(logMsg);
                    break;
                case LogSeverity.Verbose:
                    Logger.LogDebug(logMsg);
                    break;
                case LogSeverity.Debug:
                    Logger.LogTrace(logMsg);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
