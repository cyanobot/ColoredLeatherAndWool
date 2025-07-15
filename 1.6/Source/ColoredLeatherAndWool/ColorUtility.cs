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
            LogUtil.DebugLog($"GetPawnColor - pawn: {pawn}" +
                $", BodyGraphic: {GetPawnGraphic(pawn)}" +
                $", comps: " + pawn.AllComps.ToStringSafeEnumerable());


            Color color;

            //special case: humanlike - use skin color (or hair color)
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

            //special case: Alpha Animals' chameleon yak
            if (Main.alphaAnimalsLoaded && pawn.def.defName == "AA_ChameleonYak")
            {
                //Log.Message("found chameleonyak");
                HediffSet hediffs = pawn.health?.hediffSet;
                if (hediffs == null) 
                {
                    Log.Error($"[ColoredLeatherAndWool] Pawn ({pawn}) identified as Chameleon Yak but has no HediffSet, returning default yak color");
                }
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
                //default: temperate pelt
                return new Color(0.257f, 0.184f, 0.16f);
            }

            //if animal has been tagged with CompLeatherColor
            //use that to get the color
            CompLeatherColor comp = pawn.GetComp<CompLeatherColor>();
            if (comp != null)
            {
                LogUtil.DebugLog("HasComp(CompLeatherColor)");
                return comp.GetFinalColor();
            }

            //otherwise try and get the color directly from the pawn graphic
            return ColorFromPawnGraphic(pawn);
        }

        public static Color ColorFromPawnGraphic(Pawn pawn)
        {
            Graphic bodyGraphic = GetPawnGraphic(pawn);
            if (bodyGraphic == null) return Color.white;
            return bodyGraphic.Color;
        }
        
        public static Graphic GetPawnGraphic(Pawn pawn)
        {
            Graphic bodyGraphic = pawn.Drawer.renderer.BodyGraphic;
            LogUtil.DebugLog($"BodyGraphic (1st attempt): {bodyGraphic}");
            //if the body graphic is null, try asking the render tree to recalculate and try again
            if (bodyGraphic == null)
            {
                pawn.Drawer.renderer.renderTree.SetDirty();
                pawn.Drawer.renderer.EnsureGraphicsInitialized();
                bodyGraphic = pawn.Drawer.renderer.BodyGraphic;

                LogUtil.DebugLog($"BodyGraphic (2nd attempt): {bodyGraphic}");
            }
            //if it's still null, something more serious is wrong
            if (bodyGraphic == null)
            {
                Log.Error($"[Colored Leather and Wool] Failed to get BodyGraphic for pawn ({pawn}), returning default color: white");
                return null;
            }
            return bodyGraphic;
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
            LogUtil.DebugLog($"ButcherColorFor - thing: {thing}, pawn: {pawn}, comps: " + pawn.AllComps.ToStringSafeEnumerable());
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
