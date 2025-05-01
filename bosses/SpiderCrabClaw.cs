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

	[Export]
	public SpiderCrabClaw other;
	[Export]
	public Vector2 anchor;

	public void PickAttack() {
		current = (GD.Randi() % 2) switch {
			0 => Attack.JabVertical,
			1 => Attack.Slash
		};
		GD.Print(current);
	}

	public override void _Process(double delta) {
		var player = Game.instance.player;
		var myDist = (player.GlobalPosition - GlobalPosition).Length();
		var otDist = (player.GlobalPosition - other.GlobalPosition).Length();
		var between = (GlobalPosition - other.GlobalPosition).Length();
		var anchorPos = GetParent<SpiderCrab>().GlobalPosition + anchor;
		sprite.GlobalPosition = GlobalPosition.Lerp(anchorPos + (GlobalPosition - anchorPos).Normalized() * 96, 1);
		sprite.Rotation = (GlobalPosition - anchorPos).Angle() - Mathf.Pi / 2;
		switch (current) {
			case Attack.JabVertical:
				timeInState += delta;
				if (progress < 1) {
					progress = Mathf.MoveToward(progress, 1, delta);
					sprite.GlobalPosition = GlobalPosition.Lerp(sprite.GlobalPosition, ((float)progress));
					Velocity = Vector2.Zero;
				} else if (progress > 1) {
					progress = Mathf.MoveToward(progress, 2, delta);
					if (progress < 1.5) {
						sprite.GlobalPosition = GlobalPosition.Lerp(sprite.GlobalPosition, (3 - (float)progress * 2));
					} else {
						sprite.GlobalPosition = GlobalPosition;
					}
					if (progress >= 2) {
						PickAttack();
						progress = 0;
						timeInState = 0;
					}
				} else if ((other.progress == 1 || other.progress >= 1.75) && (GlobalPosition - Game.instance.player.GlobalPosition).Length() < 8) {
					progress += delta;
				} else if (otDist < myDist && between < 64 && other.progress == 1) {
					Velocity = Velocity.MoveToward((GlobalPosition - other.GlobalPosition).Normalized() * 64, (float)delta * 64);
					MoveAndSlide();
				} else {
					Velocity = Velocity.MoveToward((Game.instance.player.GlobalPosition - GlobalPosition).Normalized() * 64, (float)delta * 64 * (1 + (float)timeInState));
					MoveAndSlide();
				}
				break;
			case Attack.Slash:
				if (progress == 0) {
					GlobalPosition = new(anchorPos.X, player.GlobalPosition.Y);
					progress += delta;
				}
				if (progress <= 1) {
					GlobalPosition = new (anchorPos.X - (float)progress * anchor.X * 2, GlobalPosition.Y);
					sprite.GlobalPosition = GlobalPosition.Lerp(sprite.GlobalPosition, Mathf.Pow((float)progress * 2 - 1f, 4));
					progress += delta;
				} else {
					PickAttack();
					timeInState = 0;
					progress = 0;
				}
				break;
			default:
				break;
		}
	}
}
