using Godot;
using System;

public partial class SpiderCrab : CharacterBody2D
{
	public PackedScene leg;

	public override void _Ready() {
		leg = GD.Load<PackedScene>("res://bosses/spidercrab/spider_crab_leg.tscn");
	}

	public void _OnStompTimer() {
		var player = Game.instance.player;
		var vel = player.Velocity.Rotated(Mathf.Pow(GD.Randf() * 2 - 1, 2) * Mathf.Pi) * GD.Randf();
		var node = (SpiderCrabLeg)leg.Instantiate();
		node.Position = player.GlobalPosition + vel * 0.5f - GlobalPosition;
		AddChild(node);
	}
}
