using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class RomanticRelationExtension : DefModExtension
    { 
        public float baseAffairReluctance = 1f;
        public float baseRomanceChance = 0f;
        public string newRelationshipLetterText;
        public string newRelationshipTitleText;
        public PawnRelationDef ex;
        public bool decayable = false;
        public bool isFormalRelationship;
        public bool needsDivorce = false;
        public bool goesOnDates;
        public bool sharesBed;
        public bool doesLovin;
        public bool caresAboutCheating;
        public List<ThoughtCondition> conditions = new List<ThoughtCondition>() { };
        public int relationshipLevel;
    }

    public class ThoughtCondition
    {
        public ThoughtDef thought;
        public int numberRequired;
    }
}
