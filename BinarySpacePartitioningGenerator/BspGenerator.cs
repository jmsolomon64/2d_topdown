using D_TopDown.Utilities;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BspGenerator : Node
{
	[Export]
	public Vector2I MinPartitionSize { get; set; }
	[Export]
	private Vector2I MaxRoomMargin { get; set; }
	[Export]
	private int RoomCount { get; set; }
	private int Ascii = 64; //Starts at A

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
		info.Root.Id = (char)Ascii;
		Ascii++;

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
				subNode.Id = (char)Ascii;
				currentArea.Subdivisions.Add(subNode);
				areaQueue.Add(subNode);
				Ascii++;
			}
			//BspEventHandler.InvokeNewSplit(splitAreas);
		}

		return info;
	}

	public List<BspRoomNode> GroupRooms(BspDungeonInfo info)
	{
		List<BspRoomNode> nodes = GetRoomsFromBranchLevel(4, info.Root.Subdivisions);
		foreach(BspRoomNode node in nodes)
		{
			//Logger.VarPrint(node.Id);
			Vector2 position = node.Dimensions.Position + GetOffset();
			SpaceRooms(node.Subdivisions, position);
        }

		var addedRooms = nodes.SelectMany(x => x.Subdivisions.Select(y => y.Dimensions)).ToList();
		BspEventHandler.InvokeNewSplit(addedRooms);
		D_TopDown.Utilities.Logger.VarPrint(nodes.Count());
		return nodes;
	}
	
	public void SpaceRooms(List<BspRoomNode> nodes, Vector2 offset)
	{
		if (nodes == null)
			return;

		foreach (BspRoomNode node in nodes)
		{
			Vector2 position = node.Dimensions.Position + offset;
			node.Dimensions = new Rect2(position, node.Dimensions.Size);
		}

		List<BspRoomNode> subdivisions = nodes.SelectMany(x => x.Subdivisions)?.ToList();
		//SpaceRooms(subdivisions, offset);
    }

	public List<BspRoomNode> GetRoomsFromBranchLevel(int level, List<BspRoomNode> traversedNodes)
	{
		if (level == 0)
			return traversedNodes;

		List<BspRoomNode> nodes = new List<BspRoomNode>();
		foreach (BspRoomNode node in traversedNodes)
			nodes.AddRange(node.Subdivisions);

		return GetRoomsFromBranchLevel(level - 1, nodes);
    }

	private List<Rect2> Split(Rect2 area)
	{
		List<Rect2> rects = new List<Rect2>();

		//Check to see if split is too small
		if (area.Size.X < 2 * MinPartitionSize.X || area.Size.Y < 2 * MinPartitionSize.Y)
			return rects;

		Random rand = new Random();
		bool isVerticalSplit = rand.Next(2) == 0;

		if (!isVerticalSplit)
		{
			int splitX = rand.Next(MinPartitionSize.X, (int)area.Size.X - MinPartitionSize.X);
			rects.Add(new Rect2(area.Position, new Vector2(splitX, area.Size.Y)));
			rects.Add(new Rect2(area.Position + new Vector2(splitX, 0), new Vector2(area.Size.X - splitX, area.Size.Y)));
		}
		else
		{
			int splitY = rand.Next(MinPartitionSize.Y, (int)area.Size.Y - MinPartitionSize.Y);
			rects.Add(new Rect2(area.Position, new Vector2(area.Size.X, splitY)));
			rects.Add(new Rect2(area.Position + new Vector2(0, splitY), new Vector2(area.Size.X, area.Size.Y - splitY)));
		}

		return rects;
	}

	private Vector2 GetOffset()
	{
		Random random = new Random();
        int x = random.Next(-MaxRoomMargin.X, MaxRoomMargin.X);
		int y = random.Next(-MaxRoomMargin.Y, MaxRoomMargin.Y);
		return new Vector2(x, y);
    }
}
