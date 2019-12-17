using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;

namespace Gradual_Romance
{
    public class AttractionCalculator_RomanticDrive : AttractionCalculator
    {
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            return PsycheHelper.Comp(observer).Sexuality.AdjustedRomanticDrive;
        }
    }
}
