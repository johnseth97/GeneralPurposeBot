using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralPurposeBot
{
    public static class Util
    {
        public static string GetFullName(this ModuleInfo module)
        {
            var name = "";
            if (module.Parent != null)
            {
                name = GetFullName(module.Parent) + ".";
            }
            name += module.Name;
            return name;
        }
    }
}
