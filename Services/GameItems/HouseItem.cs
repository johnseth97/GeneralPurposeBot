using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class HouseItem : ItemBase
    {
        public override string Name => "House";

        public override string Description => "A decent sized mansion.";

        public override bool StoreBuyable => false;

        public override decimal StoreBuyPrice => 50000000;

        public override string SingularName => "House";
    }
}
