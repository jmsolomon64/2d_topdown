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
        private List<Vector2I> BranchCandidates { get; set; }
        private int BranchCount { get; set; }
        private int BranchLength { get; set; }
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
            bool criticalPathGenerated = false;
            while(!criticalPathGenerated)
                criticalPathGenerated = GeneratePath(pathLength, Start, Room.CriticalPath);

            int successfulBranchAttempts = 0;
            AttemptNumber = 0;
            foreach (Vector2I candidate in BranchCandidates)
            {
                Logger.PrintVariable(AttemptNumber);
                Logger.PrintVariable(successfulBranchAttempts);
                if (successfulBranchAttempts == BranchCount)
                    break;

                if (GeneratePath(BranchLength, candidate, Room.BranchPath))
                    successfulBranchAttempts++;
            }
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
                if (!Start.IsInBounds(Dimensions))
                    throw new Exception("Starting room is out of bounds");
            }
            Start = Vector2I.Zero;
            Map[Start.X, Start.Y] = Room.Start;
        }

        private bool CreateCriticalRoom(Vector2I location, int pathLength)
        {
            if (Map.IsRoom(location, Room.Empty))
            {
                BranchCandidates.Add(location);
                Map[location.X, location.Y] = Room.CriticalPath;
                if (GeneratePath(pathLength - PathLengthChangeIncrement, location, Room.CriticalPath))
                    return true;
            }
            return false;
        }

        private Vector2I RevertCriticalRoom(Vector2I location, Room previous)
        {
            BranchCandidates.Remove(location);
            Map[location.X, location.Y] = previous;
            return location -= CurrentDirection;
        }

        private Vector2I RevertBranchRoom(Vector2I location, Room previous)
        {
            Map[location.X, location.Y] = previous;
            return location -= CurrentDirection;
        }

        private bool CreateBranchRoom(Vector2I location, int pathLength)
        {
            Logger.StartMethod();
            bool isEmpty = Map.IsRoom(location, Room.Empty);
            bool isCriticalRoom = Map.IsRoom(location, Room.CriticalPath);
            if (isEmpty)
            {
                Logger.PrintVariable(isEmpty);
                Map[location.X, location.Y] = Room.BranchPath;
                if (GeneratePath(pathLength - PathLengthChangeIncrement, location, Room.BranchPath))
                    return true;
            }
            else if (isCriticalRoom)
            {
                Logger.PrintVariable(isCriticalRoom);
                if (GeneratePath(pathLength, location, Room.BranchPath))
                    return true;
            }
            Logger.EndMethod();
            return false;
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
                bool isValid = location.IsInBounds(Dimensions);
                if (isValid)
                {
                    Room previous = Map[location.X, location.Y];
                    switch (roomType)
                    {
                        case Room.CriticalPath:
                            isValid = CreateCriticalRoom(location, pathLength);
                            break;
                        case Room.BranchPath:
                            isValid = CreateBranchRoom(location, pathLength);
                            break;
                    }

                    if (isValid)
                        return true;
                    else
                    {
                        switch (roomType)
                    {
                        case Room.CriticalPath:
                            location = RevertCriticalRoom(location, previous);
                            break;
                        case Room.BranchPath:
                            location = RevertBranchRoom(location, previous);
                            break;
                    }
                        location -= CurrentDirection;
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
