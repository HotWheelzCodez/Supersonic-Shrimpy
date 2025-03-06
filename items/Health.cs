using Godot;
using System;

public partial class Health : Pickup {

	[Node("AnimationPlayer")]
	public AnimationPlayer anim;

	public override bool Grab() {
		if (anim.CurrentAnimationPosition < 0.25 || Game.instance.player.Health >= Game.instance.player.maxHealth) return false;
		Game.instance.player.Health += 1;
		return true;
	}
}
