using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;

namespace Gradual_Romance
{
    public class AttractionCalculator_Voice : AttractionCalculator
    {
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            float voiceFactor = 1f;
            if (assessed.story.traits.HasTrait(TraitDefOf.AnnoyingVoice))
            {
                voiceFactor *= 0.8f;
            }
            if (assessed.story.traits.HasTrait(TraitDefOfGR.MelodicVoice))
            {
                voiceFactor *= 1.25f;
            }
            return voiceFactor;
        }
    }
}
