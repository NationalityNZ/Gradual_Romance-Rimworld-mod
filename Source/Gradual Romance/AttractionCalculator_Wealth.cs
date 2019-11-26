﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class AttractionCalculator_Wealth : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            return (assessed.IsColonist && observer.IsColonist);
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            return Mathf.InverseLerp(0f, AttractionUtility.GetObjectiveWealthAttractiveness(observer), AttractionUtility.GetObjectiveWealthAttractiveness(assessed));
        }
    }
}


