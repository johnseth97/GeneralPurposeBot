using GeneralPurposeBot.Models;
using GeneralPurposeBot.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services
{
    public class GameItemService
    {
        public BotDbContext DbContext { get; set; }
        public Dictionary<string, ItemBase> Items { get; set; }
        public GameItemService(BotDbContext dbContext)
        {
            DbContext = dbContext;

            Items = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(ItemBase)))
                .Select(t => Activator.CreateInstance(t) as ItemBase)
                .ToDictionary(t => t.Name);
        }

        public int GetItemQuantity(ulong server, ulong user, string itemName)
        {
            var items = DbContext.UserItems
                .AsQueryable()
                .Where(ui => ui.ServerId == server &&
                    ui.UserId == user &&
                    ui.ItemName == itemName);
            if (!items.Any())
                return 0;
            return items.First().Quantity;
        }

        public Dictionary<string, int> GetItems(ulong server, ulong user)
        {
            return DbContext.UserItems
                .AsQueryable()
                .Where(ui => ui.ServerId == server &&
                    ui.UserId == user)
                .ToDictionary(ui => ui.ItemName, ui => ui.Quantity);
        }

        public int GiveItem(ulong server, ulong user, string itemName, int quantity = 1)
        {
            var items = DbContext.UserItems
                .AsQueryable()
                .Where(ui => ui.ServerId == server &&
                    ui.UserId == user &&
                    ui.ItemName == itemName);
            if (items.Any())
            {
                items.First().Quantity += quantity;
                DbContext.SaveChanges();
                return items.First().Quantity;
            }
            DbContext.UserItems.Add(new UserItem()
            {
                ItemName = itemName,
                Quantity = quantity,
                ServerId = server,
                UserId = user
            });
            DbContext.SaveChanges();
            return quantity;
        }

        public int TakeItem(ulong server, ulong user, string itemName, int quantity = 1)
        {
            var items = DbContext.UserItems
                .AsQueryable()
                .Where(ui => ui.ServerId == server &&
                    ui.UserId == user &&
                    ui.ItemName == itemName);
            if (!items.Any() || items.First().Quantity < quantity)
            {
                return items.Any() ? items.First().Quantity : 0;
            }
            items.First().Quantity -= quantity;
            DbContext.SaveChanges();
            return items.First().Quantity;
        }
    }
}
