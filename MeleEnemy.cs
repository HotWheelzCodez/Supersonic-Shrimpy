using Godot;
using System;

public partial class MeleEnemy : Enemy
{
	public override void _PhysicsProcess(double doubelta)
	{
		var delta = (float)doubelta;
		var acc = acceleration;
	
		if (player == null) {
		  Modulate = Colors.Yellow;
		  GD.PrintErr("Enemy Error: Player is null!");
		}
	
		Velocity = Velocity.MoveToward(hitDelay <= 0 ? (player.Position - Position).Normalized() * speed : Vector2.Zero, speed * acc * delta);
	
		if (Velocity.Dot(Vector2.Right) > 0) {
		  Scale = new Vector2(1, 1);
		} else if (Velocity.Dot(Vector2.Left) > 0) {
		  Scale = new Vector2(1, 1);
		}
	
		MoveAndSlide();
	
		if (hitDelay >= 0) {
		  hitDelay = Mathf.Max(hitDelay - delta, 0);
		  if (hitDelay <= 0 && player.Hit(this)) {
			hitDelay = 0.25f;
		  }
		}
	}    
}
