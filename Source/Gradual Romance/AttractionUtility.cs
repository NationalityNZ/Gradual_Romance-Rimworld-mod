using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Psychology;
using Verse;
using UnityEngine;
using HugsLib;
using HugsLib.Utils;

namespace Gradual_Romance
{
    public static class AttractionUtility
   {
        // AGE TOOLS //
        public static bool IsAgeAppropriate(Pawn pawn)
        {
            return (pawn.ageTracker.AgeBiologicalYearsFloat > 18f);
        }

        private static readonly SimpleCurve AgeDeviation = new SimpleCurve
        {
            {
                new CurvePoint(14, 1f),
                true
            },
            {
                new CurvePoint(16, 2f),
                true
            },
            {
                new CurvePoint(18, 3f),
                true
            },
            {
                new CurvePoint(20, 4f),
                true
            },
            {
                new CurvePoint(22, 5f),
                true
            },
            {
                new CurvePoint(24, 6f),
                true
            },
            {
                new CurvePoint(28, 7f),
                true
            },
            {
                new CurvePoint(32, 8f),
                true
            },
            {
                new CurvePoint(36, 9f),
                true
            },
            {
                new CurvePoint(40, 10f),
                true
            }
        };

        private static readonly SimpleCurve SkillAttractivenessCurve = new SimpleCurve
        {
            {
                new CurvePoint(0, 0.25f),
                true
            },
            {
                new CurvePoint(2, 1f),
                true
            },
            {
                new CurvePoint(4, 2f),
                true
            },
            {
                new CurvePoint(8, 4f),
                true
            },
            {
                new CurvePoint(12, 8f),
                true
            },
            {
                new CurvePoint(14, 16f),
                true
            },
            {
                new CurvePoint(16, 32f),
                true
            },
            {
                new CurvePoint(18, 64f),
                true
            },
            {
                new CurvePoint(20, 128f),
                true
            }
        };

        public static float GetAgeDeviation(Pawn pawn)
        {
            if (pawn.def.defName != "Human")
            {
                return (pawn.RaceProps.lifeExpectancy / 8);
            }
            return AgeDeviation.Evaluate(pawn.ageTracker.AgeBiologicalYears);

        }

        /// KINSEY TOOLS ///

        /// 
        /// The Kinsey system doesn't really reflect attraction, but rather under what circumstances they will pursue homosexual/heterosexual relationships
        /// Exclusively - will never consider relations outside of one gender
        /// Weakly - the pawn will interact informally with dispreffered sex pawns, and will never pursue formal relationships with them.
        /// Occasionally - the pawn interacts with dispreferred pawns freely, but only prefers formal relationships rarely.
        /// Bisexual - no limit to either male or female relationships.

        //Whether a pawn can be attracted to men at all.
        public static bool IsAndrophilic(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                if (pawn.gender == Gender.Female && kinsey < 6) 
                    {
                    return true;
                    }
                if (pawn.gender == Gender.Male && kinsey > 0)
                    {
                    return true;
                    }
            }
            else
            {
                if (pawn.gender == Gender.Male && pawn.story.traits.HasTrait(TraitDefOf.Gay))
                {
                    return true;
                }
                if (pawn.gender == Gender.Female && pawn.story.traits.HasTrait(TraitDefOf.Gay) == false)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsWeaklyAndrophilic(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                if (pawn.gender == Gender.Female && kinsey == 5)
                {
                    return true;
                }
                if (pawn.gender == Gender.Male && kinsey == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsOccasionallyAndrophilic(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                if (pawn.gender == Gender.Female && kinsey == 4)
                {
                    return true;
                }
                if (pawn.gender == Gender.Male && kinsey == 2)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsOccasionallyGynephilic(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                if (pawn.gender == Gender.Female && kinsey == 2)
                {
                    return true;
                }
                if (pawn.gender == Gender.Male && kinsey == 4)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsWeaklyGynephilic(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                if (pawn.gender == Gender.Female && kinsey == 1)
                {
                    return true;
                }
                if (pawn.gender == Gender.Male && kinsey == 5)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsExclusivelyAndrophilic(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                if (pawn.gender == Gender.Female && kinsey == 0)
                {
                    return true;
                }
                if (pawn.gender == Gender.Male && kinsey == 6)
                {
                    return true;
                }
            }
            else
            {
                if (pawn.gender == Gender.Male && pawn.story.traits.HasTrait(TraitDefOf.Gay))
                {
                    return true;
                }
                if (pawn.gender == Gender.Female && pawn.story.traits.HasTrait(TraitDefOf.Gay) == false)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsGynephilic(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                if (pawn.gender == Gender.Male && kinsey < 6)
                {
                    return true;
                }
                if (pawn.gender == Gender.Female && kinsey > 0)
                {
                    return true;
                }
            }
            else
            {
                if (pawn.gender == Gender.Female && pawn.story.traits.HasTrait(TraitDefOf.Gay))
                {
                    return true;
                }
                if (pawn.gender == Gender.Male && pawn.story.traits.HasTrait(TraitDefOf.Gay) == false)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsExclusivelyGynephilic(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                if (pawn.gender == Gender.Female && kinsey == 6)
                {
                    return true;
                }
                if (pawn.gender == Gender.Male && kinsey == 0)
                {
                    return true;
                }
            }
            else
            {
                if (pawn.gender == Gender.Female && pawn.story.traits.HasTrait(TraitDefOf.Gay))
                {
                    return true;
                }
                if (pawn.gender == Gender.Male && pawn.story.traits.HasTrait(TraitDefOf.Gay) == false)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool WouldConsiderFormalRelationship(Pawn pawn, Pawn other)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                if (other.gender == Gender.Male)
                {
                    int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                    if (pawn.gender == Gender.Female && kinsey >= 5)
                    {
                        return false;
                    }
                    if (pawn.gender == Gender.Male && kinsey <= 1)
                    {
                        return false;
                    }
                }
                else
                {
                    int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                    if (pawn.gender == Gender.Female && kinsey <= 1)
                    {
                        return false;
                    }
                    if (pawn.gender == Gender.Male && kinsey >= 5)
                    {
                        return false;
                    }
                }

            }
            else
            {
                if (other.gender == Gender.Male)
                {
                    if (pawn.gender == Gender.Female && pawn.story.traits.HasTrait(TraitDefOf.Gay))
                    {
                        return false;
                    }
                    if (pawn.gender == Gender.Male && pawn.story.traits.HasTrait(TraitDefOf.Gay) == false)
                    {
                        return false;
                    }

                }
                else
                {
                    if (pawn.gender == Gender.Male && pawn.story.traits.HasTrait(TraitDefOf.Gay))
                    {
                        return false;
                    }
                    if (pawn.gender == Gender.Female && pawn.story.traits.HasTrait(TraitDefOf.Gay) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsBisexual(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                if (kinsey > 0 && kinsey < 6)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsStronglyBisexual (Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                if (kinsey > 1 && kinsey < 5)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsWeaklyBisexual(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                int kinsey = PsycheHelper.Comp(pawn).Sexuality.kinseyRating;
                if (kinsey == 1 || kinsey == 5)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAromantic(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn))
            {
                return (PsycheHelper.Comp(pawn).Sexuality.AdjustedRomanticDrive < 0.01f);
            }
            return false;
        }

        public static bool IsAsexual(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn))
            {
                return (PsycheHelper.Comp(pawn).Sexuality.AdjustedSexDrive < 0.01f);
            }
            return false;
        }



        public static bool IsSociallyIncompetent(Pawn pawn)
        {
            return (pawn.story.WorkTagIsDisabled(WorkTags.Social) || pawn.skills.GetSkill(SkillDefOf.Social).Level < 2);
        }

        // ATTRACTION FACTOR TOOLS //

        public static float CalculateFactor(Pawn observer, Pawn assessed, AttractionFactorDef attractionFactor)
        {
            AttractionCalculator calculator = attractionFactor.calculator;
            if (attractionFactor.requiredTraits.Count() > 0)
            {
                bool hasRequiredTrait = false;
                foreach (TraitDef trait in attractionFactor.requiredTraits)
                {
                    if (observer.story.traits.HasTrait(trait))
                    {
                        hasRequiredTrait = true;
                        break;
                    }
                    if (hasRequiredTrait == false)
                    {
                        return 1f;
                    }
                }
            }
            if (attractionFactor.nullifyingTraits.Count() > 0)
            {
                bool hasNullifyingTraits = false;
                foreach (TraitDef trait in attractionFactor.nullifyingTraits)
                {
                    if (observer.story.traits.HasTrait(trait))
                    {
                        hasNullifyingTraits = true;
                        break;
                    }
                    if (hasNullifyingTraits == true)
                    {
                        return 1f;
                    }
                }
            }
            if (!calculator.Check(observer,assessed))
            {
                return 1f;
            }
            float value = calculator.Calculate(observer, assessed);

            if (value == 1f)
            {
                return 1f;
            }

            if (value == 0f)
            {
                return 0f;
            }

            if (attractionFactor.reversingTraits.Count() > 0)
            {
                bool hasReversingTraits = false;
                foreach (TraitDef trait in attractionFactor.reversingTraits)
                {
                    if (observer.story.traits.HasTrait(trait))
                    {
                        hasReversingTraits = true;
                        break;
                    }
                    if (hasReversingTraits == true)
                    {
                        if (value < 1)
                        {
                            value = Mathf.Max(Mathf.Lerp(2.5f, 1f, value), 0.05f);
                        }
                        else
                        {
                            value = Mathf.Pow(value, (value - (2 * value)));
                        }
                    }
                }
            }

            if (attractionFactor.traitImportanceModifiers.Count() > 0)
            {
                foreach (TraitModifier traitMod in attractionFactor.traitImportanceModifiers)
                {
                    if (observer.story.traits.HasTrait(traitMod.trait))
                    {
                        if (traitMod.modifier == 0)
                        {
                            return 0f;
                        }
                        else
                        {
                            value = Mathf.Pow(value, traitMod.modifier);
                        }
                        
                    }
                }
            }

            if (attractionFactor.personalityImportanceModifiers.Count() > 0 && PsycheHelper.PsychologyEnabled(observer) && GradualRomanceMod.AttractionCalculation == GradualRomanceMod.AttractionCalculationSetting.Complex)
            {
                foreach (PersonalityNodeModifier perMod in attractionFactor.personalityImportanceModifiers)
                {
                    if (perMod.reverse == false)
                    {
                        value = Mathf.Pow(value, Mathf.Lerp(0.5f, 1.5f, PsycheHelper.Comp(observer).Psyche.GetPersonalityRating(perMod.personalityNode)) * perMod.modifier);
                    }
                    else
                    {
                        value = Mathf.Pow(value, Mathf.Lerp(0.5f, 1.5f, 1f - PsycheHelper.Comp(observer).Psyche.GetPersonalityRating(perMod.personalityNode)) * perMod.modifier);
                    }
                }
            }

            if (value < 1)
            {
                if (attractionFactor.lowAttractionTraitModifiers.Count() > 0)
                {
                    foreach (TraitModifier traitMod in attractionFactor.lowAttractionTraitModifiers)
                    {
                        if (observer.story.traits.HasTrait(traitMod.trait))
                        {
                            if (traitMod.modifier == 0)
                            {
                                return 0f;
                            }
                            else
                            {
                                value = Mathf.Pow(value, traitMod.modifier);
                            }
                        }
                    }
                }
                if (attractionFactor.lowAttractionPersonalityModifiers.Count() > 0 && PsycheHelper.PsychologyEnabled(observer) && GradualRomanceMod.AttractionCalculation == GradualRomanceMod.AttractionCalculationSetting.Complex)
                {
                    foreach (PersonalityNodeModifier perMod in attractionFactor.lowAttractionPersonalityModifiers)
                    {
                        if (perMod.reverse == false)
                        {
                            value = Mathf.Pow(value, Mathf.Lerp(0.5f, 1.5f, PsycheHelper.Comp(observer).Psyche.GetPersonalityRating(perMod.personalityNode)) * perMod.modifier);
                        }
                        else
                        {
                            value = Mathf.Pow(value, Mathf.Lerp(0.5f, 1.5f, 1f - PsycheHelper.Comp(observer).Psyche.GetPersonalityRating(perMod.personalityNode)) * perMod.modifier);
                        }
                    }
                }
            }
            else
            {
                if (attractionFactor.highAttractionTraitModifiers.Count() > 0)
                {
                    foreach (TraitModifier traitMod in attractionFactor.highAttractionTraitModifiers)
                    {
                        if (observer.story.traits.HasTrait(traitMod.trait))
                        {
                            if (traitMod.modifier == 0)
                            {
                                return 0f;
                            }
                            else
                            {
                                value = Mathf.Pow(value, traitMod.modifier);
                            }
                        }
                    }
                }
                if (attractionFactor.highAttractionPersonalityModifiers.Count() > 0 && PsycheHelper.PsychologyEnabled(observer) && GradualRomanceMod.AttractionCalculation == GradualRomanceMod.AttractionCalculationSetting.Complex)
                {
                    foreach (PersonalityNodeModifier perMod in attractionFactor.highAttractionPersonalityModifiers)
                    {
                        if (perMod.reverse == false)
                        {
                            value = Mathf.Pow(value, Mathf.Lerp(0.5f, 1.5f, PsycheHelper.Comp(observer).Psyche.GetPersonalityRating(perMod.personalityNode)) * perMod.modifier);
                        }
                        else
                        {
                            value = Mathf.Pow(value, Mathf.Lerp(0.5f, 1.5f, 1f - PsycheHelper.Comp(observer).Psyche.GetPersonalityRating(perMod.personalityNode)) * perMod.modifier);
                        }
                    }
                }
            }

            return Mathf.Clamp(value,attractionFactor.minFactor, attractionFactor.maxFactor);
        }

        public static float CalculateAttractionCategory (AttractionFactorCategoryDef category, Pawn observer, Pawn assessed, out List<AttractionFactorDef> veryLowFactors, out List<AttractionFactorDef> lowFactors, out List<AttractionFactorDef> highFactors, out List<AttractionFactorDef> veryHighFactors, out AttractionFactorDef reasonForInstantFailure)
        {
            //Log.Message("Method start.");
            //Log.Message("Making null values.");
            veryHighFactors = new List<AttractionFactorDef>() { };
            highFactors = new List<AttractionFactorDef>() { };
            lowFactors = new List<AttractionFactorDef>() { };
            veryLowFactors = new List<AttractionFactorDef>() { };
            reasonForInstantFailure = null;
            //Log.Message("Retrieving factor defs.");
            IEnumerable<AttractionFactorDef> allFactors = (from def in DefDatabase<AttractionFactorDef>.AllDefsListForReading
                                                   where def.category == category
                                                   select def);

            float attraction = 1f;
            //Log.Message("Starting factor calculations for " + allFactors.Count().ToString() + "factors");

            foreach (AttractionFactorDef factor in allFactors)
            {
                if (factor.calculator.Check(observer,assessed) == false)
                {
                    continue;
                }

                //Log.Message("Doing calculation for " + factor.defName);
                float result = CalculateFactor(observer, assessed, factor);
                if (result == 1f)
                {
                    continue;
                }
                if (result == 0f)
                {
                    if (GradualRomanceMod.detailedAttractionLogs)
                    {
                        Log.Message("Instantly failed at " + factor.defName);
                    }
                    veryLowFactors.Add(factor);
                    reasonForInstantFailure = factor;
                    return 0f;
                }
                if (factor.hidden)
                {
                    continue;
                }
                //Log.Message("Sort factor into results.");
                if (result >= veryHighReasonThreshold)
                {
                    veryHighFactors.Add(factor);
                }
                else if (result >= highReasonThreshold)
                {
                    highFactors.Add(factor);
                }
                else if (result <= veryLowReasonThreshold)
                {
                    veryLowFactors.Add(factor);
                }
                else if (result <= lowReasonThreshold)
                {
                    lowFactors.Add(factor);
                }
                //Log.Message("Integrating result.");
                if (GradualRomanceMod.detailedAttractionLogs)
                {
                    Log.Message(factor.defName + "(" + observer.Name.ToStringShort + "," + assessed.Name.ToStringShort + ")(" + category.defName + "): " + result.ToString() + "=>" + (attraction * result).ToString());
                }
                
                attraction *= result;
            }
            //Log.Message("Concluding method.");
            return attraction;

        }

        public static float CalculateAttraction(Pawn observer, Pawn assessed, bool attractionOnly, bool formalRelationship)
        {
            List<AttractionFactorDef> trashdump = new List<AttractionFactorDef>();
            return CalculateAttraction(observer, assessed, attractionOnly, formalRelationship, out trashdump, out trashdump, out trashdump, out trashdump, out AttractionFactorDef whoCares);
        }

        public static float CalculateAttraction(Pawn observer, Pawn assessed, bool attractionOnly, bool formalRelationship, out List<AttractionFactorDef> veryLowFactors, out List<AttractionFactorDef> lowFactors, out List<AttractionFactorDef> highFactors, out List<AttractionFactorDef> veryHighFactors, out AttractionFactorDef reasonForInstantFailure)
        {
            veryLowFactors = new List<AttractionFactorDef>() { };
            lowFactors = new List<AttractionFactorDef>() { };
            highFactors = new List<AttractionFactorDef>() { };
            veryHighFactors = new List<AttractionFactorDef>() { };
            reasonForInstantFailure = null;
            float result = 1f;
            List<AttractionFactorCategoryDef> allCategories = DefDatabase<AttractionFactorCategoryDef>.AllDefsListForReading;
            
            foreach (AttractionFactorCategoryDef category in allCategories)
            {
                if (!formalRelationship && category.onlyForRomance)
                {
                    continue;
                }
                if (attractionOnly && category.chanceOnly)
                {
                    continue;
                }
                result *= AttractionUtility.CalculateAttractionCategory(category, observer, assessed, out List<AttractionFactorDef> newVeryLowFactors, out List<AttractionFactorDef> newLowFactors, out List<AttractionFactorDef> newHighFactors, out List<AttractionFactorDef> newVeryHighFactors, out AttractionFactorDef newReasonForInstantFailure);

                veryHighFactors.AddRange(newVeryHighFactors);
                highFactors.AddRange(newHighFactors);
                lowFactors.AddRange(newLowFactors);
                veryLowFactors.AddRange(newVeryLowFactors);
                if (reasonForInstantFailure == null && newReasonForInstantFailure != null)
                {
                    reasonForInstantFailure = newReasonForInstantFailure;
                }

            }

            return result;
        }

        public static bool QuickCheck(Pawn observer, Pawn assessed)
        {
            if (!IsAgeAppropriate(observer) && !IsAgeAppropriate(assessed))
            {
                return false;
            }
            if (assessed.gender == Gender.Male && IsExclusivelyGynephilic(observer))
            {
                return false;
            }
            if (assessed.gender == Gender.Female && IsExclusivelyAndrophilic(observer))
            {
                return false;
            }
            return true;
        }

        private const float veryHighReasonThreshold = 1.75f;
        private const float highReasonThreshold = 1.2f;
        private const float lowReasonThreshold = 0.8f;
        private const float veryLowReasonThreshold = 0.25f;

        public static string WriteReasonsParagraph(Pawn observer, Pawn assessed, List<AttractionFactorDef> veryHighFactors, List<AttractionFactorDef> highFactors, List<AttractionFactorDef> lowFactors, List<AttractionFactorDef> veryLowFactors)
        {
            StringBuilder paragraph = new StringBuilder();
            paragraph.AppendLine();
            paragraph.Append("WhatILike".Translate(observer.Named("PAWN1"), assessed.Named("PAWN2")));
            List<string> positiveFactors = new List<string>() { };
            List<string> negativeFactors = new List<string>() { };
            foreach (AttractionFactorDef factor in veryHighFactors)
            {
                string newString = factor.reasonVeryHigh.Formatted(observer.Named("PAWN1"), assessed.Named("PAWN2"));
                newString.UncapitalizeFirst();
                positiveFactors.Add(newString);
            }
            foreach (AttractionFactorDef factor in highFactors)
            {
                string newString = factor.reasonHigh.Formatted(observer.Named("PAWN1"), assessed.Named("PAWN2"));
                newString.UncapitalizeFirst();
                positiveFactors.Add(newString);
            }
            foreach (AttractionFactorDef factor in veryLowFactors)
            {
                string newString = factor.reasonVeryLow.Formatted(observer.Named("PAWN1"), assessed.Named("PAWN2"));
                newString.UncapitalizeFirst();
                negativeFactors.Add(newString);
            }
            foreach (AttractionFactorDef factor in lowFactors)
            {
                string newString = factor.reasonLow.Formatted(observer.Named("PAWN1"), assessed.Named("PAWN2"));
                newString.UncapitalizeFirst();
                negativeFactors.Add(newString);
            }
            if (positiveFactors.Count() == 0)
            {
                paragraph.Append("NothingInParticular".Translate());
            }
            else
            {
                paragraph.Append(GRGrammarUtility.SayList(positiveFactors));
            }
            if (negativeFactors.Count() != 0)
            {
                paragraph.Append(", " + "WhatIDislike".Translate(observer.Named("PAWN1"), assessed.Named("PAWN2")));
                paragraph.Append(GRGrammarUtility.SayList(negativeFactors));
                paragraph.Append(".");
            }
            else
            {
                paragraph.Append(".");

            }

            return paragraph.ToString();
        }


        // RELATION TOOLS //
        public static int GetRelationshipUnmodifiedOpinion(Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || pawn == other)
            {
                return 0;
            }
            if (pawn.Dead)
            {
                return 0;
            }
            int num = 0;
            if (pawn.RaceProps.Humanlike)
            {
                num += pawn.needs.mood.thoughts.TotalOpinionOffset(other);
            }
            if (num != 0)
            {
                float num2 = 1f;
                List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
                for (int i = 0; i < hediffs.Count; i++)
                {
                    if (hediffs[i].CurStage != null)
                    {
                        num2 *= hediffs[i].CurStage.opinionOfOthersFactor;
                    }
                }
                num = Mathf.RoundToInt((float)num * num2);
            }
            if (num > 0 && pawn.HostileTo(other))
            {
                num = 0;
            }
            return Mathf.Clamp(num, -100, 100);
        }

        public static float RelationshipStress(Pawn initiator, Pawn recipient, float modifier = 1f)
        {
            //Psychopaths are cold-hearted
            if (initiator.story.traits.HasTrait(TraitDefOf.Psychopath))
            {
                //But socially incompetent will still embarrass themselves
                if (IsSociallyIncompetent(initiator))
                {
                    return 1f;
                }
                else
                {
                    return 0.05f;
                }

            }
            //Lechers are used to hitting on people.

            float pressure = modifier;
            if (initiator.story.traits.HasTrait(TraitDefOfPsychology.Lecher))
            {
                pressure *= 0.5f;
            }
            int opinionDifference = initiator.relations.OpinionOf(recipient) - recipient.relations.OpinionOf(initiator);
            pressure *= Mathf.Clamp(Mathf.InverseLerp(0f, PressureStartsAtOpinionDifference, opinionDifference), 0.05f, 5f);
            //People with low mental break thresholds freak out more and say the wrong thing. 
            pressure *= Mathf.InverseLerp(0f, 0.05f, initiator.GetStatValue(StatDefOf.MentalBreakThreshold));
            //People with high social skill are less likely to pick bad flirts.
            float socialSkillFactor = Mathf.Clamp(Mathf.InverseLerp(4, 16, Mathf.Abs(initiator.skills.GetSkill(SkillDefOf.Social).Level - 20)), 0.01f, 2f);
            pressure *= socialSkillFactor;
            return Mathf.Clamp(pressure, 0.2f, 5f);
        }

        ///////SOCIAL ATTRACTIVENESS////////

        public static float GetAttractivenessForSkillLevel(int level, SkillDef skill)
        {
            return SkillAttractivenessCurve.Evaluate(level);
        }

        public static float GetObjectiveSkillAttractiveness(Pawn pawn)
        {
            float value = 0f;
            IEnumerable<SkillDef> allSkillDefs = DefDatabase<SkillDef>.AllDefsListForReading;
            foreach (SkillDef skill in allSkillDefs)
            {
                value += GetAttractivenessForSkillLevel(pawn.skills.GetSkill(skill).Level, skill);
            }
            return value;
        }

        public static float EvaluateRoomAttractiveness (Room assessedRoom)
        {
            float roomTypeFactor;
            if (assessedRoom.Role == RoomRoleDefOf.Bedroom)
            {
                roomTypeFactor = 1f;
            }
            else if (assessedRoom.Role == RoomRoleDefOf.Barracks)
            {
                roomTypeFactor = (1f / assessedRoom.ContainedBeds.Count());
            }
            else
            {
                roomTypeFactor = 0.01f;
            }
            return (assessedRoom.GetStat(RoomStatDefOf.Wealth) * roomTypeFactor);

        }

        public static float GetObjectiveWealthAttractiveness(Pawn pawn)
        {
            Room observerRoom = pawn.ownership.OwnedRoom;
            if (observerRoom == null)
            {
                return 1f;
            }
            else
            {
                return EvaluateRoomAttractiveness(observerRoom);
            }
        }



        private const float YoungAge = 10f;

        private const float AdultAge = 18f;

        private const float OldAge = 50f;

        private const float ElderAge = 65f;

        private const float AncientAge = 90f;

        private const float MinAge = 18f;

        private const float GenderedAgePreference = 0.6f;
        private const float GreedWealthPreference = 1.5f;
        private const float PressureStartsAtOpinionDifference = 15f;

        private const float SkillAttractivenessDampenerUpper = 0.5f;

        private const float SkillAttractivenessDampenerLower = 0.2f;

        private const float FriendAttractionDampener = 0.4f;

        private const float MaleSkillEvaluationFactor = 0.5f;
        private const float MaleBeautyEvaluationFactor = 1.5f;

        /*
        private const float MinScarAttraction = 0.95f;
        private const float UpperScarAttraction = 0.9f;
        private const float LowerScarAttraction = 0.7f;
        */

    }
}
