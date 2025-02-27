using Godot;
using System;

public partial class Segment : AnimatedSprite2D
{
	[Export]
	public float distance = 10;

	public Vector2 pos = Vector2.Zero;
	[Node("..")]
	public Node2D parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		var ppos = parent.GlobalPosition;
		var angle = ppos - pos;
		pos = ppos + (pos - ppos).Normalized() * distance;
		GlobalPosition = pos;

		float targangle;
		if (!(parent is Segment)) {
			targangle = ((Player)parent.GetParent()).facingAngle;
		} else {
			targangle = parent.GetAngleTo(((Node2D)parent.GetParent()).GlobalPosition);
		}
		if (Mathf.Abs(Mathf.AngleDifference(angle.Angle(), targangle)) > Mathf.Pi / 4) {
			pos -= Vector2.FromAngle(targangle) * (float)delta * 60;
		}

		if (Mathf.Abs(angle.X) > Mathf.Abs(angle.Y)) {
			Frame = 0;
			FlipH = angle.X > 0;
		} else if (angle.Y > 0) {
			Frame = 2;
			FlipH = false;
		} else {
			Frame = 1;
			FlipH = false;
		}
	}
}
