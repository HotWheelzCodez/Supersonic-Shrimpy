using Godot;
using System;

public partial class Player : CharacterBody2D, IHittable
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

	[Node("Sprite")]
	public AnimatedSprite2D sprite;
	[Node("Rotations")]
	public AnimationPlayer rotations;
	[Node("RightClaw/Sprite")]
	public Claw rightClaw;
	[Node("LeftClaw/Sprite")]
	public Claw leftClaw;
	[Node("Attacks")]
	public Area2D attacks;
	[Node("PunchSound")]
	public AudioStreamPlayer2D punchSound;
	[Node("HurtSound")]
	public AudioStreamPlayer2D hurtSound;

	public static readonly string[] rotationAnims = new string[8] {
		"right", "right", "down", "left", "left", "left_up", "up", "right_up"
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

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
		if (movement.Length() > 0.1 && (leftClaw.punch == 0 && rightClaw.punch == 0
				|| Input.IsActionJustPressed("punch_left") || Input.IsActionJustPressed("punch_right"))) {
			var angle = movement.Angle();
			attacks.Rotation = angle;
			var step = (int)(angle / Mathf.Pi * 4 + 8.5) % 8;
			rotations.Play(rotationAnims[step]);
			facingAngle = angle;
		}

		MoveAndSlide();

		if (Input.IsActionJustPressed("punch_left")) {
			leftClaw.Punch();
		}
		if (Input.IsActionJustPressed("punch_right")) {
			rightClaw.Punch();
		}
	}

	public bool Hit(IDamageSource source) {
		if (invul > 0) return false;
		hurtSound.Play();
		Game.instance.Freeze(0.05f);
		Game.instance.Shake(3, 4);
		Health -= source.Damage;
		Velocity += source.Knockback;
		invul = 0.5f;
		return true;
	}

	public void Die() {
		Modulate = Colors.Red;
		GetTree().ChangeSceneToFile("res://Main.tscn");

	}
}
