using System.Collections.Generic;
using System.Linq;
using D_TopDown.Dungeon;
using Godot;
using Util = D_TopDown.Utilities;

public partial class MdaDungeonVisualizer : Node2D
{
	[Export]
	public Vector2 RoomSize { get; set; }
	private List<MdaRoom> RoomQueue { get; set; }
	private List<MdaRoom> EmptyRooms { get; set; }
	private List<MdaRoom> RemoveQueue { get; set; }
	private Dictionary<MdaRoom, Color> AlreadyDrawn { get; set; }
	private MdaDungeon Dungeon { get; set; }
	private Godot.Timer Timer { get; set; }
	private Vector2 OffsetModifier { get; set; }
	private Vector2 CurrentPosition { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OffsetModifier = RoomSize * 2;
		CurrentPosition = new Vector2(0, 0);
		Dungeon = GetParent<MdaDungeon>();
		Timer = GetNode<Godot.Timer>("../Timer");
		RoomQueue = new List<MdaRoom>();
		EmptyRooms = new List<MdaRoom>();
		RemoveQueue = new List<MdaRoom>();
		AlreadyDrawn = new Dictionary<MdaRoom, Color>();
		Timer.Timeout += OnTimeOut;
		MdaEventHandler.RoomAdded += OnRoomAdded;
		MdaEventHandler.RoomRemoved += OnRoomRemoved;
	}

	public override void _Draw()
	{
		// foreach (MdaRoom removeRoom in RemoveQueue)
		// 	AlreadyDrawn.Remove(removeRoom);
		MdaRoom removeRoom = RemoveQueue.FirstOrDefault();
		if (removeRoom != null)
		{
			AlreadyDrawn.Remove(removeRoom);
			RemoveQueue.RemoveAt(0);
		} 

		foreach (MdaRoom emptyRoom in EmptyRooms)
			DrawRect(emptyRoom.GetRoomRect(), Colors.Black, false, 2);

		foreach (KeyValuePair<MdaRoom, Color> entry in AlreadyDrawn)
				DrawRect(entry.Key.GetRoomRect(), entry.Value, true);

		MdaRoom room = RoomQueue.FirstOrDefault();
		if (room != null)
		{
			Color color = GetRoomColor(room.Type);
			DrawRect(room.GetRoomRect(), color, true);
			AlreadyDrawn[room] = color;
			RoomQueue.RemoveAt(0);
		}
	}

	public override void _ExitTree()
	{
		MdaEventHandler.RoomAdded -= OnRoomAdded;
		MdaEventHandler.RoomRemoved -= OnRoomRemoved;
	}

	#region Event Handlers

	private void OnRoomAdded(MdaRoom roomAdd)
	{
		if (roomAdd.Type == RoomType.Empty)
		{
			EmptyRooms.Add(roomAdd);
		}
		else
		{
			RoomQueue.Add(roomAdd);
		}
		Util.Logger.VarPrint(roomAdd);
	}
	private void OnRoomRemoved(MdaRoom room) => AlreadyDrawn.Remove(room);
	private void OnTimeOut() => QueueRedraw();

	#endregion

	#region Private Static Methods

	private static Color GetRoomColor(RoomType type)
	{
		switch (type)
		{
			case RoomType.Empty:
				return Colors.Black;
			case RoomType.Start:
				return Colors.Green;
			case RoomType.CriticalPath:
				return Colors.Red;
			case RoomType.BranchPath:
				return Colors.Blue;
			case RoomType.End:
				return Colors.Purple;
			default:
				return Colors.White;
		}
	}

	#endregion
}
