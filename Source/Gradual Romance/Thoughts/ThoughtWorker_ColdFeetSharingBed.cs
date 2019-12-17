using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class ThoughtWorker_ColdFeetSharingBed : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            if (pawn.ownership.OwnedBed == null)
            {
                return ThoughtState.Inactive;
            }
            IEnumerable<Pawn> bedPartners = from partner in pawn.ownership.OwnedBed.AssignedPawns
                                     where partner != pawn && RelationshipUtility.MostAdvancedRelationshipBetween(pawn, partner) != null && RelationshipUtility.ShouldShareBed(pawn, partner) == false
                                     select partner;
            if (bedPartners.Count() == 0)
            {
                return ThoughtState.Inactive;
            }
            else
            {
                return ThoughtState.ActiveAtStage(0);
            }

        }
    }

}
