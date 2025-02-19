using Godot;
using System;

public partial class Enemy : CharacterBody2D {

  [Export]
  private Player player;

  [Export]
  protected string name;
  [Export]
  protected float speed;
  [Export]
  protected float acceleration;
  [Export]
  protected float damage;
  [Export]
  protected float Health { get; set; }

  public override void _Ready() {

  } 

  public override void _PhysicsProcess(double doubelta) {
    var delta = (float)doubelta;
    var acc = acceleration;

    if (player == null) {
      Modulate = Colors.Yellow;
      GD.PrintErr("Enemy Error: Player is null!");
    }

    Velocity = Velocity.MoveToward(player.Position, speed * acc * delta);
    if (Velocity.Length() > 0.1) {
      Rotation = Velocity.Angle(); 
    }

    if (Velocity.Dot(Vector2.Right) > 0) {
      Scale = new Vector2(1, 1);
    } else if (Velocity.Dot(Vector2.Left) > 0) {
      Scale = new Vector2(1, -1);
    }
    ((CollisionShape2D)GetNode("Hitbox")).Rotation = -Rotation;
    ((CollisionShape2D)GetNode("Attack")).Rotation = -Rotation;

    MoveAndSlide(); 
  }

  // Override per enemy class to destroy the enemy object
  public virtual void Die() { 
    Modulate = Colors.Red;
    GD.Print("Enemy "+name+" is dead!");
  }
}
