﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Gradual_Romance
{
    public class AttractionCalculator_Scars : AttractionCalculator
    {
        
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            return 1f;
        }
    }
}
