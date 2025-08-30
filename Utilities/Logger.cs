using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace D_TopDown.Utilities
{
    public enum LogLevel
    {
        Info,
        Warn,
        Error
    }

    public static class Logger
    {
        /// <summary>
        /// Set to 2 because method that invoked logging will be two calls behind GetInvokedMethod()
        /// </summary>
        private static int InvokedMethodIndex = 2;
        public static void PrintDungeon(Room[,] map)
        {
            int cols = map.GetLength(Constants.MultiDimArrayGetLengthCol);
            int rows = map.GetLength(Constants.MultiDimArrayGetLengthRow);
            StringBuilder sb = new StringBuilder();
            for (int x = Constants.ArrayMin; x < rows; ++x)
            {
                for (int y = Constants.ArrayMin; y < cols; y++)
                {
                    sb.Append($"[{GetShortNameForRoom(map[y, x])}]");
                }
                sb.AppendLine();
            }
            GD.Print(sb.ToString());
        }

        //forgot the link for this one...
        public static void PrintVariable(object value, LogLevel level = LogLevel.Info, [CallerArgumentExpression(nameof(value))] string valueVarName = "") =>
            Out($"{valueVarName}: {value}", level);
        public static void StartMethod() => Out($"Starting: {GetInvokedMethodName()}()", LogLevel.Info);
        public static void EndMethod() => Out($"Finished: {GetInvokedMethodName()}()", LogLevel.Info);
        public static void FailedMethod() => Out($"Failed: {GetInvokedMethodName()}", LogLevel.Error);
        public static void PrintEx(Exception ex) => Out(ex.ToString(), LogLevel.Error);

        public static void Out(string data, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info:
                    GD.Print(data);
                    break;
                case LogLevel.Warn:
                    GD.PushWarning(data);
                    break;
                case LogLevel.Error:
                    GD.PrintErr(data);
                    break;
            }
        }

        //https://stackoverflow.com/questions/2652460/how-to-get-the-name-of-the-current-method-from-code
        private static string GetInvokedMethodName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(InvokedMethodIndex);
            return sf.GetMethod().Name;
        }

        private static string GetShortNameForRoom(Room room)
        {
            switch (room)
            {
                case Room.Empty:
                    return "E";
                case Room.Start:
                    return "S";
                case Room.CriticalPath:
                    return "C";
                case Room.BranchPath:
                    return "B";
                default:
                    throw new Exception("Failed to get name for room");
            }
        }


    }
}
