using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Models
{
    public class UserItem
    {
        public Guid Id { get; set; }
        public ulong ServerId { get; set; }
        public ulong UserId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}
