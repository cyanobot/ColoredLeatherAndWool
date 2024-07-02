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
        public static bool AALoaded = false;
        public static bool BTELoaded = false;
        //public static bool humanButcheryLoaded = false;

        public static bool logSetColorFailures = true;

        public Main(ModContentPack mcp) : base(mcp)
        {
            GetSettings<Settings>();
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
