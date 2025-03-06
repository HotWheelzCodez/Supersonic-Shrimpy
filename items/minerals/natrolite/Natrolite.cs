using Godot;
using System;

public partial class Natrolite : Mineral {
	public override bool Apply(Claw claw) {
		claw.behaviors.Add(new Behavior(claw));
		return true;
	}

	public class Behavior : ClawBehavior {
		public Behavior(Claw claw) : base(claw) {}

		public Enemy hooked;

		public override void PunchThing(IHittable thing) {
			if (thing is Enemy enemy) {
				hooked = enemy;
				enemy.OnHit += Release;
			}
		}

		public void Release(object enemy, EventArgs e) {
			hooked = null;
		}

		public override bool Update(float delta) {
			if (hooked != null) {
				if (hooked.Health <= 0) {
					hooked = null;
					return false;
				}
				hooked.GlobalPosition = myClaw.GlobalPosition;
			}
			return false;
		}
	}
}
