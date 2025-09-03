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
        public static bool ValidateLimits(this Vector2I location, Vector2I dimmensions)
        {
            return location.IsInLowerLimit() && location.IsInUpperLimit(dimmensions);
        }

        public static bool ValidateCriticalPlacement(this Room[,] map, Vector2I location, Vector2I dimmensions)
        {
            bool isValid = location.IsInLowerLimit() && location.IsInUpperLimit(dimmensions);
            if (isValid) isValid = map.IsNotOccupied(location);
            return isValid;
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

        public static bool IsNotOccupied(this Room[,] map,Vector2I location) => map[location.X, location.Y] == Room.Empty;

        public static bool IsCriticalRoom(this Room[,] map, Vector2I location) => map[location.X, location.Y] == Room.CriticalPath;
    }
}
