﻿using System;
using RimWorld;

namespace Gradual_Romance
{
    [DefOf]
    public static class PawnRelationDefOfGR
    {
        //public static PawnRelationDef RomanticInterest;

        public static PawnRelationDef Sweetheart;

        public static PawnRelationDef Lovebuddy;

        public static PawnRelationDef Lovefriend;

        public static PawnRelationDef ExLovefriend;

        public static PawnRelationDef Paramour;

        static PawnRelationDefOfGR()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PawnRelationDefOfGR));
        }
    }
    
}
