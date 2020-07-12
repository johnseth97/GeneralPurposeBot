using Discord;
using Discord.Rest;
using Discord.WebSocket;
using GeneralPurposeBot.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services
{
    public class TempVcService
    {
        private DiscordSocketClient Client { get; }
        private ServerPropertiesService SpService { get; }
        private ILogger<TempVcService> Logger { get; }

        private readonly Dictionary<IGuildUser, long> spamProtectionDictionary = new Dictionary<IGuildUser, long>();
        private readonly Dictionary<IGuildUser, int> spamProtectionCountDictionary = new Dictionary<IGuildUser, int>();
        public TempVcService(DiscordSocketClient client, ServerPropertiesService spService, ILogger<TempVcService> logger)
        {
            Client = client;
            SpService = spService;
            Logger = logger;

            client.UserVoiceStateUpdated += UserVoiceStateUpdated;
        }

        public async Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            var guild = (after.VoiceChannel != null ? after : before).VoiceChannel.Guild;
            if (before.VoiceChannel?.Id == after.VoiceChannel?.Id) return;

            var props = SpService.GetProperties(guild.Id);
            var guildUser = guild.GetUser(user.Id);
            if (guildUser == null) return;
            var name = (!string.IsNullOrEmpty(guildUser.Nickname)) ? guildUser.Nickname : user.Username;

            // If the user switched to a 'create vc' channel
            if (after.VoiceChannel != null && after.VoiceChannel.Id == props.TempVoiceCreateChannelId)
            {
                if (!await DoSpamChecks(guildUser, props, guild).ConfigureAwait(false)) return;
                Logger.LogInformation("Creating vc in {guildName} for {name}", guild.Name, $"{guildUser.Username}#{guildUser.Discriminator}");
                var newVoiceChannel = await guild.CreateVoiceChannelAsync(name + "'s Voice Chat",
                    (properties) => properties.CategoryId = props.TempVoiceCategoryId)
                    .ConfigureAwait(false);

                if (!props.SimpleTempVc)
                {
                    var perms = GetOwnerPermissions(newVoiceChannel);
                    await newVoiceChannel.AddPermissionOverwriteAsync(user, perms).ConfigureAwait(false);
                }
                await guildUser.ModifyAsync(vcUser => vcUser.Channel = newVoiceChannel).ConfigureAwait(false);
            }
            await RemoveOldVc(before, props).ConfigureAwait(false);
            await ChangeOwnerIfCreatorLeft(before, props, user, guildUser).ConfigureAwait(false);
        }

        public static OverwritePermissions GetOwnerPermissions(IVoiceChannel newVoiceChannel)
        {
            var perms = OverwritePermissions.AllowAll(newVoiceChannel);
            return perms.Modify(createInstantInvite: PermValue.Inherit, manageChannel: PermValue.Inherit, deafenMembers: PermValue.Inherit, muteMembers: PermValue.Inherit);
        }

        public async Task<bool> DoSpamChecks(IGuildUser guildUser, ServerProperties props, IGuild guild)
        {
            if (spamProtectionDictionary.ContainsKey(guildUser))
            {
                if (spamProtectionCountDictionary[guildUser] > 4)
                {
                    if (DateTimeOffset.Now.ToUnixTimeSeconds() - spamProtectionDictionary[guildUser] < 60)
                    {
                        var spamRole = guild.GetRole(props.SpamRoleId);
                        await guildUser.AddRoleAsync(spamRole).ConfigureAwait(false);
                        Logger.LogInformation("{name} spammed VC creation in {guildName}", $"{guildUser.Username}#{guildUser.Discriminator}", guild.Name);
                        var logChannel = await guild.GetTextChannelAsync(props.LogChannelId).ConfigureAwait(false);
                        await logChannel.SendMessageAsync($"{guildUser.Mention} spammed temporary VC creation, giving them the role {spamRole.Mention}. " +
                            "Remove the role and wait 60 seconds to let them create a voice channel again.").ConfigureAwait(false);
                        await guildUser.ModifyAsync(vcUser => vcUser.Channel = null).ConfigureAwait(false);
                        return false;
                    }
                    spamProtectionCountDictionary[guildUser] = 0;
                }
            }

            spamProtectionDictionary[guildUser] = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (!spamProtectionCountDictionary.ContainsKey(guildUser))
            {
                spamProtectionCountDictionary[guildUser] = 1;
            }
            else
            {
                spamProtectionCountDictionary[guildUser]++;
            }

            return true;
        }
        private async Task ChangeOwnerIfCreatorLeft(SocketVoiceState vc, ServerProperties serverProperties, SocketUser user, IGuildUser guildUser)
        {
            string name = !string.IsNullOrEmpty(guildUser.Nickname) ? guildUser.Nickname : user.Username;
            if (vc.VoiceChannel != null &&
                vc.VoiceChannel.Users.Count != 0 &&
                vc.VoiceChannel.CategoryId == serverProperties.TempVoiceCategoryId &&
                vc.VoiceChannel.Id != serverProperties.TempVoiceCreateChannelId &&
                vc.VoiceChannel.Name == (name + "'s Voice Chat"))
            {
                var newOwner = vc.VoiceChannel.Users.First();
                var newName = !string.IsNullOrEmpty(newOwner.Nickname) ? newOwner.Nickname : newOwner.Username;
                await vc.VoiceChannel.ModifyAsync(vc => vc.Name = newName + "'s Voice Chat").ConfigureAwait(false);
                // Fix owner permissions
                if (!serverProperties.SimpleTempVc)
                {
                    await vc.VoiceChannel.RemovePermissionOverwriteAsync(user).ConfigureAwait(false);
                    await vc.VoiceChannel.AddPermissionOverwriteAsync(newOwner, GetOwnerPermissions(vc.VoiceChannel)).ConfigureAwait(false);
                }
            }
        }

        private async Task RemoveOldVc(SocketVoiceState vc, ServerProperties serverProperties)
        {
            if (vc.VoiceChannel?.Users.Count == 0 &&
                vc.VoiceChannel.CategoryId == serverProperties.TempVoiceCategoryId &&
                vc.VoiceChannel.Id != serverProperties.TempVoiceCreateChannelId)
            {
                await vc.VoiceChannel.DeleteAsync().ConfigureAwait(false);
            }
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
        public async Task<bool> CanManageVc(IVoiceChannel vc, ulong userId)
        {
            var guildUser = await vc.Guild.GetUserAsync(userId).ConfigureAwait(false);
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

        /// <summary>
        /// Checks if a VC is NSFW
        /// </summary>
        /// <param name="vc"></param>
        /// <returns></returns>
        public bool IsVcNsfw(IVoiceChannel vc)
        {
            var props = SpService.GetProperties(vc.GuildId);
            if (props.NsfwRoleId == 0)
                return false;
            var isPrivate = !IsVcPublic(vc);
            var perms = vc.GetPermissionOverwrite(vc.Guild.GetRole(props.NsfwRoleId)) ?? OverwritePermissions.InheritAll;
            return isPrivate && (perms.ViewChannel == PermValue.Allow && perms.Connect == PermValue.Allow);
        }

        /// <summary>
        /// Makes the VC public or private
        /// </summary>
        /// <param name="vc">VC to make private</param>
        /// <param name="makePrivate">If the VC should be private</param>
        public async Task SetPrivate(IVoiceChannel vc, bool makePrivate)
        {
            var perms = vc.GetPermissionOverwrite(vc.Guild.EveryoneRole) ?? OverwritePermissions.InheritAll;
            var newPerm = makePrivate ? PermValue.Deny : PermValue.Allow;
            await vc.AddPermissionOverwriteAsync(vc.Guild.EveryoneRole, perms.Modify(viewChannel: newPerm, connect: newPerm)).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks if a user can join the VC
        /// </summary>
        /// <param name="vc">VC to check</param>
        /// <param name="user">User to check</param>
        /// <returns>If the user can currently join the VC</returns>
        public bool IsUserAllowed(IVoiceChannel vc, IGuildUser user)
        {
            var userPerms = user.GetPermissions(vc);
            return userPerms.Connect && userPerms.ViewChannel;
        }

        /// <summary>
        /// Sets whether a user is allowed to join a VC
        /// </summary>
        /// <param name="vc">VC to set permissions on</param>
        /// <param name="user">User to set permissions on</param>
        /// <param name="allowUser">If the user is allowed to join</param>
        public async Task SetUserAllowed(IVoiceChannel vc, IGuildUser user, bool allowUser)
        {
            var newPerm = allowUser ? PermValue.Allow : PermValue.Inherit;
            var userPerms = vc.GetPermissionOverwrite(user) ?? OverwritePermissions.InheritAll;
            await vc.AddPermissionOverwriteAsync(user,
                userPerms.Modify(viewChannel: newPerm, connect: newPerm)).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets if a VC should be NSFW or not
        /// </summary>
        /// <param name="vc">VC to modify</param>
        /// <param name="nsfw">If the VC should be nsfw</param>
        public async Task SetVcNsfw(IVoiceChannel vc, bool nsfw)
        {
            var props = SpService.GetProperties(vc.GuildId);
            var newPerm = nsfw ? PermValue.Allow : PermValue.Inherit;
            var nsfwRole = vc.Guild.GetRole(props.NsfwRoleId);
            var perms = vc.GetPermissionOverwrite(nsfwRole) ?? OverwritePermissions.InheritAll;
            await vc.AddPermissionOverwriteAsync(nsfwRole, perms.Modify(viewChannel: newPerm, connect: newPerm)).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks if a user is trusted to modify a VC
        /// </summary>
        /// <param name="vc">VC to check permissions on</param>
        /// <param name="user">User to check permissions on</param>
        /// <returns>If the user is trusted to modify the VC</returns>
        public bool IsUserTrusted(IVoiceChannel vc, IGuildUser user)
        {
            var targetPerms = vc.GetPermissionOverwrite(user) ?? OverwritePermissions.InheritAll;
            return targetPerms.ManageRoles == PermValue.Allow;
        }

        /// <summary>
        /// Sets if a user is trusted to modify a vc's permissions
        /// </summary>
        /// <param name="vc">VC to modify</param>
        /// <param name="user">User to trust/untrust</param>
        /// <param name="trustUser">If the user should be trusted</param>
        public async Task SetUserTrusted(IVoiceChannel vc, IGuildUser user, bool trustUser)
        {
            var newPerms = trustUser ? GetOwnerPermissions(vc) : OverwritePermissions.InheritAll;
            await vc.AddPermissionOverwriteAsync(user, newPerms).ConfigureAwait(false);
        }
    }
}
