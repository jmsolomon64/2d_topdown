using Godot;
using System;
using System.Collections.Generic;

public partial class BspRoomNode : Resource
{
    [Export]
    public Rect2 Dimensions { get; set; }
    public List<BspRoomNode> Subdivisions { get; set; }

    public BspRoomNode(Rect2 dimensions)
    {
        Dimensions = dimensions;
        Subdivisions = new List<BspRoomNode>();
    }
}
