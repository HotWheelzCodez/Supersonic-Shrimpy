using Godot;
using System;

public partial class SpiderCrabClaw : CharacterBody2D, IDamageSource, IHittable
{
	public enum Attack {
		JabVertical,
		JabAngled,
		Slash
	}

	public Attack current = Attack.JabVertical;
	[Export]
	public double progress = 0;
	public double timeInState = 0;

	[Node("Sprite")]
	public AnimatedSprite2D sprite;
	[Node("AnimationTree")]
	public AnimationTree tree;
	public AnimationNodeStateMachinePlayback anims;
	[Node("AttackBox")]
	public Area2D attack;
	[Node("Particles")]
	public GpuParticles2D particles;
	[Node("CollisionShape2D")]
	public CollisionShape2D col;

	public Vector2 Direction => (GetParent<SpiderCrab>().GlobalPosition + anchor).DirectionTo(GlobalPosition);
	public float Damage => 1;
	public Vector2 Knockback => 256 * Direction;

	[Export]
	public SpiderCrabClaw other;
	[Export]
	public Vector2 anchor;
	[Export]
	public float reach = 0;
	public double shakeDuration;

	public override void _Ready() {
		anims = (AnimationNodeStateMachinePlayback)tree.Get("parameters/playback");
		PickAttack();
		sprite.PlayBackwards();
	}

	public void PickAttack() {
		current = (GD.Randi() % 1) switch {
			0 => Attack.JabVertical,
			1 => Attack.Slash
		};
		switch (current) {
			case Attack.JabVertical:
				anims.Travel("hide");
				break;
		}
	}

	public override void _Process(double delta) {
		var player = Game.instance.player;
		var shake = new Vector2(GD.Randf(), GD.Randf()) * 8 - Vector2.One * 16 * (float)shakeDuration;
		if (0 >= (shakeDuration -= delta)) {
			shake = Vector2.Zero;
		}
		switch (current) {
			case Attack.JabVertical:
				if (progress < 2) {
					GlobalPosition = GlobalPosition.MoveToward(player.GlobalPosition, 128 * (float)delta);
					if (progress + delta > 2) {
						anims.Travel("lock");
						Velocity = player.Velocity;
					}
				} else if (progress < 2.6) {
					Velocity = Velocity.MoveToward(player.Velocity, 1024 * (float)delta);
					MoveAndSlide();
				} else if (progress < 2.8) {
					sprite.Play("default");
					reach = (((float)progress - 2.6f) * 5);
					anims.Travel("hide");
					if (progress + delta >= 2.8) {
						reach = 1;
						Game.instance.Shake(5, 5);
						if (attack.OverlapsBody(player)) {
							player.Hit(this);
						} else {
							col.Disabled = false;
						}
						particles.Emitting = true;
					}
				} else if (4 < progress && progress < 5) {
					sprite.Play("open");
					reach = (5 - (float)progress);
					col.Disabled = true;
				} else if (5 <= progress){
					progress -= 5.0;
					PickAttack();
					break;
	 			}
				progress += delta;
				break;
			case Attack.Slash:
				break;
			default:
				break;
		}
		var anchorPos = GetParent<SpiderCrab>().GlobalPosition + anchor;
		sprite.GlobalPosition = (GlobalPosition + shake).Lerp(anchorPos + (GlobalPosition - anchorPos).Normalized() * 128, 1 - reach);
		sprite.Rotation = (GlobalPosition - anchorPos).Angle() - Mathf.Pi / 2;
		((ShaderMaterial)sprite.Material).SetShaderParameter("origin", GlobalPosition);
	}

	public bool Hit(IDamageSource source) {
		shakeDuration = 0.25;
		return true;
	}
}
