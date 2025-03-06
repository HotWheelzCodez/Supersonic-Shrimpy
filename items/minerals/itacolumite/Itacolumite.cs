using Godot;

public partial class Itacolumite : Mineral {

	public override bool Apply(Claw claw) {
		var area = new Area2D();
		var collider = new CollisionShape2D();
		var shape = new RectangleShape2D();
		shape.Size = new(8, 8);
		collider.Shape = shape;
		area.AddChild(collider);
		area.CollisionMask = 0b101;
		claw.AddChild(area);
		var behavior = new Behavior(claw);
		area.BodyEntered += behavior.OnClawHit;
		claw.behaviors.Add(behavior);
		return true;
	}

	public class Behavior : ClawBehavior {
		public Behavior(Claw claw) : base(claw) {}

		public bool active;
		public Vector2 pos;
		public Vector2 velocity;
		public bool left;
		public bool retract;

		public override bool PrePunch() {
			GD.Print("Natrolite");
			if (active) {
				retract = true;
				return true;
			}
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
			if (retract) {
				velocity = -myClaw.Position.Normalized() * 256;
			}
			pos += velocity * delta;
			myClaw.GlobalPosition = pos;
			if (myClaw.Position.Length() > 16) {
				left = true;
			}
			if ((left || retract) && myClaw.Position.Length() < 4) {
				GD.Print("Natrolite Deactive");
				myClaw.Position = Vector2.Zero;
				retract = false;
				active = false;
			}
			return true;
		}

		public void OnClawHit(Node node) {
			if (!active || retract) return;
			retract = true;
			if (node is IHittable hittable) {
				foreach (var behavior in myClaw.behaviors) {
					if (behavior != this && behavior.PrePunch()) return;
				}
				hittable.Hit(myClaw);
				foreach (var behavior in myClaw.behaviors) {
					behavior.PunchThing(hittable);
				}
				foreach (var behavior in myClaw.behaviors) {
					behavior.Punch();
				}
			}
		}
	}
}
