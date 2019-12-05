﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;

namespace Gradual_Romance
{
    public class AttractionCalculator_Body : AttractionCalculator
    {
        /*
        public override bool Check(Pawn observer, Pawn assessed)
        {
            if (observer.story.traits.HasTrait(TraitDefOfPsychology.OpenMinded))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        */
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            float bodyFactor = 1f;
            if (assessed.story.traits.HasTrait(TraitDefOfGR.Wimp))
            {
                bodyFactor *= 0.8f;
            }
            if (assessed.story.traits.HasTrait(TraitDefOfGR.Nimble))
            {
                bodyFactor *= 1.2f;
            }
            if (assessed.story.traits.HasTrait(TraitDefOf.Tough))
            {
                bodyFactor *= 1.2f;
            }
            if (assessed.story.traits.HasTrait(TraitDefOf.SpeedOffset))
            {
                int x = assessed.story.traits.DegreeOfTrait(TraitDefOf.SpeedOffset);
                switch (x)
                {
                    case -1:
                        bodyFactor *= 0.8f;
                        break;
                    case 1:
                        bodyFactor *= 1.2f;
                        break;
                    case 2:
                        bodyFactor *= 1.4f;
                        break;
                }
            }
            if (assessed.story.traits.HasTrait(TraitDefOfGR.Immunity))
            {
                int x = assessed.story.traits.DegreeOfTrait(TraitDefOfGR.Immunity);
                switch (x)
                {
                    case -1:
                        bodyFactor *= 0.8f;
                        break;
                    case 1:
                        bodyFactor *= 1.2f;
                        break;
                }
            }
            
            if (assessed.story.bodyType.HasModExtension<GRBodyTypeExtension>())
            {
                GRBodyTypeExtension extension = assessed.story.bodyType.GetModExtension<GRBodyTypeExtension>();
                if (extension.attractiveForGender == Gender.Male)
                {
                    if (!GRHelper.ShouldApplyMaleDifference(assessed.gender))
                    {
                        bodyFactor *= extension.attractivenessFactor;
                    }

                }
                else if (extension.attractiveForGender == Gender.Female)
                {
                    if (!GRHelper.ShouldApplyFemaleDifference(assessed.gender))
                    {
                        bodyFactor *= extension.attractivenessFactor;
                    }

                }
                else
                {
                    bodyFactor *= extension.attractivenessFactor;
                }
            }


            return bodyFactor;
        }
    }
}
