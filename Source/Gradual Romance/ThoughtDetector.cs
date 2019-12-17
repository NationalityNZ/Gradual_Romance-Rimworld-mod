using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Gradual_Romance
{
    public static class ThoughtDetector
    {
        public static bool HasSituationalThought(Pawn pawn, ThoughtDef thought)
        {
            if (!ThoughtUtility.CanGetThought(pawn, thought))
            {
                return false;
            }
            ThoughtState thoughtState = thought.Worker.CurrentState(pawn);
            if (thoughtState.Active)
            {
                return true;
            }
            return false;
        }
        public static bool HasSocialSituationalThought(Pawn pawn, Pawn other, ThoughtDef thought)
        {
            if (!ThoughtUtility.CanGetThought(pawn, thought))
            {
                return false;
            }
            ThoughtState thoughtState = thought.Worker.CurrentSocialState(pawn,other);
            if (thoughtState.Active)
            {
                return true;
            }
            return false;
        }
    }
}
