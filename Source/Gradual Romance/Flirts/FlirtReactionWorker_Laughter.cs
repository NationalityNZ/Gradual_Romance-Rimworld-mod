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
    public class FlirtReactionWorker_Laughter : FlirtReactionWorker
    {
        public override void GiveThoughts(Pawn initiator, Pawn recipient, out List<RulePackDef> yetMoreSentencePacks)
        {
            yetMoreSentencePacks = new List<RulePackDef> { };
            float interpretChance = 1f;
            if (PsycheHelper.PsychologyEnabled(initiator))
            {
                interpretChance *= 0.5f + PsycheHelper.Comp(initiator).Psyche.GetPersonalityRating(PersonalityNodeDefOf.Polite);
                interpretChance *= 0.5f + PsycheHelper.Comp(initiator).Psyche.GetPersonalityRating(PersonalityNodeDefOf.Empathetic);
                interpretChance = Mathf.InverseLerp(0.50f, 2f, interpretChance);
            }
            else
            {
                interpretChance = 0.5f;
            }
            initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.RomanticDisinterest, recipient);

            if (Rand.Value < interpretChance)
            {
                yetMoreSentencePacks.Add(RulePackDefOfGR.Sentence_FeltShame);
                initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.BotchedFlirt, recipient);
            }

        }

    }
}
