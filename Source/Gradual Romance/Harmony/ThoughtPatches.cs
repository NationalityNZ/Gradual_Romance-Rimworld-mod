﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;
using Psychology;

namespace Gradual_Romance.Harmony
{
    //This forces the want to sleep with spouse thought to be inactive.
    [HarmonyPatch(typeof(ThoughtWorker_WantToSleepWithSpouseOrLover), "CurrentStateInternal")]
    public static class GRThoughtWorker_WantToSleepWithSpouseOrLoverPatch
    {
        
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Low)]
        public static void GRNewThoughtState(ref ThoughtState __result, Pawn p)
        {
            __result = ThoughtState.Inactive;
        }
    }

    [HarmonyPatch(typeof(ThoughtWorker_Pretty), "CurrentSocialStateInternal", new Type[] { typeof(Pawn), typeof(Pawn) })]
    public static class GRThoughtWorker_PrettyPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        public static void GRPrettyPatch(ref ThoughtState __result, Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                __result = false;
            }
            else if (pawn.def.GetModExtension<XenoRomanceExtension>().faceCategory != pawn.def.GetModExtension<XenoRomanceExtension>().faceCategory)
            {
                __result = false;
            }
            else if (RelationsUtility.IsDisfigured(other))
            {
                __result = false;
            }
            else if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
            {
                __result = false;
            }
            else { 
                int num = other.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
                if (num == 1)
                {
                    __result = ThoughtState.ActiveAtStage(0);
                }
                else if (num == 2)
                {
                    __result = ThoughtState.ActiveAtStage(1);
                }
                else if (num == 3)
                {
                    __result = ThoughtState.ActiveAtStage(2);
                }
                else if (num == 4)
                {
                    __result = ThoughtState.ActiveAtStage(3);
                }
                else { __result = false; }
                
            }
        }
    }

    [HarmonyPatch(typeof(ThoughtWorker_Ugly), "CurrentSocialStateInternal", new Type[] {typeof(Pawn),typeof(Pawn) })]
    public static class GRThoughtWorker_UglyPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        public static void GRUglyPatch(ref ThoughtState __result, Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                __result = false;
            }
            else if (pawn.def.GetModExtension<XenoRomanceExtension>().faceCategory != pawn.def.GetModExtension<XenoRomanceExtension>().faceCategory)
            {
                __result = false;
            }
            else if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
            {
                __result = false;
            }

            else
            {
                int num = other.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
                if (num == -1)
                {
                    __result = ThoughtState.ActiveAtStage(0);
                }
                else if (num == -2)
                {
                    __result = ThoughtState.ActiveAtStage(1);
                }
                else if (num == -3)
                {
                    __result = ThoughtState.ActiveAtStage(2);
                }
                else if (num == -4)
                {
                    __result = ThoughtState.ActiveAtStage(3);
                }
                else
                { __result = false; }
                
            }
        }
    }
}
/*	public class ThoughtWorker_Pretty : ThoughtWorker
	{
		// Token: 0x06000A0C RID: 2572 RVA: 0x0004F564 File Offset: 0x0004D964
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
			{
				return false;
			}
			if (RelationsUtility.IsDisfigured(other))
			{
				return false;
			}
			if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
			{
				return false;
			}
			int num = other.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
			if (num == 1)
			{
				return ThoughtState.ActiveAtStage(0);
			}
			if (num == 2)
			{
				return ThoughtState.ActiveAtStage(1);
			}
			return false;
		}
	}
    */