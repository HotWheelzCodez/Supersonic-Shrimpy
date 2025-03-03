using Godot;
using System;

public abstract partial class Mineral : CharacterBody2D, IHittable {

	public abstract bool Apply(Claw claw);

	public bool Hit(IDamageSource source) {
		GD.Print("Hit Mineral");
		if (source is Claw claw && Apply(claw)) {
			return true;
		}
		Velocity += source.Knockback;
		return false;
	}

}
