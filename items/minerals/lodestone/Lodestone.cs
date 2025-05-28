using Godot;

public partial class Lodestone : Mineral {

	public override bool Apply(Claw claw) {
		if (Game.instance.map.poi) {
			return false;
		}
		Game.instance.map.poi = true;
		return true;
	}
}
