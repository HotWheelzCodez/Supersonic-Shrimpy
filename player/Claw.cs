using Godot;
using System;
using System.Collections.Generic;


public class Claw {

	public Player player;
	public List<dynamic> behaviors = new();
	public Stats stats = new();

	public float punch = 0;

	public Claw(Player player) {
		this.player = player;

	}

	public void Update(float delta) {
		punch = Mathf.MoveToward(punch, 0, delta / stats.cooldown);
		foreach (var behavior in behaviors) {
			behavior.Update(delta);
		}
	}

	public void Punch() {
		if (punch > 0) return;
		punch = 1;
		foreach (var behavior in behaviors) {
			behavior.Punch();
		}
		foreach (var body in ((Area2D)player.GetNode("Attacks")).GetOverlappingBodies()) {
			if (body is Enemy enemy) {
				enemy.Hit(this);
				player.Velocity -= player.Transform.X * stats.recoil;
				break;
			}
		}
	}

	public struct Stats {
		public float damage = 4;
		public float cooldown = 0.2f;
		public float knockback = 64;
		public float recoil = 128;

		public Stats() {}
	}
}
