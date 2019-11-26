using System;
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
    public class GRThoughtWorker_WantToSleepWithSpouseOrLoverPatch
    {
        
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Low)]
        public static void GRNewThoughtState(ref ThoughtState __result, Pawn p)
        {
            __result = ThoughtState.Inactive;
        }
    }
}
