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
                ThoughtDef thoughtToGive = reaction.givesTension.RandomElement();
                if (thoughtToGive != null)
                {
                    initiator.needs.mood.thoughts.memories.TryGainMemory(thoughtToGive, recipient);
                    recipient.needs.mood.thoughts.memories.TryGainMemory(thoughtToGive, initiator);
                }
            }
            else
            {
                initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfGR.RomanticDisinterest, recipient);
            }
        }

        public FlirtReactionDef reaction;
    }
}
