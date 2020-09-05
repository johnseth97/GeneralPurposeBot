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
    public class RoleRequestService
    {
        private BotDbContext DbContext { get; set; }

        public RoleRequestService(BotDbContext dbContext, IConfiguration config)
        {
            DbContext = dbContext;
        }
        public AssignableRole GetRole(ulong roleID)
        {
            var results = DbContext.AssignableRoles.AsEnumerable().Where(sp => sp.RoleId == roleID);
            return results.Any() ? results.First() : null;
        }
        public AssignableRole AddRole(AssignableRole entity)
        {
            entity = DbContext.AssignableRoles.Add(entity).Entity;
            DbContext.SaveChanges();
            return entity;
        }

    }
}
