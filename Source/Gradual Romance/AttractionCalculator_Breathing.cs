using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;

namespace Gradual_Romance
{
    public class AttractionCalculator_Breathing : AttractionCalculator
    {
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            float breathFactor = 1f;
            if (assessed.story.traits.HasTrait(TraitDefOf.CreepyBreathing))
            {
                breathFactor *= 0.8f;
            }
            return breathFactor;
        }
    }
}
