using Discord.Commands;
using GeneralPurposeBot.Models;
using GeneralPurposeBot.Services;
using GeneralPurposeBot.Services.GameItems;
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
            get => Transaction.GetMoney();
        }

        protected string MoneyString => Money.FormatMoney();
        public GameMoneyService GameMoneyService { get; }
        public GameItemService GameItemService { get; }

        protected Dictionary<string, ItemBase> AllItems => GameItemService.Items;
        protected GameTransaction Transaction { get; set; }
        protected GameModuleBase(GameMoneyService gameMoneyService, GameItemService gameItemService)
        {
            GameMoneyService = gameMoneyService;
            GameItemService = gameItemService;
        }

        public void PreExec()
        {
            Transaction = new GameTransaction(GameItemService, GameMoneyService, Context.Guild, Context.User);
        }

        public async Task PostExec()
        {
            var message = Transaction.FinalizeTransaction();
            if (string.IsNullOrWhiteSpace(message)) return;
            await ReplyAsync(message).ConfigureAwait(false);
            Transaction = null;
        }
    }
}
