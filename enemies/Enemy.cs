using Godot;
using System;

public partial class Enemy : CharacterBody2D, IHittable, IDamageSource {

	[Export]
	public Player player;

	[Export]
	public float speed;
	[Export]
	public float acceleration;
	[Export]
	public float Damage {get; set;}
	[Export]
	public float knockbackStrength;
	[Export]
	public int value;
	[Export]
	public int score;

	public Vector2 Direction => (player.GlobalPosition - GlobalPosition).Normalized();
	public Vector2 Knockback => Direction * knockbackStrength;

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

	public event EventHandler OnHit;

	public static PackedScene coinPickup;
	public static PackedScene healthPickup;

	public override void _Ready() {
		coinPickup ??= ResourceLoader.Load<PackedScene>("res://items/coin.tscn");
		healthPickup ??= ResourceLoader.Load<PackedScene>("res://items/health.tscn");
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
		//DrawLine(Vector2.Zero, lastSeen - GlobalPosition, playerVisible ? Colors.Green : Colors.Red);
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

	public virtual bool Hit(IDamageSource source) {
		if (Health <= 0) return false;
		OnHit?.Invoke(this, EventArgs.Empty);
		anim?.Start("hurt");
		hurtSound?.Play();
		Health -= source.Damage;
		Velocity = source.Knockback;
		return true;
	}

	// Override per enemy class to destroy the enemy object
	public virtual void Die() {
		anim?.Travel("die");
		hurtSound?.Stop();
		dieSound?.Play();
	}

	public virtual void FinishDeath() {
		for (int i = 0; i < value; i++) {
			var node = (Pickup)(player.Health < player.maxHealth && GD.Randf() > 0.8f ? healthPickup : coinPickup).Instantiate();
			node.Position = Position;
			node.Velocity = Vector2.FromAngle(GD.Randf() * Mathf.Pi * 2) * 64 + Velocity;
			AddSibling(node);
		}
		Game.instance.Score += score;
		QueueFree();
	}
}
