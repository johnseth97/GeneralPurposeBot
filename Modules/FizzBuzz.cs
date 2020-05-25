/*
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace GeneralPurposeBot.Modules
{
    public class FizzBuzz : ModuleBase
    {
        [Command("FizzBuzz"), Summary("!FizzBuzz: play the bot in fizzbuzz. The bot starts")]
        [Alias("ask")]
        public async Task FizzBuzzGame(int b, SocketUser opponet = null)
        {

            for (int i = 1; i <= b; i++)
            {
                var output = "";
                await Context.User.g

                if (i % 2 != 0)
                    if (i % 3 == 0)
                        output += "fizz";
                if (i % 5 == 0)
                    output += "buzz";
                if (output == "")
                    output += i;

                await Context.Channel.SendMessageAsync(output);
            }

        }
    }
}
*/

