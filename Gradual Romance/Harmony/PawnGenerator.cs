using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Psychology;
using RimWorld;
using Harmony;
using Verse;
using UnityEngine;

namespace Gradual_Romance.Harmony
{
    [HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
    public static class GRPawnGeneratorPatch
    {
        [LogPerformance]
        [HarmonyPriority(Priority.Low)]
        [HarmonyPostfix]
        public static void GRPawnGenerator_AddBeautyTrait(ref Pawn pawn, PawnGenerationRequest request)
        {
            if (!pawn.story.traits.HasTrait(TraitDefOf.Beauty) && GradualRomanceMod.rerollBeautyTraits)
            {
                int result = Mathf.Clamp(Mathf.RoundToInt(Rand.Gaussian(0, 1.5f)), -4, 4);
                pawn.story.traits.GainTrait(new Trait(TraitDefOf.Beauty, result, true));
            }
        }
    }
}
