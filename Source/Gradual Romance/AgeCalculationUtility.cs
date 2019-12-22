using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;
using UnityEngine;

namespace Gradual_Romance
{
    public static class AgeCalculationUtility
    {
        public static float MaturityDeviation(float maturity)
        {
            return (maturity * pctAgeDeviation);
        }

        public static float GetMaturity(Pawn pawn)
        {
            return GradualRomanceMod.GetMaturityCurveFor(pawn).Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat);
        }

        public static float GetMaturityFactor(Pawn observer, Pawn assessor)
        {
            float observerMaturity = GetMaturity(observer);
            float assessorMaturity = GetMaturity(assessor);
            float maturityDeviation = MaturityDeviation(observerMaturity);
            float maturityDifference = Mathf.Abs(observerMaturity - assessorMaturity);
            float factor = Mathf.InverseLerp(3f, maturityDeviation, maturityDifference);
            if (assessorMaturity < 1 && assessorMaturity < observerMaturity && factor != 1f)
            {
                factor = Mathf.Pow(factor, youthPenalty);
            }
            else if (assessorMaturity > 1 && assessorMaturity > observerMaturity && factor != 1f)
            {
                factor = Mathf.Pow(factor, oldAgePenalty);
            }
            return factor;
        }

        public const float pctAgeDeviation = 0.05f;
        public const float youthPenalty = 2f;
        public const float oldAgePenalty = 1.5f;
        public static readonly SimpleCurve recoveryTimeByMaturity = new SimpleCurve
        {
            { new CurvePoint(0.8f, 1.5f),true },
            { new CurvePoint(1f, 2f),true },
            { new CurvePoint(1.5f, 4f),true },
            { new CurvePoint(2f,12f),true },
            { new CurvePoint(3f,36f),true }
        };
    }
}
