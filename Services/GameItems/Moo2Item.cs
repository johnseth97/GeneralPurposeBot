using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class Moo2Item : ItemBase
    {
        public override string Name => "Moo2";

        public override string Description => "The moo has evolved into something new.";

        public override bool StoreBuyable => false;

        public override decimal StoreBuyPrice => 500000000;
    }
}
