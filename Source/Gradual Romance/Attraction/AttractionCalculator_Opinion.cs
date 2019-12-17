using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class AttractionCalculator_Opinion : AttractionCalculator
    {
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            float opinion = AttractionUtility.GetRelationshipUnmodifiedOpinion(observer, assessed);
            float romanceFactor = Mathf.InverseLerp(-100, 100, opinion) * 2f;
            return Mathf.Pow(romanceFactor,UniversalOpinionImportance);
        }

        private const float UniversalOpinionImportance = 1f;

    }
}
