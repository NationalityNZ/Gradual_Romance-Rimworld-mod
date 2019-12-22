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
using HugsLib.Settings;

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
        private SettingHandle<GenderModeSetting> settingGenderMode;
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
        private SettingHandle<AttractionCalculationSetting> settingAttractionCalculation;

        public enum SeductionModeSetting
        {
            NoRestriction,
            RelationshipAndNonColonists,
            OnlyRelationship,
            NoSeduction

        };
        public static SeductionModeSetting SeductionMode = SeductionModeSetting.OnlyRelationship;
        private SettingHandle<SeductionModeSetting> settingSeductionMode;

        private static int baseRomanceChance;
        private SettingHandle<int> settingBaseRomanceChance;
        public static float BaseRomanceChance
        {
            get
            {
                return Mathf.Max(0f, baseRomanceChance) * 0.01f;
            }
        }
        private static int baseBreakupChance;
        private SettingHandle<int> settingBaseBreakupChance;
        public static float BaseBreakupChance
        {
            get
            {
                return Mathf.Max(0f, baseBreakupChance) * 0.01f;
            }
        }
        private static int baseFlirtChance;
        private SettingHandle<int> settingBaseFlirtChance;
        public static float BaseFlirtChance
        {
            get
            {
                return Mathf.Max(0f, baseFlirtChance) * 0.01f;
            }
        }

        private static int romanticSuccessRate;
        private SettingHandle<int> settingRomanticSuccessRate;
        public static float RomanticSuccessRate
        {
            get
            {
                return Mathf.Max(0f, romanticSuccessRate) * 0.01f;
            }
        }

        private static int baseSeductionChance;
        private SettingHandle<int> settingBaseSeductionChance;
        public static float BaseSeductionChance
        {
            get
            {
                return Mathf.Max(0f, baseSeductionChance) * 0.01f;
            }
        }

        private static int minAttractionForSeduction;
        private SettingHandle<int> settingMinAttractionForSeduction;
        public static float MinAttractionForSeduction
        {
            get
            {
                return Mathf.Max(0f, minAttractionForSeduction) * 0.01f;
            }
        }

        private static int sweetheartRate;
        private SettingHandle<int> settingSweetheartRate;
        public static float SweetheartRate
        {
            get
            {
                return Mathf.Max(0f, sweetheartRate) * 0.01f;
            }
        }

        private static int decayRate;
        private SettingHandle<int> settingDecayRate;
        public static float DecayRate
        {
            get
            {
                return Mathf.Clamp01(decayRate * 0.01f);
            }
        }

        public enum KinseyDescriptor
        {
            ExclusivelyHeterosexual,
            MostlyHeterosexual,
            LeansHeterosexual,
            Bisexual,
            LeansHomosexual,
            MostlyHomosexual,
            ExclusivelyHomosexual
        }

        public static KinseyDescriptor averageKinseyFemale;
        private SettingHandle<KinseyDescriptor> settingAverageKinseyFemale;
        public static KinseyDescriptor averageKinseyMale;
        private SettingHandle<KinseyDescriptor> settingAverageKinseyMale;

        public enum ExtraspeciesRomanceSetting
        {
            NoXenoRomance,
            OnlyXenophiles,
            OnlyXenophobesNever,
            CaptainKirk
        }

        public static ExtraspeciesRomanceSetting extraspeciesRomance;
        private SettingHandle<ExtraspeciesRomanceSetting> settingExtraspeciesRomance;

        public static bool detailedAttractionLogs;
        private SettingHandle<bool> settingDetailedAttractionLogs;

        public static bool rerollBeautyTraits;
        private SettingHandle<bool> settingRerollBeautyTraits;

        public static bool polygamousWorld;
        private SettingHandle<bool> settingPolygamousWorld;

        public static int numberOfRelationships;
        private SettingHandle<int> settingNumberOfRelationships;

        public static bool informalRomanceLetters;
        private SettingHandle<bool> settingInformalRomanceLetters;

        //public static bool useFacialAttractiveness;
        public static bool detailedDebugLogs;
        private SettingHandle<bool> settingDetailedDebugLogs;

        public override void SettingsChanged()
        {
            AttractionCalculation = settingAttractionCalculation.Value;
            genderMode = settingGenderMode.Value;
            averageKinseyFemale = settingAverageKinseyFemale.Value;
            extraspeciesRomance = settingExtraspeciesRomance.Value;
            averageKinseyMale = settingAverageKinseyMale.Value;
            baseRomanceChance = settingBaseRomanceChance.Value;
            baseBreakupChance = settingBaseBreakupChance.Value;
            baseFlirtChance = settingBaseFlirtChance.Value;
            romanticSuccessRate = settingRomanticSuccessRate.Value;
            decayRate = settingDecayRate.Value;
            numberOfRelationships = settingNumberOfRelationships.Value;
            polygamousWorld = settingPolygamousWorld.Value;
            rerollBeautyTraits = settingRerollBeautyTraits.Value;
            informalRomanceLetters = settingInformalRomanceLetters.Value;
            detailedDebugLogs = settingDetailedDebugLogs.Value;
            detailedAttractionLogs = settingDetailedAttractionLogs.Value;
        }

        public override void DefsLoaded()
        {

            settingAttractionCalculation = Settings.GetHandle("GRAttractionCalculationSetting", "AttractionCalculationSetting_title".Translate(), "AttractionCalculationSetting_desc".Translate(), AttractionCalculationSetting.Complex, null, "AttractionCalculationSetting_");
            settingGenderMode = Settings.GetHandle("GRGenderModeSetting", "GenderModeSetting_title".Translate(), "GenderModeSetting_desc".Translate(), GenderModeSetting.Vanilla, null, "GenderModeSetting_");
            settingAverageKinseyFemale = Settings.GetHandle("GRaverageKinseyFemale", "AverageKinseyFemale_title".Translate(), "AverageKinseyFemale_desc".Translate(), KinseyDescriptor.ExclusivelyHeterosexual, null, "KinseyDescriptor_");
            settingAverageKinseyMale = Settings.GetHandle("GRaverageKinseyMale", "AverageKinseyMale_title".Translate(), "AverageKinseyMale_desc".Translate(), KinseyDescriptor.ExclusivelyHeterosexual, null, "KinseyDescriptor_");
            settingSeductionMode = Settings.GetHandle("GRseductionModeSetting", "SeductionModeSetting_title".Translate(), "SeductionModeSetting_desc".Translate(), SeductionModeSetting.OnlyRelationship, null, "SeductionModeDescriptor_");
            settingExtraspeciesRomance = Settings.GetHandle("GRextraspeciesRomance","ExtraspeciesRomance_title".Translate(), "ExtraspeciesRomance_desc".Translate(), ExtraspeciesRomanceSetting.OnlyXenophobesNever, null, "ExtraspeciesRomanceDescriptor_");
            settingBaseRomanceChance = Settings.GetHandle<int>("GRbaseRomanceChance", "BaseRomanceChance_title".Translate(), "BaseRomanceChance_desc".Translate(), 100);
            settingBaseBreakupChance = Settings.GetHandle<int>("GRbaseBreakupChance", "BaseBreakupChance_title".Translate(), "BaseBreakupChance_desc".Translate(), 100);
            settingBaseFlirtChance = Settings.GetHandle<int>("GRbaseFlirtChance", "BaseFlirtChance_title".Translate(), "BaseFlirtChance_desc".Translate(), 100);
            settingBaseSeductionChance = Settings.GetHandle<int>("GRbaseSeductionChance", "BaseSeductionChance_title".Translate(), "BaseSeductionChance_desc".Translate(), 100);
            settingRomanticSuccessRate = Settings.GetHandle<int>("GRromanticSuccessRate", "RomanceSuccessRate_title".Translate(), "RomanceSuccessRate_desc".Translate(), 100);
            settingDecayRate = Settings.GetHandle<int>("GRdecayRate", "DecayRate_title".Translate(), "DecayRate_desc".Translate(), 25);
            settingMinAttractionForSeduction = Settings.GetHandle<int>("GRminAttractionForSeduction", "MinAttraction_title".Translate(), "MinAttraction_desc".Translate(), 90);
            settingNumberOfRelationships = Settings.GetHandle<int>("GRnumberOfRelationships", "NumberOfRelationships_title".Translate(), "NumberOfRelationships_desc".Translate(), 3);
            settingPolygamousWorld = Settings.GetHandle<bool>("GRpolygamousWorld", "PolygamousWorld_title".Translate(), "PolygamousWorld_desc".Translate(), false);
            settingRerollBeautyTraits = Settings.GetHandle<bool>("GRrerollBeautyTraits", "RerollBeautyTraits_title".Translate(), "RerollBeautyTraits_desc".Translate(),false);
            settingInformalRomanceLetters = Settings.GetHandle<bool>("GRinformalRomanceLetters", "InformalLetters_title".Translate(), "InformalLetters_desc".Translate(), true);
            //useFacialAttractiveness = Settings.GetHandle<bool>("GRuseFacialAttractiveness", "UseFacialAttractiveness_title".Translate(), "UseFacialAttractiveness_desc".Translate(), false);
            settingDetailedDebugLogs = Settings.GetHandle<bool>("GRdetailedDebugLogs", "DetailedDebugLog_title".Translate(), "DetailedDebugLog_desc".Translate(), false);
            settingDetailedAttractionLogs = Settings.GetHandle<bool>("GRdetailedAttractionLogs", "DetailedAttractionLog_title".Translate(), "DetailedAttractionLog_desc".Translate(), false);

            AttractionCalculation = settingAttractionCalculation.Value;
            genderMode = settingGenderMode.Value;
            averageKinseyFemale = settingAverageKinseyFemale.Value;
            extraspeciesRomance = settingExtraspeciesRomance.Value;
            averageKinseyMale = settingAverageKinseyMale.Value;
            baseRomanceChance = settingBaseRomanceChance.Value;
            baseBreakupChance = settingBaseBreakupChance.Value;
            baseFlirtChance = settingBaseFlirtChance.Value;
            baseSeductionChance = settingBaseSeductionChance.Value;
            romanticSuccessRate = settingRomanticSuccessRate.Value;
            decayRate = settingDecayRate.Value;
            numberOfRelationships = settingNumberOfRelationships.Value;
            polygamousWorld = settingPolygamousWorld.Value;
            rerollBeautyTraits = settingRerollBeautyTraits.Value;
            informalRomanceLetters = settingInformalRomanceLetters.Value;
            detailedDebugLogs = settingDetailedDebugLogs.Value;
            detailedAttractionLogs = settingDetailedAttractionLogs.Value;


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
                        maleMaturityCurves.Add(def, CreateCurveFrom(def.GetModExtension<XenoRomanceExtension>().maturityByAgeCurveMale));
                        femaleMaturityCurves.Add(def, CreateCurveFrom(def.GetModExtension<XenoRomanceExtension>().maturityByAgeCurveFemale));
                    }
                }
            }
            
            //Patches

        }
        
        public static XenoRomanceExtension CreateXenoRomanceExtensionFor(ThingDef pawnType)
        {
            
            XenoRomanceExtension xenoRomance = new XenoRomanceExtension() { };
            float maxAge = pawnType.race.lifeExpectancy;
            //xenoRomance.youngAdultAge = (maxAge * 0.15f);
            //xenoRomance.midlifeAge = (maxAge * 0.5f); 
            //xenoRomance.subspeciesOf = null;
            xenoRomance.extraspeciesAppeal = 0.5f;
            List<LifeStageAge> lifeStageDefs = pawnType.race.lifeStageAges;
            float reproductiveStart = 0f; 
            for (int i = 0; i < lifeStageDefs.Count(); i++)
            {
                if (lifeStageDefs[i].def.reproductive)
                {
                    reproductiveStart = lifeStageDefs[i].minAge;
                }
            }
            xenoRomance.maturityByAgeCurveFemale.Add(new Vector2(0f, 0.01f));
            xenoRomance.maturityByAgeCurveFemale.Add(new Vector2(reproductiveStart, 1f));
            xenoRomance.maturityByAgeCurveFemale.Add(new Vector2(Mathf.Lerp(reproductiveStart, maxAge, 0.5f), 2f));
            xenoRomance.maturityByAgeCurveFemale.Add(new Vector2(maxAge, 3f));
            xenoRomance.maturityByAgeCurveMale.Add(new Vector2(0f, 0.01f));
            xenoRomance.maturityByAgeCurveMale.Add(new Vector2(reproductiveStart, 1f));
            xenoRomance.maturityByAgeCurveMale.Add(new Vector2(Mathf.Lerp(reproductiveStart, maxAge, 0.5f), 2f));
            xenoRomance.maturityByAgeCurveMale.Add(new Vector2(maxAge, 3f));
            xenoRomance.sexDriveByAgeCurveFemale.Add(new Vector2((reproductiveStart * 0.7f), 0f));
            xenoRomance.sexDriveByAgeCurveFemale.Add(new Vector2((reproductiveStart), 3f));
            xenoRomance.sexDriveByAgeCurveFemale.Add(new Vector2(Mathf.Lerp(reproductiveStart, maxAge, 0.5f), 1f));
            xenoRomance.sexDriveByAgeCurveFemale.Add(new Vector2(maxAge, 0.1f));
            xenoRomance.sexDriveByAgeCurveMale.Add(new Vector2((reproductiveStart * 0.7f), 0f));
            xenoRomance.sexDriveByAgeCurveMale.Add(new Vector2((reproductiveStart), 3f));
            xenoRomance.sexDriveByAgeCurveMale.Add(new Vector2(Mathf.Lerp(reproductiveStart, maxAge, 0.5f), 1f));
            xenoRomance.sexDriveByAgeCurveMale.Add(new Vector2(maxAge, 0.1f));
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

        public static SimpleCurve GetMaturityCurveFor(Pawn pawn)
        {
            ThingDef def = pawn.def;
            Gender gender = pawn.gender;
            if (gender == Gender.Female)
            {
                if (femaleMaturityCurves.Keys.Contains(def))
                {
                    return femaleMaturityCurves[def];
                }
            }
            if (gender == Gender.Male)
            {
                if (maleMaturityCurves.Keys.Contains(def))
                {
                    return maleMaturityCurves[def];
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
        //ERROR MESSAGES
        public void Error_TriedDecayNullRelationship(Pawn pawn, Pawn other, PawnRelationDef pawnRelationDef)
        {
            this.Logger.Error("Tried to decay a null relationship {2} between {0} and {1}".Formatted(pawn.Name.ToStringShort, other.Name.ToStringShort, pawnRelationDef.defName));
        }


        //CHECK MODS
        /*
        public static bool UsingDubsBadHygiene()
        {
            try
            {
                if (LoadedModManager.RunningModsListForReading.Any(x=> x.Name == "Dubs Bad Hygiene"))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (TypeLoadException ex) { return false; }
        }
        */

        private static Dictionary<ThingDef, SimpleCurve> maleSexDriveCurves = new Dictionary<ThingDef, SimpleCurve> { };

        private static Dictionary<ThingDef, SimpleCurve> femaleSexDriveCurves = new Dictionary<ThingDef, SimpleCurve> { };

        private static Dictionary<ThingDef, SimpleCurve> maleMaturityCurves = new Dictionary<ThingDef, SimpleCurve> { };

        private static Dictionary<ThingDef, SimpleCurve> femaleMaturityCurves = new Dictionary<ThingDef, SimpleCurve> { };


    }
}

