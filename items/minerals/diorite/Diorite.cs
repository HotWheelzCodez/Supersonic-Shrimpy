using Godot;

public partial class Diorite : Mineral {

	public override bool Apply(Claw claw) {
		if (Game.instance.map.map) {
			return false;
		}
		Game.instance.map.map = true;
		return true;
	}
}
