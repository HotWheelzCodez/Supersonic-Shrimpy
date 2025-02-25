using Godot;
using System;

public partial class FishSpine : Area2D
{
	public Enemy owner;
	public Vector2 velocity;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Position += velocity * (float)delta;
	}

	public void _OnBodyEnter(Node node) {
		if (node is Player player) {
			player.Hit(owner);
		}
		QueueFree();
	}
}
