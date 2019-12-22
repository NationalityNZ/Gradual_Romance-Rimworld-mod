using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;
using Psychology;
using System.Linq;
using HugsLib;
using Verse.Grammar;

namespace Gradual_Romance
{
    public class InteractionWorker_Flirt : InteractionWorker
    {
        private float recipientPhysicalAttraction;
        private float recipientSocialAttraction;
        private float recipientRomanticAttraction;
        private float initiatorPhysicalAttraction;
        private float initiatorSocialAttraction;
        private float initiatorRomanticAttraction;
        private float initiatorCircumstances;
        private float recipientCircumstances;
        private List<AttractionFactorDef> veryHighInitiatorReasons = new List<AttractionFactorDef>() { };
        private List<AttractionFactorDef> highInitiatorReasons = new List<AttractionFactorDef>() { };
        private List<AttractionFactorDef> lowInitiatorReasons = new List<AttractionFactorDef>() { };
        private List<AttractionFactorDef> veryLowInitiatorReasons = new List<AttractionFactorDef>() { };
        private List<AttractionFactorDef> veryHighRecipientReasons = new List<AttractionFactorDef>() { };
        private List<AttractionFactorDef> highRecipientReasons = new List<AttractionFactorDef>() { };
        private List<AttractionFactorDef> lowRecipientReasons = new List<AttractionFactorDef>() { };
        private List<AttractionFactorDef> veryLowRecipientReasons = new List<AttractionFactorDef>() { };
        private List<AttractionFactorDef> intiatorFailureReasons = new List<AttractionFactorDef>() { };
        private List<AttractionFactorDef> recipientFailureReasons = new List<AttractionFactorDef>() { };
        private Pawn lastInitiator = null;
        private Pawn lastRecipient = null;
        private bool successImpossible = false;


        private void EmptyReasons()
        {
            veryHighInitiatorReasons.Clear();
            highInitiatorReasons.Clear();
            lowInitiatorReasons.Clear();
            veryLowInitiatorReasons.Clear();
            veryHighRecipientReasons.Clear();
            highRecipientReasons.Clear();
            lowRecipientReasons.Clear();
            veryLowRecipientReasons.Clear();
            intiatorFailureReasons.Clear();
            recipientFailureReasons.Clear();
        }

        private float CalculateAndSort(AttractionFactorCategoryDef category, Pawn observer, Pawn assessed, bool observerIsInitiator = true)
        {
            float result = AttractionUtility.CalculateAttractionCategory(category, observer, assessed, out List<AttractionFactorDef> veryLowFactors, out List<AttractionFactorDef> lowFactors, out List<AttractionFactorDef> hightFactors, out List<AttractionFactorDef> veryHighFactors, out AttractionFactorDef reasonForInstantFailure);
            if (observerIsInitiator)
            {
                veryHighInitiatorReasons.AddRange(veryHighFactors);
                highInitiatorReasons.AddRange(hightFactors);
                lowInitiatorReasons.AddRange(lowFactors);
                veryLowInitiatorReasons.AddRange(veryLowFactors);
                intiatorFailureReasons.Add(reasonForInstantFailure);
            }
            else
            {
                veryHighRecipientReasons.AddRange(veryHighFactors);
                highRecipientReasons.AddRange(hightFactors);
                lowRecipientReasons.AddRange(lowFactors);
                veryLowRecipientReasons.AddRange(veryLowFactors);
                recipientFailureReasons.Add(reasonForInstantFailure);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="recipient"></param>
        /// <returns></returns>
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {

            if (!AttractionUtility.QuickCheck(initiator, recipient))
            {
                return 0f;
            }
            EmptyReasons();
            AttractionFactorDef whoCares;
            float currentAttraction = AttractionUtility.CalculateAttraction(initiator, recipient, false, false, out veryLowInitiatorReasons, out lowInitiatorReasons, out highInitiatorReasons, out veryHighInitiatorReasons, out whoCares);
            recipientPhysicalAttraction = GRHelper.GRPawnComp(initiator).RetrieveAttractionForCategory(recipient, AttractionFactorCategoryDefOf.Physical);
            recipientRomanticAttraction = GRHelper.GRPawnComp(initiator).RetrieveAttractionForCategory(recipient, AttractionFactorCategoryDefOf.Romantic);
            recipientSocialAttraction = GRHelper.GRPawnComp(initiator).RetrieveAttractionForCategory(recipient, AttractionFactorCategoryDefOf.Social);

            //initiatorCircumstances = CalculateAndSort(AttractionFactorCategoryDefOf.Circumstance, initiator, recipient);
            //if (intiatorFailureReasons.Count() > 0)
            //{
            //    EmptyReasons();
            //    return 0f;
            //}
            float flirtFactor = 0.5f;

            List<Thought_Memory> memoryList = initiator.needs.mood.thoughts.memories.Memories;

            for (int i = 0; i < memoryList.Count; i++)
            {
                Thought_Memory curMemory = memoryList[i];
                if (curMemory.def == ThoughtDefOfGR.RomanticDisinterest && curMemory.otherPawn == recipient)
                {
                    flirtFactor = flirtFactor * BadFlirtPenalty;
                }
            }
            flirtFactor = Mathf.Max(flirtFactor, 0.05f);
            lastInitiator = initiator;
            lastRecipient = recipient;
            return GradualRomanceMod.BaseFlirtChance * currentAttraction * flirtFactor * BaseFlirtWeight;
        }
        /*
        public float SuccessChance(Pawn initiator, Pawn recipient)
        {
            float recipientAttraction = recipient.relations.SecondaryRomanceChanceFactor(initiator);
            return 1f;
        }
        */
        //
        //allDefsListForReading.TryRandomElementByWeight((InteractionDef x) => x.Worker.RandomSelectionWeight(this.pawn, p), out intDef)

        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef)
        {
            if (lastInitiator != initiator || lastRecipient != recipient)
            {
                EmptyReasons(); 
                recipientPhysicalAttraction = CalculateAndSort(AttractionFactorCategoryDefOf.Physical, initiator, recipient);
                recipientRomanticAttraction = CalculateAndSort(AttractionFactorCategoryDefOf.Romantic, initiator, recipient);
                recipientSocialAttraction = CalculateAndSort(AttractionFactorCategoryDefOf.Social, initiator, recipient);
                initiatorCircumstances = CalculateAndSort(AttractionFactorCategoryDefOf.Circumstance, initiator, recipient);
            }
            AttractionFactorDef whoCares;
            /*
            initiatorPhysicalAttraction = CalculateAndSort(AttractionFactorCategoryDefOf.Physical, recipient, initiator, false);
            initiatorRomanticAttraction = CalculateAndSort(AttractionFactorCategoryDefOf.Romantic, recipient, initiator, false);
            initiatorSocialAttraction = CalculateAndSort(AttractionFactorCategoryDefOf.Social, recipient, initiator, false);
            recipientCircumstances = CalculateAndSort(AttractionFactorCategoryDefOf.Circumstance, recipient, initiator, false);
            */
            recipientCircumstances = AttractionUtility.CalculateAttractionCategory(AttractionFactorCategoryDefOf.Circumstance, recipient, initiator);
            float totalAttraction = AttractionUtility.CalculateAttraction(recipient, initiator, false, false, out veryLowRecipientReasons, out lowRecipientReasons, out highRecipientReasons, out veryHighRecipientReasons, out whoCares);
            initiatorPhysicalAttraction = GRHelper.GRPawnComp(recipient).RetrieveAttractionForCategory(initiator, AttractionFactorCategoryDefOf.Physical);
            initiatorRomanticAttraction = GRHelper.GRPawnComp(recipient).RetrieveAttractionForCategory(initiator, AttractionFactorCategoryDefOf.Romantic);
            initiatorSocialAttraction = GRHelper.GRPawnComp(recipient).RetrieveAttractionForCategory(initiator, AttractionFactorCategoryDefOf.Social);
            LogFlirt(initiator.Name.ToStringShort + "=>" + recipient.Name.ToStringShort + " attraction: physical " + recipientPhysicalAttraction.ToString() + ", romantic " + recipientRomanticAttraction.ToString() + ", social " + recipientSocialAttraction.ToString() + ".");
            List<FlirtStyleDef> allDefsListForReading = DefDatabase<FlirtStyleDef>.AllDefsListForReading;
            FlirtStyleDef flirtStyle;
            pressureCache = AttractionUtility.RelationshipStress(initiator, recipient);
            allDefsListForReading.TryRandomElementByWeight((FlirtStyleDef x) => CalculateFlirtStyleWeight(x, initiator, recipient), out flirtStyle);
            if (flirtStyle == null)
            {
                Log.Error("FailedToFindFlirt_error".Translate());
                letterText = null;
                letterLabel = null;
                letterDef = null;
                return;
            }
            if (veryHighInitiatorReasons.Count() > 0)
            {
                AttractionFactorDef reason = veryHighInitiatorReasons.RandomElement<AttractionFactorDef>();
                extraSentencePacks.Add(reason.intriguedByText);
            }
            else if (highInitiatorReasons.Count() > 0)
            {
                AttractionFactorDef reason = highInitiatorReasons.RandomElement<AttractionFactorDef>();
                extraSentencePacks.Add(reason.intriguedByText);
            }
            if (recipient.gender == Gender.Male)
            {
                extraSentencePacks.Add(flirtStyle.rulePackMale);
            }
            if (recipient.gender == Gender.Female)
            {
                extraSentencePacks.Add(flirtStyle.rulePackFemale);
            }
            LogFlirt("Flirt chosen: " + flirtStyle.defName + ".");
            LogFlirt(recipient.Name.ToStringShort + "=>" + initiator.Name.ToStringShort + " attraction: physical " + initiatorPhysicalAttraction.ToString() + ", romantic " + initiatorRomanticAttraction.ToString() + ", social " + initiatorSocialAttraction.ToString() + ".");
            
            if (initiatorPhysicalAttraction == 0f||initiatorRomanticAttraction == 0f||initiatorSocialAttraction == 0f)
            {
                successImpossible = true;
            }
            else
            {
                successImpossible = false;
            }
            FlirtReactionDef flirtReaction = null;
            IEnumerable<FlirtReactionDef> successfulFlirtReactions = (from reaction in DefDatabase<FlirtReactionDef>.AllDefsListForReading
                                                               where reaction.successful
                                                               select reaction);
            IEnumerable<FlirtReactionDef> unsuccessfulFlirtReactions = (from reaction in DefDatabase<FlirtReactionDef>.AllDefsListForReading
                                                                      where !reaction.successful
                                                                      select reaction);
            List < FlirtReactionDef > allFlirtReactions = DefDatabase<FlirtReactionDef>.AllDefsListForReading;
            FlirtReactionDef successfulFlirt;
            FlirtReactionDef unsuccessfulFlirt;
            successfulFlirtReactions.TryRandomElementByWeight((FlirtReactionDef x) => CalculateFlirtReactionWeight(flirtStyle, x, initiator, recipient), out successfulFlirt);
            unsuccessfulFlirtReactions.TryRandomElementByWeight((FlirtReactionDef x) => CalculateFlirtReactionWeight(flirtStyle,x,initiator,recipient), out unsuccessfulFlirt);
            if (successImpossible)
            {
                flirtReaction = unsuccessfulFlirt;
            }
            else
            {
                //revise to include flirt type
                float chance = Mathf.Clamp01(GradualRomanceMod.RomanticSuccessRate * Mathf.Pow(initiatorPhysicalAttraction, flirtStyle.baseSexiness) * Mathf.Pow(initiatorRomanticAttraction, flirtStyle.baseRomance) * Mathf.Pow(initiatorSocialAttraction, flirtStyle.baseLogic) * recipientCircumstances * 0.65f);
                Log.Message("Romance success chance: " + chance.ToString());
                if (Rand.Value < chance)
                {
                    flirtReaction = successfulFlirt;
                }
                else
                {
                    flirtReaction = unsuccessfulFlirt;
                }
                LogFlirt(recipient.Name.ToStringShort + " chose reaction " + flirtReaction.defName + " from Successful: " + successfulFlirt.defName + "; Unsuccessful: " + unsuccessfulFlirt.defName + ".");
            }
            

            if (flirtReaction == null)
            {
                Log.Error("FailedToFindReaction_error".Translate());
                letterText = null;
                letterLabel = null;
                letterDef = null;
                return;
            }

            if (initiator.gender == Gender.Male)
            {
                extraSentencePacks.Add(flirtReaction.maleRulePack);
            }
            if (initiator.gender == Gender.Female)
            {
                extraSentencePacks.Add(flirtReaction.femaleRulePack);
            }
            if (flirtReaction != FlirtReactionDefOf.Ignorant)
            {
                if (flirtReaction.successful)
                {
                    if (veryHighRecipientReasons.Count() > 0)
                    {
                        AttractionFactorDef reason = veryHighRecipientReasons.RandomElement<AttractionFactorDef>();
                        extraSentencePacks.Add(reason.reactionPositiveText);
                    }
                    else if (highRecipientReasons.Count() > 0)
                    {
                        AttractionFactorDef reason = highRecipientReasons.RandomElement<AttractionFactorDef>();
                        extraSentencePacks.Add(reason.reactionPositiveText);
                    }
                }
                else
                {
                    if (veryLowRecipientReasons.Count() > 0)
                    {
                        AttractionFactorDef reason = veryLowRecipientReasons.RandomElement<AttractionFactorDef>();
                        extraSentencePacks.Add(reason.reactionNegativeText);
                    }
                    else if (lowRecipientReasons.Count() > 0)
                    {
                        AttractionFactorDef reason = lowRecipientReasons.RandomElement<AttractionFactorDef>();
                        extraSentencePacks.Add(reason.reactionNegativeText);
                    }
                }
            }

            flirtReaction.worker.GiveThoughts(initiator, recipient, out List<RulePackDef> yetMoreSentencePacks);
            
            extraSentencePacks.AddRange(yetMoreSentencePacks);

            letterText = null;
            letterLabel = null;
            letterDef = null;

            List<Pawn> loversInSight = RelationshipUtility.PotentiallyJealousPawnsInLineOfSight(initiator);
            List<Pawn> loversInSight2 = RelationshipUtility.PotentiallyJealousPawnsInLineOfSight(recipient);

            for (int i = 0; i < loversInSight.Count(); i++)
            {
                if (BreakupUtility.ShouldBeJealous(loversInSight[i],initiator,recipient))
                {
                    loversInSight[i].needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.CaughtFlirting, initiator);
                    if (flirtReaction.successful)
                    {
                        loversInSight[i].needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.CaughtFlirtingWithLover, recipient);
                    }
                }
            }

            if (flirtReaction.successful)
            {
                if (flirtReaction.provokesJealousy)
                {
                    
                    for (int i = 0; i < loversInSight2.Count(); i++)
                    {
                        if (BreakupUtility.ShouldBeJealous(loversInSight2[i], initiator, recipient))
                        {
                            loversInSight2[i].needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.CaughtFlirting, recipient);
                            loversInSight2[i].needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.CaughtFlirtingWithLover, initiator);
                        }
                    }
                }


                RelationshipUtility.AdvanceInformalRelationship(initiator, recipient, out PawnRelationDef newRelation, (flirtStyle.baseSweetheartChance * flirtReaction.sweetheartModifier));
                    
                if (newRelation != null && (PawnUtility.ShouldSendNotificationAbout(initiator) || PawnUtility.ShouldSendNotificationAbout(recipient)))
                {

                    string initiatorParagraph = AttractionUtility.WriteReasonsParagraph(initiator, recipient, veryHighInitiatorReasons, highInitiatorReasons, lowInitiatorReasons, veryLowInitiatorReasons);
                    string recipientParagraph = AttractionUtility.WriteReasonsParagraph(recipient, initiator, veryHighRecipientReasons, highRecipientReasons, lowRecipientReasons, veryLowRecipientReasons);
                    letterDef = LetterDefOf.PositiveEvent;
                    letterLabel = newRelation.GetModExtension<RomanticRelationExtension>().newRelationshipTitleText.Translate();
                    letterText = newRelation.GetModExtension<RomanticRelationExtension>().newRelationshipLetterText.Translate(initiator.Named("PAWN1"), recipient.Named("PAWN2"));


                    letterText += initiatorParagraph;
                    letterText += recipientParagraph;

                    /*    if (newRelation == PawnRelationDefOfGR.Sweetheart)
                        {
                            letterDef = LetterDefOf.PositiveEvent;
                            letterLabel = "NewSweetheartsLabel".Translate();
                            letterText = "NewSweetheartsText".Translate(initiator.Named("PAWN1"), recipient.Named("PAWN2"));
                        }
                        if (newRelation == PawnRelationDefOfGR.Lovebuddy)
                        {
                            letterDef = LetterDefOf.PositiveEvent;
                            letterLabel = "NewLovebuddiesLabel".Translate();
                            letterText = "NewLovebuddiesText".Translate(initiator.Named("PAWN1"), recipient.Named("PAWN2"));
                        }
                        if (newRelation == PawnRelationDefOfGR.Paramour)
                        {
                            if (RelationshipUtility.IsAnAffair(initiator, recipient, out Pawn initiatorSO, out Pawn recipientSO))
                            {
                                letterDef = LetterDefOf.NegativeEvent;
                                letterLabel = "ParamoursAffairLabel".Translate();
                                if (initiatorSO != null && recipientSO != null)
                                {
                                    letterText = "ParamoursAffairTwoCuckoldsText".Translate(initiator.Named("PAWN1"), recipient.Named("PAWN2"), initiatorSO.Named("CUCKOLD1"), recipientSO.Named("CUCKOLD2"));
                                }
                                if (initiatorSO != null && recipientSO == null)
                                {
                                    letterText = "ParamoursAffairInitiatorCuckoldText".Translate(initiator.Named("PAWN1"), recipient.Named("PAWN2"), initiatorSO.Named("CUCKOLD1"));
                                }
                                if (initiatorSO == null && recipientSO != null)
                                {
                                    letterText = "ParamoursAffairRecipientCuckoldText".Translate(initiator.Named("PAWN1"), recipient.Named("PAWN2"), recipientSO.Named("CUCKOLD1"));
                                }
                            }
                            else
                            {
                                letterDef = LetterDefOf.PositiveEvent;
                                letterLabel = "NewParamoursLabel".Translate();
                                letterText = "NewParamoursText".Translate(initiator.Named("PAWN1"), recipient.Named("PAWN2"));
                            }
                        }*/


                }
                
            }

        }



        private float CalculateFlirtStyleWeight(FlirtStyleDef flirtStyle, Pawn pawn, Pawn other)
        {
            string flirtLog = pawn.Name.ToStringShort + " => " + other.Name.ToStringShort + " considers " + flirtStyle.defName + ": ";
            //if a pawn has a canceling trait, we abort immediately
            for (int i = 0; i < flirtStyle.cancelingTraits.Count(); i++)
            {
                if (pawn.story.traits.HasTrait(flirtStyle.cancelingTraits[i]))
                {
                    flirtLog += "canceled by " + flirtStyle.cancelingTraits[i].defName + ".";
                    LogFlirt(flirtLog);
                    return 0f;
                }
            }
            //we start with base weight chance
            float weight = flirtStyle.baseChance;
            
            //add relationship factor
            weight *= RelationshipFactorForFlirtStyle(RelationshipUtility.MostAdvancedRomanceOrExStage(pawn, other),flirtStyle);
            flirtLog += "base " + weight.ToString() + " ";
            //calculate attraction factors
            /*
            weight *= recipientPhysicalAttraction * flirtStyle.baseSexiness;
            flirtLog += "physical " + weight.ToString() + " ";
            weight *= recipientRomanticAttraction * flirtStyle.baseRomance;
            flirtLog += "romantic " + weight.ToString() + " ";
            weight *= recipientSocialAttraction * flirtStyle.baseLogic;
            flirtLog += "logical " + weight.ToString() + " ";
            */


            //calculate promoting traits
            for (int i = 0; i < flirtStyle.traitModifiers.Count(); i++)
            {
                if (pawn.story.traits.HasTrait(flirtStyle.traitModifiers[i].trait))
                {
                    weight = weight * flirtStyle.traitModifiers[i].modifier;
                    flirtLog += flirtStyle.traitModifiers[i].trait.defName + ": " + weight.ToString() + " ";
                }
            }
            if (PsycheHelper.PsychologyEnabled(pawn) && GradualRomanceMod.AttractionCalculation == GradualRomanceMod.AttractionCalculationSetting.Complex)
            {
                //calculate contributing personality traits
                for (int i = 0; i < flirtStyle.moreLikelyPersonalities.Count(); i++)
                {
                    PersonalityNodeModifier currentModifier = flirtStyle.moreLikelyPersonalities[i];
                    weight = weight * Mathf.Pow(Mathf.Lerp(0.5f, 1.5f, PsycheHelper.Comp(pawn).Psyche.GetPersonalityRating(currentModifier.personalityNode)), currentModifier.modifier);
                    flirtLog += currentModifier.personalityNode.defName + "+: " + weight.ToString() + " ";
                }
                for (int i = 0; i < flirtStyle.lessLikelyPersonalities.Count(); i++)
                {
                    PersonalityNodeModifier currentModifier = flirtStyle.lessLikelyPersonalities[i];
                    weight = weight * Mathf.Pow(Mathf.Lerp(0.5f, 1.5f, Mathf.Abs(1 - PsycheHelper.Comp(pawn).Psyche.GetPersonalityRating(currentModifier.personalityNode))), currentModifier.modifier);
                    flirtLog += currentModifier.personalityNode.defName + "-: " + weight.ToString() + " ";
                }
            }
            if (flirtStyle.incompetent)
            {
                weight *= pressureCache;
                flirtLog += "pressure: " + weight.ToString() + " ";
            }

            flirtLog += "end.";
            LogFlirt(flirtLog);
            return weight;
        }
        
        private float CalculateFlirtReactionWeight(FlirtStyleDef flirtStyle, FlirtReactionDef flirtReaction, Pawn initiator, Pawn recipient)
        {

            float chance = GradualRomanceMod.RomanticSuccessRate * flirtReaction.baseChance;
            string log = "Reaction " + flirtReaction.defName + ": base chance " + chance.ToString();

            if (successImpossible && flirtReaction.successful)
            {
                log  += ". Canceled, success impossible.";
                LogFlirt(log);
                return 0f;
            }
            if (successImpossible == false)
            {
                chance *= Mathf.Pow(flirtReaction.sexyReaction * initiatorPhysicalAttraction, flirtStyle.baseSexiness);
                log += " sexiness " + chance.ToString();
                chance *= Mathf.Pow(flirtReaction.romanticReaction * initiatorRomanticAttraction, flirtStyle.baseRomance);
                log += " romance " + chance.ToString();
                chance *= Mathf.Pow(flirtReaction.logicalReaction * initiatorSocialAttraction, flirtStyle.baseLogic);
                log += " logic " + chance.ToString();
            }
            chance *= Mathf.Pow(flirtReaction.obscureReaction, flirtStyle.baseObscurity);
            log += " obscurity " + chance.ToString();
            //risky flirts are less risky if the two pawns are familiar with each other.
            if (RelationshipUtility.MostAdvancedRelationshipBetween(initiator,recipient) == null)
            {
                chance *= Mathf.Pow(flirtReaction.riskyReaction, flirtStyle.baseRiskiness);
            }
            else
            {
                chance *= Mathf.Pow(Mathf.Pow(flirtReaction.riskyReaction, flirtStyle.baseRiskiness), FamiliarityFactor);
            }
            chance *= Mathf.Pow(flirtReaction.riskyReaction, flirtStyle.baseRiskiness);
            log += " riskiness " + chance.ToString();
            chance *= Mathf.Pow(flirtReaction.awkwardReaction, flirtStyle.baseAwkwardness);
            log += " awkward " + chance.ToString() + "; ";

            if (GradualRomanceMod.AttractionCalculation == GradualRomanceMod.AttractionCalculationSetting.Complex)
            {
                for (int i = 0; i < flirtReaction.personalityModifiers.Count(); i++)
                {
                    PersonalityNodeModifier node = flirtReaction.personalityModifiers[i];
                    if (node.modifier >= 0)
                    {
                        chance = chance * Mathf.Pow(Mathf.Lerp(0.5f, 1.5f, PsycheHelper.Comp(recipient).Psyche.GetPersonalityRating(node.personalityNode)), node.modifier);
                        log += node.personalityNode.defName + "+: " + chance.ToString() + ", ";
                    }
                    else
                    {
                        chance = chance * Mathf.Pow(Mathf.Lerp(0.5f, 1.5f, Mathf.Abs(1 - PsycheHelper.Comp(recipient).Psyche.GetPersonalityRating(node.personalityNode))), Mathf.Abs(node.modifier));
                        log += node.personalityNode.defName + "-: " + chance.ToString() + ", ";
                    }

                }
            }
            for (int i = 0; i < flirtReaction.traitModifiers.Count(); i++)
            {
                if (recipient.story.traits.HasTrait(flirtReaction.traitModifiers[i].trait))
                {
                    chance *= flirtReaction.traitModifiers[i].modifier;
                    log += flirtReaction.traitModifiers[i].trait.defName + " " + chance.ToString() + ", ";
                }
            }
            /*
            if (flirtReaction.successful == true)
            {
                chance *= initiator.GetStatValue(StatDefOf.SocialImpact);
                log += "social impact: " + chance.ToString() + ", ";
                //chance *= recipientCircumstances;
            }*/
            log += "end.";
            LogFlirt(log);
            return chance;
           
        }


        private static float RelationshipFactorForFlirtStyle(PawnRelationDef relation, FlirtStyleDef flirtStyle)
        {
            if (relation == PawnRelationDefOf.Spouse || relation == PawnRelationDefOf.ExSpouse)
            {
                return flirtStyle.spouseFactor;
            }
            if (relation == PawnRelationDefOf.Fiance || relation == PawnRelationDefOf.Lover || relation == PawnRelationDefOf.ExLover)
            {
                return flirtStyle.loverFactor;
            }
            if (relation == PawnRelationDefOfGR.Lovefriend || relation == PawnRelationDefOfGR.ExLovefriend || relation == PawnRelationDefOfGR.Paramour)
            {
                return flirtStyle.lovefriendFactor;
            }
            if (relation == PawnRelationDefOfGR.Lovebuddy)
            {
                return flirtStyle.loveBuddyFactor;
            }
            if (relation == PawnRelationDefOfGR.Sweetheart)
            {
                return flirtStyle.sweetheartFactor;
            }
            return flirtStyle.acquaitanceFactor;
        }
        private static void LogFlirt(string message)
        {
            if (GradualRomanceMod.detailedDebugLogs == true)
            {
                Log.Message(message);
            }
        }

        private static float pressureCache;

        //List<InteractionDef> allDefsListForReading = DefDatabase<InteractionDef>.AllDefsListForReading;
        //allDefsListForReading.TryRandomElementByWeight((InteractionDef x) => x.Worker.RandomSelectionWeight(this.pawn, p), out intDef)
        private const float MinAttractionForRomanceAttempt = 0.25f;

        private const int MinOpinionForRomanceAttempt = 5;

        private const float BaseSuccessChance = 1f;

        private const float BaseFlirtWeight = 0.4f;

        private const float GoodFlirtBonus = 1.5f;

        private const float BadFlirtPenalty = 0.6f;

        private const float FamiliarityFactor = 0.5f;

    }
}