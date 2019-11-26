﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Psychology;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class InteractionWorker_AskToMoveIn : InteractionWorker
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
            //Pawns will only ask their girlfriend/boyfriend to move in
            if (!initiator.relations.DirectRelationExists(PawnRelationDefOfGR.Lovefriend, recipient))
            {
                return 0f;
            }
            if (!AttractionUtility.WouldConsiderFormalRelationship(initiator, recipient))
            {
                return 0f;

            }
            if (!AttractionUtility.QuickCheck(initiator, recipient))
            {
                return 0f;
            }
            EmptyReasons();
            float attractiveness = AttractionUtility.CalculateAttraction(initiator, recipient,false,true,out veryLowInitiatorReasons,out lowInitiatorReasons,out highInitiatorReasons,out veryHighInitiatorReasons,out AttractionFactorDef reasonForInstantFailure);
            if (attractiveness == 0f)
            {
                return 0f;
            }
            
            float romanceChance = GradualRomanceMod.BaseRomanceChance;
            if (!PsycheHelper.PsychologyEnabled(initiator))
            {
                //Vanilla: Straight women are 15% as likely to romance anyone.
                romanceChance *= (!initiator.story.traits.HasTrait(TraitDefOf.Gay)) ? ((initiator.gender != Gender.Female) ? romanceChance : romanceChance * 0.15f) : romanceChance;
            }
            else
            {
                //Psychology: A pawn's likelihood to ask to move in.
                float personalityFactor = Mathf.Pow(12f, (1f - PsycheHelper.Comp(initiator).Psyche.GetPersonalityRating(PersonalityNodeDefOf.Romantic)));
                romanceChance *= personalityFactor * 0.02f;
            }
            //If their potential partner wouldn't consider a relationship with them, they're less likely to try and hit on them. But it doesn't put them off that much.
            if (AttractionUtility.WouldConsiderFormalRelationship(recipient, initiator))
            {
                romanceChance *= 0.2f;
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


                GRPawnRelationUtility.AdvanceRelationship(initiator, recipient, PawnRelationDefOf.Lover);
                LovePartnerRelationUtility.TryToShareBed(initiator, recipient);

                //TODO Add Move In tale
                

                if (PawnUtility.ShouldSendNotificationAbout(initiator) || PawnUtility.ShouldSendNotificationAbout(recipient))
                {
                    letterText = "MoveInLetterText".Translate(initiator.Named("PAWN1"), recipient.Named("PAWN2"));
                    letterText += AttractionUtility.WriteReasonsParagraph(initiator, recipient, veryHighInitiatorReasons, highInitiatorReasons, lowInitiatorReasons, veryLowInitiatorReasons);
                    letterText += AttractionUtility.WriteReasonsParagraph(recipient, initiator, veryHighRecipientReasons, highRecipientReasons, lowRecipientReasons, veryLowRecipientReasons);
                    letterLabel = "MoveInLetterTitle".Translate();
                    letterDef = LetterDefOf.PositiveEvent;
                }
                else
                {
                    letterText = null;
                    letterLabel = null;
                    letterDef = null;
                }
                //extraSentencePacks.Add(RulePackDefOf.Sentence_RomanceAttemptAccepted);
            }
            else
            {

                //extraSentencePacks.Add(RulePackDefOf.Sentence_RomanceAttemptRejected);
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

    }
}
