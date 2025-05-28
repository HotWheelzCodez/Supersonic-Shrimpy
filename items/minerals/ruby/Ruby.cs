using Godot;

public partial class Ruby : Mineral {
	public override bool Apply(Claw claw) {
		claw.player.maxHealth += 1;
		claw.player.Health += 1;
		return true;
	}
}
