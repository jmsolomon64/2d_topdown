using D_TopDown.Dungeon;
using D_TopDown.Utilities;
using Godot;
using System;
using System.Security.AccessControl;

public enum RoomType
{
	Empty,
	Start,
	CriticalPath,
	BranchPath,
	End
}

public partial class MdaDungeon : Node
{
	[Export]
	public Vector2I Dimensions { get; set; }
	[Export] 
	public Vector2I Start { get; set; }
	[Export]
	public int CriticalPathLength { get; set; }
	public RoomType[,] Map { get; set; }

	private bool IsGenerated = false;
	// Called when the node enters the scene tree for the first time.



	public override void _Ready()
	{
		//PathMaker pathMaker = new PathMaker(Dimensions, CriticalPathLength / 2);
		//Map = pathMaker.CreateDungeonMap(CriticalPathLength);
		//D_TopDown.Utilities.Logger.PrintDungeon(Map);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Node child = GetNodeOrNull("DungeonGeneratorVisualizer");
		if (child != null && !IsGenerated)
		{
			IsGenerated = true;
			PathMaker pathMaker = new PathMaker(Dimensions, CriticalPathLength / 2);
			Map = pathMaker.CreateDungeonMap(CriticalPathLength); 
		}
	}

	

	
}
