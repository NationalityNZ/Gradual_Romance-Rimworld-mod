using System;
using RimWorld;

namespace Gradual_Romance
{
    [DefOf]
    public static class ThoughtDefOfGR
    {
        public static ThoughtDef RomanticInterest;

        public static ThoughtDef RomanticDisinterest;

        public static ThoughtDef ColdFeetSharingBed;

        public static ThoughtDef BotchedFlirt;

        public static ThoughtDef CheatedOnMePolygamist;

        public static ThoughtDef CheatedOnMeTolerated;

        public static ThoughtDef CheatedOnMeHighOpinion;

        public static ThoughtDef LoversLover;
        static ThoughtDefOfGR()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThoughtDefOfGR));
        }
    }

}
