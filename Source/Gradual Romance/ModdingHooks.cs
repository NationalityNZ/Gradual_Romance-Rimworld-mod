using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Gradual_Romance
{
    public static class ModHooks
    {
        //BIRDS AND BEES
        public static bool UsingBirdsAndBees()
        {
            return (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "The Birds and the Bees"));
        }


        //HUMANOID ALIEN RACES
        public static bool UsingHumanoidAlienFramework()
        {
            return (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Humanoid Alien Races 2.0"));
        }



        //DUBS BAD HYGIENE       
        public static bool UsingDubsHygiene()
        {
            return (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Dubs Bad Hygiene"));
        }
        public static float GetHygieneNeed(Pawn pawn)
        {
            NeedDef hygiene = DefDatabase<NeedDef>.AllDefsListForReading.Find(x => x.defName == "Hygiene");
            if (hygiene == null)
            {
                return 1f;
            }
            else
            {
                return pawn.needs.TryGetNeed(hygiene).CurLevelPercentage;
            }
        }
    }
}
