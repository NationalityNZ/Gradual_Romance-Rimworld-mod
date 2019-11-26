using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class AttractionCalculator_Skill : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            return (observer.IsColonist && assessed.IsColonist);
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            float observerValue = AttractionUtility.GetObjectiveSkillAttractiveness(observer);
            float assessedValue = AttractionUtility.GetObjectiveSkillAttractiveness(assessed);
            float value = assessedValue / observerValue;
            if (value < 1)
            {
                value = Mathf.Pow(value, SkillAttractivenessDampenerUpper);
            }
            else
            {
                value = Mathf.Pow(value, SkillAttractivenessDampenerLower);
            }
            return value;
        }


        private const float SkillAttractivenessDampenerUpper = 0.5f;

        private const float SkillAttractivenessDampenerLower = 0.2f;

    }
}
