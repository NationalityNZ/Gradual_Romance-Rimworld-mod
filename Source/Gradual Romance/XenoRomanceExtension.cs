using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Psychology;

namespace Gradual_Romance
{
    public class XenoRomanceExtension : DefModExtension
    {
        //public float youngAdultAge = 18f;
        //public float midlifeAge = 40f;
        public float extraspeciesAppeal = 0.75f;
        //public float minimumAgeDeviation = 1f;
       // public float maximumAgeDeviation = 10f;
        public bool canGoIntoHeat = false;
        public int averageKinseyFemale = -1;
        public int averageKinseyMale = -1;
        public List<Vector2> sexDriveByAgeCurveMale = new List<Vector2> { };
        public List<Vector2> sexDriveByAgeCurveFemale = new List<Vector2> { };
        public List<Vector2> maturityByAgeCurveMale = new List<Vector2> { };
        public List<Vector2> maturityByAgeCurveFemale = new List<Vector2> { };
        //public List<Vector2> attractivenessByAgeCurveMale = new List<Vector2> { };
        //public List<Vector2> attractivenessByAgeCurveFemale = new List<Vector2> { };
        public ThingDef subspeciesOf = null;
        
        public string faceCategory = "Humanoid";
        public string bodyCategory = "Humanoid";
        public string mindCategory = "Humanoid";

    }
}
