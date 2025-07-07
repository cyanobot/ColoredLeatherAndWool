using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;

namespace ColoredLeatherAndWool
{
    class CompTanningColor : ThingComp
    {
        public CompProperties_TanningColor Props => (CompProperties_TanningColor)this.props;
        public Color BaseColor => Props.baseColor;
        public float Weight => Props.weight;
    }

    class CompProperties_TanningColor : CompProperties
    {
        public Color baseColor = Color.black;
        public float weight = 0f;


        public CompProperties_TanningColor()
        {
            this.compClass = typeof(CompTanningColor);
        }

        public CompProperties_TanningColor(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }

    }
}
