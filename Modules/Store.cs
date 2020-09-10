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
            var item = FindItem(itemName);
            if (item == null)
            {
                await ReplyAsync("This item does not exist!").ConfigureAwait(false);
                return;
            }
            var totalPrice = quantity * item.StoreBuyPrice;
            if (Money < totalPrice)
            {
                await ReplyAsync("You do not have enough money for this item!").ConfigureAwait(false);
                return;
            }
            Money -= totalPrice;
            var newQuantity = GiveItem(itemName, quantity);
            await ReplyAsync($"You now have **{newQuantity} {(newQuantity == 1 ? item.SingularName : item.PluralName)}**! After this transaction, your balance is **${Money}**.").ConfigureAwait(false);
        }
    }
}
