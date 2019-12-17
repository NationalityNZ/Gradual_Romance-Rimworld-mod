using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;
using UnityEngine;

namespace Gradual_Romance
{
    public class AttractionCalculator_Incest : AttractionCalculator
    {
        public override bool Check(Pawn observer, Pawn assessed)
        {
            if (!observer.relations.FamilyByBlood.Contains(assessed))
            {
                return false;
            }
            //psychopathic lechers don't care at all
            if (observer.story.traits.HasTrait(TraitDefOfPsychology.Lecher) && observer.story.traits.HasTrait(TraitDefOf.Psychopath))
            {
                return false;
            }

            return true;
        }

        public override float Calculate(Pawn observer, Pawn assessed)
        {

            float incestFactor = 1f;
            List<PawnRelationDef> relations = PawnRelationUtility.GetRelations(observer, assessed).ToList();
            PawnRelationDef relation = relations[0];
            for (int i = 0; i < relations.Count(); i++)
            {
                if (relations[i].incestOpinionOffset > relation.incestOpinionOffset)
                {
                    relation = relations[i];
                }
            }
            incestFactor = 1f / (Mathf.Abs(relation.incestOpinionOffset) + 1);

            return incestFactor;
        }
    }
}
