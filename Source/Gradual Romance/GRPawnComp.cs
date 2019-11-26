using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class GRPawnComp : ThingComp
    {
        public float cachedSkillAttractiveness = 1f;
        public float cachedBeautyAttractiveness = 1f;
        public float cachedWealthAttractiveness = 1f;
        public int cachedNumberOfColonyFriends = 0;

        public GRPawnComp_Properties Props => (GRPawnComp_Properties)this.props;

        public float facialAttractiveness = 0f;


        public override void CompTick()
        {
            Pawn pawn = this.parent as Pawn;
            int gameTicks = Find.TickManager.TicksGame;
            if (gameTicks % recachePerTick == 0 && pawn.Spawned && !pawn.Dead)
            {
                refreshCache(pawn);
            }
        }

        private void refreshCache(Pawn pawn)
        {
            cachedSkillAttractiveness = AttractionUtility.GetObjectiveSkillAttractiveness(pawn);
            cachedWealthAttractiveness = AttractionUtility.GetObjectiveWealthAttractiveness(pawn);
            //cachedNumberOfColonyFriends = GRPawnRelationUtility.NumberOfFriends(pawn);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            Pawn pawn = this.parent as Pawn;
            refreshCache(pawn);
            if (facialAttractiveness == 0f)
            {
                Rand.PushState((pawn.thingIDNumber ^ 17) * Time.time.GetHashCode());
                facialAttractiveness = Mathf.Clamp(Rand.Gaussian(1f, .3f), 0.01f, 3f);
                Rand.PopState();
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref facialAttractiveness, "facialattractiveness", 0f);
        }

        private const int recachePerTick = 2500;
    }

    public class GRPawnComp_Properties : CompProperties
    {
        public float facialAttractiveness;
        public GRPawnComp_Properties()
        {
            this.compClass = typeof(GRPawnComp);
        }

        public GRPawnComp_Properties(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }

}
