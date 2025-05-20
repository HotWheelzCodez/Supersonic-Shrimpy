using Godot;
using System;
using System.Collections.Generic;

public partial class Map : Control
{

	public List<Room> rooms;
	private int nx, ny, mx, my;

	[Export]
	public StyleBox unknown;
	[Export]
	public StyleBox known;
	[Export]
	public StyleBox current;
	[Export]
	public Texture2D doorH;
	[Export]
	public Texture2D doorV;
	[Export]
	public Texture2D boss;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void GenMap(List<Room> rooms) {
		this.rooms = rooms;
		nx = rooms[0].RoomPosition.X;
		ny = rooms[0].RoomPosition.Y;
		mx = rooms[0].RoomPosition.X + rooms[0].RoomSize.X;
		my = rooms[0].RoomPosition.Y + rooms[0].RoomSize.Y;
		foreach (var room in rooms) {
			nx = Math.Min(nx, room.RoomPosition.X);
			ny = Math.Min(ny, room.RoomPosition.Y);
			mx = Math.Max(mx, room.RoomPosition.X + room.RoomSize.X);
			my = Math.Max(my, room.RoomPosition.Y + room.RoomSize.Y);
		}
		//CustomMinimumSize = new Vector2((mx - nx) * 2, (my - ny) * 2);
	}

	public override void _Process(double delta) {
		QueueRedraw();
	}

	public override void _Draw() {
		if (rooms == null) return;
		foreach (var room in rooms) {
			//DrawRect(new Rect2((room.RoomPosition - new Vector2(nx, ny)) * 2, room.RoomSize * 2), room.active ? Colors.Green : room.visited ? Colors.Cyan : Colors.White, room.visited);
			var pos = (room.RoomPosition - new Vector2(nx, ny)) * 7;
			DrawStyleBox(room.active ? current : room.visited ? known : unknown, new Rect2(pos, room.RoomSize * 7 + Vector2.One));
			for (int i = 0; i < room.doors.Count; i++) {
				if (!room.doors[i]) continue;
				(var side, var index) = room.GetDoorSideIndex(i);
				var vert = side == Side.Top || side == Side.Bottom;
				DrawTexture(vert ? doorV : doorH, (room.GetDoorRoomPos(side, index) - new Vector2(nx, ny)) * 7 + (vert ? new Vector2(2, -1) : new Vector2(-1, 2)));
			}
			if (room.boss != null) {
				DrawTexture(boss, (room.RoomPosition + room.RoomSize.Float()/2f - new Vector2(nx, ny)) * 7 - new Vector2(3.5f, 3.5f));
			}
		}
	}
}
