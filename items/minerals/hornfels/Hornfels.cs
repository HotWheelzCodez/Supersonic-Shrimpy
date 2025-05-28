using Godot;
using System;

public partial class Hornfels : Mineral
{
	public override bool Apply(Claw claw) {
		var p = claw.player;
		p.rightClaw.stats.damage += 2;
		p.rightClaw.stats.cooldown += 0.05f;
		p.leftClaw.stats.damage += 2;
		p.leftClaw.stats.cooldown += 0.05f;
		return true;
	}
}
