using Godot;
using System;

public partial class Player : CharacterBody2D
{

  [Export]
  public float speed;
  [Export]
  public float acceleration;

  public Claw[] claws = new Claw[2];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double doubelta)
	{
    var delta = (float)doubelta;
    var acc = acceleration;

    var movement = Input.GetVector("move_left", "move_right", "move_up", "move_down");
    if (Velocity.Dot(movement) < 0) { acc *= 2; }
    Velocity = Velocity.MoveToward(movement * speed, delta * acc * speed);
    System.Console.WriteLine(acc);
    if (movement.Length() > 0.1) {
      Rotation = movement.Angle();
    }

    if (movement.Dot(Vector2.Right) > 0) {
      Scale = new Vector2(1, 1);
    } else if (movement.Dot(Vector2.Left) > 0) {
      Scale = new Vector2(1, -1);
    }
    ((CollisionShape2D)GetNode("Hitbox")).Rotation = -Rotation;
    ((CollisionShape2D)GetNode("Attacks").GetNode("Punchbox")).Rotation = -Rotation;

    GD.Print(Velocity);
    MoveAndSlide();
	}
}
