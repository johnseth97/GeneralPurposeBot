﻿using Discord;
using GeneralPurposeBot.Services;
using GeneralPurposeBot.Services.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Models
{
    public class GameTransaction
    {
        public GameItemService ItemService { get; }
        public GameMoneyService MoneyService { get; }
        public IGuild Guild { get; set; }
        private Dictionary<IGuildUser, GameTransactionChanges> Users { get; } = new Dictionary<IGuildUser, GameTransactionChanges>();

        public string Message { get; set; }
        protected Dictionary<string, ItemBase> AllItems => ItemService.Items;
        public GameTransaction(GameItemService itemService, GameMoneyService moneyService, IGuild guild, IGuildUser defaultUser)
        {
            ItemService = itemService;
            MoneyService = moneyService;
            Guild = guild;
            AddUser(defaultUser);
        }

        /// <summary>
        /// Takes an item name and returns the best matching item
        /// </summary>
        /// <param name="search">Item name to search for</param>
        /// <returns>Definition of searched item</returns>
        public ItemBase FindItem(string search)
        {
            if (ItemService.Items.ContainsKey(search))
                return ItemService.Items[search];
            var items = ItemService.Items.Values
                .Where(i => string.Equals(i.Name, search, StringComparison.InvariantCultureIgnoreCase));
            return items.FirstOrDefault();
        }

        /// <summary>
        /// Adds a user to the game transaction, if they are not already in it.
        /// In most cases you shouldn't need to call this manually.
        /// </summary>
        /// <param name="user">User to add to the transaction</param>
        public void AddUser(IGuildUser user)
        {
            Users.Add(user, new GameTransactionChanges());
        }

        /// <summary>
        /// Adds money to a user
        /// </summary>
        /// <param name="amount">Amount of money to give</param>
        /// <param name="user">User to give money to</param>
        public void AddMoney(decimal amount, IGuildUser user = null)
        {
            if (user == null) user = Users[0];
            if (!Users.ContainsKey(user)) AddUser(user);
            Users[user].Money += amount;
        }

        /// <summary>
        /// Removes money from a user
        /// </summary>
        /// <param name="amount">Amount of money to remove</param>
        /// <param name="user">User to remove money from</param>
        public void TakeMoney(decimal amount, IGuildUser user = null)
        {
            AddMoney(-amount, user);
        }

        /// <summary>
        /// Gets how much money a user has
        /// </summary>
        /// <param name="user">User to get money of</param>
        /// <returns>Amount of money user has</returns>
        public decimal GetMoney(IGuildUser user = null)
        {
            if (user == null) user = Users[0];
            if (!Users.ContainsKey(user)) AddUser(user);
            return MoneyService.GetMoney(Guild.Id, user.Id) + Users[user].Money;
        }

        /// <summary>
        /// Gives an item to a user
        /// </summary>
        /// <param name="itemName">Name of item to give</param>
        /// <param name="quantity">Quantity to give</param>
        /// <param name="user">User to give item to</param>
        public void GiveItems(string itemName, int quantity = 1, IGuildUser user = null)
        {
            if (user == null) user = Users[0];
            if (!Users.ContainsKey(user)) AddUser(user);
            if (!Users[user].Items.ContainsKey(itemName))
                Users[user].Items.Add(itemName, quantity);
            else
                Users[user].Items[itemName] += quantity;
        }

        /// <summary>
        /// Removes an item from a user
        /// </summary>
        /// <param name="itemName">Name of item to remove</param>
        /// <param name="quantity">Quantity to remove</param>
        /// <param name="user">User to remove item from</param>
        public void TakeItems(string itemName, int quantity = 1, IGuildUser user = null)
        {
            GiveItems(itemName, -quantity, user);
        }

        /// <summary>
        /// Gets how many of an item a user has
        /// </summary>
        /// <param name="itemName">Name of item to get quantity of</param>
        /// <param name="user">User to check item quantity for</param>
        /// <returns>Number of the item the user has</returns>
        public int GetItemQuantity(string itemName, IGuildUser user = null)
        {
            if (user == null) user = Users[0];
            if (!Users.ContainsKey(user)) AddUser(user);
            var baseQuantity = ItemService.GetItemQuantity(Guild.Id, user.Id, itemName);
            if (!Users[user].Items.ContainsKey(itemName)) return baseQuantity;
            return baseQuantity + Users[user].Items[itemName];
        }

        /// <summary>
        /// Checks if a user has any of an item.
        /// </summary>
        /// <param name="itemName">Item to check</param>
        /// <param name="user">User to check</param>
        /// <returns>If the user has any of the item</returns>
        public bool HasItem(string itemName, IGuildUser user = null)
        {
            return GetItemQuantity(itemName, user) > 0;
        }

        public string FinalizeTransaction()
        {
            var changedUsers = Users.Where(u => u.Value.Money != 0 || u.Value.Items.Any(i => i.Value != 0) || u.Value.WalletCreated);
            // return now if there are no changes
            if (!changedUsers.Any())
                return Message;
            string message = "";
            foreach (var user in Users.Where(u => u.Value.WalletCreated))
            {
                message += $"{user.Key.Mention}: You've never used the bot's minigame features in this server, so we went ahead and made a new wallet for you.\n";
            }

            message += Message;

            // redo query, excluding users who *only* had a wallet created
            changedUsers = Users.Where(u => u.Value.Money != 0 || u.Value.Items.Any(i => i.Value != 0));
            if (!changedUsers.Any())
                return message;
            message += "(";
            if (changedUsers.Count() > 1)
            {
                var changeStrings = new List<string>();
                foreach (var user in changedUsers)
                {
                    var userChangeStr = $"{user.Key.Mention}: ";
                    var userChangeStrings = new List<string>();
                    if (user.Value.Money > 0)
                    {
                        userChangeStrings.Add($"+${user.Value.Money.FormatMoney()}");
                        MoneyService.AddMoney(Guild.Id, user.Key.Id, user.Value.Money)
                    }

                    if (user.Value.Money < 0)
                    {
                        userChangeStrings.Add($"-${user.Value.Money.FormatMoney()}");
                        MoneyService.AddMoney(Guild.Id, user.Key.Id, user.Value.Money)
                    }

                    foreach (var item in user.Value.Items.Where(item => item.Value != 0))
                    {
                        if (item.Value > 0)
                        {
                            userChangeStrings.Add($"+{item.Value} {AllItems[item.Key].GetName(item.Value)}");
                            ItemService.GiveItem(Guild.Id, user.Key.Id, item.Key, item.Value);
                        }

                        if (item.Value < 0)
                        {
                            userChangeStrings.Add($"-{item.Value} {AllItems[item.Key].GetName(item.Value)}");
                            ItemService.TakeItem(Guild.Id, user.Key.Id, item.Key, -item.Value);
                        }
                    }
                    userChangeStr += string.Join(", ", userChangeStrings);
                    changeStrings.Add(userChangeStr);
                }
                message += string.Join("; ", changeStrings);
            }
            else
            {
                var user = changedUsers.First();
                var userChangeStrings = new List<string>();
                if (user.Value.Money > 0)
                {
                    userChangeStrings.Add($"+${user.Value.Money.FormatMoney()}");
                    MoneyService.AddMoney(Guild.Id, user.Key.Id, user.Value.Money)
                    }

                if (user.Value.Money < 0)
                {
                    userChangeStrings.Add($"-${user.Value.Money.FormatMoney()}");
                    MoneyService.AddMoney(Guild.Id, user.Key.Id, user.Value.Money)
                    }

                foreach (var item in user.Value.Items.Where(item => item.Value != 0))
                {
                    if (item.Value > 0)
                    {
                        userChangeStrings.Add($"+{item.Value} {AllItems[item.Key].GetName(item.Value)}");
                        ItemService.GiveItem(Guild.Id, user.Key.Id, item.Key, item.Value);
                    }

                    if (item.Value < 0)
                    {
                        userChangeStrings.Add($"-{item.Value} {AllItems[item.Key].GetName(item.Value)}");
                        ItemService.TakeItem(Guild.Id, user.Key.Id, item.Key, -item.Value);
                    }
                }
                message += string.Join(", ", userChangeStrings);
            }
            message += ")";
        }
    }
}