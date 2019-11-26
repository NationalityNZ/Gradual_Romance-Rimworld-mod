using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;
using UnityEngine;

namespace Gradual_Romance
{
    public class AttractionCalculator_Ability : AttractionCalculator
    {
        public override bool Check (Pawn observer, Pawn assessed)
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
            float disabilityFactor = 1f;
            disabilityFactor *= Mathf.Lerp(0.2f, 1f, assessed.health.capacities.GetLevel(PawnCapacityDefOf.Talking));
            disabilityFactor *= Mathf.Lerp(0.2f, 1f, assessed.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation));
            disabilityFactor *= Mathf.Lerp(0.2f, 1f, assessed.health.capacities.GetLevel(PawnCapacityDefOf.Moving));
            return disabilityFactor;
        }

    }
}
