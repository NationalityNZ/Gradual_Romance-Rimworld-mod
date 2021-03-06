﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using Psychology;

namespace Gradual_Romance
{
    public class ThoughtWorker_GRWantToSleepWithSpouseOrLover : ThoughtWorker
    {

        /// This is a mish-mash of copy-pasted code.
        /// Mostly copy-pasted from Psychology Mod by Word-Mule
        /// 
        /// Does the same function but excludes lovefriends and sweethearts
        /// 
        ///
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {

            DirectPawnRelation directPawnRelation = RelationshipUtility.MostLikedBedSharingRelationship(p, false);
            if (directPawnRelation == null)
            {
                return ThoughtState.Inactive;
            }

            bool multiplePartners = (from r in p.relations.PotentiallyRelatedPawns
                                         where RelationshipUtility.ShouldShareBed(p, r)
                                         select r).Count() > 1;
            bool partnerBedInRoom;
            if (p.ownership.OwnedBed != null)
            { 
                partnerBedInRoom = (from t in p.ownership.OwnedBed.GetRoom().ContainedBeds
                                         where t.AssignedPawns.Contains(directPawnRelation.otherPawn)
                                         select t).Count() > 0;
            }
            else
            {
                partnerBedInRoom = false;
            }
            if (directPawnRelation != null && p.ownership.OwnedBed != null && RelationshipUtility.IsPolygamist(p) && multiplePartners && partnerBedInRoom)
            {
                return ThoughtState.Inactive;
            }
            if (p.ownership.OwnedBed != null && p.ownership.OwnedBed == directPawnRelation.otherPawn.ownership.OwnedBed)
            {
                return ThoughtState.Inactive;
            }
            if (p.relations.OpinionOf(directPawnRelation.otherPawn) <= 0)
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveDefault;

        }
    }
}
