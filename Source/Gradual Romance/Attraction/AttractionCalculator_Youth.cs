﻿using System;
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
            return (AgeCalculationUtility.GetMaturity(assessed) < AgeCalculationUtility.GetMaturity(observer));
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {

            return (Mathf.Clamp01(AgeCalculationUtility.GetMaturityFactor(observer,assessed)));
        }


    }
}
