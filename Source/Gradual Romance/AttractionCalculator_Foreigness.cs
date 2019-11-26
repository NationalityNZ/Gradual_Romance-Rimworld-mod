using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Gradual_Romance
{
    public class AttractionCalculator_Foreigness : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            return (observer.Faction != assessed.Faction);
        }
        public override float Calculate(Pawn observer, Pawn assessed)
        {
            
            if (assessed.Faction.HostileTo(observer.Faction))
            {
                return hostileFactionModifier;
            }
            return normalFactionModifier;
        }

        private const float hostileFactionModifier = 0.65f;
        private const float normalFactionModifier = 0.8f;
    }
}
