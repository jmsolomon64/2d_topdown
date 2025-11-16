using Godot;
using System;
using System.Collections.Generic;

public partial class BspDungeonInfo : Resource
{
    public BspRoomNode Root { get; set; }
    public List<BspRoomNode> AllSubdivisions { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }

    public BspDungeonInfo()
    {
        AllSubdivisions = new List<BspRoomNode>(); 
    }
}
