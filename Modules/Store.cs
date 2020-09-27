using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [Group("store"), Summary("Use your money to buy items, or sell items for more money.")]
    public class Store : GameModuleBase
    {
        protected Store(Services.GameMoneyService gameMoneyService, Services.GameItemService gameItemService) : base(gameMoneyService, gameItemService)
        {
        }

        [Command, Summary("See what items the store has for sale")]
        [Alias("list")]
        public async Task List()
        {
            var output = "__**Items in the Store:**__\n";
            GameItemService.Items
                .Where(item => item.Value.InStore)
                .ToList()
                .ForEach(item => output += item.Value.GetStoreItemString() + "\n");
            await ReplyAsync(output).ConfigureAwait(false);
        }

        [Command("buy"), Summary("Buy an item from the store")]
        public async Task Buy(string itemName, int quantity = 1)
        {
            var item = Transaction.FindItem(itemName);
            if (item == null)
            {
                await ReplyAsync("This item does not exist!").ConfigureAwait(false);
                return;
            }
            if (quantity <= 0)
            {
                await ReplyAsync($"Huh, do you really expect me to sell **{quantity} {item.GetName(quantity)}** to you?").ConfigureAwait(false);
                return;
            }
            if (!item.StoreBuyable)
            {
                await ReplyAsync("This item is not buyable!").ConfigureAwait(false);
                return;
            }
            var totalPrice = quantity * item.StoreBuyPrice;
            if (Money < totalPrice)
            {
                await ReplyAsync("You do not have enough money for this item!").ConfigureAwait(false);
                return;
            }
            Transaction.TakeMoney(totalPrice);
            Transaction.GiveItems(item.Name, quantity);
            var newQuantity = Transaction.GetItemQuantity(item.Name);
            Transaction.Message = $"You now have **{newQuantity} {item.GetName(newQuantity)}**! After this transaction, your balance is **${MoneyString}**.";
        }

        [Command("sell"), Summary("Sell an item to the store")]
        public async Task Sell(string itemName, int quantity = 1)
        {
            var item = Transaction.FindItem(itemName);
            if (item == null)
            {
                await ReplyAsync("This item does not exist!").ConfigureAwait(false);
                return;
            }
            if (quantity <= 0)
            {
                await ReplyAsync($"Huh, do you really expect me to buy **{quantity} {item.GetName(quantity)}** from you?").ConfigureAwait(false);
                return;
            }
            if (!item.StoreSellable)
            {
                await ReplyAsync("This item is not sellable!").ConfigureAwait(false);
                return;
            }
            var totalPrice = quantity * item.StoreSellPrice;
            if (!Transaction.HasItem(item.Name, quantity))
            {
                await ReplyAsync("You do not have enough of this item!").ConfigureAwait(false);
                return;
            }
            Transaction.GiveMoney(totalPrice);
            Transaction.TakeItems(item.Name, quantity);
            var newQuantity = Transaction.GetItemQuantity(item.Name);
            Transaction.Message = $"You now have **{newQuantity} {item.GetName(newQuantity)}**! After this transaction, your balance is **${MoneyString}**.";
        }
    }
}
