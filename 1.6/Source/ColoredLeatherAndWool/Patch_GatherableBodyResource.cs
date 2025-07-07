using System.Collections.Generic;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;
using System.Reflection;
using System.Reflection.Emit;

namespace ColoredLeatherAndWool
{
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


}