using Godot;

public partial class Itacolumite : Mineral {

	public override bool Apply(Claw claw) {
		claw.behaviors.Add(new Behavior(claw));
		return true;
	}

	public class Behavior : ClawBehavior {
		public Behavior(Claw claw) : base(claw) {}

		public bool active;
		public Vector2 pos;
		public Vector2 velocity;
		public bool left;

		public override bool PrePunch() {
			GD.Print("Natrolite");
			if (active) return true;
			GD.Print("Natrolite active");
			active = true;
			left = false;
			pos = myClaw.GlobalPosition;
			velocity = myClaw.Direction * 256;
			return true;
		}

		public override bool Update(float delta) {
			if (!active) return false;
			velocity -= myClaw.Position * delta * 16;
			velocity *= 0.98f;
			pos += velocity * delta;
			myClaw.GlobalPosition = pos;
			if (myClaw.Position.Length() > 16) {
				left = true;
			}
			if (left && myClaw.Position.Length() < 4) {
				GD.Print("Natrolite Deactive");
				myClaw.Position = Vector2.Zero;
				active = false;
			}
			return true;
		}
	}
}
