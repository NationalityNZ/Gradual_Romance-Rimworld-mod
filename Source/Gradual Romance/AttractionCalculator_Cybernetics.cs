using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class AttractionCalculator_Cybernetics : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            if (GradualRomanceMod.AttractionCalculation != GradualRomanceMod.AttractionCalculationSetting.Complex)
            {
                return false;
            }

            if (!observer.story.traits.HasTrait(TraitDefOf.BodyPurist) || !assessed.story.traits.HasTrait(TraitDefOf.Transhumanist))
            {
                return false;
            }

            if (assessed.health.hediffSet.CountAddedParts() <= 0)
            {
                return false;
            }

            return true;

        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            float cyberFactor = 1f;
            List<Hediff_AddedPart> listOfAddedParts = new List<Hediff_AddedPart> { };
            foreach (Hediff hediff in assessed.health.hediffSet.hediffs)
            {
                if (hediff is Hediff_AddedPart)
                {
                    listOfAddedParts.Add(hediff as Hediff_AddedPart);
                }
            }
            float valueOfParts = 0f;
            foreach (Hediff_AddedPart hediff in listOfAddedParts)
            {
                valueOfParts += hediff.def.spawnThingOnRemoved.BaseMarketValue;
            }
            valueOfParts = Mathf.Max(valueOfParts, 1f);
            cyberFactor = Mathf.Pow(valueOfParts, ValueDampener);
            if (observer.story.traits.HasTrait(TraitDefOf.BodyPurist))
            {
                cyberFactor = Mathf.Pow(valueOfParts, -1);
            }
            
            return 1f;
        }

        private const float ValueDampener = 0.1f;

    }
}
