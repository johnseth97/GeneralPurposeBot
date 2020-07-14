using Discord;
using Discord.Commands;
using GeneralPurposeBot.Preconditions;
using GeneralPurposeBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [Name("VoiceChat"), Summary("VC Management Commands")]
    [Group("vc")]
    [RequireGuild]
    public class VcModule : ModuleBase
    {
        public TempVcService TempVcService { get; set; }
        public VcModule(TempVcService tempVcService)
        {
            TempVcService = tempVcService;
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

        [Command("info")]
        [RequireVc]
        public async Task Info()
        {
            var vc = await GetUserVoiceChannel().ConfigureAwait(false);
            var members = await vc.GetUsersAsync().FlattenAsync().ConfigureAwait(false);
            await ReplyAsync($"You are in **{vc.Name}** with **{members.Count()}** users in the vc.\n" +
                $"VC managed by bot: **{TempVcService.IsVcManaged(vc)}**\n" +
                $"VC can be managed by user: **{await TempVcService.CanManageVc(vc, Context.User.Id).ConfigureAwait(false)}**\n" +
                $"VC is public: **{TempVcService.IsVcPublic(vc)}**\n" +
                $"VC is NSFW: **{TempVcService.IsVcNsfw(vc)}**")
                .ConfigureAwait(false);
        }

        [Command("private")]
        [Alias("public", "lock", "unlock")]
        [RequireTempVcManagement]
        public async Task Private()
        {
            var vc = await GetUserVoiceChannel().ConfigureAwait(false);
            var wasPublic = TempVcService.IsVcPublic(vc);
            await TempVcService.SetPrivate(vc, wasPublic).ConfigureAwait(false);
            await ReplyAsync($"Your VC is now set to **{(wasPublic ? "private" : "public")}**").ConfigureAwait(false);
        }

        [Command("allow")]
        [Alias("disallow", "add", "remove")]
        [RequireTempVcManagement]
        public async Task Allow(IGuildUser target)
        {
            var vc = await GetUserVoiceChannel().ConfigureAwait(false);
            var allowUser = !TempVcService.IsUserAllowed(vc, target);
            await TempVcService.SetUserAllowed(vc, target, allowUser).ConfigureAwait(false);
            await ReplyAsync($"**{target.Mention}** is now **{(allowUser ? "allowed" : "not allowed")}** to join the VC when it is private.").ConfigureAwait(false);
        }

        [Command("nsfw")]
        [RequireTempVcManagement]
        [RequireGuildNsfwRole]
        public async Task Nsfw()
        {
            var vc = await GetUserVoiceChannel().ConfigureAwait(false);
            var nsfw = !TempVcService.IsVcNsfw(vc);
            await TempVcService.SetVcNsfw(vc, nsfw).ConfigureAwait(false);
            var message = $"This VC is now **{(nsfw ? "NSFW" : "SFW")}**";
            if (nsfw && TempVcService.IsVcPublic(vc))
            {
                await TempVcService.SetPrivate(vc, true).ConfigureAwait(false);
                message += "\n**Note:** Your VC was public before, and we made it NSFW, so we turned it into a private VC. ";
                message += "Everyone with the NSFW role is still able to join, but you will need to manually turn it back ";
                message += "info a public VC if you make the VC SFW again.";
            }
            else if (!nsfw && !TempVcService.IsVcPublic(vc))
            {
                message += "\n**Note:** Your VC is still private! Only those you allow with `vc allow` are able to join. ";
                message += "Run `vc private` to make it public again.";
            }
            await ReplyAsync(message).ConfigureAwait(false);
        }

        [Command("trust")]
        [Alias("untrust", "addowner", "removeowner", "owner")]
        [RequireTempVcManagement]
        public async Task Trust(IGuildUser target)
        {
            var vc = await GetUserVoiceChannel().ConfigureAwait(false);
            var canManage = !TempVcService.IsUserTrusted(vc, target);
            await TempVcService.SetUserTrusted(vc, target, !canManage).ConfigureAwait(false);
            await ReplyAsync($"{target.Mention} is **{(canManage ? "no longer able" : "able")}** to manage this VC.").ConfigureAwait(false);
        }
    }
}