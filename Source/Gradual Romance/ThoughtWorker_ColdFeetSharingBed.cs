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
            /*
            if (pawn.ownership.OwnedBed == null)
            {
                return ThoughtState.Inactive;
            }
            IEnumerable<Pawn> bedPartners = from partner in pawn.ownership.OwnedBed.AssignedPawns
                                     where LovePartnerRelationUtility.LovePartnerRelationExists(pawn, partner) == true && GRPawnRelationUtility.ShouldShareBed(pawn, partner) == false
                                     select partner;
            if (bedPartners.Count() == 0)
            {
                return ThoughtState.Inactive;
            }
            for (int i = 0; i < bedPartners.Count(); i++)
            {
                Pawn partner = bedPartners.ElementAt(0);
                if (partner.relations.DirectRelationExists(PawnRelationDefOfGR.Sweetheart, pawn))
                {
                    return ThoughtState.ActiveAtStage(1);
                }

            }
            return ThoughtState.ActiveAtStage(0);
            */

            return ThoughtState.Inactive; 

        }
    }

}
