using Godot;
using System;
using System.Collections.Generic;

public partial class BspDungeonInfo : Resource
{
    public BspRoomNode Root { get; set; }
    public List<BspRoomNode> AllSubdivisions { get; set; }
    public Vector2 MinSize { get; set; }
    public string Name { get; set; }

    public BspDungeonInfo()
    {
        AllSubdivisions = new List<BspRoomNode>();
    }

    // Add list here to get every subdivision under it 
    // can determine max size of a room and grab partitions meeting criteria
    // then I can work on an algorithm to procedurally remove rectangles 

}
