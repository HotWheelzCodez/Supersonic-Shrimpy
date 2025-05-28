using Godot;
using System;

public partial class Snipefish : Enemy
{

	[Export]
	public float approachRange;
	[Export]
	public float retreatRange;

	[Export]
	public bool aiming;

	[Node("Sprite")]
	public AnimatedSprite2D sprite;
	[Node("Area2D")]
	public Area2D raycast;
	[Node("Line2D")]
	public Line2D line;

	public override void _Ready() {
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		lastSeen = player.GlobalPosition;
		var dist = GlobalPosition.DistanceTo(lastSeen);
		if (aiming) {
			raycast.Rotation = (lastSeen - GlobalPosition).Angle();
			line.SetPointPosition(1, 1000 * (lastSeen - GlobalPosition));
		}

		if (anim.GetCurrentNode() == "walk") {
			if (dist <= approachRange) {
				if (dist >= retreatRange || !playerVisible) {
					anim.Travel("shoot");
				} else {
					MoveAway(player.Position, delta);
				}
			} else {
				MoveTo(lastSeen, delta);
			}
		} else {
			AccelerateTo(Vector2.Zero, delta);
		}

		if (Mathf.Abs(Velocity.X) > speed / 8) {
			sprite.FlipH = Velocity.X > 0;
		}

		if (MoveAndSlide() && anim.GetCurrentNode() == "hurt") {
			Velocity += GetSlideCollision(0).GetNormal() * speed * 8;
		}

	}

	public void Shoot() {
		Game.instance.Shake(1, 5);
		if (raycast.OverlapsBody(player)) {
			player.Hit(this);
		}
	}
}

