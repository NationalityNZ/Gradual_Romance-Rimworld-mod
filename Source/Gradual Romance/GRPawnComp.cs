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
        /*
        public float cachedSkillAttractiveness = 1f;
        public float cachedBeautyAttractiveness = 1f;
        public float cachedWealthAttractiveness = 1f;
        public int cachedNumberOfColonyFriends = 0;
        */

        public GRPawnComp_Properties Props => (GRPawnComp_Properties)this.props;

        //public float facialAttractiveness = 0f;


        public override void CompTick()
        {
            Pawn pawn = this.parent as Pawn;
            int gameTicks = Find.TickManager.TicksGame;
            if (gameTicks % recachePerTick == 0 && pawn.Spawned && !pawn.Dead)
            {
                refreshCache(pawn);
            }
            if (gameTicks % GenDate.TicksPerDay == 0 && pawn.Spawned && !pawn.Dead)
            {
                List<DirectPawnRelation> relations = pawn.relations.DirectRelations;
                for (int i = 0; i < relations.Count(); i++)
                {
                    if (RelationshipUtility.ListOfRomanceStages().Contains(relations[i].def))
                    {
                        if (BreakupUtility.CanDecay(pawn, relations[i].otherPawn, relations[i].def))
                        {
                            if (GradualRomanceMod.DecayRate <= Rand.Value)
                            {
                                BreakupUtility.DecayRelationship(pawn, relations[i].otherPawn, relations[i].def);
                            }
                        }
                    }

                }
                CleanAttractionRecords();
            }
        }

        private void refreshCache(Pawn pawn)
        {
            //cachedSkillAttractiveness = AttractionUtility.GetObjectiveSkillAttractiveness(pawn);
            //cachedWealthAttractiveness = AttractionUtility.GetObjectiveWealthAttractiveness(pawn);
            //cachedNumberOfColonyFriends = RelationshipUtility.NumberOfFriends(pawn);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            Pawn pawn = this.parent as Pawn;
            refreshCache(pawn);
            /*if (facialAttractiveness == 0f)
            {
                Rand.PushState((pawn.thingIDNumber ^ 17) * Time.time.GetHashCode());
                facialAttractiveness = Mathf.Clamp(Rand.Gaussian(1f, .3f), 0.01f, 3f);
                Rand.PopState();
            }*/
        }

        public override void PostExposeData()
        {
            //Scribe_Values.Look(ref facialAttractiveness, "facialattractiveness", 0f);
        }

        private bool IsPawnAttractionRelevant(Pawn other)
        {
            if (other.Dead)
            {
                return false;
            }
            if (!other.IsColonist && !other.Spawned)
            {
                return false;
            }
            return true;
        }

        private void CleanAttractionRecords()
        {
            IEnumerable<Pawn> keys = AttractionRecords.Keys;
            foreach (Pawn p in keys)
            {
                if (!IsPawnAttractionRelevant(p))
                {
                    AttractionRecords.Remove(p);
                }
            }
        }

        private AttractionRecord PullRecord(Pawn other, bool noUpdate = false)
        {
            Pawn p = (Pawn)this.parent;
            if (!AttractionRecords.ContainsKey(other))
            {
                AttractionRecords.Add(other, new AttractionRecord(p, other));
            }
            else if (noUpdate == false && Find.TickManager.TicksGame - AttractionRecords[other].lastRefreshedGameTick > recalculateAttractionPerTick)
            {
                AttractionRecords[other].Update(p, other);
            }
            return AttractionRecords[other];
        }


        public float RetrieveAttraction(Pawn other, bool romantic = false, bool chanceOnly = false)
        {
            AttractionRecord record = PullRecord(other);
            return record.RetrieveAttraction(romantic, chanceOnly);
        }
        
        public float RetrieveAttractionForCategory(Pawn other, AttractionFactorCategoryDef category)
        {

            AttractionRecord record = PullRecord(other);
            return record.RetrieveAttractionForCategory(category);
        }

        public void RetrieveFactors(Pawn other, out List<AttractionFactorDef> veryLowFactors, out List<AttractionFactorDef> lowFactors, out List<AttractionFactorDef> highFactors, out List<AttractionFactorDef> veryHighFactors, bool romantic = false, bool chanceOnly = false)
        {
            AttractionRecord record = PullRecord(other);
            record.RetrieveFactors(out veryLowFactors, out lowFactors, out highFactors, out veryHighFactors);
            if (!romantic)
            {
                veryHighFactors.RemoveAll(x => x.category.onlyForRomance);
                highFactors.RemoveAll(x => x.category.onlyForRomance);
                lowFactors.RemoveAll(x => x.category.onlyForRomance);
                veryLowFactors.RemoveAll(x => x.category.onlyForRomance);
            }
            if (!chanceOnly)
            {
                veryHighFactors.RemoveAll(x => x.category.chanceOnly);
                highFactors.RemoveAll(x => x.category.chanceOnly);
                lowFactors.RemoveAll(x => x.category.chanceOnly);
                veryLowFactors.RemoveAll(x => x.category.chanceOnly);
            }
        }

        public float RetrieveAttractionAndFactors(Pawn other, out List<AttractionFactorDef> veryLowFactors, out List<AttractionFactorDef> lowFactors, out List<AttractionFactorDef> highFactors, out List<AttractionFactorDef> veryHighFactors, bool romantic = false, bool chanceOnly = false)
        {
            AttractionRecord record = PullRecord(other);
            record.RetrieveFactors(out veryLowFactors, out lowFactors, out highFactors, out veryHighFactors);
            if (!romantic)
            {
                veryHighFactors.RemoveAll(x => x.category.onlyForRomance);
                highFactors.RemoveAll(x => x.category.onlyForRomance);
                lowFactors.RemoveAll(x => x.category.onlyForRomance);
                veryLowFactors.RemoveAll(x => x.category.onlyForRomance);
            }
            if (!chanceOnly)
            {
                veryHighFactors.RemoveAll(x => x.category.chanceOnly);
                highFactors.RemoveAll(x => x.category.chanceOnly);
                lowFactors.RemoveAll(x => x.category.chanceOnly);
                veryLowFactors.RemoveAll(x => x.category.chanceOnly);
            }
            return record.RetrieveAttraction(romantic, chanceOnly);
        }
        protected class AttractionRecord
        {
            public int lastRefreshedGameTick = 0;
            float totalAttraction;
            Dictionary<AttractionFactorCategoryDef, float> categoryCalculations;
            List<AttractionFactorDef> veryHighFactors = new List<AttractionFactorDef>() { };
            List<AttractionFactorDef> highFactors = new List<AttractionFactorDef>() { };
            List<AttractionFactorDef> lowFactors = new List<AttractionFactorDef>() { };
            List<AttractionFactorDef> veryLowFactors = new List<AttractionFactorDef>() { };
            
            public AttractionRecord(Pawn pawn, Pawn other)
            {
                //Log.Message("Start constructor for " + pawn.Name.ToStringShort + " : " + other.Name.ToStringShort);
                lastRefreshedGameTick = Find.TickManager.TicksGame;
                //Log.Message("Made dictionary.");
                categoryCalculations = new Dictionary<AttractionFactorCategoryDef, float>();
                if (pawn == other)
                {
                    return;
                }
                totalAttraction = 1f;
                //Log.Message("Going through categories.");
                List<AttractionFactorCategoryDef> allDefs = DefDatabase<AttractionFactorCategoryDef>.AllDefsListForReading;
                foreach (AttractionFactorCategoryDef category in allDefs)
                {
                    //Log.Message("Processing " + category.defName);
                    AttractionFactorDef whoCares;
                    List<AttractionFactorDef> newVeryHighFactors = new List<AttractionFactorDef>() { };
                    List<AttractionFactorDef> newHighFactors = new List<AttractionFactorDef>() { };
                    List<AttractionFactorDef> newLowFactors = new List<AttractionFactorDef>() { };
                    List<AttractionFactorDef> newVeryLowFactors = new List<AttractionFactorDef>() { };
                    if (category.alwaysRecalculate)
                    {
                        //Log.Message("Passing " + category.defName);
                        continue;
                    }
                    else
                    {
                        //Log.Message("Adding result " + category.defName);
                        float result = AttractionUtility.CalculateAttractionCategory(category, pawn, other, out newVeryLowFactors, out newLowFactors, out newHighFactors, out newVeryHighFactors, out whoCares);

                        categoryCalculations.Add(category, result);
                        totalAttraction *= result;
                    }
                    //Log.Message("Adding factors.");
                    veryHighFactors.AddRange(newVeryHighFactors);
                    highFactors.AddRange(newHighFactors);
                    lowFactors.AddRange(newLowFactors);
                    veryLowFactors.AddRange(newVeryLowFactors);
                    //Log.Message("Finished adding factors.");
                }
            }
            public void Update(Pawn pawn, Pawn other)
            {
                lastRefreshedGameTick = Find.TickManager.TicksGame;
                veryHighFactors.Clear();
                highFactors.Clear();
                lowFactors.Clear();
                veryLowFactors.Clear();
                if (pawn == other)
                {
                    return;
                }
                float oldAttraction = totalAttraction;
                totalAttraction = 1f;
                foreach (AttractionFactorCategoryDef category in DefDatabase<AttractionFactorCategoryDef>.AllDefs)
                {
                    AttractionFactorDef whoCares;
                    List<AttractionFactorDef> newVeryHighFactors;
                    List<AttractionFactorDef> newHighFactors;
                    List<AttractionFactorDef> newLowFactors;
                    List<AttractionFactorDef> newVeryLowFactors;
                    if (category.alwaysRecalculate || category.chanceOnly)
                    {
                        continue;
                    }
                    else
                    {
                        float result = AttractionUtility.CalculateAttractionCategory(category, pawn, other, out newVeryLowFactors, out newLowFactors, out newHighFactors, out newVeryHighFactors, out whoCares);

                        categoryCalculations[category] = result;
                        totalAttraction *= result;
                    }
                    veryHighFactors.AddRange(newVeryHighFactors);
                    highFactors.AddRange(newHighFactors);
                    lowFactors.AddRange(newLowFactors);
                    veryLowFactors.AddRange(newVeryLowFactors);
                }
            }
            public void RetrieveFactors(out List<AttractionFactorDef> veryLowFactors2, out List<AttractionFactorDef> lowFactors2, out List<AttractionFactorDef> highFactors2, out List<AttractionFactorDef> veryHighFactors2)
            {
                veryLowFactors2 = new List<AttractionFactorDef>();
                lowFactors2 = new List<AttractionFactorDef>();
                highFactors2 = new List<AttractionFactorDef>();
                veryHighFactors2 = new List<AttractionFactorDef>();
                veryLowFactors2.AddRange(veryLowFactors);
                lowFactors2.AddRange(lowFactors);
                highFactors2.AddRange(highFactors);
                veryHighFactors2.AddRange(veryHighFactors);
            }
            public float RetrieveAttraction(bool romantic = false, bool chanceOnly = false)
            {
                float attraction = 1f;
                foreach (AttractionFactorCategoryDef category in categoryCalculations.Keys)
                {
                    if (!romantic && category.onlyForRomance)
                    {
                        continue;
                    }
                    if (!chanceOnly && category.chanceOnly)
                    {
                        continue;
                    }
                    attraction *= categoryCalculations[category];
                }
                return attraction;
            }
            public float RetrieveAttractionForCategory(AttractionFactorCategoryDef category)
            {
                if (category.alwaysRecalculate)
                {
                    Log.Error("[Gradual_Romance] Tried to pull a record for " + category.defName + ", but category is set to AlwaysRecalculate and is never stored.");
                    return 1f;
                }
                return categoryCalculations[category];
            }

        }
        private const int recalculateAttractionPerTick = 5000;
        private const int recachePerTick = 2500;
        private Dictionary<Pawn, AttractionRecord> AttractionRecords = new Dictionary<Pawn, AttractionRecord>();
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
