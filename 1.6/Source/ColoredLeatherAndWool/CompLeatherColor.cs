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
using static ColoredLeatherAndWool.ColorUtility;
using System.IO;

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

        public Color GetFinalColor()
        {
            Color xmlColor = GetColorFromXML();

            if (!Props.ignoreGraphicColor)
            {
                Graphic pawnGraphic = GetPawnGraphic(Pawn);
                if (pawnGraphic != null)
                {
                    return Multiply(xmlColor, pawnGraphic.Color);
                }
            }

            return xmlColor;
        }
    
        public Color GetColorFromXML()
        {
            //if ColorMale is specified and the pawn is male
            //use the specified color
            if (ColorMale != Color.white && Pawn.gender == Gender.Male) return ColorMale;

            //if ColorByPawnKind is specified
            //try to get the color for the relevant pawnKind
            if (!ColorByPawnKind.NullOrEmpty())
            {
                PawnKindDef pawnKindDef = Pawn.kindDef;
                if (ColorByPawnKind.ContainsKey(pawnKindDef)) return ColorByPawnKind[pawnKindDef];
            }

            //if AlternateGraphicColors is specified
            //try to figure out which graphic we're using and look up the color for it
            if (!AlternateGraphicColors.NullOrEmpty())
            {
                return GetAlternateGraphicColor();
            }

            return ColorBase;
        }

        public Color GetAlternateGraphicColor()
        {

            Graphic pawnGraphic = GetPawnGraphic(Pawn);
            LogUtil.DebugLog($"GetAlternateGraphicColor: pawn: {Pawn}, graphic: {pawnGraphic}" +
                $", texPath: {pawnGraphic.path}, kindDef: {Pawn.kindDef}" +
                $", alternateGraphics: ({Pawn.kindDef?.alternateGraphics?.Count})"
                );
            if (pawnGraphic == null) return ColorBase;

            string texPath = pawnGraphic.path;
            if (texPath.NullOrEmpty()) return ColorBase;

            List<AlternateGraphic> altGraphics = Pawn.kindDef.alternateGraphics;
            if (altGraphics.NullOrEmpty()) return ColorBase;

            int index = altGraphics.FindIndex(ag => ag.graphicData?.texPath == texPath);
            LogUtil.DebugLog($"index from texPath: {index}" +
                $", index from pawn: {PawnGraphicUtils.GetGraphicIndex(Pawn)}"
                );
            if (index < 0 || index >= AlternateGraphicColors.Count) 
                    return ColorBase;

            return AlternateGraphicColors[index];
        }
    }

    class CompProperties_LeatherColor : CompProperties
    {
        private static FieldInfo texPath = AccessTools.Field(typeof(AlternateGraphic), "texPath");

        public Color color = Color.white;
        public Color colorMale = Color.white;
        public List<Color> alternateGraphicColors = new List<Color>();
        public List<int> indicesUsingDrawColor;
        public Dictionary<PawnKindDef, Color> colorByPawnKind;
        public bool ignoreGraphicColor;

        public CompProperties_LeatherColor()
        {
            this.compClass = typeof(CompLeatherColor);
        }

        public CompProperties_LeatherColor(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }

    }
}
