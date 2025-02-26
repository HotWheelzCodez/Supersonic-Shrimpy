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
	public Room currentRoom;

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
		GlobalPosition = Game.instance.player.GlobalPosition;
	}

	public void TransitionToRoom(Room room) {
		currentRoom?.Stop();
		currentRoom = room;
		Game.instance.player.ProcessMode = ProcessModeEnum.Disabled;
		var tween = GetTree().CreateTween().SetTrans(Tween.TransitionType.Quad);
		tween.SetParallel();
		tween.SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(this, "limit_left", room.Left, 0.5);
		tween.TweenProperty(this, "limit_right", room.Right, 0.5);
		tween.TweenProperty(this, "limit_top", room.Top, 0.5);
		tween.TweenProperty(this, "limit_bottom", room.Bottom, 0.5);
		var finish = tween.Chain();
		finish.TweenCallback(Callable.From(() => room.Start()));
		finish.TweenCallback(Callable.From(delegate {Game.instance.player.ProcessMode = ProcessModeEnum.Inherit;}));
	}
}
