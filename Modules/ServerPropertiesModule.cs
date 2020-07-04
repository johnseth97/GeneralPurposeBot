using Discord;
using Discord.Commands;
using GeneralPurposeBot.Services;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [Name("ServerProperties"), Summary("Various per-server settings for the bot")]
    [Group("ServerProperties")]
    [RequireUserPermission(Discord.GuildPermission.Administrator, ErrorMessage = "You must have the Server Administrator permission to use this command")]
    public class ServerPropertiesModule : ModuleBase
    {
        private ServerPropertiesService SpService { get; }
        public ServerPropertiesModule(ServerPropertiesService spService)
        {
            SpService = spService;
        }

        [Command, Summary("Lists the server's current properties")]
        public async Task PropertySummary()
        {
            var guild = Context.Guild;
            var props = SpService.GetProperties(Context.Guild.Id);
            var eb = new EmbedBuilder()
                .WithTitle("Server properties for " + Context.Guild.Name)
                .WithColor(0, 255, 0);
            var logChannel = await guild.GetTextChannelAsync(props.LogChannelId).ConfigureAwait(false);
            var spamRole = guild.GetRole(props.SpamRoleId);
            var nsfwRole = guild.GetRole(props.NsfwRoleId);
            var possibleTempCategory = (await guild.GetCategoriesAsync().ConfigureAwait(false)).Where(cat => cat.Id == props.TempVoiceCategoryId);
            ICategoryChannel tempCategory = null;
            if (possibleTempCategory.Any())
            {
                tempCategory = possibleTempCategory.First();
            }
            var tempChannel = await guild.GetVoiceChannelAsync(props.TempVoiceCreateChannelId).ConfigureAwait(false);

            eb.AddField("Log Channel", logChannel?.Mention ?? "None");
            eb.AddField("Spam Role", spamRole?.Name ?? "None");
            eb.AddField("NSFW Role", nsfwRole?.Name ?? "None");
            eb.AddField("Temporary Voice Channel Category", tempCategory?.Name ?? "None");
            eb.AddField("Temporary Voice Creation Channel", tempChannel?.Name ?? "None");
            await ReplyAsync("", false, eb.Build()).ConfigureAwait(false);
        }

        [Command("LogChannel"), Summary("Gets or sets the channel used for logging.")]
        public async Task LogChannel(ITextChannel channel = null)
        {
            var props = SpService.GetProperties(Context.Guild.Id);
            if (channel == null)
            {
                var logChannel = await Context.Guild.GetTextChannelAsync(props.LogChannelId).ConfigureAwait(false);
                if (logChannel == null)
                {
                    await ReplyAsync("There is no log channel set.").ConfigureAwait(false);
                    return;
                }
                await ReplyAsync("The log channel is currently " + logChannel.Mention).ConfigureAwait(false);
                return;
            }
            props.LogChannelId = channel.Id;
            SpService.UpdateProperties(props);
            await ReplyAsync("Set the log channel to " + channel.Mention).ConfigureAwait(false);
        }

        [Command("SpamRole"), Summary("Gets or sets the spam role used for people who spam VC creation")]
        public async Task SpamRole(IRole role = null)
        {
            var props = SpService.GetProperties(Context.Guild.Id);
            if (role == null)
            {
                var spamRole = Context.Guild.GetRole(props.SpamRoleId);
                if (spamRole == null)
                {
                    await ReplyAsync("There is no spam role set.").ConfigureAwait(false);
                    return;
                }
                await ReplyAsync("The spam role is currently " + spamRole.Name).ConfigureAwait(false);
                return;
            }
            props.SpamRoleId = role.Id;
            SpService.UpdateProperties(props);
            await ReplyAsync("Set the spam role to " + role.Name).ConfigureAwait(false);
        }

        [Command("NsfwRole"), Summary("Gets or sets the NSFW role used for marking a VC as NSFW")]
        public async Task NsfwRole(IRole role = null)
        {
            var props = SpService.GetProperties(Context.Guild.Id);
            if (role == null)
            {
                var spamRole = Context.Guild.GetRole(props.SpamRoleId);
                if (spamRole == null)
                {
                    await ReplyAsync("There is no NSFW role set.").ConfigureAwait(false);
                    return;
                }
                await ReplyAsync("The NSFW role is currently " + spamRole.Name).ConfigureAwait(false);
                return;
            }
            props.NsfwRoleId = role.Id;
            SpService.UpdateProperties(props);
            await ReplyAsync("Set the NSFW role to " + role.Name).ConfigureAwait(false);
        }

        [Command("TempVoiceCategory"), Summary("Gets or sets the temporary voice channel category")]
        public async Task TempVoiceCategory(ICategoryChannel category = null)
        {
            var props = SpService.GetProperties(Context.Guild.Id);
            if (category == null)
            {
                var possibleTempCategory = (await Context.Guild.GetCategoriesAsync().ConfigureAwait(false)).Where(cat => cat.Id == props.TempVoiceCategoryId);
                ICategoryChannel tempCategory = null;
                if (possibleTempCategory.Any())
                {
                    tempCategory = possibleTempCategory.First();
                }
                if (tempCategory == null)
                {
                    await ReplyAsync("There is no temporary voice channel category set.").ConfigureAwait(false);
                    return;
                }
                await ReplyAsync("The temporary voice channel category is " + tempCategory.Name).ConfigureAwait(false);
            }
            props.TempVoiceCategoryId = category.Id;
            SpService.UpdateProperties(props);
            await ReplyAsync("Set the temporary voice chat category to " + category.Name).ConfigureAwait(false);
        }

        [Command("TempVoiceChannel"), Summary("Gets or sets the temporary voice creation channel")]
        public async Task TempVoiceChannel(IVoiceChannel channel = null)
        {
            var props = SpService.GetProperties(Context.Guild.Id);
            if (channel == null)
            {
                var voiceChannel = await Context.Guild.GetVoiceChannelAsync(props.TempVoiceCreateChannelId).ConfigureAwait(false);
                if (voiceChannel == null)
                {
                    await ReplyAsync("There is no temporary voice creation channel set.").ConfigureAwait(false);
                    return;
                }
                await ReplyAsync("The temporary voice creation channel is currently " + voiceChannel.Name).ConfigureAwait(false);
                return;
            }
            props.TempVoiceCreateChannelId = channel.Id;
            SpService.UpdateProperties(props);
            await ReplyAsync("Set the temporary voice creation channel to " + channel.Name).ConfigureAwait(false);
        }
    }
}
