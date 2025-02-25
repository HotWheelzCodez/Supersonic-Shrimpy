using Godot;

[Tool]
public partial class Room : Node2D {

	private Vector2 _size;
	[Export]
	public Vector2 Size {
		get => _size;
		set {
			_size = value;
			QueueRedraw();
		}
	}

	public float Left => Position.X;
	public float Right => Position.X + Size.X * 16;
	public float Top => Position.Y;
	public float Bottom => Position.Y + Size.Y * 16;

	public override void _Ready() {
		ProcessMode = ProcessModeEnum.Disabled;
		foreach (var child in GetChildren()) {
			if (child is Enemy enemy) {
				enemy.player = ((Game)GetParent()).player;
			}
		}
	}

	public override void _Process(double delta) {
		QueueRedraw();
	}

	public override void _Draw() {
		DrawRect(new Rect2(0, 0, Size * 16), Colors.Cyan, filled: false);
	}

	public void Start() {
		ProcessMode = ProcessModeEnum.Inherit;
	}

	public void Stop() {
		ProcessMode = ProcessModeEnum.Disabled;

	}
}
