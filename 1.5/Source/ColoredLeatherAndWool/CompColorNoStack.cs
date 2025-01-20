using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;
using static ColoredLeatherAndWool.Settings;
using Verse.AI;

namespace ColoredLeatherAndWool
{
    class CompColorNoStack : ThingComp
    {

        public override bool AllowStackWith(Thing other)
        {
            LogUtil.DebugLog("AllowStackWith - parent: " + parent
                + ", other: " + other
                );
            if (disableStacking && !GenColor.IndistinguishableFrom(parent.DrawColor, other.DrawColor))
            {
                Map map = parent.MapHeld ?? other.MapHeld;
                if (map == null) return true;
                LogUtil.DebugLog(parent + ".IsReserved: " + map.reservationManager.IsReserved(parent) + ", "
                    + other + ".IsReserved: " + map.reservationManager.IsReserved(other));

                if (map.reservationManager.IsReserved(parent))
                {
                    if (!other.SpawnedOrAnyParentSpawned) return true;
                    if (map.reservationManager.IsReserved(other))
                    {
                        HashSet<Pawn> reserversOf_parent = new HashSet<Pawn>();
                        HashSet<Pawn> reserversOf_other = new HashSet<Pawn>();
                        map.reservationManager.ReserversOf(parent, reserversOf_parent);
                        map.reservationManager.ReserversOf(other, reserversOf_other);

                        LogUtil.DebugLog(parent + ".ReserversOf: " + reserversOf_parent.ToStringSafeEnumerable());
                        LogUtil.DebugLog(other + ".ReserversOf: " + reserversOf_other.ToStringSafeEnumerable());

                        if (reserversOf_parent.Any(p => reserversOf_other.Contains(p)))
                        {
                            return true;
                        }
                    }
                }
                else if (map.reservationManager.IsReserved(other))
                {
                    if (!parent.SpawnedOrAnyParentSpawned) return true;
                }
                return false;
            }
            else return true;
        }

    }
}
