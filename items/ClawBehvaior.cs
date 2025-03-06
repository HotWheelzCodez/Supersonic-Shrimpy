using Godot;

public class ClawBehavior {

	public Claw myClaw;

	public ClawBehavior(Claw claw) {
		myClaw = claw;
	}

	public virtual bool Update(float delta) {return false;}

	public virtual bool PrePunch() {return false;}

	public virtual void Punch() {}

	public virtual void PunchThing(IHittable thing) {}

}
