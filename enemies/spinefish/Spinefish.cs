using Godot;
using System;

public partial class Spinefish : Enemy
{

	private AnimatedSprite2D sprite;

	public override void _Ready() {
		base._Ready();
		sprite = GetNode<AnimatedSprite2D>("Sprite");
	}

	public override void _PhysicsProcess(double doubelta)
	{
		var delta = (float)doubelta;
		var acc = acceleration;

		if (player == null) {
			Modulate = Colors.Yellow;
			GD.PrintErr("Enemy Error: Player is null!");
		}

		var dist = Position.DistanceTo(player.Position);
		var inRange = dist <= 64;
		GD.Print(dist, inRange);

		Velocity = Velocity.MoveToward(inRange ? Vector2.Zero : (player.Position - Position).Normalized() * speed, speed * acc * delta);

		MoveAndSlide();

	}
}
