using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class ExampleCommands : ModuleBase
    {
        [Command("8ball")]
        [Alias("ask")]
        public async Task AskEightBall([Remainder]string args = null)
        {
            // I like using StringBuilder to build out the reply
            var sb = new StringBuilder();
            // let's use an embed for this one!
            var embed = new EmbedBuilder();

            // now to create a list of possible replies
            var replies = new List<string>();

            // add our possible replies
            replies.Add("yes");
            replies.Add("no");
            replies.Add("maybe");
            replies.Add("hazzzzy....");

            // time to add some options to the embed (like color and title)
            embed.WithColor(new Color(0, 255, 0));
            embed.Title = "Welcome to the 8-ball!";

            // we can get lots of information from the Context that is passed into the commands
            // here I'm setting up the preface with the user's name and a comma
            sb.AppendLine($"{Context.User.Username},");
            sb.AppendLine();

            // let's make sure the supplied question isn't null
            if (args == null)
            {
                // if no question is asked (args are null), reply with the below text
                sb.AppendLine("Sorry, can't answer a question you didn't ask!");
            }
            else
            {
                // if we have a question, let's give an answer!
                // get a random number to index our list with (arrays start at zero so we subtract 1 from the count)
                var answer = replies[new Random().Next(replies.Count - 1)];

                // build out our reply with the handy StringBuilder
                sb.AppendLine($"You asked: [**{args}**]...");
                sb.AppendLine();
                sb.AppendLine($"...your answer is [**{answer}**]");

                // bonus - let's switch out the reply and change the color based on it
                switch (answer)
                {
                    case "yes":
                        {
                            embed.WithColor(new Color(0, 255, 0));
                            break;
                        }
                    case "no":
                        {
                            embed.WithColor(new Color(255, 0, 0));
                            break;
                        }
                    case "maybe":
                        {
                            embed.WithColor(new Color(255, 255, 0));
                            break;
                        }
                    case "hazzzzy....":
                        {
                            embed.WithColor(new Color(255, 0, 255));
                            break;
                        }
                }
            }

            // now we can assign the description of the embed to the contents of the StringBuilder we created
            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build());
        }

        //Dice Roller
        [Command("dice"), Summary("Rolls dice")]
        public async Task DiceRoll()
        {
            var random = new Random();
            var number = random.Next(1, 7);
            await Context.Channel.SendMessageAsync("Your 6 sided 🎲 rolled a " + number + "!").ConfigureAwait(false);
        }

        [Command("hello")]
        public async Task HelloCommand()
        {
            // initialize empty string builder for reply
            var sb = new StringBuilder();

            // get user info from the Context
            var user = Context.User;

            // build out the reply
            sb.AppendLine($"You are -> [{user.Username}]");
            sb.AppendLine("I must now say, World!");

            // send simple string reply
            await ReplyAsync(sb.ToString());
        }

        //Who Bottoms
        [Command("whobottoms"), Summary("Decides The Bottom")]
        public async Task WhoBottoms([Remainder]string args = null)
        {
            // I like using StringBuilder to build out the reply
            var sb = new StringBuilder();
            // let's use an embed for this one!
            var embed = new EmbedBuilder();

            // now to create a list of possible replies
            var replies = new List<string>();

            // time to add some options to the embed (like color and title)
            embed.WithColor(new Color(155, 0, 155));
            embed.Title = "Who Bottoms?";
            // we can get lots of information from the Context that is passed into the commands
            // here I'm setting up the preface with the user's name and a comma

            //doing a coinflip
            var random = new Random();
            var number = random.Next(1, 3);

            if (number == 1)
            {
                sb.AppendLine($"{Context.User.Username} Bottoms!");
                sb.AppendLine();
                sb.AppendLine($"{args} Tops!");
            }

            if (number == 2)
                sb.AppendLine($"{args} Bottoms!");
            sb.AppendLine();
            sb.AppendLine($"{Context.User.Username} Tops!");

            // now we can assign the description of the embed to the contents of the StringBuilder we created
            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build());
        }

    }
}
