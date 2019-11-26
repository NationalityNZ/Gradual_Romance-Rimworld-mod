using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;

namespace Gradual_Romance
{
    public class AttractionCalculator_MaleRelationshipReluctance : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            return (assessed.gender == Gender.Male) ;
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            if (AttractionUtility.IsWeaklyAndrophilic(observer) || AttractionUtility.IsExclusivelyGynephilic(observer))
            {
                return 0f;
            }
            if (AttractionUtility.IsOccasionallyAndrophilic(observer))
            {
                return 0.5f;
            }

            return 1f;
        }
    }
}
