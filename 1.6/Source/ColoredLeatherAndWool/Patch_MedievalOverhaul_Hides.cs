using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using UnityEngine;
using System.Reflection;
using static ColoredLeatherAndWool.Main;

namespace ColoredLeatherAndWool
{
    [HarmonyPatch]
    static public class Patch_MedievalOverhaul_Hides
    {
        static public bool Prepare()
        {
            return medievalOverhaulLoaded;
        }

        static public MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.TypeByName("MedievalOverhaul.HideUtility"), "BasicHideDef");
        }

        static ThingDef Postfix(ThingDef __result)
        {
            if (__result.comps == null) return __result;
            
            if (!__result.comps.Any(c => c.compClass == typeof(CompColorable)))
            {
                __result.comps.Add(new CompProperties(typeof(CompColorable)));
            }
            __result.comps.Add(new CompProperties(typeof(CompColorNoStack)));
            

            if (__result.modExtensions == null)
            {
                __result.modExtensions = new List<DefModExtension>();
            }
            __result.modExtensions.Add(new DefModExtension_ReceivesButcherColor());

            return __result;
        }
    }
}
