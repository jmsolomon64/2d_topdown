using D_TopDown.Utilities;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace D_TopDown.Dungeon
{
    public class PathMaker
    {
        private Vector2I Start { get; set; }
        private Vector2I CurrentDirection { get; set; }
        private Vector2I Dimensions { get; set; }
        private Room[,] Map { get; set; }
        private static int PathLengthChangeIncrement = 1;
        private static int PathLengthEnd = 0;
        private static int MaxDimensionSize = 10;
        private static int MinDimensionSize = 5;
        private int AttemptNumber { get; set; }
        private static Vector2I[] Directions =
        {
            Vector2I.Down,
            Vector2I.Up,
            Vector2I.Left,
            Vector2I.Right
        };

        public PathMaker(Vector2I dimensions)
        {
            Logger.StartMethod();
            Dimensions = dimensions;
            AttemptNumber = 0;
            Logger.EndMethod();
        }

        public Room[,] CreateDungeonMap(int pathLength)
        {
            InitializeDungeonMap();
            CreateStartingRoom();
            bool criticalPathGenerated = false;
            while(!criticalPathGenerated)
                criticalPathGenerated = GeneratePath(pathLength, Start, Room.CriticalPath);
            return Map;
        }


        private void InitializeDungeonMap()
        {
            Logger.StartMethod();
            Map = new Room[Dimensions.X, Dimensions.Y];
            for (int x = Constants.ArrayMin; x < Dimensions.X; x++)
            {
                for (int y = Constants.ArrayMin; y < Dimensions.Y; y++)
                {
                    Map[x, y] = Room.Empty;
                }
            }
            Logger.EndMethod();
        }

        private void CreateStartingRoom()
        {
            if (Start == Vector2I.Zero) Start = GetRandomVector2I(Dimensions);
            else
            {
                if (!DungeonValidator.ValidateStart(Start, Dimensions))
                    throw new Exception("Starting room is out of bounds");
            }
            Start = Vector2I.Zero;
            Map[Start.X, Start.Y] = Room.Start;
        }

        public bool GeneratePath(int pathLength, Vector2I location, Room roomType)
        {
            Logger.PrintVariable(AttemptNumber);
            AttemptNumber++;
            if (pathLength == PathLengthEnd)
                return true;

            CurrentDirection = GetRandomDirection();
            for (int i = Constants.ArrayMin; i < Directions.Length; i++)
            {
                location += CurrentDirection;
                if (DungeonValidator.ValidateRoomPlacement(location, Dimensions, Map))
                {
                    try
                    {
                        Vector2I validateLocation = location;
                        Logger.PrintVariable(validateLocation);
                        Room previous = Map[location.X, location.Y];
                        Map[location.X, location.Y] = roomType;

                        if (GeneratePath(pathLength - PathLengthChangeIncrement, location, roomType))
                            return true;
                        else
                        {
                            Map[validateLocation.X, validateLocation.Y] = previous;
                            location -= CurrentDirection;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.FailedMethod();
                        Logger.PrintEx(ex);
                    }
                }
                //Will spin to check all options
                CurrentDirection = new Vector2I(CurrentDirection.Y, -CurrentDirection.X);
            }

            return false;
        }

        public static Vector2I GetRandomVector2I(Vector2I dimmensions)
        {
            Random random = new Random();
            int x = random.Next(Constants.ArrayMin, dimmensions.X);
            int y = random.Next(Constants.ArrayMin, dimmensions.Y);
            return new Vector2I(x, y);
        }

        public static Vector2I GetRandomDirection()
        {
            Random random = new Random();
            return Directions[random.Next(Constants.ArrayMin, Directions.Length - Constants.ArrayPadding)];
        }
    }
}
