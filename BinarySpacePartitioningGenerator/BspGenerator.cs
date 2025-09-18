 using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BspGenerator : Node
{
	[Export]
	public Vector2I MinRoomSize { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public BspDungeonInfo GenerateRooms(Vector2 size)
	{
		Rect2 startingArea = new Rect2(0, 0, size);
		BspDungeonInfo info = new BspDungeonInfo();
		info.Root = new BspRoomNode(startingArea);

		List<BspRoomNode> areaQueue = new List<BspRoomNode>() { info.Root };

		while (areaQueue.Any())
		{
			//Grab then remove first element of list
			BspRoomNode currentArea = areaQueue.First();
			areaQueue.RemoveAt(0);

			List<Rect2> splitAreas = Split(currentArea.Dimensions);
			foreach (Rect2 subArea in splitAreas)
			{
				BspRoomNode subNode = new BspRoomNode(subArea);
				currentArea.Subdivisions.Add(subNode);
				areaQueue.Add(subNode);
			}
			BspEventHandler.InvokeNewSplit(splitAreas);
		}

		return info;
	}

	public List<Rect2> Split(Rect2 area)
	{
		List<Rect2> rects = new List<Rect2>();

		//Check to see if split is too small
		if (area.Size.X < 2 * MinRoomSize.X || area.Size.Y < 2 * MinRoomSize.Y)
			return rects;

		Random rand = new Random();
		bool isVerticalSplit = rand.Next(2) == 0;

		if (!isVerticalSplit)
		{
			int splitX = rand.Next(MinRoomSize.X, (int)area.Size.X - MinRoomSize.X);
			rects.Add(new Rect2(area.Position, new Vector2(splitX, area.Size.Y)));
			rects.Add(new Rect2(area.Position + new Vector2(splitX, 0), new Vector2(area.Size.X - splitX, area.Size.Y)));
		}
		else
		{
			int splitY = rand.Next(MinRoomSize.Y, (int)area.Size.Y - MinRoomSize.Y);
			rects.Add(new Rect2(area.Position, new Vector2(area.Size.X, splitY)));
			rects.Add(new Rect2(area.Position + new Vector2(0, splitY), new Vector2(area.Size.X, area.Size.Y - splitY)));
		}

		return rects;
	}
}
