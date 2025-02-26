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

	public Game game;

	public float Left => GlobalPosition.X;
	public float Right => GlobalPosition.X + Size.X * 16;
	public float Top => GlobalPosition.Y;
	public float Bottom => GlobalPosition.Y + Size.Y * 16;

	public Rect2 Rect => new Rect2(GlobalPosition, Size * 16);

	public override void _Ready() {
		game = (Game)GetParent();
		ProcessMode = ProcessModeEnum.Disabled;
		foreach (var child in GetChildren()) {
			if (child is Enemy enemy) {
				enemy.player = game.player;
			}
		}
	}

	public override void _Process(double delta) {
		if (Engine.IsEditorHint()) {
			QueueRedraw();
		}
	}

	public override void _Draw() {
		if (Engine.IsEditorHint()) {
			DrawRect(new Rect2(0, 0, Size * 16), Colors.Cyan, filled: false);
		}
	}

	public void Start() {
		ProcessMode = ProcessModeEnum.Inherit;
	}

	public void Stop() {
		ProcessMode = ProcessModeEnum.Disabled;

	}
}
