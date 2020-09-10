using Discord.Commands;
using GeneralPurposeBot.Services;
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

        public GameMoneyService GameMoneyService { get; }
        protected GameModuleBase(GameMoneyService gameMoneyService)
        {
            GameMoneyService = gameMoneyService;
        }
    }
}
