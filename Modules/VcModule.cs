using Discord;
using Discord.Commands;
using GeneralPurposeBot.Preconditions;
using GeneralPurposeBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [Name("Voice Chat"), Summary("VC Management Commands")]
    [Group("vc")]
    [RequireGuild]
    public class VcModule : ModuleBase
    {
        public ServerPropertiesService SpService { get; }

        public VcModule(ServerPropertiesService spService)
        {
            SpService = spService;
        }

        /// <summary>
        /// Gets the voice channel the user from the current context is in
        /// </summary>
        /// <returns>The voice channel that the user is in</returns>
        public async Task<IVoiceChannel> GetUserVoiceChannel()
        {
            var guildVoiceChannels = await Context.Guild.GetVoiceChannelsAsync().ConfigureAwait(false);
            IVoiceChannel currentVc = null;
            foreach (var vc in guildVoiceChannels)
            {
                var members = await vc.GetUsersAsync().FlattenAsync().ConfigureAwait(false);
                if (members.Any(u => u.Id == Context.User.Id)) currentVc = vc;
            }
            return currentVc;
        }

        /// <summary>
        /// Returns whether the specified voice channel is managed by the bot
        /// </summary>
        /// <param name="vc">VC to check</param>
        /// <returns>If the VC is a temporary VC managed by the bot</returns>
        public bool IsVcManaged(IVoiceChannel vc)
        {
            var guild = vc.Guild;
            var props = SpService.GetProperties(guild.Id);
            return vc.CategoryId == props.TempVoiceCategoryId;
        }

        /// <summary>
        /// Returns if the user has permission to manage this voice channel via the bot.
        /// Only returns true if the current VC is managed by the bot.
        /// </summary>
        /// <param name="vc">VC to check</param>
        /// <returns>If the current user can manage the VC</returns>
        public async Task<bool> CanManageVc(IVoiceChannel vc)
        {
            var guildUser = await Context.Guild.GetUserAsync(Context.User.Id).ConfigureAwait(false);
            var perms = guildUser.GetPermissions(vc);
            return perms.ManageRoles && IsVcManaged(vc);
        }

        [Command("info")]
        public async Task Info()
        {
            var vc = await GetUserVoiceChannel().ConfigureAwait(false);
            if (vc == null)
            {
                await ReplyAsync("You are not in a VC!").ConfigureAwait(false);
                return;
            }
            var members = await vc.GetUsersAsync().FlattenAsync().ConfigureAwait(false);
            await ReplyAsync($"You are in **{vc.Name}** with **{members.Count()}** users in the vc. VC managed by bot: **{IsVcManaged(vc)}** VC can be managed by user: **{await CanManageVc(vc).ConfigureAwait(false)}**").ConfigureAwait(false);
        }

        [Command("private")]
        [Alias("public", "lock", "unlock")]
        public async Task Private()
        {
            var vc = await GetUserVoiceChannel().ConfigureAwait(false);
            if (vc == null)
            {
                await ReplyAsync("You are not in a VC!").ConfigureAwait(false);
                return;
            }
            if (!IsVcManaged(vc))
            {
                await ReplyAsync("You are not in a temporary VC!").ConfigureAwait(false);
                return;
            }
            if (!(await CanManageVc(vc).ConfigureAwait(false)))
            {
                await ReplyAsync("You do not have permission to manage this VC!").ConfigureAwait(false);
                return;
            }
            var perms = vc.GetPermissionOverwrite(Context.Guild.EveryoneRole) ?? OverwritePermissions.InheritAll;
            var wasPublic = (perms.ViewChannel == PermValue.Allow || perms.ViewChannel == PermValue.Inherit) &&
                (perms.Connect == PermValue.Allow || perms.Connect == PermValue.Inherit);
            var newPerm = wasPublic ? PermValue.Deny : PermValue.Allow;
            await vc.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, perms.Modify(viewChannel: newPerm, connect: newPerm)).ConfigureAwait(false);
            await ReplyAsync($"Your VC is now set to **{(wasPublic ? "private" : "public")}**").ConfigureAwait(false);
        }

        [Command("allow")]
        [Alias("disallow", "add", "remove")]
        public async Task Allow(IGuildUser target)
        {
            var vc = await GetUserVoiceChannel().ConfigureAwait(false);
            if (vc == null)
            {
                await ReplyAsync("You are not in a VC!").ConfigureAwait(false);
                return;
            }
            if (!IsVcManaged(vc))
            {
                await ReplyAsync("You are not in a temporary VC!").ConfigureAwait(false);
                return;
            }
            if (!(await CanManageVc(vc).ConfigureAwait(false)))
            {
                await ReplyAsync("You do not have permission to manage this VC!").ConfigureAwait(false);
                return;
            }
            var userPerms = target.GetPermissions(vc);
            var allowUser = !userPerms.Connect || !userPerms.ViewChannel;
            var newPerm = allowUser ? PermValue.Allow : PermValue.Inherit;
            var userOverwrite = vc.GetPermissionOverwrite(target) ?? OverwritePermissions.InheritAll;
            await vc.AddPermissionOverwriteAsync(target, userOverwrite.Modify(viewChannel: newPerm, connect: newPerm));
            await ReplyAsync($"**{target.Mention}** is now **{(allowUser ? "allowed" : "not allowed")}** to join the VC when it is private.").ConfigureAwait(false);
        }
    }
}
