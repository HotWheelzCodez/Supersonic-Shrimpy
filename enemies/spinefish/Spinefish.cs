using Godot;
using System;

public partial class Spinefish : Enemy
{
	public override void _PhysicsProcess(double doubelta)
	{
		var delta = (float)doubelta;
		var acc = acceleration;

		if (player == null) {
			Modulate = Colors.Yellow;
			GD.PrintErr("Enemy Error: Player is null!");
		}

		var inRange = Position.DistanceTo(player.Position) <= 64;

		Velocity = Velocity.MoveToward(inRange ? Vector2.Zero : (player.Position - Position).Normalized() * speed, speed * acc * delta);

		MoveAndSlide();

	}
}
