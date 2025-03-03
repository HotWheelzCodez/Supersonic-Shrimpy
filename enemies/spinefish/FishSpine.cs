using Godot;
using System;

public partial class FishSpine : CharacterBody2D, IHittable, IDamageSource
{
	public Enemy owner;
	public float spin;
	public bool Friendly { get; private set; }
	public float knockbackStrength;

	public float Damage {get; set;}
	public Vector2 Knockback => Direction * knockbackStrength;
	public Vector2 Direction => Velocity.Normalized();

	[Node("Area2D")]
	public Area2D area;
	[Node("CollisionShape2D")]
	public CollisionShape2D collider;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Rotation += spin * (float)delta;
		MoveAndSlide();
	}

	public void _OnBodyEnter(Node node) {
		if (node == this) return;
		GD.Print("Hit Body ", node);
		if (node is Enemy enemy) {
			enemy.Hit(this);
		}
		if (Velocity == Vector2.Zero) return;
		if (node is Player player) {
			player.Hit(this);
		}
		Velocity = Vector2.Zero;
		spin = 0;
		Reparent(node);
		var tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate", Colors.Transparent, 2);
		tween.TweenCallback(Callable.From(delegate {this.QueueFree();}));
		collider.Disabled = true;
	}

	public void SetFriendly(bool friendly) {
		this.Friendly = friendly;
		CollisionMask = (uint)(friendly ? 0b011 : 0b101);
		area.CollisionMask = (uint)(friendly ? 0b101 : 0b011);
	}

	public bool Hit(IDamageSource source) {
		var dir = source.Direction;
		Velocity = dir * Velocity.Length();
		spin = GD.Randi() % 2 * 100 - 50;
		SetFriendly(true);
		return true;
	}

	public override string ToString() {
		return "FishSpine";
	}
}
