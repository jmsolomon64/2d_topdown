using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using D_TopDown.Utilities;

public partial class BspVisualizer : Node2D
{
	[Export]
	BspGenerator Generator { get; set; }
	BspDungeonInfo Info { get; set; }
	[Export]
	Timer Time { get; set; }
	List<List<Rect2>> DrawQueue { get; set; }
	Dictionary<Rect2, Color> AlreadyDrawn { get; set; }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DrawQueue = new List<List<Rect2>>();
		AlreadyDrawn = new Dictionary<Rect2, Color>();
		Time.Timeout += OnTimeOut;
		BspEventHandler.NewSplit += OnNewSplit;
		Info = Generator.GenerateRooms(new Vector2(960, 540));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Draw()
	{
		foreach (KeyValuePair<Rect2, Color> entry in AlreadyDrawn)
		{
			DrawRect(entry.Key, entry.Value, true);
		}

		Random random = new Random();
		foreach (Rect2 rect in DrawQueue.First())
		{
			Color color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
			DrawRect(rect, color, true);
			AlreadyDrawn[rect] = color;
		}
		DrawQueue.RemoveAt(0);
	}

	private void OnTimeOut() => QueueRedraw();

	private void OnNewSplit(List<Rect2> splits) => DrawQueue.Add(splits);
}
