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

  public AnimationNodeStateMachine anim;

  public override void _Ready() {
  }

  public override void _PhysicsProcess(double doubelta) {  }

  public virtual void Hit(Claw source) {
	if (hitDelay > -1) {
	  hitDelay = 0.25f;
	}
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
