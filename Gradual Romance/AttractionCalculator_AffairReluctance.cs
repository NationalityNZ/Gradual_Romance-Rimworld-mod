using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class AttractionFactor_AffairReluctance : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            if (GRPawnRelationUtility.IsAnAffair(observer, assessed, out Pawn cuck1, out Pawn cuck2))
            {
                observerCuckold = cuck1;
                assessedCuckold = cuck2;
                return true;
            }

            return false;

        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            float affairReluctance = 1f;
            float affairReluctance2 = 1f;
            if (observerCuckold != null)
            {
                affairReluctance = GRPawnRelationUtility.AffairReluctance(GRPawnRelationUtility.MostAdvancedRelationshipBetween(observer, observerCuckold));
                affairReluctance *= Mathf.Pow(Mathf.InverseLerp(-100f, 5f, observer.relations.OpinionOf(observerCuckold)), -0.33f);
            }
            if (assessedCuckold != null)
            {
                affairReluctance2 = GRPawnRelationUtility.AffairReluctance(GRPawnRelationUtility.MostAdvancedRelationshipBetween(assessed, assessedCuckold));
                affairReluctance2 *= Mathf.Pow(Mathf.InverseLerp(-100f, 5f, observer.relations.OpinionOf(assessedCuckold)), -0.33f);
            }
            return Mathf.Min(affairReluctance,affairReluctance2);
        }
        private Pawn observerCuckold = null;
        private Pawn assessedCuckold = null;
    }
}
