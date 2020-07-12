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
    public class RequireTempVcManagementAttribute : RequireTempVcAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command,
            IServiceProvider services)
        {
            var parent = await base.CheckPermissionsAsync(context, command, services).ConfigureAwait(false);
            if (!parent.IsSuccess) return parent;

            var guildVoiceChannels = await context.Guild.GetVoiceChannelsAsync().ConfigureAwait(false);
            IVoiceChannel currentVc = null;
            foreach (var vc in guildVoiceChannels)
            {
                var members = await vc.GetUsersAsync().FlattenAsync().ConfigureAwait(false);
                if (members.Any(u => u.Id == context.User.Id)) currentVc = vc;
            }
            var guildUser = await context.Guild.GetUserAsync(context.User.Id).ConfigureAwait(false);
            var perms = guildUser.GetPermissions(currentVc);
            if (!perms.ManageRoles)
                return PreconditionResult.FromError("You do not have permission to manage this voice chat");
            return PreconditionResult.FromSuccess();
        }
    }
}
