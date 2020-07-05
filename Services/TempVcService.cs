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

                var perms = GetOwnerPermissions(newVoiceChannel);
                await newVoiceChannel.AddPermissionOverwriteAsync(user, perms).ConfigureAwait(false);
                await guildUser.ModifyAsync(vcUser => vcUser.Channel = newVoiceChannel).ConfigureAwait(false);
            }
            await RemoveOldVc(before, props).ConfigureAwait(false);
            await ChangeOwnerIfCreatorLeft(before, props, user, guildUser).ConfigureAwait(false);
        }

        private static OverwritePermissions GetOwnerPermissions(IVoiceChannel newVoiceChannel)
        {
            var perms = OverwritePermissions.AllowAll(newVoiceChannel);
            return perms.Modify(createInstantInvite: PermValue.Inherit, manageChannel: PermValue.Inherit);
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
                await vc.VoiceChannel.RemovePermissionOverwriteAsync(user).ConfigureAwait(false);
                await vc.VoiceChannel.AddPermissionOverwriteAsync(newOwner, GetOwnerPermissions(vc.VoiceChannel)).ConfigureAwait(false);
            }
        }

        private async Task RemoveOldVc(SocketVoiceState vc, ServerProperties serverProperties)
        {
            if (vc.VoiceChannel != null &&
                vc.VoiceChannel.Users.Count == 0 &&
                vc.VoiceChannel.CategoryId == serverProperties.TempVoiceCategoryId &&
                vc.VoiceChannel.Id != serverProperties.TempVoiceCreateChannelId)
            {
                await vc.VoiceChannel.DeleteAsync().ConfigureAwait(false);
            }
        }
    }
}
