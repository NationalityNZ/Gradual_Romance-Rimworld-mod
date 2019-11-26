using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Gradual_Romance
{
    public class FlirtReactionWorker
    {
        public virtual void GiveThoughts(Pawn initiator, Pawn recipient, out List<RulePackDef> yetMoreSentencePacks)
        {
            yetMoreSentencePacks = new List<RulePackDef> { };
            if (reaction.successful)
            {
                initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.RomanticInterest, recipient);
                recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.RomanticInterest, initiator);
            }
            else
            {
                initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.RomanticDisinterest, recipient);
            }
        }

        public FlirtReactionDef reaction;
    }
}
