using Godot;
using System;

[Tool]
public partial class healthIcon : AnimatedSprite2D
{
	public Vector2 velocity;
	[Export]
	public bool falling = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (falling) {
			velocity.Y += (float)delta * 64;
			Position += velocity * (float)delta;
			Rotation += 8 * (float)delta;
		}
	}

	public void Fall() {
		velocity = new(GD.Randf() * 64 - 32, -32);
		falling = true;

	}
	public void Return() {
		falling = false;
		Position = Vector2.Zero;
	}
}
