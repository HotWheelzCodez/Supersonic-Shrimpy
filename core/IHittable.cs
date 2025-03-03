using Godot;
using System;

public interface IHittable {
	public bool Hit(IDamageSource source);
}
