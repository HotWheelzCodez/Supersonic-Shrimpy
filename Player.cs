using Godot;
using System;

public partial class Player : CharacterBody2D
{

  [Export]
  public float speed;
  [Export]
  public float acceleration;

  private float _health;
  [Export]
  public float Health {
    get => _health;
    set {
      if (value <= 0) {
        Die();
        _health = 0;
        return;
      }
      _health = value;
    }
  }
  public float invul = 0;

  public Claw[] claws;

  private AnimatedSprite2D sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
    claws = new Claw[] {new Claw(this), new Claw(this)};
    sprite = GetNode<AnimatedSprite2D>("Sprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double doubelta) {
    var delta = (float)doubelta;
    var acc = acceleration;
    invul -= delta;

    var movement = Input.GetVector("move_left", "move_right", "move_up", "move_down");
    if (Health == 0 || invul >= 0) {
      Velocity *= 0.95f;
      MoveAndSlide();
      return;
    }
    if (Velocity.Dot(movement) < 0) { acc *= 2; }
    Velocity = movement * speed;
    if (movement.Length() > 0.1) {
      Rotation = movement.Angle();
    }

    if (movement.LengthSquared() > 0.1) {
      int frame = 0;
      if (movement.Y < 0) {
        frame++;
      }
      if (movement.X < 0.25 && movement.X > -0.25) {
        frame+=2;
      }
      sprite.FlipH = movement.X < 0;
      sprite.Frame = frame;
    }


    ((CollisionShape2D)GetNode("Hitbox")).Rotation = -Rotation * Scale.Y;
    sprite.Rotation = -Rotation * Scale.Y;

    MoveAndSlide();

    if (Input.IsActionJustPressed("punch_left")) {
      claws[0].Punch();

    }
    if (Input.IsActionJustPressed("punch_right")) {
      claws[1].Punch();
    }
	}

  public override void _Process(double delta) {
    foreach(var claw in claws) {
      claw.Update((float)delta);
    }

    GetNode<Node2D>(Scale.Y < 0 ? "FrontClaw" : "BackClaw").Position = new(12 * claws[0].punch, 0);
    GetNode<Node2D>(Scale.Y < 0 ? "BackClaw" : "FrontClaw").Position = new(12 * claws[1].punch, 0);
    GetNode<Node2D>("FrontClaw").Position -= new Vector2(1, 0);
  }

  public bool Hit(Enemy source) {
    if (invul > 0) return false;
    Health -= source.damage;
    Velocity -= (source.Position - Position).Normalized() * 128;
    invul = 0.25f;
    return true;
  }

  public void Die() {
    Modulate = Colors.Red;

  }
}
