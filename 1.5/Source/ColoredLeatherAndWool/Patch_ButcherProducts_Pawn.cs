using System.Collections.Generic;
using HarmonyLib;
using Verse;
using UnityEngine;
using static ColoredLeatherAndWool.ColorUtility;

namespace ColoredLeatherAndWool
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.ButcherProducts))]
    class Patch_ButcherProducts_Pawn
    {
        static IEnumerable<Thing> Postfix(IEnumerable<Thing> results, Pawn __instance)
        {
            LogUtil.DebugLog("Patch_ButcherProducts_Pawn Postfix, leatherDef: " + __instance.RaceProps.leatherDef + ", results: " + results.ToStringSafeEnumerable());
            //Color pawnColor = ColorUtility.GetPawnColor(__instance);

            //LogUtil.DebugLog("about to start foreach, pawnColor: " + pawnColor);
                
            foreach (Thing thing in results)
            {
                LogUtil.DebugLog("thing: " + thing + ", HasModExtension: " + thing.def.HasModExtension<DefModExtension_ReceivesButcherColor>());
                if (thing.def.HasModExtension<DefModExtension_ReceivesButcherColor>())
                {
                    Color color = ButcherColorFor(thing, __instance);

                    LogUtil.DebugLog("color pre SetColor: " + thing.DrawColor);
                    thing.SetColor(color, true);
                    LogUtil.DebugLog("color post SetColor: " + thing.DrawColor);
                }                    
                yield return thing;
            }
        }
        
    }


}