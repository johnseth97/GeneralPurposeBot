using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Preconditions
{
    public class RequireGuildAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, CommandInfo command, IServiceProvider services)
        {
            if (ctx.Guild == null)
                return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command"));
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
