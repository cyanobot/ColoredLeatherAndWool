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
    class ColorUtility
    {
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

        public static Color GetPawnColor(Pawn pawn, bool useHairColor = false)
        {
            //Log.Message("Patch_ButcherProducts_Pawn Postfix, leatherDef: " + leatherDef + ", results: " + results.ToStringSafeEnumerable());

            if (pawn.GetComp<CompLeatherColor>() != null)
            {
                //Log.Message("HasComp(CompLeatherColor)");
                CompLeatherColor comp = pawn.GetComp<CompLeatherColor>();
                return comp.GetColor();
            }

            Color color = Color.white;

            if (pawn.RaceProps.Humanlike)
            {
                if (useHairColor) color = pawn.story?.HairColor ?? Color.white;
                else color = pawn.story?.SkinColor ?? Color.white;
                if (color != Color.white)
                {
                    //Log.Message("Returning SkinColor");
                    return color;
                }
            }

            return pawn.Drawer.renderer.graphics.nakedGraphic.Color;
            //Log.Message("nakedGraphic.Color: " + color);
            
        }
    }
}
