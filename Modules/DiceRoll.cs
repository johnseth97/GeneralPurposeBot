using Discord;
using Discord.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [Summary("Rolls dice")]
    public class DiceRoll : ModuleBase
    {
        [Command("dice"), Summary("Rolls dice")]
        public async Task Dice()
        {
            var random = new Random();
            var number = random.Next(1, 7);
            await Context.Channel.SendMessageAsync("Your 6 sided 🎲 rolled a " + number + "!").ConfigureAwait(false);
        }

    }
}
