using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Gradual_Romance
{
    public class AttractionCalculator_Polygamy : AttractionCalculator
    {

        public override bool Check(Pawn observer, Pawn assessed)
        {
            return (assessed.story.traits.HasTrait(Psychology.TraitDefOfPsychology.Polygamous)) ;

        }

        public override float Calculate(Pawn observer, Pawn assessed)
        {
            float weight = 1f;
            if (GRPawnRelationUtility.IsPolygamist(assessed) && !GRPawnRelationUtility.IsPolygamist(observer))
            {
                weight *= .85f;
            }
            else if (GRPawnRelationUtility.IsPolygamist(assessed) && GRPawnRelationUtility.IsPolygamist(observer))
            {
                weight *= 1.2f;
            }
            return weight;
        }
    }
}
