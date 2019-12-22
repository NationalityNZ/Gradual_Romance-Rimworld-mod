using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Gradual_Romance
{
    
    public static class RelationshipUtility
    {
        public static bool IsPolygamist(Pawn pawn)
        {
            return (pawn.story.traits.HasTrait(Psychology.TraitDefOfPsychology.Polygamous) || GradualRomanceMod.polygamousWorld);
        }
        public static List<PawnRelationDef> ListOfBreakupRelationships()
        {
            List<PawnRelationDef> list = new List<PawnRelationDef> { PawnRelationDefOf.Fiance, PawnRelationDefOf.Lover, PawnRelationDefOfGR.Lovefriend, PawnRelationDefOfGR.Paramour, PawnRelationDefOfGR.Lovebuddy, PawnRelationDefOfGR.Darling, PawnRelationDefOfGR.Flame, PawnRelationDefOfGR.Steady, PawnRelationDefOfGR.Suitor };
            return list;
        }
        public static List<PawnRelationDef> ListOfRomanceStages()
        {
            List<PawnRelationDef> list = new List<PawnRelationDef> { PawnRelationDefOf.Spouse, PawnRelationDefOf.Fiance, PawnRelationDefOf.Lover, PawnRelationDefOfGR.Lovefriend, PawnRelationDefOfGR.Paramour, PawnRelationDefOfGR.Darling, PawnRelationDefOfGR.Suitor, PawnRelationDefOfGR.Flame, PawnRelationDefOfGR.Sweetheart, PawnRelationDefOfGR.Steady, PawnRelationDefOfGR.Lovebuddy };
            return list;
        }
        public static List<PawnRelationDef> ListOfFormalRelationships()
        {
            List<PawnRelationDef> list = new List<PawnRelationDef> { PawnRelationDefOf.Spouse, PawnRelationDefOf.Fiance, PawnRelationDefOf.Lover, PawnRelationDefOfGR.Lovefriend };
            return list;
        }
        public static List<PawnRelationDef> ListOfRomanceAndExStages()
        {
            List<PawnRelationDef> list = new List<PawnRelationDef> { PawnRelationDefOf.Spouse, PawnRelationDefOf.ExSpouse, PawnRelationDefOf.Fiance, PawnRelationDefOf.Lover, PawnRelationDefOf.ExLover, PawnRelationDefOfGR.Lovefriend, PawnRelationDefOfGR.ExLovefriend, PawnRelationDefOfGR.Paramour, PawnRelationDefOfGR.Darling, PawnRelationDefOfGR.Suitor, PawnRelationDefOfGR.Flame, PawnRelationDefOfGR.Sweetheart, PawnRelationDefOfGR.Steady, PawnRelationDefOfGR.Lovebuddy };
            return list;
        }
        public static Pawn PawnWithMostAdvancedRelationship(Pawn pawn, out PawnRelationDef relation, bool mostLikedLover = true)
        {
            List<PawnRelationDef> listOfRomanceStages = ListOfRomanceStages();
            Pawn lover = null;
            relation = null;
            for (int i = 0; i < listOfRomanceStages.Count(); i++)
            {
                relation = listOfRomanceStages[i];
                List<Pawn> lovers = GetAllPawnsWithGivenRelationshipTo(pawn, listOfRomanceStages[i]);
                if (lovers.Count() > 0)
                {
                    for (int i2 = 0; i < lovers.Count(); i++)
                    {
                        if (lover == null)
                        {
                            lover = lovers[i2];
                        }
                        else if (mostLikedLover == false && lover.relations.OpinionOf(pawn) < lovers[i2].relations.OpinionOf(pawn))
                        {
                            lover = lovers[i2];
                        }
                        else if (mostLikedLover == true && pawn.relations.OpinionOf(lover) < pawn.relations.OpinionOf(lovers[i2]))
                        {
                            lover = lovers[i2];
                        }

                    }
                    break;
                }

            }
            return lover;
        }
        //returns the most logical romance stage for the given romance stage.
        public static Pawn MostAdvancedRelationship(Pawn pawn)
        {
            List<PawnRelationDef> listOfRomanceStages = ListOfRomanceStages();
            Pawn lover = null;
            for (int i = 0; i < listOfRomanceStages.Count(); i++)
            {
                lover = pawn.relations.GetFirstDirectRelationPawn(listOfRomanceStages[i]);
                if (lover != null)
                {
                    return pawn.relations.GetFirstDirectRelationPawn(listOfRomanceStages[i]);
                }

            }
            return null;
        }
        public static PawnRelationDef MostAdvancedRelationshipBetween(Pawn pawn, Pawn other)
        {
            List<PawnRelationDef> listOfRomanceStages = ListOfRomanceStages();
            PawnRelationDef relation = null;
            for (int i = 0; i < listOfRomanceStages.Count(); i++)
            {
                relation = listOfRomanceStages[i];
                if (pawn.relations.DirectRelationExists(relation, other))
                {
                    return relation;
                }
            }
            return null;
        }
        public static bool IsAnAffair(Pawn pawn, Pawn other, out Pawn pawnSO, out Pawn otherSO)
        {
            pawnSO = null;
            otherSO = null;
            if (GradualRomanceMod.polygamousWorld) { return false; }
            List<PawnRelationDef> listOfFormalRelationships = ListOfFormalRelationships();
            for (int i = 0; i < listOfFormalRelationships.Count(); i++)
            {
                if (!CaresAboutCheating(listOfFormalRelationships[i]))
                {
                    continue;
                }
                List<Pawn> pawns = GetAllPawnsWithGivenRelationshipTo(pawn, listOfFormalRelationships[i]);
                for (int i2 = 0; i2 < pawns.Count(); i2++)
                {
                    Pawn SO = pawns[i];
                    if (!SO.Dead && !IsPolygamist(SO) && !SO.IsColonist)
                    {
                        pawnSO = SO;
                        break;
                    }
                    
                }
                if (pawnSO != null)
                {
                    break;
                }
            }
            for (int i = 0; i < listOfFormalRelationships.Count(); i++)
            {
                List<Pawn> pawns = GetAllPawnsWithGivenRelationshipTo(other, listOfFormalRelationships[i]);
                for (int i2 = 0; i2 < pawns.Count(); i2++)
                {
                    Pawn SO = pawns[i];
                    if (!SO.Dead && !IsPolygamist(SO) && !SO.IsColonist)
                    {
                        otherSO = SO;
                        break;
                    }

                }
                if (otherSO != null)
                {
                    break;
                }
            }
            if (pawnSO != null || otherSO != null)
            {
                return true;
            }
            return false;
        }
        public static PawnRelationDef CurrentRomanceStage(Pawn pawn, Pawn other)
        {
            List<PawnRelationDef> listOfRomanceStages = ListOfRomanceStages();
            for (int i = 0; i < listOfRomanceStages.Count(); i++)
            {
                if (pawn.relations.GetDirectRelation(listOfRomanceStages[i],other) != null)
                {
                    return listOfRomanceStages[i];
                }
            }
            return null;
        }
        public static PawnRelationDef MostAdvancedRomanceOrExStage(Pawn pawn, Pawn other)
        {
            List<PawnRelationDef> listOfRomanceStages = ListOfRomanceAndExStages();
            for (int i = 0; i < listOfRomanceStages.Count(); i++)
            {
                if (pawn.relations.GetDirectRelation(listOfRomanceStages[i], other) != null)
                {
                    return listOfRomanceStages[i];
                }
            }
            return null;
        }
        public static float AffairReluctance(PawnRelationDef currentStage)
        {
            try
            {
                return currentStage.GetModExtension<RomanticRelationExtension>().baseAffairReluctance;
            }
            catch (NullReferenceException)
            {
                return 1f;
            }
        }
        public static bool IsRomanticOrSexualRelationship(PawnRelationDef pawnRelation)
        {
            try
            {
                return (pawnRelation.GetModExtension<RomanticRelationExtension>().goesOnDates || pawnRelation.GetModExtension<RomanticRelationExtension>().doesLovin);
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
        public static bool IsRomanticRelationship(PawnRelationDef pawnRelation)
        {
            try
            {
                return (pawnRelation.GetModExtension<RomanticRelationExtension>().goesOnDates);
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
        public static bool IsSexualRelationship(PawnRelationDef pawnRelation)
        {
            try
            {
                return (pawnRelation.GetModExtension<RomanticRelationExtension>().doesLovin);
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
        public static bool IsInformalRelationship(PawnRelationDef pawnRelation)
        {
            try
            {
                return (!pawnRelation.GetModExtension<RomanticRelationExtension>().isFormalRelationship);
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
        public static bool HasInformalRelationship(Pawn pawn, Pawn other)
        {
            PawnRelationDef relation = CurrentRomanceStage(pawn, other);
            if (relation == null)
            {
                return false;
            }
            try
            {
                return (!relation.GetModExtension<RomanticRelationExtension>().isFormalRelationship);
            }
            catch (NullReferenceException)
            {
                return false;
            }
            
        }
        public static bool CaresAboutCheating(PawnRelationDef pawnRelation)
        {
            try
            {
                return (pawnRelation.GetModExtension<RomanticRelationExtension>().caresAboutCheating);
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
        public static bool IsBedSharingRelationship(PawnRelationDef pawnRelation)
        {
            try
            {
                return (pawnRelation.GetModExtension<RomanticRelationExtension>().sharesBed);
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
        public static List<Pawn> GetAllPawnsWithGivenRelationshipTo (Pawn pawn, PawnRelationDef relation)
        {
            List<Pawn> pawnList = new List<Pawn>{ };
            List<DirectPawnRelation> directRelations = pawn.relations.DirectRelations;
            for (int i = 0; i > directRelations.Count(); i++)
            {
                DirectPawnRelation thisRelation = directRelations[i];
                if (thisRelation.def == relation)
                {
                    pawnList.Add(thisRelation.otherPawn);
                }
            }
            return pawnList;
        }
        public static List<Pawn> GetAllPawnsRomanticWith(Pawn pawn)
        {
            List<PawnRelationDef> relationsList = ListOfRomanceStages();
            List<Pawn> loversList = new List<Pawn>() { };
            for (int i = 0; i < 0; i ++)
            {
                List<Pawn> newLovers = GetAllPawnsWithGivenRelationshipTo(pawn, relationsList[i]);
                for (int i2 = 0; i2 < newLovers.Count(); i++)
                {
                    loversList.Add(newLovers[i2]);
                }
            }
            return loversList;
        }
        public static bool ShouldShareBed(Pawn pawn, Pawn other)
        {
            PawnRelationDef relation = CurrentRomanceStage(pawn, other);
            return IsBedSharingRelationship(relation);
        }
        public static DirectPawnRelation MostLikedBedSharingRelationship(Pawn pawn, bool allowDead)
        {
            if (!pawn.RaceProps.IsFlesh)
            {
                return null;
            }
            DirectPawnRelation directPawnRelation = null;
            int num = int.MinValue;
            List<DirectPawnRelation> directRelations = pawn.relations.DirectRelations;
            for (int i = 0; i < directRelations.Count; i++)
            {
                if (allowDead || !directRelations[i].otherPawn.Dead)
                {
                    if (IsBedSharingRelationship(directRelations[i].def))
                    {
                        int num2 = pawn.relations.OpinionOf(directRelations[i].otherPawn);
                        if (directPawnRelation == null || num2 > num)
                        {
                            directPawnRelation = directRelations[i];
                            num = num2;
                        }
                    }
                }
            }
            return directPawnRelation;

        }
        public static Pawn MostLikedBedSharingPawn(Pawn pawn, bool allowDead)
        {
            DirectPawnRelation directPawnRelation = MostLikedBedSharingRelationship(pawn, allowDead);
            if (directPawnRelation != null)
            {
                return directPawnRelation.otherPawn;
            }
            return null;
        }
        public static void AdvanceRelationship(Pawn pawn, Pawn other, PawnRelationDef newRelation)
        {
            PawnRelationDef oldRelation = MostAdvancedRelationshipBetween(pawn, other);
            if (!pawn.relations.DirectRelationExists(newRelation, other))
            {
                pawn.relations.AddDirectRelation(newRelation, other);
                if (oldRelation != null)
                {
                    pawn.relations.TryRemoveDirectRelation(oldRelation, other);
                }
            }
        }
        //TODO Should add more sophisticated behavior
        public static int NumberOfFriends(Pawn pawn)
        {
            if (pawn.MapHeld != null && pawn.IsColonist)
            { 
            IEnumerable<Pawn> allPawns = pawn.MapHeld.mapPawns.FreeColonists;
            int numOfFriends = (from friend in allPawns
                                        where (friend != pawn && friend.IsColonist && !friend.Dead && friend.relations.OpinionOf(pawn) >= Pawn_RelationsTracker.FriendOpinionThreshold)
                                        select friend).Count();
                return numOfFriends;
            }
            else
            {
                return -1;
            }
        }
        public static bool RelationshipCanEvolveTo(Pawn pawn, Pawn other, PawnRelationDef newRelation)
        {
            List<ThoughtCondition> conditions;
            try
            {
                conditions = newRelation.GetModExtension<RomanticRelationExtension>().conditions;
            }
            catch (NullReferenceException)
            {
                return false;
            }
            if (conditions.NullOrEmpty())
            {
                return false;
            }
            if (!AttractionUtility.IsAgeAppropriate(pawn) || !AttractionUtility.IsAgeAppropriate(other))
            {
                try
                {
                    if (newRelation.GetModExtension<RomanticRelationExtension>().doesLovin)
                    {
                        return false;
                    }
                }
                catch (NullReferenceException)
                {
                    return false;
                }
            }
            for (int i = 0; i < conditions.Count(); i++)
            {
                Log.Message(conditions[i].thought.defName + ": " + pawn.needs.mood.thoughts.memories.NumMemoriesOfDef(conditions[i].thought).ToString() + " " + other.needs.mood.thoughts.memories.NumMemoriesOfDef(conditions[i].thought) + " needs " + conditions[i].numberRequired.ToString());
                
                if (GRThoughtUtility.NumOfMemoriesOfDefWhereOtherPawnIs(pawn, other, conditions[i].thought) < conditions[i].numberRequired || GRThoughtUtility.NumOfMemoriesOfDefWhereOtherPawnIs(other, pawn, conditions[i].thought) < conditions[i].numberRequired)
                {
                    return false;
                }
            }
            return true;
        }
        public static int GetRelationLevel(PawnRelationDef relation)
        {
            try
            {
                return (relation.GetModExtension<RomanticRelationExtension>().relationshipLevel);
            }
            catch (NullReferenceException)
            {
                return -1;
            }
        }
        public static void AdvanceInformalRelationship(Pawn pawn, Pawn other, out PawnRelationDef newRelation, float sweetheartChance = 0.5f)
        {
            PawnRelationDef oldRelation = MostAdvancedRelationshipBetween(pawn, other);
            //if (!IsInformalRelationship(oldRelation))
            newRelation = null;
            int targetLevel = 1;
            if (oldRelation != null && IsInformalRelationship(oldRelation))
            {
                targetLevel = (oldRelation.GetModExtension<RomanticRelationExtension>().relationshipLevel + 1);
            }
            IEnumerable<PawnRelationDef> candidateRelations = from relation in DefDatabase<PawnRelationDef>.AllDefsListForReading
                                                              where GetRelationLevel(relation) == targetLevel
                                                              select relation;
            if (candidateRelations.Count() > 0)
            {
                foreach(PawnRelationDef relation in candidateRelations)
                {
                    Log.Message("Testing " + relation.defName);
                    if (RelationshipCanEvolveTo(pawn,other,relation))
                    {
                        if (oldRelation != null)
                        {
                            pawn.relations.TryRemoveDirectRelation(oldRelation, other);
                        }

                        pawn.relations.AddDirectRelation(relation, other);
                        newRelation = relation;
                        break;
                    }
                }
            }
        }
        public static int LevelOfTension(Pawn pawn, Pawn other)
        {
            /*List<Thought_Memory> memories = pawn.needs.mood.thoughts.memories.Memories;
            int count = 0;
            for (int i = 0; i < memories.Count(); i++)
            {
                if (GRThoughtUtility.IsTensionMemory(memories[i].def) && memories[i].otherPawn == other)
                {
                    count++;
                }
            }*/
            return GRThoughtUtility.NumOfMemoriesOfDefWhereOtherPawnIs(pawn, other, ThoughtDefOfGR.SexualTension) + GRThoughtUtility.NumOfMemoriesOfDefWhereOtherPawnIs(pawn, other, ThoughtDefOfGR.RomanticTension) + GRThoughtUtility.NumOfMemoriesOfDefWhereOtherPawnIs(pawn, other, ThoughtDefOfGR.LogicalTension);
        }
        public static int LevelOfSexualTension(Pawn pawn, Pawn other)
        {
            return (GRThoughtUtility.NumOfMemoriesOfDefWhereOtherPawnIs(pawn, other, ThoughtDefOfGR.SexualTension));
            
        }
        public static int LevelOfRomanticTension(Pawn pawn, Pawn other)
        {
            return (GRThoughtUtility.NumOfMemoriesOfDefWhereOtherPawnIs(pawn, other, ThoughtDefOfGR.RomanticTension));

        }
        public static int LevelOfLogicalTension(Pawn pawn, Pawn other)
        {
            return (GRThoughtUtility.NumOfMemoriesOfDefWhereOtherPawnIs(pawn, other, ThoughtDefOfGR.LogicalTension));

        }
        public static List<Pawn> PotentiallyJealousPawnsInLineOfSight(Pawn pawn, Pawn pawnToIgnore = null)
        {
            List<Pawn> lovers = GetAllPawnsRomanticWith(pawn);
            List<Pawn> loversInSight = new List<Pawn>() { };
            for (int i = 0; i < lovers.Count(); i++)
            {
                if (pawnToIgnore != null && lovers[i] == pawnToIgnore)
                {
                    continue;
                }
                if (GenSight.LineOfSight(pawn.Position,lovers[i].Position,pawn.Map,true))
                {
                    loversInSight.Add(lovers[i]);
                }
            }
            return loversInSight;
        }

    }
}
