using Godot;
using System;

public abstract partial class Boss : Node2D {
	public float health;
	[Export]
	public float maxHealth;
	[Export]
	public int score;

	public override void _Ready() {
		health = maxHealth;
	}

	public void Damage(float amount) {
		if (health > amount) {
			health -= amount;
		} else if (health > 0) {
			health = 0;
			Game.instance.Score += score;
			Die();
		}
	}

	public abstract void Die();
}
