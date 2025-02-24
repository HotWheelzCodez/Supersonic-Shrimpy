using Godot;
using System;

public partial class Enemy : CharacterBody2D {

	[Export]
	public Player player;

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
		if (player == null) {
			Modulate = Colors.Yellow;
			GD.PrintErr("Enemy Error: Player is null!");
		}
	}

	public virtual void Hit(Claw source) {
		if (hitDelay > -1) {
			hitDelay = 0.25f;
		}
		anim?.Travel("hurt");
		Health -= source.stats.damage;
		var dir = source.player.Transform.X;
		Velocity = (Velocity - source.player.Velocity).Slide(dir) + dir * source.stats.knockback + source.player.Velocity;
	}

	// Override per enemy class to destroy the enemy object
	public virtual void Die() {
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
