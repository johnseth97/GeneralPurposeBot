using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralPurposeBot
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CoreModuleAttribute : Attribute
    {
    }
}
