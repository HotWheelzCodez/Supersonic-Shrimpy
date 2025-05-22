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

	[Node("Sprite")]
	public AnimatedSprite2D sprite;
	[Node("Raycast")]
	public RayCast2D raycast;

	public override void _Ready() {
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		var dist = GlobalPosition.DistanceTo(lastSeen);
		var away = (GlobalPosition - lastSeen).Normalized();
		raycast.TargetPosition = away * 16;

		if (anim.GetCurrentNode() == "walk") {
			if (playerVisible && dist <= approachRange) {
				if (dist >= retreatRange || raycast.IsColliding()) {
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
		var proj = (FishSpine)projectile.Instantiate();
		proj.Velocity = (player.GlobalPosition - GlobalPosition).Normalized() * shootSpeed;
		Velocity -= proj.Velocity / 8;
		proj.Rotation = (GetAngleTo(player.GlobalPosition));
		proj.Position = Position;
		proj.owner = this;
		proj.Damage = Damage;
		proj.knockbackStrength = knockbackStrength;
		GetParent().AddChild(proj);
	}
}
