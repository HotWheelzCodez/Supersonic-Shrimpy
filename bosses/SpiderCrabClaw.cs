using Godot;
using System;

public partial class SpiderCrabClaw : CharacterBody2D
{
	public enum Attack {
		JabVertical,
		JabAngled,
		Slash
	}

	public Attack current = Attack.JabVertical;
	public double progress = 0;
	public double timeInState = 0;

	[Node("Sprite")]
	public AnimatedSprite2D sprite;
	[Node("AnimationTree")]
	public AnimationTree tree;
	public AnimationNodeStateMachinePlayback anims;

	[Export]
	public SpiderCrabClaw other;
	[Export]
	public Vector2 anchor;
	[Export]
	public float reach = 0;

	public override void _Ready() {
		anims = (AnimationNodeStateMachinePlayback)tree.Get("parameters/playback");
		PickAttack();
	}

	public void PickAttack() {
		current = (GD.Randi() % 1) switch {
			0 => Attack.JabVertical,
			1 => Attack.Slash
		};
		switch (current) {
			case Attack.JabVertical:
				anims.Travel("idle");
				break;
		}
		progress = 0;
	}

	public override void _Process(double delta) {
		var player = Game.instance.player;
		switch (current) {
			case Attack.JabVertical:
				if (progress < 1.5) {
					GlobalPosition = player.GlobalPosition;
					if (progress + delta > 1.5) {
						anims.Travel("lock");
					}
				} else if (2.6 < progress && progress < 2.8) {
					reach = (((float)progress - 2.8f) * 5);
					anims.Travel("hide");
					if (progress + delta > 2.5) {
						reach = 1;
					}
				} else if (4 < progress && progress < 5) {
					reach = (5 - (float)progress);
				} else if (5 < progress){
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
		sprite.GlobalPosition = GlobalPosition.Lerp(anchorPos + (GlobalPosition - anchorPos).Normalized() * 96, 1 - reach);
		sprite.Rotation = (GlobalPosition - anchorPos).Angle() - Mathf.Pi / 2;
	}
}
