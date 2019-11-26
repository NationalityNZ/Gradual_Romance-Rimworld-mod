using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class XenoRomanceExtension : DefModExtension
    {
        public float peakMaturityAge = 40f;
        public float extraspeciesAppeal = 0.8f;
        public List<Vector2> sexDriveByAgeCurveMale = new List<Vector2> { };
        public List<Vector2> sexDriveByAgeCurveFemale = new List<Vector2> { };
        public List<Vector2> attractivenessByAgeCurveMale = new List<Vector2> { };
        public List<Vector2> attractivenessByAgeCurveFemale = new List<Vector2> { };
        public ThingDef subspeciesOf = null;
        public string faceCategory = "Humanoid";
        public string bodyCategory = "Humanoid";
        public string mindCategory = "Humanoid";

    }
}
