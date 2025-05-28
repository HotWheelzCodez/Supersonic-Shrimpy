using Godot;
using System;

public partial class Sapphire : Mineral
{
	public override bool Apply(Claw claw) {
		var p = claw.player;
		p.leftClaw.behaviors.Add(new Behavior(claw));
		p.rightClaw.behaviors.Add(new Behavior(claw));
		return true;
	}

	public class Behavior : ClawBehavior {
		public Behavior(Claw claw) : base(claw) {}

		public override void PunchThing(IHittable thing) {
			if (thing is CharacterBody2D enemy) {
				GD.Print("Soul");
				if (GD.Randf() < 0.05f) {
					var node = GD.Load<PackedScene>("res://items/soul.tscn").Instantiate<Node2D>();
					node.Position = enemy.Position;
					enemy.GetParent().AddChild(node);
				}
			}
		}
	}
}
