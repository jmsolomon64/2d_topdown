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
	Godot.Timer Time { get; set; }
	[Export]
	Font Font { get; set; }
	List<List<Rect2>> DrawQueue { get; set; }
	Dictionary<int, VisualizerInfo> AlreadyDrawn { get; set; }
	private int RoomCount = 0;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DrawQueue = new List<List<Rect2>>();
		AlreadyDrawn = new Dictionary<int, VisualizerInfo>();
		Time.Timeout += OnTimeOut;
		BspEventHandler.NewSplit += OnNewSplit;
		Info = Generator.GenerateRooms(new Vector2(960, 540));
		var nodes = Generator.GroupRooms(Info);
		D_TopDown.Utilities.Logger.VarPrint(nodes.Count());
		foreach(BspRoomNode node in nodes)
        {
			D_TopDown.Utilities.Logger.VarPrint(node.Id);
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Draw()
	{
		foreach (KeyValuePair<int, VisualizerInfo> entry in AlreadyDrawn)
		{
			DrawRect(entry.Value.Rect, entry.Value.Color, true);
			//DrawString(Font, entry.Value.TextPosition, entry.Key.ToString(), HorizontalAlignment.Center);
		}

		if(DrawQueue.Any())
        {
            Random random = new Random();
			foreach (Rect2 rect in DrawQueue.First())
			{
				Vector2 position = rect.Position / rect.End;
				Color color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
				DrawRect(rect, color, true);
				AlreadyDrawn[RoomCount] = new VisualizerInfo()
				{
					Rect = rect,
					Color = color,
					TextPosition = position
				};
				RoomCount++;
			}
			DrawQueue.RemoveAt(0);
        }
	}

	private void OnTimeOut() => QueueRedraw();

	private void OnNewSplit(List<Rect2> splits) => DrawQueue.Add(splits);

	private struct VisualizerInfo
    {
        public Rect2 Rect { get; set; }
		public Color Color { get; set; }
		public Vector2 TextPosition { get; set; }
    }
}
