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
	public float facingAngle = 0;

	public Claw[] claws;

	[Node("Sprite")]
	public AnimatedSprite2D sprite;
	[Node("Rotations")]
	public AnimationPlayer rotations;
	[Node("RightClaw/Sprite")]
	public AnimatedSprite2D rightClawSprite;
	[Node("LeftClaw/Sprite")]
	public AnimatedSprite2D leftClawSprite;
	[Node("Attacks")]
	public Area2D attacks;

	public static readonly string[] rotationAnims = new string[8] {
		"right", "right", "down", "left", "left", "left_up", "up", "right_up"
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		claws = new Claw[] {new Claw(this), new Claw(this)};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double doubelta) {
		var delta = (float)doubelta;
		var acc = acceleration;
		invul -= delta;

		var movement = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (Health == 0 || invul >= 0.25) {
			Velocity *= 0.95f;
			MoveAndSlide();
			return;
		}
		if (Velocity.Dot(movement) < 0) { acc *= 2; }
			Velocity = movement * speed;
		if (movement.Length() > 0.1 && (claws[0].punch == 0 && claws[1].punch == 0
				|| Input.IsActionJustPressed("punch_left") || Input.IsActionJustPressed("punch_right"))) {
			var angle = movement.Angle();
			attacks.Rotation = angle;
			var step = (int)(angle / Mathf.Pi * 4 + 8.5) % 8;
			GD.Print((int)(angle / Mathf.Pi * 4 + 8.5) % 8);
			rotations.Play(rotationAnims[step]);
			facingAngle = angle;
		}

		MoveAndSlide();

		if (Input.IsActionJustPressed("punch_left")) {
			claws[0].Punch();

		}
		if (Input.IsActionJustPressed("punch_right")) {
			claws[1].Punch();
		}

		foreach(var claw in claws) {
			claw.Update((float)delta);
		}

		rightClawSprite.Position = attacks.Transform.X * 12 * claws[0].punch;
		leftClawSprite.Position = attacks.Transform.X * 12 * claws[1].punch;

	}

	public bool Hit(Enemy source) {
		if (invul > 0) return false;
			Health -= source.damage;
			Velocity -= (source.Position - Position).Normalized() * 128;
			invul = 0.5f;
			return true;
		}

		public void Die() {
		Modulate = Colors.Red;

	}
}
