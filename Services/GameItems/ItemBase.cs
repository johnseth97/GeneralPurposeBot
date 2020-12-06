using Discord.Commands;
using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
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
        public virtual Task UseAsync(GameTransaction transaction)
        {
            transaction.Message = "Huh, this item can't be used";
            return Task.CompletedTask;
        }

        public string GetStoreItemString()
        {
            var result = "**" + Name + "** (";
            var buySellStrings = new List<string>();
            if (StoreBuyable)
                buySellStrings.Add("Buy: $" + StoreBuyPrice.FormatMoney());
            if (StoreSellable)
                buySellStrings.Add("Sell: $" + StoreSellPrice.FormatMoney());
            result += string.Join(" | ", buySellStrings);
            result += "): ";
            result += Description;
            return result;
        }

        public string GetName(int quantity = 0)
        {
            if (quantity == 1) return SingularName;
            return PluralName;
        }
    }
}
