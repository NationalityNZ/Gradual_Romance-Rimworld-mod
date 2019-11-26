using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;

namespace Gradual_Romance.Harmony
{
    
    [HarmonyPatch(typeof(LovePartnerRelationUtility), nameof(LovePartnerRelationUtility.HasAnyLovePartner))]
    public static class GRHasAnyLovePartnerPatch
    {
        [HarmonyPostfix]
        public static void GRHasAnyLovePartner (ref bool __result, Pawn pawn)
        {
            if (__result != true)
            {
                if (pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOfGR.Lovefriend) != null)
                {
                    __result = true;
                }
            }
        }
    }
    [HarmonyPatch(typeof(LovePartnerRelationUtility), nameof(LovePartnerRelationUtility.IsLovePartnerRelation))]
    public static class GRIsLovePartnerRelationPatch
    {
        [HarmonyPostfix]
        public static void GRIsLovePartnerRelation(ref bool __result, PawnRelationDef relation)
        {
            if (__result != true)
            {
                if (relation == PawnRelationDefOfGR.Lovefriend)
                {
                    __result = true;
                }
            }
        }
    }
    [HarmonyPatch(typeof(LovePartnerRelationUtility), nameof(LovePartnerRelationUtility.IsExLovePartnerRelation))]
    public static class GRIsExLovePartnerRelationPatch
    {
        [HarmonyPostfix]
        public static void GRIsExLovePartnerRelation(ref bool __result, PawnRelationDef relation)
        {
            if (__result != true)
            {
                if (relation == PawnRelationDefOfGR.ExLovefriend)
                {
                    __result = true;
                }
            }
        }
    }
    [HarmonyPatch(typeof(LovePartnerRelationUtility), nameof(LovePartnerRelationUtility.ExistingLovePartner))]
    public static class GRExistingLovePartnerPatch
    {
        [HarmonyPostfix]
        public static void GRExistingLovePartner(ref Pawn __result, Pawn pawn)
        {
            if (__result == null)
            {
                Pawn firstDirectRelationPawn = pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOfGR.Lovefriend);
                if (firstDirectRelationPawn != null)
                {
                    __result = firstDirectRelationPawn;
                }
            }
        }
    }
    [HarmonyPatch(typeof(LovePartnerRelationUtility), nameof(LovePartnerRelationUtility.LovePartnerRelationExists))]
    public static class GRLovePartnerRelationExistsPatch
    {
        [HarmonyPostfix]
        public static void GRLovePartnerRelationExists(ref bool __result, Pawn first, Pawn second)
        {
            if (__result != true)
            {
                __result = first.relations.DirectRelationExists(PawnRelationDefOfGR.Lovefriend, second);
            }
        }
    }
    [HarmonyPatch(typeof(LovePartnerRelationUtility), nameof(LovePartnerRelationUtility.ExLovePartnerRelationExists))]
    public static class GRExLovePartnerRelationExistsPatch
    {
        [HarmonyPostfix]
        public static void GRExLovePartnerRelationExists(ref bool __result, Pawn first, Pawn second)
        {
            if (__result != true)
            {
                __result = first.relations.DirectRelationExists(PawnRelationDefOfGR.ExLovefriend, second);
            }
        }
    }
    [HarmonyPatch(typeof(LovePartnerRelationUtility), nameof(LovePartnerRelationUtility.GiveRandomExLoverOrExSpouseRelation))]
    public static class GRGiveRandomExLoverOrExSpouseRelationPatch
    {
        [HarmonyPrefix]
        public static bool GRGiveRandomExLoverOrExSpouseRelation(Pawn first, Pawn second)
        {
            PawnRelationDef def;
            float value = Rand.Value;
            if (value < 0.33)
            {
                def = PawnRelationDefOfGR.ExLovefriend;
            }
            else if (value < 0.66)
            {
                def = PawnRelationDefOf.ExLover;
            }
            else
            {
                def = PawnRelationDefOf.ExSpouse;
            }
            first.relations.AddDirectRelation(def, second);
            return false;
        }
    }



    //Let's throw out even Psychology's formula!
    //Since this is causing a lot of redundant effort, it's probably a good idea to find a way to bypass Psychology and Vanilla calculation entirely.
    //For right now, this will do.
    [HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.SecondaryLovinChanceFactor))]
    public static class GRSecondaryLovinChanceFactorPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.VeryHigh)]
        public static void GRSecondaryLovinChanceFactor(Pawn_RelationsTracker __instance, ref float __result, ref Pawn ___pawn, Pawn otherPawn)
        {
            __result = AttractionUtility.CalculateAttraction(___pawn, otherPawn, true, false);
        }
    }

    [HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.SecondaryRomanceChanceFactor))]
    public static class GRSecondaryRomanceChanceFactorPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.VeryHigh)]
        public static void GRSecondaryRomanceChanceFactor(Pawn_RelationsTracker __instance, ref float __result, ref Pawn ___pawn, Pawn otherPawn)
        {
            __result = AttractionUtility.CalculateAttraction(___pawn, otherPawn, false, true);
        }
    }

}
