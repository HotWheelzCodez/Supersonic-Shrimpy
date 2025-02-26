using Godot;
using System;

public partial class Crab : Enemy
{

	public enum State {
		Follow,
		Wander
	}

	public State state = State.Wander;

	public override void _PhysicsProcess(double doubelta)
	{
		base._PhysicsProcess(doubelta);

		var delta = (float)doubelta;
		var acc = acceleration;

		switch (state) {
			case State.Follow:
				if (hitDelay <= 0) {
					MoveTo(lastSeen, delta);
				} else {
					AccelerateTo(Vector2.Zero, delta);
				}
				if (!playerVisible && Position.DistanceTo(lastSeen) < 8) {
					state = State.Wander;
				}
				break;
			case State.Wander:
				if (playerVisible) {
					state = State.Follow;
				}
				break;
		}

		MoveAndSlide();

		if (hitDelay >= 0) {
			hitDelay = Mathf.Max(hitDelay - delta, 0);
			if (hitDelay <= 0 && player.Hit(this)) {
				anim.Travel("attack");
				hitDelay = 0.25f;
			}
		}
	}
}
