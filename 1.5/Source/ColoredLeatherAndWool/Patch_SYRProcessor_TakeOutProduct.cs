using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using System.Reflection;
using static ColoredLeatherAndWool.Main;
using System.ComponentModel.Design;

namespace ColoredLeatherAndWool
{
    [HarmonyPatch]
    [HarmonyBefore(new string[] { "medievalOverhaul" })]
    static public class Patch_SYRProcessor_TakeOutProduct
    {
        static public List<string> tanningProcessorDefNames = new List<string>
        {
            "CYB_TanningVat",
            "DankPyon_TanningRack"
        };
        static public FieldInfo f_ingredientThings;

        static public bool Prepare(MethodBase original)
        {
            LogUtil.DebugLog("Patch_SYRProcessor_TakeOutProduct.Prepare - anyTanningLoaded: " + anyTanningLoaded);
            if (anyTanningLoaded)
            {
                if (original == null)
                {
                    f_ingredientThings = AccessTools.Field(AccessTools.TypeByName("ProcessorFramework.ActiveProcess"), "ingredientThings");
                    LogUtil.DebugLog("f_ingredientThings: " + f_ingredientThings);
                    if (f_ingredientThings == null) return false;
                }
                return true;
            }
            else return false;
        }

        static public MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.TypeByName("ProcessorFramework.CompProcessor"), "TakeOutProduct");
        }

        static void Prefix(ThingWithComps ___parent, object activeProcess, out Color __state)
        {
            LogUtil.DebugLog("Patch_SYRProcessor_TakeOutProduct.Prefix");
            __state = Color.white;
            List<Thing> ingredientThings = (List<Thing>)f_ingredientThings.GetValue(activeProcess);
            if (ingredientThings.NullOrEmpty()) return;

            if (!tanningProcessorDefNames.Contains(___parent.def.defName)) return;

            Thing mainIngredient = null;
            foreach (Thing thing in ingredientThings)
            {
                if (thing.def.HasModExtension<DefModExtension_ReceivesButcherColor>())
                {
                    mainIngredient = thing;
                    break;
                }
            }
            if (mainIngredient == null) return;

            __state = mainIngredient.DrawColor;

            LogUtil.DebugLog("Patch_TakeOutProduct.Prefix: mainIngredient: " + mainIngredient + ", __state: " + __state);
        }

        static Thing Postfix(Thing product, ThingWithComps ___parent, Color __state)
        {
            LogUtil.DebugLog("Patch_SYRProcessor_TakeOutProduct.Postfix");
            if (!tanningProcessorDefNames.Contains(___parent.def.defName)) return product;
            if (__state == Color.white) return product;

            if (!(product is ThingWithComps thingWithComps)) return product;
            if (thingWithComps.GetComp<CompColorable>() == null) return product;
            if (!product.def.HasModExtension<DefModExtension_ReceivesButcherColor>()) return product;

            CompTanningColor compTanningColor = thingWithComps.GetComp<CompTanningColor>();

            Color productColor;
            if (compTanningColor != null)
            {
                Log.Message("found comp, baseColor: " + compTanningColor.BaseColor 
                    + ", weight: " + compTanningColor.Weight);
                productColor = ColorUtility.WeightedColorBlend(__state, compTanningColor.BaseColor, compTanningColor.Weight);
            }
            else
            {
                productColor = __state;
            }
            Log.Message("productColor: " + productColor);
            product.SetColor(productColor);

            return product;

        }
    }
}
