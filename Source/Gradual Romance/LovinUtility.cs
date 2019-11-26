using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Gradual_Romance
{
    static class LovinUtility
    {

        public static Room FindNearbyPrivateRoom(Pawn pawn)
        {
            Region currentRegion = RegionAndRoomQuery.GetRegion(pawn);
            Room currentRoom = RegionAndRoomQuery.GetRoom(pawn);

            return null;
        }

        public static bool RoomIsPrivateFor(Room room, Pawn pawn, Pawn other)
        {
            if (room.Role == RoomRoleDefOf.RecRoom || room.Role == RoomRoleDefOf.DiningRoom)
            {
                return false;
            }

            if (room.Role == RoomRoleDefOf.Bedroom && room.Owners.Count() > 0)
            {
                if (!room.Owners.Contains(pawn) && !room.Owners.Contains(other))
                {
                    return false;
                }
            }

            IEnumerable<Pawn> roomPawns = (from thing in room.ContainedAndAdjacentThings
                                          where thing is Pawn && thing != pawn && thing != other && (thing as Pawn).NonHumanlikeOrWildMan()
                                          select thing) as IEnumerable<Pawn>;
            if (roomPawns.Count() != 0)
            {
                return false;
            }
            return true;
        }

    }
}
