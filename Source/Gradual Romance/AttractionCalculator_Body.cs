using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;

namespace Gradual_Romance
{
    public class AttractionCalculator_Body : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            if (observer.story.traits.HasTrait(TraitDefOfPsychology.OpenMinded))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            //The intention of this is to provide maluses/bonuses for pawns of specific body types.
            //I'm not entirely sure how to do this yet.
            return 1f;
        }
    }
}
