using Godot;
using System;

public abstract partial class Boss : Node2D {
	public float health;
	public float maxHealth;

	public void Damage(float amount) {
		if (health > amount) {
			health -= amount;
		} else {
			health = 0;
			Die();
		}
	}

	public abstract void Die();
}
