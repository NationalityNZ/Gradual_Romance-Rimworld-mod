using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using Psychology;

namespace Gradual_Romance
{
    public class AttractionCalculator_Beauty : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            return (observer.def.GetModExtension<XenoRomanceExtension>().faceCategory == assessed.def.GetModExtension<XenoRomanceExtension>().faceCategory);

        }



        public override float Calculate(Pawn observer, Pawn assessed)
        {
            int beauty = 0;
            float beautyFactor;
            if (assessed.RaceProps.Humanlike)
            {
                beauty = assessed.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
            }
            beauty = 0;

            switch (beauty)
            {
                case 4:
                    beautyFactor = 2.5f;
                    break;
                case 3:
                    beautyFactor = 2f;
                    break;
                case 2:
                    beautyFactor = 1.5f;
                    break;
                case 1:
                    beautyFactor = 1.2f;
                    break;
                case -1:
                    beautyFactor = 0.8f;
                    break;
                case -2:
                    beautyFactor = 0.5f;
                    break;
                case -3:
                    beautyFactor = 0.25f;
                    break;
                case -4:
                    beautyFactor = 0.05f;
                    break;
                default:
                    beautyFactor = 1f;
                    break;

            }
            return beautyFactor;

        }
    }
}
