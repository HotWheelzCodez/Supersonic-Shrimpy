using Godot;
using System;

public partial class Coin : Pickup {

	public override bool Grab() {
		if (anim.CurrentAnimationPosition < 0.25 ) return false;
		Game.instance.Money += 1;
		Game.instance.Score += 20;
		return true;
	}
}
