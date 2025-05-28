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
	private PackedScene wallH;
	private PackedScene wallV;

	private List<Room> rooms = new();
	private Dictionary<Vector2I, Room> occupied = new();
	private Queue<Room> frontier = new();

	private Random random = new Random();

	public RoomManager(string roomsDirectory, int roomCount) {
		doorH = ResourceLoader.Load<PackedScene>("res://rooms/door_h.tscn");
		doorV = ResourceLoader.Load<PackedScene>("res://rooms/door_v.tscn");
		wallH = ResourceLoader.Load<PackedScene>("res://rooms/wall_h.tscn");
		wallV = ResourceLoader.Load<PackedScene>("res://rooms/wall_v.tscn");
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
					room.ProcessMode = Node.ProcessModeEnum.Disabled;
					availableRooms.Add(room);
					GD.Print($"{filePath}, {room}");
				}
			}

			fileName = dir.GetNext();
		}
	}

	public Room AddSpecialRoom(PackedScene scene, Vector2 dir) {
		var node = (Room)scene.Instantiate();
		dir *= 1000000;
		var center = Vector2.Zero;
		var c = 0;
		foreach (var r in rooms) {
			center += r.RoomPosition + r.RoomSize.Float() / 2f;
			c += r.RoomSize.X * r.RoomSize.Y;
		}
		dir += center / c;

		rooms.Sort((a, b) =>
			(int)(
			(a.RoomPosition + a.RoomSize.Float() / 2f - dir).Length() -
			(b.RoomPosition + b.RoomSize.Float() / 2f - dir).Length() )
		);

		foreach (var room in rooms) {
			if (AddRoomAdj(room, node)) return node;
		}
		return null;
	}

	public bool AddRoomAdj(Room room, Room next) {
		GD.Print($"Building off of {room}");
		for (int i = 0; i < room.doors.Count; i++) {
			if (!room.doors[i]) continue;
			(var side, var index) = room.GetDoorSideIndex(i);
			GD.Print($"  Trying {side} door #{index}");
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
					for (int y = pos.Y; y < pos.Y + next.RoomSize.Y; y++) {
						for (int x = pos.X; x < pos.X + next.RoomSize.X; x++) {
							occupied[new (x, y)] = next;
						}
					}
					frontier.Enqueue(next);
					next.RoomPosition = pos;
					rooms.Add(next);
					GD.Print(next);
					return true;
				}
			}
		}
		return false;
	}

	public void Layout(Room origin) {
		GD.Print("Starting layout");

		rooms.Add(origin);

		var oPos = origin.RoomPosition;
		for (int y = oPos.Y; y < oPos.Y + origin.RoomSize.Y; y++) {
			for (int x = oPos.X; x < oPos.X + origin.RoomSize.X; x++) {
				occupied[new (x, y)] = origin;
			}
		}
		frontier.Enqueue(origin);
		var debug = new char[45, 15];

		while (true) {
			if (frontier.Count <= 0) break;
			var room = frontier.Dequeue();
			//GD.Print($"Building off of {room}");
			for (int i = 0; i < room.doors.Count; i++) {
				if (!room.doors[i]) continue;
				(var side, var index) = room.GetDoorSideIndex(i);
				//GD.Print($"  Trying {side} door #{index}");
				availableRooms.Shuffle(random);
				foreach (var next in availableRooms) {
					//GD.Print($"    Trying {next}");
					var doors = next.GetDoorsOnSide(side.Reverse());
					doors.Shuffle(random);
					foreach (var nextIndex in doors) {
						//GD.Print($"      Trying matching door #{nextIndex}");
						var pos = room.RoomPosition + room.GetDoorRoomOffset(side, index) - next.GetDoorRoomOffset(side.Reverse(), nextIndex);
						var fits = true;
						for (int y = pos.Y; y < pos.Y + next.RoomSize.Y; y++) {
							for (int x = pos.X; x < pos.X + next.RoomSize.X; x++) {
								if (occupied.ContainsKey(new (x, y))) {
									//GD.Print($"        Room intersects existing room at ({x}, {y})");
									fits = false;
								}
							}
						}
						if (fits) {
							//GD.Print("        Room fits");
							var nex = (Room)next.Duplicate(8);
							for (int y = pos.Y; y < pos.Y + next.RoomSize.Y; y++) {
								for (int x = pos.X; x < pos.X + next.RoomSize.X; x++) {
									occupied[new (x, y)] = nex;
								}
							}
							frontier.Enqueue(nex);
							nex.RoomPosition = pos;
							rooms.Add(nex);
							nex.depth = room.depth + 1;
							if (rooms.Count > roomCount) {
								GD.Print("-- Reached room count --");
								goto Finished;
							}
							goto AddedRoom;
						}
					}
				}
			AddedRoom:
				continue;
			}
		}
		GD.Print("-- Queue Empty --");

	Finished:

		GD.Print($"Generated {rooms.Count} rooms");

	}

	public List<Room> Finalize(Node parent) {

		foreach (var room in rooms) {

			for (int i = 0; i < room.doors.Count; i++) {
				if (!room.doors[i]) continue;
				var side = room.GetDoorSideIndex(i).Item1;
				var conPos = room.GetConnectingRoomPos(i);
				Room con;
				if (occupied.TryGetValue(conPos, out con)) {
					if (con.HasDoorAtRoomPos(room.GetDoorRoomPos(i), side)) {
						var nnode = (Node2D)((side) switch {
							Side.Top or Side.Bottom => doorH,
							Side.Left or Side.Right => doorV,
						}).Instantiate();
						nnode.Position = room.GetDoorPos(i);
						room.AddChild(nnode);
						continue;
					}
				} else {
				}
				var node = (Node2D)((side) switch {
					Side.Top or Side.Bottom => wallH,
					Side.Left or Side.Right => wallV,
				}).Instantiate();
				node.Position = room.GetDoorPos(i);
				room.AddChild(node);
				room.doors[i] = false;
			}

			parent.AddChild(room);
		}

		GD.Print("finished layout");
		return rooms;
	}

}
