using Discord.Commands;
using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services
{
    public class ServerPropertiesService
    {
        private BotDbContext DbContext { get; }
        public ServerPropertiesService(BotDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ServerProperties GetProperties(ulong serverId)
        {
            ServerProperties entity;
            var results = DbContext.ServerProperties.AsEnumerable().Where(sp => sp.ServerId == serverId);
            if (!results.Any())
            {
                entity = new ServerProperties()
                {
                    ServerId = serverId
                };
                entity = DbContext.ServerProperties.Add(entity).Entity;
                DbContext.SaveChanges();
                return entity;
            }
            return results.First();
        }
        public ServerProperties UpdateProperties(ServerProperties entity)
        {
            entity = DbContext.ServerProperties.Update(entity).Entity;
            DbContext.SaveChanges();
            return entity;
        }

        public bool IsModuleEnabled(ModuleInfo module, ulong id)
        {
            var matches = DbContext.ServerModules
                .AsQueryable()
                .Where(sm => sm.ServerId == id && sm.Name == module.GetFullName());
            if (matches.Any())
                return !matches.First().Disabled;
            return true;
        }

        public async Task EnableModule(ModuleInfo module, ulong id)
        {
            var matches = DbContext.ServerModules
                .AsQueryable()
                .Where(sm => sm.ServerId == id && sm.Name == module.GetFullName());
            if (matches.Any())
            {
                var match = matches.First();
                match.Disabled = false;
            }
            else
            {
                var match = new ServerModule()
                {
                    ServerId = id,
                    Name = module.GetFullName(),
                    Disabled = false
                };
                DbContext.ServerModules.Add(match);
            }
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task DisableModule(ModuleInfo module, ulong id)
        {
            var matches = DbContext.ServerModules
                .AsQueryable()
                .Where(sm => sm.ServerId == id && sm.Name == module.GetFullName());
            if (matches.Any())
            {
                var match = matches.First();
                match.Disabled = true;
            }
            else
            {
                var match = new ServerModule()
                {
                    ServerId = id,
                    Name = module.GetFullName(),
                    Disabled = true
                };
                DbContext.ServerModules.Add(match);
            }
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
