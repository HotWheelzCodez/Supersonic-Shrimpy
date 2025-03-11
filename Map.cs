using Godot;
using System;
using System.Collections.Generic;

public partial class Map : Control
{

	public List<Room> rooms;
	private int nx, ny, mx, my;

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
		CustomMinimumSize = new Vector2((mx - nx) * 2, (my - ny) * 2);
	}

	public override void _Process(double delta) {
		QueueRedraw();
	}

	public override void _Draw() {
		if (rooms == null) return;
		foreach (var room in rooms) {
			DrawRect(new Rect2((room.RoomPosition - new Vector2(nx, ny)) * 2, room.RoomSize * 2), room.active ? Colors.Green : room.visited ? Colors.Cyan : Colors.White, room.visited);
		}
	}
}
