using Discord.Commands;
using GeneralPurposeBot.Services;
using GeneralPurposeBot.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [RequireContext(ContextType.Guild, ErrorMessage = "This can only be run in a guild!")]
    public abstract class GameModuleBase : ModuleBase
    {
        protected decimal Money {
            get => GameMoneyService.GetMoney(Context.Guild.Id, Context.User.Id);
            set => GameMoneyService.SetMoney(Context.Guild.Id, Context.User.Id, value);
        }

        protected string MoneyString => Money.FormatMoney();
        public GameMoneyService GameMoneyService { get; }
        public GameItemService GameItemService { get; }

        protected Dictionary<string, int> UserItems => GameItemService.GetItems(Context.Guild.Id, Context.User.Id);
        protected Dictionary<string, ItemBase> AllItems => GameItemService.Items;
        protected GameModuleBase(GameMoneyService gameMoneyService, GameItemService gameItemService)
        {
            GameMoneyService = gameMoneyService;
            GameItemService = gameItemService;
        }

        public ItemBase FindItem(string search)
        {
            if (GameItemService.Items.ContainsKey(search))
                return GameItemService.Items[search];
            var items = GameItemService.Items.Values
                .Where(i => string.Equals(i.Name, search, StringComparison.InvariantCultureIgnoreCase));
            return items.FirstOrDefault();
        }

        public int GiveItem(string itemName, int quantity = 1)
        {
            return GameItemService.GiveItem(Context.Guild.Id, Context.User.Id, itemName, quantity);
        }
        public int TakeItem(string itemName, int quantity = 1)
        {
            return GameItemService.TakeItem(Context.Guild.Id, Context.User.Id, itemName, quantity);
        }
        public int ItemQuantity(string itemName)
        {
            return GameItemService.GetItemQuantity(Context.Guild.Id, Context.User.Id, itemName);
        }

        public bool HasItem(string itemName, int quantity = 1)
        {
            return ItemQuantity(itemName) >= quantity;
        }
    }
}
