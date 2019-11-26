using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;

namespace Gradual_Romance
{
    public class AttractionCalculator_Morality : AttractionCalculator
    {
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            float moralityFactor = 1f;
            if (assessed.story.traits.HasTrait(TraitDefOf.Kind))
            {
                moralityFactor *= 1.2f;
            }
            if (assessed.story.traits.HasTrait(TraitDefOfPsychology.BleedingHeart))
            {
                moralityFactor *= 1.2f;
            }
            return moralityFactor;
        }
    }
}
