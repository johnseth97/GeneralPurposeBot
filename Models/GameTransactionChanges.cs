using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Models
{
    public class GameTransactionChanges
    {
        public decimal Money { get; set; } = 0;
        public Dictionary<string, int> Items { get; set; } = new Dictionary<string, int>();
        public bool WalletCreated = false;
    }
}
