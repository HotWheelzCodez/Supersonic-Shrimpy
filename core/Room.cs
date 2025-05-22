using Godot;
using Godot.Collections;
using System.Collections.Generic;

[Tool]
public partial class Room : Node2D {

	public static GradientTexture2D lineTexture;

	public bool active = false;
	public bool visited = false;

	[Export]
	public int enemyPoints = 0;

	[Export]
	public Node2D boss;

	private Vector2I roomSize = new Vector2I(1, 1);
	[Export]
	public Vector2I RoomSize {
		get => roomSize;
		set {
			roomSize = value;
			doors.Resize(2 * value.X + 2 * value.Y);
			QueueRedraw();
		}
	}
	public Vector2 PixelSize => roomSize * Game.roomSize;

	public Vector2I RoomPosition {
		get => (Vector2I)((GlobalPosition + Vector2.One) / Game.roomSize).Floor();
		set => GlobalPosition = value * Game.roomSize;
	}

	[Export]
	public Array<bool> doors = new Array<bool>();

	private Line2D line;

	public float Left => GlobalPosition.X;
	public float Right => GlobalPosition.X + PixelSize.X;
	public float Top => GlobalPosition.Y;
	public float Bottom => GlobalPosition.Y + PixelSize.Y;

	public Rect2 Rect => new Rect2(GlobalPosition, PixelSize);

	public override void _Ready() {
		if (Engine.IsEditorHint()) return;
		if (Game.instance.camera.currentRoom != this) {
			Modulate = new Color(0f, 0f, 0f, 0f);
		}
		if (lineTexture == null) {
			lineTexture = new GradientTexture2D();
			GD.Print(lineTexture.GetSize());
			lineTexture.Gradient = new Gradient();
			lineTexture.FillTo = Vector2.Down;
			lineTexture.Gradient.SetColor(1, Game.bg * new Color(1, 1, 1, 0));
			lineTexture.Gradient.SetColor(0, Game.bg);
			lineTexture.Gradient.SetOffset(0, 7/16f);
			lineTexture.Gradient.SetOffset(1, 9/16f);
		}

		line = new Line2D();
		line.Points = new Vector2[] {
			new(0, 0),
			new(PixelSize.X, 0),
			new(PixelSize.X, PixelSize.Y),
			new(0, PixelSize.Y),
			new(0, 0)
		};
		line.TextureMode = Line2D.LineTextureMode.Stretch;
		line.Texture = lineTexture;
		line.Width = 128;
		line.ZIndex = 10;
		line.ZAsRelative = false;
		line.EndCapMode = Line2D.LineCapMode.Box;
		line.BeginCapMode = Line2D.LineCapMode.Box;
		AddChild(line);

		ProcessMode = ProcessModeEnum.Disabled;
		foreach (var child in GetChildren()) {
			if (child is Enemy enemy) {
				enemy.player = Game.instance.player;
			}
		}
	}
	public int GetDoorIndex(Side side, int index = 0) => (side) switch {
		Side.Top => 0,
		Side.Bottom => roomSize.X,
		Side.Left => 2 * roomSize.X,
		Side.Right => 2 * roomSize.X + roomSize.Y
	} + index;

	public (Side, int) GetDoorSideIndex(int index) {
		if (index >= 2 * roomSize.X + roomSize.Y) {
			return (Side.Right, index - 2 * roomSize.X - roomSize.Y);
		}
		if (index >= 2 * roomSize.X) {
			return (Side.Left, index - 2 * roomSize.X);
		}
		if (index >= roomSize.X) {
			return (Side.Bottom, index - roomSize.X);
		}
		return (Side.Top, index);
	}

	public bool HasDoor(Side side, int index = 0) => doors[GetDoorIndex(side, index)];

	public bool HasDoorOnSide(Side side) {
		var count = (side) switch {
			Side.Top or Side.Bottom => roomSize.X,
			Side.Left or Side.Right => roomSize.Y
		};
		for (int i = 0; i < count; i++) {
			if (HasDoor(side, i)) {
				return true;
			}
		}
		return false;
	}

	public List<int> GetDoorsOnSide(Side side) {
		var ret = new List<int>();
		var count = (side) switch {
			Side.Top or Side.Bottom => roomSize.X,
			Side.Left or Side.Right => roomSize.Y
		};
		for (int i = 0; i < count; i++) {
			if (HasDoor(side, i)) {
				ret.Add(i);
			}
		}

		return ret;
	}

	public Vector2 GetDoorPos(Side side, int index = 0) => (side) switch {
		Side.Top => new Vector2(Game.roomSize.X / 2 + Game.roomSize.X * index, 0),
		Side.Bottom => new Vector2(Game.roomSize.X / 2 + Game.roomSize.X * index, PixelSize.Y),
		Side.Left => new Vector2(0, Game.roomSize.Y / 2 + Game.roomSize.Y * index),
		Side.Right => new Vector2(PixelSize.X, Game.roomSize.Y / 2 + Game.roomSize.Y * index),
	};

	public Vector2 GetDoorPos(int index) {
		(var side, var i) = GetDoorSideIndex(index);
		return GetDoorPos(side, i);
	}

	public Vector2I GetDoorRoomOffset(Side side, int index = 0) => (side) switch {
		Side.Top => new Vector2I(index, 0),
		Side.Bottom => new Vector2I(index, RoomSize.Y),
		Side.Left => new Vector2I(0, index),
		Side.Right => new Vector2I(RoomSize.X, index),
	};

	public Vector2I GetDoorRoomPos(Side side, int index = 0) => GetDoorRoomOffset(side, index) + RoomPosition;

	public Vector2I GetDoorRoomPos(int index) {
		(var side, var i) = GetDoorSideIndex(index);
		return GetDoorRoomPos(side, i);
	}

	public Vector2I GetConnectingRoomPos(Side side, int index = 0) => (side) switch {
		Side.Top => GetDoorRoomPos(side, index) + Vector2I.Up,
		Side.Left => GetDoorRoomPos(side, index) + Vector2I.Left,
		_ => GetDoorRoomPos(side, index),
	};

	public Vector2I GetConnectingRoomPos(int index) {
		(var side, var i) = GetDoorSideIndex(index);
		return GetConnectingRoomPos(side, i);
	}

	public bool HasDoorAtRoomPos(Vector2I pos, Side side) => (side) switch {
		Side.Top or Side.Bottom => HasDoor(side.Reverse(), pos.X - RoomPosition.X),
		Side.Left or Side.Right => HasDoor(side.Reverse(), pos.Y - RoomPosition.Y),
	};

	public Vector2I GetDoorRoomOffset(int index) {
		(var side, var i) = GetDoorSideIndex(index);
		return GetDoorRoomOffset(side, i);
	}

	public override void _Process(double delta) {
		if (Engine.IsEditorHint()) {
			QueueRedraw();
		}

	}

	public override void _Draw() {
		if (Engine.IsEditorHint()) {
			DrawRect(new Rect2(0, 0, PixelSize), Colors.Cyan, filled: false);
			for (int i = 0; i < doors.Count; i++) {
				var pos = GetDoorPos(i);
				DrawCircle(pos, 12, doors[i] ? Colors.Green : Colors.Red, false);
				DrawString(ThemeDB.FallbackFont, pos, i.ToString());
			}
		}
	}

	public void Start() {
		ProcessMode = ProcessModeEnum.Inherit;
		active = true;
		visited = true;
		Game.instance.roomDebug.Text = ToString();
		if (boss != null) {
			GD.Print("boss");
			Game.instance.GetNode<AnimationPlayer>("%BossBarAnim").Play("intro");
		}
	}

	public void Stop() {
		ProcessMode = ProcessModeEnum.Disabled;
		active = false;

	}

	public override string ToString() {
		return $"<Room: {GetName()} at {RoomPosition}>";
	}
}
