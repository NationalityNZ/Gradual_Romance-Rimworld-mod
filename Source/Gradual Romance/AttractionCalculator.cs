using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Gradual_Romance
{
    public class AttractionCalculator
    {
        public virtual bool Check(Pawn observer, Pawn assessed)
        {
            return true;
        }
        public virtual float Calculate(Pawn observer, Pawn assessed)
        {
            return 1f;
        }

        public AttractionFactorDef def;
    }
}
