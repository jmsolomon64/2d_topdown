using Godot;
using System;

public static class MdaEventHandler
{
	public static Action<MdaRoom> RoomAdded;
	public static Action<MdaRoom> RoomRemoved;

	public static void InvokeRoomAdded(MdaRoom room) => RoomAdded?.Invoke(room);
	public static void InvokeRoomRemoved(MdaRoom room) => RoomRemoved?.Invoke(room);
}
