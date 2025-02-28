using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class RoomManager { 
  private List<PackedScene> availableRooms = new List<PackedScene>();
  private int roomCount = 10;

  public RoomManager(string roomsDirectory, int roomCount) {
    this.roomCount = roomCount;

    string path = roomsDirectory;
    var dir = DirAccess.Open(path);
    if (dir == null) {
      GD.PrintErr($"Failed to open directory: {path}");
      return;
    }

    dir.ListDirBegin();
    string fileName = dir.GetNext();

    while (!string.IsNullOrEmpty(fileName)) {
      if (!dir.CurrentIsDir() && (fileName.EndsWith(".tscn") || fileName.EndsWith(".scn"))) {
        string filePath = $"{path}/{fileName}";
        var scene = ResourceLoader.Load<PackedScene>(filePath);
        if (scene != null) {
          availableRooms.Add(scene);
        }
      }

      fileName = dir.GetNext();
    }
  }

  public void Layout(Node parent) {
    Vector2 positionOffset = Vector2.Zero;
    Random random = new Random();
    List<Room> rooms = new List<Room>();

    for (int i = 0; i < roomCount; i++) {
      Room room = (Room)availableRooms[random.Next(0, availableRooms.Count)].Instantiate();
      room.GlobalPosition = positionOffset;
      rooms.Add(room);
      
      positionOffset += new Vector2(room.Size.X, 0);
    }

    // TODO: Add controlled randamization to the room generation
    foreach (Room room in rooms) {
      parent.AddChild(room);
    }
  }
}
