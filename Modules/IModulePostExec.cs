using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    public interface IModulePostExec
    {
        void PostExec(object __instance);
    }
}
