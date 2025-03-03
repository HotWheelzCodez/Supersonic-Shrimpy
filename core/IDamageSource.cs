using Godot;
using System;

public interface IDamageSource {
	public Vector2 Direction {get;}
	public float Damage {get;}
	public Vector2 Knockback {get;}
}
