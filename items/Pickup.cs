using Godot;

public abstract partial class Pickup : CharacterBody2D {

	[Node("AnimationPlayer")]
	public AnimationPlayer anim;

	private bool pickedUp = false;

	public abstract bool Grab();

	public override void _PhysicsProcess(double delta) {
		if (pickedUp) return;
		var dif = Game.instance.player.GlobalPosition - GlobalPosition;
		var distSq = dif.LengthSquared();
		if (distSq < 256) {
			if (Grab()) {
				anim?.Play("pickup");
				pickedUp = true;
			}
		}
		Velocity *= 0.95f;
		MoveAndSlide();
	}
}
