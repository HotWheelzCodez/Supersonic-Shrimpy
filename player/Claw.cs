using Godot;
using System;
using System.Collections.Generic;


public partial class Claw : AnimatedSprite2D, IDamageSource {

	public List<ClawBehavior> behaviors = new();
	public Stats stats = new();

	public float punch = 0;

	public float Damage => stats.damage;
	public Vector2 Knockback => stats.knockback * Direction + player.Velocity;
	public Vector2 Direction => player.attacks.Transform.X;

	[Node("../..")]
	public Player player;

	public override void _PhysicsProcess(double delta) {
		foreach (var behavior in behaviors) {
			if (behavior.Update((float)delta)) {
				return;
			}
		}
		punch = Mathf.MoveToward(punch, 0, (float)delta / stats.cooldown);
		Position = Direction * 12 * punch;
		Scale = Vector2.One * (Mathf.Log(Damage / 4) + 1);
	}

	public void Punch() {
		if (punch > 0) return;
		foreach (var behavior in behaviors) {
			if (behavior.PrePunch()) {
				return;
			}
		}
		player.punchSound.Play();
		punch = 1;
		foreach (var body in player.attacks.GetOverlappingBodies()) {
			GD.Print("Intersecting ", body);
			if (body is IHittable hittableThing) {
				hittableThing.Hit(this);
				player.Velocity -= Direction * stats.recoil;
				break;
			}
		}
		foreach (var behavior in behaviors) {
			behavior.Punch();
		}

		Game.instance.Shockwave(player.attacks.GlobalTransform * new Vector2(16, 0));
	}

	public struct Stats {
		public float damage = 4;
		public float cooldown = 0.2f;
		public float knockback = 64;
		public float recoil = 256;

		public Stats() {}
	}
}
