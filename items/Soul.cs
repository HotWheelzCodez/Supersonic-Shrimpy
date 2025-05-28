using Godot;
using System;

public partial class Soul : Pickup {

	[Node("AnimationPlayer")]
	public AnimationPlayer anim;

	public override bool Grab() {
		if (anim.CurrentAnimationPosition < 0.25) return false;
		Game.instance.player.soul += 1;
		return true;
	}
}
