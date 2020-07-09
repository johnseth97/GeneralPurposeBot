using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Preconditions
{
    public class RequireVcAttribute : RequireGuildAttribute
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
            if (currentVc == null)
                return PreconditionResult.FromError("You must be in a VC to run this command");
            return PreconditionResult.FromSuccess();
        }
    }
}
