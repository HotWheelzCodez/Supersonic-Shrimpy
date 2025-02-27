using Godot;

[Tool]
public partial class Room : Node2D {

	public static GradientTexture2D lineTexture;

	private Vector2 _size;
	[Export]
	public Vector2 Size {
		get => _size;
		set {
			_size = value;
			QueueRedraw();
		}
	}

	private Line2D line;

	public float Left => GlobalPosition.X;
	public float Right => GlobalPosition.X + Size.X;
	public float Top => GlobalPosition.Y;
	public float Bottom => GlobalPosition.Y + Size.Y;

	public Rect2 Rect => new Rect2(GlobalPosition, Size);

	public override void _Ready() {
		if (Engine.IsEditorHint()) return;
		if (lineTexture == null) {
			lineTexture = new GradientTexture2D();
			GD.Print(lineTexture.GetSize());
			lineTexture.Gradient = new Gradient();
			lineTexture.FillTo = Vector2.Down;
			lineTexture.Gradient.SetColor(1, new Color(0, 0, 0, 0));
		}

		line = new Line2D();
		line.Points = new Vector2[] {
			new(0, 0),
			new(Size.X, 0),
			new(Size.X, Size.Y),
			new(0, Size.Y),
			new(0, 0)
		};
		line.TextureMode = Line2D.LineTextureMode.Stretch;
		line.Texture = lineTexture;
		line.Width = 16;
		AddChild(line);

		ProcessMode = ProcessModeEnum.Disabled;
		foreach (var child in GetChildren()) {
			if (child is Enemy enemy) {
				enemy.player = Game.instance.player;
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
			DrawRect(new Rect2(0, 0, Size), Colors.Cyan, filled: false);
		}
	}

	public void Start() {
		ProcessMode = ProcessModeEnum.Inherit;
		var tween = GetTree().CreateTween().SetParallel();
	}

	public void Stop() {
		ProcessMode = ProcessModeEnum.Disabled;
		var tween = GetTree().CreateTween().SetParallel();
		tween.TweenProperty(this, "modulate", Colors.Transparent, 0.5);

	}
}
