using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services
{
    public class GameMoneyService
    {
        public BotDbContext DbContext { get; set; }
        public GameMoneyService(BotDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public void SetDefaultIfNeeded(ulong server, ulong user)
        {
            if (DbContext.UserMoney.Any(um => um.ServerId == server && um.UserId == user))
                return;
            DbContext.UserMoney.Add(new UserMoney()
            {
                UserId = user,
                ServerId = server,
                Money = 100
            });
        }

        public decimal GetMoney(ulong server, ulong user)
        {
            SetDefaultIfNeeded(server, user);
            return DbContext.UserMoney.Find(server, user).Money;
        }
        public void SetMoney(ulong server, ulong user, decimal amount)
        {
            DbContext.UserMoney.Find(server, user).Money = amount;
            DbContext.SaveChanges();
        }
        public async Task SetMoneyAsync(ulong server, ulong user, decimal amount)
        {
            DbContext.UserMoney.Find(server, user).Money = amount;
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
