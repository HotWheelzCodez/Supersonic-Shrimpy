using Godot;
using System;

public partial class Spinefish : Enemy
{

	[Export]
	private PackedScene projectile;

	[Export]
	public float approachRange;
	[Export]
	public float retreatRange;
	[Export]
	public float shootSpeed;

	private AnimatedSprite2D sprite;

	public override void _Ready() {
		base._Ready();
		sprite = GetNode<AnimatedSprite2D>("Sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		var dist = Position.DistanceTo(lastSeen);


		if (anim.GetCurrentNode() == "walk") {
			if (playerVisible && dist <= approachRange) {
				if (dist >= retreatRange) {
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

		MoveAndSlide();

	}

	public void Shoot() {
		var proj = (FishSpine)projectile.Instantiate();
		proj.velocity = (player.Position - Position).Normalized() * shootSpeed;
		proj.Rotation = (GetAngleTo(player.Position));
		proj.Position = Position;
		proj.owner = this;
		GetParent().AddChild(proj);
	}
}
