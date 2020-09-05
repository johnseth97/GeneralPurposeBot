using System.Text;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using GeneralPurposeBot.Services;
using GeneralPurposeBot.Models;

namespace DiscordBot.Modules
{

    [Group("!role"), Summary("!role set <roleName> to set your role, !role remove to remove your role")]
    [RequireBotPermission(Discord.GuildPermission.ManageRoles, ErrorMessage = "The bot must be able to manage roles on the server to set user roles.", NotAGuildErrorMessage = "This must be run inside of a server.")]
    public class RoleRequestModule : ModuleBase
    {
        public RoleRequestService RoleRequestService { get; set; }
        public RoleRequestModule(RoleRequestService roleRequestService) { RoleRequestService = roleRequestService; }

        [Command("add"), Summary("!role add <roleName> to add the role to the list of self-assignable roles. Role must not have moderation-related permissions to work.")]
        [RequireUserPermission(GuildPermission.ManageRoles, ErrorMessage = "You must be able to manage roles to set a role as self-assignable.")]
        public async Task Add(string role)
        {
            var possibleRoles = Context.Guild.Roles.Where(r => r.Name == role);
            if (!possibleRoles.Any())
            {
                await Context.Channel.SendMessageAsync("Role does not exist!");
            }
            else
            {
                var searchRole = possibleRoles.First();
                if (RoleRequestService.GetRole(searchRole.Id) == null)
                {
                    if (!searchRole.Permissions.ManageRoles)
                    {
                        var newRole = new AssignableRole()
                        {
                            RoleId = searchRole.Id,
                            ServerId = Context.Guild.Id,
                            RoleName = searchRole.Name
                        };
                        RoleRequestService.AddRole(newRole);
                    }
                    else
                        await Context.Channel.SendMessageAsync("Moderated roles can't be self-assignable!");
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Role is already assignable!");
                }
            }
        }

        [Command("set")]
        public async Task Set(string role)
        {
            var possibleRoles = Context.Guild.Roles.Where(r => r.Name == role);
            if (!possibleRoles.Any())
            {
                await Context.Channel.SendMessageAsync("Role does not exist!");
            }
            else
            {
                var searchRole = possibleRoles.First();
                if (RoleRequestService.GetRole(searchRole.Id) != null)
                {
                    var user = await Context.Guild.GetUserAsync(Context.User.Id).ConfigureAwait(false);

                    await user.AddRoleAsync(searchRole).ConfigureAwait(false);
                    await Context.Channel.SendMessageAsync($"{user.Mention} now has the **{role}** role.").ConfigureAwait(false);
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Role is not assignable!");
                }
            }

        }

        [Command("remove"), Summary("!remove <roleName> to remove the role.")]
        public async Task Remove(string role)
        {
            var possibleRoles = Context.Guild.Roles.Where(r => r.Name == role);
            if (!possibleRoles.Any())
            {
                await Context.Channel.SendMessageAsync("Role does not exist!");
            }
            else
            {
                var user = await Context.Guild.GetUserAsync(Context.User.Id).ConfigureAwait(false);

                var searchRole = possibleRoles.First();
                if (user.Guild.GetRole(searchRole.Id) != null)
                {
                    await user.RemoveRoleAsync(searchRole).ConfigureAwait(false);
                    await Context.Channel.SendMessageAsync($"role **{role}** removed from {user.Mention}.").ConfigureAwait(false);
                }
                else
                {
                    await Context.Channel.SendMessageAsync("You do not have this role!");
                }
            }
        }

        [Command("list"), Summary("Lists all currently assignable roles")]
        public async Task List()
        {
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(3, 244, 252));

            var sb = new StringBuilder();

            embed.Title = "Currently assignable roles";

            foreach (var roles in Context.Guild.Roles)
            {
                if (RoleRequestService.GetRole(roles.Id) != null)
                {
                    sb.AppendLine(roles.Name);
                }
            }

            embed.Description = sb.ToString();
            await ReplyAsync(null, false, embed.Build());
        }
    }
}
