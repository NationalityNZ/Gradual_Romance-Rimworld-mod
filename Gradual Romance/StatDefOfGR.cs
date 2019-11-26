using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;

namespace Gradual_Romance
{
    [DefOf]
    public static class StatDefOfGR
    {
        public static StatDef FacialAttractiveness;
        static StatDefOfGR()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(StatDefOfGR));
        }
    }
}
