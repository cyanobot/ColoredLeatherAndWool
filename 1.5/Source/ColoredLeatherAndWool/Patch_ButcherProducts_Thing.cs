using System.Collections.Generic;
using HarmonyLib;
using Verse;
using UnityEngine;
using static ColoredLeatherAndWool.Main;

namespace ColoredLeatherAndWool
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.ButcherProducts))]
    class Patch_ButcherProducts_Thing
    {
        static IEnumerable<Thing> Postfix(IEnumerable<Thing> results, Thing __instance)
        {
            //avoid duplication
            if (__instance is Pawn || __instance is Corpse)
            {
                foreach (Thing thing in results) yield return thing;
                yield break;
            }

            LogUtil.DebugLog("Patch_ButcherProducts_Thing, __instance: " + __instance + ", stackCount: " + __instance.stackCount);

            Color color = __instance.TryGetComp<CompColorable>()?.Color ?? __instance.DrawColor;
            LogUtil.DebugLog("color: " + color);

            foreach (Thing thing in results)
            {
                LogUtil.DebugLog("result: " + thing + ", CompColorable: " + thing.TryGetComp<CompColorable>()
                    + ", modExtensions: " + thing.def.modExtensions.ToStringSafeEnumerable());
                if (thing.def.HasModExtension<DefModExtension_ReceivesButcherColor>())
                {
                    LogUtil.DebugLog("ReceivesButcherColor");
                    thing.SetColor(color, logSetColorFailures);
                }
                yield return thing;
            }
        }
    }


}