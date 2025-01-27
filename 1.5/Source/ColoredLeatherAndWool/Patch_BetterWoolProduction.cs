using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using HarmonyLib;
using Verse;
using UnityEngine;
using System.Reflection;
using static ColoredLeatherAndWool.Main;
using RimWorld;

namespace ColoredLeatherAndWool
{
    [HarmonyPatch(typeof(DefGenerator),nameof(DefGenerator.GenerateImpliedDefs_PreResolve))]
    static public class Patch_BetterWoolProduction_TriggerPatch
    {
        static public bool Prepare()
        {
            if (!betterWoolLoaded) return false;
            return true;
        }

        static public void Prefix()
        {
            LogUtil.DebugLog("Patch_BetterWoolProduction_TriggerPatch.Prefix");
            var postfix = typeof(Patch_BetterWoolProduction).GetMethod(nameof(Patch_BetterWoolProduction.Postfix));
            Main.harmony.Patch(
                Patch_BetterWoolProduction.TargetMethod(),
                postfix: new HarmonyMethod(postfix)
                );
        }
    }

    //[HarmonyPatch]
    static public class Patch_BetterWoolProduction
    {
        static public bool Prepare()
        {
            if (!betterWoolLoaded) return false;
            return true;
        }

        static public MethodBase TargetMethod()
        {
            MethodBase targetMethod = AccessTools.Method(AccessTools.TypeByName("AutoWool.GeneratorUtility"), "BasicFleeceDef");
            LogUtil.DebugLog("TargetMethod: " + targetMethod);
            return targetMethod;
        }

        public static void Postfix(ref ThingDef __result)
        {
            LogUtil.DebugLog("__result: " + __result);
            if (__result == null) return;

            if (__result.comps == null) __result.comps = new List<CompProperties>();
            if (!__result.HasComp<CompColorable>()) __result.comps.Add(new CompProperties() { compClass = typeof(CompColorable) });
            __result.comps.Add(new CompProperties() { compClass = typeof(CompColorNoStack) });

            if (__result.modExtensions == null) __result.modExtensions = new List<DefModExtension>();
            __result.modExtensions.Add(new DefModExtension_ReceivesAnimalColor());
            __result.modExtensions.Add(new DefModExtension_ReceivesButcherColor());
        }
    }


}