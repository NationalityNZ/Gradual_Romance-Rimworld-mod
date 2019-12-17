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
    public class ThoughtWorker_FeelingNeglected : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                return false;
            }
            PawnRelationDef pawnRelationDef = RelationshipUtility.MostAdvancedRelationshipBetween(pawn, other);
            if (pawnRelationDef == null)
            {
                return false;
            }
            if (!pawnRelationDef.GetModExtension<RomanticRelationExtension>().isFormalRelationship)
            {
                return false;
            }
            if (RelationshipUtility.LevelOfTension(pawn, other) == 0)
            {
                return ThoughtState.ActiveDefault;
            }

            return false;
        }

    }

}
