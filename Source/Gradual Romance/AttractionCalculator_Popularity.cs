using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class AttractionCalculator_Popularity : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            if (!observer.IsColonist || !assessed.IsColonist)
            {
                return false;
            }
            return true;
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            //List<Pawn> allPawns = assessed.MapHeld.mapPawns.AllPawnsSpawned;
            float numOfAssessedFriends = 0f;
            float numOfObservedFriends = 0f;
            /*
            if (cachedValues)
            {
                numOfAssessedFriends = GRHelper.GRPawnComp(assessedPawn).cachedNumberOfColonyFriends;
                numOfObservedFriends = GRHelper.GRPawnComp(observerPawn).cachedNumberOfColonyFriends;
            }
            */
            numOfAssessedFriends = GRPawnRelationUtility.NumberOfFriends(assessed);
            numOfObservedFriends = GRPawnRelationUtility.NumberOfFriends(observer);
       
            float friendDifference = numOfAssessedFriends - numOfObservedFriends;
            if (friendDifference == 0f) { return 1f; }
            return Mathf.Pow(((numOfAssessedFriends + 1f) / (numOfObservedFriends + 1f)), FriendAttractionDampener);
        }

        private const float FriendAttractionDampener = 0.4f;
    }
}
