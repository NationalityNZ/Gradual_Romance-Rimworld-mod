using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Gradual_Romance
{
    public class AttractionCalculator_Xeno : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            return (observer.def != assessed.def);
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            XenoRomanceExtension observerXenoRomance = observer.def.GetModExtension<XenoRomanceExtension>();
            XenoRomanceExtension assessedXenoRomance = assessed.def.GetModExtension<XenoRomanceExtension>();
            float extraspeciesAppeal = assessedXenoRomance.extraspeciesAppeal;
            if (extraspeciesAppeal <= 0)
            {
                return 0f;
            }
            if (extraspeciesAppeal >= 1)
            {
                return 1f;
            }
            float xenoFactor = extraspeciesAppeal;
            if (observerXenoRomance.faceCategory != assessedXenoRomance.faceCategory)
            {
                xenoFactor *= extraspeciesAppeal;
            }
            if (observerXenoRomance.bodyCategory != assessedXenoRomance.bodyCategory)
            {
                xenoFactor *= extraspeciesAppeal;
            }
            if (observerXenoRomance.mindCategory != assessedXenoRomance.mindCategory)
            {
                xenoFactor *= extraspeciesAppeal;
            }

            return xenoFactor;
        }
    }
}
