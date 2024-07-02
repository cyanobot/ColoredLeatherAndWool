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
    class Settings : ModSettings
    {
        public static bool tannedColors = true;
        public static bool disableStacking = false;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref tannedColors, "tannedColors", tannedColors, true);
            Scribe_Values.Look(ref disableStacking, "disableStacking", disableStacking, true);

            //Scribe_Values.Look(ref name, "name", name, true);
        }

        public static void DoSettingsWindowContents(Rect rect)
        {
            Listing_Standard l = new Listing_Standard(GameFont.Small)
            {
                ColumnWidth = rect.width
            };

            l.Begin(rect);

            l.CheckboxLabeled("CG_SettingLabel_DisableStacking".Translate() + " : ", ref disableStacking, tooltip: "CG_SettingDesc_DisableStacking".Translate());
            l.CheckboxLabeled("CG_SettingLabel_TannedColors".Translate() + " : ", ref tannedColors, tooltip: "CG_SettingDesc_TannedColors".Translate());

            l.End();

            //Main.ApplySettings();
        }
    }
}
