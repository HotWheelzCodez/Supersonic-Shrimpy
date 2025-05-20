using Godot;
using System;

public partial class SpiderCrabLeg : Area2D, IDamageSource
{

	public float Damage => 1f;
	public Vector2 Direction => Vector2.Zero;
	public Vector2 Knockback => Direction;

	[Node("Sprite2D")]
	public Sprite2D sprite;
	[Node("Hit")]
	public AudioStreamPlayer2D hitSound;

	public void Attack() {
		var player = Game.instance.player;
		hitSound.Play();
		if (OverlapsBody(player)) {
			player.Hit(this);
		}
	}

	public override void _Ready() {
		((ShaderMaterial)sprite.Material).SetShaderParameter("origin", GlobalPosition);
		sprite.FlipH = GD.Randi() % 2 == 1;
	}

}
