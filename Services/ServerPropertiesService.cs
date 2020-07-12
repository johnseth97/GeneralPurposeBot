using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
