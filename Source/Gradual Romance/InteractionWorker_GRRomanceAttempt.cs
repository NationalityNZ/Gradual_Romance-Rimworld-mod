﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;
using Psychology.Harmony;
using UnityEngine;

namespace Gradual_Romance
{
    public class InteractionWorker_GRRomanceAttempt : InteractionWorker
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
        private Pawn lastInitiator = null;
        private Pawn lastRecipient = null;

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

        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            //Don't hit on people in mental breaks... unless you're really freaky.
            if (recipient.InMentalState && PsycheHelper.PsychologyEnabled(initiator) && PsycheHelper.Comp(initiator).Psyche.GetPersonalityRating(PersonalityNodeDefOf.Experimental) < 0.8f)
            {
                return 0f;
            }
            //Pawns will only romance characters with which they have a mild relationship
            if (!GRPawnRelationUtility.HasInformalRelationship(initiator, recipient))
            {
                return 0f;
            }
            //Pawns won't hit on their spouses.
            if (LovePartnerRelationUtility.LovePartnerRelationExists(initiator, recipient))
            {
                return 0f;
            }
            //Codependents won't romance anyone if they are in a relationship
            if (LovePartnerRelationUtility.HasAnyLovePartner(initiator) && initiator.story.traits.HasTrait(TraitDefOfPsychology.Codependent))
            {
                return 0f;
            }
            //Only lechers will romance someone that has less than +5 opinion of them
            if (recipient.relations.OpinionOf(initiator) < 5 && !initiator.story.traits.HasTrait(TraitDefOfPsychology.Lecher))
            {
                return 0f;
            }
            //People only hit on people if they would consider a formal relationship with them.
            if (!AttractionUtility.WouldConsiderFormalRelationship(initiator,recipient))
            {
                return 0f;

            }
            if (!AttractionUtility.QuickCheck(initiator,recipient))
            {
                return 0f;
            }
            EmptyReasons();
            float attractiveness = AttractionUtility.CalculateAttraction(initiator, recipient, false, true, out veryLowInitiatorReasons, out lowInitiatorReasons, out highInitiatorReasons, out veryHighInitiatorReasons, out AttractionFactorDef reasonForInstantFailure);
            if (attractiveness == 0f)
            {
                return 0f;
            }
            int opinion = initiator.relations.OpinionOf(recipient);
            float romanceChance = GradualRomanceMod.BaseRomanceChance;
            if (!PsycheHelper.PsychologyEnabled(initiator))
            {
                //Vanilla: Straight women are 15% as likely to romance anyone.
                romanceChance *= (!initiator.story.traits.HasTrait(TraitDefOf.Gay)) ? ((initiator.gender != Gender.Female) ? romanceChance : romanceChance * 0.15f) : romanceChance;
            }
            else
            {
                //Psychology: A pawn's likelihood to romance is based on how Aggressive and Romantic they are.
                float personalityFactor = Mathf.Pow(20f, PsycheHelper.Comp(initiator).Psyche.GetPersonalityRating(PersonalityNodeDefOf.Aggressive)) * Mathf.Pow(12f, (1f - PsycheHelper.Comp(initiator).Psyche.GetPersonalityRating(PersonalityNodeDefOf.Romantic)));
                romanceChance *= personalityFactor * 0.02f;
            }
            //If their partner wouldn't consider a relationship with them, they're less likely to try and hit on them. But it doesn't put them off that much.
            if (AttractionUtility.WouldConsiderFormalRelationship(recipient, initiator))
            {
                romanceChance *= 0.4f;
            }
            lastInitiator = initiator;
            lastRecipient = recipient;

            return romanceChance * attractiveness;
        }
        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef)
        {
            if (lastInitiator != initiator || lastRecipient != recipient)
            {
                AttractionUtility.CalculateAttraction(initiator, recipient, false, true, out veryLowInitiatorReasons, out lowInitiatorReasons, out highInitiatorReasons, out veryHighInitiatorReasons, out AttractionFactorDef reasonForInstantFailure);
            }
            if (Rand.Value < this.SuccessChance(initiator, recipient))
            {
                List<Pawn> list;
                this.BreakLoverAndFianceRelations(initiator, out list);
                List<Pawn> list2;
                this.BreakLoverAndFianceRelations(recipient, out list2);
                for (int i = 0; i < list.Count; i++)
                {
                    
                    BreakupUtility.TryAddCheaterThought(list[i], initiator, recipient);
                }
                for (int j = 0; j < list2.Count; j++)
                {
                    BreakupUtility.TryAddCheaterThought(list2[j], recipient, initiator);
                }
                initiator.relations.TryRemoveDirectRelation(PawnRelationDefOf.ExLover, recipient);
                initiator.relations.TryRemoveDirectRelation(PawnRelationDefOfGR.ExLovefriend, recipient);
                GRPawnRelationUtility.AdvanceRelationship(recipient, initiator, PawnRelationDefOfGR.Lovefriend);
                
                //TODO Change record tale
                TaleRecorder.RecordTale(TaleDefOf.BecameLover, new object[]
                {
                    initiator,
                    recipient
                });
                initiator.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.BrokeUpWithMe, recipient);
                recipient.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.BrokeUpWithMe, initiator);
                initiator.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.FailedRomanceAttemptOnMe, recipient);
                initiator.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.FailedRomanceAttemptOnMeLowOpinionMood, recipient);
                recipient.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.FailedRomanceAttemptOnMe, initiator);
                recipient.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.FailedRomanceAttemptOnMeLowOpinionMood, initiator);
                if (PawnUtility.ShouldSendNotificationAbout(initiator) || PawnUtility.ShouldSendNotificationAbout(recipient))
                {
                    this.GetNewLoversLetter(initiator, recipient, list, list2, out letterText, out letterLabel, out letterDef);
                    letterText += AttractionUtility.WriteReasonsParagraph(initiator, recipient, veryHighInitiatorReasons, highInitiatorReasons, lowInitiatorReasons, veryLowInitiatorReasons);
                    letterText += AttractionUtility.WriteReasonsParagraph(recipient, initiator, veryHighRecipientReasons, highRecipientReasons, lowRecipientReasons, veryLowRecipientReasons);
                }
                else
                {
                    letterText = null;
                    letterLabel = null;
                    letterDef = null;
                }
                extraSentencePacks.Add(RulePackDefOf.Sentence_RomanceAttemptAccepted);
            }
            else
            {
                initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.RebuffedMyRomanceAttempt, recipient);
                recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.FailedRomanceAttemptOnMe, initiator);
                if (recipient.relations.OpinionOf(initiator) <= 0)
                {
                    recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.FailedRomanceAttemptOnMeLowOpinionMood, initiator);
                }
                extraSentencePacks.Add(RulePackDefOf.Sentence_RomanceAttemptRejected);
                letterText = null;
                letterLabel = null;
                letterDef = null;
            }
        }
        public float SuccessChance(Pawn initiator, Pawn recipient)
        {

            float successChance = 0.6f * GradualRomanceMod.RomanticSuccessRate;

            successChance *= AttractionUtility.CalculateAttraction(recipient, initiator, false, true, out veryLowRecipientReasons, out lowRecipientReasons, out highRecipientReasons, out veryHighRecipientReasons, out AttractionFactorDef reasonForInstantFailure);
            return Mathf.Clamp01(successChance);
        } 

        private void BreakLoverAndFianceRelations(Pawn pawn, out List<Pawn> oldLoversAndFiances)
        {
            oldLoversAndFiances = new List<Pawn> { };
            List<PawnRelationDef> relationsToBreak = GRPawnRelationUtility.ListOfBreakupRelationships();

            for (int i = 0; i < relationsToBreak.Count(); i++)
            {
                PawnRelationDef relation = relationsToBreak[i];
                List<Pawn> pawns = GRPawnRelationUtility.GetAllPawnsWithGivenRelationshipTo(pawn, relation);
                for (int i2 = 1; i2 < pawns.Count(); i2++)
                {
                    Pawn other = pawns[i2];
                    if (!GRPawnRelationUtility.IsPolygamist(pawn) || !GRPawnRelationUtility.IsPolygamist(other))
                    { 
                        BreakupUtility.ResolveBreakup(pawn, other, relation);
                        
                        oldLoversAndFiances.Add(other);
                    }
                }
            }
        }


        private void GetNewLoversLetter(Pawn initiator, Pawn recipient, List<Pawn> initiatorOldLoversAndFiances, List<Pawn> recipientOldLoversAndFiances, out string letterText, out string letterLabel, out LetterDef letterDef)
        {
            bool flag = false;
            if ((initiator.GetSpouse() != null && !initiator.GetSpouse().Dead) || (recipient.GetSpouse() != null && !recipient.GetSpouse().Dead))
            {
                letterLabel = "LetterLabelAffair".Translate();
                letterDef = LetterDefOf.NegativeEvent;
                flag = true;
            }
            else
            {
                letterLabel = "LetterLabelNewLovers".Translate();
                letterDef = LetterDefOf.PositiveEvent;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("LetterNewLovers".Translate(initiator.Named("PAWN1"), recipient.Named("PAWN2")));
            stringBuilder.AppendLine();
            if (flag)
            {
                if (initiator.GetSpouse() != null)
                {
                    stringBuilder.AppendLine("LetterAffair".Translate(initiator.LabelShort, initiator.GetSpouse().LabelShort, recipient.LabelShort, initiator.Named("PAWN1"), recipient.Named("PAWN2"), initiator.GetSpouse().Named("SPOUSE")));
                }
                if (recipient.GetSpouse() != null)
                {
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.AppendLine();
                    }
                    stringBuilder.AppendLine("LetterAffair".Translate(recipient.LabelShort, recipient.GetSpouse().LabelShort, initiator.LabelShort, recipient.Named("PAWN1"), recipient.GetSpouse().Named("SPOUSE"), initiator.Named("PAWN2")));
                }
            }
            for (int i = 0; i < initiatorOldLoversAndFiances.Count; i++)
            {
                if (!initiatorOldLoversAndFiances[i].Dead)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.AppendLine();
                    }
                    stringBuilder.AppendLine("LetterNoLongerLovers".Translate(initiator.LabelShort, initiatorOldLoversAndFiances[i].LabelShort, initiator.Named("PAWN1"), initiatorOldLoversAndFiances[i].Named("PAWN2")));
                }
            }
            for (int j = 0; j < recipientOldLoversAndFiances.Count; j++)
            {
                if (!recipientOldLoversAndFiances[j].Dead)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.AppendLine();
                    }
                    stringBuilder.AppendLine("LetterNoLongerLovers".Translate(recipient.LabelShort, recipientOldLoversAndFiances[j].LabelShort, recipient.Named("PAWN1"), recipientOldLoversAndFiances[j].Named("PAWN2")));
                }
            }
            letterText = stringBuilder.ToString().TrimEndNewlines();
        }
    }
}
