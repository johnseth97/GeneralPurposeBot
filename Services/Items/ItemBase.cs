using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.Items
{
    public abstract class ItemBase
    {
        /// <summary>
        /// Used in inventory lists
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Used in places like "you have 1 piece of dust"
        /// </summary>
        public virtual string SingularName { get => Name; }

        /// <summary>
        /// Used in places like "you have 5 pieces of dust"
        /// </summary>
        public virtual string PluralName { get => SingularName + "s"; }
        public abstract string Description { get; }
        public bool InStore { get => StoreBuyable || StoreSellable; }
        public abstract bool StoreBuyable { get; }
        public abstract decimal StoreBuyPrice { get; }
        public virtual bool StoreSellable { get => StoreBuyable; }
        public virtual decimal StoreSellPrice { get => StoreBuyPrice; }
        public virtual async Task UseAsync(ICommandContext context, GameMoneyService gameMoneyService, GameItemService gameItemService) 
        {
            await context.Channel.SendMessageAsync("Huh, this item can't be used...").ConfigureAwait(false);
        }

        public string GetStoreItemString()
        {
            // $"**{item.Key}** (Buy: ${item.Value.StoreBuyPrice}/Sell: ${item.Value.StoreSellPrice})
            var result = "**" + Name + "** (";
            var buySellStrings = new List<string>();
            if (StoreBuyable)
                buySellStrings.Add("Buy: $" + StoreBuyPrice);
            if (StoreSellable)
                buySellStrings.Add("Sell: $" + StoreSellPrice);
            result += string.Join(" | ", buySellStrings);
            result += "): ";
            result += Description;
            return result;
        }
    }
}
