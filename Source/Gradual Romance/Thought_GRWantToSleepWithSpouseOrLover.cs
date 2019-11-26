using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class Thought_GRWantToSleepWithSpouseOrLover : Thought_Situational
    {
        public override string LabelCap
        {
            get
            {
                DirectPawnRelation directPawnRelation = GRPawnRelationUtility.MostLikedBedSharingRelationship(this.pawn, false);
                return string.Format(base.CurStage.label, directPawnRelation.otherPawn.LabelShort).CapitalizeFirst();
            }
        }

        // Token: 0x1700018E RID: 398
        // (get) Token: 0x060009F2 RID: 2546 RVA: 0x0004EE78 File Offset: 0x0004D278
        protected override float BaseMoodOffset
        {
            get
            {
                float a = -0.05f * (float)this.pawn.relations.OpinionOf(GRPawnRelationUtility.MostLikedBedSharingRelationship(this.pawn, false).otherPawn);
                return Mathf.Min(a, -1f);
            }
        }
    }
}
