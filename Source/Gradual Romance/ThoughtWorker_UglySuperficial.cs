using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class ThoughtWorker_UglySuperficial : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                return false;
            }
            else if (pawn.def.GetModExtension<XenoRomanceExtension>().faceCategory != pawn.def.GetModExtension<XenoRomanceExtension>().faceCategory)
            {
                return false;
            }
            else if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
            {
                return false;
            }
            else
            {
                int num = other.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
                if (num == -1)
                {
                    return ThoughtState.ActiveAtStage(0);
                }
                else if (num == -2)
                {
                    return ThoughtState.ActiveAtStage(1);
                }
                else if (num == -3)
                {
                    return ThoughtState.ActiveAtStage(2);
                }
                else if (num == -4)
                {
                    return ThoughtState.ActiveAtStage(3);
                }
                else { return false; }

            }
        }

    }

}
