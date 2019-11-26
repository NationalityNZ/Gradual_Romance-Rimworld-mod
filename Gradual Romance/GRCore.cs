using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;
using HugsLib;

namespace Gradual_Romance
{
    public class GradualRomanceMod : ModBase
    {
        public override string ModIdentifier
        {
            get
            {
                return "GradualRomance";
            }
        }

        //Settings to change the default gender changes
        public static Gender alteredGender = Gender.Female;
        public enum GenderModeSetting
        {
            Vanilla,
            None,
            Inverse

        };
        public static GenderModeSetting genderMode;
        public static Gender AlteredGender()
        {
            return alteredGender;
        }
        public enum AttractionCalculationSetting
        {
            Vanilla,
            Simplified,
            Complex

        };
        public static AttractionCalculationSetting AttractionCalculation = AttractionCalculationSetting.Complex;

        private static int baseRomanceChance;
        public static float BaseRomanceChance
        {
            get
            {
                return Mathf.Max(0f, baseRomanceChance) * 0.01f;
            }
        }
        private static int baseBreakupChance;
        public static float BaseBreakupChance
        {
            get
            {
                return Mathf.Max(0f, baseBreakupChance) * 0.01f;
            }
        }
        private static int baseFlirtChance;

        public static float BaseFlirtChance
        {
            get
            {
                return Mathf.Max(0f, baseFlirtChance) * 0.01f;
            }
        }

        private static int romanticSuccessRate;

        public static float RomanticSuccessRate
        {
            get
            {
                return Mathf.Max(0f, romanticSuccessRate) * 0.01f;
            }
        }
        private static int sweetheartRate;
        public static float SweetheartRate
        {
            get
            {
                return Mathf.Max(0f, sweetheartRate) * 0.01f;
            }
        }

        public static bool detailedAttractionLogs;

        public static bool rerollBeautyTraits;

        public static bool polygamousWorld;
        public static int numberOfRelationships;
        public static bool informalRomanceLetters;
        public static bool useFacialAttractiveness;
        public static bool detailedDebugLogs;


        public override void DefsLoaded()
        {
            
            AttractionCalculation = Settings.GetHandle("GRAttractionCalculationSetting", "AttractionCalculationSetting_title".Translate(), "AttractionCalculationSetting_desc".Translate(), AttractionCalculationSetting.Complex, null, "AttractionCalculationSetting_");
            genderMode = Settings.GetHandle("GRGenderModeSetting", "GenderModeSetting_title".Translate(), "GenderModeSetting_desc".Translate(), GenderModeSetting.Vanilla, null, "GenderModeSetting_");
            baseRomanceChance = Settings.GetHandle<int>("GRbaseRomanceChance", "BaseRomanceChance_title".Translate(), "BaseRomanceChance_desc".Translate(), 100);
            baseBreakupChance = Settings.GetHandle<int>("GRbaseBreakupChance", "BaseBreakupChance_title".Translate(), "BaseBreakupChance_desc".Translate(), 100);
            baseFlirtChance = Settings.GetHandle<int>("GRbaseFlirtChance", "BaseFlirtChance_title".Translate(), "BaseFlirtChance_desc".Translate(), 100);
            sweetheartRate = Settings.GetHandle<int>("GRsweetheartRate", "SweetheartRate_title".Translate(), "SweetheartRate_desc".Translate(), 100);
            romanticSuccessRate = Settings.GetHandle<int>("GRromanticSuccessRate", "RomanceSuccessRate_title".Translate(), "RomanceSuccessRate_desc".Translate(), 100);
            numberOfRelationships = Settings.GetHandle<int>("GRnumberOfRelationships", "NumberOfRelationships_title".Translate(), "NumberOfRelationships_desc".Translate(), 3);
            polygamousWorld = Settings.GetHandle<bool>("GRpolygamousWorld", "PolygamousWorld_title".Translate(), "PolygamousWorld_desc".Translate(), false);
            rerollBeautyTraits = Settings.GetHandle<bool>("GRrerollBeautyTraits", "RerollBeautyTraits_title".Translate(), "RerollBeautyTraits_desc".Translate(),false);
            informalRomanceLetters = Settings.GetHandle<bool>("GRinformalRomanceLetters", "InformalLetters_title".Translate(), "InformalLetters_desc".Translate(), true);
            useFacialAttractiveness = Settings.GetHandle<bool>("GRuseFacialAttractiveness", "UseFacialAttractiveness_title".Translate(), "UseFacialAttractiveness_desc".Translate(), false);
            detailedDebugLogs = Settings.GetHandle<bool>("GRdetailedDebugLogs", "DetailedDebugLog_title".Translate(), "DetailedDebugLog_desc".Translate(), false);
            detailedAttractionLogs = Settings.GetHandle<bool>("GRdetailedAttractionLogs", "DetailedAttractionLog_title".Translate(), "DetailedAttractionLog_desc".Translate(), false);
            List<FlirtStyleDef> allDefsListForReading = DefDatabase<FlirtStyleDef>.AllDefsListForReading;
            Logger.Message("Gradual Romance loaded with " + allDefsListForReading.Count().ToString() + " flirt styles.");

            //Only give GRPawnComp to pawns that have a psychecomp
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
            {
                bool addComp = false;
                if (def.comps != null)
                {
                    for (int i = 0; i < def.comps.Count(); i++)
                    {
                        if (def.comps[i] is Psychology.CompProperties_Psychology)
                        {
                            addComp = true;
                            break;
                        }
                    }
                    if (addComp == true)
                    {
                        def.comps.Add(new GRPawnComp_Properties());
                        if (def.modExtensions == null)
                        {
                            def.modExtensions = new List<DefModExtension> { };
                        }
                        if (!def.HasModExtension<XenoRomanceExtension>())
                        {
                            Logger.Message("Autopatching " + def.defName);
                            def.modExtensions.Add(CreateXenoRomanceExtensionFor(def));
                        }

                        maleSexDriveCurves.Add(def, CreateCurveFrom(def.GetModExtension<XenoRomanceExtension>().sexDriveByAgeCurveMale));
                        femaleSexDriveCurves.Add(def, CreateCurveFrom(def.GetModExtension<XenoRomanceExtension>().sexDriveByAgeCurveFemale));
                        maleAttractivenessCurves.Add(def, CreateCurveFrom(def.GetModExtension<XenoRomanceExtension>().attractivenessByAgeCurveMale));
                        femaleAttractivenessCurves.Add(def, CreateCurveFrom(def.GetModExtension<XenoRomanceExtension>().attractivenessByAgeCurveFemale));
                    }
                }
            }

        }
        
        public static XenoRomanceExtension CreateXenoRomanceExtensionFor(ThingDef pawnType)
        {
            
                XenoRomanceExtension xenoRomance = new XenoRomanceExtension() { };
                float maxAge = pawnType.race.lifeExpectancy;
                xenoRomance.peakMaturityAge = (maxAge * 0.5f); 
                //xenoRomance.subspeciesOf = null;
                xenoRomance.extraspeciesAppeal = 0.5f;
                xenoRomance.sexDriveByAgeCurveFemale.Add(new Vector2(maxAge * 0.15f, 0));
                xenoRomance.sexDriveByAgeCurveFemale.Add(new Vector2(maxAge * 0.25f, 3));
                xenoRomance.sexDriveByAgeCurveFemale.Add(new Vector2(maxAge * 0.75f, 1));
                xenoRomance.sexDriveByAgeCurveMale.Add(new Vector2(maxAge * 0.15f, 0));
                xenoRomance.sexDriveByAgeCurveMale.Add(new Vector2(maxAge * 0.25f, 3));
                xenoRomance.sexDriveByAgeCurveMale.Add(new Vector2(maxAge * 0.75f, 1));
                xenoRomance.attractivenessByAgeCurveFemale.Add(new Vector2(maxAge * 0.15f, 0));
                xenoRomance.attractivenessByAgeCurveFemale.Add(new Vector2(maxAge * 0.25f, 3));
                xenoRomance.attractivenessByAgeCurveFemale.Add(new Vector2(maxAge * 0.75f, 1));
                xenoRomance.attractivenessByAgeCurveMale.Add(new Vector2(maxAge * 0.15f, 0));
                xenoRomance.attractivenessByAgeCurveMale.Add(new Vector2(maxAge * 0.25f, 3));
                xenoRomance.attractivenessByAgeCurveMale.Add(new Vector2(maxAge * 0.75f, 1));
                return xenoRomance;



        }

        public static SimpleCurve CreateCurveFrom (List<Vector2> curvePoints)
        {
            SimpleCurve curve = new SimpleCurve();
            for (int i = 0; i < curvePoints.Count(); i++)
            {
                curve.Add(new CurvePoint(curvePoints[i]));
            }
            return curve;
        }

        public static SimpleCurve GetAttractivenessCurveFor(Pawn pawn)
        {
            ThingDef def = pawn.def;
            Gender gender = pawn.gender;
            if (gender == Gender.Female)
            {
                if (femaleAttractivenessCurves.Keys.Contains(def))
                {
                    return femaleAttractivenessCurves[def];
                }
            }
            if (gender == Gender.Male)
            {
                if (maleAttractivenessCurves.Keys.Contains(def))
                {
                    return maleAttractivenessCurves[def];
                }
            }

            return null;
        }
        public static SimpleCurve GetSexDriveCurveFor(Pawn pawn)
        {
            ThingDef def = pawn.def;
            Gender gender = pawn.gender;
            if (gender == Gender.Female)
            {
                if (femaleSexDriveCurves.Keys.Contains(def))
                {
                    return femaleSexDriveCurves[def];
                }
            }
            if (gender == Gender.Male)
            {
                if (maleSexDriveCurves.Keys.Contains(def))
                {
                    return maleSexDriveCurves[def];
                }
            }

            return null;
        }


        private static Dictionary<ThingDef, SimpleCurve> maleSexDriveCurves = new Dictionary<ThingDef, SimpleCurve> { };

        private static Dictionary<ThingDef, SimpleCurve> femaleSexDriveCurves = new Dictionary<ThingDef, SimpleCurve> { };

        private static Dictionary<ThingDef, SimpleCurve> maleAttractivenessCurves = new Dictionary<ThingDef, SimpleCurve> { };

        private static Dictionary<ThingDef, SimpleCurve> femaleAttractivenessCurves = new Dictionary<ThingDef, SimpleCurve> { };

    }
}

