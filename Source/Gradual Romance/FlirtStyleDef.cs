using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Psychology;


namespace Gradual_Romance
{
    public class FlirtStyleDef : Def
    {
        /// <summary>
        /// Which rule pack the flirt style uses when flirting with a man.
        /// </summary>
        public RulePackDef rulePackMale;
        /// <summary>
        /// Which rule pack the flirt style uses when flirting with a woman.
        /// </summary>
        public RulePackDef rulePackFemale;
        /// <summary>
        /// Global modifier on base chance
        /// </summary>
        public float baseChance;
        public float acquaitanceFactor;
        public float loveBuddyFactor;
        public float sweetheartFactor;
        public float lovefriendFactor;
        public float loverFactor;
        public float spouseFactor;
        /// <summary>
        /// Pawns high in these personality nodes are more likely to use this flirt style
        /// </summary>
        public List<PersonalityNodeModifier> moreLikelyPersonalities = new List<PersonalityNodeModifier>();
        /// <summary>
        /// Pawns low in these personality nodes are less likely to use this flirt style
        /// </summary>
        public List<PersonalityNodeModifier> lessLikelyPersonalities = new List<PersonalityNodeModifier>();
        /// <summary>
        /// Pawns with this trait are 1.5 times more likely to use this flirt style.
        /// </summary>
        public List<TraitModifier> traitModifiers = new List<TraitModifier>();
        /// <summary>
        /// Pawns with this trait will never use this flirt style.
        /// </summary>
        public List<TraitDef> cancelingTraits = new List<TraitDef>();
        /// <summary>
        /// Socially incompetent pawns are much more likely to use this flirt style.
        /// </summary>
        public bool incompetent;
        /// <summary>
        /// The more risky a flirt is, the more likely the recipient pawn will get angry if they reject it. In extreme circumstances it might cause a social fight.
        /// </summary>
        public float baseRiskiness;
        /// <summary>
        /// The more awkward a flirt is, the more likely a pawn will embarrass themselves and/or the recipient. It can also be endearing.
        /// </summary>
        public float baseAwkwardness;
        /// <summary>
        /// The more obscure a flirt style is, the more likely it is not to be noticed.
        /// </summary>
        public float baseObscurity;
        /// <summary>
        /// Sexy flirts are more likely to be accepted if the pawn finds the other pawn physically hot.
        /// </summary>
        public float baseSexiness;
        /// <summary>
        /// Romantic flirts are more likely to be accepted if the pawn likes the other pawn.
        /// </summary>
        public float baseRomance;
        /// <summary>
        /// Logical flirts are more likely to be accepted if that pawn finds the other pawn socially attractive
        /// </summary>
        public float baseLogic;
        /// <summary>
        /// Base chance two pawns become sweethearts after using this flirt, as opposed to lovebuddies.
        /// </summary>
        public float baseSweetheartChance;

        public static FlirtStyleDef Named(string str)
        {
            return DefDatabase<FlirtStyleDef>.GetNamed(str, true);
        }

        public override int GetHashCode()
        {
            return this.defName.GetHashCode();
        }
    }
}
