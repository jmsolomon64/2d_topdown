using D_TopDown.Utilities;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D_TopDown.Dungeon
{
    public static class DungeonValidator
    {
        public static bool IsInBounds(this Vector2I location, Vector2I dimmensions)
        {
            return location.IsInLowerLimit() && IsInUpperLimit(location, dimmensions);
        }

        public static bool IsInUpperLimit(this Vector2I location, Vector2I dimmensions)
        {
            return location.X < dimmensions.X
                && location.Y < dimmensions.Y;
        }

        public static bool IsInLowerLimit(this Vector2I location)
        {
            return location.X >= Constants.ArrayMin
                && location.Y >= Constants.ArrayMin;
        }

        public static bool IsRoom(this Room[,] map, Vector2I location, Room room) => map[location.X, location.Y] == room;
    }
}
