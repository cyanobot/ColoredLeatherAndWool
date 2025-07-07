using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ColoredLeatherAndWool
{
    public class Main : Mod
    {
        public static bool alphaAnimalsLoaded = false;
        public static bool biotechExpansionLoaded = false;
        public static bool betterWoolLoaded = false;
        public static bool cyanobotsLeatherLoaded = false;
        public static bool medievalOverhaulLoaded = false;
        public static bool anyTanningLoaded = false;

        public static bool logSetColorFailures = true;

        public static Harmony harmony;

        public Main(ModContentPack mcp) : base(mcp)
        {
            alphaAnimalsLoaded = ModsConfig.IsActive("sarg.alphaanimals");
            biotechExpansionLoaded = ModsConfig.IsActive("biotexpans.core");
            betterWoolLoaded = ModsConfig.IsActive("divineDerivative.AutoWool");
            cyanobotsLeatherLoaded = ModsConfig.IsActive("cyanobot.leatheroverhaul");
            medievalOverhaulLoaded = ModsConfig.IsActive("DankPyon.Medieval.Overhaul");

            anyTanningLoaded = cyanobotsLeatherLoaded || medievalOverhaulLoaded;

            harmony = new Harmony("cyanobot.ColoredLeatherAndWool");
            
            GetSettings<Settings>();

            harmony.PatchAll();
        }

        public override string SettingsCategory()
        {
            return "Colored Leather And Wool";
        }

        /*
        static public void ApplySettings()
        {

        }
        */

        public override void DoSettingsWindowContents(Rect inRect) => Settings.DoSettingsWindowContents(inRect);
    }

}
