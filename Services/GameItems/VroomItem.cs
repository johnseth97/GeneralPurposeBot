using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class VroomItem : ItemBase
    {
        public override string Name => "Vroom";

        public override string Description => "Vroom vroom.";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 500000;
    }
}
