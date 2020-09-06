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
        private BotDbContext DbContext { get; }

        public RoleRequestService(BotDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public AssignableRole GetRole(ulong roleID)
        {
            var results = DbContext.AssignableRoles.AsEnumerable().Where(sp => sp.RoleId == roleID);
            return results.FirstOrDefault();
        }

        public AssignableRole AddRole(AssignableRole entity)
        {
            entity = DbContext.AssignableRoles.Add(entity).Entity;
            DbContext.SaveChanges();
            return entity;
        }

        public void RemoveRole(AssignableRole entity)
        {
            DbContext.Remove(entity);
        }
    }
}
