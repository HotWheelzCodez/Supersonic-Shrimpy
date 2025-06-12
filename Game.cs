using Godot;
using System;

public partial class Game : Node2D
{

	[Export]
	public Player player;
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
	public GameOver gameOver;
	[Node("%Score")]
	public Label goScore;
	[Node("%Loading")]
	public ColorRect loading;
	[Node("%BossBar")]
	public ProgressBar bossBar;
	[Node("%Health")]
	public HBoxContainer healthBar;
	[Node("%Soul")]
	public HBoxContainer soulBar;

	public static readonly Color bg = new Color(0, 0.05f, 0.1f);

	public Room bossRoom;

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
		bossRoom = roomManager.AddSpecialRoom(GD.Load<PackedScene>("rooms/reef/special/treasure.tscn"), new(0, -1));
		roomManager.AddRoomAdj(bossRoom, GD.Load<PackedScene>("rooms/reef/special/end.tscn").Instantiate<Room>());
		map.GenMap(roomManager.Finalize(this));

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		var boss = (Boss)camera.currentRoom.boss;
		if (boss != null) {
			bossBar.MaxValue = boss.maxHealth;
			bossBar.Value = boss.health;
		}
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
				//heart.GetNode<Node2D>("Slot/Icon").Scale = Vector2.One * Mathf.Min(1, player.Health - i);
			}
			i++;
		}

		i = (int)Math.Ceiling(Mathf.Max(player.soul, (float)soulBar.GetChildCount()));
		while (i --> 0) {
			HealthIndicator node;
			if (i >= soulBar.GetChildCount()) {
				node = ResourceLoader.Load<PackedScene>("res://ui/soulIndicator.tscn").Instantiate<HealthIndicator>();
				soulBar.AddChild(node);
				node.sprite.Scale = Vector2.Zero;
			} else {
				node = soulBar.GetChild<HealthIndicator>(i);
			}
			node.sprite.Scale = node.sprite.Scale.Lerp(Vector2.One * Mathf.Clamp(player.soul - i, 0, 1), 0.1f);
			if (player.soul <= i && node.sprite.Scale.LengthSquared() < 0.01f) {
				node.QueueFree();
			}
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
		goScore.Text = Score.ToString();
		if (loading.Visible) {
			GetTree().ChangeSceneToFile("res://Main.tscn");
		}

		if (Input.IsActionJustPressed("debug_boss")) {
			player.GlobalPosition = bossRoom.GlobalPosition + bossRoom.PixelSize / 2;
		}
		if (Input.IsActionJustPressed("debug_kill_boss")) {
			if (camera.currentRoom.boss is Boss bos) {
				bos.Damage(bos.health);
			}
		}
		if (Input.IsActionJustPressed("ui.cancel")) {
			GetTree().Quit();
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
