﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Psychology;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public static class BreakupUtility
    {
        public static bool HasReasonForBreakup(Pawn pawn, Pawn other)
        {
            PawnRelationDef relation = RelationshipUtility.MostAdvancedRelationshipBetween(pawn, other);
            if (relation == null)
            {
                return false;
            }
            if (AttractionUtility.GetRelationshipUnmodifiedOpinion(pawn,other) < unmodifiedRelationshipBreakupThreshold)
            {
                return true;
            }
            if (pawn.needs.mood.thoughts.memories.Memories.Any<Thought_Memory>(x => x.def.defName == "CheatedOnMe" && x.otherPawn == other))
            {
                return true;
            }
            if (pawn.needs.mood.thoughts.memories.Memories.Any<Thought_Memory>(x => x.def.defName == "CaughtFlirting" && x.otherPawn == other))
            {
                return true;
            }
            if (ThoughtDetector.HasSocialSituationalThought(pawn, other, ThoughtDefOfGR.FeelingNeglected))
            {
                return true;
            }
            return false;

        }
        public static bool CanDecay(Pawn pawn, Pawn other, PawnRelationDef relation)
        {
            DirectPawnRelation directPawnRelation = pawn.relations.GetDirectRelation(relation, other);
            if (directPawnRelation == null)
            {
                //GradualRomanceMod.Error_TriedDecayNullRelationship(pawn, other, relation);
                return false;
            }
            if (!GRHelper.RomanticRelationExtension(relation).decayable)
            {
                return false;
            }
            //TODO - Revise to reflect lovin'
            if (RelationshipUtility.LevelOfTension(pawn, other) == 0)
            {
                return true;
            }
            return false;
        }

        public static void DecayRelationship(Pawn pawn, Pawn other, PawnRelationDef relation)
        {
            pawn.relations.TryRemoveDirectRelation(relation, other);
            if ((PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(other)) && GradualRomanceMod.informalRomanceLetters)
            {
                //Finish
                Current.Game.letterStack.ReceiveLetter("RelationshipDecays_label", "RelationshipDecays_text".Translate(pawn, other, relation.label), LetterDefOf.NeutralEvent);
            }
        }
        //break all relations of given relation
        public static void BreakAllGivenRelations(Pawn pawn, PawnRelationDef relation, bool testRelations = false)
        {
            if (!RelationshipUtility.IsRomanticOrSexualRelationship(relation))
            {
                return;
            }
            List<Pawn> allPawns = RelationshipUtility.GetAllPawnsWithGivenRelationshipTo(pawn, relation);
            for (int i = 0; i < allPawns.Count(); i++)
            {
                RelationToEx(pawn, allPawns[i], relation);
            }
        }

        public static void TryAddCheaterThought(Pawn pawn, Pawn cheater, Pawn cheaterLover)
        {
            {
                if (pawn.Dead)
                {
                    return;
                }
                if (RelationshipUtility.IsPolygamist(pawn))
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.CheatedOnMePolygamist, cheater);
                }
                else
                {
                    float toleranceChance = 0.4f;
                    //people who are codependent are more tolerant of cheating, even though they are less likely to cheat
                    if (pawn.story.traits.HasTrait(TraitDefOfPsychology.Codependent))
                    {
                        toleranceChance *= 2f;
                    }
                    //moralistic characters are less tolerant of cheating.
                    if (PsycheHelper.PsychologyEnabled(pawn))
                    {
                        toleranceChance *= (1.5f - PsycheHelper.Comp(pawn).Psyche.GetPersonalityRating(PersonalityNodeDefOfGR.Moralistic));
                    }
                    //they are more likely to tolerate cheating if they like the person who the cheater is cheating with.
                    toleranceChance *= Mathf.Min(0f, Mathf.InverseLerp(-80f, 30f, pawn.relations.OpinionOf(cheaterLover)));
                    toleranceChance = Mathf.Clamp01(toleranceChance);

                    if (Rand.Value < toleranceChance)
                    {
                        pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.CheatedOnMeTolerated, cheater);
                        
                    }
                    else
                    {
                        if (pawn.relations.OpinionOf(cheaterLover) >= 30f)
                        {
                            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.CheatedOnMeHighOpinion, cheater);
                        }
                        else
                        {
                            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.CheatedOnMe, cheater);
                        }
                    }
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.LoversLover, cheaterLover);
                    //cheaterLover.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.LoversLover, pawn);
                }
            }
        }

        //converts relation to ex
        public static void RelationToEx (Pawn lover, Pawn ex, PawnRelationDef relation)
        {
            if (GRHelper.RomanticRelationExtension(relation).ex != null)
            {
                lover.relations.TryRemoveDirectRelation(relation, ex);
                lover.relations.AddDirectRelation(GRHelper.RomanticRelationExtension(relation).ex, ex);

            }
            else
            {
                lover.relations.TryRemoveDirectRelation(relation, ex);
            }
        }
        public static void ResolveBreakup(Pawn lover, Pawn ex, PawnRelationDef relation, float intensity = 1f)
        {
            if (intensity > 0f && PsycheHelper.PsychologyEnabled(lover) && PsycheHelper.PsychologyEnabled(ex))
            {
                float loverIntensity = Mathf.Clamp(Mathf.Round(intensity * (relation.opinionOffset / 5)),0f,10f);
                float exIntensity = Mathf.Clamp(Mathf.Round(intensity * (relation.opinionOffset / 5) / 3), 0f, 10f);
                AddBrokeUpMood(lover, ex, loverIntensity);
                AddBrokeUpMood(ex, lover, exIntensity);
                AddBrokeUpOpinion(ex, lover, exIntensity);
            }
            RelationToEx(lover, ex, relation);
        }
        //Modified directly from Psychology.
        private static void AddBrokeUpOpinion(Pawn lover, Pawn ex, float intensity = 1f)
        {
            ThoughtDef brokeUpDef = new ThoughtDef();
            brokeUpDef.defName = "BrokeUpWithMe" + lover.LabelShort + Find.TickManager.TicksGame;
            if (intensity < 1f)
                {
                brokeUpDef.durationDays = 10f;
                }
            else
                {
                brokeUpDef.durationDays = 40f;
                }
            brokeUpDef.thoughtClass = typeof(Thought_MemorySocialDynamic);
            ThoughtStage brokeUpStage = new ThoughtStage();
            brokeUpStage.label = "broke up with me";
            brokeUpStage.baseOpinionOffset = Mathf.RoundToInt(-10 * intensity * PsycheHelper.Comp(lover).Psyche.GetPersonalityRating(PersonalityNodeDefOf.Romantic) * Mathf.InverseLerp(5f, 100f, lover.relations.OpinionOf(ex)));
            brokeUpDef.stages.Add(brokeUpStage);
            lover.needs.mood.thoughts.memories.TryGainMemory(brokeUpDef, ex);
        }
        private static void AddBrokeUpMood(Pawn lover, Pawn ex, float intensity = 1f)
        {
            ThoughtDef brokeUpMoodDef = new ThoughtDef();
            brokeUpMoodDef.defName = "BrokeUpWithMeMood" + lover.LabelShort + Find.TickManager.TicksGame;
            if (intensity < 1f)
            {
                brokeUpMoodDef.durationDays = 10f;
            }
            else
            {
                brokeUpMoodDef.durationDays = 40f;
            }
            brokeUpMoodDef.thoughtClass = typeof(Thought_MemoryDynamic);
            brokeUpMoodDef.stackedEffectMultiplier = 1f;
            brokeUpMoodDef.stackLimit = 999;
            ThoughtStage brokeUpStage = new ThoughtStage();
            brokeUpStage.label = "Broke up with {0}";
            brokeUpStage.baseMoodEffect = Mathf.RoundToInt(-4f * intensity * Mathf.InverseLerp(0.25f, 0.75f, PsycheHelper.Comp(lover).Psyche.GetPersonalityRating(PersonalityNodeDefOf.Romantic)) * Mathf.InverseLerp(-20f, 100f, lover.relations.OpinionOf(ex)));
            if (brokeUpStage.baseMoodEffect < -5f)
            {
                brokeUpStage.description = "{0} and I parted ways amicably, but it's still a little sad.";
            }
            else
            {
                brokeUpStage.description = "I'm going through a bad break-up right now.";
            }
            brokeUpMoodDef.stages.Add(brokeUpStage);
            if (brokeUpStage.baseMoodEffect > 0f)
            {
                lover.needs.mood.thoughts.memories.TryGainMemory(brokeUpMoodDef, ex);
            }
        }
        public static bool ShouldBeJealous(Pawn observer, Pawn initiator, Pawn recipient)
        {
            if (RelationshipUtility.IsPolygamist(observer))
            {
                return false;
            }
            PawnRelationDef initiatorRelationship = RelationshipUtility.MostAdvancedRelationshipBetween(observer, initiator);
            PawnRelationDef recipientRelationship = RelationshipUtility.MostAdvancedRelationshipBetween(observer, recipient);
            if (initiatorRelationship == null)
            {
                return false;
            }
            if (!initiatorRelationship.GetModExtension<RomanticRelationExtension>().caresAboutCheating)
            {
                return false;
            }
            if (recipientRelationship != null && !observer.story.traits.HasTrait(TraitDefOfGR.Jealous))
            {
                return false;
            }
            return true;
        }

        public static bool ShouldImplicitlyEndInformalRelationship(Pawn pawn, Pawn other, PawnRelationDef relation)
        {
            if (CanDecay(pawn, other, relation))
            {
                return true;
            }
            if (RelationshipUtility.IsPolygamist(pawn))
            {
                return false;
            }

            float chance = Mathf.InverseLerp(80, -20, AttractionUtility.GetRelationshipUnmodifiedOpinion(pawn, other));
            chance *= (relation.GetModExtension<RomanticRelationExtension>().relationshipLevel / (relation.GetModExtension<RomanticRelationExtension>().relationshipLevel + 1));
            if (PsycheHelper.PsychologyEnabled(pawn))
            {
                chance *= Mathf.Lerp(0f, 2f, PsycheHelper.Comp(pawn).Psyche.GetPersonalityRating(PersonalityNodeDefOfGR.Moralistic));
            }
            if (Rand.Value < chance)
            {
                return true;
            }
            return false;

        }

        const int unmodifiedRelationshipBreakupThreshold = 10;
    }
}
