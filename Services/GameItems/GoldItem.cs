using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class GoldItem : ItemBase
    {
        public override string Name => "Gold";

        public override string Description => "Sparkly.";

        public override bool StoreBuyable => false;

        public override decimal StoreBuyPrice => 5000000;

        public override string SingularName => "Gold Ingot";
        public override string PluralName => "Gold Ingots";
    }
}
