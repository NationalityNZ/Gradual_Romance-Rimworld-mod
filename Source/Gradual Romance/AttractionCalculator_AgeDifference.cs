using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class AttractionCalculator_AgeDifference : AttractionCalculator
    {
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            //People are generally attracted to members of their own age group.
            //For young pawns, this is very significant.
            //As pawns get older, age is less of a factor.
            float observerAge = observer.ageTracker.AgeBiologicalYearsFloat;
            float assessedAge = assessed.ageTracker.AgeBiologicalYearsFloat;
            float ageDifference = Math.Abs(observerAge - assessedAge);
            float acceptableAgeDifference = AttractionUtility.GetAgeDeviation(observer);
            if (ageDifference - acceptableAgeDifference <= 0f)
            {
                return 1f;
            }

            float ageFactor = acceptableAgeDifference / ageDifference;
            /*
            //Makes females pawns more tolerant of old age difference.
            //Some people really don't like this, which is why it can be toggled off in the settings.
            if (GRHelper.ShouldApplyFemaleDifference(observer.gender))
            {
                if (observerAge < assessedAge)
                {
                    ageFactor = Mathf.Pow(ageFactor, GenderedAgePreference);
                }
            }
            */
            return ageFactor;
        }
    }
}
