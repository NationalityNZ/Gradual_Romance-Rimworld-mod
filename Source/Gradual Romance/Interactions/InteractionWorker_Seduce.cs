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
    public class InteractionWorker_Seduce : InteractionWorker
    {

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
        private float initiatorAttraction = 1f;
        private float recipientAttraction = 1f;
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

        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            PawnRelationDef pawnRelation = RelationshipUtility.MostAdvancedRelationshipBetween(initiator, recipient);
            if (!AttractionUtility.QuickCheck(initiator, recipient))
            {
                return 0f;
            }
            if (GradualRomanceMod.SeductionMode == GradualRomanceMod.SeductionModeSetting.NoSeduction)
            {
                return 0f;
            }
            else if (GradualRomanceMod.SeductionMode == GradualRomanceMod.SeductionModeSetting.OnlyRelationship)
            {
                if (pawnRelation == null || !RelationshipUtility.IsSexualRelationship(pawnRelation))
                {
                    return 0f;
                }

            }
            else if (GradualRomanceMod.SeductionMode == GradualRomanceMod.SeductionModeSetting.RelationshipAndNonColonists)
            {
                if (pawnRelation == null && recipient.IsColonist)
                {
                    return 0f;
                }
                if (pawnRelation != null && !RelationshipUtility.IsSexualRelationship(pawnRelation) && recipient.IsColonist)
                {
                    return 0f;
                }
            }

            //shouldn't seduce if you can't move
            if (initiator.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness) <= 0.5f || initiator.health.capacities.GetLevel(PawnCapacityDefOf.Moving) <= 0.25f)
            {
                return 0f;
            }
            if (recipient.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness) <= 0.5f || recipient.health.capacities.GetLevel(PawnCapacityDefOf.Moving) <= 0.25f)
            {
                return 0f;
            }
            //shouldn't seduce while working
            TimeAssignmentDef initiatorAssignment = initiator.timetable.GetAssignment(GenLocalDate.HourOfDay(initiator.Map));
            TimeAssignmentDef recipientAssignment = recipient.timetable.GetAssignment(GenLocalDate.HourOfDay(recipient.Map));
            if (initiatorAssignment != TimeAssignmentDefOf.Joy || initiatorAssignment != TimeAssignmentDefOf.Anything)
            {
                return 0f;
            }
            if (recipientAssignment != TimeAssignmentDefOf.Joy || recipientAssignment != TimeAssignmentDefOf.Anything)
            {
                return 0f;
            }
            EmptyReasons();
            AttractionFactorDef whoCares;
            initiatorAttraction = AttractionUtility.CalculateAttraction(initiator, recipient, false, false, out veryLowInitiatorReasons, out lowInitiatorReasons, out highInitiatorReasons, out veryHighInitiatorReasons, out whoCares);
            float tensionFactor = 1.33f * RelationshipUtility.LevelOfSexualTension(initiator,recipient);
            tensionFactor = Mathf.Max(1f, tensionFactor);
            lastInitiator = initiator;
            lastRecipient = recipient;
            return GradualRomanceMod.BaseSeductionChance * initiatorAttraction * tensionFactor * AttractionUtility.PropensityToSeduce(initiator);
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
            AttractionFactorDef whoCares;
            if (lastInitiator != initiator || lastRecipient != recipient)
            {
                EmptyReasons();
                initiatorAttraction = AttractionUtility.CalculateAttraction(initiator, recipient, false, false, out veryLowInitiatorReasons, out lowInitiatorReasons, out highInitiatorReasons, out veryHighInitiatorReasons, out whoCares);
            }
            
            pressureCache = AttractionUtility.RelationshipStress(initiator, recipient);


            letterText = null;
            letterLabel = null;
            letterDef = null;
            /*
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
                
                
            }
            */
        }


        private static void LogFlirt(string message)
        {
            if (GradualRomanceMod.detailedDebugLogs == true)
            {
                Log.Message(message);
            }
        }

        private static float pressureCache;

        private const float MinAttractionForRomanceAttempt = 0.25f;

        private const int MinOpinionForRomanceAttempt = 5;

        private const float BaseSuccessChance = 1f;

        private const float BaseFlirtWeight = 0.4f;

        private const float GoodFlirtBonus = 1.5f;

        private const float BadFlirtPenalty = 0.6f;

        private const float FamiliarityFactor = 0.5f;

    }
}