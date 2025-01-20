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
using static ColoredLeatherAndWool.Main;

namespace ColoredLeatherAndWool
{
    [StaticConstructorOnStartup]
    class Init
    {
        static Init()
        {
            //Main.ApplySettings();

            /*
            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "sarg.alphaanimals"))
                AALoaded = true;

            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "biotexpans.core"))
                BTELoaded = true;

            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "doctorstupid.prettyskin"))
                humanButcheryLoaded = true;
            
            */

            //Log.Message("AALoaded: " + AALoaded);

            //var harmony = new Harmony("cyanobot.ColoredLeatherAndWool");

            //Main.harmony.PatchAll();
        }
    }
}
