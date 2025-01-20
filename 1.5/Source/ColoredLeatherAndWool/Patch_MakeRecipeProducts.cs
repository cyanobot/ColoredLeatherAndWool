using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColoredLeatherAndWool
{
    [HarmonyPatch]
    public static class Patch_MakeRecipeProducts
    {
        /*
        public static bool Prepare()
        {
            Type t_GenRecipe = typeof(GenRecipe);
            Type t_MakeRecipeProducts = t_GenRecipe.GetNestedTypes(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(t => t.Name.Contains("MakeRecipeProducts"))
                .Single();
            LogUtil.DebugLog("t_MakeRecipeProducts: " + t_MakeRecipeProducts);
=
      
            foreach (MethodInfo method in t_MakeRecipeProducts.GetRuntimeMethods()) //(
                //BindingFlags.Static | BindingFlags.NonPublic))
            {
                LogUtil.DebugLog("t_MakeRecipeProducts.method: " + method
                    + ", found: " + method.Name.Contains("MoveNext"));
            }
            
            return false;
        }
        */

        public static MethodBase TargetMethod()
        {
            Type t_GenRecipe = typeof(GenRecipe);
            Type t_MakeRecipeProducts = t_GenRecipe.GetNestedTypes(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(t => t.Name.Contains("MakeRecipeProducts"))
                .Single();

            foreach (FieldInfo field in t_MakeRecipeProducts.GetRuntimeFields())
            {
                LogUtil.DebugLog("runtimefield: " + field
                    + ", isRecipeDef: " + (field.Name == "recipeDef")
                    + ", isIngredients: " + (field.Name == "ingredients")
                    );
                if (field.Name == "recipeDef") f_recipeDef = field;
                if (field.Name == "ingredients") f_ingredients = field;
            }

            //f_recipeDef = t_MakeRecipeProducts.GetRuntimeField("recipeDef");
            //f_ingredients = t_MakeRecipeProducts.GetRuntimeField("ingredients");
            
            LogUtil.DebugLog("t_MakeRecipeProducts: " + t_MakeRecipeProducts
                + ", f_recipeDef: " + f_recipeDef
                + ", f_ingredients: " + f_ingredients
                );

            foreach (MethodInfo method in t_MakeRecipeProducts.GetRuntimeMethods())
            {
                if (method.Name.Contains("MoveNext"))
                {
                    LogUtil.DebugLog("method: " + method);
                    return method;
                }
            }
            return null;
        }

        public static MethodInfo m_ButcherProducts = AccessTools.Method(typeof(Thing), nameof(Thing.ButcherProducts));
        public static MethodInfo m_AdjustedButcherProducts = AccessTools.Method(typeof(Patch_MakeRecipeProducts), nameof(Patch_MakeRecipeProducts.AdjustedButcherProducts));
        public static FieldInfo f_recipeDef;
        public static FieldInfo f_ingredients;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> instructions = codeInstructions.ToList();

            foreach (CodeInstruction cur in instructions)
            {
                if (cur.Calls(m_ButcherProducts))
                {
                    //instead want to call AdjustedButcherProducts
                    //curIngredient, worker and efficiency should already be on the stack
                    //so need to load recipeDef and ingredients list
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, f_recipeDef);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, f_ingredients);
                    yield return new CodeInstruction(OpCodes.Call, m_AdjustedButcherProducts);
                }
                else
                {
                    yield return cur;
                }
            }
        }

        public static IEnumerable<Thing> AdjustedButcherProducts(Thing curIngredient, Pawn worker, float efficiency, RecipeDef recipeDef, List<Thing> ingredients)
        {
            LogUtil.DebugLog("AdjustedButcherProducts - recipeDef: " + recipeDef + ", ingredients: " + ingredients + ", curIngredient: " + curIngredient);

            List<Thing> origButcherProducts = curIngredient.ButcherProducts(worker, efficiency).ToList();

            int ingredientDemand = recipeDef.ingredients.First(ic => ic.filter.Allows(curIngredient)).CountRequiredOfFor(curIngredient.def, recipeDef);
            int ingredientSupplied = curIngredient.stackCount;
            float propSupplied = (float)ingredientSupplied / (float)ingredientDemand;

            LogUtil.DebugLog("origButcherProducts: " + origButcherProducts.ToStringSafeEnumerable()
                + ", ingredientDemand: " + ingredientDemand
                + ", ingredientSupplied: " + ingredientSupplied
                + ", propSupplied: " + propSupplied
                );

            foreach (Thing thing in origButcherProducts)
            {
                float floatCount = thing.stackCount * propSupplied;
                int newCount = (int)Math.Round(floatCount);
                LogUtil.DebugLog("thing: " + thing + ", floatCount: " + floatCount + ", newCount: " + newCount);

                thing.stackCount = newCount;
                if (thing.stackCount > 0) yield return thing;
            }
        }
    }
}
