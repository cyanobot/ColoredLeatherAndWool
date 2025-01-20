using System;
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
    static public class Patch_Mammalia
    {
        static public bool Prepare()
        {
            if (!BTELoaded) return false;
            return true;
        }

        static public MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.TypeByName("BTE.Gene_RapidCoatGrowth"), "CreateProduce");
        }

        static Thing Postfix(Thing __result, object __instance)
        {
            if (!(__instance is Gene instanceGene)) return __result;

            if (__result.TryGetComp<CompColorable>() == null) return __result;
            if (!__result.def.HasModExtension<DefModExtension_ReceivesAnimalColor>()) return __result;

            Pawn pawn = instanceGene.pawn;
            if (pawn == null) return __result;

            Color color = ColorUtility.GetPawnColor((Pawn)pawn, true);

            if (color != Color.white) __result.SetColor(color);

            return __result;
        }
    }


}