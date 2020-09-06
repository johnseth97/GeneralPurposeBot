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
    [Group("role"), Summary("!role set <roleName> to set your role, !role remove to remove your role")]
    [RequireBotPermission(Discord.GuildPermission.ManageRoles, ErrorMessage = "The bot must be able to manage roles on the server to set user roles.", NotAGuildErrorMessage = "This must be run inside of a server.")]
    public class RoleRequestModule : ModuleBase
    {
        public RoleRequestService RoleRequestService { get; set; }
        public RoleRequestModule(RoleRequestService roleRequestService)
        {
            RoleRequestService = roleRequestService;
        }

        public IRole SearchRole(string name)
        {
            var possibleRoles = Context.Guild.Roles.Where(r => string.Equals(r.Name, name, StringComparison.InvariantCultureIgnoreCase));
            return possibleRoles.FirstOrDefault();
        }

        [Command("add"), Summary("!role add <roleName> to add the role to the list of self-assignable roles. Role must not have moderation-related permissions to work.")]
        [RequireUserPermission(GuildPermission.ManageRoles, ErrorMessage = "You must be able to manage roles to set a role as self-assignable.")]
        public async Task Add([Remainder] string role)
        {
            var roleResult = SearchRole(role);
            if (roleResult == null)
            {
                await ReplyAsync("Role does not exist!").ConfigureAwait(false);
            }
            else
            {
                if (RoleRequestService.GetRole(roleResult.Id) == null)
                {
                    if (Context.Guild.OwnerId != Context.User.Id)
                    {
                        // find highest role of the user
                        var user = await Context.Guild.GetUserAsync(Context.User.Id).ConfigureAwait(false);
                        var highestRolePosition = 0;
                        user.RoleIds
                            .Select(roleId => Context.Guild.Roles.First(r => r.Id == roleId))
                            .ToList()
                            .ForEach(role => highestRolePosition = role.Position > highestRolePosition ? role.Position : highestRolePosition);
                        if (highestRolePosition <= roleResult.Position)
                        {
                            await ReplyAsync("You cannot add a role that is a level greater than or equal to your highest role!").ConfigureAwait(false);
                            return;
                        }
                    }
                    var warnings = "";
                    warnings += roleResult.Permissions.ManageRoles ? "\n**Warning:** This role has permissions to assign roles to other users!" : "";
                    warnings += roleResult.Permissions.Administrator ? "\n**Warning:** This role has administrator permissions!" : "";
                    var newRole = new AssignableRole()
                    {
                        RoleId = roleResult.Id,
                        ServerId = Context.Guild.Id,
                        RoleName = roleResult.Name
                    };
                    RoleRequestService.AddRole(newRole);
                    await ReplyAsync($"{roleResult} is now self-assignable! {warnings}").ConfigureAwait(false);
                }
                else
                {
                    await ReplyAsync("Role is already assignable!").ConfigureAwait(false);
                }
            }
        }

        [Command("remove"), Summary("Makes a role no longer self-assignable")]
        [RequireUserPermission(GuildPermission.ManageRoles, ErrorMessage = "You must be able to manage roles to set a role as no longer self-assignable.")]
        public async Task Remove([Remainder] string role)
        {
            var roleResult = SearchRole(role);
            if (roleResult == null)
            {
                await ReplyAsync("Role does not exist!").ConfigureAwait(false);
            }
            else
            {
                var assignableRole = RoleRequestService.GetRole(roleResult.Id);
                if (assignableRole != null)
                {
                    if (Context.Guild.OwnerId != Context.User.Id)
                    {
                        // find highest role of the user
                        var user = await Context.Guild.GetUserAsync(Context.User.Id).ConfigureAwait(false);
                        var highestRolePosition = 0;
                        user.RoleIds
                            .Select(roleId => Context.Guild.Roles.First(r => r.Id == roleId))
                            .ToList()
                            .ForEach(role => highestRolePosition = role.Position > highestRolePosition ? role.Position : highestRolePosition);
                        if (highestRolePosition <= roleResult.Position)
                        {
                            await ReplyAsync("You cannot remove a role that is a level greater than or equal to your highest role!").ConfigureAwait(false);
                            return;
                        }
                    }
                    RoleRequestService.RemoveRole(assignableRole);
                    await ReplyAsync($"{roleResult} is now no longer self-assignable!").ConfigureAwait(false);
                }
                else
                {
                    await ReplyAsync("Role is not self-assignable!").ConfigureAwait(false);
                }
            }
        }

        [Command("give"), Summary("Adds a self-assignable role to you")]
        public async Task Give([Remainder] string role)
        {
            var roleResult = SearchRole(role);
            if (roleResult == null)
            {
                await ReplyAsync("Role does not exist!").ConfigureAwait(false);
            }
            else
            {
                if (RoleRequestService.GetRole(roleResult.Id) != null)
                {
                    var user = await Context.Guild.GetUserAsync(Context.User.Id).ConfigureAwait(false);

                    await user.AddRoleAsync(roleResult).ConfigureAwait(false);
                    await ReplyAsync($"{user.Mention} now has the **{roleResult.Name}** role.").ConfigureAwait(false);
                }
                else
                {
                    await ReplyAsync("Role is not assignable!").ConfigureAwait(false);
                }
            }
        }

        [Command("take"), Summary("Takes a self-assignable role away from you")]
        public async Task Take([Remainder] string role)
        {
            var roleResult = SearchRole(role);
            if (roleResult == null)
            {
                await ReplyAsync("Role does not exist!").ConfigureAwait(false);
            }
            else
            {
                var user = await Context.Guild.GetUserAsync(Context.User.Id).ConfigureAwait(false);
                if (user.Guild.GetRole(roleResult.Id) != null)
                {
                    if (RoleRequestService.GetRole(roleResult.Id) != null)
                    {
                        await user.RemoveRoleAsync(roleResult).ConfigureAwait(false);
                        await ReplyAsync($"role **{roleResult.Name}** removed from {user.Mention}.").ConfigureAwait(false);
                    }
                    else
                    {
                        await ReplyAsync("Role is not assignable!").ConfigureAwait(false);
                    }
                }
                else
                {
                    await ReplyAsync("You do not have this role!").ConfigureAwait(false);
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
            await ReplyAsync(null, false, embed.Build()).ConfigureAwait(false);
        }
    }
}
