using Godot;
using System;

[Tool]
public partial class LerpContainer : Container
{
	[Export]
	public float speed;

	public override void _Process(double delta) {
		var node = GetChild<Control>(0);
		CustomMinimumSize = CustomMinimumSize.Max(node.GetCombinedMinimumSize());
		node.TopLevel = true;
		var d = 1/Mathf.Exp((float)delta * speed);
		node.GlobalPosition = node.GlobalPosition.Lerp(GlobalPosition, 1-d);
		node.Size = node.Size.Lerp(Size, 1-d);
	}
}
