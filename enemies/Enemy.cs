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
	public bool playerVisible;
	public Vector2 lastSeen;

	public AnimationNodeStateMachinePlayback anim;
	public AnimationPlayer animPlayer;

	public override void _Ready() {
		anim = (AnimationNodeStateMachinePlayback)GetNode<AnimationTree>("AnimationTree").Get("parameters/playback");
		animPlayer = (AnimationPlayer)GetNode<AnimationPlayer>("AnimationPlayer");
		lastSeen = Position;
	}

	private bool IsPlayerVisible() {
		var res = GetWorld2D().DirectSpaceState.IntersectRay(PhysicsRayQueryParameters2D.Create(Position, player.Position, 1));
		return res.Count == 0;
	}

	public override void _PhysicsProcess(double doubelta) {
		if (player == null) {
			Modulate = Colors.Yellow;
			GD.PrintErr("Enemy Error: Player is null!");
		}
		playerVisible = IsPlayerVisible();
		if (playerVisible) {
			lastSeen = player.Position;
		}
		QueueRedraw();
	}

	public override void _Draw() {
		DrawLine(Vector2.Zero, lastSeen - Position, playerVisible ? Colors.Green : Colors.Red);
	}

	public void MoveAway(Vector2 target, AutoFloat delta) {
		AccelerateTo((Position - target).Normalized() * speed, delta);
	}

	public void MoveTo(Vector2 target, AutoFloat delta) {
		AccelerateTo((target - Position).Normalized() * speed, delta);
	}

	public void AccelerateTo(Vector2 target, AutoFloat delta) {
		Velocity = Velocity.MoveToward(target, speed * acceleration * delta);
	}

	public virtual void Hit(Claw source) {
		if (hitDelay > -1) {
			hitDelay = 0.25f;
		}
		anim?.Start("hurt");
		Health -= source.stats.damage;
		var dir = source.player.Transform.X;
		Velocity = (Velocity - source.player.Velocity).Slide(dir) + dir * source.stats.knockback + source.player.Velocity;
	}

	// Override per enemy class to destroy the enemy object
	public virtual void Die() {
		anim?.Start("die");
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
