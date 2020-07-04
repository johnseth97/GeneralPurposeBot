using Discord.Commands;
using GeneralPurposeBot.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [RequireOwner]
    [Name("Database"), Summary("Database maintainance commands.")]
    [Group("database")]
    public class DatabaseModule : ModuleBase<SocketCommandContext>
    {
        private BotDbContext DbContext { get; set; }
        public DatabaseModule(BotDbContext dbContext)
        {
            DbContext = dbContext;
        }

        [Command("migrate"), Summary("Run database migrations")]
        public async Task Migrate()
        {
            DbContext.Database.Migrate();
            await ReplyAsync("Done").ConfigureAwait(false);
        }

        [Command("version"), Summary("See the current version of the database")]
        public async Task Version()
        {
            var version = DbContext.Database.GetAppliedMigrations().AsEnumerable().Last();
            await ReplyAsync("Last migration was " + version).ConfigureAwait(false);
        }
    }
}
