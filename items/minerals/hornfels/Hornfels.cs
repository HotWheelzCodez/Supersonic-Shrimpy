using Godot;
using System;

public partial class Hornfels : Mineral
{
	public override bool Apply(Claw claw) {
		claw.stats.damage += 2;
		claw.stats.cooldown += 0.05f;
		return true;
	}
}
