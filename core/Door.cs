using Godot;
using System;

public partial class Door : StaticBody2D
{
	[Node("..")]
	public Room room;
	[Node("CollisionShape2D")]
	public CollisionShape2D col;
	[Node("Icon")]
	public AnimatedSprite2D icon;

	public override void _Process(double delta) {
		bool playerNear = GlobalPosition.DistanceTo(Game.instance.player.GlobalPosition) < 24;
		if (col.Disabled && playerNear) {
			icon.Animation = "opened";
			return;
		}
		foreach (var node in room.GetChildren()) {
			if (node is Enemy || node is Boss bs && bs.health > 0) {
				icon.Animation = "urchin";
				col.Disabled = false;
				return;
			}
		}
		col.Disabled = true;
		icon.Animation = playerNear ? "opened" : "closed";
	}
}
