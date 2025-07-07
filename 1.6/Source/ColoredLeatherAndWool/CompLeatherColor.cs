using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;


namespace ColoredLeatherAndWool
{
    class CompLeatherColor : ThingComp
    {
        public Pawn Pawn => (Pawn)parent;
        public CompProperties_LeatherColor Props => (CompProperties_LeatherColor)this.props;
        public Color ColorBase => Props.color;
        public Color ColorMale => Props.colorMale;
        //public VariantType VariantType => Props.variantType;
        public List<Color> AlternateGraphicColors => Props.alternateGraphicColors;
        public Dictionary<PawnKindDef, Color> ColorByPawnKind => Props.colorByPawnKind;

        public Color GetColor()
        {
            //Log.Message("GetColor, VariantType: " + VariantType);

            if (Main.alphaAnimalsLoaded && parent.def.defName == "AA_ChameleonYak")
            {
                //Log.Message("found chameleonyak");
                HediffSet hediffs = ((Pawn)parent).health?.hediffSet;
                if (hediffs == null) ;
                else if (hediffs.HasHediff(DefDatabase<HediffDef>.GetNamed("AA_WinterPelt")))
                {
                    return new Color(0.984f, 0.988f, 0.992f);
                }
                else if (hediffs.HasHediff(DefDatabase<HediffDef>.GetNamed("AA_JunglePelt")))
                {
                    return new Color(0.25f, 0.281f, 0.152f);
                }
                else if (hediffs.HasHediff(DefDatabase<HediffDef>.GetNamed("AA_DesertPelt")))
                {
                    return new Color(0.719f, 0.656f, 0.429f);
                }
                else return new Color(0.257f, 0.184f, 0.16f);
            }

            if (ColorMale != Color.white && Pawn.gender == Gender.Male) return ColorMale;

            Graphic bodyGraphic = Pawn.Drawer.renderer.BodyGraphic;

            Color xmlColor = Color.white;
            if (!ColorByPawnKind.NullOrEmpty())
            {
                PawnKindDef pawnKindDef = Pawn.kindDef;
                if (ColorByPawnKind.ContainsKey(pawnKindDef)) xmlColor = ColorByPawnKind[pawnKindDef];
            }
            if (xmlColor == Color.white)
            { 
                int index = -1;
                if (!AlternateGraphicColors.NullOrEmpty())
                {
                    index = Props.IndexFromTexPath(bodyGraphic.path);
                }
                xmlColor = Props.ColorFromIndex(index);
            }

            Color graphicColor = bodyGraphic.Color;
            //Log.Message("xmlColor: " + xmlColor
            //    + ", graphicColor: " + graphicColor
            //    + ", multiplied: " + ColorUtility.Multiply(xmlColor, graphicColor));

            return ColorUtility.Multiply(xmlColor, graphicColor);
        }
    }

    class CompProperties_LeatherColor : CompProperties
    {
        private static FieldInfo texPath = AccessTools.Field(typeof(AlternateGraphic), "texPath");

        public Color color = Color.white;
        public Color colorMale = Color.white;
        //public VariantType variantType = VariantType.None;
        public List<Color> alternateGraphicColors = new List<Color>();
        public List<int> indicesUsingDrawColor;
        public Dictionary<PawnKindDef, Color> colorByPawnKind;

        public ThingDef pawnDef;
        public PawnKindDef PawnKindDef => pawnDef.race.AnyPawnKind;
        public List<AlternateGraphic> AlternateGraphics => PawnKindDef.alternateGraphics;

        public CompProperties_LeatherColor()
        {
            this.compClass = typeof(CompLeatherColor);
        }

        public CompProperties_LeatherColor(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }

        public override void PostLoadSpecial(ThingDef parent)
        {
            pawnDef = parent;
        }

        public int IndexFromTexPath(string path)
        {
            int index = AlternateGraphics.FindIndex(ag => (string)texPath.GetValue(ag) == path);
            //Log.Message("IndexFromTexPath, path: " + path + ", index: " + index);
            //Log.Message(AlternateGraphics.Select(ag => texPath.GetValue(ag)).ToStringSafeEnumerable());

            if (index < 0 || index >= alternateGraphicColors.Count)
            {
                return -1;
            }
            else
            {
                return index;
            }
        }

        public Color ColorFromIndex(int index)
        {
            if (index < 0 || index >= alternateGraphicColors.Count)
            {
                return color;
            }
            else return alternateGraphicColors[index];
        }
    }
}
