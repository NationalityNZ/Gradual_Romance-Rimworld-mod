using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class CellQuery
    {
        public static IEnumerable<IntVec3> GetRandomCellSampleAround(Thing t, int numberOfSamples, int distance)
        {
            IntVec3 center = t.Position;
            CellRect area = new CellRect(center.x - distance, center.z - distance, distance * 2, distance * 2);
            if (!area.InBounds(t.Map))
            {
                area = area.ClipInsideMap(t.Map);
            }
            for (int i = 0; i < numberOfSamples; i++)
            {
                yield return area.RandomCell;
            }
        }
    }
}
