using Godot;
using System;

public partial class MdaRoom
{
	public RoomType Type { get; set; }
	public Rect2 Space { get; set; }
	//need to set up some bit masking here for the doors

	private static int DefaultRectSize = 20;

	public MdaRoom(RoomType type, Vector2I position, int? size = null)
	{
		Type = type;
		Space = new Rect2(position, GetRectSize(size));
	}

	public MdaRoom(RoomType type, Rect2 space)
	{
		Type = type;
		Space = space;
	}

	public Rect2 GetRoomRect() => new Rect2(Space.Position * Space.Size, Space.Size);

	private static Vector2 GetRectSize(int? size = null)
	{
		if (size.HasValue)
			return new Vector2(size.Value, size.Value);
		else
			return new Vector2(DefaultRectSize, DefaultRectSize);
	}
}
