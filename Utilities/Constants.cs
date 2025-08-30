using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D_TopDown.Utilities
{
    public static class Constants
    {
        public static int ArrayPadding = 1;
        public static int ArrayMin = 0;
        public static int MultiDimArrayGetLengthCol = 0;
        public static int MultiDimArrayGetLengthRow = 1;

        public static int GetLengthOfMapCols<T>(T[,] multiArray) =>
            multiArray.GetLength(MultiDimArrayGetLengthCol) - ArrayPadding;
        public static int GetLengthOfMapRows<T>(T[,] multiArray) =>
            multiArray.GetLength(MultiDimArrayGetLengthRow) - ArrayPadding;
    }
}
