using Godot;
using System;

public partial class NormalizedCamera : Camera2D
{
	public enum Mode : int {
		Fit = 0,
		Fill = 1,
		Stretch = 2
	}

	[Export]
	public int unitsH = 1920;
	[Export]
	public int unitsV = 1080;
	[Export]
	public Mode mode = Mode.Fit;

	[Export]
	private Room currentRoom;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		TransitionToRoom(currentRoom);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		var size = GetTree().Root.Size;
		var zh = (float)size.X / unitsH;
		var zv = (float)size.Y / unitsV;
		Zoom = (mode) switch {
			Mode.Fill => new(Mathf.Max(zh, zv), Mathf.Max(zh, zv)),
			Mode.Fit => new(Mathf.Min(zh, zv), Mathf.Min(zh, zv)),
			Mode.Stretch => new(zh, zv),
		};
	}

	public void TransitionToRoom(Room room) {
		currentRoom.Stop();
		currentRoom = room;
		var tween = GetTree().CreateTween();
		tween.SetParallel();
		tween.SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(this, "limit_left", room.Left, 1);
		tween.TweenProperty(this, "limit_right", room.Right, 1);
		tween.TweenProperty(this, "limit_top", room.Top, 1);
		tween.TweenProperty(this, "limit_bottom", room.Bottom, 1);
		tween.Chain().TweenCallback(Callable.From(() => room.Start()));
	}
}
