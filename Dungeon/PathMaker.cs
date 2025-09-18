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
            Utilities.Logger.StartMethod();
            Dimensions = dimensions;
            AttemptNumber = 0;
            BranchCount = branchCount;
            BranchCandidates = new List<Vector2I>();
            Utilities.Logger.EndMethod();
        }

        /// <summary>
        /// Initializes, populates, and returns a multi dimensional array representing a dungeon map
        /// </summary>
        /// <param name="pathLength">How long the critical path length should be</param>
        /// <returns></returns>
        public Room[,] CreateDungeonMap(int pathLength)
        {
            InitializeDungeonMap();
            CreateStartingRoom();
            bool criticalPathGenerated = false;
            while (!criticalPathGenerated)
                criticalPathGenerated = GeneratePath(pathLength, Start, Room.CriticalPath);

            int successfulBranchAttempts = 0;
            BranchLength = pathLength / BranchCount;
            foreach (Vector2I candidate in BranchCandidates)
            {
                Utilities.Logger.PrintVariable(AttemptNumber);
                Utilities.Logger.PrintVariable(successfulBranchAttempts);
                if (successfulBranchAttempts == BranchCount)
                    break;

                if (GeneratePath(BranchLength, candidate, Room.BranchPath))
                    successfulBranchAttempts++;
            }
            return Map;
        }


        #region Private Methods

        private bool GeneratePath(int pathLength, Vector2I location, Room roomType)
        {
            Utilities.Logger.PrintVariable(AttemptNumber);
            AttemptNumber++;
            if (pathLength == PathLengthEnd)
                return true;

            Vector2I direction = GetRandomDirection();
            for (int i = Constants.ArrayMin; i < Directions.Length; i++)
            {
                location += direction;
                bool isValid = location.IsInBounds(Dimensions);
                if (isValid)
                {
                    Room previous = Map[location.X, location.Y];
                    isValid = CreateRoom(location, pathLength, roomType);

                    if (isValid)
                        return true;
                    else
                        location = RevertRoom(location, direction, previous, roomType);
                }
                //Will spin to check all options
                direction = new Vector2I(direction.Y, -direction.X);
            }

            return false;
        }

        private void InitializeDungeonMap()
        {
            Utilities.Logger.StartMethod();
            Map = new Room[Dimensions.X, Dimensions.Y];
            for (int x = Constants.ArrayMin; x < Dimensions.X; x++)
            {
                for (int y = Constants.ArrayMin; y < Dimensions.Y; y++)
                {
                    Map[x, y] = Room.Empty;
                }
            }
            Utilities.Logger.EndMethod();
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

        private bool CreateRoom(Vector2I location, int pathLength, Room room)
        {
            if (Map.IsRoom(location, Room.Empty))
            {
                if(room == Room.CriticalPath)
                    BranchCandidates.Add(location);

                Map[location.X, location.Y] = room;

                if (GeneratePath(pathLength - PathLengthChangeIncrement, location, room))
                    return true;
            }
            return false;
        }
        
        private Vector2I RevertRoom(Vector2I location, Vector2I direction, Room previous, Room attemptedRoom)
        {
            if (attemptedRoom == Room.CriticalPath)
                BranchCandidates.Remove(location);

            Map[location.X, location.Y] = previous;
            return location -= direction;
        }
        #endregion


        #region Private Static Methods
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
        #endregion
    }
}
