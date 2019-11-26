using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Gradual_Romance
{
    
    class GRPawnRelationUtility
    {
        public static bool IsPolygamist(Pawn pawn)
        {
            return (pawn.story.traits.HasTrait(Psychology.TraitDefOfPsychology.Polygamous) || GradualRomanceMod.polygamousWorld);
        }

        public static List<PawnRelationDef> ListOfBreakupRelationships()
        {
            List<PawnRelationDef> list = new List<PawnRelationDef> { PawnRelationDefOf.Fiance, PawnRelationDefOf.Lover, PawnRelationDefOfGR.Lovefriend, PawnRelationDefOfGR.Paramour, PawnRelationDefOfGR.Lovebuddy };
            return list;
        }
        public static List<PawnRelationDef> ListOfRomanceStages()
        {
            List<PawnRelationDef> list = new List<PawnRelationDef> { PawnRelationDefOf.Spouse, PawnRelationDefOf.Fiance, PawnRelationDefOf.Lover, PawnRelationDefOfGR.Lovefriend, PawnRelationDefOfGR.Paramour, PawnRelationDefOfGR.Sweetheart, PawnRelationDefOfGR.Lovebuddy };
            return list;
        }
        public static List<PawnRelationDef> ListOfFormalRelationships()
        {
            List<PawnRelationDef> list = new List<PawnRelationDef> { PawnRelationDefOf.Spouse, PawnRelationDefOf.Fiance, PawnRelationDefOf.Lover, PawnRelationDefOfGR.Lovefriend };
            return list;
        }

        public static List<PawnRelationDef> ListOfRomanceAndExStages()
        {
            List<PawnRelationDef> list = new List<PawnRelationDef> { PawnRelationDefOf.Spouse, PawnRelationDefOf.ExSpouse, PawnRelationDefOf.Fiance, PawnRelationDefOf.Lover, PawnRelationDefOf.ExLover, PawnRelationDefOfGR.Lovefriend, PawnRelationDefOfGR.ExLovefriend, PawnRelationDefOfGR.Paramour, PawnRelationDefOfGR.Sweetheart, PawnRelationDefOfGR.Lovebuddy };
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
        public static PawnRelationDef NextRomanceStage(PawnRelationDef currentStage)
        {
            if (currentStage == PawnRelationDefOf.Fiance)
            {
                return PawnRelationDefOf.Spouse;
            }
            else if (currentStage == PawnRelationDefOf.Lover)
            {
                return PawnRelationDefOf.Fiance;
            }
            else if (currentStage == PawnRelationDefOfGR.Lovefriend)
            {
                return PawnRelationDefOf.Lover;
            }
            else if (currentStage == PawnRelationDefOfGR.Sweetheart)
            {
                return PawnRelationDefOfGR.Lovefriend;
            }
            else if (currentStage == PawnRelationDefOfGR.Lovebuddy)
            {
                return PawnRelationDefOfGR.Lovefriend;
            }
            else
            {
                return null;
            }
        }
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
            if (currentStage == PawnRelationDefOf.Spouse)
            {
                return 0.3f;
            }
            else if (currentStage == PawnRelationDefOf.Fiance)
            {
                return 0.1f;
            }
            else if (currentStage == PawnRelationDefOf.Lover)
            {
                return 0.3f;
            }
            else if (currentStage == PawnRelationDefOfGR.Lovefriend)
            {
                return 0.5f;
            }
            else if (currentStage == PawnRelationDefOfGR.Paramour)
            {
                return 0.75f;
            }
            else if (currentStage == PawnRelationDefOfGR.Sweetheart)
            {
                return 0.85f;
            }
            else
            {
                return 1f;
            }
        }
        public static bool IsRomanticOrSexualRelationship(PawnRelationDef pawnRelation)
        {
            return (LovePartnerRelationUtility.IsLovePartnerRelation(pawnRelation) || pawnRelation == PawnRelationDefOfGR.Lovebuddy || pawnRelation == PawnRelationDefOfGR.Sweetheart || pawnRelation == PawnRelationDefOfGR.Paramour);
        }
        public static bool IsRomanticRelationship(PawnRelationDef pawnRelation)
        {
            return (LovePartnerRelationUtility.IsLovePartnerRelation(pawnRelation) || pawnRelation == PawnRelationDefOfGR.Sweetheart || pawnRelation == PawnRelationDefOfGR.Paramour);
        }
        public static bool IsSexualRelationship(PawnRelationDef pawnRelation)
        {
            return (LovePartnerRelationUtility.IsLovePartnerRelation(pawnRelation) || pawnRelation == PawnRelationDefOfGR.Lovebuddy || pawnRelation == PawnRelationDefOfGR.Paramour);
        }
        public static bool IsInformalRelationship(PawnRelationDef pawnRelation)
        {
            return (pawnRelation == PawnRelationDefOfGR.Sweetheart || pawnRelation == PawnRelationDefOfGR.Lovebuddy || pawnRelation == PawnRelationDefOfGR.Paramour);
        }
        public static bool HasInformalRelationship(Pawn pawn, Pawn other)
        {
            return (pawn.relations.GetDirectRelation(PawnRelationDefOfGR.Lovebuddy, other) != null || pawn.relations.GetDirectRelation(PawnRelationDefOfGR.Sweetheart, other) != null || pawn.relations.GetDirectRelation(PawnRelationDefOfGR.Paramour, other) != null) ;
        }
        public static bool IsBedSharingRelationship(PawnRelationDef pawnRelation)
        {
            return (pawnRelation == PawnRelationDefOf.Spouse || pawnRelation == PawnRelationDefOf.Fiance || pawnRelation == PawnRelationDefOf.Lover);
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
            return pawn.relations.DirectRelationExists(PawnRelationDefOf.Spouse, other) || pawn.relations.DirectRelationExists(PawnRelationDefOf.Fiance, other) || pawn.relations.DirectRelationExists(PawnRelationDefOf.Lover, other);
        }
        //returns the most liked character who the pawn should/is sleeping with
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
        public static void AdvanceInformalRelationship(Pawn pawn, Pawn other, out PawnRelationDef newRelation, float sweetheartChance = 0.5f)
        {
            PawnRelationDef oldRelation = MostAdvancedRelationshipBetween(pawn, other);
            newRelation = null;
            if (oldRelation == null)
            {
                if (AttractionUtility.IsAgeAppropriate(pawn) && AttractionUtility.IsAgeAppropriate(other))
                {
                    if (Rand.Value < sweetheartChance)
                    {
                        pawn.relations.AddDirectRelation(PawnRelationDefOfGR.Sweetheart, other);
                        other.relations.AddDirectRelation(PawnRelationDefOfGR.Sweetheart, pawn);
                        newRelation = PawnRelationDefOfGR.Sweetheart;
                    }
                    else
                    {
                        pawn.relations.AddDirectRelation(PawnRelationDefOfGR.Lovebuddy, other);
                        other.relations.AddDirectRelation(PawnRelationDefOfGR.Lovebuddy, pawn);
                        newRelation = PawnRelationDefOfGR.Lovebuddy;
                    }

                }
                else
                {
                    pawn.relations.AddDirectRelation(PawnRelationDefOfGR.Sweetheart, other);
                    other.relations.AddDirectRelation(PawnRelationDefOfGR.Sweetheart, pawn);
                    newRelation = PawnRelationDefOfGR.Sweetheart;
                }
            }
            else if (oldRelation == PawnRelationDefOfGR.Lovebuddy || oldRelation == PawnRelationDefOfGR.Sweetheart)
            {
                pawn.relations.TryRemoveDirectRelation(oldRelation, other);
                other.relations.TryRemoveDirectRelation(oldRelation, pawn);
                pawn.relations.AddDirectRelation(PawnRelationDefOfGR.Paramour, other);
                other.relations.AddDirectRelation(PawnRelationDefOfGR.Paramour, pawn);
                newRelation = PawnRelationDefOfGR.Paramour;
            }
        }
        /// <summary>
        /// Returns the number of colonist friends (pawns with opinions higher than 20) that a given pawn has. If the pawn is not a colonist, this is returned as -1.
        /// </summary>
        /// <param name="pawn">pawn to be evaluated</param>
        /// <returns>number of friends</returns>
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

//        public static bool ShouldAdvanceRelationship(Pawn pawn, Pawn other, float modifier = 1f)
//        {
//
//            if Rand.Value > 
//        }
    }
}
