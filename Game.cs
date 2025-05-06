using Godot;
using System;

public partial class Game : Node2D
{

	[Export]
	public Player player;
	[Export]
	public HBoxContainer healthBar;
	[Export]
	public Label moneyLabel;
	[Export]
	public Label scoreLabel;
	[Export]
	public NormalizedCamera camera;
	[Export(PropertyHint.Dir)]
	public string roomsDirectory;
	[Export]
	public int roomCount;
	[Export]
	public Room startingRoom;

	private int _money;
	public int Money {
		get => _money;
		set {
			moneyLabel.Text = value.ToString();
			_money = value;
		}
	}
	private int _score;
	public int Score;

	public static Game instance;

	public static readonly Vector2 roomSize = new (256, 144);

	[Node("Timer")]
	public Timer timer;
	[Node("%Shockwave")]
	public Sprite2D shockwaveNode;
	public ShaderMaterial shockwaveShader;
	private int nextShockwave;
	private Vector3[] shockwaves = new Vector3[16];
	[Node("%Map")]
	public Map map;
	[Node("%RoomDebug")]
	public Label roomDebug;
	[Node("%PosDebug")]
	public Label posDebug;
	[Node("%GameOver")]
	public ColorRect gameOver;
	[Node("%Score")]
	public Label goScore;
	[Node("%Loading")]
	public ColorRect loading;

	public static readonly Color bg = new Color(0, 0.05f, 0.1f);

	public override void _EnterTree() {
		instance = this;
		NodeAttribute.StartTreeListener(GetTree().Root);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		timer.Timeout += _OnTimerTimeout;
		shockwaveShader = (ShaderMaterial)shockwaveNode.Material;

		RoomManager roomManager = new RoomManager(roomsDirectory, roomCount);
		roomManager.Layout(startingRoom);
		var room = roomManager.AddSpecialRoom(GD.Load<PackedScene>("rooms/reef/special/treasure.tscn"), new(0, -1));
		map.GenMap(roomManager.Finalize(this));
		GD.Print(room?.RoomPosition);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		for (int j = 0; j < shockwaves.Length; j++) {
			shockwaves[j].Z += (float)delta;
		}
		shockwaveShader.SetShaderParameter("centers", shockwaves);
		int i = Mathf.CeilToInt(player.maxHealth) - healthBar.GetChildren().Count;
		for (; i --> 0 ;) {
			healthBar.AddChild(ResourceLoader.Load<PackedScene>("res://ui/healthIndicator.tscn").Instantiate());
		}
		i = 0;
		foreach (Node heart in healthBar.GetChildren()) {
			if (i >= player.Health) {
				((HealthIndicator)heart).SetState("lose");
			} else {
				((HealthIndicator)heart).SetState("normal");
			}
			i++;
		}
		foreach (var child in GetChildren()) {
			if (child is Room room && room != camera.currentRoom) {
				if (room.Rect.HasPoint(player.Position)) {
					camera.TransitionToRoom(room);
				}
			}
		}
		_score = (int)Mathf.MoveToward(_score, Score, (float)delta * 1000);
		scoreLabel.Text = _score.ToString();
		posDebug.Text = player.GlobalPosition.ToString();
		gameOver.Visible = player.Health <= 0;
		goScore.Text = _score.ToString();
		if (loading.Visible) {
			GetTree().ChangeSceneToFile("res://Main.tscn");
		}
		if (gameOver.Visible) {
			if (Input.IsActionJustPressed("punch_left") || Input.IsActionJustPressed("punch_right")) {
				loading.Visible = true;
			}
		}

	}

	private void _OnTimerTimeout() {
		ProcessMode = ProcessModeEnum.Inherit;
	}

	public void Shockwave(Vector2 pos) {
		shockwaves[nextShockwave] = new Vector3(pos.X, pos.Y, 0);
		nextShockwave++;
		nextShockwave %= shockwaves.Length;
	}

	public void Freeze(float secs) {
		timer.WaitTime = secs;
		timer.Start();
		ProcessMode = ProcessModeEnum.Disabled;
	}

	public void Shake(float intensity, int count) {
		var tween = GetTree().CreateTween();
		for (int i = 0; i < count; i++) {
			tween.TweenProperty(camera, "offset", new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1) * intensity, 0).SetDelay(0.02);
		}
		tween.TweenProperty(camera, "offset", Vector2.Zero, 0);
	}
}
