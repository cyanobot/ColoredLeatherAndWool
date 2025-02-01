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
using static ColoredLeatherAndWool.Settings;
using static ColoredLeatherAndWool.Main;

namespace ColoredLeatherAndWool
{
    class ColorUtility
    {
        public static Color GetPawnColor(Pawn pawn, bool useHairColor = false)
        {
            LogUtil.DebugLog("GetPawnColor - pawn: " + pawn + ", comps: " + pawn.AllComps.ToStringSafeEnumerable());
            CompLeatherColor comp = pawn.GetComp<CompLeatherColor>();
            if (comp != null)
            {
                LogUtil.DebugLog("HasComp(CompLeatherColor)");
                return comp.GetColor();
            }

            Color color;

            if (pawn.RaceProps.Humanlike)
            {
                if (useHairColor) color = pawn.story?.HairColor ?? Color.white;
                else color = pawn.story?.SkinColor ?? Color.white;
                if (color != Color.white)
                {
                    LogUtil.DebugLog("Returning SkinColor");
                    return color;
                }
            }
            
            return pawn.Drawer.renderer.BodyGraphic.Color;
            LogUtil.DebugLog("nakedGraphic.Color: " + color);
        }

        public static bool UseTannedColor()
        {
            return tannedColors && !anyTanningLoaded;
        }

        public static bool UseTannedColor(Thing thing, out CompTanningColor compTanningColor)
        {
            compTanningColor = null;
            if (!UseTannedColor()) return false;

            compTanningColor = thing.TryGetComp<CompTanningColor>();
            if (compTanningColor == null) return false;

            else return true;
        }

        public static Color WeightedColorBlend(Color color1, Color color2, float weight)
        {
            //Log.Message("WeightedColorBlend color1: " + color1 + ", color2: " + color2 + ", weight: " + weight);
            weight = Mathf.Clamp(weight, 0f, 1f);
            float red = (color1.r * (1 - weight)) + (color2.r * weight);
            //Log.Message("color1.r: " + color1.r + ", 1-weight: " + (1 - weight)
            //    + ", color2.r: " + color2.r + ", weight: " + weight
            //    + ", red: " + red);
            float green = (color1.g * (1 - weight)) + (color2.g * weight);
            float blue = (color1.b * (1 - weight)) + (color2.b * weight);
            float alpha = (color1.a * (1 - weight)) + (color2.a * weight);

            return new Color(red, green, blue, alpha);
        }

        public static Color Multiply(Color color1, Color color2)
        {
            float red = color1.r * color2.r;
            red = Mathf.Clamp01(red);
            float green = color1.g * color2.g;
            green = Mathf.Clamp01(green);
            float blue = color1.b * color2.b;
            blue = Mathf.Clamp01(blue);
            float alpha = color1.a * color2.a;
            alpha = Mathf.Clamp01(alpha);

            return new Color(red, green, blue, alpha);
        }

        public static Color ButcherColorFor(Thing thing, Pawn pawn)
        {
            Color pawnColor = GetPawnColor(pawn);
            if (UseTannedColor(thing, out CompTanningColor compTanningColor))
            {
                return ColorUtility.WeightedColorBlend(pawnColor, compTanningColor.BaseColor, compTanningColor.Weight);
            }
            else
            {
                return pawnColor;
            }
        }
    }
}
