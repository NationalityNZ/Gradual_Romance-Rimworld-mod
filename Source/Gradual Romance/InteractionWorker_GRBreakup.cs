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
    public class InteractionWorker_GRBreakup : InteractionWorker
    {
        // Plundered and adapted from Psychology
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            if (!GRPawnRelationUtility.HasInformalRelationship(initiator, recipient) && !LovePartnerRelationUtility.LovePartnerRelationExists(initiator, recipient))
            {
                return 0f;

            }
            else if (initiator.story.traits.HasTrait(TraitDefOfPsychology.Codependent))
            {
                return 0f;
            }
            float chance = 0.02f * GradualRomanceMod.BaseBreakupChance;
            float romanticFactor = 1f;
            if (PsycheHelper.PsychologyEnabled(initiator))
            {
                chance = 0.05f * GradualRomanceMod.BaseBreakupChance;
                romanticFactor = Mathf.InverseLerp(1.05f, 0f, PsycheHelper.Comp(initiator).Psyche.GetPersonalityRating(PersonalityNodeDefOf.Romantic));
            }
            float opinionFactor = Mathf.InverseLerp(100f, -100f, (float)initiator.relations.OpinionOf(recipient));
            float spouseFactor = 1f;
            if (initiator.relations.DirectRelationExists(PawnRelationDefOf.Spouse, recipient))
            {
                spouseFactor = 0.4f;
            }
            return chance * romanticFactor * opinionFactor * spouseFactor;

        }

        public Thought RandomBreakupReason(Pawn initiator, Pawn recipient)
        {
            List<Thought_Memory> list = (from m in initiator.needs.mood.thoughts.memories.Memories
                                         where m != null && m.otherPawn == recipient && m.CurStage != null && m.CurStage.baseOpinionOffset < 0f
                                         select m).ToList<Thought_Memory>();
            if (list.Count == 0)
            {
                return null;
            }
            float worstMemoryOpinionOffset = list.Max((Thought_Memory m) => -m.CurStage.baseOpinionOffset);
            Thought_Memory result = null;
            (from m in list
             where -m.CurStage.baseOpinionOffset >= worstMemoryOpinionOffset / 2f
             select m).TryRandomElementByWeight((Thought_Memory m) => -m.CurStage.baseOpinionOffset, out result);
            return result;
        }

        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef)
        {
            ///Stitched in from Psychology.
            Thought thought = this.RandomBreakupReason(initiator, recipient);
            PawnRelationDef relation = GRPawnRelationUtility.MostAdvancedRelationshipBetween(initiator, recipient);

            if (initiator.relations.DirectRelationExists(PawnRelationDefOf.Spouse, recipient))
            {
                BreakupUtility.RelationToEx(initiator, recipient, PawnRelationDefOf.Spouse);
                recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.DivorcedMe, initiator);
                recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfPsychology.BrokeUpWithMeCodependent, initiator);
                initiator.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.GotMarried);
                recipient.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.GotMarried);
                initiator.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.HoneymoonPhase, recipient);
                recipient.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.HoneymoonPhase, initiator);
            }
            else
            {
                BreakupUtility.ResolveBreakup(initiator, recipient, GRPawnRelationUtility.MostAdvancedRelationshipBetween(initiator, recipient));
            }
            if (initiator.ownership.OwnedBed != null && initiator.ownership.OwnedBed == recipient.ownership.OwnedBed)
            {
                Pawn pawn = (Rand.Value >= 0.5f) ? recipient : initiator;
                pawn.ownership.UnclaimBed();
            }
            TaleRecorder.RecordTale(TaleDefOf.Breakup, new object[]
            {
                initiator,
                recipient
            });
            StringBuilder stringBuilder = new StringBuilder();
            if (GRPawnRelationUtility.IsInformalRelationship(relation))
            {
                stringBuilder.AppendLine("LetterInformalRelationsEnds".Translate(initiator.Named("PAWN1"), recipient.Named("PAWN2")));
                letterDef = LetterDefOf.NeutralEvent;
                letterLabel = "LetterLabelInformalRelationsEnds".Translate();
            }
            else
            {
                stringBuilder.AppendLine("LetterNoLongerLovers".Translate(initiator.LabelShort, recipient.LabelShort, initiator.Named("PAWN1"), recipient.Named("PAWN2")));
                letterDef = LetterDefOf.NegativeEvent;
                letterLabel = "LetterLabelBreakup".Translate();
            }
            
            if (thought != null)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("FinalStraw".Translate(thought.CurStage.label.CapitalizeFirst()));
            }
            if (PawnUtility.ShouldSendNotificationAbout(initiator) || PawnUtility.ShouldSendNotificationAbout(recipient))
            {
                letterDef = null;
                letterLabel = null;
                letterText = null;
            }
            else if (GRPawnRelationUtility.IsInformalRelationship(relation) && GradualRomanceMod.informalRomanceLetters == false)
            {
                letterDef = null;
                letterLabel = null;
                letterText = null;
            }
            else
            {
                letterText = stringBuilder.ToString();
            }
        }
    }
}
