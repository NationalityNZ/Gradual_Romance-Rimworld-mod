using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class AttractionCalculator_Youth : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            return (assessed.ageTracker.AgeBiologicalYearsFloat < assessed.def.GetModExtension<XenoRomanceExtension>().midlifeAge);
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            SimpleCurve curve = GradualRomanceMod.GetAttractivenessCurveFor(assessed);
            if (curve == null)
            {
                ThingDef assessedRace = assessed.def;
                Log.Error("NoAttractivenessCurve_error".Translate(assessedRace.defName));
                return 0f;
            }
            return (curve.Evaluate(assessed.ageTracker.AgeBiologicalYearsFloat));
        }


    }
}
