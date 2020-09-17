﻿using GeneralPurposeBot.Models;
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
            if (UserHasWallet(server, user))
                return;
            DbContext.UserMoney.Add(new UserMoney()
            {
                UserId = user,
                ServerId = server,
                Money = 100
            });
            DbContext.SaveChanges();
        }

        public decimal GetMoney(ulong server, ulong user)
        {
            SetDefaultIfNeeded(server, user);
            return DbContext.UserMoney.Find(server, user).Money;
        }

        public bool UserHasWallet(ulong server, ulong user)
        {
            return DbContext.UserMoney.Any(um => um.ServerId == server && um.UserId == user);
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
        public void AddMoney(ulong server, ulong user, decimal amount)
        {
            SetMoney(server, user, GetMoney(server, user) + amount);
        }
        public void RemoveMoney(ulong server, ulong user, decimal amount)
        {
            SetMoney(server, user, GetMoney(server, user) - amount);
        }

        public IOrderedQueryable<UserMoney> GetAllInServer(ulong server)
        {
            return DbContext.UserMoney
                .AsQueryable()
                .Where(um => um.ServerId == server)
                .OrderByDescending(um => um.Money);
        }
    }
}