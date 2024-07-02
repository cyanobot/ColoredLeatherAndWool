using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;
using static ColoredLeatherAndWool.Settings;

namespace ColoredLeatherAndWool
{
    class CompColorNoStack : ThingComp
    {

        public override bool AllowStackWith(Thing other)
        {
            if (disableStacking && !GenColor.IndistinguishableFrom(parent.DrawColor, other.DrawColor)) return false;
            else return true;
        }
    }
}
