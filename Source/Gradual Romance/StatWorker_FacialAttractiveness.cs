using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Gradual_Romance
{
/*   
    public class StatWorker_FacialAttractiveness : StatWorker
    {
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (req.HasThing && req.Thing is Pawn)
            {
                Pawn pawn = req.Thing as Pawn;
                return GRHelper.GRPawnComp(pawn).facialAttractiveness;
            }
            return 1f;
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            string stringBuilder = "";
            if (req.HasThing && req.Thing is Pawn)
            {
                Pawn pawn = req.Thing as Pawn;
                
                stringBuilder = ("FacialAttractivenessExplanation".Translate() + GRHelper.GRPawnComp(pawn).facialAttractiveness.ToString());
            }
            return stringBuilder;
        }
    }
    */
}
