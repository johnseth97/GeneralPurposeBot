using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("Color"), Summary("Manages color roles")]
    [Remarks("!color set <colorName> or !color set <hexCode>. !color remove to remove your color role.")]
    [RequireBotPermission(Discord.GuildPermission.ManageRoles, ErrorMessage = "The bot must be able to manage roles on the server to set user colors.", NotAGuildErrorMessage = "This must be run inside of a server.")]
    public class ColorRolesModule : ModuleBase
    {
        [Command("set")]
        public async Task Set(string color)
        {
            var user = await Context.Guild.GetUserAsync(Context.User.Id).ConfigureAwait(false);
            await Set(user, color).ConfigureAwait(false);
        }

        [Command("set")]
        [RequireUserPermission(GuildPermission.ManageRoles, ErrorMessage = "You must be able to manage roles to set another user's color.")]
        public async Task Set(IGuildUser user, string colorStr)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (colorStr is null)
            {
                throw new ArgumentNullException(nameof(colorStr));
            }

            // Try and convert the color to a .NET color to pass to Discord later.
            // This also serves as validation that the color string is valid since this _should_ throw an exception if it isn't.
            System.Drawing.Color color;
            try
            {
                color = System.Drawing.ColorTranslator.FromHtml(colorStr);
            }
            catch (Exception)
            {
                await ReplyAsync("Color was not known!").ConfigureAwait(false);
                return;
            }

            colorStr = ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.ToArgb())); // Done to standardize the role name
            var discordColor = new Discord.Color(color.R, color.G, color.B); // Because Discord.Net has it's own color format, ig
            var roleName = "color-" + colorStr;
            var possibleRoles = Context.Guild.Roles
                .Where(role => role.Name == roleName && role.Color == discordColor); // Check if the server has roles already
            IRole role;
            if (possibleRoles.Any()) // if it does..
            {
                role = possibleRoles.First(); // grab that role
            }
            else // create if if not
            {
                role = await Context.Guild.CreateRoleAsync(roleName, GuildPermissions.None, discordColor, false, null).ConfigureAwait(false);
                await role.ModifyAsync(r => r.Position = 2).ConfigureAwait(false);
            }

            // remove any existing color roles from the user
            var roles = user.RoleIds
                .Select(id => Context.Guild.GetRole(id)) // Convert each role ID the user has into a role object
                .Where(role => role.Name.StartsWith("color-")); // Find the ones starting with "color"
            await user.RemoveRolesAsync(roles).ConfigureAwait(false);
            // add new color role to user
            await user.AddRoleAsync(role).ConfigureAwait(false);
            await ReplyAsync($"{user.Mention} now has the **{roleName}** role.").ConfigureAwait(false);
        }

        [Command("remove")]
        public async Task Remove()
        {
            var user = await Context.Guild.GetUserAsync(Context.User.Id).ConfigureAwait(false);
            await Remove(user).ConfigureAwait(false);
        }

        [Command("remove")]
        [RequireUserPermission(GuildPermission.ManageRoles, ErrorMessage = "You must be able to manage roles to set another user's color.")]
        public async Task Remove(IGuildUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            // remove any existing color roles from the user
            var roles = user.RoleIds
                .Select(id => Context.Guild.GetRole(id)) // Convert each role ID the user has into a role object
                .Where(role => role.Name.StartsWith("color-")); // Find the ones starting with "color"
            await user.RemoveRolesAsync(roles).ConfigureAwait(false);
            await ReplyAsync($"Removed all color roles from {user.Mention}").ConfigureAwait(false);
        }
    }
}
