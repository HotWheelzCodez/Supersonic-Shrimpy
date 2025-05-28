using Godot;
using System;

[Tool]
public partial class SpiderCrabClaw : CharacterBody2D, IDamageSource, IHittable
{
	[Node("Sprite")]
	public AnimatedSprite2D sprite;
	[Node("CollisionShape2D/AttackBox")]
	public Area2D attack;
	[Node("Animations")]
	public AnimationPlayer anims;
	[Node("CollisionShape2D/Particles")]
	public GpuParticles2D particles;
	[Node("CollisionShape2D")]
	public CollisionShape2D col;
	[Node("../Hit")]
	public AudioStreamPlayer2D hitSound;
	[Node("../Hurt")]
	public AudioStreamPlayer2D hurtSound;

	public Vector2 Direction => (GetParent<SpiderCrab>().GlobalPosition + anchor).DirectionTo(GlobalPosition);
	public float Damage => 1;
	public Vector2 Knockback => 256 * Direction;

	[Export]
	public SpiderCrabClaw other;
	[Export]
	public Vector2 anchor;
	[Export]
	public float reach = 0;
	[Export]
	public float follow;
	[Export]
	public bool hurts;
	public double shakeDuration;

	public void PickAttack() {
		var list = anims.GetAnimationList();
		var i = GD.RandRange(0, list.Length-1);
		GD.Print("claw",i);
		anims.Play(list[i]);
	}

	public override void _Ready() {
		NodeAttribute.AssignNodes(this);
		anims.Play("desync");
	}

	public override void _Process(double delta) {
		var shake = new Vector2(GD.Randf(), GD.Randf()) * 8 - Vector2.One * 16 * (float)shakeDuration;
		if (0 >= (shakeDuration -= delta)) {
			shake = Vector2.Zero;
		}
		if (!Engine.IsEditorHint() && GetNode<SpiderCrab>("..").health > 0) {
			var parent = GetParent<SpiderCrab>();
			//anims.SpeedScale = 2 - (parent.health / parent.maxHealth);
			if (!anims.IsPlaying()) {
				PickAttack();
			}
			if (!Engine.IsEditorHint()) {
				var player = Game.instance.player;
				GlobalPosition = GlobalPosition.MoveToward(player.GlobalPosition, follow * (float)delta);
			}
		}
		var anchorPos = GetParent<Node2D>().GlobalPosition + anchor;
		sprite.GlobalPosition = (col.GlobalPosition + shake).Lerp(anchorPos + (GlobalPosition - anchorPos).Normalized() * 128, 1 - reach);
		sprite.Rotation = (GlobalPosition - anchorPos).Angle() - Mathf.Pi / 2;
		((ShaderMaterial)sprite.Material).SetShaderParameter("origin", GlobalPosition);
	}

	public void Attack() {
		var player = Game.instance.player;
		Game.instance.Shake(5, 5);
		hitSound.Play();
		if (attack.OverlapsBody(player)) {
			player.Hit(this);
		} else {
			col.Disabled = false;
		}
		particles.Emitting = true;
	}

	public bool Hit(IDamageSource source) {
		shakeDuration = 0.25;
		GetParent<SpiderCrab>().Damage(source.Damage);
		hurtSound.Play();
		return true;
	}

	public void _OnHitBody(Node2D node) {
		GD.Print("Claw hit ", node);
		if (hurts && node == Game.instance.player) {
			hitSound.Play();
			Game.instance.player.Hit(this);
		}
	}
}
