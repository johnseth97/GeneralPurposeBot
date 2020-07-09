using Discord;
using Discord.Commands;
using GeneralPurposeBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Preconditions
{
    public class RequireTempVcAttribute : RequireVcAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command,
            IServiceProvider services)
        {
            var parent = await base.CheckPermissionsAsync(context, command, services).ConfigureAwait(false);
            if (!parent.IsSuccess) return parent;
            var spService = services.GetRequiredService<ServerPropertiesService>();
            var props = spService.GetProperties(context.Guild.Id);
            if (props.TempVoiceCategoryId == 0)
                return PreconditionResult.FromError("This server does not have temporary VCs configured!");
            var guildVoiceChannels = await context.Guild.GetVoiceChannelsAsync().ConfigureAwait(false);
            IVoiceChannel currentVc = null;
            foreach (var vc in guildVoiceChannels)
            {
                var members = await vc.GetUsersAsync().FlattenAsync().ConfigureAwait(false);
                if (members.Any(u => u.Id == context.User.Id)) currentVc = vc;
            }
            if (currentVc.CategoryId != props.TempVoiceCategoryId)
                return PreconditionResult.FromError("You are not in a temporary voice chat!");
            return PreconditionResult.FromSuccess();
        }
    }
}
