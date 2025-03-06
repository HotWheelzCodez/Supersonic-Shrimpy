using Godot;
using System;
using System.Collections.Generic;
using System.Text;

[Tool]
public partial class RoomManager {

	public static string chars = "*|-/\\XO@";

	private List<Room> availableRooms = new List<Room>();
	private int roomCount = 10;

	private PackedScene doorH;
	private PackedScene doorV;

	public RoomManager(string roomsDirectory, int roomCount) {
		doorH = ResourceLoader.Load<PackedScene>("res://rooms/door_h.tscn");
		doorV = ResourceLoader.Load<PackedScene>("res://rooms/door_v.tscn");
		this.roomCount = roomCount;

		string path = roomsDirectory;
		var dir = DirAccess.Open(path);
		if (dir == null) {
			GD.PrintErr($"Failed to open directory: {path}");
			return;
		}

		dir.ListDirBegin();
		string fileName = dir.GetNext();

		GD.Print("Available Rooms:");
		while (!string.IsNullOrEmpty(fileName)) {
			if (!dir.CurrentIsDir() && (fileName.EndsWith(".tscn") || fileName.EndsWith(".scn"))) {
				string filePath = $"{path}/{fileName}";
				var scene = ResourceLoader.Load<PackedScene>(filePath);
				if (scene != null) {
					var room = (Room)scene.Instantiate();
					availableRooms.Add(room);
					GD.Print($"{filePath}, {room}");
				}
			}

			fileName = dir.GetNext();
		}
	}

	public List<Room> Layout(Room origin) {
		var random = new Random();
		var rooms = new List<Room>();
		var occupied = new Dictionary<Vector2I, Room>();
		var frontier = new Queue<Room>();
		var parent = origin.GetParent();

		var oPos = origin.RoomPosition;
		for (int y = oPos.Y; y < oPos.Y + origin.RoomSize.Y; y++) {
			for (int x = oPos.X; x < oPos.X + origin.RoomSize.X; x++) {
				occupied[new (x, y)] = origin;
				frontier.Enqueue(origin);
			}
		}
		var debug = new char[45, 15];

		while (true) {
			if (frontier.Count <= 0) break;
			var room = frontier.Dequeue();
			GD.Print($"Building off of {room}");
			for (int i = 0; i < room.doors.Count; i++) {
				if (!room.doors[i]) continue;
				(var side, var index) = room.GetDoorSideIndex(i);
				GD.Print($"  Trying {side} door #{index}");
				availableRooms.Shuffle(random);
				foreach (var next in availableRooms) {
					GD.Print($"    Trying {next}");
					var doors = next.GetDoorsOnSide(side.Reverse());
					doors.Shuffle(random);
					foreach (var nextIndex in doors) {
						GD.Print($"      Trying matching door #{nextIndex}");
						var pos = room.RoomPosition + room.GetDoorRoomOffset(side, index) - next.GetDoorRoomOffset(side.Reverse(), nextIndex);
						var fits = true;
						for (int y = pos.Y; y < pos.Y + next.RoomSize.Y; y++) {
							for (int x = pos.X; x < pos.X + next.RoomSize.X; x++) {
								if (occupied.ContainsKey(new (x, y))) {
									GD.Print($"        Room intersects existing room at ({x}, {y})");
									fits = false;
								}
							}
						}
						if (fits) {
							GD.Print("        Room fits");
							var nex = (Room)next.Duplicate(8);
							for (int y = pos.Y; y < pos.Y + next.RoomSize.Y; y++) {
								for (int x = pos.X; x < pos.X + next.RoomSize.X; x++) {
									occupied[new (x, y)] = nex;
								}
							}
							frontier.Enqueue(nex);
							nex.RoomPosition = pos;
							rooms.Add(nex);
							if (rooms.Count > roomCount) {
								GD.Print("-- Reached room count --");
								goto Finished;
							}
							goto AddedRoom;
						}
					}
				}
				AddedRoom:
				for (int y = -7; y < 8; y++) {
					var sb = new StringBuilder();
					for (int x = -7; x < 8; x++) {
						char c = occupied.ContainsKey(new (x, y)) ? chars[(rooms.IndexOf(occupied[new (x, y)]) + chars.Length) % chars.Length] : ' ';
						sb.Append(c);
						sb.Append(c);
						sb.Append(c);
					}
					//GD.Print(sb);
				}
			}
		}
		GD.Print("-- Queue Empty --");

	Finished:

		GD.Print($"Generated {rooms.Count} rooms");

		// TODO: Add controlled randamization to the room generation
		foreach (var room in rooms) {

			for (int i = 0; i < room.doors.Count; i++) {
				if (!room.doors[i]) continue;
				var side = room.GetDoorSideIndex(i).Item1;
				var conPos = room.GetConnectingRoomPos(i);
				Room con;
				if (occupied.TryGetValue(conPos, out con)) {
					if (con.HasDoorAtRoomPos(room.GetDoorRoomPos(i), side)) {
						continue;
					}
				} else {
				}
				var node = (Node2D)((side) switch {
					Side.Top or Side.Bottom => doorH,
					Side.Left or Side.Right => doorV,
				}).Instantiate();
				node.Position = room.GetDoorPos(i);
				room.AddChild(node);
			}

			parent.AddChild(room);
		}
		return rooms;
	}

}
