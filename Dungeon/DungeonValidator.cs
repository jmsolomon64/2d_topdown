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
        public static bool ValidateStart(Vector2I start, Vector2I dimmensions)
        {
            return IsInLowerLimit(start) && IsInUpperLimit(start, dimmensions);
        }

        public static bool ValidateRoomPlacement(Vector2I location, Vector2I dimmensions, Room[,] map)
        {
            bool isValid = IsInLowerLimit(location) && IsInUpperLimit(location, dimmensions);
            if(isValid) isValid = IsNotOccupied(location, map);
            return isValid;
        }

        private static bool IsInUpperLimit(Vector2I location, Vector2I dimmensions)
        {
            return location.X < dimmensions.X
                && location.Y < dimmensions.Y;
        }

        private static bool IsInLowerLimit(Vector2I location)
        {
            return location.X >= Constants.ArrayMin
                && location.Y >= Constants.ArrayMin;
        }

        private static bool IsNotOccupied(Vector2I location, Room[,] map) => map[location.X, location.Y] == Room.Empty;
    }
}
