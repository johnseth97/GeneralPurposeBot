using Discord.Commands;
using GeneralPurposeBot.Models;
using Microsoft.Extensions.Configuration;
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
        public IConfiguration Config { get; }

        public ServerPropertiesService(BotDbContext dbContext, IConfiguration config)
        {
            DbContext = dbContext;
            Config = config;
        }
        public Dictionary<string, string> DisableableServices { get; } = new Dictionary<string, string>();
        public ServerProperties GetProperties(ulong serverId)
        {
            ServerProperties entity;
            var results = DbContext.ServerProperties.AsEnumerable().Where(sp => sp.ServerId == serverId);
            if (!results.Any())
            {
                entity = new ServerProperties()
                {
                    ServerId = serverId,
                    Prefix = Config["prefix"]
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
            => IsModuleEnabled(module.GetFullName(), id);
        public bool IsModuleEnabled(string moduleName, ulong id)
        {
            var matches = DbContext.ServerModules
                .AsQueryable()
                .Where(sm => sm.ServerId == id && sm.Name == moduleName);
            if (matches.Any())
                return !matches.First().Disabled;
            return true;
        }

        public async Task EnableModule(ModuleInfo module, ulong id)
            => await EnableModule(module.GetFullName(), id).ConfigureAwait(false);
        public async Task EnableModule(string moduleName, ulong id)
        {
            var matches = DbContext.ServerModules
                .AsQueryable()
                .Where(sm => sm.ServerId == id && sm.Name == moduleName);
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
                    Name = moduleName,
                    Disabled = false
                };
                DbContext.ServerModules.Add(match);
            }
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DisableModule(ModuleInfo module, ulong id)
            => await DisableModule(module.GetFullName(), id).ConfigureAwait(false);

        public async Task DisableModule(string moduleName, ulong id)
        {
            var matches = DbContext.ServerModules
                .AsQueryable()
                .Where(sm => sm.ServerId == id && sm.Name == moduleName);
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
                    Name = moduleName,
                    Disabled = true
                };
                DbContext.ServerModules.Add(match);
            }
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
