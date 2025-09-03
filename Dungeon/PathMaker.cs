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
        private List<Vector2I> BranchCandidates { get; set; }
        private Room[,] Map { get; set; }
        private int BranchCount { get; set; }
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

        public PathMaker(Vector2I dimensions, int branchCount)
        {
            Logger.StartMethod();
            Dimensions = dimensions;
            AttemptNumber = 0;
            BranchCount = branchCount;
            BranchCandidates = new List<Vector2I>();
            Logger.EndMethod();
        }

        public Room[,] CreateDungeonMap(int pathLength)
        {
            InitializeDungeonMap();
            CreateStartingRoom();

            //Create the critical path
            bool criticalPathGenerated = false;
            while (!criticalPathGenerated)
                criticalPathGenerated = GenerateCriticalPath(pathLength, Start);

            //Attempt to create branch paths
            // int successfulAttempts = 0;
            // foreach (Vector2I candidate in BranchCandidates)
            // {
            //     if (successfulAttempts == BranchCount)
            //         break;

            //     if (GenerateCriticalPath(GetBranchLength(), candidate, Room.BranchPath))
            //         successfulAttempts++;
            // }

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
                if (!DungeonValidator.ValidateLimits(Start, Dimensions))
                    throw new Exception("Starting room is out of bounds");
            }
            Start = Vector2I.Zero;
            Map[Start.X, Start.Y] = Room.Start;
        }

        public bool GenerateCriticalPath(int pathLength, Vector2I location)
        {
            Logger.PrintVariable(AttemptNumber);
            AttemptNumber++;
            if (pathLength == PathLengthEnd)
                return true;

            CurrentDirection = GetRandomDirection();
            for (int i = Constants.ArrayMin; i < Directions.Length; i++)
            {
                location += CurrentDirection;
                if (Map.ValidateCriticalPlacement(location, Dimensions))
                {
                    try
                    {
#if DEBUG
                        Vector2I validateLocation = location;
                        Logger.PrintVariable(validateLocation);
#endif
                        Room previous = Map[location.X, location.Y];
                        Map[location.X, location.Y] = Room.CriticalPath;
                        BranchCandidates.Add(location);

                        if (GenerateCriticalPath(pathLength - PathLengthChangeIncrement, location))
                            return true;
                        else
                        {
                            Map[validateLocation.X, validateLocation.Y] = previous;
                            BranchCandidates.Remove(location);
                            location -= CurrentDirection;
                            CurrentDirection = new Vector2I(CurrentDirection.Y, -CurrentDirection.X);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.FailedMethod();
                        Logger.PrintEx(ex);
                    }
                }
                //Will spin to check all options
                //CurrentDirection = new Vector2I(CurrentDirection.Y, -CurrentDirection.X);
            }

            return false;
        }

        public bool GenerateBranchPaths(int pathLength, Vector2I location)
        {
            if (pathLength == PathLengthEnd)
                return true;

            CurrentDirection = GetRandomDirection();
            for (int i = Constants.ArrayMin; i < Directions.Length; i++)
            {
                location += CurrentDirection;
                if (Map.IsCriticalRoom(location))
                {
                    
                }
                else
                {

                }
            }
        }

        private int GetBranchLength()
        {
            int emptyRooms = GetAmountOfEmptyRooms();
            int roomCount = Dimensions.X + Dimensions.Y;
            return (roomCount - emptyRooms) / BranchCount;
        }

        private int GetAmountOfEmptyRooms()
        {
            int emptyRooms = 0;
            foreach (Room room in Map)
            {
                if (room == Room.Empty)
                    emptyRooms++;
            }
            return emptyRooms;
        }

        private static Vector2I GetRandomVector2I(Vector2I dimmensions)
        {
            Random random = new Random();
            int x = random.Next(Constants.ArrayMin, dimmensions.X);
            int y = random.Next(Constants.ArrayMin, dimmensions.Y);
            return new Vector2I(x, y);
        }

        private static Vector2I GetRandomDirection()
        {
            Random random = new Random();
            return Directions[random.Next(Constants.ArrayMin, Directions.Length - Constants.ArrayPadding)];
        }
        
    }
}
