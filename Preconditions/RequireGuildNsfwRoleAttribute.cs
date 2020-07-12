using Discord.Commands;
using GeneralPurposeBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GeneralPurposeBot.Preconditions
{
    public class RequireGuildNsfwRoleAttribute : RequireGuildAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, CommandInfo command, IServiceProvider services)
        {
            var parent = await base.CheckPermissionsAsync(ctx, command, services).ConfigureAwait(false);
            if (!parent.IsSuccess)
                return parent;
            var guildProps = services.GetRequiredService<ServerPropertiesService>().GetProperties(ctx.Guild.Id);
            if (guildProps.NsfwRoleId == 0)
                return PreconditionResult.FromError("This guild does not have an NSFW role set! Do `serverproperties nsfwrole [role]` to set it!");
            return PreconditionResult.FromSuccess();
        }
    }
}
