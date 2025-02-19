using Godot;
using System;

public partial class Player : CharacterBody2D
{

  [Export]
  public float speed;
  [Export]
  public float acceleration;

  public Claw[] claws;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
    claws = new Claw[] {new Claw(this), new Claw(this)};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double doubelta) {
    var delta = (float)doubelta;
    var acc = acceleration;

    var movement = Input.GetVector("move_left", "move_right", "move_up", "move_down");
    if (Velocity.Dot(movement) < 0) { acc *= 2; }
    Velocity = Velocity.MoveToward(movement * speed, delta * acc * speed);
    if (movement.Length() > 0.1) {
      Rotation = movement.Angle();
    }

    if (movement.Dot(Vector2.Right) > 0) {
      Scale = new Vector2(1, 1);
    } else if (movement.Dot(Vector2.Left) > 0) {
      Scale = new Vector2(1, -1);
    }
    ((CollisionShape2D)GetNode("Hitbox")).Rotation = -Rotation * Scale.Y;
    ((CollisionShape2D)GetNode("Attacks").GetNode("Punchbox")).Rotation = -Rotation * Scale.Y;

    MoveAndSlide();

    if (Input.IsActionJustPressed("punch_left")) {
      claws[0].Punch();
      GD.Print("punch_left");
    }
    if (Input.IsActionJustPressed("punch_right")) {
      claws[1].Punch();
      GD.Print("punch_right");
    }
	}

  public override void _Process(double delta) {
    foreach(var claw in claws) {
      claw.Update((float)delta);
    }

    ((Node2D)GetNode(Scale.Y < 0 ? "FrontClaw" : "BackClaw")).Position = new(12 * claws[0].punch, 0);
    ((Node2D)GetNode(Scale.Y < 0 ? "BackClaw" : "FrontClaw")).Position = new(12 * claws[1].punch, 0);
    ((Node2D)GetNode("FrontClaw")).Position -= new Vector2(1, 0);
  }

  public void Die() {
    Modulate = Colors.Red;

  }
}
