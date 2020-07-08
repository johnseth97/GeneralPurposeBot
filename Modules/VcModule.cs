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

        /// <summary>
        /// Returns if everyone is able to join the VC
        /// </summary>
        /// <param name="vc">VC to check</param>
        /// <returns>If everyone is able to join the VC</returns>
        public bool IsVcPublic(IVoiceChannel vc)
        {
            var perms = vc.GetPermissionOverwrite(vc.Guild.EveryoneRole) ?? OverwritePermissions.InheritAll;
            return (perms.ViewChannel == PermValue.Allow || perms.ViewChannel == PermValue.Inherit) &&
                (perms.Connect == PermValue.Allow || perms.Connect == PermValue.Inherit);
        }
        public bool IsVcNsfw(IVoiceChannel vc)
        {
            var props = SpService.GetProperties(vc.Guild.Id);
            if (props.NsfwRoleId == 0)
                return false;
            var isPrivate = !IsVcPublic(vc);
            var perms = vc.GetPermissionOverwrite(vc.Guild.GetRole(props.NsfwRoleId)) ?? OverwritePermissions.InheritAll;
            return isPrivate && (perms.ViewChannel == PermValue.Allow && perms.Connect == PermValue.Allow);
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
            await ReplyAsync($"You are in **{vc.Name}** with **{members.Count()}** users in the vc.\n" +
                $"VC managed by bot: **{IsVcManaged(vc)}**\n" +
                $"VC can be managed by user: **{await CanManageVc(vc).ConfigureAwait(false)}**\n" +
                $"VC is public: **{IsVcPublic(vc)}**\n" +
                $"VC is NSFW: **{IsVcNsfw(vc)}**")
                .ConfigureAwait(false);
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
            var wasPublic = IsVcPublic(vc);
            var perms = vc.GetPermissionOverwrite(Context.Guild.EveryoneRole) ?? OverwritePermissions.InheritAll;
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
            await vc.AddPermissionOverwriteAsync(target, userOverwrite.Modify(viewChannel: newPerm, connect: newPerm)).ConfigureAwait(false);
            await ReplyAsync($"**{target.Mention}** is now **{(allowUser ? "allowed" : "not allowed")}** to join the VC when it is private.").ConfigureAwait(false);
        }

        [Command("nsfw")]
        public async Task Nsfw()
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
            if (!await CanManageVc(vc).ConfigureAwait(false))
            {
                await ReplyAsync("You do not have permission to manage this VC!").ConfigureAwait(false);
                return;
            }
            var props = SpService.GetProperties(vc.GuildId);
            if (props.NsfwRoleId == 0)
            {
                await ReplyAsync("There is no NSFW role defined! Have a server admin run `serverproperties nsfwrole [rolename]`").ConfigureAwait(false);
                return;
            }
            var nsfw = !IsVcNsfw(vc);
            var newPerm = nsfw ? PermValue.Allow : PermValue.Inherit;
            var nsfwRole = vc.Guild.GetRole(props.NsfwRoleId);
            var perms = vc.GetPermissionOverwrite(nsfwRole) ?? OverwritePermissions.InheritAll;
            await vc.AddPermissionOverwriteAsync(nsfwRole, perms.Modify(viewChannel: newPerm, connect: newPerm)).ConfigureAwait(false);
            var message = $"This VC is now **{(nsfw ? "NSFW" : "SFW")}**";
            if (nsfw && IsVcPublic(vc))
            {
                var everyonePerms = vc.GetPermissionOverwrite(Context.Guild.EveryoneRole) ?? OverwritePermissions.InheritAll;
                everyonePerms = everyonePerms.Modify(viewChannel: PermValue.Deny, connect: PermValue.Deny);
                await vc.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, everyonePerms).ConfigureAwait(false);
                message += "\n**Note:** Your VC was public before, and we made it NSFW, so we turned it into a private VC. ";
                message += "Everyone with the NSFW role is still able to join, but you will need to manually turn it back ";
                message += "info a public VC if you make the VC SFW again.";
            }
            else if (!nsfw && !IsVcPublic(vc))
            {
                message += "\n**Note:** Your VC is still private! Only those you allow with `vc allow` are able to join. ";
                message += "Run `vc private` to make it public again.";
            }
            await ReplyAsync(message).ConfigureAwait(false);
        }
    }
}
