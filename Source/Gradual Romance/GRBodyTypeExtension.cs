using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Gradual_Romance
{
    public class GRBodyTypeExtension : DefModExtension
    {

        public string bodyCategory = "Humanoid";

        public float attractivenessFactor = 1.0f;

        public Gender attractiveForGender;

    }
}
