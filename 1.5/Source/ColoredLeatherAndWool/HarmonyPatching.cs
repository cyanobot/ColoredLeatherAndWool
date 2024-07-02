using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;
using System.Reflection;
using static ColoredLeatherAndWool.Main;
using System.Reflection.Emit;

namespace ColoredLeatherAndWool
{
    /*
    [HarmonyPatch(typeof(ThingMaker),nameof(ThingMaker.MakeThing))]
    class TestPatch_MakeThing
    {
        static void Prefix(ThingDef def)
        {
            Log.Message("Called MakeThing(" + def + ")");
        }
    }
    */

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.ButcherProducts))]
    class Patch_ButcherProducts_Pawn
    {
        static IEnumerable<Thing> Postfix(IEnumerable<Thing> results, Pawn __instance)
        {
            //Log.Message("Patch_ButcherProducts_Pawn Postfix, leatherDef: " + leatherDef + ", results: " + results.ToStringSafeEnumerable());
            Color color = ColorUtility.GetPawnColor(__instance);

            //Log.Message("about to start foreach, color: " + color);
                
            foreach (Thing thing in results)
            {
                //Log.Message("thing: " + thing + ", HasModExtension: " + thing.def.HasModExtension<DefModExtension_ReceivesButcherColor>());
                if (color != Color.white && thing.def.HasModExtension<DefModExtension_ReceivesButcherColor>())
                {
                    if (Settings.tannedColors)
                    {
                        //Log.Message("thing.comps: " + ((ThingWithComps)thing).AllComps.ToStringSafeEnumerable());
                        CompTanningColor compTanningColor = thing.TryGetComp<CompTanningColor>();
                        if (compTanningColor != null)
                        {
                            //Log.Message("Found compTanningColor. animalColor: " + color
                            //    + ", tanningColor: " + compTanningColor.BaseColor
                            //    + ", weight: " + compTanningColor.Weight
                            //    );

                            color = ColorUtility.WeightedColorBlend(color, compTanningColor.BaseColor, compTanningColor.Weight);
                        }
                    }
                    //Log.Message("color pre SetColor: " + thing.DrawColor);
                    thing.SetColor(color, true);
                    //Log.Message("color post SetColor: " + thing.DrawColor);
                }
                yield return thing;
            }
        }
        
    }

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

            //Log.Message("Patch_ButcherProducts_Thing, __instance: " + __instance + ", stackCount: " + __instance.stackCount);

            Color color = __instance.TryGetComp<CompColorable>()?.Color ?? __instance.DrawColor;
            //Log.Message("color: " + color);

            foreach (Thing thing in results)
            {
                //Log.Message("result: " + thing + ", CompColorable: " + thing.TryGetComp<CompColorable>()
                //    + ", modExtensions: " + thing.def.modExtensions.ToStringSafeEnumerable());
                if (thing.def.HasModExtension<DefModExtension_ReceivesButcherColor>())
                {
                    //Log.Message("ReceivesButcherColor");
                    thing.SetColor(color, logSetColorFailures);
                }
                yield return thing;
            }
        }
    }

    [HarmonyPatch(typeof(CompHasGatherableBodyResource), nameof(CompHasGatherableBodyResource.Gathered))]
    static public class Patch_GatherableBodyResource
    {
        static FieldInfo f_stackCount = AccessTools.Field(typeof(Thing), nameof(Thing.stackCount));
        static MethodInfo m_TryGiveColor = AccessTools.Method(typeof(Patch_GatherableBodyResource), nameof(Patch_GatherableBodyResource.TryGiveColor));

        public static void TryGiveColor(Thing product, CompHasGatherableBodyResource resourceComp)
        {
            if (product.TryGetComp<CompColorable>() == null) return;
            if (!product.def.HasModExtension<DefModExtension_ReceivesAnimalColor>()) return;

            ThingWithComps pawn = resourceComp.parent;
            if (!(pawn is Pawn)) return;

            Color color = ColorUtility.GetPawnColor((Pawn)pawn, true);

            if (color != Color.white) product.SetColor(color);
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;

                if (instruction.opcode == OpCodes.Stfld && instruction.OperandIs(f_stackCount))
                {
                    yield return new CodeInstruction(OpCodes.Dup);                  //duplicate the reference to thing that's already on the stack
                    yield return new CodeInstruction(OpCodes.Ldarg_0);              //arg0 ought to be the comp instance
                    yield return new CodeInstruction(OpCodes.Call, m_TryGiveColor); //call custom method above
                }
            }
        }
    }

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