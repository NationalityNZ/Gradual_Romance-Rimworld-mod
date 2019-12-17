﻿using System;
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
            return (RelationshipUtility.GetAllPawnsRomanticWith(observer).Count() >= GradualRomanceMod.numberOfRelationships && !RelationshipUtility.GetAllPawnsRomanticWith(observer).Contains(assessed));
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            return (GradualRomanceMod.numberOfRelationships / (RelationshipUtility.GetAllPawnsRomanticWith(observer).Count() + 1));
        }


    }
}
