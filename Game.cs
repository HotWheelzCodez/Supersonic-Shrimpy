using Godot;
using System;

public partial class Game : Node2D
{

	[Export]
	public Player player;
	[Export]
	public HBoxContainer healthBar;
	[Export]
	public NormalizedCamera camera;
	[Export(PropertyHint.Dir)]
	public string roomsDirectory;
	[Export]
	public int roomCount;
	[Export]
	public Room startingRoom;

	public static Game instance;

	public static readonly Vector2 roomSize = new (256, 144);

	[Node("Timer")]
	public Timer timer;
	[Node("%Shockwave")]
	public Sprite2D shockwaveNode;
	public ShaderMaterial shockwaveShader;
	private int nextShockwave;
	private Vector3[] shockwaves = new Vector3[16];

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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		for (int j = 0; j < shockwaves.Length; j++) {
			shockwaves[j].Z += (float)delta;
		}
		shockwaveShader.SetShaderParameter("centers", shockwaves);
		int i = 0;
		foreach (Node heart in healthBar.GetChildren()) {
			((CanvasItem)heart).Modulate = Colors.White * Mathf.Clamp(player.Health - i, 0, 1);
			i++;
		}
		foreach (var child in GetChildren()) {
			if (child is Room room && room != camera.currentRoom) {
				if (room.Rect.HasPoint(player.Position)) {
					camera.TransitionToRoom(room);
				}
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
