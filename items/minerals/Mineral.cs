using Godot;
using System;

public abstract partial class Mineral : CharacterBody2D, IHittable {

	public abstract bool Apply(Claw claw);

	public bool hit = false;
	public Vector2 target;
	private float t = 0;

	[Node("Icon")]
	public Node2D sprite;

	public bool Hit(IDamageSource source) {
		if (hit) return false;
		GD.Print("Hit Mineral");
		Velocity += source.Knockback;
		if (source is Claw claw && Apply(claw)) {
			hit = true;
			return true;
		}
		return false;
	}

	public override void _Ready() {
		target = GlobalPosition;
		SetInstanceShaderParameter("alpha", 1);
	}

	public override void _PhysicsProcess(double delta) {
		if (hit) {
			t += (float)delta;
			sprite.SetInstanceShaderParameter("dist", t / 4);
			sprite.SetInstanceShaderParameter("alpha", 1-t);
			if (t > 1) {
				QueueFree();
			}
		} else {
			Velocity += (target - GlobalPosition) * 50f * (float)delta;
		}
		Velocity *= 0.97f;
		MoveAndSlide();
	}

}
