using D_TopDown.Dungeon;
using D_TopDown.Utilities;
using Godot;
using System;
using System.Security.AccessControl;

public enum Room
{
    Empty,
    Start,
    CriticalPath,
    BranchPath
}

public partial class Dungeon : Node
{
	[Export]
	public Vector2I Dimensions { get; set; }
	[Export] 
	public Vector2I Start { get; set; }
	[Export]
	public int CriticalPathLength { get; set; }
	public Room[,] Map { get; set; }
	// Called when the node enters the scene tree for the first time.
	


	public override void _Ready()
	{
		PathMaker pathMaker = new PathMaker(Dimensions);
		Map = pathMaker.CreateDungeonMap(CriticalPathLength);
		Logger.PrintDungeon(Map);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	

	
}
