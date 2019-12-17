﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;

namespace Gradual_Romance
{
    public class AttractionCalculator_Maleness : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            if (assessed.gender != Gender.Male)
            {
                return false;
            }
            if (PsychologyBase.ActivateKinsey() && PsycheHelper.PsychologyEnabled(observer))
            {
                if (PsycheHelper.Comp(observer).Sexuality.kinseyRating == 3)
                {
                    return false;
                }
            }
            return true;
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            if (!AttractionUtility.IsAndrophilic(observer))
            {
                return 0f;
            }
            if (AttractionUtility.IsWeaklyAndrophilic(observer))
            {
                return 0.5f;

            }

            return 1f;
        }

    }
}
