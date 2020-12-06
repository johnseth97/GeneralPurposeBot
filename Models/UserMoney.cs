using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Models
{
    public class UserMoney
    {
        public Guid Id { get; set; }
        public ulong ServerId { get; set; }
        public ulong UserId { get; set; }
        public decimal Money { get; set; }
    }
}
