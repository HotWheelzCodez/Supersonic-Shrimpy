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
	public bool playerVisible;
	public Vector2 lastSeen;

	public AnimationNodeStateMachinePlayback anim;

	public Room room;

	[Node("HurtSound")]
	public AudioStreamPlayer2D hurtSound;
	[Node("DieSound")]
	public AudioStreamPlayer2D dieSound;

	public override void _Ready() {
		anim = (AnimationNodeStateMachinePlayback)GetNode<AnimationTree>("AnimationTree").Get("parameters/playback");
		lastSeen = GlobalPosition;
		var cur = GetParent();
		while (cur != null && !(cur is Room)) {
			cur = cur.GetParent();
		}
		room = (Room)cur;
	}

	private bool IsPlayerVisible() {
		var res = GetWorld2D().DirectSpaceState.IntersectRay(PhysicsRayQueryParameters2D.Create(GlobalPosition, player.GlobalPosition, 1));
		return res.Count == 0;
	}

	public override void _PhysicsProcess(double doubelta) {
		if (player == null) {
			Modulate = Colors.Yellow;
			GD.PrintErr("Enemy Error: Player is null!");
		}
		playerVisible = IsPlayerVisible();
		if (playerVisible) {
			lastSeen = player.GlobalPosition;
		}
		var vel = Velocity;
		if (GlobalPosition.X < room.Left) vel.X = speed;
		if (GlobalPosition.X > room.Right) vel.X = -speed;
		if (GlobalPosition.Y < room.Top) vel.Y = speed;
		if (GlobalPosition.Y > room.Bottom) vel.Y = -speed;
		Velocity = vel;
		QueueRedraw();
	}

	public override void _Draw() {
		DrawLine(Vector2.Zero, lastSeen - GlobalPosition, playerVisible ? Colors.Green : Colors.Red);
	}

	public void MoveAway(Vector2 target, AutoFloat delta) {
		AccelerateTo((GlobalPosition - target).Normalized() * speed, delta);
	}

	public void MoveTo(Vector2 target, AutoFloat delta) {
		AccelerateTo((target - GlobalPosition).Normalized() * speed, delta);
	}

	public void AccelerateTo(Vector2 target, AutoFloat delta) {
		Velocity = Velocity.MoveToward(target, speed * acceleration * delta);
	}

	public virtual void Hit(Claw source) {
		if (Health <= 0) return;
		anim?.Start("hurt");
		hurtSound?.Play();
		Health -= source.stats.damage;
		var dir = source.player.attacks.Transform.X;
		Velocity = (Velocity - source.player.Velocity).Slide(dir) + dir * source.stats.knockback + source.player.Velocity;
	}

	// Override per enemy class to destroy the enemy object
	public virtual void Die() {
		anim?.Travel("die");
		hurtSound?.Stop();
		dieSound?.Play();
	}
}
