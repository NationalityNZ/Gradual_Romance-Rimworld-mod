using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class AttractionCalculator_RelationshipLimit : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            return (GRPawnRelationUtility.GetAllPawnsRomanticWith(observer).Count() >= GradualRomanceMod.numberOfRelationships && !GRPawnRelationUtility.GetAllPawnsRomanticWith(observer).Contains(assessed));
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            return (GradualRomanceMod.numberOfRelationships / (GRPawnRelationUtility.GetAllPawnsRomanticWith(observer).Count() + 1));
        }


    }
}
