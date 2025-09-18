using Godot;
using System;
using System.Collections.Generic;

public static class BspEventHandler
{
    public static Action<List<Rect2>> NewSplit;

    public static void InvokeNewSplit(List<Rect2> rects) => NewSplit?.Invoke(rects);
}
