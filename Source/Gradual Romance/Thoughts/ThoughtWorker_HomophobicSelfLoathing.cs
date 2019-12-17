﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Psychology;

namespace Gradual_Romance
{
    public class ThoughtWorker_HomophobicSelfLoathing : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            if (PsycheHelper.PsychologyEnabled(pawn) && PsychologyBase.ActivateKinsey())
            {
                if (PsycheHelper.Comp(pawn).Sexuality.kinseyRating >= 2)
                {
                    return ThoughtState.ActiveAtStage(0);
                }

                return false;
            }
            if (pawn.story.traits.HasTrait(TraitDefOf.Gay))
            {
                return ThoughtState.ActiveAtStage(0);
            }
            return false;
        }
    }

}
