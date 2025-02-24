using Godot;
using System;

public partial class Enemy : CharacterBody2D {

  [Export]
  public Player player;

  [Export]
  public string name;
  [Export]
  public float speed;
  [Export]
  public float acceleration;
  [Export]
  public float damage;
  private float _health;
  [Export]
  public virtual float Health {
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
  public float hitDelay = -1;

  public AnimationNodeStateMachinePlayback anim;

  public override void _Ready() {
    anim = (AnimationNodeStateMachinePlayback)GetNode<AnimationTree>("AnimationTree").Get("parameters/playback");
  }

  public override void _PhysicsProcess(double doubelta) {
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
        anim.Travel("attack");
        hitDelay = 0.25f;
      }
    }
  }

  public virtual void Hit(Claw source) {
    if (hitDelay > -1) {
      hitDelay = 0.25f;
    }
    anim.Travel("hurt");
    Health -= source.stats.damage;
    var dir = source.player.Transform.X;
    GD.Print(Velocity.Slide(dir));
    Velocity = (Velocity - source.player.Velocity).Slide(dir) + dir * source.stats.knockback + source.player.Velocity;
  }

  // Override per enemy class to destroy the enemy object
  public virtual void Die() {
    GD.Print("Enemy "+name+" is dead!");
    QueueFree();
  }

  public void _OnBodyEnter(Node2D body) {
    if (body == player) {
      hitDelay = 0.25f;
    }
  }

  public void _OnBodyExit(Node2D body) {
    if (body == player) {
      hitDelay = -1;
    }
  }
}
