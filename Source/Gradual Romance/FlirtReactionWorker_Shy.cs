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
    public class FlirtReactionWorker_Shy : FlirtReactionWorker
    {
        public override void GiveThoughts(Pawn initiator, Pawn recipient, out List<RulePackDef> yetMoreSentencePacks)
        {
            yetMoreSentencePacks = new List<RulePackDef> { };
            float interpretChance = 1f;
            if (PsycheHelper.PsychologyEnabled(initiator))
            {
                interpretChance *= 0.5f + PsycheHelper.Comp(initiator).Psyche.GetPersonalityRating(PersonalityNodeDefOfGR.Optimistic);
                interpretChance *= 0.5f + Mathf.Abs(1- PsycheHelper.Comp(initiator).Psyche.GetPersonalityRating(PersonalityNodeDefOf.Empathetic));
                interpretChance = Mathf.InverseLerp(0.50f, 2f, interpretChance);
            }
            else
            {
                interpretChance = 0.5f;
            }
            recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.RomanticInterest, initiator);

            if (Rand.Value < interpretChance)
            {
                yetMoreSentencePacks.Add(RulePackDefOfGR.Sentence_InterpretFlirtPositively);
                initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.RomanticInterest, recipient);
            }
            else
            {
                initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.RomanticDisinterest, recipient);
            }
        }

    }
}
