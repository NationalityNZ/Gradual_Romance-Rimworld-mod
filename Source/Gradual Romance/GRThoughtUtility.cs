using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;


namespace Gradual_Romance
{
    public static class GRThoughtUtility
    {
        public static int NumOfMemoriesOfDefWhereOtherPawnIs(Pawn pawn, Pawn other, ThoughtDef thought)
        {
            List<Thought_Memory> memories = pawn.needs.mood.thoughts.memories.Memories;
            int count = 0;
            for (int i = 0; i < memories.Count(); i++)
            {
                if (memories[i].def == thought && memories[i].otherPawn == other)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
