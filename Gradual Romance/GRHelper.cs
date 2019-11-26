using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Gradual_Romance
{
    public class GRHelper
    {
        /// <summary>
        /// 0 = no reason
        /// 1 = sexual attractiveness
        /// 2 = wealth attractiveness
        /// 3 = social attractiveness
        /// 4 = romantic attractiveness
        /// </summary>
        public static int relationshipChangeReasonCache = 0;
        public static bool ShouldApplyFemaleDifference(Gender testedGender = Gender.Female)
        {
            return ((GradualRomanceMod.genderMode == GradualRomanceMod.GenderModeSetting.Vanilla && testedGender == Gender.Female) || (GradualRomanceMod.genderMode == GradualRomanceMod.GenderModeSetting.Inverse && testedGender == Gender.Male));
        }
        public static bool ShouldApplyMaleDifference(Gender testedGender = Gender.Male)
        {
            return ((GradualRomanceMod.genderMode == GradualRomanceMod.GenderModeSetting.Vanilla && testedGender == Gender.Male) || (GradualRomanceMod.genderMode == GradualRomanceMod.GenderModeSetting.Inverse && testedGender == Gender.Female));
        }
        public static GRPawnComp GRPawnComp(Pawn pawn)
        {
            return pawn.GetComp<GRPawnComp>();
        }
    }
}
