using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Psychology;
using RimWorld;
using Verse;
using Harmony;
using UnityEngine;

namespace Gradual_Romance.Harmony
{


    [HarmonyPatch(typeof(Pawn_SexualityTracker))]
    [HarmonyPatch("AdjustedSexDrive", MethodType.Getter)]
    public class AdjustedSexDrive_GRPatch
    {
        [HarmonyPostfix]
        public static void GRXenoSexDrive(ref float __result, ref Pawn_SexualityTracker __instance, ref Pawn ___pawn)
        {
            SimpleCurve curve = GradualRomanceMod.GetSexDriveCurveFor(___pawn);
            if (curve != null)
            {
                __result = Mathf.Clamp01(curve.Evaluate(___pawn.ageTracker.AgeBiologicalYearsFloat) * Mathf.InverseLerp(0f, 0.5f, __instance.sexDrive));
            }
        }
    }
}


/*
 *         public float AdjustedSexDrive
        {
            get
            {
                float ageFactor = 1f;
                if (pawn.gender == Gender.Male) {
                    ageFactor = MaleSexDriveCurve.Evaluate(pawn.ageTracker.AgeBiologicalYears);
                }
                else if (pawn.gender == Gender.Female)
                {
                    ageFactor = FemaleSexDriveCurve.Evaluate(pawn.ageTracker.AgeBiologicalYears);
                }
                return Mathf.Clamp01(ageFactor * Mathf.InverseLerp(0f, 0.5f, this.sexDrive));
            }
        }
 * 
 * 
 */
